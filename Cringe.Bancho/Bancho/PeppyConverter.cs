using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Cringe.Utils;

namespace Cringe.Bancho.Bancho;

[AttributeUsage(AttributeTargets.Property)]
public class Secret : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class PeppyField : Attribute
{
}

[AttributeUsage(AttributeTargets.Property)]
public class EnumTypeAttribute : Attribute
{
    public Type Type { get; }

    public EnumTypeAttribute(Type type)
    {
        Type = type;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class DependentSized : Attribute
{
}

public interface IDependant
{
    public int? Dependency(string propertyName);
}

[AttributeUsage(AttributeTargets.Property)]
public class ConstantSized : Attribute
{
    public int Size { get; }

    public ConstantSized(int size)
    {
        Size = size;
    }
}

public abstract class PeppyRawPayload : RequestPacket
{
    public byte[] Payload { get; set; }
}

public static class PeppyConverter
{
    private static readonly Type _payloadType = typeof(PeppyRawPayload);

    private static int GetArrayLength(MemberInfo propertyInfo, BinaryReader reader, object buildingObject)
    {
        var isSized = propertyInfo.GetCustomAttribute<ConstantSized>();
        if (isSized is not null)
        {
            return isSized.Size;
        }

        var isDependant = propertyInfo.GetCustomAttribute<DependentSized>();
        if (isDependant is null)
        {
            return reader.ReadInt16();
        }

        return ((IDependant) buildingObject).Dependency(propertyInfo.Name) ?? reader.ReadInt16();
    }

    private static byte[] ConvertString(string s)
    {
        using var stream = new MemoryStream();
        stream.WriteByte(0x0B);
        stream.WriteLEB128Unsigned((ulong) Encoding.Latin1.GetByteCount(s));
        stream.Write(Encoding.Latin1.GetBytes(s));

        return stream.ToArray();
    }

    private static byte[] WritePrimitive(Type type, object obj, bool isSecret = false)
    {
        if (obj is bool x1)
            return new[] { (byte) (x1 ? 1 : 0) };

        if (obj is string s)
        {
            if (!isSecret) return ConvertString(s);

            return string.IsNullOrEmpty(s) ? new byte[] { 0 } : ConvertString("");
        }

        if (type == typeof(int))
            return BitConverter.GetBytes((int) obj);

        if (type == typeof(byte))
            return BitConverter.GetBytes((byte) obj);

        if (type == typeof(long))
            return BitConverter.GetBytes((long) obj);

        if (type == typeof(short))
            return BitConverter.GetBytes((short) obj);

        return Array.Empty<byte>();
    }

    private static object ReadPrimitive(Type type, BinaryReader reader)
    {
        if (type == typeof(bool))
        {
            return reader.ReadBoolean();
        }

        if (type == typeof(byte))
        {
            return reader.ReadByte();
        }

        if (type == typeof(short))
        {
            return reader.ReadInt16();
        }

        if (type == typeof(int))
        {
            return reader.ReadInt32();
        }

        if (type == typeof(string))
        {
            reader.ReadByte();
            var len = (int) reader.ReadLEB128Unsigned();
            var buffer = new byte[len];
            var _ = reader.Read(buffer, 0, len);

            return Encoding.Latin1.GetString(buffer);
        }

        throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported deserialization value");
    }

    private static object Deserialize(PropertyInfo propertyInfo, BinaryReader reader, object buildingObject)
    {
        var declaringType = propertyInfo.PropertyType;
        if (declaringType.IsPrimitive || declaringType == typeof(string))
        {
            return ReadPrimitive(declaringType, reader);
        }

        if (declaringType.IsEnum)
        {
            var underlyingType = propertyInfo.GetCustomAttribute<EnumTypeAttribute>()!.Type;
            var value = ReadPrimitive(underlyingType, reader);

            return Enum.ToObject(declaringType, value);
        }

        if (declaringType.IsArray)
        {
            var length = GetArrayLength(propertyInfo, reader, buildingObject);
            var buffer = new object[length];
            var elementType = declaringType.GetElementType()!;
            for (var i = 0; i < length; i++) buffer[i] = ReadPrimitive(elementType, reader);

            var destinationArray = Array.CreateInstance(elementType, length);
            Array.Copy(buffer, destinationArray, length);

            return destinationArray;
        }

        return Deserialize(declaringType, reader);
    }

    private static object Deserialize(Type type, BinaryReader reader)
    {
        var obj = Activator.CreateInstance(type);
        var properties = type
            .GetProperties(BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute(typeof(PeppyField)) is not null);

        foreach (var property in properties)
        {
            var value = Deserialize(property, reader, obj);

            property.SetValue(obj, value);
        }

        return obj;
    }

    public static byte[] Serialize(object obj, bool secretContext = false)
    {
        var properties = obj.GetType()
            .GetProperties(BindingFlags.Instance)
            .Where(x => x.GetCustomAttribute(typeof(PeppyField)) is not null);

        using var stream = new MemoryStream();

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            var type = property.PropertyType;

            if (type.IsEnum)
            {
                var enumType = property.GetCustomAttribute<EnumTypeAttribute>();
                stream.Write(WritePrimitive(enumType!.Type, value));

                continue;
            }

            if (type.IsArray)
            {
                var array = value as Array;
                var underlyingType = type.GetElementType();
                var predefinedSize = property.GetCustomAttribute<DependentSized>()?.TypeId ??
                                     property.GetCustomAttribute<ConstantSized>()?.TypeId;
                if (predefinedSize is null)
                {
                    stream.Write(BitConverter.GetBytes((uint) array!.Length));
                }

                foreach (var elem in array!)
                {
                    stream.Write(WritePrimitive(underlyingType, elem));
                }

                continue;
            }

            if (type.IsPrimitive || type == typeof(string))
            {
                var isSecret = secretContext && property.GetCustomAttribute<Secret>() is not null;
                stream.Write(WritePrimitive(type, value, isSecret));

                continue;
            }

            stream.Write(Serialize(value));
        }

        return stream.ToArray();
    }

    public static T Deserialize<T>(byte[] bytes) where T : class
    {
        return Deserialize(typeof(T), bytes) as T;
    }

    public static object Deserialize(Type type, byte[] bytes)
    {
        if (type.BaseType == _payloadType)
        {
            var obj = Activator.CreateInstance(type);
            var property = type.GetProperty(nameof(PeppyRawPayload.Payload));
            property!.SetValue(obj, bytes);

            return obj;
        }

        using var binaryReader = new BinaryReader(new MemoryStream(bytes));

        return Deserialize(type, binaryReader);
    }
}

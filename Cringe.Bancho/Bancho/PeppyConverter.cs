using System;
using System.IO;
using System.Reflection;
using System.Text;
using Cringe.Utils;

namespace Cringe.Bancho.Bancho;

public static class PeppyConverter
{
    private static object Convert(Type declaringType, BinaryReader reader)
    {
        if (declaringType == typeof(string))
        {
            reader.ReadByte();
            var len = (int) reader.ReadLEB128Unsigned();
            var buffer = new byte[len];
            var _ = reader.Read(buffer, 0, len);

            return Encoding.Latin1.GetString(buffer);
        }

        if (declaringType == typeof(int))
        {
            return reader.ReadInt32();
        }

        if (declaringType == typeof(int[]))
        {
            var length = reader.ReadInt16();
            var buffer = new int[length];
            for (var i = 0; i < length; i++) buffer[i] = reader.ReadInt32();

            return buffer;
        }

        throw new ArgumentOutOfRangeException(nameof(declaringType), declaringType,
            "Unsupported deserialization value");
    }

    private static object Deserialize(Type type, BinaryReader reader)
    {
        var obj = Activator.CreateInstance(type);
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            var value = Convert(propertyType, reader);
            property.SetValue(obj, value);
        }

        return obj;
    }

    public static T Deserialize<T>(byte[] bytes) where T : RequestPacket
    {
        using var binaryReader = new BinaryReader(new MemoryStream(bytes));

        return Deserialize(typeof(T), binaryReader) as T;
    }
}

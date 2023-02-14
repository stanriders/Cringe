using System;
using System.Linq;

namespace Cringe.Bancho.Tests;

public static class Helpers
{
    public static byte[] FromByteString(string str, bool trim7Bytes)
    {
        var x = str.Split(" ").Select(x => Convert.ToByte(x, 16));

        return trim7Bytes ? x.Skip(7).ToArray() : x.ToArray();
    }
}

﻿namespace Spice86.Emulator.Gdb;

using Spice86.Utils;

public class GdbFormatter
{
    public string FormatValueAsHex32(int value)
    {
        return $"{ConvertUtils.Swap32(value):x8}";
    }

    public string FormatValueAsHex8(int value)
    {
        return $"{ConvertUtils.Uint8(value):x2}";
    }
}
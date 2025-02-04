﻿namespace Spice86.Emulator.Cpu;

using Spice86.Emulator.Errors;
using Spice86.Emulator.Machine;
using Spice86.Utils;

using System;

[Serializable]
public class InvalidGroupIndexException : InvalidVMOperationException
{
    public InvalidGroupIndexException(Machine machine, int groupIndex) : base(machine, $"Invalid group index {ConvertUtils.ToHex(groupIndex)}")
    {
    }
}
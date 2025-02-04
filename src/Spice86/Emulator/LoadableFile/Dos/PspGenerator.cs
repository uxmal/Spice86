﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spice86.Emulator.Loadablefile.Dos;

using Spice86.Emulator.InterruptHandlers.Dos;
using Spice86.Emulator.Machine;
using Spice86.Emulator.Memory;
using Spice86.Utils;

public class PspGenerator
{
    private static readonly int LAST_FREE_SEGMENT_OFFSET = 0x02;
    private static readonly int DTA_OR_COMMAND_LINE_OFFSET = 0x80;
    private readonly Machine machine;
    public PspGenerator(Machine machine)
    {
        this.machine = machine;
    }

    public virtual void GeneratePsp(int pspSegment, string arguments)
    {
        Memory memory = machine.GetMemory();
        int pspAddress = MemoryUtils.ToPhysicalAddress(pspSegment, 0);

        // https://en.wikipedia.org/wiki/Program_Segment_Prefix
        memory.SetUint16(pspAddress, 0xCD20); // INT20h

        // last free segment, dosbox seems to put it just before VRAM.
        ushort lastFreeSegment = MemoryMap.GraphicVideoMemorySegment - 1;
        memory.SetUint16(pspAddress + LAST_FREE_SEGMENT_OFFSET, lastFreeSegment);
        memory.LoadData(pspAddress + DTA_OR_COMMAND_LINE_OFFSET, ArgumentsToDosBytes(arguments));
        DosInt21Handler dosFunctionDispatcher = machine.GetDosInt21Handler();
        dosFunctionDispatcher.GetDosMemoryManager().Init(pspSegment, lastFreeSegment);
        dosFunctionDispatcher.GetDosFileManager().SetDiskTransferAreaAddress(pspSegment, DTA_OR_COMMAND_LINE_OFFSET);
    }

    private byte[] ArgumentsToDosBytes(string arguments)
    {
        byte[] res = new byte[128];
        string correctLengthArguments = "";
        if (string.IsNullOrWhiteSpace(arguments) == false)
        {

            // Cut strings longer than 127 chrs
            correctLengthArguments = arguments.Length > 127 ? arguments[..127] : arguments;
        }


        // Command line size
        res[0] = ConvertUtils.Uint8b(correctLengthArguments.Length);

        // Copy actual characters
        int index = 0;
        for (; index < correctLengthArguments.Length; index++)
        {
            var str = correctLengthArguments[index];
            var chr = Encoding.ASCII.GetBytes(str.ToString())[0];
            res[index + 1] = chr;
        }

        res[index + 1] = 0x0D;
        return res;
    }
}

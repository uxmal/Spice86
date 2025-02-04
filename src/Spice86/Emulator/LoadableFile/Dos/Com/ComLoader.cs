﻿namespace Spice86.Emulator.Loadablefile.Dos.Com;

using Spice86.Emulator.Machine;
using Spice86.Emulator.Memory;
using Spice86.Emulator.LoadableFile;

public class ComLoader : ExecutableFileLoader
{
    private const int ComOffset = 0x100;
    private readonly int _startSegment;
    public ComLoader(Machine machine, int startSegment) : base(machine)
    {
        this._startSegment = startSegment;
    }

    public override byte[] LoadFile(string file, string arguments)
    {
        new PspGenerator(_machine).GeneratePsp(_startSegment, arguments);
        byte[] com = this.ReadFile(file);
        int physicalStartAddress = MemoryUtils.ToPhysicalAddress(_startSegment, ComOffset);
        _memory.LoadData(physicalStartAddress, com);
        var state = _cpu.GetState();

        // Make DS and ES point to the PSP
        state.SetDS(_startSegment);
        state.SetES(_startSegment);
        SetEntryPoint(_startSegment, ComOffset);
        return com;
    }
}

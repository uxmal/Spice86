﻿namespace Spice86.Emulator.Memory;

/// <summary> Informations about memory mapping of an IBM PC </summary>
public static class MemoryMap
{
    public const int BiosDataAreaLength = 256;

    // This is where the port to get VGA CRT status is stored
    public const int BiosDataAreaOffsetCrtIoPort = 0x63;

    // Counter incremented 18.2 times per second
    public const int BiosDataAreaOffsetTickCounter = 0x6C;

    public const int BiosDataAreaSegment = 0x40;

    public const int BootSectorCodeLength = 512;

    public const int BootSectorCodeSegment = 0x07C0;

    public const int ColorTextVideoMemoryLength = 32767;

    public const int ColorTextVideoMemorySegment = 0xB800;

    public const int FreeMemoryStartSegment = 0x50;

    public const int GraphicVideoMemorylength = 65535;

    public const int GraphicVideoMemorySegment = 0xA000;

    public const int InterruptVectorLength = 1024;

    public const int InterruptVectorSegment = 0x0;

    public const int MonochromeTextVideoMemoryLength = 32767;

    public const int MonochromeTextVideoMemorySegment = 0xB000;
}
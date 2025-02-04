﻿namespace Spice86.Emulator.Devices.Video;

using Spice86.Emulator.Machine;

/// <summary>
/// VGA Digital Analog Converter Implementation.
/// </summary>
public class VgaDac
{
    private const int Redindex = 0;
    private const int BlueIndex = 2;
    private const int GreenIndex = 1;
    public const int VgaDacnotInitialized = 0;
    public const int VgaDacRead = 1;
    public const int VgaDacWrite = 2;
    private readonly Machine _machine;
    private int _state = 1;
    private int _colour; /* 0 = red, 1 = green, 2 = blue */
    private int _readIndex;
    private int _writeIndex;
    private readonly Rgb[] _rgbs = new Rgb[256];
    public VgaDac(Machine machine)
    {
        _machine = machine;

        // Initial VGA default palette initialization
        for (int i = 0; i < _rgbs.Length; i++)
        {
            Rgb rgb = new();
            rgb.SetR((((i >> 5) & 0x7) * 255 / 7));
            rgb.SetG((((i >> 2) & 0x7) * 255 / 7));
            rgb.SetB(((i & 0x3) * 255 / 3));
            _rgbs[i] = rgb;
        }
    }

    public static int From8bitTo6bitColor(int color8bit) => (byte)((uint)color8bit >> 2);

    public static int From6bitColorTo8bit(int color6bit) => (byte)((color6bit & 0b111111) << 2);

    public virtual void WriteColor(int colorValue)
    {
        Rgb rgb = _rgbs[_writeIndex];
        if (_colour == Redindex)
        {
            rgb.SetR(colorValue);
        }
        else if (_colour == GreenIndex)
        {
            rgb.SetG(colorValue);
        }
        else if (_colour == BlueIndex)
        {
            rgb.SetB(colorValue);
        }
        else
        {
            throw new InvalidColorIndexException(_machine, _colour);
        }

        _colour = (_colour + 1) % 3;
        if (_colour == 0)
        {
            _writeIndex++;
        }
    }

    public int ReadColor()
    {
        Rgb rgb = _rgbs[_readIndex];
        int value = _colour switch
        {
            Redindex => rgb.GetR(),
            GreenIndex => rgb.GetG(),
            BlueIndex => rgb.GetB(),
            _ => throw new InvalidColorIndexException(_machine, _colour)
        };
        _colour = (_colour + 1) % 3;
        if (_colour == 0)
        {
            _writeIndex++;
        }
        return value;
    }

    public virtual int GetState()
    {
        return _state;
    }

    public virtual void SetState(int state)
    {
        this._state = state;
    }

    public virtual int GetColour()
    {
        return _colour;
    }

    public virtual void SetColour(int colour)
    {
        this._colour = colour;
    }

    public virtual int GetReadIndex()
    {
        return _readIndex;
    }

    public virtual void SetReadIndex(int readIndex)
    {
        this._readIndex = readIndex;
    }

    public virtual int GetWriteIndex()
    {
        return _writeIndex;
    }

    public virtual void SetWriteIndex(int writeIndex)
    {
        this._writeIndex = writeIndex;
    }

    public virtual Rgb[] GetRgbs()
    {
        return _rgbs;
    }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

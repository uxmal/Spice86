﻿namespace Spice86.Emulator.Gdb;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Serilog;

using Spice86.Emulator.Cpu;
using Spice86.Emulator.Machine;
using Spice86.Utils;

public class GdbCommandRegisterHandler
{
    private static readonly ILogger _logger = Log.Logger.ForContext<GdbCommandRegisterHandler>();
    private GdbIo gdbIo;
    private Machine machine;
    private GdbFormatter gdbFormatter = new GdbFormatter();
    public GdbCommandRegisterHandler(GdbIo gdbIo, Machine machine)
    {
        this.gdbIo = gdbIo;
        this.machine = machine;
    }

    public virtual string WriteRegister(string commandContent)
    {
        String[] split = commandContent.Split("=");
        uint registerIndex = (uint)ConvertUtils.ParseHex32(split[0]);
        uint registerValue = (uint)ConvertUtils.Swap32((int)ConvertUtils.ParseHex32(split[1]));
        SetRegisterValue((int)registerIndex, (int)registerValue);
        return gdbIo.GenerateResponse("OK");
    }

    public virtual string ReadRegister(string commandContent)
    {
        try
        {
            long index = ConvertUtils.ParseHex32(commandContent);
            _logger.Information("Reading register {@RegisterIndex}", index);
            return gdbIo.GenerateResponse(gdbFormatter.FormatValueAsHex32(GetRegisterValue((int)index)));
        }
        catch (FormatException nfe)
        {
            _logger.Error(nfe, "Register read requested but could not understand the request {@CommandContent}", commandContent);
            return gdbIo.GenerateUnsupportedResponse();
        }
    }

    public virtual string WriteAllRegisters(string commandContent)
    {
        try
        {
            byte[] data = ConvertUtils.HexToByteArray(commandContent);
            for (int i = 0; i < data.Length; i += 4)
            {
                long value = ConvertUtils.BytesToInt32(data, i);
                SetRegisterValue(i / 4, (int)value);
            }

            return gdbIo.GenerateResponse("OK");
        }
        catch (FormatException nfe)
        {
            _logger.Error(nfe, "Register write requested but could not understand the request {@CommandContent}", commandContent);
            return gdbIo.GenerateUnsupportedResponse();
        }
    }

    public virtual string ReadAllRegisters()
    {
        _logger.Information("Reading all registers");
        StringBuilder response = new(2 * 4 * 16);
        for (int i = 0; i < 16; i++)
        {
            string regValue = gdbFormatter.FormatValueAsHex32(GetRegisterValue(i));
            response.Append(regValue);
        }

        return gdbIo.GenerateResponse(response.ToString());
    }

    private int GetRegisterValue(int regIndex)
    {
        State state = machine.GetCpu().GetState();
        if (regIndex < 8)
        {
            return state.GetRegisters().GetRegister(regIndex);
        }

        if (regIndex == 8)
        {
            return state.GetIpPhysicalAddress();
        }

        if (regIndex == 9)
        {
            return state.GetFlags().GetFlagRegister();
        }

        if (regIndex < 16)
        {
            return state.GetSegmentRegisters().GetRegister(GetSegmentRegisterIndex(regIndex));
        }

        return 0;
    }

    private void SetRegisterValue(int regIndex, int value)
    {
        State state = machine.GetCpu().GetState();
        if (regIndex < 8)
        {
            state.GetRegisters().SetRegister(regIndex, value);
        }
        else if (regIndex == 8)
        {
            state.SetIP(value);
        }
        else if (regIndex == 9)
        {
            state.GetFlags().SetFlagRegister(value);
        }
        else if (regIndex < 16)
        {
            state.GetSegmentRegisters().SetRegister(GetSegmentRegisterIndex(regIndex), value);
        }
    }

    private int GetSegmentRegisterIndex(int gdbRegisterIndex)
    {
        int registerIndex = gdbRegisterIndex - 10;
        if (registerIndex < 3)
        {
            return registerIndex + 1;
        }

        if (registerIndex == 3)
        {
            return 0;
        }

        return registerIndex;
    }
}

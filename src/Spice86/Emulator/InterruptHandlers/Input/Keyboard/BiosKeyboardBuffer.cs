﻿namespace Spice86.Emulator.InterruptHandlers.Input.Keyboard;
using Spice86.Emulator.Memory;
using Spice86.Emulator.ReverseEngineer;

public class BiosKeyboardBuffer : MemoryBasedDataStructureWithBaseAddress
{
    private static readonly int START = 0x480;
    private static readonly int END = 0x482;
    private static readonly int HEAD = 0x41A;
    private static readonly int TAIL = 0x41C;
    private static readonly int INITIAL_START_ADDRESS = 0x41E;
    private static readonly int INITIAL_LENGTH = 0x20;
    public BiosKeyboardBuffer(Memory memory) : base(memory, 0)
    {
    }

    public virtual void Init()
    {
        this.SetStartAddress(INITIAL_START_ADDRESS);
        this.SetEndAddress(INITIAL_START_ADDRESS + INITIAL_LENGTH);
        this.SetHeadAddress(INITIAL_START_ADDRESS);
        this.SetTailAddress(INITIAL_START_ADDRESS);
    }

    public virtual int GetStartAddress()
    {
        return this.GetUint16(START);
    }

    public virtual void SetStartAddress(int value)
    {
        this.SetUint16(START, (ushort)value);
    }

    public virtual int GetEndAddress()
    {
        return this.GetUint16(END);
    }

    public virtual void SetEndAddress(int value)
    {
        this.SetUint16(END, (ushort)value);
    }

    public virtual int GetHeadAddress()
    {
        return this.GetUint16(HEAD);
    }

    public virtual void SetHeadAddress(int value)
    {
        this.SetUint16(HEAD, (ushort)value);
    }

    public virtual int GetTailAddress()
    {
        return this.GetUint16(TAIL);
    }

    public virtual void SetTailAddress(int value)
    {
        this.SetUint16(TAIL, (ushort)value);
    }

    public virtual bool AddKeyCode(int code)
    {
        int tail = GetTailAddress();
        int newTail = AdvancePointer(tail);
        if (newTail == GetHeadAddress())
        {

            // buffer full
            return false;
        }

        this.SetUint16(tail, (ushort)code);
        this.SetTailAddress(newTail);
        return true;
    }

    public virtual int? GetKeyCode()
    {
        int head = GetHeadAddress();
        if (Empty())
        {
            return null;
        }

        int newHead = AdvancePointer(GetHeadAddress());
        this.SetHeadAddress(newHead);
        return this.GetUint16(head);
    }

    public virtual bool Empty()
    {
        int head = GetHeadAddress();
        int tail = GetTailAddress();
        return head == tail;
    }

    private int AdvancePointer(int value)
    {
        int res = value + 2;
        if (res >= GetEndAddress())
        {
            return GetStartAddress();
        }

        return res;
    }
}
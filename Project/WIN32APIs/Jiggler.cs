/// Source: https://github.com/cerebrate/mousejiggler/blob/master/Jiggler.cs
/// With bug fix.
#region header

// MouseJiggle - Jiggler.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2013.  All rights reserved.
// 
// Created: 2014-04-24 8:08 AM

#endregion

#region using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

#endregion

namespace ArkaneSystems.MouseJiggle
{
    public static class Jiggler
    {
        /// <summary>
        /// The event is a mouse event. Use the mi structure of the union.
        /// </summary>
        internal const int INPUT_MOUSE = 0;
        /// <summary>
        /// Movement occurred.
        /// </summary>
        internal const int MOUSEEVENTF_MOVE = 0x0001;

        /// <summary>
        /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        public static void Jiggle(int dx, int dy)
        {
            var inp = new INPUT
            {
                Type = Jiggler.INPUT_MOUSE,
                Data = new MOUSEKEYBDHARDWAREINPUT()
                {
                    Mouse = new MOUSEINPUT()
                    {
                        X = dx,
                        Y = dy,
                        MouseData = 0,
                        Flags = Jiggler.MOUSEEVENTF_MOVE,
                        Time = 0,
                        ExtraInfo = (IntPtr)0,
                    }
                }
            };

            var retval = SendInput(1, ref inp, Marshal.SizeOf(inp));

            if (retval != 1)
            {
                var errcode = Marshal.GetLastWin32Error();
                Debugger.Log(1, "Jiggle", $"failed to insert event to input stream; retval={retval}, errcode={errcode:x}");
            }
        }
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646270(v=vs.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct INPUT
    {
        public uint Type;
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    /// <summary>
    /// http://social.msdn.microsoft.com/Forums/en/csharplanguage/thread/f0e82d6e-4999-4d22-b3d3-32b25f61fb2a
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct MOUSEKEYBDHARDWAREINPUT
    {
        [FieldOffset(0)]
        public HARDWAREINPUT Hardware;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        public uint Msg;
        public ushort ParamL;
        public ushort ParamH;
    }

    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/ms646310(v=vs.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        public ushort Vk;
        public ushort Scan;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    /// <summary>
    /// http://social.msdn.microsoft.com/forums/en-US/netfxbcl/thread/2abc6be8-c593-4686-93d2-89785232dacd
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MOUSEINPUT
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

}
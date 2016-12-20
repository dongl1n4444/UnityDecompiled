namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>Key codes returned by Event.keyCode. These map directly to a physical key on the keyboard.</para>
    /// </summary>
    public enum KeyCode
    {
        /// <summary>
        /// <para>'a' key.</para>
        /// </summary>
        A = 0x61,
        /// <summary>
        /// <para>The '0' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha0 = 0x30,
        /// <summary>
        /// <para>The '1' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha1 = 0x31,
        /// <summary>
        /// <para>The '2' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha2 = 50,
        /// <summary>
        /// <para>The '3' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha3 = 0x33,
        /// <summary>
        /// <para>The '4' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha4 = 0x34,
        /// <summary>
        /// <para>The '5' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha5 = 0x35,
        /// <summary>
        /// <para>The '6' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha6 = 0x36,
        /// <summary>
        /// <para>The '7' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha7 = 0x37,
        /// <summary>
        /// <para>The '8' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha8 = 0x38,
        /// <summary>
        /// <para>The '9' key on the top of the alphanumeric keyboard.</para>
        /// </summary>
        Alpha9 = 0x39,
        /// <summary>
        /// <para>Alt Gr key.</para>
        /// </summary>
        AltGr = 0x139,
        /// <summary>
        /// <para>Ampersand key '&amp;'.</para>
        /// </summary>
        Ampersand = 0x26,
        /// <summary>
        /// <para>Asterisk key '*'.</para>
        /// </summary>
        Asterisk = 0x2a,
        /// <summary>
        /// <para>At key '@'.</para>
        /// </summary>
        At = 0x40,
        /// <summary>
        /// <para>'b' key.</para>
        /// </summary>
        B = 0x62,
        /// <summary>
        /// <para>Back quote key '`'.</para>
        /// </summary>
        BackQuote = 0x60,
        /// <summary>
        /// <para>Backslash key '\'.</para>
        /// </summary>
        Backslash = 0x5c,
        /// <summary>
        /// <para>The backspace key.</para>
        /// </summary>
        Backspace = 8,
        /// <summary>
        /// <para>Break key.</para>
        /// </summary>
        Break = 0x13e,
        /// <summary>
        /// <para>'c' key.</para>
        /// </summary>
        C = 0x63,
        /// <summary>
        /// <para>Capslock key.</para>
        /// </summary>
        CapsLock = 0x12d,
        /// <summary>
        /// <para>Caret key '^'.</para>
        /// </summary>
        Caret = 0x5e,
        /// <summary>
        /// <para>The Clear key.</para>
        /// </summary>
        Clear = 12,
        /// <summary>
        /// <para>Colon ':' key.</para>
        /// </summary>
        Colon = 0x3a,
        /// <summary>
        /// <para>Comma ',' key.</para>
        /// </summary>
        Comma = 0x2c,
        /// <summary>
        /// <para>'d' key.</para>
        /// </summary>
        D = 100,
        /// <summary>
        /// <para>The forward delete key.</para>
        /// </summary>
        Delete = 0x7f,
        /// <summary>
        /// <para>Dollar sign key '$'.</para>
        /// </summary>
        Dollar = 0x24,
        /// <summary>
        /// <para>Double quote key '"'.</para>
        /// </summary>
        DoubleQuote = 0x22,
        /// <summary>
        /// <para>Down arrow key.</para>
        /// </summary>
        DownArrow = 0x112,
        /// <summary>
        /// <para>'e' key.</para>
        /// </summary>
        E = 0x65,
        /// <summary>
        /// <para>End key.</para>
        /// </summary>
        End = 0x117,
        /// <summary>
        /// <para>Equals '=' key.</para>
        /// </summary>
        Equals = 0x3d,
        /// <summary>
        /// <para>Escape key.</para>
        /// </summary>
        Escape = 0x1b,
        /// <summary>
        /// <para>Exclamation mark key '!'.</para>
        /// </summary>
        Exclaim = 0x21,
        /// <summary>
        /// <para>'f' key.</para>
        /// </summary>
        F = 0x66,
        /// <summary>
        /// <para>F1 function key.</para>
        /// </summary>
        F1 = 0x11a,
        /// <summary>
        /// <para>F10 function key.</para>
        /// </summary>
        F10 = 0x123,
        /// <summary>
        /// <para>F11 function key.</para>
        /// </summary>
        F11 = 0x124,
        /// <summary>
        /// <para>F12 function key.</para>
        /// </summary>
        F12 = 0x125,
        /// <summary>
        /// <para>F13 function key.</para>
        /// </summary>
        F13 = 0x126,
        /// <summary>
        /// <para>F14 function key.</para>
        /// </summary>
        F14 = 0x127,
        /// <summary>
        /// <para>F15 function key.</para>
        /// </summary>
        F15 = 0x128,
        /// <summary>
        /// <para>F2 function key.</para>
        /// </summary>
        F2 = 0x11b,
        /// <summary>
        /// <para>F3 function key.</para>
        /// </summary>
        F3 = 0x11c,
        /// <summary>
        /// <para>F4 function key.</para>
        /// </summary>
        F4 = 0x11d,
        /// <summary>
        /// <para>F5 function key.</para>
        /// </summary>
        F5 = 0x11e,
        /// <summary>
        /// <para>F6 function key.</para>
        /// </summary>
        F6 = 0x11f,
        /// <summary>
        /// <para>F7 function key.</para>
        /// </summary>
        F7 = 0x120,
        /// <summary>
        /// <para>F8 function key.</para>
        /// </summary>
        F8 = 0x121,
        /// <summary>
        /// <para>F9 function key.</para>
        /// </summary>
        F9 = 290,
        /// <summary>
        /// <para>'g' key.</para>
        /// </summary>
        G = 0x67,
        /// <summary>
        /// <para>Greater than '&gt;' key.</para>
        /// </summary>
        Greater = 0x3e,
        /// <summary>
        /// <para>'h' key.</para>
        /// </summary>
        H = 0x68,
        /// <summary>
        /// <para>Hash key '#'.</para>
        /// </summary>
        Hash = 0x23,
        /// <summary>
        /// <para>Help key.</para>
        /// </summary>
        Help = 0x13b,
        /// <summary>
        /// <para>Home key.</para>
        /// </summary>
        Home = 0x116,
        /// <summary>
        /// <para>'i' key.</para>
        /// </summary>
        I = 0x69,
        /// <summary>
        /// <para>Insert key key.</para>
        /// </summary>
        Insert = 0x115,
        /// <summary>
        /// <para>'j' key.</para>
        /// </summary>
        J = 0x6a,
        /// <summary>
        /// <para>Button 0 on first joystick.</para>
        /// </summary>
        Joystick1Button0 = 350,
        /// <summary>
        /// <para>Button 1 on first joystick.</para>
        /// </summary>
        Joystick1Button1 = 0x15f,
        /// <summary>
        /// <para>Button 10 on first joystick.</para>
        /// </summary>
        Joystick1Button10 = 360,
        /// <summary>
        /// <para>Button 11 on first joystick.</para>
        /// </summary>
        Joystick1Button11 = 0x169,
        /// <summary>
        /// <para>Button 12 on first joystick.</para>
        /// </summary>
        Joystick1Button12 = 0x16a,
        /// <summary>
        /// <para>Button 13 on first joystick.</para>
        /// </summary>
        Joystick1Button13 = 0x16b,
        /// <summary>
        /// <para>Button 14 on first joystick.</para>
        /// </summary>
        Joystick1Button14 = 0x16c,
        /// <summary>
        /// <para>Button 15 on first joystick.</para>
        /// </summary>
        Joystick1Button15 = 0x16d,
        /// <summary>
        /// <para>Button 16 on first joystick.</para>
        /// </summary>
        Joystick1Button16 = 0x16e,
        /// <summary>
        /// <para>Button 17 on first joystick.</para>
        /// </summary>
        Joystick1Button17 = 0x16f,
        /// <summary>
        /// <para>Button 18 on first joystick.</para>
        /// </summary>
        Joystick1Button18 = 0x170,
        /// <summary>
        /// <para>Button 19 on first joystick.</para>
        /// </summary>
        Joystick1Button19 = 0x171,
        /// <summary>
        /// <para>Button 2 on first joystick.</para>
        /// </summary>
        Joystick1Button2 = 0x160,
        /// <summary>
        /// <para>Button 3 on first joystick.</para>
        /// </summary>
        Joystick1Button3 = 0x161,
        /// <summary>
        /// <para>Button 4 on first joystick.</para>
        /// </summary>
        Joystick1Button4 = 0x162,
        /// <summary>
        /// <para>Button 5 on first joystick.</para>
        /// </summary>
        Joystick1Button5 = 0x163,
        /// <summary>
        /// <para>Button 6 on first joystick.</para>
        /// </summary>
        Joystick1Button6 = 0x164,
        /// <summary>
        /// <para>Button 7 on first joystick.</para>
        /// </summary>
        Joystick1Button7 = 0x165,
        /// <summary>
        /// <para>Button 8 on first joystick.</para>
        /// </summary>
        Joystick1Button8 = 0x166,
        /// <summary>
        /// <para>Button 9 on first joystick.</para>
        /// </summary>
        Joystick1Button9 = 0x167,
        /// <summary>
        /// <para>Button 0 on second joystick.</para>
        /// </summary>
        Joystick2Button0 = 370,
        /// <summary>
        /// <para>Button 1 on second joystick.</para>
        /// </summary>
        Joystick2Button1 = 0x173,
        /// <summary>
        /// <para>Button 10 on second joystick.</para>
        /// </summary>
        Joystick2Button10 = 380,
        /// <summary>
        /// <para>Button 11 on second joystick.</para>
        /// </summary>
        Joystick2Button11 = 0x17d,
        /// <summary>
        /// <para>Button 12 on second joystick.</para>
        /// </summary>
        Joystick2Button12 = 0x17e,
        /// <summary>
        /// <para>Button 13 on second joystick.</para>
        /// </summary>
        Joystick2Button13 = 0x17f,
        /// <summary>
        /// <para>Button 14 on second joystick.</para>
        /// </summary>
        Joystick2Button14 = 0x180,
        /// <summary>
        /// <para>Button 15 on second joystick.</para>
        /// </summary>
        Joystick2Button15 = 0x181,
        /// <summary>
        /// <para>Button 16 on second joystick.</para>
        /// </summary>
        Joystick2Button16 = 0x182,
        /// <summary>
        /// <para>Button 17 on second joystick.</para>
        /// </summary>
        Joystick2Button17 = 0x183,
        /// <summary>
        /// <para>Button 18 on second joystick.</para>
        /// </summary>
        Joystick2Button18 = 0x184,
        /// <summary>
        /// <para>Button 19 on second joystick.</para>
        /// </summary>
        Joystick2Button19 = 0x185,
        /// <summary>
        /// <para>Button 2 on second joystick.</para>
        /// </summary>
        Joystick2Button2 = 0x174,
        /// <summary>
        /// <para>Button 3 on second joystick.</para>
        /// </summary>
        Joystick2Button3 = 0x175,
        /// <summary>
        /// <para>Button 4 on second joystick.</para>
        /// </summary>
        Joystick2Button4 = 0x176,
        /// <summary>
        /// <para>Button 5 on second joystick.</para>
        /// </summary>
        Joystick2Button5 = 0x177,
        /// <summary>
        /// <para>Button 6 on second joystick.</para>
        /// </summary>
        Joystick2Button6 = 0x178,
        /// <summary>
        /// <para>Button 7 on second joystick.</para>
        /// </summary>
        Joystick2Button7 = 0x179,
        /// <summary>
        /// <para>Button 8 on second joystick.</para>
        /// </summary>
        Joystick2Button8 = 0x17a,
        /// <summary>
        /// <para>Button 9 on second joystick.</para>
        /// </summary>
        Joystick2Button9 = 0x17b,
        /// <summary>
        /// <para>Button 0 on third joystick.</para>
        /// </summary>
        Joystick3Button0 = 390,
        /// <summary>
        /// <para>Button 1 on third joystick.</para>
        /// </summary>
        Joystick3Button1 = 0x187,
        /// <summary>
        /// <para>Button 10 on third joystick.</para>
        /// </summary>
        Joystick3Button10 = 400,
        /// <summary>
        /// <para>Button 11 on third joystick.</para>
        /// </summary>
        Joystick3Button11 = 0x191,
        /// <summary>
        /// <para>Button 12 on third joystick.</para>
        /// </summary>
        Joystick3Button12 = 0x192,
        /// <summary>
        /// <para>Button 13 on third joystick.</para>
        /// </summary>
        Joystick3Button13 = 0x193,
        /// <summary>
        /// <para>Button 14 on third joystick.</para>
        /// </summary>
        Joystick3Button14 = 0x194,
        /// <summary>
        /// <para>Button 15 on third joystick.</para>
        /// </summary>
        Joystick3Button15 = 0x195,
        /// <summary>
        /// <para>Button 16 on third joystick.</para>
        /// </summary>
        Joystick3Button16 = 0x196,
        /// <summary>
        /// <para>Button 17 on third joystick.</para>
        /// </summary>
        Joystick3Button17 = 0x197,
        /// <summary>
        /// <para>Button 18 on third joystick.</para>
        /// </summary>
        Joystick3Button18 = 0x198,
        /// <summary>
        /// <para>Button 19 on third joystick.</para>
        /// </summary>
        Joystick3Button19 = 0x199,
        /// <summary>
        /// <para>Button 2 on third joystick.</para>
        /// </summary>
        Joystick3Button2 = 0x188,
        /// <summary>
        /// <para>Button 3 on third joystick.</para>
        /// </summary>
        Joystick3Button3 = 0x189,
        /// <summary>
        /// <para>Button 4 on third joystick.</para>
        /// </summary>
        Joystick3Button4 = 0x18a,
        /// <summary>
        /// <para>Button 5 on third joystick.</para>
        /// </summary>
        Joystick3Button5 = 0x18b,
        /// <summary>
        /// <para>Button 6 on third joystick.</para>
        /// </summary>
        Joystick3Button6 = 0x18c,
        /// <summary>
        /// <para>Button 7 on third joystick.</para>
        /// </summary>
        Joystick3Button7 = 0x18d,
        /// <summary>
        /// <para>Button 8 on third joystick.</para>
        /// </summary>
        Joystick3Button8 = 0x18e,
        /// <summary>
        /// <para>Button 9 on third joystick.</para>
        /// </summary>
        Joystick3Button9 = 0x18f,
        /// <summary>
        /// <para>Button 0 on forth joystick.</para>
        /// </summary>
        Joystick4Button0 = 410,
        /// <summary>
        /// <para>Button 1 on forth joystick.</para>
        /// </summary>
        Joystick4Button1 = 0x19b,
        /// <summary>
        /// <para>Button 10 on forth joystick.</para>
        /// </summary>
        Joystick4Button10 = 420,
        /// <summary>
        /// <para>Button 11 on forth joystick.</para>
        /// </summary>
        Joystick4Button11 = 0x1a5,
        /// <summary>
        /// <para>Button 12 on forth joystick.</para>
        /// </summary>
        Joystick4Button12 = 0x1a6,
        /// <summary>
        /// <para>Button 13 on forth joystick.</para>
        /// </summary>
        Joystick4Button13 = 0x1a7,
        /// <summary>
        /// <para>Button 14 on forth joystick.</para>
        /// </summary>
        Joystick4Button14 = 0x1a8,
        /// <summary>
        /// <para>Button 15 on forth joystick.</para>
        /// </summary>
        Joystick4Button15 = 0x1a9,
        /// <summary>
        /// <para>Button 16 on forth joystick.</para>
        /// </summary>
        Joystick4Button16 = 0x1aa,
        /// <summary>
        /// <para>Button 17 on forth joystick.</para>
        /// </summary>
        Joystick4Button17 = 0x1ab,
        /// <summary>
        /// <para>Button 18 on forth joystick.</para>
        /// </summary>
        Joystick4Button18 = 0x1ac,
        /// <summary>
        /// <para>Button 19 on forth joystick.</para>
        /// </summary>
        Joystick4Button19 = 0x1ad,
        /// <summary>
        /// <para>Button 2 on forth joystick.</para>
        /// </summary>
        Joystick4Button2 = 0x19c,
        /// <summary>
        /// <para>Button 3 on forth joystick.</para>
        /// </summary>
        Joystick4Button3 = 0x19d,
        /// <summary>
        /// <para>Button 4 on forth joystick.</para>
        /// </summary>
        Joystick4Button4 = 0x19e,
        /// <summary>
        /// <para>Button 5 on forth joystick.</para>
        /// </summary>
        Joystick4Button5 = 0x19f,
        /// <summary>
        /// <para>Button 6 on forth joystick.</para>
        /// </summary>
        Joystick4Button6 = 0x1a0,
        /// <summary>
        /// <para>Button 7 on forth joystick.</para>
        /// </summary>
        Joystick4Button7 = 0x1a1,
        /// <summary>
        /// <para>Button 8 on forth joystick.</para>
        /// </summary>
        Joystick4Button8 = 0x1a2,
        /// <summary>
        /// <para>Button 9 on forth joystick.</para>
        /// </summary>
        Joystick4Button9 = 0x1a3,
        /// <summary>
        /// <para>Button 0 on fifth joystick.</para>
        /// </summary>
        Joystick5Button0 = 430,
        /// <summary>
        /// <para>Button 1 on fifth joystick.</para>
        /// </summary>
        Joystick5Button1 = 0x1af,
        /// <summary>
        /// <para>Button 10 on fifth joystick.</para>
        /// </summary>
        Joystick5Button10 = 440,
        /// <summary>
        /// <para>Button 11 on fifth joystick.</para>
        /// </summary>
        Joystick5Button11 = 0x1b9,
        /// <summary>
        /// <para>Button 12 on fifth joystick.</para>
        /// </summary>
        Joystick5Button12 = 0x1ba,
        /// <summary>
        /// <para>Button 13 on fifth joystick.</para>
        /// </summary>
        Joystick5Button13 = 0x1bb,
        /// <summary>
        /// <para>Button 14 on fifth joystick.</para>
        /// </summary>
        Joystick5Button14 = 0x1bc,
        /// <summary>
        /// <para>Button 15 on fifth joystick.</para>
        /// </summary>
        Joystick5Button15 = 0x1bd,
        /// <summary>
        /// <para>Button 16 on fifth joystick.</para>
        /// </summary>
        Joystick5Button16 = 0x1be,
        /// <summary>
        /// <para>Button 17 on fifth joystick.</para>
        /// </summary>
        Joystick5Button17 = 0x1bf,
        /// <summary>
        /// <para>Button 18 on fifth joystick.</para>
        /// </summary>
        Joystick5Button18 = 0x1c0,
        /// <summary>
        /// <para>Button 19 on fifth joystick.</para>
        /// </summary>
        Joystick5Button19 = 0x1c1,
        /// <summary>
        /// <para>Button 2 on fifth joystick.</para>
        /// </summary>
        Joystick5Button2 = 0x1b0,
        /// <summary>
        /// <para>Button 3 on fifth joystick.</para>
        /// </summary>
        Joystick5Button3 = 0x1b1,
        /// <summary>
        /// <para>Button 4 on fifth joystick.</para>
        /// </summary>
        Joystick5Button4 = 0x1b2,
        /// <summary>
        /// <para>Button 5 on fifth joystick.</para>
        /// </summary>
        Joystick5Button5 = 0x1b3,
        /// <summary>
        /// <para>Button 6 on fifth joystick.</para>
        /// </summary>
        Joystick5Button6 = 0x1b4,
        /// <summary>
        /// <para>Button 7 on fifth joystick.</para>
        /// </summary>
        Joystick5Button7 = 0x1b5,
        /// <summary>
        /// <para>Button 8 on fifth joystick.</para>
        /// </summary>
        Joystick5Button8 = 0x1b6,
        /// <summary>
        /// <para>Button 9 on fifth joystick.</para>
        /// </summary>
        Joystick5Button9 = 0x1b7,
        /// <summary>
        /// <para>Button 0 on sixth joystick.</para>
        /// </summary>
        Joystick6Button0 = 450,
        /// <summary>
        /// <para>Button 1 on sixth joystick.</para>
        /// </summary>
        Joystick6Button1 = 0x1c3,
        /// <summary>
        /// <para>Button 10 on sixth joystick.</para>
        /// </summary>
        Joystick6Button10 = 460,
        /// <summary>
        /// <para>Button 11 on sixth joystick.</para>
        /// </summary>
        Joystick6Button11 = 0x1cd,
        /// <summary>
        /// <para>Button 12 on sixth joystick.</para>
        /// </summary>
        Joystick6Button12 = 0x1ce,
        /// <summary>
        /// <para>Button 13 on sixth joystick.</para>
        /// </summary>
        Joystick6Button13 = 0x1cf,
        /// <summary>
        /// <para>Button 14 on sixth joystick.</para>
        /// </summary>
        Joystick6Button14 = 0x1d0,
        /// <summary>
        /// <para>Button 15 on sixth joystick.</para>
        /// </summary>
        Joystick6Button15 = 0x1d1,
        /// <summary>
        /// <para>Button 16 on sixth joystick.</para>
        /// </summary>
        Joystick6Button16 = 0x1d2,
        /// <summary>
        /// <para>Button 17 on sixth joystick.</para>
        /// </summary>
        Joystick6Button17 = 0x1d3,
        /// <summary>
        /// <para>Button 18 on sixth joystick.</para>
        /// </summary>
        Joystick6Button18 = 0x1d4,
        /// <summary>
        /// <para>Button 19 on sixth joystick.</para>
        /// </summary>
        Joystick6Button19 = 0x1d5,
        /// <summary>
        /// <para>Button 2 on sixth joystick.</para>
        /// </summary>
        Joystick6Button2 = 0x1c4,
        /// <summary>
        /// <para>Button 3 on sixth joystick.</para>
        /// </summary>
        Joystick6Button3 = 0x1c5,
        /// <summary>
        /// <para>Button 4 on sixth joystick.</para>
        /// </summary>
        Joystick6Button4 = 0x1c6,
        /// <summary>
        /// <para>Button 5 on sixth joystick.</para>
        /// </summary>
        Joystick6Button5 = 0x1c7,
        /// <summary>
        /// <para>Button 6 on sixth joystick.</para>
        /// </summary>
        Joystick6Button6 = 0x1c8,
        /// <summary>
        /// <para>Button 7 on sixth joystick.</para>
        /// </summary>
        Joystick6Button7 = 0x1c9,
        /// <summary>
        /// <para>Button 8 on sixth joystick.</para>
        /// </summary>
        Joystick6Button8 = 0x1ca,
        /// <summary>
        /// <para>Button 9 on sixth joystick.</para>
        /// </summary>
        Joystick6Button9 = 0x1cb,
        /// <summary>
        /// <para>Button 0 on seventh joystick.</para>
        /// </summary>
        Joystick7Button0 = 470,
        /// <summary>
        /// <para>Button 1 on seventh joystick.</para>
        /// </summary>
        Joystick7Button1 = 0x1d7,
        /// <summary>
        /// <para>Button 10 on seventh joystick.</para>
        /// </summary>
        Joystick7Button10 = 480,
        /// <summary>
        /// <para>Button 11 on seventh joystick.</para>
        /// </summary>
        Joystick7Button11 = 0x1e1,
        /// <summary>
        /// <para>Button 12 on seventh joystick.</para>
        /// </summary>
        Joystick7Button12 = 0x1e2,
        /// <summary>
        /// <para>Button 13 on seventh joystick.</para>
        /// </summary>
        Joystick7Button13 = 0x1e3,
        /// <summary>
        /// <para>Button 14 on seventh joystick.</para>
        /// </summary>
        Joystick7Button14 = 0x1e4,
        /// <summary>
        /// <para>Button 15 on seventh joystick.</para>
        /// </summary>
        Joystick7Button15 = 0x1e5,
        /// <summary>
        /// <para>Button 16 on seventh joystick.</para>
        /// </summary>
        Joystick7Button16 = 0x1e6,
        /// <summary>
        /// <para>Button 17 on seventh joystick.</para>
        /// </summary>
        Joystick7Button17 = 0x1e7,
        /// <summary>
        /// <para>Button 18 on seventh joystick.</para>
        /// </summary>
        Joystick7Button18 = 0x1e8,
        /// <summary>
        /// <para>Button 19 on seventh joystick.</para>
        /// </summary>
        Joystick7Button19 = 0x1e9,
        /// <summary>
        /// <para>Button 2 on seventh joystick.</para>
        /// </summary>
        Joystick7Button2 = 0x1d8,
        /// <summary>
        /// <para>Button 3 on seventh joystick.</para>
        /// </summary>
        Joystick7Button3 = 0x1d9,
        /// <summary>
        /// <para>Button 4 on seventh joystick.</para>
        /// </summary>
        Joystick7Button4 = 0x1da,
        /// <summary>
        /// <para>Button 5 on seventh joystick.</para>
        /// </summary>
        Joystick7Button5 = 0x1db,
        /// <summary>
        /// <para>Button 6 on seventh joystick.</para>
        /// </summary>
        Joystick7Button6 = 0x1dc,
        /// <summary>
        /// <para>Button 7 on seventh joystick.</para>
        /// </summary>
        Joystick7Button7 = 0x1dd,
        /// <summary>
        /// <para>Button 8 on seventh joystick.</para>
        /// </summary>
        Joystick7Button8 = 0x1de,
        /// <summary>
        /// <para>Button 9 on seventh joystick.</para>
        /// </summary>
        Joystick7Button9 = 0x1df,
        /// <summary>
        /// <para>Button 0 on eighth joystick.</para>
        /// </summary>
        Joystick8Button0 = 490,
        /// <summary>
        /// <para>Button 1 on eighth joystick.</para>
        /// </summary>
        Joystick8Button1 = 0x1eb,
        /// <summary>
        /// <para>Button 10 on eighth joystick.</para>
        /// </summary>
        Joystick8Button10 = 500,
        /// <summary>
        /// <para>Button 11 on eighth joystick.</para>
        /// </summary>
        Joystick8Button11 = 0x1f5,
        /// <summary>
        /// <para>Button 12 on eighth joystick.</para>
        /// </summary>
        Joystick8Button12 = 0x1f6,
        /// <summary>
        /// <para>Button 13 on eighth joystick.</para>
        /// </summary>
        Joystick8Button13 = 0x1f7,
        /// <summary>
        /// <para>Button 14 on eighth joystick.</para>
        /// </summary>
        Joystick8Button14 = 0x1f8,
        /// <summary>
        /// <para>Button 15 on eighth joystick.</para>
        /// </summary>
        Joystick8Button15 = 0x1f9,
        /// <summary>
        /// <para>Button 16 on eighth joystick.</para>
        /// </summary>
        Joystick8Button16 = 0x1fa,
        /// <summary>
        /// <para>Button 17 on eighth joystick.</para>
        /// </summary>
        Joystick8Button17 = 0x1fb,
        /// <summary>
        /// <para>Button 18 on eighth joystick.</para>
        /// </summary>
        Joystick8Button18 = 0x1fc,
        /// <summary>
        /// <para>Button 19 on eighth joystick.</para>
        /// </summary>
        Joystick8Button19 = 0x1fd,
        /// <summary>
        /// <para>Button 2 on eighth joystick.</para>
        /// </summary>
        Joystick8Button2 = 0x1ec,
        /// <summary>
        /// <para>Button 3 on eighth joystick.</para>
        /// </summary>
        Joystick8Button3 = 0x1ed,
        /// <summary>
        /// <para>Button 4 on eighth joystick.</para>
        /// </summary>
        Joystick8Button4 = 0x1ee,
        /// <summary>
        /// <para>Button 5 on eighth joystick.</para>
        /// </summary>
        Joystick8Button5 = 0x1ef,
        /// <summary>
        /// <para>Button 6 on eighth joystick.</para>
        /// </summary>
        Joystick8Button6 = 0x1f0,
        /// <summary>
        /// <para>Button 7 on eighth joystick.</para>
        /// </summary>
        Joystick8Button7 = 0x1f1,
        /// <summary>
        /// <para>Button 8 on eighth joystick.</para>
        /// </summary>
        Joystick8Button8 = 0x1f2,
        /// <summary>
        /// <para>Button 9 on eighth joystick.</para>
        /// </summary>
        Joystick8Button9 = 0x1f3,
        /// <summary>
        /// <para>Button 0 on any joystick.</para>
        /// </summary>
        JoystickButton0 = 330,
        /// <summary>
        /// <para>Button 1 on any joystick.</para>
        /// </summary>
        JoystickButton1 = 0x14b,
        /// <summary>
        /// <para>Button 10 on any joystick.</para>
        /// </summary>
        JoystickButton10 = 340,
        /// <summary>
        /// <para>Button 11 on any joystick.</para>
        /// </summary>
        JoystickButton11 = 0x155,
        /// <summary>
        /// <para>Button 12 on any joystick.</para>
        /// </summary>
        JoystickButton12 = 0x156,
        /// <summary>
        /// <para>Button 13 on any joystick.</para>
        /// </summary>
        JoystickButton13 = 0x157,
        /// <summary>
        /// <para>Button 14 on any joystick.</para>
        /// </summary>
        JoystickButton14 = 0x158,
        /// <summary>
        /// <para>Button 15 on any joystick.</para>
        /// </summary>
        JoystickButton15 = 0x159,
        /// <summary>
        /// <para>Button 16 on any joystick.</para>
        /// </summary>
        JoystickButton16 = 0x15a,
        /// <summary>
        /// <para>Button 17 on any joystick.</para>
        /// </summary>
        JoystickButton17 = 0x15b,
        /// <summary>
        /// <para>Button 18 on any joystick.</para>
        /// </summary>
        JoystickButton18 = 0x15c,
        /// <summary>
        /// <para>Button 19 on any joystick.</para>
        /// </summary>
        JoystickButton19 = 0x15d,
        /// <summary>
        /// <para>Button 2 on any joystick.</para>
        /// </summary>
        JoystickButton2 = 0x14c,
        /// <summary>
        /// <para>Button 3 on any joystick.</para>
        /// </summary>
        JoystickButton3 = 0x14d,
        /// <summary>
        /// <para>Button 4 on any joystick.</para>
        /// </summary>
        JoystickButton4 = 0x14e,
        /// <summary>
        /// <para>Button 5 on any joystick.</para>
        /// </summary>
        JoystickButton5 = 0x14f,
        /// <summary>
        /// <para>Button 6 on any joystick.</para>
        /// </summary>
        JoystickButton6 = 0x150,
        /// <summary>
        /// <para>Button 7 on any joystick.</para>
        /// </summary>
        JoystickButton7 = 0x151,
        /// <summary>
        /// <para>Button 8 on any joystick.</para>
        /// </summary>
        JoystickButton8 = 0x152,
        /// <summary>
        /// <para>Button 9 on any joystick.</para>
        /// </summary>
        JoystickButton9 = 0x153,
        /// <summary>
        /// <para>'k' key.</para>
        /// </summary>
        K = 0x6b,
        /// <summary>
        /// <para>Numeric keypad 0.</para>
        /// </summary>
        Keypad0 = 0x100,
        /// <summary>
        /// <para>Numeric keypad 1.</para>
        /// </summary>
        Keypad1 = 0x101,
        /// <summary>
        /// <para>Numeric keypad 2.</para>
        /// </summary>
        Keypad2 = 0x102,
        /// <summary>
        /// <para>Numeric keypad 3.</para>
        /// </summary>
        Keypad3 = 0x103,
        /// <summary>
        /// <para>Numeric keypad 4.</para>
        /// </summary>
        Keypad4 = 260,
        /// <summary>
        /// <para>Numeric keypad 5.</para>
        /// </summary>
        Keypad5 = 0x105,
        /// <summary>
        /// <para>Numeric keypad 6.</para>
        /// </summary>
        Keypad6 = 0x106,
        /// <summary>
        /// <para>Numeric keypad 7.</para>
        /// </summary>
        Keypad7 = 0x107,
        /// <summary>
        /// <para>Numeric keypad 8.</para>
        /// </summary>
        Keypad8 = 0x108,
        /// <summary>
        /// <para>Numeric keypad 9.</para>
        /// </summary>
        Keypad9 = 0x109,
        /// <summary>
        /// <para>Numeric keypad '/'.</para>
        /// </summary>
        KeypadDivide = 0x10b,
        /// <summary>
        /// <para>Numeric keypad enter.</para>
        /// </summary>
        KeypadEnter = 0x10f,
        /// <summary>
        /// <para>Numeric keypad '='.</para>
        /// </summary>
        KeypadEquals = 0x110,
        /// <summary>
        /// <para>Numeric keypad '-'.</para>
        /// </summary>
        KeypadMinus = 0x10d,
        /// <summary>
        /// <para>Numeric keypad '*'.</para>
        /// </summary>
        KeypadMultiply = 0x10c,
        /// <summary>
        /// <para>Numeric keypad '.'.</para>
        /// </summary>
        KeypadPeriod = 0x10a,
        /// <summary>
        /// <para>Numeric keypad '+'.</para>
        /// </summary>
        KeypadPlus = 270,
        /// <summary>
        /// <para>'l' key.</para>
        /// </summary>
        L = 0x6c,
        /// <summary>
        /// <para>Left Alt key.</para>
        /// </summary>
        LeftAlt = 0x134,
        /// <summary>
        /// <para>Left Command key.</para>
        /// </summary>
        LeftApple = 310,
        /// <summary>
        /// <para>Left arrow key.</para>
        /// </summary>
        LeftArrow = 0x114,
        /// <summary>
        /// <para>Left square bracket key '['.</para>
        /// </summary>
        LeftBracket = 0x5b,
        /// <summary>
        /// <para>Left Command key.</para>
        /// </summary>
        LeftCommand = 310,
        /// <summary>
        /// <para>Left Control key.</para>
        /// </summary>
        LeftControl = 0x132,
        /// <summary>
        /// <para>Left Parenthesis key '('.</para>
        /// </summary>
        LeftParen = 40,
        /// <summary>
        /// <para>Left shift key.</para>
        /// </summary>
        LeftShift = 0x130,
        /// <summary>
        /// <para>Left Windows key.</para>
        /// </summary>
        LeftWindows = 0x137,
        /// <summary>
        /// <para>Less than '&lt;' key.</para>
        /// </summary>
        Less = 60,
        /// <summary>
        /// <para>'m' key.</para>
        /// </summary>
        M = 0x6d,
        /// <summary>
        /// <para>Menu key.</para>
        /// </summary>
        Menu = 0x13f,
        /// <summary>
        /// <para>Minus '-' key.</para>
        /// </summary>
        Minus = 0x2d,
        /// <summary>
        /// <para>First (primary) mouse button.</para>
        /// </summary>
        Mouse0 = 0x143,
        /// <summary>
        /// <para>Second (secondary) mouse button.</para>
        /// </summary>
        Mouse1 = 0x144,
        /// <summary>
        /// <para>Third mouse button.</para>
        /// </summary>
        Mouse2 = 0x145,
        /// <summary>
        /// <para>Fourth mouse button.</para>
        /// </summary>
        Mouse3 = 0x146,
        /// <summary>
        /// <para>Fifth mouse button.</para>
        /// </summary>
        Mouse4 = 0x147,
        /// <summary>
        /// <para>Sixth mouse button.</para>
        /// </summary>
        Mouse5 = 0x148,
        /// <summary>
        /// <para>Seventh mouse button.</para>
        /// </summary>
        Mouse6 = 0x149,
        /// <summary>
        /// <para>'n' key.</para>
        /// </summary>
        N = 110,
        /// <summary>
        /// <para>Not assigned (never returned as the result of a keystroke).</para>
        /// </summary>
        None = 0,
        /// <summary>
        /// <para>Numlock key.</para>
        /// </summary>
        Numlock = 300,
        /// <summary>
        /// <para>'o' key.</para>
        /// </summary>
        O = 0x6f,
        /// <summary>
        /// <para>'p' key.</para>
        /// </summary>
        P = 0x70,
        /// <summary>
        /// <para>Page down.</para>
        /// </summary>
        PageDown = 0x119,
        /// <summary>
        /// <para>Page up.</para>
        /// </summary>
        PageUp = 280,
        /// <summary>
        /// <para>Pause on PC machines.</para>
        /// </summary>
        Pause = 0x13,
        /// <summary>
        /// <para>Period '.' key.</para>
        /// </summary>
        Period = 0x2e,
        /// <summary>
        /// <para>Plus key '+'.</para>
        /// </summary>
        Plus = 0x2b,
        /// <summary>
        /// <para>Print key.</para>
        /// </summary>
        Print = 0x13c,
        /// <summary>
        /// <para>'q' key.</para>
        /// </summary>
        Q = 0x71,
        /// <summary>
        /// <para>Question mark '?' key.</para>
        /// </summary>
        Question = 0x3f,
        /// <summary>
        /// <para>Quote key '.</para>
        /// </summary>
        Quote = 0x27,
        /// <summary>
        /// <para>'r' key.</para>
        /// </summary>
        R = 0x72,
        /// <summary>
        /// <para>Return key.</para>
        /// </summary>
        Return = 13,
        /// <summary>
        /// <para>Right Alt key.</para>
        /// </summary>
        RightAlt = 0x133,
        /// <summary>
        /// <para>Right Command key.</para>
        /// </summary>
        RightApple = 0x135,
        /// <summary>
        /// <para>Right arrow key.</para>
        /// </summary>
        RightArrow = 0x113,
        /// <summary>
        /// <para>Right square bracket key ']'.</para>
        /// </summary>
        RightBracket = 0x5d,
        /// <summary>
        /// <para>Right Command key.</para>
        /// </summary>
        RightCommand = 0x135,
        /// <summary>
        /// <para>Right Control key.</para>
        /// </summary>
        RightControl = 0x131,
        /// <summary>
        /// <para>Right Parenthesis key ')'.</para>
        /// </summary>
        RightParen = 0x29,
        /// <summary>
        /// <para>Right shift key.</para>
        /// </summary>
        RightShift = 0x12f,
        /// <summary>
        /// <para>Right Windows key.</para>
        /// </summary>
        RightWindows = 0x138,
        /// <summary>
        /// <para>'s' key.</para>
        /// </summary>
        S = 0x73,
        /// <summary>
        /// <para>Scroll lock key.</para>
        /// </summary>
        ScrollLock = 0x12e,
        /// <summary>
        /// <para>Semicolon ';' key.</para>
        /// </summary>
        Semicolon = 0x3b,
        /// <summary>
        /// <para>Slash '/' key.</para>
        /// </summary>
        Slash = 0x2f,
        /// <summary>
        /// <para>Space key.</para>
        /// </summary>
        Space = 0x20,
        /// <summary>
        /// <para>Sys Req key.</para>
        /// </summary>
        SysReq = 0x13d,
        /// <summary>
        /// <para>'t' key.</para>
        /// </summary>
        T = 0x74,
        /// <summary>
        /// <para>The tab key.</para>
        /// </summary>
        Tab = 9,
        /// <summary>
        /// <para>'u' key.</para>
        /// </summary>
        U = 0x75,
        /// <summary>
        /// <para>Underscore '_' key.</para>
        /// </summary>
        Underscore = 0x5f,
        /// <summary>
        /// <para>Up arrow key.</para>
        /// </summary>
        UpArrow = 0x111,
        /// <summary>
        /// <para>'v' key.</para>
        /// </summary>
        V = 0x76,
        /// <summary>
        /// <para>'w' key.</para>
        /// </summary>
        W = 0x77,
        /// <summary>
        /// <para>'x' key.</para>
        /// </summary>
        X = 120,
        /// <summary>
        /// <para>'y' key.</para>
        /// </summary>
        Y = 0x79,
        /// <summary>
        /// <para>'z' key.</para>
        /// </summary>
        Z = 0x7a
    }
}


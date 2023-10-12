using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CustomMacroPlugin0.Tools.StaticManager
{
    //引用dll部分
    static partial class SendKBMInput
    {
        #region 引用/声明
        ///// APIの利用に必要な構造体・共用体の定義　ここから /////
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public int X;
            public int Y;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        };

        [StructLayout(LayoutKind.Explicit)]
        private struct INPUT_UNION
        {
            [FieldOffset(0)] public MOUSEINPUT mouse;
            [FieldOffset(0)] public KEYBDINPUT keyboard;
            [FieldOffset(0)] public HARDWAREINPUT hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public int type;
            public INPUT_UNION ui;
        };
        ///// APIの利用に必要な構造体・共用体の定義　ここまで /////


        // 関数
        [DllImport("user32.dll")]
        private extern static void SendInput(int nInputs, ref INPUT pInputs, int cbsize);
        [DllImport("user32.dll")]
        private extern static void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        //マウスとキーボードをロックできる関数
        [DllImport("user32.dll")]
        private static extern void BlockInput(bool Block);
        //
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);


        // 定数の定義
        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        private const int MOUSEEVENTF_MOVE = 0x1;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const int MOUSEEVENTF_LEFTDOWN = 0x2;
        private const int MOUSEEVENTF_LEFTUP = 0x4;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x8;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENTF_MIDDLEUP = 0x40;
        private const int MOUSEEVENTF_WHEEL = 0x800;
        private const int WHEEL_DELTA = 120;

        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        #endregion

        //要操作鼠标哪个键
        public enum MouseKeys
        {
            Left = 0,
            Right,
            Middle,
        }

        /// <summary>
        /// SendInput简化
        /// </summary>
        private static void PreSendInput(ref INPUT[] _inputs) => SendInput(_inputs.Length, ref _inputs[0], Marshal.SizeOf(_inputs[0]));
    }

    //鼠标键盘代理（仅内部调用）
    static partial class SendKBMInput
    {
        //鼠标
        private partial class MouseProxy
        {
            private static Dictionary<int, bool> internalList = new() { };
            private static Dictionary<string, bool> internalListEx = new() { };
            private static readonly object objlock = new object();
            private static MouseProxy? _instance;
            public static MouseProxy Instance
            {
                get
                {
                    lock (objlock)
                    {
                        if (_instance is null) { _instance = new(); }
                    }
                    return _instance;
                }
            }

            static MouseProxy()
            {
                foreach (var item in Enum.GetValues(typeof(MouseKeys)).Cast<MouseKeys>().ToList())
                {
                    if (internalList.ContainsKey((short)item) is false) internalList.Add((short)item, false);
                }
            }

            private static void SendMouseMove(Point pt)
            {
                Win32Point mousePosition = new Win32Point
                {
                    X = (int)(pt.X * ((double)65535 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)),
                    Y = (int)(pt.Y * ((double)65535 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height))
                };

                //Move
                INPUT[] inputs_move = new INPUT[] {
                    new INPUT {
                        type = INPUT_MOUSE,
                        ui = new INPUT_UNION {
                            mouse = new MOUSEINPUT {
                                dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
                                dx = mousePosition.X,
                                dy = mousePosition.Y,
                                mouseData = 0,
                                dwExtraInfo = IntPtr.Zero,
                                time = 0
                            }
                        }
                    }
                };

                PreSendInput(ref inputs_move);
            }
            private static void SendMouseDown(int key)
            {
                //Down
                INPUT[] inputs_click = new INPUT[]
                {
                    new INPUT {
                        type = INPUT_MOUSE,
                        ui = new INPUT_UNION {
                            mouse = new MOUSEINPUT {
                                dwFlags = key,//MOUSEEVENTF_LEFTDOWN
                                dx = 0,
                                dy = 0,
                                mouseData = 0,
                                dwExtraInfo = IntPtr.Zero,
                                time = 0
                            }
                        }
                    }
                };

                PreSendInput(ref inputs_click);
            }
            private static void SendMouseUp(int key)
            {
                //Up
                INPUT[] inputs_click = new INPUT[]
                {
                    new INPUT {
                        type = INPUT_MOUSE,
                        ui = new INPUT_UNION {
                            mouse = new MOUSEINPUT {
                                dwFlags = key,//MOUSEEVENTF_LEFTUP
                                dx = 0,
                                dy = 0,
                                mouseData = 0,
                                dwExtraInfo = IntPtr.Zero,
                                time = 0
                            }
                        }
                    }
                };

                PreSendInput(ref inputs_click);
            }

            private static int PressOrRelease(int key, bool flag)
            {
                return (MouseKeys)key switch
                {
                    MouseKeys.Left => flag ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP,
                    MouseKeys.Right => flag ? MOUSEEVENTF_RIGHTDOWN : MOUSEEVENTF_RIGHTUP,
                    MouseKeys.Middle => flag ? MOUSEEVENTF_MIDDLEDOWN : MOUSEEVENTF_MIDDLEUP,
                    _ => 0
                };
            }

            public void Update(Point pt)
            {
                SendMouseMove(pt);
            }
            public void Update(MouseKeys[] keys, bool flag)
            {
                if (keys.Length == 1)
                {
                    int key = (int)keys[0];
                    if (flag is true && internalList[key] is false)
                    {
                        SendMouseDown(PressOrRelease(key, flag)); internalList[key] = true;
                    }
                    if (flag is false && internalList[key] is true)
                    {
                        SendMouseUp(PressOrRelease(key, flag)); internalList[key] = false;
                    }
                }
                else
                {
                    string key = "combine";
                    foreach (var item in keys) { key = $"{key}_{item}"; }

                    if (internalListEx.ContainsKey(key) is false) { internalListEx.Add(key, true); }

                    if (flag is true && internalListEx[key] is false)
                    {
                        foreach (var item in keys) { SendMouseDown(PressOrRelease((int)item, flag)); }
                        internalListEx[key] = true;
                    }

                    if (flag is false && internalListEx[key] is true)
                    {
                        foreach (var item in keys) { SendMouseUp(PressOrRelease((int)item, flag)); }
                        internalListEx[key] = false;
                    }
                }
            }

        }

        //键盘
        private partial class KeyBoardProxy
        {
            private static Dictionary<short, bool> internalKeyList = new() { };
            private static Dictionary<short, InnerTimer> timerList = new() { };
            private static Dictionary<short, InnerTimer> timerList2 = new() { };

            private static Dictionary<string, bool> internalListEx = new() { };

            private static readonly object objlock = new object();
            private static KeyBoardProxy? _instance;
            public static KeyBoardProxy Instance
            {
                get
                {
                    lock (objlock)
                    {
                        if (_instance is null) { _instance = new(); }
                    }
                    return _instance;
                }
            }

            static KeyBoardProxy()
            {
                foreach (var item in Enum.GetValues(typeof(System.Windows.Forms.Keys)).Cast<System.Windows.Forms.Keys>().ToList())
                {
                    if (internalKeyList.ContainsKey((short)item) is false)
                    {
                        internalKeyList.Add((short)item, false);
                        timerList.Add((short)item, new());
                        timerList2.Add((short)item, new());
                    }
                }
            }

            private static void SendKeyDown(short key)
            {
                INPUT[] inputs_key = new INPUT[]
                {
                    //KeyDown
                    new INPUT {
                        type = INPUT_KEYBOARD,
                        ui = new INPUT_UNION
                        {
                            keyboard = new KEYBDINPUT
                            {
                                wVk = key,
                                wScan = (short)MapVirtualKey((uint)key, 0),
                                dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN,
                                dwExtraInfo = IntPtr.Zero,
                                time = 0
                            }
                        }
                    },
                };
                PreSendInput(ref inputs_key);
            }
            private static void SendKeyUp(short key)
            {
                INPUT[] inputs_key = new INPUT[]
                {
                    //KeyUp
                    new INPUT {
                        type = INPUT_KEYBOARD,
                        ui = new INPUT_UNION
                        {
                            keyboard = new KEYBDINPUT
                            {
                                wVk = key,
                                wScan = (short)MapVirtualKey((uint)key, 0),
                                dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                                dwExtraInfo = IntPtr.Zero,
                                time = 0
                            }
                        }
                    }
                };
                PreSendInput(ref inputs_key);
            }

            public void Update(System.Windows.Forms.Keys[] keys, bool flag, bool autorelease = false)
            {
                if (keys.Length == 1)
                {
                    short key = (short)keys[0];
                    if (flag is true && internalKeyList[key] is false)
                    {
                        SendKeyDown(key); internalKeyList[key] = true;
                    }

                    if (flag is false && internalKeyList[key] is true)
                    {
                        SendKeyUp(key); internalKeyList[key] = false;

                        //复位
                        timerList[key].Reset();
                        timerList2[key].Reset();
                        return;
                    }

                    //长按???毫秒
                    if (autorelease && flag is true && internalKeyList[key] is true && timerList[key].CoolDown(384))
                    {
                        SendKeyUp(key);
                        //弹起??ms
                        if (timerList2[key].CoolDown(64))
                        {
                            internalKeyList[key] = false;
                            timerList2[key].Reset();
                        }
                    }

                }
                else if (keys.Length > 1)
                {
                    string key = "combine";
                    foreach (var item in keys) { key = $"{key}_{item}"; }

                    if (internalListEx.ContainsKey(key) is false) { internalListEx.Add(key, false); }

                    if (flag is true && internalListEx[key] is false)
                    {
                        foreach (var item in keys) { SendKeyDown((short)item); }
                        internalListEx[key] = true;
                    }

                    if (flag is false && internalListEx[key] is true)
                    {
                        foreach (var item in keys) { SendKeyUp((short)item); }
                        internalListEx[key] = false;
                    }
                }
            }
        }
    }


    //单击鼠标或键盘（供外部调用）
    static partial class SendKBMInput
    {
        private static bool mouse_task_is_running = false;
        private static bool keyboard_task_is_running = false;

        /// <summary>
        /// <para>鼠标左键单击</para>
        /// <para>参数 pt：绝对坐标</para>
        /// </summary>
        public static void SendMouseLeftClick(Point pt)
        {
            if (mouse_task_is_running is false)
            {
                mouse_task_is_running = true;
                {
                    Win32Point mousePosition = new Win32Point
                    {
                        X = (int)(pt.X * ((double)65535 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width)),
                        Y = (int)(pt.Y * ((double)65535 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height))
                    };

                    //Move
                    INPUT[] inputs_move = new INPUT[] {
                        new INPUT {
                            type = INPUT_MOUSE,
                            ui = new INPUT_UNION {
                                mouse = new MOUSEINPUT {
                                    dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE,
                                    dx = mousePosition.X,
                                    dy = mousePosition.Y,
                                    mouseData = 0,
                                    dwExtraInfo = IntPtr.Zero,
                                    time = 0
                                }
                            }
                        }
                    };

                    //Click
                    INPUT[] inputs_click = new INPUT[]
                    {
                        new INPUT {
                            type = INPUT_MOUSE,
                            ui = new INPUT_UNION {
                                mouse = new MOUSEINPUT {
                                    dwFlags = MOUSEEVENTF_LEFTDOWN,
                                    dx = mousePosition.X,
                                    dy = mousePosition.Y,
                                    mouseData = 0,
                                    dwExtraInfo = IntPtr.Zero,
                                    time = 0
                                }
                            }
                        },
                        new INPUT {
                            type = INPUT_MOUSE,
                            ui = new INPUT_UNION {
                                mouse = new MOUSEINPUT {
                                    dwFlags = MOUSEEVENTF_LEFTUP,
                                    dx = mousePosition.X,
                                    dy = mousePosition.Y,
                                    mouseData = 0,
                                    dwExtraInfo = IntPtr.Zero,
                                    time = 0
                                }
                            }
                        }
                    };

                    PreSendInput(ref inputs_move);
                    PreSendInput(ref inputs_click);
                }
                mouse_task_is_running = false;
            }
        }

        /// <summary>
        /// <para>键盘某键按下&amp;弹起</para>
        /// <para>参数 key：类似 (short)WinForm.Keys.A 这样</para>
        /// </summary>
        public static void SendKeyBoardClick(System.Windows.Forms.Keys key)
        {
            if (keyboard_task_is_running is false)
            {
                keyboard_task_is_running = true;
                {
                    INPUT[] inputs_key = new INPUT[] {
                        //KeyDown
                        new INPUT {
                            type = INPUT_KEYBOARD,
                            ui = new INPUT_UNION
                            {
                                keyboard = new KEYBDINPUT
                                {
                                    wVk = (short)key,
                                    wScan = (short)MapVirtualKey((uint)key, 0),
                                    dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN,
                                    dwExtraInfo = IntPtr.Zero,
                                    time = 0
                                }
                            }
                        },
                        //KeyUp
                        new INPUT {
                            type = INPUT_KEYBOARD,
                            ui = new INPUT_UNION
                            {
                                keyboard = new KEYBDINPUT
                                {
                                    wVk = (short)key,
                                    wScan = (short)MapVirtualKey((uint)key, 0),
                                    dwFlags = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                                    dwExtraInfo = IntPtr.Zero,
                                    time = 0
                                }
                            }
                        }
                    };

                    PreSendInput(ref inputs_key);
                }
                keyboard_task_is_running = false;
            }
        }
    }

    //长按鼠标或键盘（供外部调用）
    static partial class SendKBMInput
    {
        /// <summary>
        /// <para>鼠标移动</para>
        /// <para>参数 point：绝对坐标</para>
        /// </summary>
        public static void MouseMove(Point point)
        {
            MouseProxy.Instance.Update(point);
        }


        /// <summary>
        /// <para>鼠标某键按下</para>
        /// <para>参数 flag：true按下 false弹起</para>
        /// <para>参数 keys：一次性按多个键，Key数组</para>
        /// </summary>
        public static void MouseDown(bool flag, params MouseKeys[] keys)
        {
            MouseProxy.Instance.Update(keys, flag);
        }


        /// <summary>
        /// <para>键盘某键按下</para>
        /// <para>参数 flag：true按下 false弹起</para>
        /// <para>参数 keys：一次性按多个键，Key数组（比如填'<see cref="System.Windows.Forms.Keys.Enter"/>'）</para>
        /// </summary>
        public static void KeyDown(bool flag, params System.Windows.Forms.Keys[] keys)
        {
            KeyBoardProxy.Instance.Update(keys, flag);
        }
        /// <summary>
        /// <para>键盘某键按下，长按后激活连发（组合键不激活连发）</para>
        /// <para>参数 flag：true按下 false弹起</para>
        /// <para>参数 keys：一次性按多个键，Key数组（比如填'<see cref="System.Windows.Forms.Keys.Enter"/>'）</para>
        /// </summary>
        public static void KeyDownEx(bool flag, params System.Windows.Forms.Keys[] keys)
        {
            KeyBoardProxy.Instance.Update(keys, flag, true);
        }
    }


    //计时器
    static partial class SendKBMInput
    {
        public sealed class InnerTimer
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token;

            DateTime starttime;

            bool locker = false;
            bool flag = false;


            public void Reset()
            {
                if (locker) { tokenSource.Cancel(); }

                locker = flag = false;
            }

            /// <summary>
            /// 长按时间小于_threshold返回false，否则返回true
            /// </summary>
            public bool CoolDown(int _threshold)
            {
                if (locker is false)
                {
                    locker = true;
                    flag = false;

                    tokenSource = new();
                    token = tokenSource.Token;

                    starttime = DateTime.Now;

                    Task.Run(async () =>
                    {
                        int countdownDuration = _threshold; // 
                        int interval = 24; // 

                        Func<bool> timeout = () => { return ((DateTime.Now).Subtract(starttime).TotalMilliseconds >= countdownDuration); };

                        while (timeout() is false)
                        {
                            // 是否取消
                            if (token.IsCancellationRequested) { break; }

                            // 延迟interval毫秒
                            await Task.Delay(interval);
                        }

                        flag = (token.IsCancellationRequested is false);

                    }, token).ConfigureAwait(false);

                    if (token.IsCancellationRequested)
                    {
                        locker = flag = false;
                    }
                }

                return flag;
            }
        }
    }
}

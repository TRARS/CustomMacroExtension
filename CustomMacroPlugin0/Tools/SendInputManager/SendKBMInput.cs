using CustomMacroBase.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CustomMacroPlugin0.Tools.StaticManager
{
    //引用dll部分
    partial class SendKBMInput
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
    partial class SendKBMInput
    {
        //鼠标
        private partial class MouseProxy
        {
            private Dictionary<int, bool> internalList = new() { };
            private Dictionary<string, bool> internalListEx = new() { };

            private static readonly Lazy<MouseProxy> lazyObject = new(() => new MouseProxy());
            public static MouseProxy Instance => lazyObject.Value;

            private MouseProxy()
            {
                foreach (var item in Enum.GetValues(typeof(MouseKeys)).Cast<MouseKeys>().ToList())
                {
                    if (internalList.ContainsKey((short)item) is false) internalList.Add((short)item, false);
                }
            }

            private void SendMouseMove(Point pt)
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
            private void SendMouseDown(int key)
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
            private void SendMouseUp(int key)
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

            private int PressOrRelease(int key, bool flag)
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
            private Dictionary<short, KeyStateMachine> internalKeyStateMachineList = new() { };
            private Dictionary<string, KeyStateMachine> internalCombineKeyStateMachineList = new() { };
            private int cycleActivationDuration;
            private int cycleDuration;

            private static readonly Lazy<KeyBoardProxy> lazyObject = new(() => new KeyBoardProxy());
            public static KeyBoardProxy Instance => lazyObject.Value;

            private KeyBoardProxy()
            {
                foreach (var item in Enum.GetValues(typeof(Keys)).Cast<Keys>().ToList())
                {
                    internalKeyStateMachineList.TryAdd((short)item, new KeyStateMachine(new Keys[] { item }, SendKeyUp, SendKeyDown));
                }
            }

            private void SendKeyDown(short key)
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
            private void SendKeyUp(short key)
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

            public void Update(Keys[] keys, bool flag, bool autorelease = false)
            {
                if (keys.Length == 1)
                {
                    var key = keys[0];
                    var target = internalKeyStateMachineList[(short)key];
                    target?.SetCycleOptions(cycleActivationDuration, cycleDuration);
                    target?.Update(flag ? KeyFlag.Press : KeyFlag.Release, autorelease);
                }

                if (keys.Length > 1)
                {
                    string key = "Combine";
                    foreach (var item in keys) { key = $"{key}_{item}"; }
                    internalCombineKeyStateMachineList.TryAdd(key, new(keys, SendKeyUp, SendKeyDown));
                    var target = new KeyStateMachine(keys, SendKeyUp, SendKeyDown);
                    target?.Update(flag ? KeyFlag.Press : KeyFlag.Release, false);
                }
            }
            public void Options(int _cycleActivationDuration, int _cycleDuration)
            {
                cycleActivationDuration = _cycleActivationDuration;
                cycleDuration = _cycleDuration;
            }
        }
    }


    //单击鼠标或键盘（供外部调用）
    partial class SendKBMInput
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
    partial class SendKBMInput
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
        public static void KeyDown(bool flag, params Keys[] keys)
        {
            KeyBoardProxy.Instance.Update(keys, flag);
        }
        /// <summary>
        /// <para>键盘某键按下，长按后激活连发（组合键不激活连发）</para>
        /// <para>参数 flag：true按下 false弹起</para>
        /// <para>参数 keys：一次性按多个键，Key数组（比如填'<see cref="System.Windows.Forms.Keys.Enter"/>'）</para>
        /// </summary>
        public static void KeyDownEx(bool flag, params Keys[] keys)
        {
            KeyBoardProxy.Instance.Update(keys, flag, true);
        }

        /// <summary>
        /// 设置连发相关延时
        /// </summary>
        public static void KeyCycleOptions(int cycleActivationDuration, int cycleDuration)
        {
            KeyBoardProxy.Instance.Options(cycleActivationDuration, cycleDuration);
        }
    }


    //计时器
    partial class SendKBMInput
    {
        private sealed class InnerTimer
        {
            bool stop_condition = false;

            bool task_locker = false;
            bool flag = false;

            Func<DateTime, DateTime, int, bool> timeout = (a, b, c) => ((b).Subtract(a).TotalMilliseconds >= c);

            public void Reset()
            {
                if (stop_condition is false) { stop_condition = true; }
                if (task_locker) { task_locker = false; }
                if (flag) { flag = false; }
            }

            /// <summary>
            /// 长按时间小于_threshold返回false，否则返回true
            /// </summary>
            public bool CoolDown(int _threshold)
            {
                if (task_locker is false && flag is false)
                {
                    stop_condition = false;

                    var starttime = DateTime.Now; //计时开始

                    Task.Run(async () =>
                    {
                        task_locker = true;
                        {
                            await Task.Yield();

                            while (timeout.Invoke(starttime, DateTime.Now, _threshold) is false && stop_condition is false)
                            {
                                await Task.Delay(32);// 延迟设置较低，不需要传入token来取消
                            }

                            flag = (stop_condition is false); //true
                        }
                        task_locker = false;
                    }).ConfigureAwait(false);
                }

                return flag;
            }
        }
    }

    //按键状态机
    partial class SendKBMInput
    {
        private interface KeyState
        {
            abstract Action? NextState { get; init; }
            abstract void Press(Keys[] keys);
            abstract void Release(Keys[] keys);
        }
        private class KeyStatePressed : KeyState
        {
            public Action? NextState { get; init; }
            public Action<short>? SendKeyUp { get; init; }
            public void Press(Keys[] keys) { }
            public void Release(Keys[] keys)
            {
                //弹起
                foreach (var key in keys)
                {
                    SendKeyUp?.Invoke((short)key);
                    Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, $"Release :{key}");
                }

                //状态转换为已弹起
                NextState?.Invoke();
            }
        }
        private class KeyStateReleased : KeyState
        {
            public Action? NextState { get; init; }
            public Action<short>? SendKeyDown { get; init; }
            public void Press(Keys[] keys)
            {
                //按下
                foreach (var key in keys)
                {
                    SendKeyDown?.Invoke((short)key);
                    Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, $"Press :{key}");
                }

                //状态转换为已按下
                NextState?.Invoke();
            }
            public void Release(Keys[] keys) { }
        }

        private enum KeyFlag
        {
            Press,
            Release,
        }

        private sealed class KeyStateMachine
        {
            private readonly InnerTimer innerTimer = new();//计时器安排个

            private bool task_locker = false;
            private CancellationTokenSource? task_cts = null;
            private bool task_cts_is_disposed;

            private bool auto_cycle_flag = false;
            private int auto_cycle_duration = 0;
            private int auto_cycle_activation_duration = 0;

            private Keys[] currentKeys = new Keys[0];
            private KeyState? currentState = null;
            private readonly KeyState? pressed = null;
            private readonly KeyState? released = null;


            public KeyStateMachine(Keys[] keys, Action<short> keyup, Action<short> keydown)
            {
                pressed = new KeyStatePressed() { NextState = () => { this.currentState = released; }, SendKeyUp = keyup };
                released = new KeyStateReleased() { NextState = () => { this.currentState = pressed; }, SendKeyDown = keydown };

                currentKeys = keys;
                currentState = released;//默认状态已弹起
            }
            public void SetCycleOptions(int cycleActivationDuration, int cycleDuration)
            {
                auto_cycle_activation_duration = cycleActivationDuration;
                auto_cycle_duration = cycleDuration;
            }
            public void Update(KeyFlag flag, bool auto_cycle_onoff)
            {
                //长按后激活循环
                if (auto_cycle_onoff)
                {
                    if (flag == KeyFlag.Press && innerTimer.CoolDown(auto_cycle_activation_duration))
                    {
                        auto_cycle_flag = true; PressReleaseCycle(); return;
                    }
                    if (flag == KeyFlag.Release)
                    {
                        auto_cycle_flag = false; PressReleaseCycle(); innerTimer.Reset();
                    }
                }

                //常规按下弹起
                switch (flag)
                {
                    case KeyFlag.Press:
                        currentState?.Press(currentKeys); break;
                    case KeyFlag.Release:
                        currentState?.Release(currentKeys); break;
                    default: break;
                }
            }

            //
            private void PressReleaseCycle()
            {
                if (auto_cycle_flag is false)
                {
                    if (task_cts_is_disposed is false) { task_cts?.Cancel(); }
                    task_locker = false;
                    return;
                }

                if (task_locker is false)
                {
                    task_locker = true;

                    Task.Run(async () =>
                    {
                        await Task.Yield();

                        using (task_cts = new())
                        {
                            task_cts_is_disposed = false;
                            {
                                var token = task_cts.Token;
                                try
                                {
                                    do
                                    {
                                        currentState?.Release(currentKeys);
                                        await Task.Delay(auto_cycle_duration, token);
                                        currentState?.Press(currentKeys);
                                        await Task.Delay(auto_cycle_duration, token);
                                    }
                                    while (auto_cycle_flag && token.IsCancellationRequested is false);
                                }
                                catch { }
                                finally { currentState?.Release(currentKeys); }
                            }
                            task_cts_is_disposed = true;
                        }
                    }).ContinueWith(_ => { task_locker = false; }).ConfigureAwait(false);
                }
            }
        }
    }
}

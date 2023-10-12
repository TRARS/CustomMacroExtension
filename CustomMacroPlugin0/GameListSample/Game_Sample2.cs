using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroPlugin0.Tools.TimeManager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//This sample adds two Toggle buttons on the UI interface,
//each button corresponds to a complete implementation of a macro,
//but it is too complex to be practical.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(202)]
    //[DoNotLoad]
    partial class Game_Sample2 : MacroBase
    {
        readonly CooldownTimer CT = new();//计时器

        public override void Init()
        {
            MainGate.Text = "Sample2_主开关，均为长按×激活脚本，松开×中断脚本";

            MainGate.Add(new() { GroupName = "u^(#mVm4", Text = "子开关0，○×□△(可循环)", Enable = true });
            MainGate.Add(new() { GroupName = "u^(#mVm4", Text = "子开关1，双摇杆画圆(可循环)", Enable = false });
            //MainGate.Add(new() { GroupName = "u^(#mVm4", Text = "子开关2，yield 测试", Enable = true });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
            //if (MainGate[2].Enable) { Macro2(); }
        }
    }

    //Macro0
    partial class Game_Sample2
    {
        List<dynamic>? macro0_actioninfo_list = null;

        bool macro0_start_condition = false;
        bool macro0_stop_condition = false;
        bool macro0_repeat_condition = false;

        bool macro0_task_locker = false;
        bool macro0_task_is_running = false;
        bool macro0_task_cancelflag = false;

        Action? macro0_act_pre = null;
        Action? macro0_act = null;

        private void Macro0()
        {
            macro0_start_condition = (RealDS4.Cross && CT["lR^p#rx7"].Elapsed(1000));//(真实手柄)长按×激活脚本
            macro0_stop_condition = (RealDS4.Cross is false);//(真实手柄)松开×中断脚本
            macro0_repeat_condition = true;//使得脚本可以循环

            macro0_act_pre ??= () => { VirtualDS4.Cross = false; };//初始化macro0_act_pre，使得脚本执行期间(真实手柄)的Cross不会影响(虚拟手柄)的Cross
            macro0_actioninfo_list ??= new() //初始化macro0_actioninfo_list
            {
                new { Action = (Action)(()=>{ VirtualDS4.DpadRight = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.DpadDown = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.DpadLeft = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.DpadUp = true; }), Duration = 200 },

                new { Action = (Action)(()=>{ VirtualDS4.Circle = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.Cross = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.Square = true; }), Duration = 200 },
                new { Action = (Action)(()=>{ VirtualDS4.Triangle = true; }), Duration = 200 },
            };

            if (macro0_stop_condition)
            {
                if (macro0_task_is_running is false) { macro0_task_locker = false; return; }
                macro0_task_cancelflag = true;
            };

            if (macro0_start_condition)
            {
                if (macro0_task_locker is false && macro0_task_is_running is false)
                {
                    macro0_task_locker = true;//上锁
                    macro0_task_cancelflag = false;

                    Task.Run(() =>
                    {
                        Print($"Macro0 Start");
                        {
                            macro0_task_is_running = true;//二次上锁
                            {
                                do
                                {
                                    if (macro0_actioninfo_list is not null)
                                    {
                                        foreach (var item in macro0_actioninfo_list)
                                        {
                                            if (macro0_task_cancelflag) { break; }

                                            macro0_act = item.Action;
                                            Task.Delay(item.Duration).Wait();
                                        }
                                    }
                                }
                                while (macro0_repeat_condition && (macro0_task_cancelflag is false));
                            }
                            macro0_task_is_running = false;
                        }
                        Print($"Macro0 End");
                    });
                }
            }
            else
            {
                macro0_task_locker = false;//锁1复位
            }

            if (macro0_task_is_running && macro0_task_cancelflag is false)
            {
                macro0_act_pre?.Invoke();
                macro0_act?.Invoke();
            }
        }
    }

    //Macro1
    partial class Game_Sample2
    {
        bool macro1_start_condition = false;
        bool macro1_stop_condition = false;
        bool macro1_repeat_condition = false;

        bool macro1_task_locker = false;
        bool macro1_task_is_running = false;
        bool macro1_task_cancelflag = false;

        Action? macro1_act_pre = null;
        Action? macro1_act = null;

        private void Macro1()
        {
            macro1_start_condition = (RealDS4.Cross && CT["dJ@X@4q*"].Elapsed(1000));//(真实手柄)长按×激活脚本
            macro1_stop_condition = (RealDS4.Cross is false);//(真实手柄)松开×中断脚本
            macro1_repeat_condition = true;//使得脚本可以循环

            macro1_act_pre ??= () => { VirtualDS4.Cross = false; };//初始化macro_act_pre，使得脚本执行期间(真实手柄)的Cross不会影响(虚拟手柄)的Cross

            if (macro1_stop_condition)
            {
                if (macro1_task_is_running is false) { macro1_task_locker = false; return; }
                macro1_task_cancelflag = true;
            };

            if (macro1_start_condition)
            {
                if (macro1_task_locker is false && macro1_task_is_running is false)
                {
                    macro1_task_locker = true;//上锁
                    macro1_task_cancelflag = false;

                    Task.Run(() =>
                    {
                        Print($"Macro1 Start");
                        {
                            macro1_task_is_running = true;//二次上锁
                            {
                                do
                                {
                                    if (true)
                                    {
                                        double size = 45;
                                        double r = (byte.MaxValue / 2);
                                        for (var times = 0; times < size; times++)
                                        {
                                            var radian = (2 * Math.PI / size) * times;
                                            var X = Math.Sin(radian) * r;
                                            var Y = Math.Cos(radian) * r;
                                            var LX = 127 + X;
                                            var LY = 127 - Y;
                                            var RX = byte.MaxValue - (127 + (X * 0.5));
                                            var RY = byte.MaxValue - (127 - (Y * 0.5));

                                            if (macro1_task_cancelflag) { macro1_task_is_running = false; break; }

                                            macro1_act = () =>
                                            {
                                                UpdateNow(btnKey.LX, (byte)LX); UpdateNow(btnKey.LY, (byte)LY);
                                                UpdateNow(btnKey.RX, (byte)RX); UpdateNow(btnKey.RY, (byte)RY);
                                            };
                                            Task.Delay(2).Wait();
                                        }
                                    }
                                }
                                while (macro1_repeat_condition && (macro1_task_cancelflag is false));
                            }
                            macro1_task_is_running = false;
                        }
                        Print($"Macro1 End");
                    });
                }
            }
            else
            {
                macro1_task_locker = false;//锁1复位
            }

            if (macro1_task_is_running && macro1_task_cancelflag is false)
            {
                macro1_act_pre?.Invoke();
                macro1_act?.Invoke();
            }
        }
    }

    //Macro2
    partial class Game_Sample2
    {
        bool _canExecute = true;
        Action? _action = null;

        private void Macro2()
        {
            VirtualDS4.Cross = false;

            if (RealDS4.Cross) { StartCombo(ComboFlow()); }

            _action?.Invoke();
        }
        private async IAsyncEnumerable<string> ComboFlow()
        {
            if (_canExecute is false) { yield break; }

            _canExecute = false;
            for (int i = 0; i < 3; i++)
            {
                _action = () => { VirtualDS4.Square = true; VirtualDS4.Circle = false; }; yield return "press Square"; await Task.Delay(50);
                _action = () => { VirtualDS4.Square = false; VirtualDS4.Circle = false; }; yield return "release Square"; await Task.Delay(50);
                _action = () => { VirtualDS4.Circle = true; VirtualDS4.Square = false; }; yield return "press Circle"; await Task.Delay(50);
                _action = () => { VirtualDS4.Circle = false; VirtualDS4.Square = false; }; yield return "release Circle"; await Task.Delay(50);
                yield return "wait 2 second"; await Task.Delay(2000);
            }
            _action = null;
            _canExecute = true;
        }
        private async void StartCombo(IAsyncEnumerable<string> list)
        {
            await foreach (var result in list)
            {
                Print($"{result}");
            };
        }
    }
}
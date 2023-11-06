# CustomMacroExtension
* This is a macro plugin project only work on the [`custom build of DS4Windows(3.2.19)`](https://github.com/TRARS/DS4Windows)  
* With this project we can focus on writing macros using C#  

## How to use
* To use the macro plugin, place the generated DLL in the same directory as the  DS4Windows.exe file and ensure that the DLL prefix contains "CustomMacroPlugin" (for example, "XXXCustomMacroPluginYYYZZZ.dll")  

## Limitations when creating macro classes
* Ensure that the class inherits from the MacroBase class  
* Ensure that the class name contains "Game_" (for example, "XXXGame_YYYZZZ")  
* Ensure that the namespace of the class contains "CustomMacroPlugin" (for example,  "XXX.CustomMacroPluginYYY.ZZZ")  

## Refer to the following code examples in order to understand how to write macros
<details><summary>Game_Sample0.cs(Blank Template)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample only adds three Toggle buttons on the UI.
//It does not implement macros,
//but only shows how to add Toggle buttons.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(200)]
    [DoNotLoad]
    partial class Game_Sample0 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Sample0_主开关";

            MainGate.Add(new() { Text = "子开关0", Enable = true });
            MainGate.Add(new() { Text = "子开关1", Enable = true });
            MainGate.Add(new() { Text = "子开关2", Enable = true });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { }
            if (MainGate[1].Enable) { }
            if (MainGate[2].Enable) { }
        }
    }
}
```
</details>

<details><summary>Game_Sample1.cs(Simple Remapping)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample adds four Toggle buttons on the UI,
//each button corresponds to a remapping,
//but it does not implement macros and is not practical.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(201)]
    //[DoNotLoad]
    partial class Game_Sample1 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Sample1_主开关，均为按○激活脚本，松开○中断脚本";

            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关0，○×", Enable = true });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关1，×", Enable = true });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关2，□以及推左摇杆", Enable = false });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关3，○×□△", Enable = false });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
            if (MainGate[2].Enable) { Macro2(); }
            if (MainGate[3].Enable) { Macro3(); }
        }
    }

    partial class Game_Sample1
    {
        private void Macro0()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Cross = true; //操作(虚拟手柄)按下×
            }
            //最终效果：(虚拟手柄)同时按下○×
        }

        private void Macro1()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Circle = false;//操作(虚拟手柄)弹起○
                VirtualDS4.Cross = true;//操作(虚拟手柄)按下×
            }
            //最终效果：(虚拟手柄)按下×
        }

        private void Macro2()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Circle = false;//操作(虚拟手柄)弹起○
                VirtualDS4.LX = 0;
                VirtualDS4.LY = 0;//操作(虚拟手柄)左摇杆
                VirtualDS4.Square = true;//操作(虚拟手柄)按下□
            }
            //最终效果：(虚拟手柄)按下□的同时左摇杆往左上角推
        }

        private void Macro3()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Cross = true;//操作(虚拟手柄)按下×
                VirtualDS4.Square = true;//操作(虚拟手柄)按下□
                VirtualDS4.Triangle = true;//操作(虚拟手柄)按下△
            }
            //最终效果：(虚拟手柄)同时按下○×□△
        }
    }
}
```
</details>

<details><summary>Game_Sample2.cs(One-click Combo)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroPlugin0.Tools.TimeManager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//This sample adds two Toggle buttons on the UI,
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
```
</details>
<details><summary>Game_Sample3.cs(Simplify One-click Combo with FlowControllerV0/V1/V2 class)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroPlugin0.Tools.FlowManager;
using CustomMacroPlugin0.Tools.TimeManager;
using System;
using System.Collections.Generic;

//This sample adds three Toggle buttons on the UI,
//each button corresponds to a macro and the writing process has been simplified.
//It is already practical.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(203)]
    //[DoNotLoad]
    partial class Game_Sample3 : MacroBase
    {
        readonly StopwatchTask StopwatchTask = new();

        public override void Init()
        {
            MainGate.Text = "Sample3_主开关，均为按□激活脚本，按○中断脚本";

            MainGate.Add(new() { GroupName = "jmB!$h@T", Text = "神躯化剑V0", Enable = true });
            MainGate.Add(new() { GroupName = "jmB!$h@T", Text = "神躯化剑V1", Enable = true });
            MainGate.Add(new() { GroupName = "jmB!$h@T", Text = "神躯化剑V2", Enable = true });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
            if (MainGate[2].Enable) { Macro2(); }
        }
    }

    //神躯化剑V0——脚本执行时，可以手动操作其他按键
    partial class Game_Sample3
    {
        FlowControllerV0 Macro0_Flow = new("神躯化剑V0", () => { VirtualDS4.Square = false; }) //脚本执行期间，先弹起□，再执行脚本具体动作
        {
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },2200),//跑路
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 16; VirtualDS4.LY = 0; },1400),//跑路
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },100),//跑路

            new(()=>{ VirtualDS4.L2 = 255; },5500),//出招收菜
            new(()=>{ VirtualDS4.OutputTouchButton = true;},400),//按触摸板，弹出地图
            new(()=>{ VirtualDS4.Triangle = true;},200),//按△，光标复位
            new(()=>{ VirtualDS4.Cross = true;},100),//按×，确定
            new(()=>{ VirtualDS4.Cross = false;},100),//弹起×，避开按键粘滞
            new(()=>{ VirtualDS4.Cross = true;},100),//再按×，确定

            new(4600),//从读条到人物起身大概5秒
        };

        private void Macro0()
        {
            Macro0_Flow.Start_Condition = RealDS4.Square;//按一下□以激活脚本
            Macro0_Flow.Stop_Condition = RealDS4.Circle;//按一下○使脚本停止
            Macro0_Flow.Repeat_Condition = true;//使脚本可循环
            Macro0_Flow.ExecuteMacro();
        }
    }

    //神躯化剑V1——脚本执行时，无法手动操作任何按键
    partial class Game_Sample3
    {
        FlowControllerV1 Macro1_Flow = new("神躯化剑V1")
        {
            new(btnKey.Circle,true),
            new(btnKey.LX,72), new(btnKey.LY,0), new(2200),//跑路
            new(btnKey.LX,16), new(btnKey.LY,0), new(1400),//跑路
            new(btnKey.LX,72), new(btnKey.LY,0), new(100),//跑路
            new(btnKey.Circle, false), new(btnKey.LX, 128), new(btnKey.LY, 128),//※与V0不同，需要自行恢复按键默认状态

            new(btnKey.L2,255,5500),//出招收菜
            new(btnKey.L2,0),//※与V0不同，需要自行恢复按键默认状态

            new(btnKey.OutputTouchButton,true,400),//按触摸板，弹出地图
            new(btnKey.OutputTouchButton,false),//※与V0不同，需要自行恢复按键默认状态

            new(btnKey.Triangle,true,200),//按△，光标复位
            new(btnKey.Triangle,false),//※与V0不同，需要自行恢复按键默认状态

            new(btnKey.Cross,true,100),//按×，确定
            new(btnKey.Cross,false,100),//弹起×，避开按键粘滞
            new(btnKey.Cross,true,100),//再按×，确定
            new(btnKey.Cross,false),//※与V0不同，需要自行恢复按键默认状态

            new(4600),//从读条到人物起身大概5秒
        };

        private void Macro1()
        {
            Macro1_Flow.Start_Condition = RealDS4.Square;//按一下□以激活脚本
            Macro1_Flow.Stop_Condition = RealDS4.Circle;//按一下○使脚本停止
            Macro1_Flow.Repeat_Condition = true;//使脚本可循环
            Macro1_Flow.ExecuteMacro(VirtualDS4);//※与V0不同，需要将VirtualDS4作参数传进去
        }
    }

    //神躯化剑V2——脚本执行时，可以手动操作其他按键
    partial class Game_Sample3
    {
        FlowControllerV2? Macro2_Flow;

        private void Macro2()
        {
            Macro2_Flow ??= new("神躯化剑V2", () => { VirtualDS4.Square = false; })
            {
                (x, y, z) => { Macro2_Detail(ref x[0], ref y[0], ref z); }
            };
            Macro2_Flow.Start_Condition = RealDS4.Square;//按一下□以激活脚本
            Macro2_Flow.Stop_Condition = RealDS4.Circle;//按一下○使脚本停止
            Macro2_Flow.ExecuteMacro();
        }

        private void Macro2_Detail(ref Action _action, ref bool _cancel, ref Func<int, bool> _wait)
        {
            var wait = _wait;//使用内置延时方法

            int pRunes = 0;//储存卢恩数量
            Dictionary<Action, int> ActionList = new()
            {
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },2200},//跑路
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 16; VirtualDS4.LY = 0; },1400},//跑路
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },100},//跑路

                {() => { VirtualDS4.L2 = 255; },5500},//出招收菜
                {() => { VirtualDS4.OutputTouchButton = true; },400},//按触摸板，弹出地图
                {() => { VirtualDS4.Triangle = true; },200},//按△，光标复位
                {() => { VirtualDS4.Cross = true; },100},//按×，确定
                {() => { VirtualDS4.Cross = false; },100},//弹起×，避开按键粘滞
                {() => { VirtualDS4.Cross = true; },100},//再按×，确定

                {() => { },4600 }
            };

            while (_cancel is false)
            {
                //另起一个线程，用来做数字识别
                StopwatchTask["2scGb%&p"].Run((sw) =>
                {
                    var cancel_a_lengthy_delay = wait(2800);//等待数字停止跳动
                    if (cancel_a_lengthy_delay) { return; }

                    sw.Restart();
                    {
                        if (int.TryParse(FindNumber(new(1730, 1020, 130, 24)), out int cRunes))//获取数字
                        {
                            Print($"Runes: {cRunes} (+{cRunes - pRunes}) -> ({sw.ElapsedMilliseconds}ms)");
                            pRunes = cRunes;
                        }
                        else { Print($"Runes: Error"); }
                    }
                    sw.Stop();
                });

                //按顺序执行动作
                foreach (var item in ActionList)
                {
                    if (_cancel) { return; }

                    _action = item.Key; wait(item.Value);
                }
            }
        }

        //private void wait(int t) => Task.Delay(t).Wait();
    }
}
```
</details>

## For more detail about writing macro
<details>
<summary>----------</summary>

<strong>Some tips:</strong><br>
* The RealDS4 object and the VirtualDS4 object are both different copies of the same object, with the difference being that only the VirtualDS4 object is returned to DS4Windows.<br>
* The only thing we need to do is to check the properties of the RealDS4 object and operate on the properties of the VirtualDS4 object.<br>
* Using FlowControllerV0/V1/V2 classes can simplify macro programming.<br>

<strong>The FlowControllerV0/V1/V2 classes have the following differences:</strong><br>
* <strong>V0:</strong> Allows us to control other buttons that are not used in the macro while the macro is running.<br>
* <strong>V1:</strong> Does not allow us to control any button while the macro is running.<br>
* <strong>V2:</strong> Same as V0, but allows us to create macros with logical conditions. For example, it can utilize the FindColor/FindImage methods, which are encapsulated from OpenCvSharp, to assist in macro programming.<br>
※ Drag the crosshair to the target window before using FindXXX method.

<strong>Several important properties of the FlowControllerV0/V1/V2 class include:</strong><br>
* <strong>Start_Condition:</strong> If true, the macro will be executed at least once only if it is not already running.<br>
* <strong>Stop_Condition:</strong> If true, the macro will be terminated if it is already running.<br>
* <strong>Repeat_Condition:</strong> If true, the macro will repeat indefinitely if it is already running.<br>
※ Among these properties, the Stop_Condition has the highest priority.<br>
※ Note that FlowControllerV2 class doesn't have "Repeat_Condition" property. Looping and exiting logic for a macro must be controlled manually in the code.
<br>
</details>

## Other considerations
※Don't forget to use [`HidHide`](https://github.com/ViGEm/HidHide/releases) to hide the real controller<br>
※Check if macros working fine on [`gamepad-tester.com`](https://gamepad-tester.com/)<br>


## PNG TEST
![Image text](https://i.imgur.com/Hw3LKqU.png)
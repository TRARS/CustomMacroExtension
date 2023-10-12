using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroPlugin0.Tools.FlowManager;
using CustomMacroPlugin0.Tools.TimeManager;
using System;
using System.Collections.Generic;

//This sample adds three Toggle buttons on the UI interface,
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
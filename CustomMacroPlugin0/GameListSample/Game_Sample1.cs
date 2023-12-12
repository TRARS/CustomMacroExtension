using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Tools.FlowManager;
using CustomMacroBase.Helper.Tools.TimeManager;
using System;
using System.Collections.Generic;

//This sample introduces three Toggle buttons to the UI, with each button corresponding to a distinct macro.
//This simplifies the process of macro creation, making it more user-friendly.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(201)]
    partial class Game_Sample1 : MacroBase
    {
        readonly StopwatchTask StopwatchTask = new();

        public override void Init()
        {
            MainGate.Text = "Slain Albinauric mobs. All macros are activated by pressing □ and termination upon pressing ○";

            MainGate.Add(CreateGateBase("Sacred Relic Sword V0", groupName: "jmB!$h@T"));
            MainGate.Add(CreateGateBase("Sacred Relic Sword V1", groupName: "jmB!$h@T"));
            MainGate.Add(CreateGateBase("Sacred Relic Sword V2", groupName: "jmB!$h@T"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
            if (MainGate[2].Enable) { Macro2(); }
        }
    }

    //Sacred Relic Sword V0 —— During macro execution, we can still operate buttons that are not utilized within the macro.
    partial class Game_Sample1
    {
        FlowControllerV0 Macro0_Flow = new("Sacred Relic Sword V0", () => { VirtualDS4.Square = false; }) //Before each action in the macro is executed, □ will be released.
        {
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },2200),
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 16; VirtualDS4.LY = 0; },1400),
            new(()=>{ VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },100),

            new(()=>{ VirtualDS4.L2 = 255; },5500),
            new(()=>{ VirtualDS4.OutputTouchButton = true;},400),
            new(()=>{ VirtualDS4.Triangle = true;},200),
            new(()=>{ VirtualDS4.Cross = true;},100),
            new(()=>{ VirtualDS4.Cross = false;},100),
            new(()=>{ VirtualDS4.Cross = true;},100),

            new(4600),//Loading time
        };

        private void Macro0()
        {
            Macro0_Flow.Start_Condition = RealDS4.Square;//Activated by pressing □
            Macro0_Flow.Stop_Condition = RealDS4.Circle;//Termination upon pressing ○
            Macro0_Flow.Repeat_Condition = true;//Automatic Loop
            Macro0_Flow.ExecuteMacro();
        }
    }

    //Sacred Relic Sword V1 —— During macro execution, we are unable to operate any buttons.
    partial class Game_Sample1
    {
        FlowControllerV1 Macro1_Flow = new("Sacred Relic Sword V1")
        {
            new(btnKey.Circle,true),
            new(btnKey.LX,72), new(btnKey.LY,0), new(2200),
            new(btnKey.LX,16), new(btnKey.LY,0), new(1400),
            new(btnKey.LX,72), new(btnKey.LY,0), new(100),
            new(btnKey.Circle, false), new(btnKey.LX, 128), new(btnKey.LY, 128),//※ Unlike V0, we need to manually reset the state of the buttons used previously.

            new(btnKey.L2,255,5500),
            new(btnKey.L2,0),//※ Unlike V0, we need to manually reset the state of the buttons used previously

            new(btnKey.OutputTouchButton,true,400),
            new(btnKey.OutputTouchButton,false),//※ Unlike V0, we need to manually reset the state of the buttons used previously

            new(btnKey.Triangle,true,200),
            new(btnKey.Triangle,false),//※ Unlike V0, we need to manually reset the state of the buttons used previously

            new(btnKey.Cross,true,100),
            new(btnKey.Cross,false,100),
            new(btnKey.Cross,true,100),
            new(btnKey.Cross,false),//※ Unlike V0, we need to manually reset the state of the buttons used previously

            new(4600),//Loading time
        };

        private void Macro1()
        {
            Macro1_Flow.Start_Condition = RealDS4.Square;//Activated by pressing □
            Macro1_Flow.Stop_Condition = RealDS4.Circle;//Termination upon pressing ○
            Macro1_Flow.Repeat_Condition = true;//Automatic Loop
            Macro1_Flow.ExecuteMacro(VirtualDS4);//※ Unlike V0, we need to pass VirtualDS4 as a parameter here.
        }
    }

    //Sacred Relic Sword V2 —— During macro execution, we can still operate buttons that are not utilized within the macro.
    partial class Game_Sample1
    {
        FlowControllerV2? Macro2_Flow;

        private void Macro2()
        {
            Macro2_Flow ??= new("Sacred Relic Sword V2", () => { VirtualDS4.Square = false; })
            {
                (x, y, z) => { Macro2_Detail(ref x[0], ref y[0], ref z); }
            };
            Macro2_Flow.Start_Condition = RealDS4.Square;//Activated by pressing □
            Macro2_Flow.Stop_Condition = RealDS4.Circle;//Termination upon pressing ○
            Macro2_Flow.ExecuteMacro();
        }

        private void Macro2_Detail(ref Action _action, ref bool _cancel, ref Func<int, bool> _wait)
        {
            var wait = _wait;

            int pRunes = 0;//Used to keep track of the current number of runes
            Dictionary<Action, int> ActionList = new()
            {
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },2200},
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 16; VirtualDS4.LY = 0; },1400},
                {() => { VirtualDS4.Circle = true; VirtualDS4.LX = 72; VirtualDS4.LY = 0; },100},

                {() => { VirtualDS4.L2 = 255; },5500},
                {() => { VirtualDS4.OutputTouchButton = true; },400},
                {() => { VirtualDS4.Triangle = true; },200},
                {() => { VirtualDS4.Cross = true; },100},
                {() => { VirtualDS4.Cross = false; },100},
                {() => { VirtualDS4.Cross = true; },100},

                {() => { },4600 } //Loading time
            };

            while (_cancel is false)
            {
                //Performing numerical recognition in another thread.
                StopwatchTask["2scGb%&p"].Run((sw) =>
                {
                    var cancel_a_lengthy_delay = wait(2800);//Waiting for the numbers to stop jumping.
                    if (cancel_a_lengthy_delay) { return; }

                    sw.Restart();
                    {
                        if (int.TryParse(FindNumber(new(1730, 1020, 130, 24)), out int cRunes))//Get the number of runes.
                        {
                            Print($"Runes: {cRunes} (+{cRunes - pRunes}) -> ({sw.ElapsedMilliseconds}ms)");
                            pRunes = cRunes;
                        }
                        else { Print($"Runes: Error"); }
                    }
                    sw.Stop();
                });

                //Execute pre-defined actions in sequence.
                foreach (var item in ActionList)
                {
                    if (_cancel) { return; }

                    _action = item.Key; wait(item.Value);
                }
            }
        }
    }
}
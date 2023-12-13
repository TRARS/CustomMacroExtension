# CustomMacroExtension
* This is a macro plugin project only work on the [`custom build of DS4Windows(3.2.20)`](https://github.com/TRARS/DS4Windows)  

## How to use
To employ the macro plugin, follow these steps:
* Position the generated DLL in the identical directory as the DS4Windows.exe.
* Confirm that the DLL prefix contains 'CustomMacroPlugin' (e.g.,  'XXXCustomMacroPluginYYYZZZ.dll').

## Constraints in Macro Class Creation
When crafting macro classes, please observe the following limitations:
* Confirm that the class inherits from the MacroBase class.
* Confirm that the class name includes 'Game_' (e.g.,  'XXXGame_YYYZZZ').
* Confirm that the namespace of the class contains 'CustomMacroPlugin' (e.g., 'XXX.CustomMacroPluginYYY.ZZZ').   

## Explore the Code Examples Below to Master Macro Writing Techniques
<details><summary>Game_Sample0.cs(Blank Template)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample introduces three Toggle buttons to the UI.
//It focuses on demonstrating the addition of Toggle buttons and does not implement macros.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(200)]
    partial class Game_Sample0 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Main_ToggleButton";

            MainGate.Add(CreateGateBase("Sub_ToggleButton_0"));
            MainGate.Add(CreateGateBase("Sub_ToggleButton_1"));
            MainGate.Add(CreateGateBase("Sub_ToggleButton_2"));
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


<details><summary>Game_Sample1.cs(Simplify One-click Combo with the FlowControllerV0/V1/V2 class)</summary>

```csharp
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
            Macro2_Flow ??= new("Sacred Relic Sword V2", () => { VirtualDS4.Square = false; }) //Before each action in the macro is executed, □ will be released.
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
```
</details>


<details><summary>Game_Sample2.cs(How to use the built-in Slider and ComboBox)</summary>

```csharp
using CustomMacroBase;
using CustomMacroBase.Helper;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Tools.FlowManager;
using System;
using System.Collections.ObjectModel;

//This sample illustrates how to use the built-in Slider and ComboBox.
namespace CustomMacroPlugin0.GameListSample
{
    partial class Game_Sample2
    {
        enum ComboBoxEnum
        {
            delay128, delay256, delay512, delay1024,
        }

        class InnerModel
        {
            public double SliderValue = 0;
            public ComboBoxEnum ComboBoxSelectedItem = ComboBoxEnum.delay128;
            public ObservableCollection<string> ComboBoxItemsSource = ConvertEnumToObservableCollection<ComboBoxEnum>();

            private static ObservableCollection<string> ConvertEnumToObservableCollection<T>() where T : Enum
            {
                return new ObservableCollection<string>(Enum.GetNames(typeof(T)));
            }
        }

        class InnerViewModel : NotificationObject
        {
            InnerModel model = new();

            public double SliderValue
            {
                get => model.SliderValue;
                set
                {
                    if (model.SliderValue != value)
                    {
                        model.SliderValue = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }
            public ComboBoxEnum ComboBoxSelectedItem
            {
                get => model.ComboBoxSelectedItem;
                set
                {
                    if (model.ComboBoxSelectedItem != value)
                    {
                        model.ComboBoxSelectedItem = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public ObservableCollection<string> ComboBoxItemsSource
            {
                get => model.ComboBoxItemsSource;
                set
                {
                    if (model.ComboBoxItemsSource != value)
                    {
                        model.ComboBoxItemsSource = value;
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        static InnerViewModel viewmodel = new();
    }

    [SortIndex(202)]
    partial class Game_Sample2 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Slider and ComboBox";

            MainGate.Add(CreateGateBase("hold press □ to observe the delay during rapid firing")); //[0]
            MainGate[0].AddEx(() => CreateSlider(5, 1000, viewmodel, nameof(viewmodel.SliderValue), 1, sliderTextPrefix: $"delay:", defalutValue: 50, sliderTextSuffix: $"ms"));

            MainGate.Add(CreateGateBase("hold press ○ to observe the delay during rapid firing")); //[1]
            MainGate[1].AddEx(() => CreateComboBox(viewmodel, nameof(viewmodel.ComboBoxItemsSource), nameof(viewmodel.ComboBoxSelectedItem), commentText: "ms", defalutIndex: 0));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
        }
    }

    partial class Game_Sample2
    {
        FlowControllerV0 Macro0_Flow = new("Macro0", () => { VirtualDS4.Square = false; })
        {
            new(()=>{ VirtualDS4.Square = true;}, 50),
            new(()=>{ VirtualDS4.Square = false;},()=>(int)viewmodel.SliderValue),
        };

        private void Macro0()
        {
            Macro0_Flow.Start_Condition = RealDS4.Square; //Activated by pressing □
            Macro0_Flow.Stop_Condition = RealDS4.Square is false; //Termination upon releasing □
            Macro0_Flow.Repeat_Condition = true;
            Macro0_Flow.ExecuteMacro();
        }
    }

    partial class Game_Sample2
    {
        FlowControllerV0? Macro1_Flow;

        Func<int> durationfunc = () =>
        {
            switch (viewmodel.ComboBoxSelectedItem)
            {
                case ComboBoxEnum.delay128: return 128;
                case ComboBoxEnum.delay256: return 256;
                case ComboBoxEnum.delay512: return 512;
                case ComboBoxEnum.delay1024: return 1024;
                default: return 1024;
            }
        };

        private void Macro1()
        {
            Macro1_Flow ??= new("Macro1", () => { VirtualDS4.Circle = false; })
            {
                new(() => { VirtualDS4.Circle = true; }, 50),
                new(() => { VirtualDS4.Circle = false; }, durationfunc),
            };

            Macro1_Flow.Start_Condition = RealDS4.Circle;  //Activated by pressing ○
            Macro1_Flow.Stop_Condition = RealDS4.Circle is false;  //Termination upon releasing ○
            Macro1_Flow.Repeat_Condition = true;
            Macro1_Flow.ExecuteMacro();
        }
    }
}
```
</details>

## For more in-depth information on writing macros
<strong>Note:</strong><br>
* Both the RealDS4 and VirtualDS4 objects represent distinct instances of the same object. The key distinction lies in the fact that only the VirtualDS4 object is returned to DS4Windows.<br>
* Our primary task involves inspecting the properties of the RealDS4 object and manipulating the properties of the VirtualDS4 object.<br>
* The utilization of FlowControllerV0/V1/V2 classes has the potential to streamline macro programming.<br>

<strong>The FlowControllerV0/V1/V2 classes exhibit distinct characteristics:</strong><br>
* <strong>V0:</strong> Permits the manipulation of buttons not employed in the running macro.<br>
* <strong>V1:</strong> Restricts the control of any button during the macro execution.<br>
* <strong>V2:</strong> Mirrors V0 functionality, with the added capability to create macros featuring logical conditions. For instance, it can leverage encapsulated methods such as FindColor/FindImage from OpenCvSharp to enhance macro programming.<br>
※ Drag the crosshair to the target window before employing the FindXXX method.

<strong>Several important properties of the FlowControllerV0/V1/V2 class include:</strong><br>
* <strong>Start_Condition:</strong> If set to true, the macro will be executed at least once, but only if it is not already running.<br>
* <strong>Stop_Condition:</strong> If set to true, the macro will be terminated if it is already in progress. This condition takes the highest priority among the listed properties.<br>
* <strong>Repeat_Condition:</strong> If set to true, the macro will repeat indefinitely if it is already running.<br>
※ The FlowControllerV2 class does not feature the "Repeat_Condition" property. Therefore, looping and exiting logic for a macro must be manually controlled within the code.
<br>

## Additional Considerations
※ <strong>HidHide Usage:</strong> Ensure to employ [`HidHide`](https://github.com/ViGEm/HidHide/releases) for concealing the actual controller, enhancing privacy and security.<br>
※ <strong>Macro Functionality Verification:</strong> It is recommended to test the macros on [`gamepad-tester.com`](https://gamepad-tester.com/) to confirm their proper functioning.<br>


## Preview
![Image text](https://i.imgur.com/Hw3LKqU.png)
using CustomMacroBase;
using CustomMacroBase.Helper;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Tools.FlowManager;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
                var enumValues = Enum.GetValues(typeof(T)).Cast<T>();
                var stringValues = enumValues.Select(e => e.ToString());
                var observableCollection = new ObservableCollection<string>(stringValues);

                return observableCollection;
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
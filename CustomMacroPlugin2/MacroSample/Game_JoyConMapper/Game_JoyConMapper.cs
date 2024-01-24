using CustomMacroBase;
using CustomMacroBase.Helper;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Tools.OtherManager;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper
{
    partial class Game_JoyConMapper
    {
        class InnerModel : NotificationObject
        {
            //
            private double _LeftStickDeadZone = 0;
            public double LeftStickDeadZone
            {
                get => _LeftStickDeadZone;
                set
                {
                    if (_LeftStickDeadZone != value)
                    {
                        _LeftStickDeadZone = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }
            private double _LeftStickEnlargementFactor = 0;
            public double LeftStickEnlargementFactor
            {
                get => _LeftStickEnlargementFactor;
                set
                {
                    if (_LeftStickEnlargementFactor != value)
                    {
                        _LeftStickEnlargementFactor = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }
            private double _LeftStickClipRadius = 0;
            public double LeftStickClipRadius
            {
                get => _LeftStickClipRadius;
                set
                {
                    if (_LeftStickClipRadius != value)
                    {
                        _LeftStickClipRadius = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }

            //
            private double _CycleActivationDuration = 0;
            public double CycleActivationDuration
            {
                get => _CycleActivationDuration;
                set
                {
                    if (_CycleActivationDuration != value)
                    {
                        _CycleActivationDuration = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }
            private double _CycleDuration = 0;
            public double CycleDuration
            {
                get => _CycleDuration;
                set
                {
                    if (_CycleDuration != value)
                    {
                        _CycleDuration = Math.Floor(value);
                        NotifyPropertyChanged();
                    }
                }
            }
        }

        InnerModel model = new();
    }

    [SortIndex(-9998)]
    partial class Game_JoyConMapper : MacroBase
    {
        const bool hide_it = true;
        readonly Dictionary<string, bool> debug_flag = new()
        {
            {"LeftStick 8-direction-movement", hide_it},
            {"LeftStick 8-direction-movement slider", hide_it},
            {"LeftStick cycle parameta slider", false},
        };

        public override void Init()
        {
            this.UseColorfulText = true;

            MainGate.Text = Title = "JoyCon(L)";

            MainGate.Add(CreateGateBase("enable mapping left-stick to buttons"));//[0]
            MainGate[0].Add(CreateGateBase("8-direction-movement", hideself: debug_flag["LeftStick 8-direction-movement"]));//[0][0]
            MainGate[0].AddEx(() => CreateSlider(0, 127, model, nameof(model.LeftStickDeadZone), 1, sliderTextPrefix: $"DeadZone:", defalutValue: 30, hideself: debug_flag["LeftStick 8-direction-movement slider"]));
            MainGate[0].AddEx(() => CreateSlider(128, 1280, model, nameof(model.LeftStickEnlargementFactor), 1, sliderTextPrefix: $"EnlargementFactor:", defalutValue: 1280, hideself: debug_flag["LeftStick 8-direction-movement slider"]));
            MainGate[0].AddEx(() => CreateSlider(64, 180, model, nameof(model.LeftStickClipRadius), 1, sliderTextPrefix: $"ClipRadius:", defalutValue: 128, hideself: debug_flag["LeftStick 8-direction-movement slider"]));

            MainGate.Add(CreateGateBase("enable mapping"));//[1]
            MainGate[1].AddEx(() => CreateSlider(16, 1024, model, nameof(model.CycleActivationDuration), 1, sliderTextPrefix: $"CycleActivationDuration:", defalutValue: 384, sliderTextSuffix: "ms", hideself: debug_flag["LeftStick cycle parameta slider"]));
            MainGate[1].AddEx(() => CreateSlider(16, 1024, model, nameof(model.CycleDuration), 1, sliderTextPrefix: $"CycleDuration:", defalutValue: 64, sliderTextSuffix: "x2 ms", hideself: debug_flag["LeftStick cycle parameta slider"]));

            MainGate.Add(CreateGateBase("enable output"));//[2]

            MainGate.AddEx(() => new cJoyConMapper() { Margin = new Thickness(0, 4, 0, 4) });
            MainGate.AddEx(() => new TextBlock() { Text = "- Make sure to map Capture->Triangle, Side_L->Square, and Side_R->Circle in DS4W.", Foreground = new SolidColorBrush(Colors.White) });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable)
            {
                LeftStickFix();
                LeftStickNormalized(MainGate[0][0].Enable);
            }
            if (MainGate[1].Enable) { MappingToKBM(); }
            if (MainGate[2].Enable is false) { DisableOutput(); }
        }
    }

    //摇杆修正
    partial class Game_JoyConMapper
    {
        StickController SM = new(new(64, 1280), new(64, 1280));

        private void LeftStickFix()
        {
            SM.SetLeft((int)model.LeftStickDeadZone, (int)model.LeftStickEnlargementFactor, (int)model.LeftStickClipRadius);
            SM.ExecuteAction(VirtualDS4);
        }
    }

    //映射至键鼠
    partial class Game_JoyConMapper
    {
        private void LeftStickNormalized(bool onoff)
        {
            if (onoff)
            {
                Normalizer.Instance.StickNormalize(VirtualDS4.LX, VirtualDS4.LY, out VirtualDS4.LX, out VirtualDS4.LY, StickPosition.Left);
            }
            else
            {
                Normalizer.Instance.StickNormalize(VirtualDS4.LX, VirtualDS4.LY, out _, out _, StickPosition.Left);
            }
        }

        private void MappingToKBM()
        {
            JoyConMapperController.Instance.ApplyMapping(VirtualDS4, model.CycleActivationDuration, model.CycleDuration);
        }

        private void DisableOutput()
        {
            VirtualDS4.Reset();
        }
    }
}

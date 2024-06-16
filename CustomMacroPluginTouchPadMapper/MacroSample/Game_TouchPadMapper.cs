using CustomMacroBase;
using CustomMacroBase.Helper;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomMacroPluginGamepadMixer.MacroSample
{
    public partial class Game_TouchPadMapper
    {
        enum Position
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Unknown
        }

        class InnerModel
        {
            public ObservableCollection<int> IntList = new(Enumerable.Repeat(0, 20));
            public ObservableCollection<bool> BoolList = new(Enumerable.Repeat(false, 20));
            public ObservableCollection<double> DoubleList = new(Enumerable.Repeat(0d, 20));
        }

        class InnerViewModel : NotificationObject
        {
            InnerModel model = new();

            #region int
            // 0~3
            public int Touch0_X
            {
                get => model.IntList[0];
                set
                {
                    if (model.IntList[0] != value)
                    {
                        model.IntList[0] = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public int Touch0_Y
            {
                get => model.IntList[1];
                set
                {
                    if (model.IntList[1] != value)
                    {
                        model.IntList[1] = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public int Touch1_X
            {
                get => model.IntList[2];
                set
                {
                    if (model.IntList[2] != value)
                    {
                        model.IntList[2] = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public int Touch1_Y
            {
                get => model.IntList[3];
                set
                {
                    if (model.IntList[3] != value)
                    {
                        model.IntList[3] = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            // 4~7
            public int Center_X
            {
                get => model.IntList[4];
                set
                {
                    if (model.IntList[4] != value)
                    {
                        model.IntList[4] = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public int Center_Y
            {
                get => model.IntList[5];
                set
                {
                    if (model.IntList[5] != value)
                    {
                        model.IntList[5] = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            #endregion

            #region bool
            // 0

            #endregion


            public InnerViewModel()
            {
                Touch0_X = Touch0_Y = Touch1_X = Touch1_Y = -1;
                Center_X = 959; Center_Y = 470;
            }
        }

        static InnerViewModel viewmodel = new();
    }

    public partial class Game_TouchPadMapper : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Root";

            MainGate.AddEx(() =>
            {
                var sp = new StackPanel() { Orientation = Orientation.Horizontal };
                {
                    var sp0 = new StackPanel();
                    {
                        sp.Children.Add(CreateValueIndicator(viewmodel, [CreateValueIndicatorPacket(nameof(viewmodel.Touch0_X)),
                                                                         CreateValueIndicatorPacket(nameof(viewmodel.Touch0_Y))]));
                    }

                    var sp1 = new StackPanel();
                    {
                        sp.Children.Add(CreateValueIndicator(viewmodel, [CreateValueIndicatorPacket(nameof(viewmodel.Touch1_X)),
                                                                         CreateValueIndicatorPacket(nameof(viewmodel.Touch1_Y))]));
                    }

                    sp.Children.Add(sp0);
                    sp.Children.Add(sp1);
                }
                return sp;
            });
            MainGate.AddEx(() => CreateSlider(0, 1919, viewmodel, nameof(viewmodel.Center_X), 1, sliderTextPrefix: $"Left", defalutValue: 959, sliderTextSuffix: "Right"));
            MainGate.AddEx(() => CreateSlider(0, 941, viewmodel, nameof(viewmodel.Center_Y), 1, sliderTextPrefix: $"Top", defalutValue: 470, sliderTextSuffix: "Bottom"));

            MainGate.Add(CreateGateBase("BottomLeft -> L3", true));   //[0]
            MainGate.Add(CreateGateBase("BottomRight -> R3", true));
            MainGate.Add(CreateGateBase("TopLeft -> L3 + R3", true));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            GetTouchPos();

            TouchPadBottomLeft(MainGate[0].Enable);
            TouchPadBottomRight(MainGate[1].Enable);
            TouchPadTopLeft(MainGate[2].Enable);
        }
    }

    // MACRO
    public partial class Game_TouchPadMapper
    {
        Func<Position> GetTouch0Pos = () => CalculatePos(viewmodel.Touch0_X, viewmodel.Touch0_Y, viewmodel.Center_X, viewmodel.Center_Y);
        Func<Position> GetTouch1Pos = () => CalculatePos(viewmodel.Touch1_X, viewmodel.Touch1_Y, viewmodel.Center_X, viewmodel.Center_Y);

        private void GetTouchPos()
        {
            if (RealDS4.Touch0IsActive)
            {
                viewmodel.Touch0_X = RealDS4.Touch0[0];
                viewmodel.Touch0_Y = RealDS4.Touch0[1];
            }
            else
            {
                viewmodel.Touch0_X = viewmodel.Touch0_Y = -1;
            }

            if (RealDS4.Touch1IsActive)
            {
                viewmodel.Touch1_X = RealDS4.Touch1[0];
                viewmodel.Touch1_Y = RealDS4.Touch1[1];
            }
            else
            {
                viewmodel.Touch1_X = viewmodel.Touch1_Y = -1;
            }
        }

        private void TouchPadBottomLeft(bool canexecute)
        {
            if (!canexecute) { return; }

            var a = RealDS4.Touch0IsActive && GetTouch0Pos.Invoke() is Position.BottomLeft;
            var b = RealDS4.Touch1IsActive && GetTouch1Pos.Invoke() is Position.BottomLeft;

            if (a || b)
            {
                VirtualDS4.L3 = true;
            }
        }

        private void TouchPadBottomRight(bool canexecute)
        {
            if (!canexecute) { return; }

            var a = RealDS4.Touch0IsActive && GetTouch0Pos.Invoke() is Position.BottomRight;
            var b = RealDS4.Touch1IsActive && GetTouch1Pos.Invoke() is Position.BottomRight;

            if (a || b)
            {
                VirtualDS4.R3 = true;
            }
        }

        private void TouchPadTopLeft(bool canexecute)
        {
            if (!canexecute) { return; }

            var a = RealDS4.Touch0IsActive && GetTouch0Pos.Invoke() is Position.TopLeft;
            var b = RealDS4.Touch1IsActive && GetTouch1Pos.Invoke() is Position.TopLeft;

            if (a || b)
            {
                VirtualDS4.L3 = VirtualDS4.R3 = true;
            }
        }
    }

    // HELPER
    public partial class Game_TouchPadMapper
    {
        private TextBlock CreateTextBlock(string text, Color? color = null)
        {
            return new TextBlock()
            {
                Text = text,
                Foreground = new SolidColorBrush(color ?? Colors.White),
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };
        }
        private ValueIndicatorPacket CreateValueIndicatorPacket(string name)
        {
            return new ValueIndicatorPacket(propname: name,
                                            propnamecolor: new(Colors.Gray),
                                            propvaluecolor: new(Colors.LawnGreen),
                                            propvaluecolorswitcher: value =>
                                            {
                                                return value switch
                                                {
                                                    > 0 => new(Colors.LawnGreen),
                                                    _ => new(Colors.Crimson)
                                                };
                                            });
        }

        private static Position CalculatePos(int x, int y, int cx, int cy)
        {
            var isTop = y <= cy;
            var isLeft = x <= cx;

            if (isTop && isLeft) { return Position.TopLeft; }
            if (!isTop && isLeft) { return Position.BottomLeft; }
            if (isTop && !isLeft) { return Position.TopRight; }
            if (!isTop && !isLeft) { return Position.BottomRight; }

            return Position.Unknown;
        }
    }
}

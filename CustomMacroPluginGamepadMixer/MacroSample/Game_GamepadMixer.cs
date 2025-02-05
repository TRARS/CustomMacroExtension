using CommunityToolkit.Mvvm.Input;
using CustomMacroBase;
using CustomMacroBase.CustomControlEx.RippleButtonEx;
using CustomMacroBase.GamePadState;
using CustomMacroBase.PreBase;
using System.Windows.Controls;
using System.Windows.Media;

namespace CustomMacroPluginGamepadMixer.MacroSample
{
    public partial class Game_GamepadMixer
    {
        private DS4StateLite[] gamepadList = new List<DS4StateLite>(Enumerable.Repeat(new DS4StateLite(), 2)).ToArray();
        private ref DS4StateLite gamepad0 => ref gamepadList[0];
        private ref DS4StateLite gamepad1 => ref gamepadList[1];
    }

    public partial class Game_GamepadMixer : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Root";

            MainGate.AddEx(() => CreateTextBlock("- To enable the mapping feature, set the Ex3 toggle switch to 'ON' position.", Colors.LightGray));
            MainGate.AddEx(() => CreateTextBlock("- Be sure to use HidHide to solve the double input issue.", Colors.LightGray));

            MainGate.Add(CreateGateBase("Options", true));   //[0]
            {
                MainGate[0].Add(CreateGateBase("Disable the output of the 2nd controller.", true));   //[0][0]
                MainGate[0].Add(CreateGateBase("Only the 2nd controller can operate the buttons.", false));   //[0][1]
            }

            MainGate.Add(CreateGateBase("Managed by the 2nd Controller", true));   //[1]
            {
                MainGate[1].AddEx(() =>
                {
                    var sp = new StackPanel() { Orientation = Orientation.Horizontal };
                    {
                        sp.Children.Add(new cRippleButton()
                        {
                            Width = 80,
                            Height = 22,
                            Type = ButtonType.Custom,
                            Text = "Enable All",
                            Command = new RelayCommand(() =>
                            {
                                MainGate[1].GateBaseList.ForEach(x => x.Enable = true);
                            }),
                            Margin = new(0, 0, 5, 0),
                        });
                        sp.Children.Add(new cRippleButton()
                        {
                            Width = 80,
                            Height = 22,
                            Type = ButtonType.Custom,
                            Text = "Disable All",
                            Command = new RelayCommand(() =>
                            {
                                MainGate[1].GateBaseList.ForEach(x => x.Enable = false);
                            }),
                            Margin = new(0, 0, 5, 0),
                        });
                    }
                    return sp;
                });

                MainGate[1].Add(CreateGateBase("□", true));   //[1][0]
                MainGate[1].Add(CreateGateBase("△", true)); //[1][1]
                MainGate[1].Add(CreateGateBase("○", true));   //[1][2]
                MainGate[1].Add(CreateGateBase("×", true));    //[1][3]

                MainGate[1].Add(CreateGateBase("DpadLeft", true));  //[1][4]
                MainGate[1].Add(CreateGateBase("DpadUp", true));    //[1][5]
                MainGate[1].Add(CreateGateBase("DpadRight", true)); //[1][6]
                MainGate[1].Add(CreateGateBase("DpadDown", true));  //[1][7]

                MainGate[1].Add(CreateGateBase("Share", true));       //[1][8]
                MainGate[1].Add(CreateGateBase("Options", true));     //[1][9]
                MainGate[1].Add(CreateGateBase("TouchButton", true)); //[1][10]

                MainGate[1].Add(CreateGateBase("PS", true));   //[1][11]
                MainGate[1].Add(CreateGateBase("Mute", true)); //[1][12]

                MainGate[1].Add(CreateGateBase("L1", true)); //[1][13]
                MainGate[1].Add(CreateGateBase("L2", true)); //[1][14]
                MainGate[1].Add(CreateGateBase("L3", true)); //[1][15]

                MainGate[1].Add(CreateGateBase("R1", true)); //[1][16]
                MainGate[1].Add(CreateGateBase("R2", true)); //[1][17]
                MainGate[1].Add(CreateGateBase("R3", true)); //[1][18]

                MainGate[1].Add(CreateGateBase("Left Stick", true)); //[1][19]
                MainGate[1].Add(CreateGateBase("Right Stick", true)); //[1][20]
            }
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            Gamepad0(Ind, MainGate[1]);
            Gamepad1(Ind);
        }
    }

    public partial class Game_GamepadMixer
    {
        bool controller2_output_disabled => MainGate[0][0].Enable;
        bool controller2_buttons_only => MainGate[0][1].Enable;

        private void Gamepad0(int ind, GateBase host)
        {
            if (ind != 0) { return; }

            //VirtualDS4.CopyTo(ref gamepadList[ind]);

            // Map the button status of 2nd controller to 1st controller 
            {
                if (controller2_buttons_only)
                {
                    if (host[0].Enable) { VirtualDS4.Square = gamepad1.Square; }
                    if (host[1].Enable) { VirtualDS4.Triangle = gamepad1.Triangle; }
                    if (host[2].Enable) { VirtualDS4.Circle = gamepad1.Circle; }
                    if (host[3].Enable) { VirtualDS4.Cross = gamepad1.Cross; }

                    if (host[4].Enable) { VirtualDS4.DpadLeft = gamepad1.DpadLeft; }
                    if (host[5].Enable) { VirtualDS4.DpadUp = gamepad1.DpadUp; }
                    if (host[6].Enable) { VirtualDS4.DpadRight = gamepad1.DpadRight; }
                    if (host[7].Enable) { VirtualDS4.DpadDown = gamepad1.DpadDown; }

                    if (host[8].Enable) { VirtualDS4.Share = gamepad1.Share; }
                    if (host[9].Enable) { VirtualDS4.Options = gamepad1.Options; }
                    if (host[10].Enable) { VirtualDS4.OutputTouchButton = gamepad1.TouchButton; }

                    if (host[11].Enable) { VirtualDS4.PS = gamepad1.PS; }
                    if (host[12].Enable) { VirtualDS4.Mute = gamepad1.Mute; }

                    if (host[13].Enable) { VirtualDS4.L1 = gamepad1.L1; }
                    if (host[14].Enable) { VirtualDS4.L2 = gamepad1.L2; }
                    if (host[15].Enable) { VirtualDS4.L3 = gamepad1.L3; }

                    if (host[16].Enable) { VirtualDS4.R1 = gamepad1.R1; }
                    if (host[17].Enable) { VirtualDS4.R2 = gamepad1.R2; }
                    if (host[18].Enable) { VirtualDS4.R3 = gamepad1.R3; }

                    //
                    if (host[19].Enable) { VirtualDS4.LX = gamepad1.LX; VirtualDS4.LY = gamepad1.LY; }
                    if (host[20].Enable) { VirtualDS4.RX = gamepad1.RX; VirtualDS4.RY = gamepad1.RY; }
                }
                else
                {
                    if (host[0].Enable) { VirtualDS4.Square |= gamepad1.Square; }
                    if (host[1].Enable) { VirtualDS4.Triangle |= gamepad1.Triangle; }
                    if (host[2].Enable) { VirtualDS4.Circle |= gamepad1.Circle; }
                    if (host[3].Enable) { VirtualDS4.Cross |= gamepad1.Cross; }

                    if (host[4].Enable) { VirtualDS4.DpadLeft |= gamepad1.DpadLeft; }
                    if (host[5].Enable) { VirtualDS4.DpadUp |= gamepad1.DpadUp; }
                    if (host[6].Enable) { VirtualDS4.DpadRight |= gamepad1.DpadRight; }
                    if (host[7].Enable) { VirtualDS4.DpadDown |= gamepad1.DpadDown; }

                    if (host[8].Enable) { VirtualDS4.Share |= gamepad1.Share; }
                    if (host[9].Enable) { VirtualDS4.Options |= gamepad1.Options; }
                    if (host[10].Enable) { VirtualDS4.OutputTouchButton |= gamepad1.TouchButton; }

                    if (host[11].Enable) { VirtualDS4.PS |= gamepad1.PS; }
                    if (host[12].Enable) { VirtualDS4.Mute |= gamepad1.Mute; }

                    if (host[13].Enable) { VirtualDS4.L1 |= gamepad1.L1; }
                    if (host[14].Enable) { VirtualDS4.L2 |= gamepad1.L2; }
                    if (host[15].Enable) { VirtualDS4.L3 |= gamepad1.L3; }

                    if (host[16].Enable) { VirtualDS4.R1 |= gamepad1.R1; }
                    if (host[17].Enable) { VirtualDS4.R2 |= gamepad1.R2; }
                    if (host[18].Enable) { VirtualDS4.R3 |= gamepad1.R3; }

                    //
                    if (host[19].Enable) { VirtualDS4.LX = AplusB(VirtualDS4.LX, gamepad1.LX); VirtualDS4.LY = AplusB(VirtualDS4.LY, gamepad1.LY); }
                    if (host[20].Enable) { VirtualDS4.RX = AplusB(VirtualDS4.RX, gamepad1.RX); VirtualDS4.RY = AplusB(VirtualDS4.RY, gamepad1.RY); }
                }
            }
        }

        private void Gamepad1(int ind)
        {
            if (ind != 1) { return; }

            VirtualDS4.CopyTo(ref gamepadList[ind]); //
            if (controller2_output_disabled) { VirtualDS4.Reset(); }
        }
    }

    public partial class Game_GamepadMixer
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

        private byte AplusB(byte a, byte b)
        {
            var va = a - 127;
            var vb = b - 127;
            var c = (va + vb);

            return (byte)Math.Clamp((127 + c), byte.MinValue, byte.MaxValue);
        }
    }
}

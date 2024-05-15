using CustomMacroBase.GamePadState;
using CustomMacroBase.Helper.Extensions;
using CustomMacroBase.Helper.Tools.SendInputManager;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI;
using System;
using System.Windows;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base
{
    //单例
    partial class JoyConMapperController
    {
        private static readonly Lazy<JoyConMapperController> lazyObject = new(() => new JoyConMapperController());
        public static JoyConMapperController Instance => lazyObject.Value;

        private static DS4StateLite vStateLite = new();

        private JoyConMapperController() { }
    }

    //
    partial class JoyConMapperController
    {
        public bool L => vStateLite.L1;
        public bool ZL => vStateLite.L2 > 0;
        public bool Minus => vStateLite.Share;
        public bool North => DirectionMap.Instance.North;
        public bool South => DirectionMap.Instance.South;
        public bool West => DirectionMap.Instance.West;
        public bool East => DirectionMap.Instance.East;
        public bool L3 => vStateLite.L3;
        public bool DpadUp => vStateLite.DpadUp;
        public bool DpadDown => vStateLite.DpadDown;
        public bool DpadLeft => vStateLite.DpadLeft;
        public bool DpadRight => vStateLite.DpadRight;
        public bool Capture => vStateLite.Triangle;
        public bool SL => vStateLite.Square;
        public bool SR => vStateLite.Circle;

        public void ApplyMapping(DS4StateLite v, double cycleActivationDuration, double cycleDuration)
        {
            v.CopyTo(ref vStateLite);

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                cJoyConMapper_viewmodel.Instance.KeyboardMappingInfoList.ForEach(x =>
                {
                    var condition = x.BtnMapping.Condition.Invoke();
                    var keys = x.BtnMapping.GetKeys;
                    var cycle = x.BtnMapping.Cycle;
                    SendKBMInput.KeyDownEx(x.DisplayName, condition, keys, cycle, (int)cycleActivationDuration, (int)cycleDuration);
                });
            });
        }
    }
}

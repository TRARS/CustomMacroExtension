using CustomMacroBase.GamePadState;
using CustomMacroBase.Helper.Extensions;
using CustomMacroBase.Helper.Tools.SendInputManager;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI;
using System;

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

    //内置映射（不再使用）
    //partial class JoyConMapperController
    //{
    //    //映射到键盘按键
    //    private readonly List<MappingInfo<KeyboardKeys>> KeyboardMapping = new()
    //    {
    //        new(() => DirectionMap.Instance.North, new [] { KeyboardKeys.S }, AutoCycle.On),//放大 ↑
    //        new(() => DirectionMap.Instance.South, new [] { KeyboardKeys.T }, AutoCycle.On),//缩小 ↓
    //        new(() => DirectionMap.Instance.West, new [] { KeyboardKeys.Z }, AutoCycle.On),//撤销 ←
    //        new(() => DirectionMap.Instance.East, new [] { KeyboardKeys.Y }, AutoCycle.On),//重做 →

    //        new(() => vStateLite.L1, new [] { KeyboardKeys.C }, AutoCycle.On),//水彩笔
    //        new(() => vStateLite.L2 > 0, new [] { KeyboardKeys.Escape }, AutoCycle.On),//ESC

    //        new(() => vStateLite.DpadUp, new [] { KeyboardKeys.F6 }, AutoCycle.On),//前景色切换至透明
    //        new(() => vStateLite.DpadDown, new [] { KeyboardKeys.Space }, AutoCycle.On),//空格
    //        new(() => vStateLite.DpadLeft, new [] { KeyboardKeys.Q }, AutoCycle.On),//顺时针旋转
    //        new(() => vStateLite.DpadRight, new [] { KeyboardKeys.R }, AutoCycle.On),//逆时针旋转

    //        new(() => vStateLite.Triangle, new [] { KeyboardKeys.RControlKey }, null),//CTRL
    //        new(() => vStateLite.Square, new [] { KeyboardKeys.LControlKey, KeyboardKeys.S}, null),//CTRL + S
    //        new(() => vStateLite.Circle, new [] { KeyboardKeys.ShiftKey }, null),//Shift
    //    };

    //    //映射到鼠标按键
    //    private readonly List<MappingInfo<MouseKeys>> MouseMapping = new()
    //    {
    //        //new(() => vStateLite.L1, new [] { MouseKeys.Left }, AutoCycle.On),
    //        //new(() => vStateLite.L2 > 0, new [] { MouseKeys.Right }, null),
    //    };
    //}

    partial class JoyConMapperController
    {
        public bool L => vStateLite.L1;
        public bool ZL => vStateLite.L2 > 0;
        public bool Minus => vStateLite.Share;
        public bool North => DirectionMap.Instance.North;
        public bool South => DirectionMap.Instance.South;
        public bool West => DirectionMap.Instance.West;
        public bool East => DirectionMap.Instance.East;
        public bool DpadUp => vStateLite.DpadUp;
        public bool DpadDown => vStateLite.DpadDown;
        public bool DpadLeft => vStateLite.DpadLeft;
        public bool DpadRight => vStateLite.DpadRight;
        public bool Capture => vStateLite.Triangle;
        public bool SL => vStateLite.Square;
        public bool SR => vStateLite.Circle;

        public void ApplyMapping(DS4StateLite v, double cycleActivationDuration, double cycleDuration)
        {
            vStateLite = v;

            cJoyConMapper_viewmodel.Instance.KeyboardMappingInfoList.ForEach(x =>
            {
                var condition = x.BtnMapping.Condition.Invoke();
                var keys = x.BtnMapping.GetKeys;
                var cycle = x.BtnMapping.Cycle;
                SendKBMInput.KeyDownEx(x.DisplayName, condition, keys, cycle, (int)cycleActivationDuration, (int)cycleDuration);
            });

            //
            //KeyboardMapping.ForEach(x =>
            //{
            //    SendKBMInput.KeyDownEx(x.Condition.Invoke(), x.Keys, x.Cycle, (int)cycleActivationDuration, (int)cycleDuration);
            //});
            //MouseMapping.ForEach(x =>
            //{
            //    SendKBMInput.MouseDownEx(x.Condition.Invoke(), x.Keys, x.Cycle, (int)cycleActivationDuration, (int)cycleDuration);
            //});
        }
    }
}

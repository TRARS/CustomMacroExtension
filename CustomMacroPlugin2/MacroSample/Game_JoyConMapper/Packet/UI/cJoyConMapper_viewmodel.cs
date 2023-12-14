using CustomMacroBase.Helper;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using KeyboardKeys = System.Windows.Forms.Keys;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI
{
    //单例
    public partial class cJoyConMapper_viewmodel
    {
        private static readonly Lazy<cJoyConMapper_viewmodel> lazyObject = new(() => new cJoyConMapper_viewmodel());
        public static cJoyConMapper_viewmodel Instance => lazyObject.Value;
    }

    public partial class cJoyConMapper_viewmodel : NotificationObject
    {
        cJoyConMapper_model model = new();

        public Thickness? CurrentBtnPos
        {
            get { return model.CurrentBtnPos; }
            set
            {
                model.CurrentBtnPos = value;
                NotifyPropertyChanged();
            }
        }
        public BitmapImage GamePadImage
        {
            get { return model.GamePadImage; }
            set
            {
                model.GamePadImage = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<MappingInfoPacket<KeyboardKeys>> KeyboardMappingInfoList
        {
            get { return model.KeyboardMappingInfoList; }
            set
            {
                model.KeyboardMappingInfoList = value;
                NotifyPropertyChanged();
            }
        }

        private string? AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        private JoyConMapperController Joycon => JoyConMapperController.Instance;

        private cJoyConMapper_viewmodel()
        {
            //按键原名
            List<string> BtnNameList = new()
            {
                "North",
                "South",
                "West",
                "East",
                "DpadUp",
                "DpadDown",
                "DpadLeft",
                "DpadRight",
                "L",
                "ZL",
                "Minus",
                "Capture",
                "SL",
                "SR",
            };

            //按键别名，禁止value重复
            Dictionary<string, string> DisplayNameDic = new Dictionary<string, string>
            {
                { "North", "LS_Up" },
                { "South", "LS_Down" },
                { "West", "LS_Left" },
                { "East", "LS_Right" },
                { "DpadUp", "DpadUp" },
                { "DpadDown", "DpadDown" },
                { "DpadLeft", "DpadLeft" },
                { "DpadRight", "DpadRight" },
                { "L", "L" },
                { "ZL", "ZL" },
                { "Minus", "Minus" },
                { "Capture", "Capture" },
                { "SL", "SL" },
                { "SR", "SR" }
            };

            //按键相对坐标
            Dictionary<string, Thickness> BtnPosDic = new Dictionary<string, Thickness>
            {
                { "North", new Thickness(27, 116, 0, 0) },
                { "South", new Thickness(27, 143, 0, 0) },
                { "West", new Thickness(12, 129, 0, 0) },
                { "East", new Thickness(42, 129, 0, 0) },
                { "DpadUp", new Thickness(27, 170, 0, 0) },
                { "DpadDown", new Thickness(27, 200, 0, 0) },
                { "DpadLeft", new Thickness(12, 185, 0, 0) },
                { "DpadRight", new Thickness(42, 185, 0, 0) },
                { "L", new Thickness(27, 24, 0, 0) },
                { "ZL", new Thickness(27, 3, 0, 0) },
                { "Minus", new Thickness(47, 98, 0, 0) },
                { "Capture", new Thickness(38, 222, 0, 0) },
                { "SL", new Thickness(119, 131, 0, 0) },
                { "SR", new Thickness(119, 211, 0, 0) }
            };

            //映射信息
            Dictionary<string, MappingInfo<KeyboardKeys>> BtnMappingCondition = new Dictionary<string, MappingInfo<KeyboardKeys>>
            {
                { "North", new (()=> Joycon.North,         new[] { KeyboardKeys.S }, AutoCycle.On) },
                { "South", new (()=> Joycon.South,         new[] { KeyboardKeys.T }, AutoCycle.On ) },
                { "West",  new (()=> Joycon.West,          new[] { KeyboardKeys.Z }, AutoCycle.On ) },
                { "East",  new (()=> Joycon.East,          new[] { KeyboardKeys.Y }, AutoCycle.On ) },
                { "DpadUp",    new (()=> Joycon.DpadUp,    new[] { KeyboardKeys.F6 }, AutoCycle.On ) },
                { "DpadDown",  new (()=> Joycon.DpadDown,  new[] { KeyboardKeys.Space }, AutoCycle.On ) },
                { "DpadLeft",  new (()=> Joycon.DpadLeft,  new[] { KeyboardKeys.Q }, AutoCycle.On ) },
                { "DpadRight", new (()=> Joycon.DpadRight, new[] { KeyboardKeys.R }, AutoCycle.On ) },
                { "L",  new (()=> Joycon.L,                new[] { KeyboardKeys.C }, AutoCycle.On ) },
                { "ZL", new (()=> Joycon.ZL,               new[] { KeyboardKeys.Escape }, AutoCycle.On ) },
                { "Minus",    new (()=> Joycon.Minus,      new[] { KeyboardKeys.None }, AutoCycle.On ) },
                { "Capture",  new (()=> Joycon.Capture,    new[] { KeyboardKeys.RControlKey }, null ) },
                { "SL", new (()=> Joycon.SL,               new[] { KeyboardKeys.LControlKey, KeyboardKeys.S }, null ) },
                { "SR", new (()=> Joycon.SR,               new[] { KeyboardKeys.ShiftKey }, null ) }
            };

            //注释
            Dictionary<string, string?> CommentDic = new Dictionary<string, string?>
            {
                { "North", "放大 ↑" },
                { "South", "缩小 ↓" },
                { "West", "撤销 ←" },
                { "East", "重做 →" },
                { "DpadUp", "前景色切换至透明" },
                { "DpadDown", "空格" },
                { "DpadLeft", "顺时针旋转" },
                { "DpadRight", "逆时针旋转" },
                { "L", "水彩笔" },
                { "ZL", "ESC" },
                { "Minus", null },
                { "Capture", "CTRL" },
                { "SL", "CTRL + S" },
                { "SR", "Shift" }
            };

            //载入映射信息
            BtnNameList.ForEach((x) =>
            {
                this.KeyboardMappingInfoList = this.KeyboardMappingInfoList ?? new();
                this.KeyboardMappingInfoList.Add(new()
                {
                    DisplayName = DisplayNameDic[x],
                    BtnName = x,
                    BtnPos = BtnPosDic[x],
                    BtnMapping = BtnMappingCondition[x],
                    Comment = CommentDic[x] ?? "",
                });
            });

            //载入手柄图
            GamePadImage = new BitmapImage(new Uri($"pack://application:,,,/{AssemblyName};component/MacroSample/Game_JoyConMapper/Packet/Resources/Joycon(L).png", UriKind.Absolute));


            //
            Mediator.Instance.Register(JoyConMapperMessageType.Instance.GetCurrentJoyConMapperMouseEnterItemModel, (para) =>
            {
                if (para is MappingInfoPacket<KeyboardKeys> model)
                {
                    CurrentBtnPos = model.BtnPos;
                    Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, model.Comment);
                }
                else
                {
                    CurrentBtnPos = null;
                }
            });
        }
    }
}

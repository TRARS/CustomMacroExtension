using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CustomMacroBase.Messages;
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

    public partial class cJoyConMapper_viewmodel : ObservableObject
    {
        cJoyConMapper_model model = new();

        public Thickness? CurrentBtnPos
        {
            get { return model.CurrentBtnPos; }
            set
            {
                model.CurrentBtnPos = value;
                OnPropertyChanged();
            }
        }
        public BitmapImage GamePadImage
        {
            get { return model.GamePadImage; }
            set
            {
                model.GamePadImage = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<MappingInfoPacket<KeyboardKeys>> KeyboardMappingInfoList
        {
            get { return model.KeyboardMappingInfoList; }
            set
            {
                model.KeyboardMappingInfoList = value;
                OnPropertyChanged();
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
                "L3",
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
                { "L3", "LS" },
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
                { "L3", new Thickness(27, 129, 0, 0) },
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
                { "North", new (()=> Joycon.North,         new[] { KeyboardKeys.S }, AutoCycle.Yes) },
                { "South", new (()=> Joycon.South,         new[] { KeyboardKeys.T }, AutoCycle.Yes ) },
                { "West",  new (()=> Joycon.West,          new[] { KeyboardKeys.Z }, AutoCycle.Yes ) },
                { "East",  new (()=> Joycon.East,          new[] { KeyboardKeys.Y }, AutoCycle.Yes ) },
                { "L3",    new (()=> Joycon.L3,            new[] { KeyboardKeys.Enter }, AutoCycle.Yes ) },
                { "DpadUp",    new (()=> Joycon.DpadUp,    new[] { KeyboardKeys.F6 }, AutoCycle.Yes ) },
                { "DpadDown",  new (()=> Joycon.DpadDown,  new[] { KeyboardKeys.Space }, null ) },
                { "DpadLeft",  new (()=> Joycon.DpadLeft,  new[] { KeyboardKeys.Q }, AutoCycle.Yes ) },
                { "DpadRight", new (()=> Joycon.DpadRight, new[] { KeyboardKeys.R }, AutoCycle.Yes ) },
                { "L",  new (()=> Joycon.L,                new[] { KeyboardKeys.C }, AutoCycle.Yes ) },
                { "ZL", new (()=> Joycon.ZL,               new[] { KeyboardKeys.Escape }, AutoCycle.Yes ) },
                { "Minus",    new (()=> Joycon.Minus,      new[] { KeyboardKeys.None }, AutoCycle.Yes ) },
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
                { "L3", "回车" },
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
            WeakReferenceMessenger.Default.Register<GetCurrentJoyConMapperMouseEnterItemModel<KeyboardKeys>>(this, (r, m) =>
            {
                if (m.Value is MappingInfoPacket<KeyboardKeys> model)
                {
                    CurrentBtnPos = model.BtnPos;
                    WeakReferenceMessenger.Default.Send(new PrintNewMessage(model.Comment));
                }
                else
                {
                    CurrentBtnPos = null;
                }
            });
        }
    }
}

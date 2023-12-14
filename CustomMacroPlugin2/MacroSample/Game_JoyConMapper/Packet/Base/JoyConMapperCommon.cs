using CustomMacroBase.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base
{
    //飞消息用
    public class JoyConMapperMessageType
    {
        private static readonly Lazy<JoyConMapperMessageType> lazyObject = new(() => new JoyConMapperMessageType());
        public static JoyConMapperMessageType Instance => lazyObject.Value;

        public string GetCurrentJoyConMapperMouseEnterItemModel { get; init; }

        public JoyConMapperMessageType()
        {
            GetCurrentJoyConMapperMouseEnterItemModel = nameof(GetCurrentJoyConMapperMouseEnterItemModel) + GenerateRandomString(16);
        }

        private Random random = new Random();
        private string GenerateRandomString(int length)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
    }

    //二值化用
    public enum StickPosition
    {
        Left,
        Right,
    }
    public class Normalizer
    {
        private static readonly Lazy<Normalizer> lazyObject = new(() => new Normalizer());
        public static Normalizer Instance => lazyObject.Value;

        private static double slant = 128 / Math.Sqrt(2);
        private Vector[] directionsList = new Vector[8]
        {
            new Vector(0, 128), new Vector(-slant, slant), new Vector(-128,0), new Vector(-slant, -slant),
            new Vector(0, -128), new Vector(slant, -slant), new Vector(128, 0), new Vector(slant, slant)
        };

        private int noise_reduction = 2;//防抖参数
        private int left_count = 0;
        private int right_count = 0;

        public int LeftAngleIdx { get; set; } = -1;
        public int RightAngleIdx { get; set; } = -1;

        public void TriggerNormalize(byte input, out byte output)
        {
            output = input > 0 ? byte.MaxValue : input;
        }
        public void StickNormalize(byte x, byte y, out byte nx, out byte ny, StickPosition pos)
        {
            var idx = GetAngle(x, y);
            nx = (byte)Math.Clamp(128 + (idx == -1 ? 0 : directionsList[idx].X), 0, 255);
            ny = (byte)Math.Clamp(128 + (idx == -1 ? 0 : directionsList[idx].Y), 0, 255);

            switch (pos)
            {
                case StickPosition.Left:
                    if (LeftAngleIdx != idx && ++left_count > noise_reduction)
                    {
                        LeftAngleIdx = idx;
                        left_count = 0;
                    }
                    break;
                case StickPosition.Right:
                    if (RightAngleIdx != idx && ++right_count > noise_reduction)
                    {
                        RightAngleIdx = idx;
                        right_count = 0;
                    }
                    break;
            }
        }

        private int GetAngle(byte x, byte y, bool applyAngleDeadZone = false)
        {
            if (Math.Pow(x - 128, 2.0) + Math.Pow(y - 128, 2.0) < Math.Pow(64, 2.0))
            {
                return -1;
            }
            return AngleCalculate(x, y, applyAngleDeadZone);
        }
        private int AngleCalculate(byte x, byte y, bool applyAngleDeadZone)
        {
            var angle = -Math.Atan2(x - 128, y - 128) * 180 / Math.PI;
            angle += angle < 0 ? 360 : 0;
            angle += 22.5;
            var angle_idx = AngleDeadzone(angle / 45.0, applyAngleDeadZone); //(int)(angle / 45.0);// AngleDeadzone(angle / 45.0);//

            return angle_idx == 8 ? 0 : angle_idx;
        }
        private int AngleDeadzone(double src, bool flag)
        {
            if (flag is false) { return (int)src; }

            int a = (int)src;//整数部分
            double b = src - a;//小数部分
            return Math.Abs(b - 0.5) < 0.45 ? a : -1;
        }
    }

    //找摇杆方向用
    public enum EightDirections
    {
        /// <summary> ↓ </summary>
        South = 0,

        /// <summary> ↙ </summary>
        Southwest,

        /// <summary> ← </summary>
        West,

        /// <summary> ↖ </summary>
        Northwest,

        /// <summary> ↑ </summary>
        North,

        /// <summary> ↗ </summary>
        Northeast,

        /// <summary> → </summary>
        East,

        /// <summary> ↘ </summary>
        Southeast,
    }
    public class DirectionMap
    {
        private static readonly Lazy<DirectionMap> lazyObject = new(() => new DirectionMap());
        public static DirectionMap Instance => lazyObject.Value;

        private int LeftAngle => Normalizer.Instance.LeftAngleIdx;
        //private int RightAngle => Normalizer.Instance.RightAngleIdx;

        public bool South => LeftAngle == (int)EightDirections.South;
        public bool Southwest => LeftAngle == (int)EightDirections.Southwest;
        public bool West => LeftAngle == (int)EightDirections.West;
        public bool Northwest => LeftAngle == (int)EightDirections.Northwest;
        public bool North => LeftAngle == (int)EightDirections.North;
        public bool Northeast => LeftAngle == (int)EightDirections.Northeast;
        public bool East => LeftAngle == (int)EightDirections.East;
        public bool Southeast => LeftAngle == (int)EightDirections.Southeast;
    }

    //映射到键盘用
    public enum AutoCycle
    {
        On,
        Off
    }
    public class MappingInfo<T> : NotificationObject
    {
        public Func<bool> Condition { get; set; }
        public T[] Keys { get; set; }

        //循环选项
        private List<string> _CycleEnumList = new();
        public List<string> CycleEnumList
        {
            get { return _CycleEnumList; }
            set { _CycleEnumList = value; NotifyPropertyChanged(); }
        }
        private bool _Cycle;
        public bool Cycle
        {
            get => _Cycle;
            set { _Cycle = value; NotifyPropertyChanged(); }
        }

        //第一个Key
        private List<T> _KeyEnumList0 = new();//List<KeyboardKeys>
        public List<T> KeyEnumList0
        {
            get { return _KeyEnumList0; }
            set { _KeyEnumList0 = value; NotifyPropertyChanged(); }
        }
        private int _SelectedKey0 = 0;
        public int SelectedKey0
        {
            get { return _SelectedKey0; }
            set { _SelectedKey0 = value; NotifyPropertyChanged(); }
        }
        //第二个Key
        private List<T> _KeyEnumList1 = new();
        public List<T> KeyEnumList1
        {
            get { return _KeyEnumList1; }
            set { _KeyEnumList1 = value; NotifyPropertyChanged(); }
        }
        private int _SelectedKey1 = 0;
        public int SelectedKey1
        {
            get { return _SelectedKey1; }
            set { _SelectedKey1 = value; NotifyPropertyChanged(); }
        }

        //Key汇总
        public T[] GetKeys
        {
            get
            {
                if (SelectedKey0 > 0 && SelectedKey1 > 0)
                {
                    return new T[] { KeyEnumList0[SelectedKey0], KeyEnumList1[SelectedKey1] };
                }
                else if (SelectedKey0 > 0)
                {
                    return new T[] { KeyEnumList0[SelectedKey0] };
                }
                else if (SelectedKey1 > 0)
                {
                    return new T[] { KeyEnumList1[SelectedKey1] };
                }
                else
                {
                    return Array.Empty<T>();
                }
            }
        }

        public MappingInfo(Func<bool> condition, T[] keys, AutoCycle? autocycle = null)
        {
            Condition = condition;
            Keys = keys;
            Cycle = autocycle switch
            {
                AutoCycle.On => true,
                AutoCycle.Off => false,
                null => false,
                _ => false
            };


            CycleEnumList = Enum.GetValues(typeof(AutoCycle))
                                .Cast<AutoCycle>()
                                .Select(e => e.ToString())
                                .ToList();

            KeyEnumList0 = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            KeyEnumList1 = Enum.GetValues(typeof(T)).Cast<T>().ToList();

            if (Keys.Length > 0) { SelectedKey0 = KeyEnumList0.IndexOf((Keys[0])); }
            if (Keys.Length > 1) { SelectedKey1 = KeyEnumList1.IndexOf((Keys[1])); }
        }
    }



    //绑定到UI用
    public class MappingInfoPacket<T>
    {
        //按键别名
        public string DisplayName { get; set; }

        //按键原名
        public string BtnName { get; set; }

        //按键相对坐标
        public Thickness BtnPos { get; set; }

        //映射信息
        public MappingInfo<T> BtnMapping { get; set; }

        //注释
        public string Comment { get; set; }
    }
}

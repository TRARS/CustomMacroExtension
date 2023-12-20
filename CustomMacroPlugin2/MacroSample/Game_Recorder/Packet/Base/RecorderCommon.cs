using CustomMacroBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base
{
    //飞消息用（不使用enum，使用string，省得撞数字）
    public class RecorderMessageType
    {
        private static readonly Lazy<RecorderMessageType> lazyObject = new(() => new RecorderMessageType());
        public static RecorderMessageType Instance => lazyObject.Value;

        public string Record { get; init; }
        public string ApplyRecord { get; init; }
        public string StartRecordedAction { get; init; }
        public string StopRecordedAction { get; init; }
        public string GetCurrentRecorderMouseEnterItemModel { get; init; }
        public string ItemHitTest { get; init; }

        private RecorderMessageType()
        {
            Record = nameof(Record) + GenerateRandomString(16);
            ApplyRecord = nameof(ApplyRecord) + GenerateRandomString(16);
            StartRecordedAction = nameof(StartRecordedAction) + GenerateRandomString(16);
            StopRecordedAction = nameof(StopRecordedAction) + GenerateRandomString(16);
            GetCurrentRecorderMouseEnterItemModel = nameof(GetCurrentRecorderMouseEnterItemModel) + GenerateRandomString(16);
            ItemHitTest = nameof(ItemHitTest) + GenerateRandomString(16);
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



    //录制动作时用的
    public class RecorderData
    {
        public int Holdtime { get; set; }
        public string Key { get; set; }
        public string State { get; set; }
        public int DefaultDuration { get; set; }
    }

    //播放动作时用的
    public enum RecorderKeyType
    {
        Button,
        Trigger,
        LeftStick,
        RightStick,
    }
    public class RecorderAction
    {
        public RecorderKeyType Type { get; set; }
        public string Key { get; set; }
        public dynamic Value { get; set; } //byte or bool
        public byte X { get; set; }
        public byte Y { get; set; }
        public int Duration { get; set; }
    }

    //DS4Btn转List用的
    public class RecorderKeyList
    {
        private static readonly Lazy<RecorderKeyList> lazyObject = new(() => new RecorderKeyList());
        public static RecorderKeyList Instance => lazyObject.Value;

        public List<string> ButtonList { get; init; }
        public List<string> TriggerList { get; init; }
        public List<string> DirectionList { get; init; }

        private RecorderKeyList()
        {
            ButtonList = (from prop in typeof(DS4Btn).GetProperties()
                          where (prop.Name.IndexOf("Touch") == -1 || prop.Name == "OutputTouchButton") && //prop.Name != "TouchButton" && prop.Name != "Touch0RawTrackingNum" && prop.Name != "Touch0Id" && prop.Name != "Touch0IsActive" &&
                                prop.Name != "L2" && prop.Name != "R2" && prop.Name != "LX" && prop.Name != "LY" && prop.Name != "RX" && prop.Name != "RY"
                          select prop.GetValue(new DS4Btn())!.ToString()).ToList();

            TriggerList = new() { "L2", "R2" };

            DirectionList = new() { "↓", "↙", "← ", "↖", "↑", "↗", "→", "↘" };
        }
    }
}

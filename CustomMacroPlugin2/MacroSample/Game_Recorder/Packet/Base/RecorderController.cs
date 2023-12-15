using CustomMacroBase.GamePadState;
using CustomMacroBase.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

//Base
namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base
{
    //一般按键状态机
    public partial class RecorderBase
    {
        //状态
        private abstract class BtnState
        {
            public Action<string>? RecordIt { get; init; }
            public Action? NextState { get; init; }
            public abstract void Press();
            public abstract void Release();
        }
        private class BtnPressed : BtnState
        {
            public override void Press() { }
            public override void Release()
            {
                RecordIt?.Invoke("release");
                NextState?.Invoke();
            }
        }
        private class BtnReleased : BtnState
        {
            public override void Press()
            {
                RecordIt?.Invoke("press");
                NextState?.Invoke();
            }
            public override void Release() { }
        }
        //状态机
        private sealed class BtnStateMachine
        {
            public string currentKey = string.Empty;

            private BtnState? currentState = null;
            private readonly BtnState? pressedState = null;
            private readonly BtnState? releasedState = null;

            public BtnStateMachine(string key, Action<string>? record_it)
            {
                currentKey = key;

                pressedState = new BtnPressed() { NextState = () => { currentState = releasedState; }, RecordIt = record_it };
                releasedState = new BtnReleased() { NextState = () => { currentState = pressedState; }, RecordIt = record_it };
                currentState = releasedState;
            }

            public void Update(Func<string, bool> callback)
            {
                switch (callback.Invoke(currentKey))
                {
                    case true:
                        currentState?.Press(); break;
                    case false:
                        currentState?.Release(); break;
                }
            }
        }
    }

    //八方向摇杆状态机
    public partial class RecorderBase
    {
        //状态机（伪）
        private sealed class StickStateMachine
        {
            private List<string> valueList = new() { "↓", "↙", "← ", "↖", "↑", "↗", "→", "↘" };
            private Action<string>? recordIt;
            private int previousIdx;
            private int currentIdx;

            public StickStateMachine(Action<string>? record_it)
            {
                currentIdx = previousIdx = -1;
                recordIt = record_it;
            }

            public void Update(int idx)
            {
                currentIdx = idx;

                if (currentIdx != previousIdx)
                {
                    recordIt?.Invoke(currentIdx > -1 ? valueList[currentIdx] : "●");
                }

                previousIdx = currentIdx;
            }
        }
    }

    //构造
    public partial class RecorderBase
    {
        public readonly Stopwatch InnerStopwatch = new();

        private readonly List<BtnStateMachine> StateMachineListA;//
        private readonly List<BtnStateMachine> StateMachineListB;//L2R2
        private readonly StickStateMachine StateMachineLS;
        private readonly StickStateMachine StateMachineRS;

        public RecorderBase()
        {
            Mediator.Instance.Register(RecorderMessageType.Instance.ApplyRecord, (para) => { Actions = (List<RecorderAction>)para; });

            StateMachineListA = RecorderKeyList.Instance.ButtonList.Select(key => new BtnStateMachine(key, state => { NotifySend(key, state); })).ToList();
            StateMachineListB = RecorderKeyList.Instance.TriggerList.Select(key => new BtnStateMachine(key, state => { NotifySend(key, state); })).ToList();
            StateMachineLS = new StickStateMachine(direction => { NotifySend("Left Stick", direction); });
            StateMachineRS = new StickStateMachine(direction => { NotifySend("Right Stick", direction); });
        }
    }

    //流程
    public partial class RecorderBase
    {
        int defDuration = 1000;

        //读取默认延时
        private void SetDefaultDuration(Func<int>? _defDuration)
        {
            defDuration = _defDuration?.Invoke() ?? defDuration;
        }

        //L2R2以及摇杆二值化
        private void Normalized(in DS4StateLite _real, in DS4StateLite _virtual)
        {
            //L2 R2
            Normalizer.Instance.TriggerNormalize(_real.L2, out _virtual.L2);
            Normalizer.Instance.TriggerNormalize(_real.R2, out _virtual.R2);
            //Left Stick
            //Right Stick
            Normalizer.Instance.StickNormalize(_real.LX, _real.LY, out _virtual.LX, out _virtual.LY, StickPosition.Left);
            Normalizer.Instance.StickNormalize(_real.RX, _real.RY, out _virtual.RX, out _virtual.RY, StickPosition.Right);
        }

        //记录虚拟手柄按键状态，并通过NotifySend方法与UI的model交互
        private void Record(in DS4StateLite _virtual)
        {
            var obj = _virtual;
            var type = obj.GetType();

            StateMachineListA.ForEach(machine => { machine.Update(key => (bool)(type!.GetField(key)!.GetValue(obj)!)); });
            StateMachineListB.ForEach(machine => { machine.Update(key => (byte)(type!.GetField(key)!.GetValue(obj)!) > 0); });
            StateMachineLS.Update(Normalizer.Instance.LeftAngleIdx);
            StateMachineRS.Update(Normalizer.Instance.RightAngleIdx);
        }

        //具体的发送方法
        private void NotifySend(string key, string value)
        {
            int holdtime = (int)this.InnerStopwatch.ElapsedMilliseconds;
            Print($"{holdtime} ms");
            Print($"{key}: {value}");
            Send(new() { Holdtime = holdtime, Key = key, State = value, DefaultDuration = defDuration });
            this.InnerStopwatch.Restart();
        }
    }

    //子类可见
    public partial class RecorderBase
    {
        private protected List<RecorderAction>? Actions;

        private protected void TrySetDefaultDuration(in Func<int>? _setDefDuration)
        {
            SetDefaultDuration(_setDefDuration);
        }
        private protected void TryNormalized(in DS4StateLite _real, in DS4StateLite _virtual)
        {
            Normalized(_real, _virtual);
        }
        private protected void TryRecord(in DS4StateLite _virtual)
        {
            Record(in _virtual);
        }

        private protected void Print([CallerMemberName] string str = "")
        {
            Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, str);
        }
        private protected void Send(RecorderData obj)
        {
            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.Record, obj);
        }
    }
}

//Controller
namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base
{
    //单例
    partial class RecorderController
    {
        private static readonly Lazy<RecorderController> lazyObject = new(() => new RecorderController());
        public static RecorderController Instance => lazyObject.Value;

        private RecorderController() { }
    }

    //入口
    sealed partial class RecorderController : RecorderBase
    {
        public void Record(in DS4StateLite _real, in DS4StateLite _virtual, in Func<int>? _setDefDuration)
        {
            base.TrySetDefaultDuration(in _setDefDuration);
            base.TryNormalized(in _real, in _virtual);
            base.TryRecord(in _virtual);
        }

        public void Playback(DS4StateLite _virtual)
        {
            this.ExecuteMacro(_virtual);
        }
    }

    //复制粘贴
    sealed partial class RecorderController
    {
        /// <summary>
        /// 该值为true时触发脚本
        /// </summary>
        public bool Start_Condition { get => macro_start_condition; set => macro_start_condition = value; }
        /// <summary>
        /// 该值为true时中断脚本
        /// </summary>
        public bool Stop_Condition { get => macro_stop_condition; set => macro_stop_condition = value; }
        /// <summary>
        /// 该值为true时令脚本可以循环
        /// </summary>
        public bool Repeat_Condition { get => macro_repeat_condition; set => macro_repeat_condition = value; }

        /// <summary>
        /// 该值为true时意为正在播放宏
        /// </summary>
        public bool MacroTaskIsRunning => macro_task_is_running;
    }

    //播放
    sealed partial class RecorderController
    {
        List<RecorderAction>? macro_actioninfo_list => Actions;
        //
        bool macro_start_condition = false;
        bool macro_stop_condition = false;
        bool macro_repeat_condition = false;

        bool macro_task_locker = false;
        bool macro_task_is_running = false;
        bool macro_task_cancelflag = false;

        bool macro_not_empty => macro_actioninfo_list is not null && macro_actioninfo_list.Count > 0;

        DS4StateLite v_src = new();

        CancellationTokenSource? macro_cts;
        bool macro_cts_is_disposed = true;

        public void ExecuteMacro(DS4StateLite _virtual)
        {
            if (macro_stop_condition)
            {
                if (macro_task_is_running is false) { macro_task_locker = false; return; }
                if (macro_task_cancelflag is false) { macro_task_cancelflag = true; }
                if (macro_cts_is_disposed is false) { macro_cts?.Cancel(); }
            };

            if (macro_start_condition)
            {
                if (macro_task_locker is false && macro_task_is_running is false)
                {
                    macro_task_locker = true;//上锁
                    macro_task_cancelflag = false;

                    ((Func<Task>)(async () =>
                    {
                        using (macro_cts = new())
                        {
                            macro_cts_is_disposed = false;
                            {
                                var current_token = macro_cts.Token;
                                var canceled = false;
                                var count = 0;

                                Print($"Record Player Start");
                                {
                                    macro_task_is_running = true;//二次上锁
                                    {
                                        try
                                        {
                                            do
                                            {
                                                Print(macro_repeat_condition ? $"Loop({count++})" : $"No loop");

                                                if (macro_not_empty)
                                                {
                                                    v_src.Reset(); //播放前将v_src复位
                                                    foreach (var item in macro_actioninfo_list!)
                                                    {
                                                        if (macro_task_cancelflag) { break; }

                                                        switch (item.Type)
                                                        {
                                                            case RecorderKeyType.LeftStick:
                                                                v_src.LX = item.X; v_src.LY = item.Y;
                                                                break;
                                                            case RecorderKeyType.RightStick:
                                                                v_src.RX = item.X; v_src.RY = item.Y;
                                                                break;
                                                            case RecorderKeyType.Trigger:
                                                                UpdateNow(item.Key, (byte)item.Value);
                                                                break;
                                                            case RecorderKeyType.Button:
                                                                UpdateNow(item.Key, (bool)item.Value);
                                                                break;
                                                        }

                                                        var duration = item.Duration;
                                                        var token = duration < 120 ? CancellationToken.None : current_token;
                                                        {
                                                            await Task.Delay(duration, token).ConfigureAwait(false);
                                                        }
                                                    }
                                                }
                                            }
                                            while (macro_repeat_condition && (macro_task_cancelflag is false) && macro_not_empty);
                                        }
                                        catch
                                        {
                                            canceled = true;
                                        }
                                    }
                                    macro_task_is_running = false;
                                }
                                Print($"Record Player End {(canceled ? "(cancel a long-running delay task)" : string.Empty)}");
                            }
                            macro_cts_is_disposed = true;
                        }
                    }))();
                }
            }
            else
            {
                macro_task_locker = false;//锁1复位
            }

            if (macro_task_is_running) { v_src?.CopyTo(ref _virtual); }
        }

        private void UpdateNow<T>(string propName, T propValue)
        {
            v_src?.GetType()?.GetField(propName)?.SetValue(v_src, propValue);
        }
    }
}

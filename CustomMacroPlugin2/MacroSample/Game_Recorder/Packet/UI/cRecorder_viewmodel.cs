using CustomMacroBase.Helper;
using CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    //单例
    public partial class cRecorder_viewmodel
    {
        private static readonly Lazy<cRecorder_viewmodel> lazyObject = new(() => new cRecorder_viewmodel() { Title = $"{DateTime.Now}" });
        public static cRecorder_viewmodel Instance => lazyObject.Value;
    }

    public partial class cRecorder_viewmodel : NotificationObject
    {
        cRecorder_model model = new();

        public string Title
        {
            get { return model.Title; }
            set { model.Title = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<Minunit> ItemsSource
        {
            get { return model.ItemsSource; }
            set { model.ItemsSource = value; NotifyPropertyChanged(); }
        }

        public ICommand? ApplyBtnCommand
        {
            get { return model.ApplyBtnCommand; }
            set { model.ApplyBtnCommand = value; NotifyPropertyChanged(); }
        }
        public ICommand? AddBtnCommand
        {
            get { return model.AddBtnCommand; }
            set { model.AddBtnCommand = value; NotifyPropertyChanged(); }
        }
        public ICommand? ClearBtnCommand
        {
            get { return model.ClearBtnCommand; }
            set { model.ClearBtnCommand = value; NotifyPropertyChanged(); }
        }
        public ICommand? StartBtnCommand
        {
            get { return model.StartBtnCommand; }
            set { model.StartBtnCommand = value; NotifyPropertyChanged(); }
        }
        public ICommand? StopBtnCommand
        {
            get { return model.StopBtnCommand; }
            set { model.StopBtnCommand = value; NotifyPropertyChanged(); }
        }

        public Action? DeleteAction
        {
            get { return model.DeleteAction; }
            set { model.DeleteAction = value; NotifyPropertyChanged(); }
        }

        public bool ItemHitTest
        {
            get { return model.ItemHitTest; }
            set { model.ItemHitTest = value; NotifyPropertyChanged(); }
        }
    }

    public partial class cRecorder_viewmodel
    {
        List<string> keyList = RecorderKeyList.Instance.ButtonList.Concat(RecorderKeyList.Instance.TriggerList).ToList();
        List<string> valueList = new() { "press", "release", "↑", "↓", "← ", "→", "↖", "↗", "↙", "↘", "●" };

        private cRecorder_viewmodel()
        {
            //Init_Combobox
            keyList.Insert(0, string.Empty); keyList.AddRange(new List<string>() { "Left Stick", "Right Stick" });
            valueList.Insert(0, string.Empty);

            //Init_Command
            this.ApplyBtnCommand = new RelayCommand(_ => { ApplyBtnAction(); });
            this.AddBtnCommand = new RelayCommand(_ => { AddBtnAction(); });
            this.ClearBtnCommand = new RelayCommand(_ => { ClearBtnAction(); });
            this.StartBtnCommand = new RelayCommand(_ => { StartBtnAction(); });
            this.StopBtnCommand = new RelayCommand(_ => { StopBtnAction(); });

            //Init_Delegate
            int previous_duration = 0;
            Mediator.Instance.Register(RecorderMessageType.Instance.Record, (para) =>
            {
                previous_duration = ((RecorderData)para).Holdtime;//'上个动作'到'当前动作'的时间间隔
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (this.ItemsSource.Count > 0) { this.ItemsSource[this.ItemsSource.Count - 1].Duration = previous_duration; }
                        this.ItemsSource.Add(new()
                        {
                            Name = $"{this.ItemsSource.Count}",
                            KeyList = keyList,
                            ValueList = valueList,
                            Parent = this.ItemsSource,
                            SelectedKey = keyList.IndexOf(((RecorderData)para).Key),
                            SelectedValue = valueList.IndexOf(((RecorderData)para).State),
                            Duration = ((RecorderData)para).DefaultDuration //0
                        });
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                });
            });

            dynamic? currentItemModel = null;
            Mediator.Instance.Register(RecorderMessageType.Instance.GetCurrentRecorderMouseEnterItemModel, (para) => { currentItemModel = para; });

            Mediator.Instance.Register(RecorderMessageType.Instance.ItemHitTest, (para) =>
            {
                ItemHitTest = ((bool)para);
                //Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, $"ItemHitTest = {ItemHitTest}");
            });

            //Init_DeleteAction
            this.DeleteAction = () =>
            {
                if (currentItemModel is not null)
                {
                    ItemsSource.Remove((Minunit)currentItemModel);//删除当前鼠标经过的那个Item
                }
            };
        }
    }

    public partial class cRecorder_viewmodel
    {
        private protected void Print([CallerMemberName] string str = "")
        {
            Mediator.Instance.NotifyColleagues(MessageType.PrintNewMessage, str);
        }
        private protected void PrintClear([CallerMemberName] string str = "")
        {
            Mediator.Instance.NotifyColleagues(MessageType.PrintCleanup, str);
        }

        private protected void Start([CallerMemberName] string str = "")
        {
            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.StartRecordedAction, str);
        }
        private protected void Stop([CallerMemberName] string str = "")
        {
            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.StopRecordedAction, str);
        }

        //
        private void TrySetRecordedItemToStateList()
        {
            List<RecorderAction> statelist = new();
            List<string> directions = new() { "↑", "↓", "← ", "→", "↖", "↗", "↙", "↘" };

            //临时防呆
            foreach (var item in this.ItemsSource)
            {
                string key = item.KeyList[item.SelectedKey];
                string value = item.ValueList[item.SelectedValue];

                switch (key)
                {
                    case "Left Stick":
                    case "Right Stick":
                        if (directions.IndexOf(value) == -1)
                        {
                            item.SelectedValue = 11;//居中●  //valueList.IndexOf("●") 
                        }
                        break;
                    case "L2":
                    case "R2":
                        if (value != "press" && value != "release")
                        {
                            item.SelectedValue = 2;//release //valueList.IndexOf("release")
                        }
                        break;
                    default:
                        if (string.IsNullOrEmpty(key)) { break; }
                        if (value != "press" && value != "release")
                        {
                            item.SelectedValue = 2;//release //valueList.IndexOf("release")
                        }
                        break;
                }
            }

            //读取动作
            foreach (var item in this.ItemsSource)
            {
                string key = item.KeyList[item.SelectedKey];
                string value = item.ValueList[item.SelectedValue];
                int duration = item.Duration;
                double slant = 128 / Math.Sqrt(2);

                switch (key)
                {
                    case "Left Stick":
                        {
                            byte x, y;
                            switch (directions.IndexOf(value))
                            {
                                case 0: x = 128; y = 0; break;
                                case 1: x = 128; y = 255; break;
                                case 2: x = 0; y = 128; break;
                                case 3: x = 255; y = 128; break;
                                case 4: x = (byte)(127 - slant); y = (byte)(127 - slant); break;
                                case 5: x = (byte)(127 + slant); y = (byte)(127 - slant); break;
                                case 6: x = (byte)(127 - slant); y = (byte)(127 + slant); break;
                                case 7: x = (byte)(127 + slant); y = (byte)(127 + slant); break;
                                default: x = 128; y = 128; break;
                            }
                            statelist.Add(new() { Type = RecorderKeyType.LeftStick, X = x, Y = y, Duration = duration });
                        }
                        break;

                    case "Right Stick":
                        {
                            byte x, y;
                            switch (directions.IndexOf(value))
                            {
                                case 0: x = 128; y = 0; break;
                                case 1: x = 128; y = 255; break;
                                case 2: x = 0; y = 128; break;
                                case 3: x = 255; y = 128; break;
                                case 4: x = (byte)(127 - slant); y = (byte)(127 - slant); break;
                                case 5: x = (byte)(127 + slant); y = (byte)(127 - slant); break;
                                case 6: x = (byte)(127 - slant); y = (byte)(127 + slant); break;
                                case 7: x = (byte)(127 + slant); y = (byte)(127 + slant); break;
                                default: x = 128; y = 128; break;
                            }
                            statelist.Add(new() { Type = RecorderKeyType.RightStick, X = x, Y = y, Duration = duration });
                        }
                        break;
                    case "L2":
                    case "R2":
                        {
                            byte v = value switch { "press" => byte.MaxValue, "release" => byte.MinValue, _ => byte.MinValue };
                            statelist.Add(new() { Type = RecorderKeyType.Trigger, Key = key, Value = v, Duration = duration });
                        }
                        break;

                    default:
                        {
                            if (string.IsNullOrEmpty(key)) { break; }
                            bool v = value switch { "press" => true, "release" => false, _ => false };
                            statelist.Add(new() { Type = RecorderKeyType.Button, Key = key, Value = v, Duration = duration });
                        }
                        break;
                }
            }

            //检查
            bool delay_check = false;//简易防呆
            int valid_action_count = 0;//有效动作数量
            foreach (var item in statelist)
            {
                if (item.Duration > 0)
                {
                    delay_check = true;
                    valid_action_count++;
                }
            }

            //提前打印
            {
                this.Title = $"{DateTime.Now} (duration check: {valid_action_count}/{statelist.Count})";
                Print($"Ratio of actions with non-zero duration to total actions: {valid_action_count}/{statelist.Count}");
            }

            if (statelist.Count > 0 && delay_check is false) { return; }

            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.ApplyRecord, statelist);
        }


        private void ApplyBtnAction()
        {
            this.PrintClear();
            this.TrySetRecordedItemToStateList();
        }
        private void AddBtnAction()
        {
            this.ItemsSource.Add(new() { Name = $"{this.ItemsSource.Count}", KeyList = keyList, ValueList = valueList, Parent = this.ItemsSource }); //Print("Add");
        }
        private void ClearBtnAction()
        {
            this.PrintClear();
            this.ItemsSource.Clear();
            this.TrySetRecordedItemToStateList();
        }
        private void StartBtnAction()
        {
            this.Start();
        }
        private void StopBtnAction()
        {
            this.Stop();
        }
    }
}

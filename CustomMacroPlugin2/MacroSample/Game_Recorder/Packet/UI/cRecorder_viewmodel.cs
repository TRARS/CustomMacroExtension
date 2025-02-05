using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CustomMacroBase.Messages;
using CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    //单例
    public partial class cRecorder_viewmodel
    {
        private static readonly Lazy<cRecorder_viewmodel> lazyObject = new(() => new cRecorder_viewmodel() { Title = $"{DateTime.Now}" });
        public static cRecorder_viewmodel Instance => lazyObject.Value;
    }

    public partial class cRecorder_viewmodel : ObservableObject
    {
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true,
            WriteIndented = true
        };

        [ObservableProperty]
        private string title;

        public ObservableCollection<Minunit> ItemsSource { get; set; } = new();

        [ObservableProperty]
        private bool itemHitTest;

        private Minunit? currentItemModel;
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

            //Init_Delegate
            int previous_duration = 0;
            WeakReferenceMessenger.Default.Register<Record>(this, (r, m) =>
            {
                var recorderData = m.Value;

                previous_duration = recorderData.Holdtime;//'上个动作'到'当前动作'的时间间隔
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
                            SelectedKey = keyList.IndexOf(recorderData.Key),
                            SelectedValue = valueList.IndexOf(recorderData.State),
                            Duration = recorderData.DefaultDuration //0
                        });
                        Print($"k:{recorderData.Key}, v:{recorderData.State}");
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                });
            });

            WeakReferenceMessenger.Default.Register<GetCurrentRecorderMouseEnterItemModel>(this, (r, m) =>
            {
                currentItemModel = m.Value;
            });

            WeakReferenceMessenger.Default.Register<ItemHitTest>(this, (r, m) =>
            {
                ItemHitTest = m.Value;
            });
        }
    }

    public partial class cRecorder_viewmodel
    {
        private protected void Print([CallerMemberName] string str = "")
        {
            WeakReferenceMessenger.Default.Send(new PrintNewMessage(str));
        }
        private protected void PrintClear([CallerMemberName] string str = "")
        {
            WeakReferenceMessenger.Default.Send(new PrintCleanup(str));
        }

        private protected void Start([CallerMemberName] string str = "")
        {
            WeakReferenceMessenger.Default.Send(new StartRecordedAction(str));
        }
        private protected void Stop([CallerMemberName] string str = "")
        {
            WeakReferenceMessenger.Default.Send(new StopRecordedAction(str));
        }

        private protected void SetCurrentActions(List<RecorderAction>? statelist)
        {
            if (statelist is null) { return; }
            WeakReferenceMessenger.Default.Send(new ApplyRecord(statelist));
        }

        //
        private List<RecorderAction>? TrySetRecordedItemToStateList()
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

            if (statelist.Count > 0 && delay_check is false) { return null; }

            return statelist;
        }

        [RelayCommand]
        private void OnApplyBtn()
        {
            this.PrintClear();
            this.SetCurrentActions(this.TrySetRecordedItemToStateList());
            this.Print();
        }
        [RelayCommand]
        private void OnAddBtn()
        {
            this.ItemsSource.Add(new() { Name = $"{this.ItemsSource.Count}", KeyList = keyList, ValueList = valueList, Parent = this.ItemsSource });
            this.Print();
        }
        [RelayCommand]
        private void OnClearBtn()
        {
            this.PrintClear();
            this.ItemsSource.Clear();
            this.SetCurrentActions(this.TrySetRecordedItemToStateList());
            this.Print();
        }
        [RelayCommand]
        private void OnStartBtn()
        {
            this.Start();
            this.Print();
        }
        [RelayCommand]
        private void OnStopBtn()
        {
            this.Stop();
            this.Print();
        }

        [RelayCommand]
        private void OnDelete()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (currentItemModel is null) { return; }

                ItemsSource.Remove(currentItemModel);//删除当前鼠标经过的那个Item

                this.Print();
            });
        }

        [RelayCommand]
        private void OnSave()
        {
            this.PrintClear();

            var actions = this.ItemsSource;
            if (!(actions is not null && actions.Count > 0)) { return; }

            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var savePath = Path.Combine(desktopPath, "cRecorder.json");
                using (StreamWriter sw = new StreamWriter($"{savePath}", false, Encoding.Unicode))
                {
                    sw.WriteLine(JsonSerializer.Serialize(actions, jsonOptions));
                }
            }
            catch (Exception ex)
            {
                Print(ex.Message);
            }
            finally
            {
                Print();
            }
        }
        [RelayCommand]
        private void OnLoad()
        {
            this.PrintClear();

            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var loadPath = Path.Combine(desktopPath, "cRecorder.json");

                if (!File.Exists(loadPath))
                    throw new FileNotFoundException($"Config file not found at: {loadPath}");

                var json = File.ReadAllText(loadPath, Encoding.Unicode);

                var config = JsonSerializer.Deserialize<List<Minunit>>(json, jsonOptions);

                if (config is not null && config.Count > 0)
                {
                    this.ItemsSource.Clear();
                    foreach (var item in config)
                    {
                        item.Name = $"{this.ItemsSource.Count}";
                        item.KeyList = keyList;
                        item.ValueList = valueList;
                        item.Parent = this.ItemsSource;
                        this.ItemsSource.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Print(ex.Message);
            }
            finally
            {
                Print();
            }
        }
    }
}

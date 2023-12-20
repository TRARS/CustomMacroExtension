using CustomMacroBase;
using CustomMacroBase.CustomControlEx.NormalButtonEx;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CustomMacroPlugin2.MacroSample.Game_FileSort
{
    partial class Game_FileSort
    {
        class FileInfoModel
        {
            public string Path { get; }
            public string Name { get; }
            public string CreationTime { get; }
            public string LastWriteTime { get; }

            public FileInfoModel(string _path)
            {
                Path = _path;
                Name = System.IO.Path.GetFileName(Path);
                CreationTime = new FileInfo(Path).CreationTime.ToString();
                LastWriteTime = new FileInfo(Path).LastWriteTime.ToString();
            }
        }
    }

    [SortIndex(101)]
    partial class Game_FileSort : MacroBase
    {
        public override void Init()
        {
            this.UseColorfulText = true;

            MainGate.Text = "FileSort";

            MainGate.AddEx(() =>
            {
                var border = new Border() 
                { 
                    BorderThickness = new(1),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Margin = new(0,4,0,0)
                };
                {
                    var stackpanel = new StackPanel() { Orientation = Orientation.Vertical };
                    {
                        var listbox = new ListBox()
                        {
                            MinWidth = 320,
                            Height = 240,
                            Background = new SolidColorBrush(Colors.White),
                            BorderBrush = new SolidColorBrush(Colors.Transparent),
                            AllowDrop = true
                        };
                        {
                            var _List = new ObservableCollection<FileInfoModel>();

                            listbox.DragEnter += (s, e) =>
                            {
                                listbox.AddAdorner();
                                if (e.Data.GetDataPresent(DataFormats.FileDrop)) { e.Effects = DragDropEffects.All; }
                            };
                            listbox.DragLeave += (s, e) =>
                            {
                                listbox.RemoveAdorner();
                            };
                            listbox.PreviewDrop += (s, e) =>
                            {
                                listbox.RemoveAdorner();
                                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                                {
                                    _List.Clear();
                                    foreach (var item in e.Data.GetData(DataFormats.FileDrop) as Array)
                                    {
                                        _List.Add(new(item.ToString()));
                                    }
                                }
                            };
                            listbox.Loaded += (s, e) =>
                            {
                                Func<FileInfoModel?, FileInfoModel?, bool?> isMoveUp = (source, target) =>
                                {
                                    if (source is null || target is null) return null;
                                    int removedIdx = listbox.Items.IndexOf(source);
                                    int targetIdx = listbox.Items.IndexOf(target);
                                    if (removedIdx == targetIdx) return null;
                                    return (removedIdx > targetIdx);
                                };

                                listbox.DisplayMemberPath = "Name";
                                listbox.ItemsSource = _List;

                                Style itemContainerStyle = new(typeof(ListBoxItem));
                                itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
                                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler((s, e) =>
                                {
                                    if (s is ListBoxItem)
                                    {
                                        ListBoxItem draggedItem = (s as ListBoxItem)!;
                                        draggedItem.IsSelected = true;
                                        DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                                    }
                                })));
                                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DragEnterEvent, new DragEventHandler((s, e) =>
                                {
                                    if (s is ListBoxItem)
                                    {
                                        FileInfoModel? source = e.Data.GetData(typeof(FileInfoModel)) as FileInfoModel;
                                        FileInfoModel? target = ((ListBoxItem)s).DataContext as FileInfoModel;
                                        bool? flag = isMoveUp(source, target);
                                        if (source is not null && target is not null)
                                        {
                                            ((ListBoxItem)s).AddAdorner(flag);
                                        }
                                    }
                                })));
                                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DragLeaveEvent, new DragEventHandler((s, e) =>
                                {
                                    if (s is ListBoxItem)
                                    {
                                        ((ListBoxItem)s).RemoveAdorner();
                                    }
                                })));
                                itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler((s, e) =>
                                {
                                    FileInfoModel? source = e.Data.GetData(typeof(FileInfoModel)) as FileInfoModel;
                                    FileInfoModel? target = ((ListBoxItem)s).DataContext as FileInfoModel;

                                    ((ListBoxItem)s).RemoveAdorner();

                                    if (source is null || source is not FileInfoModel) { return; };

                                    int removedIdx = listbox.Items.IndexOf(source);
                                    int targetIdx = listbox.Items.IndexOf(target);

                                    if (removedIdx < targetIdx)
                                    {
                                        _List.Insert(targetIdx + 1, source);
                                        _List.RemoveAt(removedIdx);
                                    }
                                    else
                                    {
                                        int remIdx = removedIdx + 1;
                                        if (_List.Count + 1 > remIdx)
                                        {
                                            _List.Insert(targetIdx, source);
                                            _List.RemoveAt(remIdx);
                                        }
                                    }
                                })));
                                listbox.ItemContainerStyle = itemContainerStyle;
                            };
                        }

                        var button = new cNormalButton() { Text = "ReSort" };
                        {
                            button.Click += (s, e) =>
                            {
                                var second = 0;
                                var minute = 0;
                                var hour = 0;
                                if (listbox.ItemsSource is ObservableCollection<FileInfoModel> modelList)
                                {
                                    foreach (var item in modelList)
                                    {
                                        var fi = new FileInfo(item.Path);
                                        fi.CreationTime = fi.LastWriteTime = DateTime.Parse($"2037/01/01 {hour}:{minute}:{second}");
                                        minute++;
                                        if (second >= 60) { second = 0; minute++; }
                                        if (minute >= 60) { minute = 0; hour++; }
                                        if (hour >= 24) { hour = 0; }
                                        Print($"{item.CreationTime} -> {item.Name}");
                                    }
                                }
                            };
                        }

                        stackpanel.Children.Add(listbox);
                        stackpanel.Children.Add(button);
                    }

                    border.Child = stackpanel;
                }
                return border;
            });
            MainGate.AddEx(() => new TextBlock() { Text = "- Drag and drop the files that need to be sorted by time into the box.", Foreground = new SolidColorBrush(Colors.White) });
        }

        public override void UpdateState()
        {

        }
    }
}

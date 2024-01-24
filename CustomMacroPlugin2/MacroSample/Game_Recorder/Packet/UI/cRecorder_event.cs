using CustomMacroBase.Helper;
using CustomMacroBase.Helper.Extensions;
using CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    public partial class cRecorder_event
    {
        bool isDrag;
        ListBoxItem? move_source = null;
        ListBoxItem? move_target = null;
        Func<ObservableCollection<Minunit>, Minunit?, Minunit?, bool?> isMoveUp = (parent, source, target) =>
        {//不得使用dynamic/object
            if (source is null || target is null) { return null; }
            int removedIdx = parent.IndexOf(source);
            int targetIdx = parent.IndexOf(target);
            if (removedIdx == targetIdx) { return null; }
            return (removedIdx > targetIdx);
        };

        //ListBoxItemMouseEvent
        private void PreviewMouseLeftButtonDown(object s, MouseButtonEventArgs e)
        {
            var p = e.GetPosition((ListBoxItem)s);
            if (p.X > 0 && p.Y > 0 && p.X < 22 && p.Y < 22)
            {
                isDrag = true;
            }
            if (isDrag)
            {
                move_source = (ListBoxItem)s;
                move_source.Opacity = 0.5;

                Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.ItemHitTest, false);
            }
        }
        private void PreviewMouseLeftButtonUp(object s, MouseButtonEventArgs e)
        {
            move_target = (ListBoxItem)s;
            if (move_source is not null) { move_source.Opacity = 1; }
        }
        private void PreviewMouseMove(object s, MouseEventArgs e)
        {
            if (isDrag && move_source is not null)
            {
                try
                {
                    if (((ListBoxItem)s).DataContext is Minunit pre_move_target)
                    {
                        var parent = ((Minunit)move_source.DataContext).Parent;
                        var source = (Minunit)move_source.DataContext;
                        var target = pre_move_target;
                        (s as ListBoxItem)?.AddAdorner(isMoveUp(parent, source, target));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"PreviewMouseMove Error {ex.Message}");
                    move_source = move_target = null;
                }
            }
        }
        private void MouseLeave(object s, MouseEventArgs e)
        {
            ((ListBoxItem)s).RemoveAdorner();
            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.GetCurrentRecorderMouseEnterItemModel, null);
        }
        private void MouseEnter(object s, MouseEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.GetCurrentRecorderMouseEnterItemModel, (Minunit)((ListBoxItem)s).DataContext);
        }
    }

    public partial class cRecorder_event
    {
        private double lastExtentHeight = 0;

        //ListBoxMouseEvent
        private void ListBox_MouseLeftButtonUp(object s, MouseButtonEventArgs e)
        {
            try
            {
                if (move_source is not null) { move_source.Opacity = 1; }
                if (move_source is not null && move_target is not null)
                {
                    var source = (Minunit)move_source.DataContext;
                    var target = (Minunit)move_target.DataContext;
                    var parent = source.Parent;
                    int removedIdx = parent.IndexOf(source);
                    int targetIdx = parent.IndexOf(target);

                    if (removedIdx == targetIdx) { return; }
                    if (removedIdx < targetIdx)
                    {
                        parent.Insert(targetIdx + 1, source);
                        parent.RemoveAt(removedIdx);
                    }
                    else
                    {
                        int remIdx = removedIdx + 1;
                        if (parent.Count + 1 > remIdx)
                        {
                            parent.Insert(targetIdx, source);
                            parent.RemoveAt(remIdx);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"ListBox_MouseLeftButtonUp Error {ex.Message}"); }
            finally
            {
                move_source = move_target = null;
                isDrag = false;
                Mediator.Instance.NotifyColleagues(RecorderMessageType.Instance.ItemHitTest, true);
            }
        }
        private void ListBox_OnScrollChanged(object s, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer)
            {
                double currentExtentHeight = e.ExtentHeight;

                if (Math.Abs(e.ExtentHeightChange) > 0.0 && currentExtentHeight > lastExtentHeight)
                {
                    scrollViewer.ScrollToBottom();
                }

                lastExtentHeight = currentExtentHeight;
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (((ComboBox)sender).IsDropDownOpen is false)
            {
                e.Handled = true;
            }
        }
    }
}

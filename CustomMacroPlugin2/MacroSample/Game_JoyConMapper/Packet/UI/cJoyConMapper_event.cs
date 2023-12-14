using CustomMacroBase.Helper;
using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base;
using System.Windows.Controls;
using System.Windows.Input;
using KeyboardKeys = System.Windows.Forms.Keys;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI
{
    partial class cJoyConMapper_event
    {
        private void ListBoxItem_MouseLeave(object s, MouseEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(JoyConMapperMessageType.Instance.GetCurrentJoyConMapperMouseEnterItemModel, null);
        }
        private void ListBoxItem_MouseEnter(object s, MouseEventArgs e)
        {
            Mediator.Instance.NotifyColleagues(JoyConMapperMessageType.Instance.GetCurrentJoyConMapperMouseEnterItemModel, (MappingInfoPacket<KeyboardKeys>)((ListBoxItem)s).DataContext);
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

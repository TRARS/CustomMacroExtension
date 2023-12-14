using System.Windows;
using System.Windows.Controls;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI
{
    public class cJoyConMapper : Control
    {
        static cJoyConMapper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cJoyConMapper), new FrameworkPropertyMetadata(typeof(cJoyConMapper)));
        }

        public cJoyConMapper()
        {
            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            DataContext = cJoyConMapper_viewmodel.Instance;
        }
    }
}

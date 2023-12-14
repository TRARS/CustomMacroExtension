using System.Windows;
using System.Windows.Controls;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    public class cRecorder : Control
    {
        static cRecorder()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(cRecorder), new FrameworkPropertyMetadata(typeof(cRecorder)));
        }

        public cRecorder()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;

            DataContext = cRecorder_viewmodel.Instance;
        }
    }
}

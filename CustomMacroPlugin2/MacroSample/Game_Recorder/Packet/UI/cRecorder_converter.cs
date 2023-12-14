using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shapes;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    public class cRecorder_converter_listcheck : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<UIElement>)
            {
                return value;
            }

            if (value is Path path)
            {
                return new List<UIElement>() { path };
            }

            if (value is Path[] pathList)
            {
                return new List<UIElement>() { pathList[0] };
            }

            return new List<UIElement>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

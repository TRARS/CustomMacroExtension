using CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;
using KeyboardKeys = System.Windows.Forms.Keys;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.UI
{
    internal class cJoyConMapper_model
    {
        public Thickness? CurrentBtnPos { get; set; }
        public BitmapImage GamePadImage { get; set; }
        public ObservableCollection<MappingInfoPacket<KeyboardKeys>> KeyboardMappingInfoList { get; set; }
        //public ObservableCollection<MappingInfoPacket<MouseKeys>> MouseMappingInfoList { get; set; }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    public partial class Minunit : ObservableObject
    {
        [property: JsonIgnore]
        [ObservableProperty]
        private string name = string.Empty;

        [property: JsonIgnore]
        [ObservableProperty]
        private List<string> keyList = new();

        [property: JsonIgnore]
        [ObservableProperty]
        private List<string> valueList = new();

        [JsonIgnore]
        public ObservableCollection<Minunit> Parent { get; set; } = new();

        [ObservableProperty]
        private int selectedKey = 0;

        [ObservableProperty]
        private int selectedValue = 0;

        [ObservableProperty]
        private int duration = 0;
    }
}

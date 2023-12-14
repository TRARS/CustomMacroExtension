using CustomMacroBase.Helper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI
{
    public class Minunit : NotificationObject
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }

        private List<string> _KeyList = new();
        public List<string> KeyList
        {
            get { return _KeyList; }
            set { _KeyList = value; NotifyPropertyChanged(); }
        }

        private List<string> _ValueList = new();
        public List<string> ValueList
        {
            get { return _ValueList; }
            set { _ValueList = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<Minunit> _Parent = new();
        public ObservableCollection<Minunit> Parent
        {
            get { return _Parent; }
            set { _Parent = value; NotifyPropertyChanged(); }
        }

        private int _SelectedKey = 0;
        public int SelectedKey
        {
            get { return _SelectedKey; }
            set { _SelectedKey = value; NotifyPropertyChanged(); }
        }

        private int _SelectedValue = 0;
        public int SelectedValue
        {
            get { return _SelectedValue; }
            set { _SelectedValue = value; NotifyPropertyChanged(); }
        }

        private int _Duration = 0;
        public int Duration
        {
            get { return _Duration; }
            set { _Duration = value; NotifyPropertyChanged(); }
        }
    }

    public class cRecorder_model
    {
        public string Title { get; set; } = string.Empty;
        public ObservableCollection<Minunit> ItemsSource { get; set; } = new();

        public ICommand? ApplyBtnCommand { get; set; } = null;
        public ICommand? AddBtnCommand { get; set; } = null;
        public ICommand? ClearBtnCommand { get; set; } = null;
        public ICommand? StartBtnCommand { get; set; } = null;
        public ICommand? StopBtnCommand { get; set; } = null;

        public Action? DeleteAction { get; set; } = null;
    }
}

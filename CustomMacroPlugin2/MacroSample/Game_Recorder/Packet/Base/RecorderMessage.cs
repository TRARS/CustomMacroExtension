using CommunityToolkit.Mvvm.Messaging.Messages;
using CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.UI;
using System.Collections.Generic;

namespace CustomMacroPlugin2.MacroSample.Game_Recorder.Packet.Base
{
    public class ApplyRecord : ValueChangedMessage<List<RecorderAction>>
    {
        public ApplyRecord(List<RecorderAction> value) : base(value)
        {
        }
    }
    public class StartRecordedAction : ValueChangedMessage<string>
    {
        public StartRecordedAction(string value) : base(value)
        {
        }
    }
    public class StopRecordedAction : ValueChangedMessage<string>
    {
        public StopRecordedAction(string value) : base(value)
        {
        }
    }


    public class Record : ValueChangedMessage<RecorderData>
    {
        public Record(RecorderData value) : base(value)
        {
        }
    }
    public class GetCurrentRecorderMouseEnterItemModel : ValueChangedMessage<Minunit?>
    {
        public GetCurrentRecorderMouseEnterItemModel(Minunit? value) : base(value)
        {
        }
    }
    public class ItemHitTest : ValueChangedMessage<bool>
    {
        public ItemHitTest(bool value) : base(value)
        {
        }
    }
}

using CommunityToolkit.Mvvm.Messaging.Messages;

namespace CustomMacroPlugin2.MacroSample.Game_JoyConMapper.Packet.Base
{
    public class GetCurrentJoyConMapperMouseEnterItemModel<T> : ValueChangedMessage<MappingInfoPacket<T>?>
    {
        public GetCurrentJoyConMapperMouseEnterItemModel(MappingInfoPacket<T>? value) : base(value)
        {
        }
    }
}

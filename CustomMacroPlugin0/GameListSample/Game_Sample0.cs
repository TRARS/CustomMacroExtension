using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample only adds three Toggle buttons on the UI.
//It does not implement macros,
//but only shows how to add Toggle buttons.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(200)]
    partial class Game_Sample0 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Main_ToggleButton";

            MainGate.Add(CreateGateBase("Sub_ToggleButton_0"));
            MainGate.Add(CreateGateBase("Sub_ToggleButton_1"));
            MainGate.Add(CreateGateBase("Sub_ToggleButton_2"));
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { }
            if (MainGate[1].Enable) { }
            if (MainGate[2].Enable) { }
        }
    }
}
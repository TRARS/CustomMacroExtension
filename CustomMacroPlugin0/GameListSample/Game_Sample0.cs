using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample introduces three Toggle buttons to the UI.
//It focuses on demonstrating the addition of Toggle buttons and does not implement macros.
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
using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample only adds three Toggle buttons on the UI interface.
//It does not implement macros,
//but only shows how to add Toggle buttons.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(200)]
    [DoNotLoad]
    partial class Game_Sample0 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Sample0_主开关";

            MainGate.Add(new() { Text = "子开关0", Enable = true });
            MainGate.Add(new() { Text = "子开关1", Enable = true });
            MainGate.Add(new() { Text = "子开关2", Enable = true });
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
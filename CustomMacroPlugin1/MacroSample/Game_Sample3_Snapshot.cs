using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;
using CustomMacroBase.Helper.Tools.FlowManager;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomMacroPlugin1.MacroSample
{
    [SortIndex(999)]
    partial class Game_Sample3_Snapshot : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Root";
            MainGate.Add(CreateTVN("Call FindColor")); //[0]
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            CallFindColor(MainGate[0].Enable);
        }
    }

    partial class Game_Sample3_Snapshot
    {
        FlowControllerV2? CallFindColor_Flow;

        private void CallFindColor(bool canExecute)
        {
            if (!canExecute) { return; }

            CallFindColor_Flow ??= new(nameof(CallFindColor))
            {
                (x, y, z) => { CallFindColor_Detail(ref x[0], ref y[0], ref z); }
            };
            CallFindColor_Flow.Start_Condition = RealDS4.Square; // Start if □ is pressed
            CallFindColor_Flow.Stop_Condition = RealDS4.Circle;  // Stop if ○ is pressed
            CallFindColor_Flow.ExecuteMacro();
        }

        private void CallFindColor_Detail(ref Action _action, ref bool _cancel, ref Func<int, bool> _wait)
        {
            var flag = false;
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            var ActionList_01 = new Dictionary<Action, int>()
            {
                { () => { VirtualDS4.Triangle = true; }, 10000 },
            };

            Task.Run(async () =>
            {
                while (token.IsCancellationRequested is false)
                {
                    //if (MacroBase.FindColor(color: 0xFFFFFF, rect: new(0, 0, 100, 100), tolerance: 20) is int white)
                    //{
                    //    flag = white > 0;

                    //    Print($"The total number of white pixels in the rectangle (0,0,100,100) is {white}.");
                    //}

                    var text = MacroBase.FindText(new(105, 245, 145, 24), CustomMacroBase.PixelMatcher.OpenCV.DeviceType.Mkldnn, CustomMacroBase.PixelMatcher.OpenCV.ModelType.EnglishV3);

                    Print($"text: {text}");

                    await Task.Delay(256, token);
                }
            }, token);

            while (_cancel is false)
            {
                if (flag)
                {
                    ExecuteAction(ActionList_01, in _cancel, ref _action, in _wait, cts);
                }
            }

            cts.Cancel();
        }

        private void ExecuteAction(Dictionary<Action, int> actList, in bool _cancel, ref Action _action, in Func<int, bool> _wait, CancellationTokenSource cts)
        {
            foreach (var item in actList)
            {
                if (_cancel) { cts.Cancel(); break; }

                _action = item.Key;
                _wait.Invoke(item.Value);
            }

            _action = null;
        }
    }
}
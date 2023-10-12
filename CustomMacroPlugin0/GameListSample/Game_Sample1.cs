using CustomMacroBase;
using CustomMacroBase.Helper.Attributes;

//This sample adds four Toggle buttons on the UI interface,
//each button corresponds to a remapping,
//but it does not implement macros and is not practical.
namespace CustomMacroPlugin0.GameListSample
{
    [SortIndex(201)]
    //[DoNotLoad]
    partial class Game_Sample1 : MacroBase
    {
        public override void Init()
        {
            MainGate.Text = "Sample1_主开关，均为按○激活脚本，松开○中断脚本";

            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关0，○×", Enable = true });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关1，×", Enable = true });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关2，□以及推左摇杆", Enable = false });
            MainGate.Add(new() { GroupName = "X3mYmBz@", Text = "子开关3，○×□△", Enable = false });
        }

        public override void UpdateState()
        {
            if (MainGate.Enable is false) { return; }

            if (MainGate[0].Enable) { Macro0(); }
            if (MainGate[1].Enable) { Macro1(); }
            if (MainGate[2].Enable) { Macro2(); }
            if (MainGate[3].Enable) { Macro3(); }
        }
    }

    partial class Game_Sample1
    {
        private void Macro0()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Cross = true; //操作(虚拟手柄)按下×
            }
            //最终效果：(虚拟手柄)同时按下○×
        }

        private void Macro1()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Circle = false;//操作(虚拟手柄)弹起○
                VirtualDS4.Cross = true;//操作(虚拟手柄)按下×
            }
            //最终效果：(虚拟手柄)按下×
        }

        private void Macro2()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Circle = false;//操作(虚拟手柄)弹起○
                VirtualDS4.LX = 0;
                VirtualDS4.LY = 0;//操作(虚拟手柄)左摇杆
                VirtualDS4.Square = true;//操作(虚拟手柄)按下□
            }
            //最终效果：(虚拟手柄)按下□的同时左摇杆往左上角推
        }

        private void Macro3()
        {
            //若(真实手柄)按下○
            if (RealDS4.Circle)
            {
                VirtualDS4.Cross = true;//操作(虚拟手柄)按下×
                VirtualDS4.Square = true;//操作(虚拟手柄)按下□
                VirtualDS4.Triangle = true;//操作(虚拟手柄)按下△
            }
            //最终效果：(虚拟手柄)同时按下○×□△
        }
    }
}
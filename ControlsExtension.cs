using System;
using System.Windows.Forms;

namespace BudgetLedger
{
    public static class ControlExtensions
    {
        public static void EnableDoubleBuffering(this Control control)
        {
            // Set the control's DoubleBuffered property to true
            typeof(Control).InvokeMember(
                "DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null,
                control,
                new object[] { true });
        }
    }
}

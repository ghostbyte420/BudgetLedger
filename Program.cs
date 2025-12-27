using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BudgetLedger
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize and run the BudgetLedger application
            ApplicationConfiguration.Initialize();

            // Launch the main form
            var mainForm = new budgetLedgerMain();
            Application.Run(mainForm);
        }
    }
}

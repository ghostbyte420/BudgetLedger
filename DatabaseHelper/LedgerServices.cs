using System;
using System.IO;

namespace BudgetLedger.Data;

internal static class LedgerServices
{
    private static readonly object _gate = new();

    public static LedgerDb Db { get; private set; } = null!;
    public static ExpenseRepository Expenses { get; private set; } = null!;
    public static MonthSummaryRepository MonthSummaries { get; private set; } = null!;

    public static int LedgerYear { get; private set; }

    public static void Initialize(int year)
    {
        lock (_gate)
        {
            LedgerYear = year;

            #region Save Into AppData: Local (Recommended For Desktops)
            /*
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dir = Path.Combine(appData, "BudgetLedger");
            var path = Path.Combine(dir, $"budgetledger_{year}.sqlite");
            */
            #endregion

            #region Save Into Root Directory (Run Application From USB)

            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(rootDir, $"budgetledger_{year}.sqlite");

            #endregion

            Db = new LedgerDb(path);
            Db.Initialize();
            Expenses = new ExpenseRepository(Db);
            MonthSummaries = new MonthSummaryRepository(Db);
        }
    }
}

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace BudgetLedger.Data;

internal static class DataGridLedgerBinder
{
    private const string RecordIdColumnName = "__record_id";

    public static void Bind(DataGridView grid, int month, string category)
    {
        if (grid is null) throw new ArgumentNullException(nameof(grid));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required", nameof(category));

        EnsureHiddenIdColumn(grid);

        grid.Tag = new GridScope(month, category);

        grid.CellValueChanged -= GridOnCellValueChanged;
        grid.UserDeletingRow -= GridOnUserDeletingRow;
        grid.RowsAdded -= GridOnRowsAdded;

        grid.CellValueChanged += GridOnCellValueChanged;
        grid.UserDeletingRow += GridOnUserDeletingRow;
        grid.RowsAdded += GridOnRowsAdded;

        Load(grid);
    }

    private static void EnsureHiddenIdColumn(DataGridView grid)
    {
        if (grid.Columns.Contains(RecordIdColumnName)) return;

        var col = new DataGridViewTextBoxColumn
        {
            Name = RecordIdColumnName,
            HeaderText = string.Empty,
            Visible = false,
            ReadOnly = true
        };

        grid.Columns.Insert(0, col);
    }

    private static void Load(DataGridView grid)
    {
        var scope = (GridScope)grid.Tag!;
        var records = LedgerServices.Expenses.GetByScope(LedgerServices.LedgerYear, scope.Month, scope.Category);

        grid.CellValueChanged -= GridOnCellValueChanged;
        grid.UserDeletingRow -= GridOnUserDeletingRow;
        grid.RowsAdded -= GridOnRowsAdded;

        try
        {
            grid.Rows.Clear();

            foreach (var r in records)
            {
                var rowIndex = grid.Rows.Add();
                var row = grid.Rows[rowIndex];

                row.Cells[RecordIdColumnName].Value = r.Id;

                SetText(row, "What Did You Spend Money On?", r.Expense);
                SetDate(row, "Date", r.Date);
                SetText(row, "Why You Spent Money On This Expense?", r.Descriptor);
                SetDecimal(row, "How Much Was Owed For This Expense?", r.AmountOwed);
                SetDecimal(row, "How Much Did You Pay Toward This Expense?", r.AmountPaid);
                SetText(row, "What Currency Type Was Used To Pay For This Expense?", r.Currency);
                SetText(row, "What Payment Method Did You Use For This Expense?", r.PayMethod);
                SetText(row, "If Credit, What Were The Last Four Digits Of The Card Number Used For This Expense?", r.LastFour);
                SetText(row, "What Was The Confirmation Number Given To You After Payment For This Expense Was Made?", r.Confirmation);
                SetDecimal(row, "Was There An Account Credit?", r.AccountCredit);
                SetText(row, "Does The Company Have A Contact Phone Number?", r.Phone);
                SetText(row, "Whom Did You Pay For Services Rendered?", r.Business);
                SetBool(row, "Was This Expense Paid For The Month?", r.Paid);
            }
        }
        finally
        {
            grid.CellValueChanged += GridOnCellValueChanged;
            grid.UserDeletingRow += GridOnUserDeletingRow;
            grid.RowsAdded += GridOnRowsAdded;
        }
    }

    private static void GridOnRowsAdded(object? sender, DataGridViewRowsAddedEventArgs e)
    {
        if (sender is not DataGridView grid) return;

        for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
        {
            if (i < 0 || i >= grid.Rows.Count) continue;
            var row = grid.Rows[i];
            if (row.IsNewRow) continue;

            if (row.Cells[RecordIdColumnName].Value is null)
            {
                row.Cells[RecordIdColumnName].Value = 0L;
            }
        }
    }

    private static void GridOnUserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
    {
        if (sender is not DataGridView grid) return;

        if (e.Row.IsNewRow) return;
        var id = GetRecordId(e.Row);
        if (id <= 0) return;

        LedgerServices.Expenses.Delete(id);

        // Notify the main form to update the summary
        if (grid.FindForm() is budgetLedgerMain mainForm)
        {
            var scope = (GridScope)grid.Tag!;
            mainForm.NotifyExpenseChanged(scope.Month);
        }
    }

    private static void GridOnCellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (sender is not DataGridView grid) return;
        if (e.RowIndex < 0) return;

        var row = grid.Rows[e.RowIndex];
        if (row.IsNewRow) return;

        var scope = (GridScope)grid.Tag!;
        var record = BuildRecordFromRow(row, scope);

        var id = GetRecordId(row);
        if (id <= 0)
        {
            var newId = LedgerServices.Expenses.Insert(record);
            row.Cells[RecordIdColumnName].Value = newId;
        }
        else
        {
            LedgerServices.Expenses.Update(id, record);
        }

        // Notify the main form to update the summary
        if (grid.FindForm() is budgetLedgerMain mainForm)
        {
            mainForm.NotifyExpenseChanged(scope.Month);
        }
    }

    private static long GetRecordId(DataGridViewRow row)
    {
        var v = row.Cells[RecordIdColumnName].Value;
        if (v is null) return 0;
        if (v is long l) return l;
        return long.TryParse(v.ToString(), out var parsed) ? parsed : 0;
    }

    private static ExpenseRecord BuildRecordFromRow(DataGridViewRow row, GridScope scope)
    {
        return new ExpenseRecord(
            Id: GetRecordId(row),
            Year: LedgerServices.LedgerYear,
            Month: scope.Month,
            Category: scope.Category,
            Expense: GetText(row, "What Did You Spend Money On?"),
            Date: GetDate(row, "Date"),
            Descriptor: GetText(row, "Why You Spent Money On This Expense?"),
            AmountOwed: GetDecimal(row, "How Much Was Owed For This Expense?"),
            AmountPaid: GetDecimal(row, "How Much Did You Pay Toward This Expense?"),
            Currency: GetText(row, "What Currency Type Was Used To Pay For This Expense?"),
            PayMethod: GetText(row, "What Payment Method Did You Use For This Expense?"),
            LastFour: GetText(row, "If Credit, What Were The Last Four Digits Of The Card Number Used For This Expense?"),
            Confirmation: GetText(row, "What Was The Confirmation Number Given To You After Payment For This Expense Was Made?"),
            AccountCredit: GetDecimal(row, "Was There An Account Credit?"),
            Phone: GetText(row, "Does The Company Have A Contact Phone Number?"),
            Business: GetText(row, "Whom Did You Pay For Services Rendered?"),
            Paid: GetBool(row, "Was This Expense Paid For The Month?")
        );
    }

    private static string? GetText(DataGridViewRow row, string columnName)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return null;
        var v = row.Cells[columnName].Value;
        return v?.ToString();
    }

    private static void SetText(DataGridViewRow row, string columnName, string? value)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return;
        row.Cells[columnName].Value = value;
    }

    private static DateTime? GetDate(DataGridViewRow row, string columnName)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return null;
        var v = row.Cells[columnName].Value;
        if (v is null) return null;
        if (v is DateTime dt) return dt;
        return DateTime.TryParse(v.ToString(), out var parsed) ? parsed : null;
    }

    private static void SetDate(DataGridViewRow row, string columnName, DateTime? value)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return;
        row.Cells[columnName].Value = value?.ToShortDateString();
    }

    private static decimal? GetDecimal(DataGridViewRow row, string columnName)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return null;
        var v = row.Cells[columnName].Value;
        if (v is null) return null;
        if (v is decimal dec) return dec;
        if (v is double dbl) return (decimal)dbl;
        if (v is float fl) return (decimal)fl;
        if (decimal.TryParse(v.ToString(), NumberStyles.Any, CultureInfo.CurrentCulture, out var parsed)) return parsed;
        return null;
    }

    private static void SetDecimal(DataGridViewRow row, string columnName, decimal? value)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return;
        row.Cells[columnName].Value = value;
    }

    private static bool? GetBool(DataGridViewRow row, string columnName)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return null;
        var v = row.Cells[columnName].Value;
        if (v is null) return null;
        if (v is bool b) return b;
        if (bool.TryParse(v.ToString(), out var parsed)) return parsed;
        return null;
    }

    private static void SetBool(DataGridViewRow row, string columnName, bool? value)
    {
        if (!row.DataGridView.Columns.Contains(columnName)) return;
        row.Cells[columnName].Value = value;
    }

    private sealed record GridScope(int Month, string Category);
}

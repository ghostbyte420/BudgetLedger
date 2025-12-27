using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace BudgetLedger.Data;

internal sealed class ExpenseRepository
{
    private readonly LedgerDb _db;

    public ExpenseRepository(LedgerDb db) => _db = db;

    public List<ExpenseRecord> GetByScope(int year, int month, string category)
    {
        var result = new List<ExpenseRecord>();

        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
SELECT id, year, month, category,
       expense, date, descriptor,
       amount_owed, amount_paid,
       currency, pay_method,
       last_four, confirmation,
       account_credit, phone,
       business, paid
FROM expense
WHERE year = $year AND month = $month AND category = $category
ORDER BY COALESCE(date, ''), id;
";
        cmd.Parameters.AddWithValue("$year", year);
        cmd.Parameters.AddWithValue("$month", month);
        cmd.Parameters.AddWithValue("$category", category);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new ExpenseRecord(
                Id: reader.GetInt64(0),
                Year: reader.GetInt32(1),
                Month: reader.GetInt32(2),
                Category: reader.GetString(3),
                Expense: reader.IsDBNull(4) ? null : reader.GetString(4),
                Date: reader.IsDBNull(5) ? null : DateTime.Parse(reader.GetString(5), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                Descriptor: reader.IsDBNull(6) ? null : reader.GetString(6),
                AmountOwed: reader.IsDBNull(7) ? null : reader.GetDecimal(7),
                AmountPaid: reader.IsDBNull(8) ? null : reader.GetDecimal(8),
                Currency: reader.IsDBNull(9) ? null : reader.GetString(9),
                PayMethod: reader.IsDBNull(10) ? null : reader.GetString(10),
                LastFour: reader.IsDBNull(11) ? null : reader.GetString(11),
                Confirmation: reader.IsDBNull(12) ? null : reader.GetString(12),
                AccountCredit: reader.IsDBNull(13) ? null : reader.GetDecimal(13),
                Phone: reader.IsDBNull(14) ? null : reader.GetString(14),
                Business: reader.IsDBNull(15) ? null : reader.GetString(15),
                Paid: reader.IsDBNull(16) ? null : reader.GetInt32(16) != 0
            ));
        }

        return result;
    }

    public long Insert(ExpenseRecord record)
    {
        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT INTO expense(
    year, month, category,
    expense, date, descriptor,
    amount_owed, amount_paid,
    currency, pay_method,
    last_four, confirmation,
    account_credit, phone,
    business, paid,
    created_utc, updated_utc
)
VALUES(
    $year, $month, $category,
    $expense, $date, $descriptor,
    $amount_owed, $amount_paid,
    $currency, $pay_method,
    $last_four, $confirmation,
    $account_credit, $phone,
    $business, $paid,
    $created_utc, $updated_utc
);
SELECT last_insert_rowid();
";

        Bind(cmd, record);
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        cmd.Parameters.AddWithValue("$created_utc", now);
        cmd.Parameters.AddWithValue("$updated_utc", now);

        return (long)cmd.ExecuteScalar()!;
    }

    public void Update(long id, ExpenseRecord record)
    {
        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
UPDATE expense
SET expense = $expense,
    date = $date,
    descriptor = $descriptor,
    amount_owed = $amount_owed,
    amount_paid = $amount_paid,
    currency = $currency,
    pay_method = $pay_method,
    last_four = $last_four,
    confirmation = $confirmation,
    account_credit = $account_credit,
    phone = $phone,
    business = $business,
    paid = $paid,
    updated_utc = $updated_utc
WHERE id = $id;
";
        cmd.Parameters.AddWithValue("$id", id);
        Bind(cmd, record);
        cmd.Parameters.AddWithValue("$updated_utc", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
        cmd.ExecuteNonQuery();
    }

    public void Delete(long id)
    {
        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM expense WHERE id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    private static void Bind(SqliteCommand cmd, ExpenseRecord record)
    {
        cmd.Parameters.AddWithValue("$year", record.Year);
        cmd.Parameters.AddWithValue("$month", record.Month);
        cmd.Parameters.AddWithValue("$category", record.Category);

        cmd.Parameters.AddWithValue("$expense", (object?)record.Expense ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$date", record.Date is null ? DBNull.Value : record.Date.Value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
        cmd.Parameters.AddWithValue("$descriptor", (object?)record.Descriptor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$amount_owed", record.AmountOwed is null ? DBNull.Value : record.AmountOwed.Value);
        cmd.Parameters.AddWithValue("$amount_paid", record.AmountPaid is null ? DBNull.Value : record.AmountPaid.Value);
        cmd.Parameters.AddWithValue("$currency", (object?)record.Currency ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$pay_method", (object?)record.PayMethod ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$last_four", (object?)record.LastFour ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$confirmation", (object?)record.Confirmation ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$account_credit", record.AccountCredit is null ? DBNull.Value : record.AccountCredit.Value);
        cmd.Parameters.AddWithValue("$phone", (object?)record.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$business", (object?)record.Business ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$paid", record.Paid is null ? DBNull.Value : (record.Paid.Value ? 1 : 0));
    }
}

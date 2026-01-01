using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;

namespace BudgetLedger.Data;

internal sealed class MonthSummaryRepository
{
    private readonly LedgerDb _db;

    public MonthSummaryRepository(LedgerDb db) => _db = db;

    public void Upsert(int year, int month, decimal startTotal, decimal endTotal)
    {
        //Debug: Can be deleted or commented out in production
        Debug.WriteLine($"Upsert called for month {month}/{year}: StartTotal={startTotal}, EndTotal={endTotal}");

        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
INSERT OR REPLACE INTO month_summary (year, month, monthly_start_total, monthly_end_total)
VALUES ($year, $month, $startTotal, $endTotal);
";
        cmd.Parameters.AddWithValue("$year", year);
        cmd.Parameters.AddWithValue("$month", month);
        cmd.Parameters.AddWithValue("$startTotal", startTotal);
        cmd.Parameters.AddWithValue("$endTotal", endTotal);
        cmd.ExecuteNonQuery();

        //Debug: Can be deleted or commented out in production
        Debug.WriteLine("Upsert completed.");
    }

    public (decimal StartTotal, decimal EndTotal)? GetByMonth(int year, int month)
    {
        //Debug: Can be deleted or commented out in production
        Debug.WriteLine($"GetByMonth called for month {month}/{year}");

        using var connection = new SqliteConnection(_db.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
    SELECT monthly_start_total, monthly_end_total
    FROM month_summary
    WHERE year = $year AND month = $month;
    ";
        cmd.Parameters.AddWithValue("$year", year);
        cmd.Parameters.AddWithValue("$month", month);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            decimal startTotal = reader.GetDecimal(0);
            decimal endTotal = reader.GetDecimal(1);

            //Debug: Can be deleted or commented out in production
            Debug.WriteLine($"Retrieved summary for month {month}/{year}: StartTotal={startTotal}, EndTotal={endTotal}");

            return (startTotal, endTotal);
        }

        //Debug: Can be deleted or commented out in production
        Debug.WriteLine($"No summary found for month {month}/{year}");
        return null;
    }
}

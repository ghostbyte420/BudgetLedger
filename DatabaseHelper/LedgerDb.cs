using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace BudgetLedger.Data;

internal sealed class LedgerDb
{
    private readonly string _dbPath;

    public LedgerDb(string dbPath)
    {
        if (string.IsNullOrWhiteSpace(dbPath)) throw new ArgumentException("DB path is required", nameof(dbPath));
        _dbPath = dbPath;
    }

    public string ConnectionString => new SqliteConnectionStringBuilder
    {
        DataSource = _dbPath,
        Mode = SqliteOpenMode.ReadWriteCreate,
        Cache = SqliteCacheMode.Shared
    }.ToString();

    public void Initialize()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_dbPath) ?? AppContext.BaseDirectory);

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var pragma = connection.CreateCommand();
        pragma.CommandText = "PRAGMA journal_mode=WAL; PRAGMA synchronous=NORMAL;";
        pragma.ExecuteNonQuery();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS meta (
    key TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS expense (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    year INTEGER NOT NULL,
    month INTEGER NOT NULL,
    category TEXT NOT NULL,

    expense TEXT NULL,
    date TEXT NULL,
    descriptor TEXT NULL,
    amount_owed REAL NULL,
    amount_paid REAL NULL,
    currency TEXT NULL,
    pay_method TEXT NULL,
    last_four TEXT NULL,
    confirmation TEXT NULL,
    account_credit REAL NULL,
    phone TEXT NULL,
    business TEXT NULL,
    paid INTEGER NULL,

    created_utc TEXT NOT NULL,
    updated_utc TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS idx_expense_ymc ON expense(year, month, category);

CREATE TABLE IF NOT EXISTS month_summary (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    year INTEGER NOT NULL,
    month INTEGER NOT NULL,
    monthly_start_total REAL NOT NULL,
    monthly_end_total REAL NOT NULL,
    UNIQUE(year, month)
);
";
        cmd.ExecuteNonQuery();
    }
}

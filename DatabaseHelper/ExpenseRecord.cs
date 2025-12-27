using System;

namespace BudgetLedger.Data;

internal sealed record ExpenseRecord(
    long Id,
    int Year,
    int Month,
    string Category,
    string? Expense,
    DateTime? Date,
    string? Descriptor,
    decimal? AmountOwed,
    decimal? AmountPaid,
    string? Currency,
    string? PayMethod,
    string? LastFour,
    string? Confirmation,
    decimal? AccountCredit,
    string? Phone,
    string? Business,
    bool? Paid
);

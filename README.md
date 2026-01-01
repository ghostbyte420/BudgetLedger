# 💰 BudgetLedger: A Private Expense Tracker

**A powerful, offline-first budgeting tool for households, freelancers, and crypto users**

<img width="385" height="177" alt="image" src="https://github.com/user-attachments/assets/bb9e8bfd-617a-4d16-bf9e-f65dfc00933c" />      <img width="385" height="177" alt="image" src="https://github.com/user-attachments/assets/bdcdec19-facc-493d-9acc-d0a4ac201943" />

<img width="1688" height="836" alt="image" src="https://github.com/user-attachments/assets/cc3395c7-9c1e-43b8-9899-7d8647eaa7d5" />
<img width="1688" height="836" alt="image" src="https://github.com/user-attachments/assets/c697f86e-6da1-40d2-93d4-38e411fa6984" />

<img width="1688" height="836" alt="image" src="https://github.com/user-attachments/assets/012353cc-bd6d-46bd-8762-65cfa97d91b5" />
<img width="1688" height="836" alt="image" src="https://github.com/user-attachments/assets/26615b9e-750a-4de8-9e8b-833dc38c8b72" />
<img width="322" height="534" alt="image" src="https://github.com/user-attachments/assets/27abbe33-ad00-41e7-9c29-08fa7af3f368" />

# BudgetLedger
*A secure, digital ledger for tracking monthly expenses with Windows Forms and SQLite.*

---

## 📌 Overview
BudgetLedger replaces paper ledgers with a **password-protected**, **offline-first** Windows Forms application. It organizes expenses by month and category, calculates totals automatically, and exports reports as PDFs—**no CSV/Excel required**.

### ✨ Key Features
- **12-Month Structure**: Each month has tabs for categories (Household, Taxes, Vehicles, etc.).
- **Expense Tracking**: Log details like date, amount, payment method, and recipient.
- **Automatic Summaries**: Displays **Monthly Start Total**, **Expenses Paid Out**, and **Monthly End Total** (color-coded for positive/negative balances).
- **Password Protection**: AES-encrypted password prompt on launch.
- **PDF Export**: Generate detailed monthly reports with [QuestPDF](https://www.questpdf.com/).
- **Dark Theme UI**: Reduces eye strain with customizable label styles (bold/italic).
- **Local SQLite Database**: No cloud sync or external dependencies.

---

### Prerequisites
- **.NET 10 SDK** (https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- **Visual Studio 2026 Community Edition** (https://visualstudio.microsoft.com/vs/community/)
- **Windows OS** (Windows Forms compatibility)

---

## 🔧 Configuration
Customize BudgetLedger via the **menu strip** (top-right corner):
- **Password Reset**: Delete `PasswordConfig.txt` to reset the password on next launch
- **Database Location**: The SQLite database (`budgetledger_YYYY.sqlite`) is saved in the application’s root directory by default for portability

---

## 🚨 Security Notes
- **Password Storage**: Passwords are encrypted using **AES-256** and stored locally in `PasswordConfig.txt`.
  > ⚠️ **Important**: The encryption key is hardcoded in `PasswordEncryption.cs`.
  > **For production use**, replace `Key` and `IV` with secure, environment-specific values (e.g., Windows DPAPI or Azure Key Vault).
- **Database Backup**: Manually copy `budgetledger_YYYY.sqlite` to back up your data.
- **First Run**: If no password file exists, the app will prompt you to create one.

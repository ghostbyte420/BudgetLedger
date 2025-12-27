using System;
using System.Drawing;
using System.Windows.Forms;
using BudgetLedger.Controls;
using BudgetLedger.Data;
namespace BudgetLedger
{
    public partial class BaseMonth : UserControl
    {
        protected OpacityPanel monthOpacityPanel;
        protected DarkTabControl monthTabControl;

        public BaseMonth()
        {
            InitializeComponent();
            InitializeTabs();
        }

        protected virtual void InitializeComponent()
        {
            monthOpacityPanel = new OpacityPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Name = "month_opacityPanel"
            };

            monthTabControl = new DarkTabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F),
                Name = "month_opacityPanel_darkTabControl"
            };

            // Enable double buffering for the DarkTabControl
            monthTabControl.EnableDoubleBuffering();

            monthOpacityPanel.Controls.Add(monthTabControl);
            this.Controls.Add(monthOpacityPanel);
        }

        protected virtual void InitializeTabs()
        {
            // Tab names and order
            string[] tabNames =
            {
                "Header", "Household", "Subscriptions", "State Taxes", "Vehicles", "Gas", "Trips", "Food", "Frivelous", "Emergency"
            };

            foreach (string tabName in tabNames)
            {
                TabPage tabPage = new TabPage(tabName)
                {
                    BackColor = Color.Transparent,
                    Name = $"month_opacityPanel_darkTabControl_tabPage_{tabName.Replace(" ", "")}",
                    Padding = new Padding(3)
                };
                monthTabControl.TabPages.Add(tabPage);

                // Skip adding DataGridView for the "Header" tab
                if (tabName != "Header")
                {
                    // Add a DataGridView to each tab for expenses
                    DataGridView expenseGrid = CreateExpenseGrid();
                    tabPage.Controls.Add(expenseGrid);

                    var month = GetMonthNumber();
                    if (month is not null)
                    {
                        DataGridLedgerBinder.Bind(expenseGrid, month.Value, tabName);
                    }
                }
            }
        }

        protected DataGridView CreateExpenseGrid()
        {
            DataGridView grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.WhiteSmoke,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AllowUserToResizeColumns = false,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(240, 240, 240),
                    Font = new Font("Segoe UI", 9F)
                },
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold)
                },
                DefaultCellStyle =
                {
                    Font = new Font("Segoe UI", 9F)
                }
            };

            // Add a DateTimePicker control to the form (hidden by default)
            DateTimePicker datePicker = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Visible = false
            };
            grid.Controls.Add(datePicker);

            // Handle cell clicks to show the DateTimePicker
            grid.CellClick += (sender, e) =>
            {
                if (e.ColumnIndex == grid.Columns["Date"].Index && e.RowIndex >= 0)
                {
                    // Position the DateTimePicker over the clicked cell
                    Rectangle cellRect = grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    datePicker.Size = new Size(cellRect.Width, cellRect.Height);
                    datePicker.Location = new Point(cellRect.X, cellRect.Y);

                    // Set the datePicker value safely
                    if (grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null &&
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != DBNull.Value)
                    {
                        if (DateTime.TryParse(grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out DateTime cellDate))
                        {
                            datePicker.Value = cellDate;
                        }
                        else
                        {
                            datePicker.Value = DateTime.Now;
                        }
                    }
                    else
                    {
                        datePicker.Value = DateTime.Now;
                    }

                    datePicker.Visible = true;
                }
                else
                {
                    datePicker.Visible = false;
                }
            };

            // Handle date selection
            datePicker.CloseUp += (sender, e) =>
            {
                if (datePicker.Visible)
                {
                    grid.CurrentCell.Value = datePicker.Value.ToShortDateString();
                    datePicker.Visible = false;
                }
            };

            // Handle leaving the cell
            grid.CellLeave += (sender, e) =>
            {
                datePicker.Visible = false;
            };

            // Handle cell formatting to ensure dates are displayed properly
            grid.CellFormatting += (sender, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name == "Date" && e.Value != null)
                {
                    if (DateTime.TryParse(e.Value.ToString(), out DateTime dateValue))
                    {
                        e.Value = dateValue.ToShortDateString();
                        e.FormattingApplied = true;
                    }
                }
            };

            // Purchase Name (Left-aligned)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Expense",
                Name = "What Did You Spend Money On?",
                Width = 175,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Date (Left-aligned, with date picker)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date",
                Name = "Date",
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Descriptor (Left-aligned)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Descriptor",
                Name = "Why You Spent Money On This Expense?",
                Width = 175,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Amount Owed (Right-aligned cells, centered header, no sorting)
            DataGridViewTextBoxColumn amountOwedColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Amount Owed",
                Name = "How Much Was Owed For This Expense?",
                Width = 130,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F), Format = "N2" }
            };
            amountOwedColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            amountOwedColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            grid.Columns.Add(amountOwedColumn);

            // Amount Paid (Right-aligned cells, centered header, no sorting)
            DataGridViewTextBoxColumn amountPaidColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Amount Paid",
                Name = "How Much Did You Pay Toward This Expense?",
                Width = 130,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F), Format = "N2" }
            };
            amountPaidColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            amountPaidColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            grid.Columns.Add(amountPaidColumn);

            // Currency (Center-aligned header and cells, no sorting)
            DataGridViewTextBoxColumn currencyColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Currency",
                Name = "What Currency Type Was Used To Pay For This Expense?",
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F) }
            };
            currencyColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            currencyColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            grid.Columns.Add(currencyColumn);

            // Payment Method (Left-aligned)
            grid.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Pay Method",
                Name = "What Payment Method Did You Use For This Expense?",
                Items = { "Credit", "Cash", "Crypto", "Voucher" },
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Last 4 Digits (Center-aligned header and cells)
            DataGridViewTextBoxColumn lastFourDigitsColumn = new DataGridViewTextBoxColumn
            {
                HeaderText = "Last Four",
                Name = "If Credit, What Were The Last Four Digits Of The Card Number Used For This Expense?",
                Width = 100,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F) }
            };
            lastFourDigitsColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns.Add(lastFourDigitsColumn);

            // Confirmation # (Left-aligned)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Confirmation #",
                Name = "What Was The Confirmation Number Given To You After Payment For This Expense Was Made?",
                Width = 150,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Account Credit (Right-aligned cells, left-aligned header by default)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Account Credit?",
                Name = "Was There An Account Credit?",
                Width = 150,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight, Font = new Font("Segoe UI", 9F), Format = "N2" }
            });

            // Phone # (Left-aligned)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Phone #",
                Name = "Does The Company Have A Contact Phone Number?",
                Width = 120,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Business Name (Left-aligned)
            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Business Name",
                Name = "Whom Did You Pay For Services Rendered?",
                Width = 150,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F) }
            });

            // Paid (Center-aligned header and cells)
            DataGridViewCheckBoxColumn paidColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = "Paid",
                Name = "Was This Expense Paid For The Month?",
                Width = 75,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9F) }
            };
            paidColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Columns.Add(paidColumn);

            return grid;
        }

        private int? GetMonthNumber()
        {
            var name = GetType().Name;
            return name switch
            {
                "January" => 1,
                "February" => 2,
                "March" => 3,
                "April" => 4,
                "May" => 5,
                "June" => 6,
                "July" => 7,
                "August" => 8,
                "September" => 9,
                "October" => 10,
                "November" => 11,
                "December" => 12,
                _ => null
            };
        }
    }
}
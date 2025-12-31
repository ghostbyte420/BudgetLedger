using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace BudgetLedger.Password
{
    public partial class passwordPrompt : Form
    {
        private bool isFirstRun = false;
        private int attemptsLeft = 3;

        public passwordPrompt(bool firstRun)
        {
            InitializeComponent();
            isFirstRun = firstRun;
            if (isFirstRun)
            {
                passwordPrompt_opacityPanel_darkTextBox_txtPassword_Label_enterPassword.Text = "Create a New Password:";
                this.Text = "Create Password";
            }
        }

        private void passwordPrompt_opacityPanel_darkTextBox_txtPassword_button_submit_Click(object sender, EventArgs e)
        {
            string passwordFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PasswordConfig.txt");

            if (isFirstRun)
            {
                // Encrypt and save the new password
                string encryptedPassword = PasswordEncryption.Encrypt(passwordPrompt_opacityPanel_darkTextBox_txtPassword.Text);
                File.WriteAllText(passwordFilePath, encryptedPassword);
                MessageBox.Show("Password created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Check the password
                if (File.Exists(passwordFilePath))
                {
                    string encryptedPassword = File.ReadAllText(passwordFilePath).Trim();
                    string enteredPassword = passwordPrompt_opacityPanel_darkTextBox_txtPassword.Text;
                    string decryptedPassword = PasswordEncryption.Decrypt(encryptedPassword);

                    if (enteredPassword == decryptedPassword)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        attemptsLeft--;
                        passwordPrompt_opacityPanel_darkTextBox_txtPassword_status.Text = $"Incorrect password. {attemptsLeft} attempts left.";
                        passwordPrompt_opacityPanel_darkTextBox_txtPassword_status.ForeColor = Color.Red;
                        passwordPrompt_opacityPanel_darkTextBox_txtPassword.Clear();

                        if (attemptsLeft <= 0)
                        {
                            MessageBox.Show("Too many failed attempts. Application will close.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.DialogResult = DialogResult.Cancel;
                            this.Close();
                            Environment.Exit(0);
                        }
                    }
                }
                else
                {
                    // If no password file exists, treat it as a first run
                    MessageBox.Show("No password found. Please restart the application to create a new password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    Environment.Exit(0);
                }
            }
        }
    }
}

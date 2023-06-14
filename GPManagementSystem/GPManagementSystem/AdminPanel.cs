using GPManagementSystem.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPManagementSystem
{
    public partial class AdminPanel : Form
    {
        private DatabaseConnection dbConnection;

        public AdminPanel()
        {
            InitializeComponent();
            dbConnection = new DatabaseConnection();
        }

        private void AdminPanel_Load(object sender, EventArgs e)
        {
            UpdateList("");
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            UpdateList(textBox5.Text);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBox1.SelectedItem is Account selectedAccount))
                return;

            int accountId = selectedAccount.GetID();
            AccountDetails accountDetails = GetAccountDetails(accountId);

            if (accountDetails != null)
            {
                textBox6.Text = accountId.ToString();
                textBox7.Text = accountDetails.Username;
                textBox8.Text = accountDetails.AccountName;
                textBox9.Text = accountDetails.DateOfBirth;
                textBox10.Text = accountDetails.PhoneNumber;

                textBox11.Text = accountDetails.AccountType == 0 ? "Secretary" : "Doctor";

                textBox12.Text = accountDetails.Notes;
                textBox13.Text = accountDetails.CreationDate;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Please check the input fields again!");
                return;
            }

            string username = textBox1.Text;
            string password = textBox2.Text;
            string accountName = textBox3.Text;
            int accountType = comboBox1.SelectedIndex;
            string notes = textBox4.Text;
            DateTime creationDate = DateTime.Now;

            bool userCreated = CreateUser(username, password, out int userId);
            bool accountCreated = false;

            if (userCreated)
                accountCreated = CreateAccount(userId, accountName, accountType, notes, creationDate);

            if (accountCreated)
            {
                MessageBox.Show("Account was successfully created!");
            }
            else
            {
                MessageBox.Show("Error while creating the account!");
                if (userCreated)
                    DeleteUser(userId);
            }

            UpdateList("");
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                return false;

            if (comboBox1.SelectedIndex < 0)
                return false;

            return true;
        }

        private bool CreateUser(string username, string password, out int userId)
        {
            string query = "INSERT INTO [user] (user_username, user_password) VALUES (@username, @password); SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                dbConnection.GetConnection().Open();
                object result = command.ExecuteScalar();
                dbConnection.GetConnection().Close();

                if (result != null && int.TryParse(result.ToString(), out userId))
                {
                    return true;
                }
            }

            userId = -1;
            return false;
        }

        private bool CreateAccount(int userId, string accountName, int accountType, string notes, DateTime creationDate)
        {
            string query = "INSERT INTO account (account_user_id, account_name, account_type, account_notes, account_creation_date) VALUES (@userId, @accountName, @accountType, @notes, @creationDate); SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@accountName", accountName);
                command.Parameters.AddWithValue("@accountType", accountType);
                command.Parameters.AddWithValue("@notes", notes);
                command.Parameters.AddWithValue("@creationDate", creationDate);

                dbConnection.GetConnection().Open();
                object result = command.ExecuteScalar();
                dbConnection.GetConnection().Close();

                if (result != null && int.TryParse(result.ToString(), out int accountId))
                {
                    return true;
                }
            }

            return false;
        }

        private void DeleteUser(int userId)
        {
            string query = "DELETE FROM [user] WHERE user_id = @userId";
            dbConnection.AddParameter("@userId", userId);
            dbConnection.ExecuteNonQuery(query);
        }

        private void UpdateList(string query)
        {
            string selectQuery = "SELECT account_id, account_name, account_type FROM account WHERE account_type IN (0, 1) AND (account_name LIKE @query OR account_phone LIKE @query) ORDER BY account_type";

            using (SqlCommand command = new SqlCommand(selectQuery, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@query", query + "%");

                dbConnection.GetConnection().Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    listBox1.Items.Clear();
                    while (reader.Read())
                    {
                        int accountId = reader.GetInt32(0);
                        string accountName = reader.GetString(1);
                        int accountType = reader.GetInt32(2);

                        listBox1.Items.Add(new Account(accountId, accountName, accountType));
                    }
                }
                dbConnection.GetConnection().Close();
            }
        }

        private AccountDetails GetAccountDetails(int accountId)
        {
            string query = "SELECT user_username, account_name, account_dob, account_phone, account_type, account_notes, account_creation_date FROM [user], account WHERE user_id = account_user_id AND account_id = @accountId";

            using (SqlCommand command = new SqlCommand(query, dbConnection.GetConnection()))
            {
                command.Parameters.AddWithValue("@accountId", accountId);

                dbConnection.GetConnection().Open();
                SqlDataReader reader = command.ExecuteReader();

                AccountDetails accountDetails = null;
                if (reader.Read())
                {
                    string username = reader.GetString(0);
                    string accountName = reader.GetString(1);
                    string dateOfBirth = reader.IsDBNull(2) ? string.Empty : reader.GetDateTime(2).ToString();
                    string phoneNumber = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    int accountType = reader.GetInt32(4);
                    string notes = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    string creationDate = reader.GetDateTime(6).ToString();

                    accountDetails = new AccountDetails(accountId, username, accountName, dateOfBirth, phoneNumber, accountType, notes, creationDate);
                }

                reader.Close();
                dbConnection.GetConnection().Close();

                return accountDetails;
            }
        }
    }
}
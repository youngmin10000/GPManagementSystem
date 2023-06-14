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
    public partial class EditProfile : Form
    {
        private int account_id;
        private SqlConnection con;

        public EditProfile(int account_id)
        {
            InitializeComponent();
            this.account_id = account_id;
            con = new SqlConnection(Properties.Resources.connectionString);
        }

        private void EditProfile_Load(object sender, EventArgs e)
        {
            LoadAccountDetails();
        }

        private void LoadAccountDetails()
        {
            SqlCommand command = new SqlCommand("SELECT user_username, account_name, account_dob, account_phone, account_type, account_notes, account_creation_date FROM [user] INNER JOIN account ON account_user_id = user_id WHERE account_id = @account_id", con);
            command.Parameters.AddWithValue("@account_id", account_id);

            con.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                textBox1.Text = account_id.ToString();
                textBox2.Text = reader.GetString(0);
                textBox3.Text = reader.GetString(1);

                if (!reader.IsDBNull(2))
                {
                    DateTime dob = reader.GetDateTime(2);
                    dateTimePicker1.Value = dob;
                }

                textBox4.Text = reader.GetString(3);
                int accountType = reader.GetInt32(4);
                textBox5.Text = GetAccountTypeText(accountType);
                textBox6.Text = reader.GetString(5);
                textBox7.Text = reader.GetDateTime(6).ToString();
            }

            con.Close();
        }

        private string GetAccountTypeText(int accountType)
        {
            switch (accountType)
            {
                case 0:
                    return "Secretary";
                case 1:
                    return "Doctor";
                case 2:
                    return "Patient";
                default:
                    return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                MessageBox.Show("Please enter a name!");
                return;
            }

            string updateQuery = "UPDATE account SET account_name = @name, account_dob = @dob, account_notes = @notes, account_phone = @phone WHERE account_id = @account_id";

            using (SqlCommand command = new SqlCommand(updateQuery, con))
            {
                command.Parameters.AddWithValue("@name", textBox3.Text);
                command.Parameters.AddWithValue("@dob", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@phone", textBox4.Text);
                command.Parameters.AddWithValue("@notes", textBox6.Text);
                command.Parameters.AddWithValue("@account_id", account_id);

                con.Open();
                int rowsAffected = command.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Account was updated!");
                }
                else
                {
                    MessageBox.Show("Account was not updated!");
                }
            }
        }
    }
}



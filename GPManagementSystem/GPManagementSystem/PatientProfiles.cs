using GPManagementSystem.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GPManagementSystem
{
    public partial class PatientProfiles : Form
    {
        private SqlConnection con;
        private SqlCommand command;

        public PatientProfiles()
        {
            InitializeComponent();
            con = new SqlConnection(Properties.Resources.connectionString);
        }

        private void PatientProfiles_Load(object sender, EventArgs e)
        {
            UpdateList("");
        }

        private void UpdateList(string query)
        {
            command = new SqlCommand("SELECT account_id, account_name, account_type FROM account WHERE account_type = 2 AND (account_name LIKE @query OR account_phone LIKE @query)", con);
            command.Parameters.AddWithValue("@query", query + "%");

            con.Open();
            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();

            while (reader.Read())
            {
                listBox1.Items.Add(new Account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }

            con.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            UpdateList(textBox4.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Inputs Validation
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Please check the inputs!");
                return;
            }

            // Account Creation
            command = new SqlCommand("INSERT INTO account (account_name, account_phone, account_notes, account_type, account_creation_date) VALUES (@name, @phone, @notes, 2, @date)", con);
            command.Parameters.AddWithValue("@name", textBox1.Text);
            command.Parameters.AddWithValue("@phone", textBox2.Text);
            command.Parameters.AddWithValue("@notes", textBox3.Text);
            command.Parameters.AddWithValue("@date", DateTime.Now);

            con.Open();

            int rowsAffected = command.ExecuteNonQuery();
            con.Close();

            if (rowsAffected > 0)
            {
                MessageBox.Show("Account was created!");
                UpdateList("");
            }
            else
            {
                MessageBox.Show("Failed to create the account!");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
                return;

            int accountId = ((Account)listBox1.SelectedItem).GetID();

            command = new SqlCommand("SELECT account_name, account_dob, account_phone, account_notes, account_creation_date FROM account WHERE account_id = @id", con);
            command.Parameters.AddWithValue("@id", accountId);

            con.Open();
            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                textBox5.Text = accountId.ToString();
                textBox6.Text = reader.GetString(0);

                DateTime dob;
                if (DateTime.TryParse(reader.GetValue(1).ToString(), out dob))
                    dateTimePicker1.Value = dob;

                textBox7.Text = reader.GetString(2);
                textBox8.Text = reader.GetString(3);
                textBox9.Text = reader.GetValue(4).ToString();
            }

            con.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Inputs Validation
            if (string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(textBox7.Text))
            {
                MessageBox.Show("Please check the inputs!");
                return;
            }

            // Editing the account
            command = new SqlCommand("UPDATE account SET account_name = @name, account_phone = @phone, account_dob = @dob, account_notes = @notes WHERE account_id = @id", con);
            command.Parameters.AddWithValue("@name", textBox6.Text);
            command.Parameters.AddWithValue("@phone", textBox7.Text);
            command.Parameters.AddWithValue("@dob", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@notes", textBox8.Text);
            command.Parameters.AddWithValue("@id", textBox5.Text);

            con.Open();

            int rowsAffected = command.ExecuteNonQuery();
            con.Close();

            if (rowsAffected > 0)
            {
                MessageBox.Show("Account was updated!");
            }
            else
            {
                MessageBox.Show("Failed to update the account!");
            }
        }
    }
}

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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GPManagementSystem
{
    public partial class ViewReservation : Form
    {
        int account_id, account_type;
        public ViewReservation(int id)
        {
            InitializeComponent();
            account_id = id;
        }

        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
        SqlCommand command;

        private void UpdateList()
        {
            string query = "SELECT reservation_id, reservation_patient_id, patient.account_name, reservation_secretary_id, secretary.account_name, reservation_visit_date, reservation_visit_slot, reservation_date FROM reservation, account as patient, account as secretary WHERE reservation_patient_id = patient.account_id AND reservation_secretary_id = secretary.account_id";

            if (radioButton1.Checked)
            {
                command = new SqlCommand(query + " AND DATEPART(YEAR, reservation_visit_date) = @year AND DATEPART(MONTH, reservation_visit_date) = @month AND DATEPART(DAY, reservation_visit_date) = @day", con);
                command.Parameters.AddWithValue("@year", dateTimePicker1.Value.Year);
                command.Parameters.AddWithValue("@month", dateTimePicker1.Value.Month);
                command.Parameters.AddWithValue("@day", dateTimePicker1.Value.Day);
            }
            else if (radioButton2.Checked)
            {
                command = new SqlCommand(query + " AND (patient.account_name LIKE @query OR patient.account_phone LIKE @query OR reservation_id LIKE @query)", con);
                command.Parameters.AddWithValue("@query", textBox1.Text + "%");
            }
            else if (radioButton3.Checked)
            {
                command = new SqlCommand(query + " AND (patient.account_name LIKE @query OR patient.account_phone LIKE @query OR reservation_id LIKE @query) AND DATEPART(YEAR, reservation_visit_date) = @year AND DATEPART(MONTH, reservation_visit_date) = @month AND DATEPART(DAY, reservation_visit_date) = @day", con);
                command.Parameters.AddWithValue("@query", textBox1.Text + "%");
                command.Parameters.AddWithValue("@year", dateTimePicker1.Value.Year);
                command.Parameters.AddWithValue("@month", dateTimePicker1.Value.Month);
                command.Parameters.AddWithValue("@day", dateTimePicker1.Value.Day);
            }
            else
            {
                // No radio button is checked, return or show an error message
                MessageBox.Show("Please select a search option!");
                return;
            }

            con.Open();

            SqlDataReader reader = command.ExecuteReader();
            listBox1.Items.Clear();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int patient_id = reader.GetInt32(1);
                string patient_name = reader.GetString(2);
                int secretary_id = reader.GetInt32(3);
                string secretary_name = reader.GetString(4);
                DateTime visit_date = new DateTime();
                DateTime.TryParse(reader.GetValue(5).ToString(), out visit_date);
                int slot = reader.GetInt32(6);

                DateTime date = new DateTime();
                DateTime.TryParse(reader.GetValue(7).ToString(), out date);

                listBox1.Items.Add(new Reservation(id, patient_id, patient_name, secretary_id, secretary_name, slot, visit_date, date));
            }

            con.Close();
        }


        private void ViewReservation_Load(object sender, EventArgs e)
        {
            UpdateList();
            command = con.CreateCommand();
            command.CommandText = "SELECT account_type FROM account WHERE account_id = @id";
            command.Parameters.AddWithValue("@id", account_id);
            con.Open();
            account_type = (int)command.ExecuteScalar();
            con.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            Reservation res = (Reservation)listBox1.SelectedItem;

            Hide();
            EditReservation editReservation = new EditReservation(res);
            editReservation.ShowDialog();
            UpdateList();
            Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            Reservation res = (Reservation)listBox1.SelectedItem;
            Hide();
            Visits visits = new Visits(account_id, res.patient.Key, res.id);
            visits.ShowDialog();
            Show();
        }

        private void updateForm()
        {
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a reservation!");
                return;
            }

            Reservation res = (Reservation)listBox1.SelectedItem;
            textBox2.Text = res.id.ToString();
            textBox3.Text = res.patient.ToString();
            textBox4.Text = res.secretary.ToString();
            textBox5.Text = res.visit_date.Date.ToString();
            textBox6.Text = Utils.getSlots()[res.slot];
            textBox7.Text = res.date.ToString();

            if (account_type == 0 && res.visit_date >= DateTime.Today)
                button1.Enabled = true;
            else
                button1.Enabled = false;

            if (account_type == 1)
                button2.Enabled = true;
            else
                button2.Enabled = false;
        }
    }
}

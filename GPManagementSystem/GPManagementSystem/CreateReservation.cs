﻿using GPManagementSystem.Class;
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
    public partial class CreateReservation : Form
    {
        int secretary_id;
        SqlConnection con = new SqlConnection(Properties.Resources.connectionString);
        SqlCommand command;

        public CreateReservation(int id)
        {
            InitializeComponent();
            secretary_id = id;
        }

        private void CreateReservation_Load(object sender, EventArgs e)
        {
            LoadReservationData();
        }

        private void LoadReservationData()
        {
            UpdateAccountList("");
            UpdateSlots();
            dateTimePicker1.MinDate = DateTime.Today;
        }

        private void UpdateAccountList(string query)
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT account_id, account_name, account_type FROM account WHERE account_type = 2 AND (account_name LIKE @query OR account_phone LIKE @query)";
            command.Parameters.AddWithValue("@query", query + "%");

            con.Open();
            listBox1.Items.Clear();

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                listBox1.Items.Add(new Account(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
            }

            con.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateAccountList(textBox1.Text);
        }

        private void UpdateSlots()
        {
            command = con.CreateCommand();
            command.CommandText = "SELECT reservation_visit_slot FROM reservation WHERE reservation_visit_date = @date";
            command.Parameters.AddWithValue("@date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));

            con.Open();
            SqlDataReader reader = command.ExecuteReader();

            Dictionary<int, string> slots = Utils.getSlots();

            while (reader.Read())
            {
                slots.Remove(reader.GetInt32(0));
            }

            comboBox1.Items.Clear();
            foreach (object slot in slots.ToArray())
            {
                comboBox1.Items.Add(slot);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            con.Close();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            UpdateSlots();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Inputs Validation
            if (listBox1.SelectedIndex < 0 || listBox1.SelectedIndex >= listBox1.Items.Count)
            {
                MessageBox.Show("Please select a patient!");
                return;
            }
            if (comboBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a slot!");
                return;
            }

            // Perform the reservation
            int patient_id = ((Account)listBox1.SelectedItem).GetID();
            int slot = ((KeyValuePair<int, string>)comboBox1.SelectedItem).Key;

            command = con.CreateCommand();
            command.CommandText = "INSERT INTO reservation (reservation_secretary_id, reservation_patient_id, reservation_visit_date, reservation_visit_slot, reservation_date) VALUES (@secretary_id, @patient_id, @visit_date, @visit_slot, @date)";
            command.Parameters.AddWithValue("@secretary_id", secretary_id);
            command.Parameters.AddWithValue("@patient_id", patient_id);
            command.Parameters.AddWithValue("@visit_date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
            command.Parameters.AddWithValue("@visit_slot", slot);
            command.Parameters.AddWithValue("@date", DateTime.Now);

            con.Open();

            if (command.ExecuteNonQuery() > 0)
            {
                // Successfully performed the reservation
                command.CommandText = "SELECT reservation_id FROM reservation WHERE reservation_visit_date = @visit_date AND reservation_visit_slot = @visit_slot";
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@visit_date", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@visit_slot", slot);
                int reservation_id = Convert.ToInt32(command.ExecuteScalar());

                MessageBox.Show("Reservation was made!");
                MessageBox.Show("Reservation ID: " + reservation_id.ToString());
            }
            else
            {
                MessageBox.Show("Failed to perform the reservation!");
            }

            con.Close();
            UpdateSlots();
        }
    }
}
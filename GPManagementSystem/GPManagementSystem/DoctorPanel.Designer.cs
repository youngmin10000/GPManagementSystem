﻿namespace GPManagementSystem
{
    partial class DoctorPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(127, 136);
            button1.Name = "button1";
            button1.Size = new Size(120, 28);
            button1.TabIndex = 0;
            button1.Text = "Edit Account";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(127, 214);
            button2.Name = "button2";
            button2.Size = new Size(120, 28);
            button2.TabIndex = 1;
            button2.Text = "View Reservation";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // DoctorPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(367, 364);
            Controls.Add(button2);
            Controls.Add(button1);
            Name = "DoctorPanel";
            Text = "DcotorPanel";
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private Button button2;
    }
}
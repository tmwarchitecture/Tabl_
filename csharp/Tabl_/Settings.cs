using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tabl_cs
{
    public partial class Settings : Form
    {
        internal int dp = 2; // decimal place
        internal string ts =","; // thousands separater
        internal int cf = 0; // color format
        internal bool update = false; // true as automatic
        internal double su = 1.0; // scale units
        internal string cun = ""; // custom unit name

        private string[] tsvalues = new string[] { ",", ".", " ", "", };

        //constructor
        public Settings()
        {
            InitializeComponent();
            Icon = Properties.Resources.main;

            Shown += OnPopUp;
        }

        // cancel
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            // meant to be shown modal under main form
            // therefore dispose is NOT triggered
        }

        // handle pop up, refresh control values from fields
        private void OnPopUp(object s, EventArgs e)
        {
            numericUpDown1.Value = dp;

            if (ts == ",") comboBox1.SelectedIndex = 0;
            else if (ts == ".") comboBox1.SelectedIndex = 1;
            else if (ts == " ") comboBox1.SelectedIndex = 2;
            else comboBox1.SelectedIndex = 3;

            if (cf == 0) radioButton1.Checked = true;
            else if (cf == 1) radioButton2.Checked = true;
            else radioButton3.Checked = true;

            radioButton5.Checked = update;
            radioButton4.Checked = !update;

            numericUpDown2.Value = (decimal)su;

            textBox1.Text = cun;
        }

        // commit changes
        private void button1_Click(object sender, EventArgs e)
        {
            dp = (int)Math.Round(numericUpDown1.Value);
            ts = tsvalues[comboBox1.SelectedIndex];
            if (radioButton1.Checked) cf = 0;
            else if (radioButton2.Checked) cf = 1;
            else if (radioButton3.Checked) cf = 2;
            else cf = 0; //TODO: should never reach this; add debug message
            update = radioButton5.Checked;
            su = (double)numericUpDown2.Value;
            cun = textBox1.Text;

            Close();
        }
    }
}

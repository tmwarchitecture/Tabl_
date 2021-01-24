using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Commands;

namespace Tabl_cs
{
    public partial class PlaceSettings : Form
    {
        internal int fitting = 0;
        internal double cw = 40;
        internal decimal cellpad = 2;
        internal bool ok = false;
        internal string fn = "Arial";
        internal double fs = 10;
        private Plane bp;

        public PlaceSettings()
        {
            InitializeComponent();

            switch (fitting)
            {
                case 0:
                    radioButton1.Checked = true;
                    break;
                case 1:
                    radioButton2.Checked = true;
                    break;
                case 2:
                    radioButton3.Checked = true;
                    break;
                case 3:
                    radioButton4.Checked = true;
                    break;
                default:
                    break;
            }
            textBox1.Text = cw.ToString();
            textBox1.Enabled = false;
            numericUpDown1.Value = cellpad;
            label4.Text = fn;
            textBox2.Text = fs.ToString();
            radioButton5.Checked = true;
            
            bp = Plane.WorldXY;
        }

        internal Plane BasePlane
        {
            get { return bp; }
        }

        // check changed for column fitting raido bttns, all radio bttn check change call this one
        private void OnFittingTypeChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                fitting = 0;
            else if (radioButton2.Checked)
                fitting = 1;
            else if (radioButton3.Checked)
                fitting = 2;
            else if (radioButton4.Checked)
                fitting = 3;
            else
                fitting = 0;

            if (fitting == 0) textBox1.Enabled = false;
            else textBox1.Enabled = true;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton s = sender as RadioButton;
            if (s.Checked == false) return;
            OnFittingTypeChanged(sender, e);
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_CheckedChanged(sender, e);
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_CheckedChanged(sender, e);
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1_CheckedChanged(sender, e);
        }

        // cancel click
        private void button1_Click(object sender, EventArgs e)
        {
            ok = false;
            Close();
        }

        // ok click
        private void button2_Click(object sender, EventArgs e)
        {
            ok = true;
            Close();
        }

        // width
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox s = sender as TextBox;
            double temp = cw;
            if (!double.TryParse(s.Text, out cw))
            {
                // failed parse
                cw = temp;
                s.Text = temp.ToString();
                MessageBox.Show("NOT a valid number input!");
            }
        }

        // padding
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown s = sender as NumericUpDown;
            cellpad = s.Value;
        }

        // font pick
        private void label4_Click(object sender, EventArgs e)
        {
            Label s = sender as Label;
            if (fontDialog1.ShowDialog(this) == DialogResult.OK)
            {
                fn = fontDialog1.Font.Name;
                string display = fn.Length > 23 ? fn.Substring(0, 23) : fn;
                s.Text = display;
            }
        }

        // font size
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox s = sender as TextBox;
            double temp = fs;
            if (!double.TryParse(s.Text, out fs))
            {
                // failed parse
                fs = temp;
                s.Text = temp.ToString();
                MessageBox.Show("NOT a valid number input!");
            }
        }

        // change planes radio bttns event chain
        private void OnPlaneTypeChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked) bp = Plane.WorldXY;
            else if (radioButton6.Checked) bp = Plane.WorldYZ;
            else if (radioButton7.Checked) bp = Plane.WorldZX;
            else if (radioButton8.Checked) 
                if (RhinoGet.GetPlane(out bp) != Result.Success)
                {
                    radioButton5.Checked = true;
                    MessageBox.Show("plane selection failed\ndefaulted to world XY");
                }
        }
        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton s = sender as RadioButton;
            if (s.Checked == false) return;
            OnPlaneTypeChanged(sender, e);
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            radioButton8_CheckedChanged(sender, e);
        }
        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            radioButton8_CheckedChanged(sender, e);
        }
        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            radioButton8_CheckedChanged(sender, e);
        }
    }
}

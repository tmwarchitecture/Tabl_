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

namespace Tabl_
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
                    tbFitData.Checked = true;
                    break;
                case 1:
                    rbFitTablW.Checked = true;
                    break;
                case 2:
                    rbEvenTablW.Checked = true;
                    break;
                case 3:
                    rbFitCol.Checked = true;
                    break;
                default:
                    break;
            }
            textBox1.Text = cw.ToString();
            textBox1.Enabled = false;
            nudPad.Value = cellpad;
            labelFontBtn.Text = fn;
            tbFontSize.Text = fs.ToString();
            rbXY.Checked = true;
            
            bp = Plane.WorldXY;
        }

        internal Plane BasePlane
        {
            get { return bp; }
        }

        // check changed for column fitting raido bttns, all radio bttn check change call this one
        private void OnFittingTypeChanged(object sender, EventArgs e)
        {
            if (tbFitData.Checked)
                fitting = 0;
            else if (rbFitTablW.Checked)
                fitting = 1;
            else if (rbEvenTablW.Checked)
                fitting = 2;
            else if (rbFitCol.Checked)
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
            if (rbXY.Checked) bp = Plane.WorldXY;
            else if (rbYZ.Checked) bp = Plane.WorldYZ;
            else if (rbXZ.Checked) bp = Plane.WorldZX;
            else if (rbPickPlane.Checked) 
                if (RhinoGet.GetPlane(out bp) != Result.Success)
                {
                    rbXY.Checked = true;
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

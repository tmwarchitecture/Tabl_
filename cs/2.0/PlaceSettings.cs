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

        internal Plane BasePlane
        {
            get { return bp; }
        }
        internal RectPreview RectFollower;

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
                    rbFitCol.Checked = true;
                    break;
                default:
                    break;
            }
            tbPlaceColW.Text = cw.ToString();
            tbPlaceColW.Enabled = false;
            nudPad.Value = cellpad;
            labelFontBtn.Text = fn;
            tbFontSize.Text = fs.ToString();
            rbXY.Checked = true;
            
            bp = Plane.WorldXY;
            RectFollower = new RectPreview();
        }

        // check changed for column fitting raido bttns, all radio bttn check change call this one
        private void OnFittingTypeChanged(object sender, EventArgs e)
        {
            if (tbFitData.Checked)
                fitting = 0;
            else if (rbFitTablW.Checked)
                fitting = 1;
            else if (rbFitCol.Checked)
                fitting = 2;
            else
                fitting = 0;

            if (fitting == 0) tbPlaceColW.Enabled = false;
            else tbPlaceColW.Enabled = true;
        }
        private void ColWFit_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton s = sender as RadioButton;
            if (s.Checked == false) return;
            OnFittingTypeChanged(sender, e);
        }


        // cancel click
        private void PlaceCancel_Click(object sender, EventArgs e)
        {
            ok = false;
            Close();
        }

        // ok click
        private void PlaceOK_Click(object sender, EventArgs e)
        {
            ok = true;
            Close();
        }

        // width
        private void ColWInput_TextChanged(object sender, EventArgs e)
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
        private void Padding_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown s = sender as NumericUpDown;
            cellpad = s.Value;
        }

        // font pick
        private void LabelFont_Click(object sender, EventArgs e)
        {
            Label s = sender as Label;
            if (dlogFont.ShowDialog(this) == DialogResult.OK)
            {
                fn = dlogFont.Font.Name;
                string display = fn.Length > 23 ? fn.Substring(0, 23) : fn;
                s.Text = display;
            }
        }

        // font size
        private void FontSize_TextChanged(object sender, EventArgs e)
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
        private void Plane_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton s = sender as RadioButton;
            if (s.Checked == false) return;
            OnPlaneTypeChanged(sender, e);
        }
        

    }
}

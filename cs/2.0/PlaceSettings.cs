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
        internal int fitting = 0; // 0-fit data, 1-even divide, 2-fixed column
        internal double cw = 40; // column width
        internal int cellpad = 2;
        internal bool ok = false; // clicked ok or not upon form close
        internal bool custompl = false;
        internal Plane BasePl { get; set; }

        internal RectPreview RectFollower;

        public PlaceSettings()
        {
            InitializeComponent();
            RestoreParams();

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
            rbXY.Checked = true;

            BasePl = Plane.WorldXY;
            RectFollower = new RectPreview();
        }

        private void RestoreParams()
        {
            if (!TablPlugin.Instance.Settings.TryGetInteger("colfit", out fitting))
                fitting = 0;
            if (!TablPlugin.Instance.Settings.TryGetDouble("colwidth", out cw))
                cw = 40;
            if (!TablPlugin.Instance.Settings.TryGetInteger("cellpad", out cellpad))
                cellpad = 2;
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
            cellpad = (int)s.Value;
        }

        // change planes radio bttns event chain
        private void OnPlaneTypeChanged(object sender, EventArgs e)
        {
            custompl = false; // covers following 3 conditions unless 4th changes it
            if (rbXY.Checked)
                BasePl = Plane.WorldXY;
            else if (rbYZ.Checked)
                BasePl = Plane.WorldYZ;
            else if (rbXZ.Checked)
                BasePl = Plane.WorldZX;
            else
            {
                custompl = true;
                BasePl = Plane.Unset;
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tabl_
{
    public partial class Settings : Form
    {
        internal int dp = 2; // decimal place
        internal string ts =""; // thousands separater
        internal int cf = 0; // color format
        internal bool update = false; // true as automatic
        internal double su = 1.0; // scale units
        internal string cun = ""; // custom unit name
        internal bool[] ssopt; // spreadsheet options, 0-unit, 1-total, 2-export headers, 3-threaded
        // thousand separators
        private string[] tsvalues = new string[] { ",", ".", " ", "", };

        //constructor
        public Settings()
        {
            InitializeComponent();
            Icon = Properties.Resources.main;
            ssopt = new bool[chklTablDisplay.Items.Count];
            for (int i = 0; i < chklTablDisplay.Items.Count; i++)
                ssopt.SetValue(false, i);

            Shown += OnPopUp;
        }

        // cancel
        private void Cancel_Click(object sender, EventArgs e)
        {
            Close();
            // meant to be shown modal under main form
            // therefore dispose is NOT triggered
        }

        // handle pop up, refresh control values from fields
        private void OnPopUp(object s, EventArgs e)
        {
            nudDP.Value = dp;

            if (ts == ",") cbThousand.SelectedIndex = 0;
            else if (ts == ".") cbThousand.SelectedIndex = 1;
            else if (ts == " ") cbThousand.SelectedIndex = 2;
            else cbThousand.SelectedIndex = 3;

            if (cf == 0) rbClrName.Checked = true;
            else if (cf == 1) rbClrDash.Checked = true;
            else rbClrComma.Checked = true;

            rbAutoUpdate.Checked = update;
            rbManual.Checked = !update;

            nudUnitScale.Value = (decimal)su;

            tbUnitName.Text = cun;

            for (int i = 0; i < ssopt.Length; i++)
            {
                if (ssopt[i])
                    chklTablDisplay.SetItemChecked(i, true);
                else
                    chklTablDisplay.SetItemChecked(i, false);
            }
        }

        // commit changes
        private void OK_Click(object sender, EventArgs e)
        {
            dp = (int)Math.Round(nudDP.Value);
            ts = tsvalues[cbThousand.SelectedIndex];
            if (rbClrName.Checked) cf = 0;
            else if (rbClrDash.Checked) cf = 1;
            else if (rbClrComma.Checked) cf = 2;
            else MessageBox.Show("color format radio buttons error");
            update = rbAutoUpdate.Checked;
            su = (double)nudUnitScale.Value;
            cun = tbUnitName.Text.Trim();
            for (int i = 0; i < chklTablDisplay.Items.Count; i++)
            {
                if (chklTablDisplay.CheckedIndices.Contains(i))
                    ssopt[i] = true;
                else
                    ssopt[i] = false;
            }

            // must be shown modal and therefore not disposed after below call
            Close(); 
        }
    }
}

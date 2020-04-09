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
        //constructor
        public Settings()
        {
            InitializeComponent();
            Icon = Properties.Resources.main;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

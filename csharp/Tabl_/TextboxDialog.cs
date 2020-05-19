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
    public partial class TextboxDialog : Form
    {
        internal string txtval = "";
        internal bool commited = false;

        public TextboxDialog()
        {
            InitializeComponent();
            textBox1.KeyUp += OnTBEnter;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtval = textBox1.Text;
            commited = true;
            Close();
        }
        private void OnTBEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Enter)
            {
                txtval = textBox1.Text;
                commited = true;
                Close();
            }
        }
    }
}

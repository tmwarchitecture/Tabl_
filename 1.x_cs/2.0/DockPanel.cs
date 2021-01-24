using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino;

namespace Tabl_
{
    [Guid("FA271E46-C13B-47A5-9B69-46C578A74EA4")]
    public partial class DockPanel : UserControl
    {
        internal RhinoDoc ParentDoc { get; private set; } = RhinoDoc.ActiveDoc;
        private double tol;
        private bool showunit = false;
        private bool showtotal = false;
        private bool expheader = false;

        public static Guid PanelId
        {
            get { return typeof(DockPanel).GUID; }
        }

        public DockPanel()
        {
            InitializeComponent();
        }
    }
}

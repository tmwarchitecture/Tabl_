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
using Rhino.Commands;
using Rhino.DocObjects;

namespace Tabl_
{
    [Guid("FA271E46-C13B-47A5-9B69-46C578A74EA4")]
    public partial class DockPanel : UserControl
    {
        internal RhinoDoc ParentDoc { get; private set; }
        internal ObjRef[] Loaded { get; set; } = new ObjRef[] { };

        private double tol;
        private bool in_mod = false; // whether doc is modifying attr, see OnAttrMod
        private bool menustripaction = false; // whether user (un)checked in header menustrip

        // settings popup
        private Settings settings = new Settings();
        // tracking header visibilities
        private Dictionary<string, bool> headers = new Dictionary<string, bool>
        {
            { "#", false},
            {"GUID", true },
            { "Type", false},
            { "Name", true },
            { "Layer", false},
            { "Color", false},
            { "LineType", true},
            { "PrintColor", false},
            { "PrintWidth", false},
            {"Material", false },
            {"Length", false },
            {"Area" ,false},
            {"Volume", false },
            {"NumPts", false },
            {"NumEdges", false },
            {"NumFaces" , false},
            {"Degree",false },
            {"CenterPt",false },
            {"IsPlanar",false },
            {"IsClosed", false },
            {"Comments",false },
        };
        // header order
        private string[] ho = new string[]
        {
            "#","GUID","Type","Name","Layer","Color","LineType", "PrintColor","PrintWidth","Material","Length","Area", "Volume","NumPts","NumEdges","NumFaces", "Degree","CenterPt","IsPlanar","IsClosed","Comments",
        };
        // comparer
        private int HeaderSorter(string a, string b)
        {
            if (Array.IndexOf(ho, a) > Array.IndexOf(ho, b)) return 1;
            else return -1;
        }
        

        public static Guid PanelId
        {
            get { return typeof(DockPanel).GUID; }
        }

        public DockPanel()
        {
            InitializeComponent();
            InitializeCLMS();

            ParentDoc = RhinoDoc.ActiveDoc;
            tol = ParentDoc.ModelAbsoluteTolerance;
            settings.FormClosing += Refresh_Click;
            Command.EndCommand += OnDocChange;
            // set up first mod trigger, unlistened during event itself
            RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            // will relisten attr mod after all mod finish
            RhinoApp.Idle += OnRhIdle;
        }

        #region non-UI events
        // refresh after each command
        private void OnDocChange(object s, EventArgs e)
        {
            if (settings.update)
                RefreshTabl();
        }
        // first mod event handled
        private void OnAttrMod(object s, RhinoModifyObjectAttributesEventArgs e)
        {
            in_mod = true;
            RhinoDoc.ModifyObjectAttributes -= OnAttrMod;
            // unbind so subsequent events do not trigger until idle
        }
        // handle idle after all attr mod
        private void OnRhIdle(object sender, EventArgs e)
        {
            if (in_mod)
            {
                RefreshTabl();
                in_mod = false;
                RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            }
        }
        
        #endregion

        private void TablColClick(object s, ColumnClickEventArgs e)
        {
            MessageBox.Show("sorting not implemented");
        }
        private void HeaderStripClosing(object s, ToolStripDropDownClosingEventArgs e)
        {
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                menustripaction = true; // flags that column headers should be re-done
                e.Cancel = true;
            }
            else
            {
                if (!menustripaction) return; // no action, no need to redo spreadsheet headers
                else menustripaction = false; // user clicked, reset, and handle below

                SetHeaderVis();
                RefreshTabl();
            }
        }
        private void HeaderStripOpening(object s, EventArgs e)
        {
            MirrorHeaders();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            SetPickFilter(true);
            var orefs = PickObj();
            if (orefs == null)
            {
                SetPickFilter(false);
                return;
            }

            List<string> loadedids = new List<string>();
            Guid[] guids = orefs.Select(i => i.ObjectId).ToArray();
            try
            {
                // try to append to existing
                string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
                string[] idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries);
                loadedids.AddRange(idstrings);
                loadedids.AddRange(guids.Select(i=>i.ToString()));
                ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", loadedids));
            }
            catch (NullReferenceException)
            {
                ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", guids.Select(i => i.ToString())));
            }

            SetPickFilter(false);
            RefreshTabl();
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (Loaded.Length == 0)
            {
                RhinoApp.WriteLine(" nothing to remove");
                return;
            }

            List<RhinoObject> userlocked = new List<RhinoObject>();
            foreach (RhinoObject ro in ParentDoc.Objects)
                if (ro.IsLocked)
                    userlocked.Add(ro);

            SetPickFilter(userlocked.ToArray());
            ObjRef[] picked = PickObj();
            if (picked == null)
            {
                SetPickFilter(userlocked.ToArray(), false);
                return;
            }

            //first eliminate from doc string
            string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
            List<string> idstrs = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (ObjRef oref in picked)
                idstrs.Remove(oref.ObjectId.ToString());
            ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", idstrs));

            //refresh Loaded property
            ReloadRefs(idstrs);

            SetPickFilter(userlocked.ToArray(), false);
            RefreshTabl();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshTabl();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            settings.Location = Cursor.Position;
            settings.ShowDialog(this);
        }

        private void Trash_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear all?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ParentDoc.Strings.Delete("tabl_cs_selected");
                Loaded = new ObjRef[] { };
                RefreshTabl();
            }
        }

        private void Place_Click(object sender, EventArgs e)
        {
            MessageBox.Show("place spreadsheet not implemented yet");
        }

        private void Info_Click(object sender, EventArgs e)
        {
            AboutTabl_ about = new AboutTabl_();
            about.ShowDialog(this);
        }

        private void Env_Click(object sender, EventArgs e)
        {
            // this is debug popup
            // TODO: delete in production
            string msg = "";
            msg += string.Format("Loaded: {0}", Loaded.Length);

            MessageBox.Show(msg);
        }

    }
}

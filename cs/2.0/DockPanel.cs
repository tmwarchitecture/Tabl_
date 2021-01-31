using System;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace Tabl_
{
    [Guid("FA271E46-C13B-47A5-9B69-46C578A74EA4")]
    public partial class DockPanel : UserControl
    {
        internal static object locker = new object();

        internal RhinoDoc ParentDoc { get; private set; }
        // loaded objref order must always be the same as tabl line items
        internal ObjRef[] Loaded { get; set; } = new ObjRef[] { };

        private double tol;
        private double rtol;
        private bool in_mod = false; // whether doc is modifying attr, see OnAttrMod
        private bool menustripaction = false; // whether user (un)checked in header menustrip
        private bool shiftselected = false; // whether lvTabl select was multi select previously
        private int sorthdr = 0; // index of column to sort
        private int sortord = 0; // order, 0 none, 1 small to large, -1 reverse

        // settings popup
        private Settings settings = new Settings();
        // whether tabl has left most column that counts line items
        private bool linecounter = false;
        // header visibilities, dict data type has no order
        private Dictionary<string, bool> headers;
        // header order
        private string[] ho = new string[]
        {
            "GUID","Type","Name","Layer","Color","LineType", "PrintColor","PrintWidth","Material","Length","Area", "Volume","NumPts","NumEdges","NumFaces", "Degree","CenterPt","IsPlanar","IsClosed","Comments",
        };
        // comparer, used to sort dict keys
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
            InitializeLVMS();
            headers = new Dictionary<string, bool> {
                {"GUID", false },
                { "Type", true},
                { "Name", false },
                { "Layer", true},
                { "Color", false},
                { "LineType", false},
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

            ParentDoc = RhinoDoc.ActiveDoc;
            tol = ParentDoc.ModelAbsoluteTolerance;
            rtol = ParentDoc.ModelAngleToleranceRadians;
            settings.FormClosing += Refresh_Click;
            settings.docmarker.Enabled = true; // start display conduit
            Command.EndCommand += OnDocChange;
            // set up first mod trigger, unlistened during event itself
            RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            // will relisten attr mod after all mod finish
            RhinoApp.Idle += OnRhIdle;

            lvTabl.ColumnClick += TablColClick;
            /* lvTabl.MouseUp += TablShiftSelect; // multiple select, mark altogether
            lvTabl.ItemSelectionChanged += TablSelectedChanged; // one item, mark individually
            */
            lvTabl.MouseUp += TablLeftClick;
            lvTabl.KeyUp += TablCtrlC;

            ReloadRefs();
            if (Loaded.Length != 0) RefreshTabl();
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
                if (settings.update)
                    RefreshTabl();
                in_mod = false;
                RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            }
        }
        
        #endregion

        private void TablColClick(object s, ColumnClickEventArgs e)
        {
            sorthdr = e.Column;
            if (sortord == 0)
                sortord = 1;
            else if (sortord == 1)
                sortord = -1;
            else
                sortord = 0;
            
            TablSort();

            settings.docmarker.Empty();
            settings.docmarker.AddMarkerGeometries(lvTabl, Loaded);
            ParentDoc.Views.Redraw();
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
        /* deprecated
         * private void TablSelectedChanged(object s, ListViewItemSelectionChangedEventArgs e)
        {
            if (ModifierKeys == Keys.Shift) // shift + click
                return; // making sure handler not triggered for each of the multi-select
            else if (lvTabl.SelectedItems.Count>1) // click when others are selected
                return; // skip redraw

            int hli = e.ItemIndex;
            if (e.IsSelected)
                settings.docmarker.AddMarkerGeometry(hli, Loaded);
            else
                settings.docmarker.crvs[hli] = new Curve[] { };
            ParentDoc.Views.Redraw();
        }
        private async void TablShiftSelect(object s, MouseEventArgs e)
        {
            if (ModifierKeys == Keys.Shift) //shift + click
            {
                shiftselected = true;
                settings.docmarker.Empty();
                settings.docmarker.AddMarkerGeometries(lvTabl, Loaded);
                ParentDoc.Views.Redraw();
            }
            else if (shiftselected == true) // no shift this time but following a shift+click
            {
                // following a shift select
                await Task.Delay(50); // give arbitrary amount of time to docmarker unselecting
                if (ModifierKeys!= Keys.Control)
                    shiftselected = false; // control de-select 
                settings.docmarker.Empty();
                settings.docmarker.AddMarkerGeometries(lvTabl, Loaded);
                ParentDoc.Views.Redraw();
            }
        }
        */
        private void TablLeftClick(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                settings.docmarker.Empty();
                settings.docmarker.AddMarkerGeometries(lvTabl, Loaded);
            }
            ParentDoc.Views.Redraw();
        }
        private void MenuStripCopyTabl_Click(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem tsi)) return;

            string s;
            if (tsi.Text.Contains("Texts")) s = ",";
            else s = "\t";

            string[] sprdsheet = new string[lvTabl.SelectedItems.Count];
            for (int ri = 0; ri < lvTabl.SelectedItems.Count; ri++)
            {
                string[] line = new string[lvTabl.SelectedItems[ri].SubItems.Count];
                for (int ii = 0; ii < lvTabl.SelectedItems[ri].SubItems.Count; ii++)
                    line.SetValue(lvTabl.SelectedItems[ri].SubItems[ii].Text, ii);
                sprdsheet.SetValue(string.Join(s, line), ri);
            }
            string content = string.Join("\n", sprdsheet);
            Clipboard.SetText(content, TextDataFormat.Text);
        }
        private void TablCtrlC(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && ModifierKeys == Keys.Control)
            {
                ToolStripMenuItem dummy = new ToolStripMenuItem() { Text = "Spreadsheet" };
                MenuStripCopyTabl_Click(dummy, e);
            }
        }

        private void Add_Click(object sender, EventArgs e)
        {
            AddPickFilter(true);
            ObjRef[] orefs = PickObj(); // launch interactive pick
            if (orefs == null)
            {
                AddPickFilter(false);
                return;
            }

            List<string> loadedids = new List<string>();
            Guid[] guids = orefs.Select(i => i.ObjectId).ToArray();
            string raw; string[] idstrs; // get existing in tabl_cs_selected, if any
            try
            {
                raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
                idstrs = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (NullReferenceException)
            {
                raw = "";
                idstrs = new string[] { };
            }
            loadedids.AddRange(idstrs);
            loadedids.AddRange(guids.Select(i => i.ToString()));
            ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", loadedids)); // push back to talb_cs_selected
            ReloadRefs(loadedids); // sync docstr and loaded

            AddPickFilter(false);
            if (!settings.update) RefreshTabl();
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            if (Loaded.Length == 0)
            {
                RhinoApp.WriteLine(" nothing to remove");
                return;
            }

            // these are user locked, exempt from lock status change
            List<RhinoObject> userlocked = new List<RhinoObject>();
            foreach (RhinoObject ro in ParentDoc.Objects)
                if (ro.IsLocked)
                    userlocked.Add(ro);

            RmvPickFilter(userlocked.ToArray(), true);
            ObjRef[] picked = PickObj();
            if (picked == null)
            {
                RmvPickFilter(userlocked.ToArray(), false);
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

            RmvPickFilter(userlocked.ToArray(), false);
            if (!settings.update) RefreshTabl(); // cuz PickObj() triggers OnRhIdle post-command
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
        // TODO: delete in production
        private void Env_Click(object sender, EventArgs e)
        {
            // this is debug popup
            string msg = "";
            msg += string.Format("Loaded: {0}", Loaded.Length);
            string[] keys = new string[ParentDoc.Strings.Count];
            for (int i = 0; i < keys.Length; i++)
                keys.SetValue(ParentDoc.Strings.GetKey(i), i);
            msg += "\n\r" + "docstr keys: " + string.Join(",", keys);

            MessageBox.Show(msg);
        }

        private void Export_Click(object sender, EventArgs e)
        {
            DialogResult r = dlogExport.ShowDialog(this);
            if (r== DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dlogExport.OpenFile()))
                {
                    if (settings.ssopt[2])
                    {
                        // write headers
                        string[] htxts = new string[lvTabl.Columns.Count];
                        for (int i = 0; i < lvTabl.Columns.Count; i++)
                            htxts.SetValue(lvTabl.Columns[i].Text, i);
                        sw.WriteLine(string.Join(",", htxts));
                    }
                    foreach (TablLineItem li in lvTabl.Items)
                    {
                        string[] fields = new string[li.SubItems.Count];
                        for (int i = 0; i < li.SubItems.Count; i++)
                            if (li.SubItems[i].Text.Contains(","))
                                fields.SetValue("\"" + li.SubItems[i].Text + "\"", i);
                            else
                                fields.SetValue(li.SubItems[i].Text, i);
                        sw.WriteLine(string.Join(",", fields));
                    }
                }
                ToolStripMenuItem g = msHeaders.Items[1] as ToolStripMenuItem;
                if (!g.Checked)
                    MessageBox.Show("GUID column not enabled, exported csv will not be recognized upon re-import back here", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Import_Click(object sender, EventArgs e)
        {
            DialogResult r = dlogImport.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                List<ObjRef> read = new List<ObjRef>();
                List<string> errmsgs = new List<string>();
                int idcol = -1; // where is guid column, -1 no id, 0 first col, 1 second col
                foreach (string fn in dlogImport.FileNames)
                    using(TextFieldParser tpar = new TextFieldParser(fn))
                    {
                        tpar.Delimiters = new string[] { ",", };
                        string[] row;

                        // -- process first line and see which column has GUID
                        try { row = tpar.ReadFields(); } // read first line
                        catch (MalformedLineException)
                        {
                            errmsgs.Add(Path.GetFileName(fn) + " --> " + "Bad first line, aborted");
                            continue; //next file
                        }
                        string test0 = string.Empty;
                        string test1 = string.Empty;
                        if (row.Length == 1)
                            test0 = row[0];
                        else
                        {
                            test0 = row[0];
                            test1 = row[1];
                        }
                        // if one of these satifies, we are good to proceed
                        if (test0 == "GUID")
                        {
                            idcol = 0;
                        }
                        else if (test1 == "GUID")
                        {
                            idcol = 1;
                        }
                        else if (Guid.TryParse(test0, out Guid g0))
                        {
                            idcol = 0;
                            read.Add(new ObjRef(g0));
                        }
                        else if (Guid.TryParse(test1, out Guid g1))
                        {
                            idcol = 1;
                            read.Add(new ObjRef(g1));
                        }
                        else
                        {
                            errmsgs.Add(fn + "  --> " + "Missing GUID, aborted");
                            continue; //next file
                        }
                        // end of first line process --

                        while (!tpar.EndOfData)
                        {
                            try { row = tpar.ReadFields(); } // read current line
                            catch (MalformedLineException)
                            {
                                string msg = string.Format("{0} --> Bad line {1}, line skipped", Path.GetFileName(fn), tpar.LineNumber);
                                errmsgs.Add(msg);
                                continue; //next line
                            }
                            read.Add(new ObjRef(new Guid(row[idcol])));
                        }
                    }
                Loaded = read.ToArray();
                PushRefs();
                RefreshTabl();
                if (errmsgs.Count != 0)
                    MessageBox.Show(string.Join("\n\r", errmsgs), "Import errors", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        

    }
}

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
using Rhino.Geometry;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.UI;
using Rhino.Input.Custom;

namespace Tabl_
{
    [Guid("FA271E46-C13B-47A5-9B69-46C578A74EA4")] // required by Rhino
    public partial class TablDockPanel : UserControl
    {
        internal static object locker = new object();

        internal RhinoDoc ParentDoc { get; private set; }
        /*loaded objref order must always be the same as tabl line items
         headersort method reorders tabl so it has sync built in*/
        internal ObjRef[] Loaded { get; set; } = new ObjRef[] { };

        private double tol;
        private double rtol;
        private bool in_mod = false; // whether doc is modifying attr, see OnAttrMod
        private bool menustripaction = false; // whether user (un)checked in header menustrip
        private int sorthdr = 0; // index of column to sort
        private int sortord = 0; // order, 0 none, 1 small to large, -1 reverse
        private GetPoint ptgetter;

        // settings popup
        private Settings settings;
        // placement popup
        private PlaceSettings plcsettings;
        // whether tabl has left most column that counts line items
        // whenever a TablLineItem is created, query this
        private bool linecounter = false;
        // header visibilities, dict data type has no order
        private Dictionary<string, bool> headers;
        // header order
        private string[] ho = new string[]
        {
            "GUID","Type","Name","Layer","Color","LineType", "PrintColor","PrintWidth","Material","Length","Area", "Volume","NumPts","NumEdges","NumFaces", "Degree","CenterPt","Extents","IsPlanar","IsClosed","Comments",
        };
        // delegate comparer, used to sort dict keys
        private int HeaderSorter(string a, string b)
        {
            if (Array.IndexOf(ho, a) > Array.IndexOf(ho, b)) return 1;
            else return -1;
        }
        // required by Rhino
        public static Guid PanelId
        {
            get { return typeof(TablDockPanel).GUID; }
        }

        public TablDockPanel()
        {
            InitializeComponent();
            InitializeLVMS();
            
            InitializePtGetter();
            
#if !DEBUG
            btnEnv.Visible = false;
#endif

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
                {"Extents", false },
                {"IsPlanar",false },
                {"IsClosed", false },
                {"Comments",false },
            };
            ParentDoc = RhinoDoc.ActiveDoc;
            settings = new Settings(this);
            plcsettings = new PlaceSettings();
            tol = ParentDoc.ModelAbsoluteTolerance;
            rtol = ParentDoc.ModelAngleToleranceRadians;
            Command.EndCommand += OnDocChange;
            // set up first mod trigger, unlistened during event itself
            RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            // will relisten attr mod cuz all mod has finished
            RhinoApp.Idle += OnRhIdle;

            lvTabl.ColumnClick += TablColClick;
            lvTabl.MouseUp += TablMouseUp;
            lvTabl.KeyUp += TablCtrlC;
            tbmsNameChange.KeyUp += MenuStripNameChange_Enter;
            tbmsNameChange.MouseLeave += MenuStripNameTB_DeFocus;

            ReloadRefs();
            if (Loaded.Length != 0) RefreshTabl();
        }

#region non-UI handlers
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
            // will relisten attr mod cuz all mod has finished
            if (in_mod)
            {
                if (settings.update)
                    RefreshTabl();
                in_mod = false;
                RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            }
        }

        

#endregion

#region ListVeiw tabl handlers
        private void TablColClick(object s, ColumnClickEventArgs e)
        {
            sorthdr = e.Column;
            if (sortord == 0)
                sortord = 1;
            else if (sortord == 1)
                sortord = -1;
            else
                sortord = 0;
            
            HeaderClickSort();

            settings.docmarker.Empty();
            settings.docmarker.AddMarkers(lvTabl, Loaded);
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
        private void TablMouseUp(object s, MouseEventArgs e)
        {
            settings.docmarker.Empty();
            settings.docmarker.AddMarkers(lvTabl, Loaded);
            
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
            try { Clipboard.SetText(content, TextDataFormat.Text); }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Nothing to copy\nEither no items selected or no property column is showing at all", "Not Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void TablCtrlC(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && ModifierKeys == Keys.Control)
            {
                ToolStripMenuItem dummy = new ToolStripMenuItem() { Text = "Spreadsheet" };
                MenuStripCopyTabl_Click(dummy, e);
            }
        }
        private void MenuStripZoom_Click(object sender, EventArgs e)
        {
            ParentDoc.Views.RedrawEnabled = false;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
                ParentDoc.Objects.Select(tli.RefId, true);
            ParentDoc.Views.ActiveView.ActiveViewport.ZoomExtentsSelected();
            foreach (TablLineItem tli in lvTabl.SelectedItems)
                ParentDoc.Objects.Select(tli.RefId, false);
            ParentDoc.Views.RedrawEnabled = true;
        }
        private void MenuStripChangeLayer_Click(object sender, EventArgs e)
        {
            int li = -1;
            bool current = false;
            bool r = Dialogs.ShowSelectLayerDialog(ref li, "Target Layer", false, false, ref current);
            if (r && li != -1)
            {
                int errn = 0;
                foreach (TablLineItem tli in lvTabl.SelectedItems)
                {
                    ObjRef oref = new ObjRef(tli.RefId);
                    var obj = oref.Object();
                    obj.Attributes.LayerIndex = li;
                    if (!obj.CommitChanges())
                        errn++;
                }
                if (errn>0)
                    MessageBox.Show(string.Format("error changing {0} layer(s)\nplease email support\nsorry about the inconvenience",errn));
                ParentDoc.Views.Redraw();
            }
        }
        private void MenuStripNameChange_Apply(object sender, EventArgs e)
        {
            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.Name = tbmsNameChange.Text;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} layer(s)\nplease email support\nsorry about the inconvenience", errn));

            foreach (TablLineItem tli in lvTabl.SelectedItems)
                RefreshTabl(new ObjRef(tli.RefId));
        }
        private void MenuStripNameChange_Enter(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                MenuStripNameChange_Apply(null, null); // dummy args cuz method doesn't use them
                lvCtxtMenu.Close();
            }
        }
        private void MenuStripNameTB_DeFocus(object s, EventArgs e)
        {
            if (s is ToolStripTextBox menutb)
                menutb.GetCurrentParent().Focus();
        }
        private void MenuStripColorChange_Click(object s, EventArgs e)
        {
            Color lclr = Color.Chartreuse;
            bool r = Dialogs.ShowColorDialog(ref lclr);
            if (!r) return;

            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.ColorSource = ObjectColorSource.ColorFromObject;
                obj.Attributes.ObjectColor = lclr;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} color(s)\nplease email support\nsorry about the inconvenience", errn));

            foreach (TablLineItem tli in lvTabl.SelectedItems)
                RefreshTabl(new ObjRef(tli.RefId));
        }
        private void MenuStripLTypeChange_Click(object s, EventArgs e)
        {
            int ltidx = 0;
            bool r = Dialogs.ShowSelectLinetypeDialog(ref ltidx, false);
            if (!r) return;

            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.LinetypeSource = ObjectLinetypeSource.LinetypeFromObject;
                obj.Attributes.LinetypeIndex = ltidx;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} line type(s)\nplease email support\nsorry about the inconvenience", errn));

            foreach (TablLineItem tli in lvTabl.SelectedItems)
                RefreshTabl(new ObjRef(tli.RefId));
        }
        private void MenuStripPrintClrChange_Click(object sender, EventArgs e)
        {
            Color pclr = Color.Black;
            bool r = Dialogs.ShowColorDialog(ref pclr);
            if (!r) return;

            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.PlotColorSource = ObjectPlotColorSource.PlotColorFromObject;
                obj.Attributes.PlotColor = pclr;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} plot color(s)\nplease email support\nsorry about the inconvenience", errn));

            foreach (TablLineItem tli in lvTabl.SelectedItems)
                RefreshTabl(new ObjRef(tli.RefId));
        }
        private void MenuStripPrintWChange_Click(object sender, EventArgs e)
        {
            double wnum = 0;
            bool r = Dialogs.ShowNumberBox("New PlotWidth", "input number ", ref wnum);
            if (!r) return;

            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.PlotWeightSource = ObjectPlotWeightSource.PlotWeightFromObject;
                obj.Attributes.PlotWeight = wnum;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} plot color(s)\nplease email support\nsorry about the inconvenience", errn));

            foreach (TablLineItem tli in lvTabl.SelectedItems)
                RefreshTabl(new ObjRef(tli.RefId));
        }
        private void MenuStripMatChange_Click(object sender, EventArgs e)
        {
            // TODO: needs working material assignment
#if DEBUG
            MessageBox.Show("not implemented...");
#else
            int matidx;
            List<string> mats = new List<string>();
            foreach (RenderMaterial mat in ParentDoc.RenderMaterials)
            {
                mats.Add(mat.Name);
            }
            object selection = Dialogs.ShowListBox("Materials", "select target material", mats);
            if (selection is string lname)
                matidx = ParentDoc.Materials.Find(lname, true);
            else
            {
                MessageBox.Show("Material selection failed. Either no material in the document or selection improper", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int errn = 0;
            foreach (TablLineItem tli in lvTabl.SelectedItems)
            {
                ObjRef oref = new ObjRef(tli.RefId);
                var obj = oref.Object();
                obj.Attributes.MaterialSource = ObjectMaterialSource.MaterialFromObject;
                if (matidx == -1)
                {
                    errn++;
                    continue;
                }
                obj.Attributes.MaterialIndex = matidx;
                if (!obj.CommitChanges())
                    errn++;
            }
            if (errn > 0)
                MessageBox.Show(string.Format("error changing {0} material(s)\nplease email support\nsorry about the inconvenience", errn));
            RefreshTabl();
#endif
        }
        private void MenuStripCommentsChange_Click(object sender, EventArgs e)
        {
            if (Dialogs.ShowEditBox("User Text", "set key", "Tabl_", false, out string k))
                if (Dialogs.ShowEditBox("User Text", "set texts", "", true, out string txts))
                {
                    if (k == "" || txts == "") return;
                    foreach (TablLineItem tli in lvTabl.SelectedItems)
                    {
                        ObjRef oref = new ObjRef(tli.RefId);
                        var obj = oref.Object();
                        obj.Attributes.SetUserString(k, txts);
                    }

                    foreach (TablLineItem tli in lvTabl.SelectedItems)
                        RefreshTabl(new ObjRef(tli.RefId));
                }
        }
        #endregion

        #region main UI handlers
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

        internal void Refresh_Click(object sender, EventArgs e)
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
                settings.docmarker.Empty();
                RefreshTabl();
            }
        }

        private void Place_Click(object sender, EventArgs e)
        {
            plcsettings.Location = Cursor.Position;
            plcsettings.ShowDialog(); // closing form won't dispose it
            if (plcsettings.ok)
            {
                plcsettings.RectFollower.rec = InDocTabl(out List<TextEntity[]> content, out List<Line> borders);
                if (ptgetter.Get(true)== Rhino.Input.GetResult.Point)
                {
                    // TODO: record undos
                    Plane trgt = new Plane(ptgetter.Point(), plcsettings.BasePlane.XAxis, plcsettings.BasePlane.YAxis);
                    Transform xform = Orient(plcsettings.BasePlane, trgt);
                    foreach (Line l in borders)
                    {
                        l.Transform(xform);
                        ParentDoc.Objects.AddLine(l);
                    }
                    foreach (TextEntity[] row in content)
                        foreach(TextEntity cell in row)
                        {
                            cell.Transform(xform);
                            ParentDoc.Objects.AddText(cell);
                        }
                }
            }
            //TODO: implement placement of tabl in doc view
        }

        private void Info_Click(object sender, EventArgs e)
        {
            AboutTabl_ about = new AboutTabl_();
            about.ShowDialog(this);
        }
#if DEBUG
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
#endif
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

        private void SelectInDoc_Click(object sender, EventArgs e)
        {
            foreach(TablLineItem tli in lvTabl.SelectedItems)
                ParentDoc.Objects.Select(tli.RefId, true);
            ParentDoc.Views.Redraw();
        }
        #endregion
    }
}

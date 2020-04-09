using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Rhino.DocObjects;
using Rhino;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.Commands;


namespace Tabl_cs
{
    public partial class MainForm : Form
    {
        private RhinoDoc parent;
        private ObjRef[] selected;

        private double tol;
        private double rtol;
        private double atol_deg;
        private double atol_rad;
        // 0 - show units; 1 - show total; 2 - export headers
        private bool[] options = new bool[] { false, false, false };

        /// <summary>
        /// construtor
        /// </summary>
        /// <param name="rhsess">current rhino session</param>
        /// <param name="title">name of the window</param>
        public MainForm(RhinoDoc rhsess, string title = "Tabl_")
        {
            // handle bank-end
            parent = rhsess;
            RegisterTabl_();
            tol = parent.ModelAbsoluteTolerance;
            rtol = parent.ModelRelativeTolerance;
            atol_deg = parent.ModelAngleToleranceDegrees;
            atol_rad = parent.ModelAngleToleranceRadians;
            LoadORefs();


            // handle front-end
            InitializeComponent();
            FormClosing += OnFormClosing;
            Name = title;
            Icon = Properties.Resources.main;

            // set default checked items on properties
            checkedListBox1.SetItemChecked(3, true);
            checkedListBox1.SetItemChecked(4, true);
            // only bind before last setitemcheck so
            // itemcheck event only fire once
            checkedListBox1.ItemCheck += checkedListBox1_CheckedChanged;
            checkedListBox1.SetItemChecked(12, true);

            checkedListBox2.ItemCheck += checkedListBox2_CheckedChanged;

            dataGridView1.ColumnCount = checkedListBox1.CheckedItems.Count;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                dataGridView1.Columns[i].Name = checkedListBox1.CheckedItems[i].ToString();
        }

        #region user methods

        /// <summary>
        /// refresh datagridview headers; call RefreshDGVContent instead
        /// </summary>
        /// <param name="header">header to subtract or add</param>
        /// <param name="add">true to add, false to subtract</param>
        private void RefreshDGVHeaders(string header, bool add)
        {
            List<string> headers = new List<string>();
            List<int> hi = new List<int>(); // index of header in headers as positioned in checklist
            
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                headers.Add(checkedListBox1.CheckedItems[i].ToString());
                hi.Add(checkedListBox1.CheckedIndices[i]);
            }

            if (!add) headers.Remove(header);
            else
            {
                List<string> allitems = new List<string>(); // str repr of all items in checklistbox
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    allitems.Add(checkedListBox1.Items[i].ToString());
                int newi = allitems.IndexOf(header);
                if (hi.Count != 0)
                {
                    for (int ii = 0; ii < hi.Count; ii++)
                        if (newi < hi[ii])
                        {
                            headers.Insert(ii, header);
                            break;
                        }
                        else if (ii == hi.Count - 1 && newi > hi[ii])
                            headers.Add(header);
                }
                else
                    headers.Add(header);
            }

            dataGridView1.ColumnCount = headers.Count;
            for (int j = 0; j < headers.Count; j++)
                dataGridView1.Columns[j].Name = headers[j];
        }
        /// <summary>
        /// update the datagridview
        /// </summary>
        /// <param name="header">header to modify; use empty to skip</param>
        /// <param name="add">add or remove header; irrelevatn if `header` is empty string</param>
        /// <param name="orefs">selected rhino object references</param>
        internal void RefreshDGVContent(string header, bool add)
        {
            if (header!="")
                RefreshDGVHeaders(header, add);

            dataGridView1.Rows.Clear();
            string[] headers = new string[dataGridView1.Columns.Count];
            for (int i=0;i<dataGridView1.Columns.Count;i++)
            {
                DataGridViewColumn c = dataGridView1.Columns[i];
                headers.SetValue(c.Name, i);
            }

            LoadORefs(); // this eidts "selected" field
            string[,] meta = GetMeta(selected, headers);
            for (int i = 0; i <= meta.GetUpperBound(0); i++)
            {
                string[] row = new string[meta.GetUpperBound(1)+1];
                for (int j = 0; j <= meta.GetUpperBound(1); j++)
                    row.SetValue(meta[i, j], j);

                dataGridView1.Rows.Add(row);
                // enumerate row #
                int last = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.None);
                dataGridView1.Rows[last].HeaderCell.Value = i.ToString();
            }

            if (options[1])
                DBVTotal();
        }

        /// <summary>
        /// load "tabl_cs_selected" into obj refs
        /// </summary>
        /// <returns>returns true if something is loaded</returns>
        private bool LoadORefs()
        {
            
            try
            {
                string raw = parent.Strings.GetValue("tabl_cs_selected");
                string[] idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries);
                selected = new ObjRef[idstrings.Length];
                for (int i = 0; i < idstrings.Length; i++)
                    selected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
                return true;
            }
            catch (NullReferenceException)
            {
                selected = new ObjRef[] { };
                return false;
            }

        }

        /// <summary>
        /// register tabl_ state
        /// </summary>
        /// <param name="open">set state value</param>
        private void RegisterTabl_(bool open = true)
        {
            if (open)
                parent.Strings.SetString("tabl_cs_session", "open");
            else
                parent.Strings.SetString("tabl_cs_session", "closed");
        }

        /// <summary>
        /// launch rhino get object command
        /// </summary>
        /// <param name="parent">parent form that started the command</param>
        /// <param name="ot">selection object type</param>
        /// <returns>array of ObjRef</returns>
        private ObjRef[] PickObj(ObjectType ot = ObjectType.AnyObject)
        {
            WindowState = FormWindowState.Minimized;

            //ObjRef[] picked = await Task.Run(() =>
            //{
            //    Result r = RhinoGet.GetMultipleObjects(" select object(s)", true, ot, out ObjRef[] objs);
            //    if (r == Result.Success)
            //        return objs;
            //    else return new ObjRef[] { };
            //});
            //parent.TopMost = true;
            //parent.BringToFront();
            //return picked;


            Result r = RhinoGet.GetMultipleObjects(" select object(s)", true, ot, out ObjRef[] picked);
            if (r == Result.Success)
            {
                WindowState = FormWindowState.Normal;
                return picked;
            }
            else
            {
                RhinoApp.WriteLine(" nothing selected...");
                WindowState = FormWindowState.Normal;
                return new ObjRef[] { };
            }
        }

        /// <summary>
        /// read meta data from doc objects
        /// </summary>
        /// <param name="orefs">Object references from doc</param>
        /// <param name="propkeys">names of the meta data to query</param>
        /// <returns>2d array of meta data, i row index, j column index</returns>
        private string[,] GetMeta(ObjRef[] orefs, string[] propkeys)
        {
            string[,] matrix = new string[orefs.Length, propkeys.Length];
            for (int i = 0; i < orefs.Length; i++)
            {
                ObjRef oref = orefs[i];
                string[] props = GetProp(oref, propkeys);
                for (int j = 0; j < props.Length; j++)
                    matrix.SetValue(props[j], i, j);
            }

            return matrix;
        }
        /// <summary>
        /// read meta data from one doc object
        /// </summary>
        /// <param name="oref">object reference</param>
        /// <param name="propkeys">query keys</param>
        /// <returns>array of properties</returns>
        private string[] GetProp(ObjRef oref, string[] propkeys)
        {
            string[] line = new string[propkeys.Length];
            RhinoObject obj = oref.Object();
            //TODO: obj could be null if user deleted something referenced in "selected"
            for (int i = 0; i < propkeys.Length; i++)
            {
                string k = propkeys[i];
                switch (k)
                {
                    case "GUID":
                        line.SetValue(oref.ObjectId.ToString(), i);
                        break;
                    case "Type":
                        line.SetValue(obj.ObjectType.ToString(), i);
                        break;
                    case "Name":
                        line.SetValue(obj.Name, i);
                        break;
                    case "Layer":
                        var li = obj.Attributes.LayerIndex;
                        var layer = RhinoDoc.ActiveDoc.Layers.FindIndex(li);
                        line.SetValue(layer.Name, i);
                        break;
                    case "Color":
                        var c = obj.Attributes.ObjectColor;
                        line.SetValue(c.ToString(), i);
                        break;
                    case "LineType":
                        var lti = obj.Attributes.LinetypeIndex;
                        var lt = RhinoDoc.ActiveDoc.Linetypes.FindIndex(lti);
                        line.SetValue(lt.Name, i);
                        break;
                    case "PrintColor":
                        var pc = obj.Attributes.PlotColor;
                        line.SetValue(pc.ToString(), i);
                        break;
                    case "PrintWidth":
                        var pw = obj.Attributes.PlotWeight;
                        line.SetValue(pw.ToString(), i);
                        break;
                    case "Material":
                        var mti = obj.Attributes.MaterialIndex;
                        var mt = RhinoDoc.ActiveDoc.Materials.FindIndex(mti);
                        if (mt == null) line.SetValue(null, i);
                        else line.SetValue(mt.Name, i);
                        break;
                    case "Length":
                        if (obj.ObjectType == ObjectType.Curve)
                            line.SetValue(oref.Curve().GetLength().ToString(), i);
                        else
                            line.SetValue(null, i);
                        break;
                    case "Area":
                        AreaMassProperties amp = null;
                        if (obj.ObjectType == ObjectType.Brep)
                            amp = AreaMassProperties.Compute(oref.Brep());
                        else if (obj.ObjectType == ObjectType.Curve)
                            if (oref.Curve().IsClosed)
                                amp = AreaMassProperties.Compute(oref.Curve());
                            else amp = null;
                        else if (obj.ObjectType == ObjectType.Extrusion)
                            if (obj.Geometry.HasBrepForm)
                                amp = AreaMassProperties.Compute(Brep.TryConvertBrep(obj.Geometry));
                            else amp = null;

                        if (amp != null)
                            line.SetValue(amp.Area.ToString(), i);
                        else
                            line.SetValue(null, i);
                        break;
                    case "Volume":
                        if (obj.ObjectType == ObjectType.Brep)
                            if (oref.Brep().IsSolid)
                                line.SetValue(oref.Brep().GetVolume(rtol, tol), i);
                            else
                                line.SetValue("open brep", i);
                        else if (obj.ObjectType == ObjectType.Extrusion)
                        {
                            Brep b = Brep.TryConvertBrep(obj.Geometry);
                            if (b == null) line.SetValue(" invalid extrusion", i);
                            else
                                if (b.IsSolid)
                                    line.SetValue(b.GetVolume(rtol, tol), i);
                                else line.SetValue("open brep", i);
                        }
                        else
                            line.SetValue(null, i);
                        break;
                    case "NumPts":
                        if (obj.ObjectType == ObjectType.Curve)
                            line.SetValue(oref.Curve().ToNurbsCurve().Points.Count.ToString(), i);
                        else line.SetValue(null, i);
                        break;
                    case "NumEdges":
                        if (obj.ObjectType == ObjectType.Brep)
                            line.SetValue(oref.Brep().Edges.Count.ToString(), i);
                        else if (obj.ObjectType == ObjectType.Extrusion)
                            if (obj.Geometry.HasBrepForm)
                                line.SetValue(Brep.TryConvertBrep(obj.Geometry).Edges.Count.ToString(), i);
                            else line.SetValue("invalid extrusion", i);
                        else line.SetValue(null, i);
                        break;
                    case "NumFaces":
                        if (obj.ObjectType == ObjectType.Brep)
                            line.SetValue(oref.Brep().Faces.Count.ToString(), i);
                        else if (obj.ObjectType == ObjectType.Extrusion)
                            if (obj.Geometry.HasBrepForm)
                                line.SetValue(Brep.TryConvertBrep(obj.Geometry).Faces.Count.ToString(), i);
                            else line.SetValue("invalid extrusion", i);
                        else line.SetValue(null, i);
                        break;
                    case "Degree":
                        if (obj.ObjectType == ObjectType.Curve)
                            line.SetValue(oref.Curve().Degree.ToString(), i);
                        else line.SetValue(null, i);
                        break;
                    case "CenterX":
                        line.SetValue(obj.Geometry.GetBoundingBox(false).Center.X.ToString(), i);
                        break;
                    case "CenterY":
                        line.SetValue(obj.Geometry.GetBoundingBox(false).Center.Y.ToString(), i);
                        break;
                    case "CenterZ":
                        line.SetValue(obj.Geometry.GetBoundingBox(false).Center.Z.ToString(), i);
                        break;
                    case "IsPlanar":
                        if (obj.ObjectType == ObjectType.Brep)
                        {
                            Brep brep = oref.Brep();
                            if (brep.IsSurface)
                                if (brep.Faces[0].IsPlanar())
                                    line.SetValue("yes", i);
                                else line.SetValue("no", i);
                            else line.SetValue("polysrf", i);
                        }
                        else if (obj.ObjectType == ObjectType.Curve)
                            if (oref.Curve().IsPlanar())
                                line.SetValue("yes", i);
                            else line.SetValue("no", i);
                        else if (obj.ObjectType == ObjectType.Extrusion)
                            if (obj.Geometry.HasBrepForm)
                            {
                                Brep brep = Brep.TryConvertBrep(obj.Geometry);
                                if (brep.IsSurface)
                                    if (brep.Faces[0].IsPlanar())
                                        line.SetValue("yes", i);
                                    else line.SetValue("no", i);
                                else line.SetValue("polysrf", i);
                            }
                            else line.SetValue("invalid extrusion", i);
                        else line.SetValue("irrelevant", i);
                        break;
                    case "IsClosed":
                        if (obj.ObjectType == ObjectType.Brep)
                        {
                            Brep brep = oref.Brep();
                            if (brep.IsSolid)
                                line.SetValue("yes", i);
                            else line.SetValue("no", i);
                        }
                        else if (obj.ObjectType == ObjectType.Curve)
                            if (oref.Curve().IsClosed)
                                line.SetValue("yes", i);
                            else line.SetValue("no", i);
                        else if (obj.ObjectType == ObjectType.Extrusion)
                            if (obj.Geometry.HasBrepForm)
                            {
                                Brep brep = Brep.TryConvertBrep(obj.Geometry);
                                if (brep.IsSolid)
                                    line.SetValue("yes", i);
                                else line.SetValue("no", i);
                            }
                            else line.SetValue("invalid extrusion", i);
                        else line.SetValue("irrelevant", i);
                        break;
                    case "Comments":
                        var usertxts = obj.Attributes.GetUserStrings();
                        line.SetValue("no_imp", i);
                        break;
                    default:
                        break;
                }
            }
            return line;
        }

        /// <summary>
        /// lock or unlock objects not in tabl_ selected
        /// </summary>
        /// <param name="expt">exempt objects to remain locked; can be null if !unlock</param>
        /// <param name="unlock">set true to lock</param>
        private void PickFilter(RhinoObject[] expt, bool unlock=false)
        {
            string[] idstrings = new string[selected.Length];
            for (int i = 0; i < idstrings.Length; i++)
                idstrings.SetValue(selected[i].ObjectId.ToString(), i);
            parent.Views.RedrawEnabled = false;
            if (unlock)
                foreach (RhinoObject robj in parent.Objects)
                {
                    bool skip = false;
                    foreach (RhinoObject e in expt)
                        if (robj.Id == e.Id)
                        {
                            skip = true;
                            break;
                        }
                    if (skip) continue;
                    else parent.Objects.Unlock(robj.Id, false);
                }
            else
                foreach (RhinoObject robj in parent.Objects)
                    if (!idstrings.Contains(robj.Id.ToString()))
                        parent.Objects.Lock(robj.Id, false);
                    else continue;
            parent.Views.RedrawEnabled = true;
        }

        /// <summary>
        /// add total at bottom of datagridview
        /// </summary>
        private void DBVTotal()
        {
            string[] totalrow = new string[dataGridView1.ColumnCount];
            for (int ci = 0; ci < dataGridView1.ColumnCount; ci++)
            {
                double colsum = 0.0;
                for (int ri = 0; ri < dataGridView1.RowCount; ri++)
                {
                    string val = dataGridView1.Rows[ri].Cells[ci].Value.ToString();
                    if (double.TryParse(val, out double num))
                        colsum += num;
                }
                totalrow.SetValue(colsum.ToString(), ci);
            }
            dataGridView1.Rows.Add(totalrow);
            int last = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.None);
            dataGridView1.Rows[last].HeaderCell.Value = "SUM";
        }

        #endregion

        // right before winform closes
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            RegisterTabl_(false);
            // register this form as closed
        }

        // click add
        private void button1_Click(object sender, EventArgs e)
        {
            var orefs = PickObj();
            if (orefs == null) return;
            List<string> selectedids = new List<string>();
            string[] guids = new string[orefs.Length];
            for (int i = 0; i < orefs.Length; i++)
                guids.SetValue(orefs[i].ObjectId.ToString(), i);
            try
            {
                // try to append to existing
                string raw = parent.Strings.GetValue("tabl_cs_selected");
                string[] idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries);
                selectedids.AddRange(idstrings);
                selectedids.AddRange(guids);
                parent.Strings.SetString("tabl_cs_selected", string.Join(",", selectedids));
            }
            catch (NullReferenceException) { parent.Strings.SetString("tabl_cs_selected", string.Join(",", guids)); }

            RefreshDGVContent("", true);
        }

        // properties check change
        private void checkedListBox1_CheckedChanged(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                RefreshDGVContent(checkedListBox1.Items[e.Index].ToString(), true);
            else
                RefreshDGVContent(checkedListBox1.Items[e.Index].ToString(), false);
        }

        // check all propoerties
        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                // unbind and bind before last item is checked
                // to eliminate check event trigger every single time!
                if (i == 0)
                    checkedListBox1.ItemCheck -= checkedListBox1_CheckedChanged;
                if (i== checkedListBox1.Items.Count-1)
                    checkedListBox1.ItemCheck += checkedListBox1_CheckedChanged;

                if (!checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, true);
            }
        
        }

        // check none properties
        private void checkNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                // unbind 
                // to eliminate check event trigger every single time!
                if (i == 0)
                    checkedListBox1.ItemCheck -= checkedListBox1_CheckedChanged;

                if (checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, false);
            }
            dataGridView1.Columns.Clear();
            // bind back check event trigger
            checkedListBox1.ItemCheck += checkedListBox1_CheckedChanged;
        }

        // click to remove
        private void button2_Click(object sender, EventArgs e)
        {
            if (selected.Length == 0)
            {
                RhinoApp.WriteLine(" nothing to remove");
                return;
            }

            List<RhinoObject> userlocked = new List<RhinoObject>();
            foreach (RhinoObject ro in parent.Objects)
                if (ro.IsLocked)
                    userlocked.Add(ro);
            PickFilter(userlocked.ToArray());
            ObjRef[] picked = PickObj();
            if (picked == null)
            {
                PickFilter(userlocked.ToArray(), true);
                return;
            }

            //first eliminate from doc string
            string raw = parent.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (ObjRef oref in picked)
                idstrings.Remove(oref.ObjectId.ToString());
            parent.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace `selected` field
            ObjRef[] newselected = new ObjRef[idstrings.Count];
            for (int i = 0; i < idstrings.Count; i++)
                newselected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
            selected = newselected;

            PickFilter(userlocked.ToArray(), true);
            RefreshDGVContent("", true);
        }

        // refresh meta data button
        private void button3_Click(object sender, EventArgs e)
        {
            RefreshDGVContent("", true);
        }

        // options check box check change
        private void checkedListBox2_CheckedChanged(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                options.SetValue(true, e.Index);
            else options.SetValue(false, e.Index);
            RefreshDGVContent("", true);
        }

        // show doc string info in a msgbox
        private void devLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string msg = "";
            for (int i = 0; i < parent.Strings.Count; i++)
            {
                string k = parent.Strings.GetKey(i);
                string v = parent.Strings.GetValue(k);
                if (v.Length > 100)
                    v = v.Substring(0, 99) + " ...";
                msg += i.ToString() + " - " + k + "\n" + v + "\n";
            }
            if (msg.Length > 1000)
                msg = msg.Substring(0, 999) + "(truncated)";
            MessageBox.Show(msg, "DevLog", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // settings button click
        private void button4_Click(object sender, EventArgs e)
        {
            Settings popup = new Settings();
            popup.ShowDialog(this);
        }

        // close this tabl_ window
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        // clear selected and the doctring k-v pair
        private void clearTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            parent.Strings.Delete("tabl_cs_selected");
            selected = new ObjRef[] { };
            RefreshDGVContent("", true);
        }
    }
}
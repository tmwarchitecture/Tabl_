using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino.DocObjects;
using Rhino;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.Commands;
using System.IO;

namespace Tabl_cs
{
    [Guid("FA291E46-C13B-47A5-9B59-46C578A74EA3")]
    public partial class DockPanel : UserControl
    {
        private RhinoDoc parent;
        private ObjRef[] selected;

        private double tol;
        private double rtol;
        private double atol_deg;
        private double atol_rad;
        private string docunit;
        // 0 - show units; 1 - show total; 2 - export headers
        private bool[] options = new bool[] { false, false, false };
        private int clickedrowindex; // see event handler OnRowHeaderRightClick

        private DataTable dt = new DataTable();

        private Settings popup = new Settings();

        // constructor
        public DockPanel()
        {
            parent = RhinoDoc.ActiveDoc;
            //RegisterTabl_();
            tol = parent.ModelAbsoluteTolerance;
            rtol = parent.ModelRelativeTolerance;
            atol_deg = parent.ModelAngleToleranceDegrees;
            atol_rad = parent.ModelAngleToleranceRadians;

            LoadORefs();

            InitializeComponent();
            Tabl_cs.Instance.TablPanel = this;
            VisibleChanged += OnDockVisibleChanged;
            Disposed += OnDisposed;
            Resize += OnResize;

            popup.FormClosing += button3_Click;
            Command.EndCommand += OnRhDocChange;
            RhinoDoc.ModifyObjectAttributes += OnRhDocChange;
            // set default checked items on properties
            checkedListBox1.SetItemChecked(3, true);
            checkedListBox1.SetItemChecked(4, true);
            // only bind before last setitemcheck so
            // itemcheck event only fire once
            checkedListBox1.ItemCheck += checkedListBox1_CheckedChanged;
            checkedListBox1.SetItemChecked(1, true);

            checkedListBox2.ItemCheck += checkedListBox2_CheckedChanged;

            dataGridView1.ColumnCount = checkedListBox1.CheckedItems.Count;
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].Name = checkedListBox1.CheckedItems[i].ToString();
                dt.Columns.Add(dataGridView1.Columns[i].Name);
            }
            dataGridView1.CellMouseClick += OnDGVRightClick;


        }

        public static Guid PanelId
        {
            get { return typeof(DockPanel).GUID; }
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
            if (header != "")
                RefreshDGVHeaders(header, add);

            dataGridView1.Rows.Clear();
            string[] headers = new string[dataGridView1.Columns.Count];
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                DataGridViewColumn c = dataGridView1.Columns[i];
                headers.SetValue(c.Name, i);
            }

            LoadORefs(); // this eidts "selected" field
            if (headers.Length == 0) return; // no columns, no data should show
            string[,] meta = GetMeta(selected, headers);

            for (int i = 0; i <= meta.GetUpperBound(0); i++)
            {
                string[] row = new string[meta.GetUpperBound(1) + 1];
                for (int j = 0; j <= meta.GetUpperBound(1); j++)
                    row.SetValue(meta[i, j], j);

                
                dataGridView1.Rows.Add(row);
                // enumerate row # and set in row header
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
        /// register tabl_ state DEFUNCT
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
            //WindowState = FormWindowState.Minimized;

            Result r = RhinoGet.GetMultipleObjects(" select object(s)", true, ot, out ObjRef[] picked);
            if (r == Result.Success)
            {
                //WindowState = FormWindowState.Normal;
                return picked;
            }
            else
            {
                RhinoApp.WriteLine(" nothing selected...");
                //WindowState = FormWindowState.Normal;
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

            Parallel.For(0, orefs.Length, i =>
            {
                ObjRef oref = orefs[i];
                string[] props = GetProp(oref, propkeys);
                object locker = new object();
                for (int j = 0; j < props.Length; j++)
                    lock (locker) { matrix.SetValue(props[j], i, j); }
            });
            /* uncomment below block to go back to single thread query
            for (int i = 0; i < orefs.Length; i++)
            {
                ObjRef oref = orefs[i];
                string[] props = GetProp(oref, propkeys);
                for (int j = 0; j < props.Length; j++)
                    matrix.SetValue(props[j], i, j);
            }*/

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

            // catch when user delete object in doc but tabl_ still has reference
            // oref can still return guid string, use it to remove in selected and docstr
            if (obj == null || oref == null)
            {
                line.SetValue("MISSING OBJECT", 0);
                return line;
            }

            for (int i = 0; i < propkeys.Length; i++)
            {
                string k = propkeys[i];
                if (popup.cun != "" && popup.cun != null)
                    docunit = popup.cun;
                else
                    docunit = parent.GetUnitSystemName(true, false, true, true);
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
                        var layer = parent.Layers.FindIndex(li);
                        line.SetValue(layer.Name, i);
                        break;
                    case "Color":
                        var c = obj.Attributes.DrawColor(parent);
                        if (popup.cf == 0)
                            line.SetValue(c.ToString(), i);
                        else if (popup.cf == 1)
                            line.SetValue(string.Format("{0}-{1}-{2}", c.R, c.G, c.B), i);
                        else
                            line.SetValue(string.Format("{0} {1} {2}", c.R, c.G, c.B), i);
                        break;
                    case "LineType":
                        var lti = parent.Linetypes.LinetypeIndexForObject(obj);
                        var lt = parent.Linetypes[lti].Name;
                        line.SetValue(lt, i);
                        break;
                    case "PrintColor":
                        Color pc;
                        var pcs = obj.Attributes.PlotColorSource;
                        if (pcs == ObjectPlotColorSource.PlotColorFromLayer)
                        {
                            li = obj.Attributes.LayerIndex;
                            layer = parent.Layers.FindIndex(li);
                            pc = layer.PlotColor;
                        }
                        else pc = obj.Attributes.PlotColor;
                        if (popup.cf == 0)
                            line.SetValue(pc.ToString(), i);
                        else if (popup.cf == 1)
                            line.SetValue(string.Format("{0}-{1}-{2}", pc.R, pc.G, pc.B), i);
                        else
                            line.SetValue(string.Format("{0} {1} {2}", pc.R, pc.G, pc.B), i);
                        break;
                    case "PrintWidth":
                        double pw;
                        var pws = obj.Attributes.PlotWeightSource;
                        if (pws == ObjectPlotWeightSource.PlotWeightFromLayer)
                        {
                            li = obj.Attributes.LayerIndex;
                            layer = parent.Layers.FindIndex(li);
                            pw = layer.PlotWeight;
                        }
                        else pw = obj.Attributes.PlotWeight;
                        if (options[0]) line.SetValue(pw.ToString() + "pt", i); // with unit
                        else line.SetValue(pw.ToString(), i);
                        break;
                    case "Material":
                        var mti = obj.Attributes.MaterialIndex;
                        var mt = parent.Materials.FindIndex(mti);
                        if (mt == null) line.SetValue(null, i);
                        else line.SetValue(mt.Name, i);
                        break;
                    case "Length":
                        string len = null;
                        if (obj.ObjectType == ObjectType.Curve)
                        {
                            var len_num = Math.Round(oref.Curve().GetLength(), popup.dp); //decimal
                            len_num *= popup.su; //custom scale
                            if (popup.ts == ",")
                                len = len_num.ToString("#,##0.00");
                            else if (popup.ts == ".")
                                len = len_num.ToString("#.##0.00");
                            else if (popup.ts == " ")
                                len = len_num.ToString("# ##0.00");
                            else len = len_num.ToString();
                        }
                        if (options[0] && len != null) len += docunit; // with unit
                        line.SetValue(len, i);
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
                        {
                            double area_num = Math.Round(amp.Area, popup.dp);
                            area_num *= popup.su;
                            string area;
                            if (popup.ts == ",")
                                area = area_num.ToString("#,##0.00");
                            else if (popup.ts == ".")
                                area = area_num.ToString("#.##0.00");
                            else if (popup.ts == " ")
                                area = area_num.ToString("# ##0.00");
                            else area = area_num.ToString();

                            if (options[0])
                            {
                                if (popup.cun != "" && popup.cun != null)
                                    line.SetValue(area + popup.cun + "sq", i);
                                else line.SetValue(area + docunit + "sq", i);
                            }
                            else line.SetValue(area, i);
                        }
                        else
                            line.SetValue(null, i);
                        break;
                    case "Volume":
                        string vol = null;
                        if (obj.ObjectType == ObjectType.Brep)
                            if (oref.Brep().IsSolid)
                            {
                                double vol_num = oref.Brep().GetVolume(rtol, tol);
                                vol_num = Math.Round(vol_num, popup.dp);
                                vol_num *= popup.su;
                                if (popup.ts == ",")
                                    vol = vol_num.ToString("#,##0.00");
                                else if (popup.ts == ".")
                                    vol = vol_num.ToString("#.##0.00");
                                else if (popup.ts == " ")
                                    vol = vol_num.ToString("# ##0.00");
                                else vol = vol_num.ToString();
                            }
                            else
                                vol = "open brep";
                        else if (obj.ObjectType == ObjectType.Extrusion)
                        {
                            Brep b = Brep.TryConvertBrep(obj.Geometry);
                            if (b == null) vol = "invalid extrusion";
                            else
                            {
                                if (b.IsSolid)
                                {
                                    double vol_num = b.GetVolume(rtol, tol);
                                    vol_num = Math.Round(vol_num, popup.dp);
                                    vol_num *= popup.su;
                                    if (popup.ts == ",")
                                        vol = vol_num.ToString("#,##0.00");
                                    else if (popup.ts == ".")
                                        vol = vol_num.ToString("#.##0.00");
                                    else if (popup.ts == " ")
                                        vol = vol_num.ToString("# ##0.00");
                                    else vol = vol_num.ToString();
                                }
                                else vol = "open brep";
                            }
                        }

                        if (!options[0] || vol == null) line.SetValue(vol, i);
                        else if (vol != "open brep" && vol != "invalid extrusion")
                        {
                            if (popup.cun != "" && popup.cun != null)
                                line.SetValue(vol + popup.cun + "cu", i);
                            else line.SetValue(vol + docunit + "cu", i);
                        }
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
                        var num = obj.Geometry.GetBoundingBox(false).Center.X;
                        num = Math.Round(num, popup.dp);
                        num *= popup.su;
                        line.SetValue(num.ToString(), i);
                        break;
                    case "CenterY":
                        num = obj.Geometry.GetBoundingBox(false).Center.Y;
                        num = Math.Round(num, popup.dp);
                        num *= popup.su;
                        line.SetValue(num.ToString(), i);
                        break;
                    case "CenterZ":
                        num = obj.Geometry.GetBoundingBox(false).Center.Z;
                        num = Math.Round(num, popup.dp);
                        num *= popup.su;
                        line.SetValue(num.ToString(), i);
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
                        string txt = null;
                        if (usertxts.Count == 1)
                            txt = usertxts[0];
                        else
                            txt = string.Join(";", usertxts.AllKeys);
                        line.SetValue("keys_" + txt, i);
                        break;
                    default:
                        break;
                }
            }
            return line;
        }

        /// <summary>
        /// lock or unlock objects NOT in tabl_ selected
        /// </summary>
        /// <param name="expt">exempt objects to remain locked; can be null if !unlock</param>
        /// <param name="unlock">set true to lock</param>
        private void PickFilter(RhinoObject[] expt, bool unlock = false)
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
        /// lock or unlock objects IN tabl_ selected
        /// </summary>
        /// <param name="lockselected">set true to lock already in tabl_</param>
        private void PickFilter(bool lockselected)
        {
            if (lockselected)
            {
                parent.Views.RedrawEnabled = false;
                foreach (ObjRef r in selected)
                    parent.Objects.Lock(r.ObjectId, false);
                parent.Views.RedrawEnabled = true;
            }
            else
            {
                parent.Views.RedrawEnabled = false;
                foreach (ObjRef r in selected)
                    parent.Objects.Unlock(r.ObjectId, false);
                parent.Views.RedrawEnabled = true;
            }
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
                    string val;
                    // TODO: make sure to take out unit when parsing
                    try
                    {
                        val = dataGridView1.Rows[ri].Cells[ci].Value.ToString();
                        val = val.TrimEnd("abcdefghijklmnopqrstuvwxyz".ToCharArray());
                    }
                    catch (NullReferenceException) { val = null; }
                    
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

        private void OnDockVisibleChanged(object sender, EventArgs e)
        {

        }

        private void OnDisposed(object sender, EventArgs e)
        {
            // Clear the user control property on our plug-in
            Tabl_cs.Instance.TablPanel = null;
        }

        private void OnResize(object sender, EventArgs e)
        {
            tableLayoutPanel1.Width = Width - 10;
            tableLayoutPanel1.Height = Height - 10;
        }

        // bind automatic refresh
        private void OnRhDocChange(object sender, EventArgs e)
        {
            if (popup.update)
                RefreshDGVContent("", true);
        }

        // click add
        private void button1_Click(object sender, EventArgs e)
        {
            PickFilter(true);            

            var orefs = PickObj();
            if (orefs == null)
            {
                PickFilter(false);
                return;
            }
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

            PickFilter(false);
            RefreshDGVContent("", true);
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

        // refresh meta, shared between Click and Settings.FormClosing
        private void button3_Click(object sender, EventArgs e)
        {
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

        // options check box check change
        private void checkedListBox2_CheckedChanged(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
                options.SetValue(true, e.Index);
            else options.SetValue(false, e.Index);
            RefreshDGVContent("", true);
        }

        // right click context menu within datagridview
        private void OnDGVRightClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                clickedrowindex = e.RowIndex;
                // catch click on headers row
                if (clickedrowindex == -1) return;
                contextMenuStrip1.Show(Cursor.Position);
            }
        }
        // right click and add more
        private void addMoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }
        // right click and remove row handler
        private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //first eliminate from doc string
            string raw = parent.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            idstrings.RemoveAt(clickedrowindex);
            parent.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace `selected` field
            ObjRef[] newselected = new ObjRef[idstrings.Count];
            for (int i = 0; i < idstrings.Count; i++)
                newselected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
            selected = newselected;

            RefreshDGVContent("", true);
        }
        // right click and remove highlit rows handler
        private void removeRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //first eliminate from doc string
            string raw = parent.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<int> ri = new List<int>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                ri.Add(row.Index);
            ri.Sort();
            ri.Reverse(); // these two are to guarantee no index out of range while removing
            foreach (int i in ri)
                idstrings.RemoveAt(i);
            parent.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace `selected` field
            ObjRef[] newselected = new ObjRef[idstrings.Count];
            for (int i = 0; i < idstrings.Count; i++)
                newselected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
            selected = newselected;

            RefreshDGVContent("", true);
        }

        // export csv click
        private void button6_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    for (int ri = 0; ri < dataGridView1.RowCount; ri++)
                    {
                        string[] linetxt = new string[dataGridView1.ColumnCount];
                        if (ri == 0 && options[2])
                        {
                            for (int i = 0; i < dataGridView1.ColumnCount; i++)
                                linetxt.SetValue(dataGridView1.Columns[i].Name, i);
                            sw.WriteLine(string.Join(",", linetxt));
                        }
                        var row = dataGridView1.Rows[ri];
                        for (int ci = 0; ci < dataGridView1.ColumnCount; ci++)
                            try { linetxt.SetValue(row.Cells[ci].Value.ToString(), ci); }
                            catch (NullReferenceException) { linetxt.SetValue("no-value", ci); }
                        sw.WriteLine(string.Join(",", linetxt));
                    }
                }
        }

        // click clear datagridview and doctstring
        private void button11_Click(object sender, EventArgs e)
        {
            parent.Strings.Delete("tabl_cs_selected");
            selected = new ObjRef[] { };
            RefreshDGVContent("", true);
        }

        // check all properties click
        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                // unbind and bind before last item is checked
                // to eliminate check event trigger every single time!
                if (i == 0)
                    checkedListBox1.ItemCheck -= checkedListBox1_CheckedChanged;
                if (i == checkedListBox1.Items.Count - 1)
                    checkedListBox1.ItemCheck += checkedListBox1_CheckedChanged;

                if (!checkedListBox1.GetItemChecked(i))
                    checkedListBox1.SetItemChecked(i, true);
            }
        }

        // check none properties click
        private void button8_Click(object sender, EventArgs e)
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

        // settings button click
        private void button4_Click(object sender, EventArgs e)
        {
            popup.Location = Cursor.Position;
            popup.ShowDialog(this); // showmodal doesn't dispose on close
        }

        // place this table in rhino model click
        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("you'll have to hunt down this lazy bum Will", "well...", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        // click to show about window
        private void button12_Click(object sender, EventArgs e)
        {
            AboutTabl_ about = new AboutTabl_();
            about.ShowDialog(this);
        }

        
    }
}

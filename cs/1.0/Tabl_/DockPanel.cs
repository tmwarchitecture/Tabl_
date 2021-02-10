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
using System.IO;
using System.Collections;
using Rhino.DocObjects;
using Rhino;
using Rhino.Input;
using Rhino.Geometry;
using Rhino.Commands;

namespace Tabl_cs
{
    [Guid("FA291E46-C13B-47A5-9B59-46C578A74EA3")]
    public partial class DockPanel : UserControl
    {
        #region overhead for redraw disable
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 11;
        #endregion

        private RhinoDoc parent;
        private ObjRef[] selected;

        private double tol;
        private double rtol;
        private double atol_deg;
        private double atol_rad;
        private string docunit;
        // [0] show units; [1] show total; [2] export headers
        private bool[] options = new bool[] { false, false, false };

        private bool mod = false; // see OnAttrMod
        private int clickedrowindex; // see event handler OnRowHeaderRightClick

        private Settings popup = new Settings();
        private WaitScreen waitform = new WaitScreen() { TopMost = true };
        private PlaceSettings placepopup = new PlaceSettings();
        private TextboxDialog txtinput = new TextboxDialog();

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
            //VisibleChanged += OnDockVisibleChanged;
            Disposed += OnDisposed;

            popup.FormClosing += button3_Click;
            placepopup.FormClosing += OnPlaceClose;
            Command.EndCommand += OnRhDocChange;
            RhinoApp.Idle += OnIdlePostMod; // handle idle right after all attribute mod finish
            RhinoDoc.ModifyObjectAttributes += OnAttrMod; // first mod trigger
            
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
                dataGridView1.Columns[i].Name = checkedListBox1.CheckedItems[i].ToString();
            dataGridView1.CellMouseClick += OnDGVRightClick;
            dataGridView1.SortCompare += OnSort;
        }

        public static Guid PanelId
        {
            get { return typeof(DockPanel).GUID; }
        }


        #region user methods

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
            
            SendMessage(dataGridView1.Handle, WM_SETREDRAW, false, 0); // disable redraw with windows api
            for (int i = 0; i <= meta.GetUpperBound(0); i++)
            {
                string[] row = new string[meta.GetUpperBound(1) + 1];
                for (int j = 0; j <= meta.GetUpperBound(1); j++)
                    row.SetValue(meta[i, j], j);

                
                dataGridView1.Rows.Add(row);
                // enumerate row # and set in row header
                int last = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.None);
                dataGridView1.Rows[last].HeaderCell.Value = (i+1).ToString();

            }

            SendMessage(dataGridView1.Handle, WM_SETREDRAW, true, 0); // enable redraw with windows api
            dataGridView1.Refresh();

            if (options[1])
            {
                if (dataGridView1.RowCount > 100)
                {
                    waitform.Location = Cursor.Position;
                    waitform.Show();
                }
                DBVTotal();
            }
            

        }
        /// <summary>
        /// simple refresh
        /// </summary>
        internal void RefreshDGVContent()
        {
            RefreshDGVContent("", true);
        }
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
        /// reload tabl_cs_selected into obj refs
        /// </summary>
        /// <param name="idstrings">each id strings split from tabl_cs_selected</param>
        /// <returns>true on success</returns>
        private bool ReloadRefs(List<string> idstrings)
        {
            ObjRef[] newselected = new ObjRef[idstrings.Count];
            for (int i = 0; i < idstrings.Count; i++)
                newselected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
            selected = newselected;
            return true;
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
            // [row index, col index]
            Parallel.For(0, orefs.Length, i =>
            {
                ObjRef oref = orefs[i];
                string[] props = GetProp(oref, propkeys);
                object locker = new object();
                for (int j = 0; j < props.Length; j++)
                    lock (locker) { matrix.SetValue(props[j], i, j); }
            });

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
            if (obj == null)
            {
                try { line.SetValue("MISSING! Please refresh table", 0); }
                catch (IndexOutOfRangeException) { }
                RemoveById(oref.ObjectId.ToString());
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
                            line.SetValue(string.Format("{0},{1},{2}", c.R, c.G, c.B), i);
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
                                len = KMarker(len_num, ',');
                            else if (popup.ts == ".")
                                len = KMarker(len_num,'.');
                            else if (popup.ts == " ")
                                len = KMarker(len_num, ' ');
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
                                area = KMarker(area_num, ',');
                            else if (popup.ts == ".")
                                area = KMarker(area_num,'.');
                            else if (popup.ts == " ")
                                area = KMarker(area_num, ' ');
                            else area = area_num.ToString();

                            if (options[0])
                            {
                                if (popup.cun != "" && popup.cun != null)
                                    line.SetValue(area + popup.cun + "\xB2", i);
                                else line.SetValue(area + docunit + "\xB2", i);
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
                                    vol = KMarker(vol_num, ',');
                                else if (popup.ts == ".")
                                    vol = KMarker(vol_num, '.');
                                else if (popup.ts == " ")
                                    vol = KMarker(vol_num, ' ');
                                else vol = vol_num.ToString();
                            }
                            else
                                vol = "0";
                        else if (obj.ObjectType == ObjectType.Extrusion)
                        {
                            Brep b = Brep.TryConvertBrep(obj.Geometry);
                            if (b == null) vol = "0";
                            else
                            {
                                if (b.IsSolid)
                                {
                                    double vol_num = b.GetVolume(rtol, tol);
                                    vol_num = Math.Round(vol_num, popup.dp);
                                    vol_num *= popup.su;
                                    if (popup.ts == ",")
                                        vol = KMarker(vol_num, ',');
                                    else if (popup.ts == ".")
                                        vol = KMarker(vol_num, '.');
                                    else if (popup.ts == " ")
                                        vol = KMarker(vol_num, ' ');
                                    else vol = vol_num.ToString();
                                }
                                else vol = "0";
                            }
                        }

                        if (!options[0] || vol == null) line.SetValue(vol, i);
                        else if (vol != "open brep" && vol != "invalid extrusion")
                        {
                            if (popup.cun != "" && popup.cun != null)
                                line.SetValue(vol + popup.cun + "\xB3", i);
                            else line.SetValue(vol + docunit + "\xB3", i);
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
        /// remove single object by id from tabl 
        /// DO NOT use in a loop
        /// </summary>
        /// <param name="guid">object guid</param>
        /// <returns>true if success</returns>
        private bool RemoveById (string guid)
        {
            //first eliminate from doc string
            string raw = parent.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            idstrings.Remove(guid);
            parent.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace `selected` field
            ReloadRefs(idstrings);

            return true;
        }

        /// <summary>
        /// add total at bottom of datagridview
        /// </summary>
        private async void DBVTotal()
        {
            string[] totalrow = new string[dataGridView1.ColumnCount];
            string[] txts = new string[] { "GUID", "Type", "Name", "Layer", "Color", "LineType", "PrintColor", "Material", "IsPlanar", "IsClosed", "Comments", };
            await Task.Run(() =>
            {
                object locker = new object();

                Parallel.For(0, dataGridView1.ColumnCount, ci =>
                {
                    double colsum = 0.0;

                    for (int ri = 0; ri < dataGridView1.RowCount; ri++)
                    {
                        if (txts.Contains(dataGridView1.Columns[ci].Name)) break; // skip non-numeric columns
                        string val;
                        try
                        {
                            val = dataGridView1.Rows[ri].Cells[ci].Value.ToString();
                            // remove superscript and then unit mark
                            val = val.TrimEnd(new char[] { '\xB2', '\xB3', });
                            val = val.TrimEnd("abcdefghijklmnopqrstuvwxyz".ToCharArray());
                        }
                        catch (NullReferenceException) { val = null; }

                        if (double.TryParse(val, out double num))
                            colsum += num;
                    }
                    lock (locker)
                        totalrow.SetValue(colsum.ToString(), ci);
                });
                /*
                for (int ci = 0; ci < dataGridView1.ColumnCount; ci++)
                {
                    double colsum = 0.0;
                    
                    for (int ri = 0; ri < dataGridView1.RowCount; ri++)
                    {
                        if (txts.Contains(dataGridView1.Columns[ci].Name)) break; // skip non-numeric columns
                        string val;
                        try
                        {
                            val = dataGridView1.Rows[ri].Cells[ci].Value.ToString();
                            // remove superscript and then unit mark
                            val = val.TrimEnd(new char[] { '\xB2', '\xB3', });
                            val = val.TrimEnd("abcdefghijklmnopqrstuvwxyz".ToCharArray());
                        }
                        catch (NullReferenceException) { val = null; }

                        if (double.TryParse(val, out double num))
                            colsum += num;
                    }

                    totalrow.SetValue(colsum.ToString(), ci);
                }*/

                if (waitform.InvokeRequired) // cross thread condition
                    waitform.Invoke((Action)delegate { waitform.Hide(); });
                else waitform.Hide();
            });
            
            dataGridView1.Rows.Add(totalrow);
            int last = dataGridView1.Rows.GetLastRow(DataGridViewElementStates.None);
            dataGridView1.Rows[last].HeaderCell.Value = "SUM";
        }

        /// <summary>
        /// custom format of thousand separators
        /// </summary>
        /// <param name="num">the number to format</param>
        /// <param name="marker">separator marker</param>
        /// <returns>string with proper separator inserted</returns>
        private string KMarker(double num, char marker)
        {
            string nums = num.ToString();
            string[] parts = nums.Split(new char[] { '.', });
            string whole = parts[0];
            // int/int is an integral devision, same as "//" in python
            int ng = whole.Length % 3 == 0 ? whole.Length / 3 : whole.Length / 3 + 1;
            string[] groups = new string[ng];
            for (int i = 0; i < ng; i++)
            {
                if (i != ng - 1)
                {
                    groups.SetValue(whole.Substring(whole.Length - 3), i);
                    whole = whole.Remove(whole.Length - 3);
                }
                else groups.SetValue(whole, i);
            }
            nums = string.Join(marker.ToString(), groups.Reverse());
            nums += "." + parts[1];
            return nums;
        }
        private string KMarker(int num, char marker)
        {
            return KMarker((double)num, marker);
        }
        private string KMarker(decimal num, char marker)
        {
            return KMarker((double)num, marker);
        }

        /// <summary>
        /// plop a table in RhinoDoc
        /// </summary>
        /// <param name="anchor">mouse clicked location, top left of table</param>
        private void TableInDoc(Point3d anchor)
        {
            // calculate how to come back to clicked location
            Point3d trueanchor = new Point3d(anchor);
            Point3d mappedanchor = new Point3d(anchor);
            Transform remap = Transform.PlaneToPlane(Plane.WorldXY, placepopup.BasePlane);
            mappedanchor.Transform(remap);
            Transform comeback = Transform.Translation(new Vector3d(trueanchor - mappedanchor));

            // load matrix
            List<TextEntity[]> tablcontent = new List<TextEntity[]>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                TextEntity[] line = new TextEntity[row.Cells.Count];
                for (int i = 0; i < line.Length; i++)
                {
                    string txt;
                    try { txt = row.Cells[i].Value.ToString(); }
                    catch (NullReferenceException) { txt = "no_value"; }
                    TextEntity txtobj = new TextEntity()
                    {
                        PlainText = txt,
                        Justification = TextJustification.Center,
                        TextHeight = placepopup.fs,
                        Font = new Rhino.DocObjects.Font(placepopup.fn),
                        Plane = Plane.WorldXY, // overridden later in text placement block
                    };
                    line.SetValue(txtobj, i);
                }
                tablcontent.Add(line);
            }
            // column widths
            double[] cws = new double[tablcontent[0].Length];
            if (placepopup.fitting == 3)
                for (int n = 0; n < cws.Length; n++) cws.SetValue(placepopup.cw, n);
            else
            {
                for (int n = 0; n < cws.Length; n++) cws.SetValue(0, n);
                for (int ri = 0; ri < tablcontent.Count; ri++)
                    for (int ci = 0; ci < tablcontent[ri].Length; ci++)
                        cws.SetValue(
                            tablcontent[ri][ci].TextModelWidth > cws[ci] ?
                            tablcontent[ri][ci].TextModelWidth + (double)placepopup.cellpad * 2 :
                            cws[ci] + (double)placepopup.cellpad * 2,
                            ci);
            }

            // furnish collection to be placed in doc later
            List<GeometryBase> tablobjs = new List<GeometryBase>(); //collect what's added
            Line h0 = new Line(anchor, anchor + new Point3d(cws.Sum(), 0, 0)); // first horizontal
            double vdim = tablcontent.Count * (placepopup.fs + (double)placepopup.cellpad * 2);
            Line v0 = new Line(anchor, anchor + new Point3d(0, -vdim, 0)); // first vertical
            tablobjs.AddRange(new GeometryBase[] { h0.ToNurbsCurve(), v0.ToNurbsCurve() });
            // all horizontals
            for (int i=0;i<tablcontent.Count;i++)
            {
                Point3d dy = new Point3d(0, -(i + 1) * (placepopup.fs + (double)placepopup.cellpad * 2), 0);
                Point3d start = anchor + dy;
                Point3d end = anchor + new Point3d(cws.Sum(), 0, 0) +dy;
                Line hl = new Line(start, end);
                tablobjs.Add(hl.ToNurbsCurve());
            }
            // all verticals
            double xnow = 0;
            for (int i = 0; i < cws.Length; i++)
            {
                xnow += cws[i];
                Point3d dx = new Point3d(xnow, 0, 0);
                Point3d start = anchor + dx;
                Point3d end = anchor + new Point3d(0, -vdim, 0) + dx;
                Line vl = new Line(start, end);
                tablobjs.Add(vl.ToNurbsCurve());
            }
            // texts placments
            for (int ri=0; ri<tablcontent.Count; ri++)
            {
                double rowY = ri * ((double)placepopup.cellpad * 2 + placepopup.fs) + (double)placepopup.cellpad;
                TextEntity[] ts = tablcontent[ri];
                double txtX = 0;
                for (int ci=0; ci<ts.Length; ci++)
                {
                    TextEntity te = ts[ci];
                    if (ci == 0) txtX = cws[0] / 2;
                    else txtX += cws[ci - 1] / 2 + cws[ci] / 2;
                    te.Plane = new Plane(anchor + new Point3d(txtX, -rowY, 0), Plane.WorldXY.ZAxis);
                    tablobjs.Add(te); // TextEntity inherits from GeometryBase
                }
            }

            // tranform and place
            Parallel.For(0, tablobjs.Count, i =>
            {
                tablobjs[i].Transform(remap);
                tablobjs[i].Transform(comeback);
            });
            foreach (GeometryBase g in tablobjs) parent.Objects.Add(g); // add to doc
        }

        #endregion

        // first mod event handled
        private void OnAttrMod(object sender, RhinoModifyObjectAttributesEventArgs e)
        {
            mod = true;
            RhinoDoc.ModifyObjectAttributes -= OnAttrMod;
            // unbind so subsequent events do not trigger until idle
        }

        // handle idle after all attr mod
        private void OnIdlePostMod(object sender, EventArgs e)
        {
            if (mod)
            {
                RefreshDGVContent();
                mod = false;
                RhinoDoc.ModifyObjectAttributes += OnAttrMod;
            }
        }

        private void OnDockVisibleChanged(object sender, EventArgs e)
        {

        }

        private void OnDisposed(object sender, EventArgs e)
        {
            // Clear the user control property on our plug-in
            Tabl_cs.Instance.TablPanel = null;
        }

        // bind automatic refresh
        private void OnRhDocChange(object sender, EventArgs e)
        {
            
            if (popup.update)
                RefreshDGVContent();
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
            RefreshDGVContent();
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
            ReloadRefs(idstrings);

            PickFilter(userlocked.ToArray(), true);
            RefreshDGVContent();
        }

        // refresh meta, shared between Click and Settings.FormClosing
        private void button3_Click(object sender, EventArgs e)
        {
            RefreshDGVContent();
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
            RefreshDGVContent();
        }

        #region datagridview context menu handlers
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
            ReloadRefs(idstrings);

            RefreshDGVContent();
        }
        // right click and remove highlit rows handler
        private void removeRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //first eliminate from doc string
            List<int> ri = new List<int>();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                ri.Add(row.Index);
            if (ri.Count==0)
            {
                MessageBox.Show("no row highlighted");
                return;
            }
            string raw = parent.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ri.Sort();
            ri.Reverse(); // these two are to guarantee no index out of range while removing
            foreach (int i in ri)
                idstrings.RemoveAt(i);
            parent.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace `selected` field
            ReloadRefs(idstrings);
            
            RefreshDGVContent();
        }
        // select highlighted rows in model
        private void selectHighlightedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                MessageBox.Show("no row highlighted");
            else
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    selected[row.Index].Object().Select(true);
            parent.Views.Redraw();
        }
        // recolor objects
        private void recolorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                List<int> ri = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    ri.Add(row.Index);
                if (ri.Count == 0) ri.Add(clickedrowindex);
                foreach (int i in ri)
                {
                    RhinoObject o = selected[i].Object();
                    o.Attributes.ColorSource = ObjectColorSource.ColorFromObject;
                    o.Attributes.ObjectColor = colorDialog1.Color;
                    if (!o.CommitChanges()) MessageBox.Show("unknown internal error");
                }
                parent.Views.Redraw();
            }
        }
        // rename objects
        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtinput.Location = Cursor.Position;
            txtinput.ShowDialog();
            if (txtinput.commited)
            {
                List<int> ri = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    ri.Add(row.Index);
                if (ri.Count == 0) ri.Add(clickedrowindex);
                foreach (int i in ri)
                {
                    RhinoObject o = selected[i].Object();
                    o.Attributes.Name = txtinput.txtval;
                    if (!o.CommitChanges()) MessageBox.Show("unknown internal error");
                }
                txtinput.commited = false;
            }
        }
        // change layers
        private void changeLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int li = -1;
            bool current = false;
            bool r = Rhino.UI.Dialogs.ShowSelectLayerDialog(ref li, "Select layer", false, false, ref current);
            if (r && li != -1)
            {
                List<int> ri = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    ri.Add(row.Index);
                if (ri.Count == 0) ri.Add(clickedrowindex);
                foreach (int i in ri)
                {
                    RhinoObject o = selected[i].Object();
                    o.Attributes.LayerIndex = li;
                    if (!o.CommitChanges()) MessageBox.Show("unknown internal error");
                }
                parent.Views.Redraw();
            }
        }
        #endregion

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
                            try
                            {
                                string v = row.Cells[ci].Value.ToString();
                                string properv = v.Contains(",") ? "\"" + v + "\"" : v;
                                linetxt.SetValue(properv, ci);
                            }
                            catch (NullReferenceException) { linetxt.SetValue("no-value", ci); }
                        sw.WriteLine(string.Join(",", linetxt));
                    }
                }
        }

        // click clear datagridview and doctstring
        private void button11_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to clear all?","Confirm", MessageBoxButtons.YesNo)== DialogResult.Yes)
            {
                parent.Strings.Delete("tabl_cs_selected");
                selected = new ObjRef[] { };
                RefreshDGVContent();
            }
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
            placepopup.Location = Cursor.Position;
            placepopup.Show(this);
            tableLayoutPanel1.Enabled = false;
        }
        private void OnPlaceClose(object sender, FormClosingEventArgs e)
        {
            Form s = sender as Form;
            s.Hide();
            e.Cancel = true;
            tableLayoutPanel1.Enabled = true;
            if (placepopup.ok)
            {
                placepopup.ok = false;
                Result r = RhinoGet.GetPoint("pick table top left corner", true, out Point3d anchor);
                if (r == Result.Success) TableInDoc(anchor);
            }
        }

        // click to show about window
        private void button12_Click(object sender, EventArgs e)
        {
            AboutTabl_ about = new AboutTabl_();
            about.ShowDialog(this);
        }

        // click to export GUID
        private void button10_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    for (int ri = 0; ri < dataGridView1.RowCount; ri++)
                    {
                        ObjRef oref = selected[ri];
                        sw.WriteLine(oref.ObjectId.ToString());
                    }
                }
        }

        // click to import GUID
        private void button9_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string[] guids; string[] idstrings; string raw;
                using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
                    guids = sr.ReadToEnd().TrimEnd('\r','\n').Split(new char[] { '\n', });
                try { raw = parent.Strings.GetValue("tabl_cs_selected"); }
                catch (NullReferenceException) { raw = null; }

                List<string> errors = new List<string>();
                List<string> validstrs = new List<string>();
                ObjRef[] added;
                if (raw!=null) // condition where tabl_cs_selected already in
                {
                    idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries);
                    List<string> newids = new List<string>(idstrings);
                    foreach (string guid in guids)
                    {
                        string clean = guid.TrimEnd('\r', '\n');
                        if (!idstrings.Contains(clean))
                            newids.Add(clean);
                    }
                        

                    added = new ObjRef[newids.Count];
                    for (int i = 0; i < newids.Count; i++)
                    {
                        ObjRef oref;
                        try
                        {
                            oref = new ObjRef(new Guid(newids[i]));
                            added.SetValue(oref, i);
                            validstrs.Add(newids[i]);
                        }
                        catch (Exception exc)
                        {
                            errors.Add(exc.Message);
                        }
                    }
                    
                }
                else // import guid into blank doc
                {
                    added = new ObjRef[guids.Length];
                    for (int i = 0; i < guids.Length; i++)
                    {
                        ObjRef oref;
                        string guid = guids[i].TrimEnd('\r', '\n');
                        try
                        {
                            oref = new ObjRef(new Guid(guid));
                            added.SetValue(oref, i);
                            validstrs.Add(guid);
                        }
                        catch (Exception exc)
                        {
                            errors.Add(exc.Message);
                        }
                    }
                }
                if (errors.Count != 0) MessageBox.Show("guid import error detected\ncheck source");
                parent.Strings.SetString("tabl_cs_selected", string.Join(",", validstrs));
                selected = added;
                RefreshDGVContent();
            }
        }

        // custom sort logic as compare event handler
        private void OnSort(object sender, DataGridViewSortCompareEventArgs e)
        {
            string v1 = e.CellValue1 as string;
            string v2 = e.CellValue2 as string;

            if (string.IsNullOrEmpty(v1) && string.IsNullOrEmpty(v2))
                e.SortResult = 0;
            else if (string.IsNullOrEmpty(v1) && !string.IsNullOrEmpty(v2))
                e.SortResult = -1;
            else if (!string.IsNullOrEmpty(v1) && string.IsNullOrEmpty(v2))
                e.SortResult = 1;
            else
            {
                if (double.TryParse(v1, out double n1) && double.TryParse(v2, out double n2))
                    e.SortResult = n1 >= n2 ? 1 : -1;
                else
                    e.SortResult = string.Compare(v1, v2);
            }
            e.Handled = true;
        }

        
    }
}

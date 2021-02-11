using Rhino;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Display;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Tabl_
{
    internal class Highlighter : DisplayConduit
    {
        /* groups of curves, each group is from an object
         largely unnecessary to keep this indexing, flat list should work
         this was put in place for possibly deleting markers by index
             */
        public List<Curve[]> crvs;

        public List<Point3d> pts;
        public List<AnnotationBase> annos;
        public List<TextDot> dottxts;
        public List<Box> blockbbs; // bounding boxes for blocks

        public Color clr;
        public int w; // wire line width in pixels

        /// <summary>
        /// constructor for highlight marker in rhino model, of selected line item of Tabl_
        /// </summary>
        public Highlighter()
        {
            crvs = new List<Curve[]>();
            pts = new List<Point3d>();
            annos = new List<AnnotationBase>();
            dottxts = new List<TextDot>();
            blockbbs = new List<Box>();
            clr = Color.HotPink;
            w = 3;
        }

        /// <summary>
        /// empty preview geometries
        /// </summary>
        public void Empty()
        {
            for (int i = 0; i < crvs.Count; i++)
                crvs[i] = new Curve[] { };
            pts.Clear();
            annos.Clear();
            dottxts.Clear();
            blockbbs.Clear();
        }

        /// <summary>
        /// add geometries selected in listview to mark in rhino doc
        /// </summary>
        /// <param name="lv">ListView where items are pulled</param>
        /// <param name="loaded">objrefs corresponding to the provided listview items</param>
        public void AddMarkers(ListView lv, ObjRef[] loaded)
        {
            if (!Enabled) return;
            foreach (int hli in lv.SelectedIndices)
            {
                if (hli == lv.Items.Count - 1)
                {
                    // test if this last line item is the totals
                    TablLineItem lastitem = lv.Items[hli] as TablLineItem;
                    if (lastitem.RefId == Guid.Empty || lastitem.RefId == null) break;
                }
                AddMarker(hli, loaded);
            }
        }
        /// <summary>
        /// add specific geometries from loaded ObjRef[] to preview markers. you should check if listview items and loaded match order and capacity
        /// </summary>
        /// <param name="idx">index in the loaded collection</param>
        /// <param name="loaded">loaded collection</param>
        public void AddMarker(int idx, ObjRef[] loaded)
        {
            if (!Enabled) return;
            GeometryBase g = loaded[idx].Geometry();
            if (g.HasBrepForm)
            {
                Brep brep = Brep.TryConvertBrep(g);
                BrepEdge[] group = brep.Edges.ToArray();
                crvs[idx] = group.Select(edge => edge.ToNurbsCurve()).ToArray();
            }
            else if (g is Curve c)
                crvs[idx] = new Curve[] { c, };
            else if (g is Mesh m)
            {
                Curve[] edges = new Curve[m.TopologyEdges.Count];
                for (int ei = 0; ei < edges.Length; ei++)
                    edges.SetValue(m.TopologyEdges.EdgeLine(ei).ToNurbsCurve(), ei);
                crvs[idx] = edges;
            }
            else if (g is TextDot txtdot)
                dottxts.Add(txtdot);
            else if (g is AnnotationBase anno)
                annos.Add(anno);
            else if (g is InstanceReferenceGeometry block)
                blockbbs.Add(new Box(block.GetBoundingBox(false)));
            else if (g is Rhino.Geometry.Point pt)
                pts.Add(pt.Location);
            else
            {
                // anything else to preview???
            }
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            if (crvs == null)
            {
                base.CalculateBoundingBox(e);
                return;
            }
            foreach (Curve[] cg in crvs)
                foreach (Curve c in cg)
                    e.IncludeBoundingBox(c.GetBoundingBox(false));
        }

        protected override void PostDrawObjects(DrawEventArgs e)
        {
            if (crvs == null)
            {
                base.PostDrawObjects(e);
                return;
            }
            foreach (Curve[] cg in crvs)
                foreach (Curve c in cg)
                    e.Display.DrawCurve(c, clr, w);
        }

        protected override void DrawOverlay(DrawEventArgs e)
        {
            //base.DrawOverlay(e);
            foreach (TextDot td in dottxts)
                e.Display.DrawDot(td, clr, Color.DarkSlateGray, Color.DarkSlateGray);
            foreach (AnnotationBase anno in annos)
                e.Display.DrawAnnotation(anno, clr);
            e.Display.DrawPoints(pts, PointStyle.X, w*2, clr);
            foreach (Box b in blockbbs)
                e.Display.DrawBox(b, clr, w+w/2);
        }
    }

    internal class RectPreview: DisplayConduit
    {
        public Rectangle3d rec;
        public Color clr;

        /// <summary>
        /// construct display conduit for preview rectangle during placement of tabl in rhino doc
        /// </summary>
        public RectPreview()
        {
            clr = Color.HotPink;
        }
        /// <summary>
        /// construct display conduit for preview rectangle during placement of tabl in rhino doc
        /// </summary>
        /// <param name="c">color of preview</param>
        public RectPreview(Color c)
        {
            clr = c;
        }

        protected override void CalculateBoundingBox(CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(rec.BoundingBox);
        }
        protected override void DrawOverlay(DrawEventArgs e)
        {
            e.Display.DrawDottedPolyline(new Point3d[] {
                rec.Corner(0),
                rec.Corner(1),
                rec.Corner(2),
                rec.Corner(3),
            }, clr, true);
        }
    }

    internal class TablLineItem : ListViewItem
    {
        /// <summary>
        /// guid belonging to the rhino object this tabl line corresponds
        /// </summary>
        public Guid RefId { get; set; }
        
        /// <summary>
        /// constructor, this doesn't assign RefId. set property separately
        /// </summary>
        /// <param name="items">collection of cells on the row</param>
        public TablLineItem(string[] items) : base(items)
        {
            
        }
    }

    public partial class TablDockPanel : UserControl
    {
        /// <summary>
        /// get the length unit to show in spreadsheet
        /// </summary>
        /// <returns>unit string</returns>
        private string LenUnit()
        {
            string docunit;
            if (settings.cun != "" && settings.cun != null)
                docunit = settings.cun;
            else
                docunit = ParentDoc.GetUnitSystemName(true, false, true, true);
            return docunit;
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
        /// <summary>
        /// transform string back to parse friendly digits by taking out thousand marker or unit suffix
        /// </summary>
        /// <param name="withunit">string with units or markers</param>
        /// <returns>parse ready digits as string</returns>
        internal static string DeUnit(string withunit)
        {
            return string.Join("", withunit.ToList().FindAll(s => char.IsDigit(s) || s == '.'));
        }

        /// <summary>
        /// initialize listview tabl's menu strip
        /// </summary>
        private void InitializeLVMS()
        {
            msHeaders.Closing += HeaderStripClosing;
            msHeaders.Opening += HeaderStripOpening;
            foreach (ToolStripMenuItem i in msHeaders.Items)
                i.CheckOnClick = true;
        }
        /// <summary>
        /// know which headers to show from the menustrip
        /// </summary>
        private void SetHeaderVis()
        {
            foreach (ToolStripMenuItem i in msHeaders.Items)
            {
                if (i.Text == "#")
                {
                    linecounter = i.Checked;
                }
                else if (i.Checked)
                    headers[i.Text] = true;
                else
                    headers[i.Text] = false;
            }
        }
        /// <summary>
        /// know which headers should be checked once menustrip is shown
        /// </summary>
        private void MirrorHeaders()
        {
            foreach (ColumnHeader h in lvTabl.Columns)
            {
                if (h.Text == "#")
                {
                    ToolStripMenuItem hashtag = msHeaders.Items[0] as ToolStripMenuItem;
                    hashtag.Checked = linecounter;
                    continue;
                }
                foreach (ToolStripMenuItem i in msHeaders.Items)
                    if (h.Text == i.Text)
                    {
                        i.Checked = true;
                        break;
                    }
            }
        }

        /// <summary>
        /// launch rhino get object command
        /// </summary>
        /// <param name="parent">parent form that started the command</param>
        /// <param name="ot">selection object type</param>
        /// <returns>array of ObjRef</returns>
        private ObjRef[] PickObj(ObjectType ot = ObjectType.AnyObject)
        {
            Enabled = false;

            Result r = RhinoGet.GetMultipleObjects(" select object(s)", true, ot, out ObjRef[] picked);
            if (r == Result.Success)
            {
                Enabled = true;
                return picked;
            }
            else
            {
                RhinoApp.WriteLine(" error in selection");
                Enabled = true;
                return new ObjRef[] { };
            }
        }

        /// <summary>
        /// lock or unlock objects within Tabl_ loaded, should be called in pairs
        /// </summary>
        /// <param name="l">set true to lock already in tabl_</param>
        private void AddPickFilter(bool l)
        {
            ParentDoc.Views.RedrawEnabled = false;

            if (l)
                foreach (ObjRef r in Loaded)
                    ParentDoc.Objects.Lock(r.ObjectId, false);
            else
                foreach (ObjRef r in Loaded)
                    ParentDoc.Objects.Unlock(r.ObjectId, false);
            
            ParentDoc.Views.RedrawEnabled = true;
        }
        /// <summary>
        /// lock or unlock objects outside the Tabl_ loaded objects, should be called in pairs
        /// </summary>
        /// <param name="expt">exempt objects, lock status won't alter</param>
        /// <param name="l">set true to lock (enter picking)</param>
        private void RmvPickFilter(RhinoObject[] expt, bool l = true)
        {
            Guid[] loadedids = Loaded.Select(i => i.ObjectId).ToArray();
            Guid[] exptids = expt.Select(i => i.Id).ToArray();
            ParentDoc.Views.RedrawEnabled = false;

            if (!l)
                foreach (RhinoObject robj in ParentDoc.Objects)
                {
                    if (exptids.Contains(robj.Id)) continue;
                    else ParentDoc.Objects.Unlock(robj.Id, false);
                }
            else
                foreach (RhinoObject robj in ParentDoc.Objects)
                {
                    if (!loadedids.Contains(robj.Id))
                        ParentDoc.Objects.Lock(robj.Id, false);
                    else continue;
                }
            ParentDoc.Views.RedrawEnabled = true;
        }

        /// <summary>
        /// reload ObjRef[] by id strings
        /// </summary>
        /// <param name="idstrings">each GUID strings</param>
        private void ReloadRefs(IEnumerable<string> idstrings)
        {
            string[] strs = idstrings.ToArray();
            ObjRef[] docstrrecord = new ObjRef[strs.Length];
            for (int i = 0; i < strs.Length; i++)
                docstrrecord.SetValue(new ObjRef(new Guid(strs[i])), i);
            Loaded = docstrrecord;
        }
        /// <summary>
        /// reload record on docstr into loaded ObjRef[], docstr key tabl_cs_selected
        /// </summary>
        private void ReloadRefs()
        {
            try
            {
                string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
                List<string> idstrs = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
                ReloadRefs(idstrs);
            }
            catch (ArgumentNullException) { }
            catch (NullReferenceException) { }
        }

        /// <summary>
        /// push guid strings to to docstr, key tabl_cs_selected
        /// </summary>
        /// <param name="idstrs">guid strings to push</param>
        private void PushRefs(IEnumerable<string> idstrs)
        {
            string[] strs = idstrs.ToArray();
            string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
            List<string> docstrids;
            if (raw == null)
                docstrids = new List<string>();
            else
                docstrids = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string id in idstrs)
                if (!docstrids.Contains(id))
                    docstrids.Add(id);
            ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", docstrids.ToArray()));
        }
        /// <summary>
        /// push loaded ObjRef[] into docstr record, key tabl_cs_selected
        /// </summary>
        internal void PushRefs()
        {
            IEnumerable<string> idstrs = Loaded.Select(o => o.ObjectId.ToString());
            PushRefs(idstrs);
        }

        /// <summary>
        /// refresh spreadsheet
        /// </summary>
        /// <param name="th">true if threaded computing</param>
        private void RefreshTabl()
        {
            lvTabl.Clear();

            // set up headers
            string[] numheaders = new string[] { "NumPts", "Area", "Volume", "NumEdges", "NumFaces", "Length", "PrintWidth", "Degree", };
            if (!headers.Values.Contains(true))
            {
                lvCtxtMenu.Close();
                MessageBox.Show("No property column is checked\nRight click spreadsheet area and select columns to show", "Nothing to see", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            List<string> keys = headers.Keys.ToList();
            keys.Sort(HeaderSorter); // this guarantees header order matching with menustrip's
            if (linecounter)
                lvTabl.Columns.Add(new ColumnHeader() { Text = "#", });
            foreach (string k in keys)
                if (headers[k])
                {
                    ColumnHeader ch = new ColumnHeader()
                    {
                        Text = k,
                        TextAlign = numheaders.Contains(k)? HorizontalAlignment.Right: HorizontalAlignment.Center,
                    };
                    lvTabl.Columns.Add(ch);
                }

            // fill spreadsheet
            List<TablLineItem> lines = new List<TablLineItem>();
            List<int> badidx = new List<int>(); // missing in document but objref still references
            for (int oi = 0; oi < Loaded.Length; oi++)
            {
                string[] infos = TablLineFields(oi);
                if (infos.Length == 0)
                    break;
                else if (infos[0] == "MISSING! DELETE!")
                {
                    badidx.Add(oi);
                    continue;
                }
                else if (!linecounter)
                    lines.Add(new TablLineItem(infos) { RefId = Loaded[oi].ObjectId,});
                else
                {
                    List<string> infolist = new List<string> { (oi + 1).ToString(), };
                    infolist.AddRange(infos);
                    lines.Add(new TablLineItem(infolist.ToArray()) { RefId = Loaded[oi].ObjectId,});
                }
            }
            lvTabl.Items.AddRange(lines.ToArray()); // faster with addrange rather than add in a loop
            TablColAutoSize();

            // remove if something in doc was deleted by user
            if (badidx.Count != 0)
            {
                List<string> todelete = new List<string>();
                foreach (int idx in badidx)
                    todelete.Add(Loaded[idx].ObjectId.ToString());
                RemoveByIds(todelete.ToArray());
            }

            /* prep for click-n-mark
             If object type isn't one that produces wireframes like annotations,
             there will be an empty array left in place.
             This index-preservation seems largely unncessary with new click-n-mark mechanism
             */
            settings.docmarker.crvs = new List<Curve[]>(lvTabl.Items.Count);
            for (int n = 0; n < lvTabl.Items.Count; n++)
                settings.docmarker.crvs.Add(new Curve[] { });
            
            // totals
            // whenever iterating through lvTabl.items, beware of last item!
            if (settings.ssopt[1])
                lvTabl.Items.Add(TablTotals());
            
            ParentDoc.Views.Redraw();
        }
        /// <summary>
        /// refresh just an item in Tabl by objref, no null checking in this method
        /// </summary>
        /// <param name="oref">object reference</param>
        private void RefreshTabl(ObjRef oref)
        {
            int tliidx = -1;
            for (int i=0; i< lvTabl.Items.Count; i++)
            {
                if (settings.ssopt[1] && i == lvTabl.Items.Count - 1)
                    break;
                TablLineItem li = lvTabl.Items[i] as TablLineItem;
                if (li.RefId == oref.ObjectId)
                {
                    tliidx = i;
                    break;
                }
            }
            string[] infos = TablLineFields(tliidx);
            if (!linecounter)
                lvTabl.Items[tliidx] = new TablLineItem(infos) {
                    RefId = Loaded[tliidx].ObjectId,
                };
            else
            {
                List<string> infolist = new List<string> { (tliidx + 1).ToString(), };
                infolist.AddRange(infos);
                lvTabl.Items[tliidx] = new TablLineItem(infolist.ToArray()) {
                    RefId = Loaded[tliidx].ObjectId,
                };
            }
        }

        /// <summary>
        /// autosize column width of tabl
        /// </summary>
        private void TablColAutoSize()
        {
            if (lvTabl.Items.Count > 0)
                lvTabl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            else
                lvTabl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        /// <summary>
        /// get the line item fields for a tabl line
        /// </summary>
        /// <param name="refi">index of objref in loaded</param>
        /// <returns>array of the obj properties</returns>
        private string[] TablLineFields(int refi)
        {
            string[] infos;
            if (linecounter)
                infos = new string[lvTabl.Columns.Count - 1];
            else
                infos = new string[lvTabl.Columns.Count];

            for (int ci = 0; ci < infos.Length; ci++)
            {
                string field;
                if (linecounter)
                    field = ExtractObjInfo(lvTabl.Columns[ci + 1].Text, refi);
                else
                    field = ExtractObjInfo(lvTabl.Columns[ci].Text, refi);

                if (field == "MISSING! DELETE!")
                    return new string[] { "MISSING! DELETE!", };
                    
                infos.SetValue(field, ci);
            }
            
            return infos;
        }
        /// <summary>
        /// extract object information from loaded objref
        /// </summary>
        /// <param name="htxt">header text</param>
        /// <param name="refi">index of ObjRef in loaded</param>
        /// <returns></returns>
        private string ExtractObjInfo(string htxt, int refi)
        {
            string info="";
            RhinoObject obj = Loaded[refi].Object();
            // flag missing obj in doc, deletion handled outside this method
            if (obj == null)
                return "MISSING! DELETE!";
            
            switch (htxt)
            {
                case "GUID":
                    info = obj.Id.ToString();
                    break;
                case "Type":
                    info = obj.ObjectType.ToString();
                    break;
                case "Name":
                    info = obj.Name;
                    break;
                case "Layer":
                    var li = obj.Attributes.LayerIndex;
                    var l = ParentDoc.Layers.FindIndex(li);
                    info = l.Name;
                    break;
                case "Color":
                    var c = obj.Attributes.DrawColor(ParentDoc);
                    if (settings.cf == 0)
                        info = c.ToString();
                    else if (settings.cf == 1)
                        info = string.Format("{0}-{1}-{2}", c.R, c.G, c.B);
                    else
                        info = string.Format("{0},{1},{2}", c.R, c.G, c.B);
                    break;
                case "LineType":
                    var lti = ParentDoc.Linetypes.LinetypeIndexForObject(obj);
                    info = ParentDoc.Linetypes[lti].Name;
                    break;
                case "PrintWidth":
                    double pw;
                    var pws = obj.Attributes.PlotWeightSource;
                    if (pws == ObjectPlotWeightSource.PlotWeightFromLayer)
                    {
                        li = obj.Attributes.LayerIndex;
                        l = ParentDoc.Layers.FindIndex(li);
                        pw = l.PlotWeight;
                    }
                    else pw = obj.Attributes.PlotWeight;
                    if (settings.ssopt[0]) info = pw.ToString() + "pt";
                    else info = pw.ToString();
                    break;
                case "PrintColor":
                    Color pc;
                    var pcs = obj.Attributes.PlotColorSource;
                    if (pcs == ObjectPlotColorSource.PlotColorFromLayer)
                    {
                        li = obj.Attributes.LayerIndex;
                        l = ParentDoc.Layers.FindIndex(li);
                        pc = l.PlotColor;
                    }
                    else pc = obj.Attributes.PlotColor;
                    if (settings.cf == 0)
                        info = pc.ToString();
                    else if (settings.cf == 1)
                        info = string.Format("{0}-{1}-{2}", pc.R, pc.G, pc.B);
                    else
                        info = string.Format("{0} {1} {2}", pc.R, pc.G, pc.B);
                    break;
                case "Material":
                    var mti = obj.Attributes.MaterialIndex;
                    var mt = ParentDoc.Materials.FindIndex(mti);
                    if (mt != null) info = mt.Name;
                    break;
                case "Length":
                    string len = "";
                    if (obj.ObjectType == ObjectType.Curve)
                    {
                        double len_num = Loaded[refi].Curve().GetLength();
                        len_num= Math.Round(len_num * settings.su, settings.dp); // scale + decimal place
                        if (settings.ts == ",")
                            len = KMarker(len_num, ',');
                        else if (settings.ts == ".")
                            len = KMarker(len_num, '.');
                        else if (settings.ts == " ")
                            len = KMarker(len_num, ' ');
                        else len = len_num.ToString();
                    }
                    if (settings.ssopt[0] && len != null) len += LenUnit(); // with unit
                    info = len;
                    break;
                case "Area":
                    AreaMassProperties amp = null;
                    if (obj.ObjectType == ObjectType.Brep)
                        amp = AreaMassProperties.Compute(Loaded[refi].Brep());
                    else if (obj.ObjectType == ObjectType.Curve)
                        if (Loaded[refi].Curve().IsClosed)
                            amp = AreaMassProperties.Compute(Loaded[refi].Curve());
                        else amp = null;
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                            amp = AreaMassProperties.Compute(Brep.TryConvertBrep(obj.Geometry));
                        else amp = null;
                    else if (obj.ObjectType == ObjectType.Mesh)
                        amp = AreaMassProperties.Compute(Loaded[refi].Mesh());

                    if (amp != null)
                    {
                        double area_num = Math.Round(amp.Area * settings.su, settings.dp);
                        string area;
                        if (settings.ts == ",")
                            area = KMarker(area_num, ',');
                        else if (settings.ts == ".")
                            area = KMarker(area_num, '.');
                        else if (settings.ts == " ")
                            area = KMarker(area_num, ' ');
                        else area = area_num.ToString();

                        if (settings.ssopt[0])
                        {
                            if (settings.cun != "" && settings.cun != null)
                                info = area + settings.cun + "\xB2";
                            else
                                info = area + LenUnit() + "\xB2";
                        }
                        else info = area;
                    }
                    break;
                case "Volume":
                    string vol = null;
                    if (obj.ObjectType == ObjectType.Brep)
                        if (Loaded[refi].Brep().IsSolid)
                        {
                            double vol_num = Loaded[refi].Brep().GetVolume(rtol, tol);
                            vol_num = Math.Round(vol_num * settings.su, settings.dp);
                            if (settings.ts == ",")
                                vol = KMarker(vol_num, ',');
                            else if (settings.ts == ".")
                                vol = KMarker(vol_num, '.');
                            else if (settings.ts == " ")
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
                                vol_num = Math.Round(vol_num * settings.su, settings.dp); // scale unit + decimal place
                                if (settings.ts == ",")
                                    vol = KMarker(vol_num, ',');
                                else if (settings.ts == ".")
                                    vol = KMarker(vol_num, '.');
                                else if (settings.ts == " ")
                                    vol = KMarker(vol_num, ' ');
                                else vol = vol_num.ToString();
                            }
                            else vol = "0";
                        }
                    }
                    else if (obj.ObjectType == ObjectType.Mesh)
                        if (Loaded[refi].Mesh().IsClosed)
                        {
                            VolumeMassProperties vmp = VolumeMassProperties.Compute(Loaded[refi].Mesh());
                            double vol_num = vmp.Volume;
                            vol_num = Math.Round(vol_num* settings.su, settings.dp); // scale unit + decimla place
                            if (settings.ts == ",")
                                vol = KMarker(vol_num, ',');
                            else if (settings.ts == ".")
                                vol = KMarker(vol_num, '.');
                            else if (settings.ts == " ")
                                vol = KMarker(vol_num, ' ');
                            else vol = vol_num.ToString();
                        }
                        else
                            vol = "0";
                    // append unit
                    if (!settings.ssopt[0] || vol == null) info = vol;
                    else if (vol != "open brep" && vol != "invalid extrusion")
                    {
                        if (settings.cun != "" && settings.cun != null)
                            info = vol + settings.cun + "\xB3";
                        else info = vol + LenUnit() + "\xB3";
                    }
                    break;
                case "NumPts":
                    if (obj.ObjectType == ObjectType.Curve)
                        info = Loaded[refi].Curve().ToNurbsCurve().Points.Count.ToString();
                    else if (obj.ObjectType == ObjectType.Mesh)
                        info = Loaded[refi].Mesh().Vertices.Count.ToString();
                    else if (obj.Geometry.HasBrepForm)
                        info = Brep.TryConvertBrep(obj.Geometry).Vertices.Count.ToString();
                        break;
                case "NumEdges":
                    if (obj.ObjectType == ObjectType.Brep)
                        info = Loaded[refi].Brep().Edges.Count.ToString();
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                            info = Brep.TryConvertBrep(obj.Geometry).Edges.Count.ToString();
                        else info = "invalid extrusion";
                    else if (obj.ObjectType == ObjectType.Mesh)
                        info = Loaded[refi].Mesh().TopologyEdges.Count.ToString();
                    break;
                case "NumFaces":
                    if (obj.ObjectType == ObjectType.Brep)
                        info = Loaded[refi].Brep().Faces.Count.ToString();
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                            info = Brep.TryConvertBrep(obj.Geometry).Faces.Count.ToString();
                        else info = "invalid extrusion";
                    else if (obj.ObjectType == ObjectType.Mesh)
                        info = Loaded[refi].Mesh().Faces.Count.ToString();
                    break;
                case "Degree":
                    if (obj.ObjectType == ObjectType.Curve)
                        info = Loaded[refi].Curve().Degree.ToString();
                    break;
                case "IsPlanar":
                    if (obj.ObjectType == ObjectType.Brep)
                    {
                        Brep brep = Loaded[refi].Brep();
                        if (brep.IsSurface)
                            if (brep.Faces[0].IsPlanar())
                                info = "yes";
                            else info = "no";
                        else info = "polysrf";
                    }
                    else if (obj.ObjectType == ObjectType.Curve)
                        if (Loaded[refi].Curve().IsPlanar())
                            info = "yes";
                        else info = "no";
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                        {
                            Brep brep = Brep.TryConvertBrep(obj.Geometry);
                            if (brep.IsSurface)
                                if (brep.Faces[0].IsPlanar())
                                    info = "yes";
                                else info = "no";
                            else info = "polysrf";
                        }
                        else info = "invalid extrusion";
                    else info = "irrelevant";
                    break;
                case "IsClosed":
                    if (obj.ObjectType == ObjectType.Brep)
                    {
                        Brep brep = Loaded[refi].Brep();
                        if (brep.IsSolid)
                            info = "yes";
                        else info = "no";
                    }
                    else if (obj.ObjectType == ObjectType.Curve)
                        if (Loaded[refi].Curve().IsClosed)
                            info = "yes";
                        else info = "no";
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                        {
                            Brep brep = Brep.TryConvertBrep(obj.Geometry);
                            if (brep.IsSolid)
                                info = "yes";
                            else info = "no";
                        }
                        else info = "invalid extrusion";
                    else info = "irrelevant";
                    break;
                case "Comments":
                    var usertxts = obj.Attributes.GetUserStrings(); // it's like a dictionary
                    string txt = "";
                    if (usertxts.AllKeys.Contains("Tabl_"))
                        txt = usertxts["Tabl_"];
                    else if (usertxts.Count == 1)
                        txt = usertxts[usertxts.AllKeys[0]];
                    else
                        txt = "keys: " + string.Join(";", usertxts.AllKeys);
                    info = txt.Length <= 50 ? txt : txt.Substring(0, 50) + "(...truncated)";
                    break;
                case "CenterPt":
                    Point3d ctr = obj.Geometry.GetBoundingBox(false).Center;
                    double px = Math.Round(ctr.X * settings.su, settings.dp);
                    double py = Math.Round(ctr.Y * settings.su, settings.dp);
                    double pz = Math.Round(ctr.Z * settings.su, settings.dp);
                    info = string.Format("{0}, {1}, {2}", px, py, pz);
                    break;
                case "Extents":
                    BoundingBox bb = obj.Geometry.GetBoundingBox(false);
                    double xe = Math.Round(bb.Diagonal.X * settings.su, settings.dp);
                    double ye = Math.Round(bb.Diagonal.Y * settings.su, settings.dp);
                    double ze = Math.Round(bb.Diagonal.Z * settings.su, settings.dp);
                    info = string.Format("{0}, {1}, {2}", xe, ye, ze);
                    break;
                default:
                    break;
            }
            return info;
        }

        /// <summary>
        /// make the totals line item for tabl
        /// </summary>
        /// <returns>line item to add to listview</returns>
        private TablLineItem TablTotals()
        {
            string[] numkeys = new string[] { "Area", "Volume", "Length", "NumPts", "NumFaces", "NumEdges", };
            string[] totals = new string[lvTabl.Columns.Count];
            for (int i = 0; i < totals.Length; i++)
            {
                ColumnHeader c = lvTabl.Columns[i];
                if (numkeys.Contains(c.Text))
                {
                    double tot = 0;
                    foreach (TablLineItem tli in lvTabl.Items)
                    {
                        string numstr = tli.SubItems[i].Text;
                        if (double.TryParse(DeUnit(numstr), out double n))
                            tot += n;
                    }
                    totals.SetValue(tot.ToString(), i);
                }
                else
                    totals.SetValue("", i);
            }
            return new TablLineItem(totals)
            {
                BackColor = Color.LightSteelBlue
            };
        }

        /// <summary>
        /// remove from tabl
        /// </summary>
        /// <param name="guids">object guid strings</param>
        private void RemoveByIds(string[] guids)
        {
            //first eliminate from doc string
            string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string id in guids)
                idstrings.Remove(id);
            ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace Loaded
            ReloadRefs(idstrings);
        }
        
        /// <summary>
        /// sort tabl by column header, only called when header click happens, this also maintains synchronization between tabl and loaded objrefs
        /// </summary>
        private void HeaderClickSort()
        {
            // detect total line
            TablLineItem totalline = null;
            if (settings.ssopt[1])
            {
                totalline = lvTabl.Items[lvTabl.Items.Count - 1] as TablLineItem;
                lvTabl.Items.RemoveAt(lvTabl.Items.Count - 1);
            }

            string htxt = lvTabl.Columns[sorthdr].Text;
            string[] txtfields = new string[] { "GUID", "Name", "Comments", "Type", "LineType", "Layer", "PrintColor", "Color", "Material", "IsClosed", "IsPlanar", };
            if (txtfields.Contains(htxt))
                lvTabl.ListViewItemSorter = new LVSorterByStr(sorthdr, sortord);
            else if (htxt == "CenterPt" || htxt == "Extents")
                lvTabl.ListViewItemSorter = new LVSorterByPt(sorthdr, sortord);
            else
                lvTabl.ListViewItemSorter = new LVSorterByNum(sorthdr, sortord);
            
            lvTabl.Sort();

            // !!! SYNC ORDER between tabl items and loaded objrefs here !!!
            for (int i = 0; i< lvTabl.Items.Count; i++)
            {
                if (settings.ssopt[1] && i == lvTabl.Items.Count - 1)
                    break;
                TablLineItem li = lvTabl.Items[i] as TablLineItem;
                Loaded.SetValue(new ObjRef(li.RefId), i);
            }

            // reset so next time tabl refreshes it stays same order as Loaded
            lvTabl.ListViewItemSorter = Comparer.Default;

            // restore if total line is visible
            if (settings.ssopt[1] && totalline != null)
                lvTabl.Items.Add(totalline);
        }
        private class LVSorterByStr : IComparer
        {
            public int hdridx;
            public int sortorder;
            public LVSorterByStr(int i, int o)
            {
                hdridx = i;
                sortorder = o;
            }
            int IComparer.Compare(object x, object y)
            {
                ListViewItem a = x as ListViewItem;
                ListViewItem b = y as ListViewItem;
                int r = Comparer.Default.Compare(a.SubItems[hdridx].Text, b.SubItems[hdridx].Text);
                if (sortorder != -1)
                    return r;
                else
                {
                    if (r == 1) return -1;
                    else if (r == -1) return 1;
                    else return 0;
                }
            }
        }
        private class LVSorterByNum : IComparer
        {
            public int hdridx;
            public int sortorder;
            public LVSorterByNum(int i, int o)
            {
                hdridx = i;
                sortorder = o;
            }
            int IComparer.Compare(object x, object y)
            {
                ListViewItem a = x as ListViewItem;
                ListViewItem b = y as ListViewItem;
                double.TryParse(DeUnit(a.SubItems[hdridx].Text), out double anum);
                double.TryParse(DeUnit(b.SubItems[hdridx].Text), out double bnum);
                int r = Comparer.Default.Compare(anum, bnum);

                // reverse if necessary
                if (sortorder != -1)
                    return r;
                else
                {
                    if (r == 1) return -1;
                    else if (r == -1) return 1;
                    else return 0;
                }
            }
        }
        private class LVSorterByPt : IComparer
        {
            public int hdridx;
            public int sortorder;
            public LVSorterByPt(int i, int o)
            {
                hdridx = i;
                sortorder = o;
            }
            int IComparer.Compare(object x, object y)
            {
                ListViewItem a = x as ListViewItem;
                ListViewItem b = y as ListViewItem;
                string atxt = a.SubItems[hdridx].Text;
                string btxt = b.SubItems[hdridx].Text;
                if (Point3d.TryParse(atxt, out Point3d apt) && Point3d.TryParse(btxt, out Point3d bpt))
                {
                    int r = Comparer.Default.Compare(apt.X, bpt.X);
                    if (r == 0) r = Comparer.Default.Compare(apt.Y, bpt.Y);
                    if (r == 0) r = Comparer.Default.Compare(apt.Z, bpt.Z);
                    
                    // test reverse or not
                    if (sortorder != -1)
                        return r;
                    else
                    {
                        if (r == 1) return -1;
                        else if (r == -1) return 1;
                        else return 0;
                    }
                }
                else
                    return 0;
            }
        }
        
        /// <summary>
        /// initialize point getter during placement of Tabl_ in rhinodoc
        /// </summary>
        private void InitializePtGetter()
        {
            ptgetter = new GetPoint();
            ptgetter.DynamicDraw += PlaceRectUpdate;
        }
        private void PlaceRectUpdate(object s, GetPointDrawEventArgs e)
        {
            if (!plcsettings.RectFollower.Enabled)
            {
                plcsettings.RectFollower.Enabled = true;
                Plane trgt = new Plane(e.CurrentPoint, plcsettings.BasePl.XAxis, plcsettings.BasePl.YAxis);
                plcsettings.RectFollower.rec.Transform(Transform.PlaneToPlane(plcsettings.RectFollower.rec.Plane, trgt));
            }
            Transform xform = Transform.Translation(new Vector3d(e.CurrentPoint - plcsettings.RectFollower.rec.Plane.Origin));
            plcsettings.RectFollower.rec.Transform(xform);
        }

        /// <summary>
        /// generate the Tabl_ to be placed in rhino doc, everything positioned against point3d.origin
        /// </summary>
        /// <param name="content">output the collection of cell contents</param>
        /// <param name="borders">output the collection of Tabl_ borders as lines</param>
        /// <returns>bounding rectangle of the Tabl_</returns>
        private Rectangle3d InDocTabl(out List<TextEntity[]> content, out List<Line> borders)
        {
            // load cell contents (texts)
            content = new List<TextEntity[]>(lvTabl.Items.Count+1); //+1 includes headers
            // headers
            TextEntity[] headerline = new TextEntity[lvTabl.Columns.Count];
            for (int i = 0; i < lvTabl.Columns.Count; i++)
            {
                TextEntity rhtxt = new TextEntity()
                {
                    PlainText = lvTabl.Columns[i].Text,
                    Plane = Plane.WorldXY,
                    DimensionStyleId = ParentDoc.DimStyles.CurrentId,
                };
                headerline.SetValue(rhtxt, i);
            }
            content.Add(headerline);
            // line items
            foreach (TablLineItem tli in lvTabl.Items)
            {
                TextEntity[] l = new TextEntity[tli.SubItems.Count];
                for (int i = 0; i < l.Length; i++)
                {
                    string txt = tli.SubItems[i].Text;
                    TextEntity rhtxt = new TextEntity()
                    {
                        PlainText = txt,
                        Plane = Plane.WorldXY,
                        DimensionStyleId = ParentDoc.DimStyles.CurrentId,
                    };
                    l.SetValue(rhtxt, i);
                }
                content.Add(l);
            }

            // column widths
            double[] cws = new double[content[0].Length]; // use first row to get num of col
            if (plcsettings.fitting == 2)
                for (int n = 0; n < cws.Length; n++)
                    cws.SetValue(plcsettings.cw, n);
            else if (plcsettings.fitting == 0)
            {
                // start from 0 and fit up
                for (int n = 0; n < cws.Length; n++)
                    cws.SetValue(0, n);
                // fit up
                for (int ri = 0; ri < content.Count; ri++)
                    for (int ci = 0; ci < content[ri].Length; ci++)
                        cws.SetValue(
                            content[ri][ci].TextModelWidth > cws[ci] ? content[ri][ci].TextModelWidth + (double)plcsettings.cellpad * 2 : cws[ci],
                            ci);
            }

            // borders, all based off point3d.origin
            borders = new List<Line>();
            double txtht = ParentDoc.DimStyles.Current.TextHeight;
            Line h0 = new Line(Point3d.Origin, Point3d.Origin + new Point3d(cws.Sum(), 0, 0)); // first horizontal
            double vdim = content.Count * (txtht + (double)plcsettings.cellpad * 2);// cell height
            Line v0 = new Line(Point3d.Origin, Point3d.Origin + new Point3d(0, -vdim, 0)); // first vertical
            borders.AddRange(new Line[] { h0, v0 });
            // all horizontals
            for (int i = 0; i < content.Count; i++)
            {
                Point3d dy = new Point3d(0, -(i + 1) * (txtht + (double)plcsettings.cellpad * 2), 0);
                Point3d start = Point3d.Origin + dy;
                Point3d end = Point3d.Origin + new Point3d(cws.Sum(), 0, 0) + dy;
                Line hl = new Line(start, end);
                borders.Add(hl);
            }
            // all verticals
            double xnow = 0;
            for (int i = 0; i < cws.Length; i++)
            {
                xnow += cws[i];
                Point3d dx = new Point3d(xnow, 0, 0);
                Point3d start = Point3d.Origin + dx;
                Point3d end = Point3d.Origin + new Point3d(0, -vdim, 0) + dx;
                Line vl = new Line(start, end);
                borders.Add(vl);
            }

            // position cell content texts
            for (int ri = 0; ri < content.Count; ri++)
            {
                double rowY = ri * ((double)plcsettings.cellpad * 2 + txtht) + (double)plcsettings.cellpad;
                TextEntity[] ts = content[ri];
                double txtX = 0; // mid cell x coordinate
                for (int ci = 0; ci < ts.Length; ci++)
                {
                    TextEntity te = ts[ci];
                    if (ci == 0) txtX = cws[0] / 2;
                    else txtX += cws[ci - 1] / 2 + cws[ci] / 2;
                    te.Plane = new Plane(Point3d.Origin + new Point3d(txtX-te.TextModelWidth/2.0, -rowY, 0), Plane.WorldXY.ZAxis);
                }
            }

            return new Rectangle3d(Plane.WorldXY, h0.Length, -v0.Length);
        }

    }

}
﻿using Rhino;
using Rhino.Input;
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
        // groups of curves, each group is from an object
        public List<Curve[]> crvs;
        public Color clr;
        public int w; // wire line width in pixels

        public Highlighter()
        {
            crvs = null;
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
        }

        /// <summary>
        /// add geometries selected in listview to mark in rhino doc
        /// </summary>
        /// <param name="lv">ListView where items are pulled</param>
        /// <param name="loaded">objrefs corresponding to the provided listview items</param>
        public void AddMarkerGeometries(ListView lv, ObjRef[] loaded)
        {
            foreach (int hli in lv.SelectedIndices)
                AddMarkerGeometry(hli, loaded);
        }
        /// <summary>
        /// add specific geometries from loaded ObjRef[] to preview markers. you should check if listview items and loaded match order and capacity
        /// </summary>
        /// <param name="idx">index in the loaded collection</param>
        /// <param name="loaded">loaded collection</param>
        public void AddMarkerGeometry(int idx, ObjRef[] loaded)
        {
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
            else
            {
                //TODO: more to preview
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

    }

    public partial class DockPanel : UserControl
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
            string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
            List<string> idstrs = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ReloadRefs(idstrs);
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
        private void PushRefs()
        {
            IEnumerable<string> idstrs = Loaded.Select(o => o.ObjectId.ToString());
            PushRefs(idstrs);
        }

        /// <summary>
        /// refresh spreadsheet
        /// </summary>
        /// <param name="th">true if threaded computing</param>
        private void RefreshTabl(bool th = false)
        {
            lvTabl.Clear();
            // set up headers
            List<string> keys = headers.Keys.ToList();
            keys.Sort(HeaderSorter); // this guarantees header order matching with menustrip's
            if (linecounter)
                lvTabl.Columns.Add(new ColumnHeader() { Text = "#", });
            foreach (string k in keys)
                if (headers[k])
                {
                    ColumnHeader ch = new ColumnHeader() { Text = k, TextAlign = HorizontalAlignment.Center };
                    lvTabl.Columns.Add(ch);
                }

            // fill spreadsheet
            List<ListViewItem> lines = new List<ListViewItem>();
            List<int> badidx = new List<int>(); // missing in document but objref still references

            /*
            if (settings.ssopt[3])
                Parallel.For(0, Loaded.Length, oi =>
                {
                    lock (locker)
                    {
                        string[] infos = TablLineItem(oi);
                        lis.Add(new ListViewItem(infos));
                    }
                });
            else
             */
            for (int oi =0; oi<Loaded.Length; oi++)
            {
                // serial
                string[] infos = TablLineItem(oi);
                if (infos[0] == "MISSING! DELETE!")
                {
                    badidx.Add(oi);
                    continue;
                }
                else if (!linecounter)
                    lines.Add(new ListViewItem(infos));
                else
                {
                    List<string> infolist = new List<string> { (oi+1).ToString(), };
                    infolist.AddRange(infos);
                    lines.Add(new ListViewItem(infolist.ToArray()));
                }
            }
            

            lvTabl.Items.AddRange(lines.ToArray()); // faster with addrange rather than add in a loop
            TablColAutoSize();

            if (badidx.Count != 0)
            {
                List<string> todelete = new List<string>();
                foreach (int idx in badidx)
                    todelete.Add(Loaded[idx].ObjectId.ToString());
                RemoveByIds(todelete.ToArray());
            }
            // prep for click-n-mark
            settings.docmarker.crvs = new List<Curve[]>(lvTabl.Items.Count);
            for (int n = 0; n < lvTabl.Items.Count; n++)
                settings.docmarker.crvs.Add(new Curve[] { });

            ParentDoc.Views.Redraw();
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
        /// get the line item for tabl
        /// </summary>
        /// <param name="refi">index of objref in loaded</param>
        /// <param name="th">true if threaded computing</param>
        /// <returns>array of the obj properties</returns>
        private string[] TablLineItem(int refi)
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
            
            // catches when user delete object in doc but tabl_ still has reference
            // objref can still return a guid, use it to remove in loaded and docstr
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
                    System.Drawing.Color pc;
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
                        var len_num = Math.Round(Loaded[refi].Curve().GetLength(), settings.dp); //decimal
                        len_num *= settings.su; //custom scale
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

                    if (amp != null)
                    {
                        double area_num = Math.Round(amp.Area, settings.dp);
                        area_num *= settings.su;
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
                            vol_num = Math.Round(vol_num, settings.dp);
                            vol_num *= settings.su;
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
                                vol_num = Math.Round(vol_num, settings.dp);
                                vol_num *= settings.su;
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
                    break;
                case "NumEdges":
                    if (obj.ObjectType == ObjectType.Brep)
                        info = Loaded[refi].Brep().Edges.Count.ToString();
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                            info = Brep.TryConvertBrep(obj.Geometry).Edges.Count.ToString();
                        else info = "invalid extrusion";

                    break;
                case "NumFaces":
                    if (obj.ObjectType == ObjectType.Brep)
                        info=Loaded[refi].Brep().Faces.Count.ToString();
                    else if (obj.ObjectType == ObjectType.Extrusion)
                        if (obj.Geometry.HasBrepForm)
                            info=Brep.TryConvertBrep(obj.Geometry).Faces.Count.ToString();
                        else info="invalid extrusion";

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
                    var usertxts = obj.Attributes.GetUserStrings();
                    string txt = null;
                    if (usertxts.Count == 1)
                        txt = usertxts[0];
                    else
                        txt = string.Join(";", usertxts.AllKeys);
                    info = "keys_" + txt;
                    break;
                case "CenterPt":
                    Point3d ctr = obj.Geometry.GetBoundingBox(false).Center;
                    double px = Math.Round(ctr.X, settings.dp) * settings.su;
                    double py = Math.Round(ctr.Y, settings.dp) * settings.su;
                    double pz = Math.Round(ctr.Z, settings.dp) * settings.su;
                    info = string.Format("{0}, {1}, {2}", px, py, pz);
                    break;
                default:
                    break;
            }
            return info;
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

        private void TablSort()
        {
            
        }
        /*private int LVItemComparer(ListViewItem a, ListViewItem b)
        {
            if (a.)
        }*/
    }

}
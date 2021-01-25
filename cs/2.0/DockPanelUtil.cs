using Rhino;
using Rhino.Input;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tabl_
{
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
        /// initialize checklistbox's menu strip
        /// </summary>
        private void InitializeCLMS()
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
        /// lock or unlock objects within Tabl_ loaded
        /// </summary>
        /// <param name="l">set true to lock already in tabl_</param>
        private void SetPickFilter(bool l)
        {
            if (l)
            {
                ParentDoc.Views.RedrawEnabled = false;
                foreach (ObjRef r in Loaded)
                    ParentDoc.Objects.Lock(r.ObjectId, false);
                ParentDoc.Views.RedrawEnabled = true;
            }
            else
            {
                ParentDoc.Views.RedrawEnabled = false;
                foreach (ObjRef r in Loaded)
                    ParentDoc.Objects.Unlock(r.ObjectId, false);
                ParentDoc.Views.RedrawEnabled = true;
            }
        }
        /// <summary>
        /// lock or unlock objects outside the Tabl_ loaded objects
        /// </summary>
        /// <param name="expt">exempt objects, lock status won't alter</param>
        /// <param name="l">set true to lock</param>
        private void SetPickFilter(RhinoObject[] expt, bool l = true)
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
        /// reload tabl_cs_selected into ObjRef
        /// </summary>
        /// <param name="idstrings">each id strings split from tabl_cs_selected</param>
        /// <returns>true on success</returns>
        private void ReloadRefs(List<string> idstrings)
        {
            ObjRef[] newselected = new ObjRef[idstrings.Count];
            for (int i = 0; i < idstrings.Count; i++)
                newselected.SetValue(new ObjRef(new Guid(idstrings[i])), i);
            Loaded = newselected;
        }

        private void RefreshTabl()
        {
            lvTabl.Clear();
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

            List<ListViewItem> lis = new List<ListViewItem>();
            // serial
            for (int oi =0; oi<Loaded.Length; oi++)
            {
                if (linecounter)
                string[] infos = TablLineItem(oi);
                lis.Add(new ListViewItem(infos));
                
            }
            // parallel
            /*
            Parallel.For(0, Loaded.Length, oi =>
            {
                lock (locker)
                {
                    string[] infos = TablLineItem(oi);
                    lis.Add(new ListViewItem(infos));
                }
            });*/
            
            lvTabl.Items.AddRange(lis.ToArray());
            lvTabl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// get the line item for tabl
        /// </summary>
        /// <param name="refi">index of objref in loaded</param>
        /// <returns>array of the obj properties</returns>
        private string[] TablLineItem(int refi, bool th = false)
        {
            string[] infos = new string[lvTabl.Columns.Count];
            if (th)
            {
                // TODO: finish this
                return infos;
            }
            else
            {
                for (int ci = 0; ci < infos.Length; ci++)
                    infos.SetValue(ExtractObjInfo(lvTabl.Columns[ci].Text, refi), ci);
                return infos;
            }
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
            {
                info = "MISSING! Please refresh table";
                RemoveById(obj.Id.ToString());
                return info;
            }

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
                    if (settings.displayopt[0]) info = pw.ToString() + "pt";
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
                    if (settings.displayopt[0] && len != null) len += LenUnit(); // with unit
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

                        if (settings.displayopt[0])
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

                    if (!settings.displayopt[0] || vol == null) info = vol;
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
        /// remove single object by id from tabl 
        /// DO NOT use in a loop
        /// </summary>
        /// <param name="guid">object guid string</param>
        /// <returns>true if success</returns>
        private void RemoveById(string guid)
        {
            //first eliminate from doc string
            string raw = ParentDoc.Strings.GetValue("tabl_cs_selected");
            List<string> idstrings = raw.Split(new string[] { ",", }, StringSplitOptions.RemoveEmptyEntries).ToList();
            idstrings.Remove(guid);
            ParentDoc.Strings.SetString("tabl_cs_selected", string.Join(",", idstrings));

            //replace Loaded
            ReloadRefs(idstrings);
        }
    }
}
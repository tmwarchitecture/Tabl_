using Rhino;
using Rhino.Input;
using Rhino.Commands;
using Rhino.DocObjects;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tabl_
{
    public partial class DockPanel : UserControl
    {
        /// <summary>
        /// initialize checklistbox's menu strip
        /// </summary>
        private void InitializeCLMS()
        {
            msHeader.Closing += HeaderStripClosing;
            msHeader.Opening += HeaderStripOpening;
            foreach (ToolStripMenuItem i in msHeader.Items)
                i.CheckOnClick = true;
        }
        /// <summary>
        /// know which headers to show from the menustrip
        /// </summary>
        private void SetHeaderVis()
        {
            foreach (ToolStripMenuItem i in msHeader.Items)
            {
                if (i.Checked)
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
                foreach (ToolStripMenuItem i in msHeader.Items)
                    if (h.Text == i.Text)
                    {
                        i.Checked = true;
                        break;
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
            keys.Sort(HeaderSorter);
            foreach (string k in keys)
                if (headers[k])
                {
                    ColumnHeader ch = new ColumnHeader() { Text = k, TextAlign = HorizontalAlignment.Center };
                    lvTabl.Columns.Add(ch);
                }
            lvTabl.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            /*TODO: finish this
             * acutally populate data
            */

        }

        /// <summary>
        /// extract object information from loaded objref
        /// </summary>
        /// <param name="htxt">header text</param>
        /// <param name="refi">index of objref in loaded</param>
        /// <returns></returns>
        private string ExtractObjInfo(string htxt, int refi)
        {
            string info;
            ObjRef oref = Loaded[refi];
            switch (htxt)
            {
                case "#":
                    info = refi.ToString();
                    break;
                case "GUID":
                    info = oref.ObjectId.ToString();
                    break;
                case "Type":
                    info = oref.Object().GetType().ToString();
                    break;
                case "Name":
                    info = oref.Object().Name;
                    break;
                default:
                    info = "";
                    break;
            }
            return info;
        }
    }
}
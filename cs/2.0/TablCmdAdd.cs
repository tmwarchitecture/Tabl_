using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Rhino;
using Rhino.Input;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.UI;

namespace Tabl_
{
    public class TablCmdAdd : Command
    {
        static TablCmdAdd _instance;
        public TablCmdAdd()
        {
            _instance = this;
        }

        ///<summary>The only instance of the TablCmdAdd command.</summary>
        public static TablCmdAdd Instance
        {
            get { return _instance; }
        }

        public override string EnglishName
        {
            get { return "AddToTabl"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: add to loaded, refresh doctstr and refresh tabl if needed
            TablDockPanel tablpanel = Panels.GetPanel<TablDockPanel>(RhinoDoc.ActiveDoc);
            bool blitz = false;
            var r = RhinoGet.GetBool(" Erase existing Tabl_ contents?", false, "No", "Yes", ref blitz);
            if (r == Result.Success)
            {
                r = RhinoGet.GetMultipleObjects(" Select objects to add", true, ObjectType.AnyObject, out ObjRef[] picked);
                if (r == Result.Success)
                {
                    if (blitz)
                    {
                        tablpanel.PushRefs(picked.Select(i => i.ObjectId.ToString()));
                        tablpanel.Loaded = picked;
                    }
                    else
                    {
                        List<ObjRef> concat = new List<ObjRef>();
                        concat.AddRange(tablpanel.Loaded);
                        var ids = tablpanel.Loaded.Select(i => i.ObjectId.ToString());
                        foreach (ObjRef oref in picked)
                            if (!ids.Contains(oref.ObjectId.ToString()))
                                concat.Add(oref);
                        tablpanel.PushRefs(concat.Select(i => i.ObjectId.ToString()));
                        tablpanel.Loaded = concat.ToArray();
                    }
                    tablpanel.Refresh_Click(null, null); // dummy args, method doesn't use them anyway
                    if (!Panels.IsPanelVisible(TablDockPanel.PanelId))
                        MessageBox.Show("Added to Tabl_\nType \"LaunchTabl_\" to open it", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    RhinoApp.WriteLine("command interrupted, nothing done...");
            }
            else
                RhinoApp.WriteLine("command interrupted, nothing done...");

            return Result.Success;
        }
    }
}
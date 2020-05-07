using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;

namespace Tabl_cs
{
    public class TablTools : Command
    {
        MainForm app = null; // keeps a copy of main app

        public TablTools()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static TablTools Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "TablTools"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            /*
            // once instantiated, app window is never disposed until rhino closes
            if (app == null) app = new MainForm(RhinoDoc.ActiveDoc);
            try { app.Show(RhinoWinApp.MainWindow); }
            catch (InvalidOperationException) { RhinoApp.WriteLine(" dialog is already open"); }
            RhinoDoc.ModifyObjectAttributes += app.AutoRefresh;
            EndCommand += app.AutoRefresh; // static event, bond to all command endings
            */
            RhinoApp.WriteLine(" deprecated... try \"TablDock\"");
            return Result.Success;
        }


    }
}

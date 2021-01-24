using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;

namespace Tabl_
{
    public class TablCmd : Command
    {
        public TablCmd()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static TablCmd Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "LaunchTabl_"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Guid pid = DockPanel.PanelId;
            bool open = Panels.IsPanelVisible(pid);
            if (open) Panels.ClosePanel(pid);
            else Panels.OpenPanel(pid);

            return Result.Success;
        }
    }
}

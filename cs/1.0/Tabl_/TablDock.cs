using System;
using Rhino;
using Rhino.Commands;
using Rhino.UI;

namespace Tabl_cs
{
    public class TablDock : Command
    {

        public TablDock()
        {
            Instance = this;
        }

        ///<summary>The only instance of the TablDock command.</summary>
        public static TablDock Instance
        {
            get; private set;
        }

        public override string EnglishName
        {
            get { return "TablDock"; }
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
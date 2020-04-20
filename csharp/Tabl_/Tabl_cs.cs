using Rhino.PlugIns;
using System;
using System.Drawing;
using Rhino.UI;

namespace Tabl_cs
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class Tabl_cs : PlugIn

    {
        public Tabl_cs()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the Tabl plug-in.</summary>
        public static Tabl_cs Instance
        {
            get; private set;
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            Type paneltype = typeof(DockPanel);
            Panels.RegisterPanel(this, paneltype, "Tabl_dock", Properties.Resources.main);
            return LoadReturnCode.Success;
        }

        public DockPanel TablPanel { get; set; }
    }
}
﻿namespace Tabl_
{
    partial class TablDockPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvTabl = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvCtxtMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ShowColToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msHeaders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.areaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numPtsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numEdgesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numFacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.degreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LocPtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isPlanarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isClosedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editHighlightedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msEdits = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbmsNameChange = new System.Windows.Forms.ToolStripTextBox();
            this.applyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lineTypeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.printColorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.printWidthToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.materialToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.commentsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fitCellWidthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTextsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copySpreadsheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToHighlightedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ttipBtns = new System.Windows.Forms.ToolTip(this.components);
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnPlace = new System.Windows.Forms.Button();
            this.btnTrash = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSelectInDoc = new System.Windows.Forms.Button();
            this.dlogExport = new System.Windows.Forms.SaveFileDialog();
            this.dlogImport = new System.Windows.Forms.OpenFileDialog();
            this.btnEnv = new System.Windows.Forms.Button();
            this.lvCtxtMenu.SuspendLayout();
            this.msHeaders.SuspendLayout();
            this.msEdits.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvTabl
            // 
            this.lvTabl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTabl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvTabl.ContextMenuStrip = this.lvCtxtMenu;
            this.lvTabl.FullRowSelect = true;
            this.lvTabl.HideSelection = false;
            this.lvTabl.Location = new System.Drawing.Point(4, 37);
            this.lvTabl.Margin = new System.Windows.Forms.Padding(4);
            this.lvTabl.Name = "lvTabl";
            this.lvTabl.Size = new System.Drawing.Size(540, 590);
            this.lvTabl.TabIndex = 0;
            this.lvTabl.UseCompatibleStateImageBehavior = false;
            this.lvTabl.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Type";
            this.columnHeader1.Width = 97;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Layer";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 126;
            // 
            // lvCtxtMenu
            // 
            this.lvCtxtMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.lvCtxtMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ShowColToolStripMenuItem,
            this.editHighlightedToolStripMenuItem,
            this.fitCellWidthsToolStripMenuItem,
            this.copyTextsToolStripMenuItem,
            this.copySpreadsheetToolStripMenuItem,
            this.zoomToHighlightedToolStripMenuItem});
            this.lvCtxtMenu.Name = "lvCtxtMenu";
            this.lvCtxtMenu.Size = new System.Drawing.Size(199, 148);
            // 
            // ShowColToolStripMenuItem
            // 
            this.ShowColToolStripMenuItem.DropDown = this.msHeaders;
            this.ShowColToolStripMenuItem.Name = "ShowColToolStripMenuItem";
            this.ShowColToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.ShowColToolStripMenuItem.Text = "Show Columns";
            // 
            // msHeaders
            // 
            this.msHeaders.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msHeaders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.gUIDToolStripMenuItem,
            this.typeToolStripMenuItem,
            this.nameToolStripMenuItem,
            this.layerToolStripMenuItem,
            this.colorToolStripMenuItem,
            this.lineTypeToolStripMenuItem,
            this.printColorToolStripMenuItem,
            this.printWidthToolStripMenuItem,
            this.materialToolStripMenuItem,
            this.lengthToolStripMenuItem,
            this.areaToolStripMenuItem,
            this.volumeToolStripMenuItem,
            this.numPtsToolStripMenuItem,
            this.numEdgesToolStripMenuItem,
            this.numFacesToolStripMenuItem,
            this.degreeToolStripMenuItem,
            this.LocPtToolStripMenuItem,
            this.extentsToolStripMenuItem,
            this.isPlanarToolStripMenuItem,
            this.isClosedToolStripMenuItem,
            this.commentsToolStripMenuItem});
            this.msHeaders.Name = "msHeader";
            this.msHeaders.OwnerItem = this.ShowColToolStripMenuItem;
            this.msHeaders.ShowCheckMargin = true;
            this.msHeaders.ShowImageMargin = false;
            this.msHeaders.Size = new System.Drawing.Size(151, 532);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(150, 24);
            this.toolStripMenuItem2.Text = "#";
            this.toolStripMenuItem2.ToolTipText = "item order may change\r\nuse GUID to ID objects";
            // 
            // gUIDToolStripMenuItem
            // 
            this.gUIDToolStripMenuItem.CheckOnClick = true;
            this.gUIDToolStripMenuItem.Name = "gUIDToolStripMenuItem";
            this.gUIDToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.gUIDToolStripMenuItem.Text = "GUID";
            this.gUIDToolStripMenuItem.ToolTipText = "unique object identifier";
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.typeToolStripMenuItem.Text = "Type";
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.CheckOnClick = true;
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.nameToolStripMenuItem.Text = "Name";
            // 
            // layerToolStripMenuItem
            // 
            this.layerToolStripMenuItem.Name = "layerToolStripMenuItem";
            this.layerToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.layerToolStripMenuItem.Text = "Layer";
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.colorToolStripMenuItem.Text = "Color";
            // 
            // lineTypeToolStripMenuItem
            // 
            this.lineTypeToolStripMenuItem.Name = "lineTypeToolStripMenuItem";
            this.lineTypeToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.lineTypeToolStripMenuItem.Text = "LineType";
            // 
            // printColorToolStripMenuItem
            // 
            this.printColorToolStripMenuItem.Name = "printColorToolStripMenuItem";
            this.printColorToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.printColorToolStripMenuItem.Text = "PrintColor";
            // 
            // printWidthToolStripMenuItem
            // 
            this.printWidthToolStripMenuItem.Name = "printWidthToolStripMenuItem";
            this.printWidthToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.printWidthToolStripMenuItem.Text = "PrintWidth";
            // 
            // materialToolStripMenuItem
            // 
            this.materialToolStripMenuItem.Name = "materialToolStripMenuItem";
            this.materialToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.materialToolStripMenuItem.Text = "Material";
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.lengthToolStripMenuItem.Text = "Length";
            // 
            // areaToolStripMenuItem
            // 
            this.areaToolStripMenuItem.ForeColor = System.Drawing.Color.Firebrick;
            this.areaToolStripMenuItem.Name = "areaToolStripMenuItem";
            this.areaToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.areaToolStripMenuItem.Text = "Area";
            this.areaToolStripMenuItem.ToolTipText = "lots of computing power needed\r\nexpect delays in UI";
            // 
            // volumeToolStripMenuItem
            // 
            this.volumeToolStripMenuItem.ForeColor = System.Drawing.Color.Firebrick;
            this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            this.volumeToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.volumeToolStripMenuItem.Text = "Volume";
            this.volumeToolStripMenuItem.ToolTipText = "lots of computing power needed\r\nexpect delays in UI";
            // 
            // numPtsToolStripMenuItem
            // 
            this.numPtsToolStripMenuItem.Name = "numPtsToolStripMenuItem";
            this.numPtsToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.numPtsToolStripMenuItem.Text = "NumPts";
            this.numPtsToolStripMenuItem.ToolTipText = "number of control points or vertices";
            // 
            // numEdgesToolStripMenuItem
            // 
            this.numEdgesToolStripMenuItem.Name = "numEdgesToolStripMenuItem";
            this.numEdgesToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.numEdgesToolStripMenuItem.Text = "NumEdges";
            this.numEdgesToolStripMenuItem.ToolTipText = "number of edges";
            // 
            // numFacesToolStripMenuItem
            // 
            this.numFacesToolStripMenuItem.Name = "numFacesToolStripMenuItem";
            this.numFacesToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.numFacesToolStripMenuItem.Text = "NumFaces";
            this.numFacesToolStripMenuItem.ToolTipText = "number of faces";
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.degreeToolStripMenuItem.Text = "Degree";
            this.degreeToolStripMenuItem.ToolTipText = "curve or isocurve degree";
            // 
            // LocPtToolStripMenuItem
            // 
            this.LocPtToolStripMenuItem.Name = "LocPtToolStripMenuItem";
            this.LocPtToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.LocPtToolStripMenuItem.Text = "LocationPt";
            this.LocPtToolStripMenuItem.ToolTipText = "general location xyz of the object";
            // 
            // extentsToolStripMenuItem
            // 
            this.extentsToolStripMenuItem.Name = "extentsToolStripMenuItem";
            this.extentsToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.extentsToolStripMenuItem.Text = "Extents";
            this.extentsToolStripMenuItem.ToolTipText = "x,y,z extents of object bounding box (world xyz aligned)";
            // 
            // isPlanarToolStripMenuItem
            // 
            this.isPlanarToolStripMenuItem.Name = "isPlanarToolStripMenuItem";
            this.isPlanarToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.isPlanarToolStripMenuItem.Text = "IsPlanar";
            // 
            // isClosedToolStripMenuItem
            // 
            this.isClosedToolStripMenuItem.Name = "isClosedToolStripMenuItem";
            this.isClosedToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.isClosedToolStripMenuItem.Text = "IsClosed";
            // 
            // commentsToolStripMenuItem
            // 
            this.commentsToolStripMenuItem.Name = "commentsToolStripMenuItem";
            this.commentsToolStripMenuItem.Size = new System.Drawing.Size(150, 24);
            this.commentsToolStripMenuItem.Text = "Comments";
            this.commentsToolStripMenuItem.ToolTipText = "a.k.a user text";
            // 
            // editHighlightedToolStripMenuItem
            // 
            this.editHighlightedToolStripMenuItem.DropDown = this.msEdits;
            this.editHighlightedToolStripMenuItem.Name = "editHighlightedToolStripMenuItem";
            this.editHighlightedToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.editHighlightedToolStripMenuItem.Text = "Edit Properties";
            this.editHighlightedToolStripMenuItem.ToolTipText = "warning: no undo";
            // 
            // msEdits
            // 
            this.msEdits.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.msEdits.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cToolStripMenuItem,
            this.changeLayerToolStripMenuItem,
            this.colorToolStripMenuItem1,
            this.lineTypeToolStripMenuItem1,
            this.printColorToolStripMenuItem1,
            this.printWidthToolStripMenuItem1,
            this.materialToolStripMenuItem1,
            this.commentsToolStripMenuItem1});
            this.msEdits.Name = "msEdits";
            this.msEdits.Size = new System.Drawing.Size(211, 224);
            // 
            // cToolStripMenuItem
            // 
            this.cToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbmsNameChange,
            this.applyToolStripMenuItem});
            this.cToolStripMenuItem.Name = "cToolStripMenuItem";
            this.cToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.cToolStripMenuItem.Text = "Name";
            // 
            // tbmsNameChange
            // 
            this.tbmsNameChange.BackColor = System.Drawing.SystemColors.Menu;
            this.tbmsNameChange.Name = "tbmsNameChange";
            this.tbmsNameChange.Size = new System.Drawing.Size(100, 27);
            // 
            // applyToolStripMenuItem
            // 
            this.applyToolStripMenuItem.Name = "applyToolStripMenuItem";
            this.applyToolStripMenuItem.Size = new System.Drawing.Size(175, 26);
            this.applyToolStripMenuItem.Text = "Apply change";
            this.applyToolStripMenuItem.Click += new System.EventHandler(this.MenuStripNameChange_Apply);
            // 
            // changeLayerToolStripMenuItem
            // 
            this.changeLayerToolStripMenuItem.Name = "changeLayerToolStripMenuItem";
            this.changeLayerToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.changeLayerToolStripMenuItem.Text = "Layer";
            this.changeLayerToolStripMenuItem.Click += new System.EventHandler(this.MenuStripChangeLayer_Click);
            // 
            // colorToolStripMenuItem1
            // 
            this.colorToolStripMenuItem1.Name = "colorToolStripMenuItem1";
            this.colorToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.colorToolStripMenuItem1.Text = "Color";
            this.colorToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripColorChange_Click);
            // 
            // lineTypeToolStripMenuItem1
            // 
            this.lineTypeToolStripMenuItem1.Name = "lineTypeToolStripMenuItem1";
            this.lineTypeToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.lineTypeToolStripMenuItem1.Text = "LineType";
            this.lineTypeToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripLTypeChange_Click);
            // 
            // printColorToolStripMenuItem1
            // 
            this.printColorToolStripMenuItem1.Name = "printColorToolStripMenuItem1";
            this.printColorToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.printColorToolStripMenuItem1.Text = "PrintColor";
            this.printColorToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripPrintClrChange_Click);
            // 
            // printWidthToolStripMenuItem1
            // 
            this.printWidthToolStripMenuItem1.Name = "printWidthToolStripMenuItem1";
            this.printWidthToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.printWidthToolStripMenuItem1.Text = "PrintWidth";
            this.printWidthToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripPrintWChange_Click);
            // 
            // materialToolStripMenuItem1
            // 
            this.materialToolStripMenuItem1.Name = "materialToolStripMenuItem1";
            this.materialToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.materialToolStripMenuItem1.Text = "Material";
            this.materialToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripMatChange_Click);
            // 
            // commentsToolStripMenuItem1
            // 
            this.commentsToolStripMenuItem1.Name = "commentsToolStripMenuItem1";
            this.commentsToolStripMenuItem1.Size = new System.Drawing.Size(210, 24);
            this.commentsToolStripMenuItem1.Text = "Comments";
            this.commentsToolStripMenuItem1.Click += new System.EventHandler(this.MenuStripCommentsChange_Click);
            // 
            // fitCellWidthsToolStripMenuItem
            // 
            this.fitCellWidthsToolStripMenuItem.Name = "fitCellWidthsToolStripMenuItem";
            this.fitCellWidthsToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.fitCellWidthsToolStripMenuItem.Text = "Fit Cell Widths";
            this.fitCellWidthsToolStripMenuItem.Click += new System.EventHandler(this.MenuStripFitCellWidth_Click);
            // 
            // copyTextsToolStripMenuItem
            // 
            this.copyTextsToolStripMenuItem.Name = "copyTextsToolStripMenuItem";
            this.copyTextsToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.copyTextsToolStripMenuItem.Text = "Copy Text";
            this.copyTextsToolStripMenuItem.ToolTipText = "Copy selected as CSV";
            this.copyTextsToolStripMenuItem.Click += new System.EventHandler(this.MenuStripCopyTabl_Click);
            // 
            // copySpreadsheetToolStripMenuItem
            // 
            this.copySpreadsheetToolStripMenuItem.Name = "copySpreadsheetToolStripMenuItem";
            this.copySpreadsheetToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.copySpreadsheetToolStripMenuItem.Text = "Copy Spreadsheet";
            this.copySpreadsheetToolStripMenuItem.ToolTipText = "Copy selected as tab-separated values";
            this.copySpreadsheetToolStripMenuItem.Click += new System.EventHandler(this.MenuStripCopyTabl_Click);
            // 
            // zoomToHighlightedToolStripMenuItem
            // 
            this.zoomToHighlightedToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.zoomToHighlightedToolStripMenuItem.Name = "zoomToHighlightedToolStripMenuItem";
            this.zoomToHighlightedToolStripMenuItem.Size = new System.Drawing.Size(198, 24);
            this.zoomToHighlightedToolStripMenuItem.Text = "Zoom Selected";
            this.zoomToHighlightedToolStripMenuItem.Click += new System.EventHandler(this.MenuStripZoom_Click);
            // 
            // btnInfo
            // 
            this.btnInfo.BackgroundImage = global::Tabl_.Properties.Resources.Info_2x;
            this.btnInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnInfo.Location = new System.Drawing.Point(299, 4);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(25, 25);
            this.btnInfo.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnInfo, "About Tabl_");
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.Info_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackgroundImage = global::Tabl_.Properties.Resources.Export_2x;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExport.Location = new System.Drawing.Point(266, 4);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(25, 25);
            this.btnExport.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnExport, "Export data to CSV");
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.Export_Click);
            // 
            // btnImport
            // 
            this.btnImport.BackgroundImage = global::Tabl_.Properties.Resources.Import_2x;
            this.btnImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnImport.Location = new System.Drawing.Point(233, 4);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(25, 25);
            this.btnImport.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnImport, "Import data from CSV.\r\nWill add to existing list");
            this.btnImport.UseVisualStyleBackColor = true;
            this.btnImport.Click += new System.EventHandler(this.Import_Click);
            // 
            // btnPlace
            // 
            this.btnPlace.BackgroundImage = global::Tabl_.Properties.Resources.Placement_2x;
            this.btnPlace.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPlace.Location = new System.Drawing.Point(200, 4);
            this.btnPlace.Margin = new System.Windows.Forms.Padding(4);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(25, 25);
            this.btnPlace.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnPlace, "Place Tabl_ data in Rhino");
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Click += new System.EventHandler(this.Place_Click);
            // 
            // btnTrash
            // 
            this.btnTrash.BackgroundImage = global::Tabl_.Properties.Resources.Trash_2x;
            this.btnTrash.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnTrash.Location = new System.Drawing.Point(167, 4);
            this.btnTrash.Margin = new System.Windows.Forms.Padding(4);
            this.btnTrash.Name = "btnTrash";
            this.btnTrash.Size = new System.Drawing.Size(25, 25);
            this.btnTrash.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnTrash, "Clear Tabl_");
            this.btnTrash.UseVisualStyleBackColor = true;
            this.btnTrash.Click += new System.EventHandler(this.Trash_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.BackgroundImage = global::Tabl_.Properties.Resources.Settings_2x;
            this.btnSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSettings.Location = new System.Drawing.Point(134, 4);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(25, 25);
            this.btnSettings.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnSettings, "Settings");
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackgroundImage = global::Tabl_.Properties.Resources.Refresh_2x;
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRefresh.Location = new System.Drawing.Point(70, 4);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(25, 25);
            this.btnRefresh.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnRefresh, "Refresh Tabl_");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackgroundImage = global::Tabl_.Properties.Resources.Remove_2x;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.Location = new System.Drawing.Point(37, 4);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 25);
            this.btnRemove.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnRemove, "Select objects to remove");
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackgroundImage = global::Tabl_.Properties.Resources.Add_2x;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.Location = new System.Drawing.Point(4, 4);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 25);
            this.btnAdd.TabIndex = 1;
            this.ttipBtns.SetToolTip(this.btnAdd, "Add object from Rhino");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.Add_Click);
            // 
            // btnSelectInDoc
            // 
            this.btnSelectInDoc.BackgroundImage = global::Tabl_.Properties.Resources.Select_2x;
            this.btnSelectInDoc.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSelectInDoc.Location = new System.Drawing.Point(102, 4);
            this.btnSelectInDoc.Name = "btnSelectInDoc";
            this.btnSelectInDoc.Size = new System.Drawing.Size(25, 25);
            this.btnSelectInDoc.TabIndex = 3;
            this.ttipBtns.SetToolTip(this.btnSelectInDoc, "Select object in Rhino");
            this.btnSelectInDoc.UseVisualStyleBackColor = true;
            this.btnSelectInDoc.Click += new System.EventHandler(this.SelectInDoc_Click);
            // 
            // dlogExport
            // 
            this.dlogExport.Filter = "Comma Separated Values|*.csv";
            // 
            // dlogImport
            // 
            this.dlogImport.FileName = "select csv to import";
            this.dlogImport.Filter = "Comma Separated Values|*.csv";
            this.dlogImport.Multiselect = true;
            this.dlogImport.ReadOnlyChecked = true;
            // 
            // btnEnv
            // 
            this.btnEnv.BackColor = System.Drawing.Color.Orange;
            this.btnEnv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnv.Image = global::Tabl_.Properties.Resources.export;
            this.btnEnv.Location = new System.Drawing.Point(332, 4);
            this.btnEnv.Margin = new System.Windows.Forms.Padding(4);
            this.btnEnv.Name = "btnEnv";
            this.btnEnv.Size = new System.Drawing.Size(25, 25);
            this.btnEnv.TabIndex = 1;
            this.btnEnv.UseVisualStyleBackColor = false;
            this.btnEnv.Visible = false;
            this.btnEnv.Click += new System.EventHandler(this.Env_Click);
            // 
            // TablDockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnSelectInDoc);
            this.Controls.Add(this.btnInfo);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnEnv);
            this.Controls.Add(this.btnPlace);
            this.Controls.Add(this.btnTrash);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lvTabl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "TablDockPanel";
            this.Size = new System.Drawing.Size(548, 631);
            this.lvCtxtMenu.ResumeLayout(false);
            this.msHeaders.ResumeLayout(false);
            this.msEdits.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvTabl;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnTrash;
        private System.Windows.Forms.Button btnPlace;
        private System.Windows.Forms.Button btnEnv;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnInfo;
        private System.Windows.Forms.ContextMenuStrip msHeaders;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem gUIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineTypeToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printColorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem printWidthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem areaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem volumeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numPtsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numEdgesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem numFacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem degreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LocPtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isPlanarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isClosedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip lvCtxtMenu;
        private System.Windows.Forms.ToolStripMenuItem ShowColToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editHighlightedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTextsToolStripMenuItem;
        private System.Windows.Forms.ToolTip ttipBtns;
        private System.Windows.Forms.ContextMenuStrip msEdits;
        private System.Windows.Forms.ToolStripMenuItem changeLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem lineTypeToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem printColorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem printWidthToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem materialToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem commentsToolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog dlogExport;
        private System.Windows.Forms.OpenFileDialog dlogImport;
        private System.Windows.Forms.ToolStripMenuItem zoomToHighlightedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copySpreadsheetToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox tbmsNameChange;
        private System.Windows.Forms.ToolStripMenuItem applyToolStripMenuItem;
        private System.Windows.Forms.Button btnSelectInDoc;
        private System.Windows.Forms.ToolStripMenuItem extentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fitCellWidthsToolStripMenuItem;
    }
}

﻿namespace Tabl_
{
    partial class DockPanel
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnTrash = new System.Windows.Forms.Button();
            this.btnPlace = new System.Windows.Forms.Button();
            this.btnEnv = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnInfo = new System.Windows.Forms.Button();
            this.msHeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.gUIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lineTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.centerPtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isPlanarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.isClosedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.msHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvTabl
            // 
            this.lvTabl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTabl.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvTabl.ContextMenuStrip = this.msHeader;
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
            // btnAdd
            // 
            this.btnAdd.Image = global::Tabl_.Properties.Resources.add;
            this.btnAdd.Location = new System.Drawing.Point(4, 4);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(25, 25);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.Add_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Image = global::Tabl_.Properties.Resources.remove;
            this.btnRemove.Location = new System.Drawing.Point(37, 4);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(25, 25);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Image = global::Tabl_.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(70, 4);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(25, 25);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Image = global::Tabl_.Properties.Resources.setting;
            this.btnSettings.Location = new System.Drawing.Point(103, 4);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(4);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(25, 25);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.Settings_Click);
            // 
            // btnTrash
            // 
            this.btnTrash.Image = global::Tabl_.Properties.Resources.trash;
            this.btnTrash.Location = new System.Drawing.Point(136, 4);
            this.btnTrash.Margin = new System.Windows.Forms.Padding(4);
            this.btnTrash.Name = "btnTrash";
            this.btnTrash.Size = new System.Drawing.Size(25, 25);
            this.btnTrash.TabIndex = 1;
            this.btnTrash.UseVisualStyleBackColor = true;
            this.btnTrash.Click += new System.EventHandler(this.Trash_Click);
            // 
            // btnPlace
            // 
            this.btnPlace.Image = global::Tabl_.Properties.Resources.import;
            this.btnPlace.Location = new System.Drawing.Point(169, 4);
            this.btnPlace.Margin = new System.Windows.Forms.Padding(4);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(25, 25);
            this.btnPlace.TabIndex = 1;
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Click += new System.EventHandler(this.Place_Click);
            // 
            // btnEnv
            // 
            this.btnEnv.BackColor = System.Drawing.Color.Orange;
            this.btnEnv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnv.Image = global::Tabl_.Properties.Resources.export;
            this.btnEnv.Location = new System.Drawing.Point(301, 4);
            this.btnEnv.Margin = new System.Windows.Forms.Padding(4);
            this.btnEnv.Name = "btnEnv";
            this.btnEnv.Size = new System.Drawing.Size(25, 25);
            this.btnEnv.TabIndex = 1;
            this.btnEnv.UseVisualStyleBackColor = false;
            this.btnEnv.Click += new System.EventHandler(this.Env_Click);
            // 
            // btnImport
            // 
            this.btnImport.Image = global::Tabl_.Properties.Resources.download;
            this.btnImport.Location = new System.Drawing.Point(202, 4);
            this.btnImport.Margin = new System.Windows.Forms.Padding(4);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(25, 25);
            this.btnImport.TabIndex = 1;
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Image = global::Tabl_.Properties.Resources.upload;
            this.btnExport.Location = new System.Drawing.Point(235, 4);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(25, 25);
            this.btnExport.TabIndex = 1;
            this.btnExport.UseVisualStyleBackColor = true;
            // 
            // btnInfo
            // 
            this.btnInfo.Image = global::Tabl_.Properties.Resources.info;
            this.btnInfo.Location = new System.Drawing.Point(268, 4);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(4);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(25, 25);
            this.btnInfo.TabIndex = 1;
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.Info_Click);
            // 
            // msHeader
            // 
            this.msHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.centerPtToolStripMenuItem,
            this.isPlanarToolStripMenuItem,
            this.isClosedToolStripMenuItem,
            this.commentsToolStripMenuItem});
            this.msHeader.Name = "msHeader";
            this.msHeader.ShowCheckMargin = true;
            this.msHeader.ShowImageMargin = false;
            this.msHeader.Size = new System.Drawing.Size(134, 466);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(120, 22);
            this.toolStripMenuItem2.Text = "#";
            // 
            // gUIDToolStripMenuItem
            // 
            this.gUIDToolStripMenuItem.CheckOnClick = true;
            this.gUIDToolStripMenuItem.Name = "gUIDToolStripMenuItem";
            this.gUIDToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.gUIDToolStripMenuItem.Text = "GUID";
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.CheckOnClick = true;
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.nameToolStripMenuItem.Text = "Name";
            // 
            // layerToolStripMenuItem
            // 
            this.layerToolStripMenuItem.Name = "layerToolStripMenuItem";
            this.layerToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.layerToolStripMenuItem.Text = "Layer";
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.colorToolStripMenuItem.Text = "Color";
            // 
            // lineTypeToolStripMenuItem
            // 
            this.lineTypeToolStripMenuItem.Name = "lineTypeToolStripMenuItem";
            this.lineTypeToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.lineTypeToolStripMenuItem.Text = "LineType";
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "GUID";
            this.columnHeader1.Width = 97;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Name";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 126;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "LineType";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 148;
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.typeToolStripMenuItem.Text = "Type";
            // 
            // printColorToolStripMenuItem
            // 
            this.printColorToolStripMenuItem.Name = "printColorToolStripMenuItem";
            this.printColorToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.printColorToolStripMenuItem.Text = "PrintColor";
            // 
            // printWidthToolStripMenuItem
            // 
            this.printWidthToolStripMenuItem.Name = "printWidthToolStripMenuItem";
            this.printWidthToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.printWidthToolStripMenuItem.Text = "PrintWidth";
            // 
            // materialToolStripMenuItem
            // 
            this.materialToolStripMenuItem.Name = "materialToolStripMenuItem";
            this.materialToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.materialToolStripMenuItem.Text = "Material";
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.lengthToolStripMenuItem.Text = "Length";
            // 
            // areaToolStripMenuItem
            // 
            this.areaToolStripMenuItem.Name = "areaToolStripMenuItem";
            this.areaToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.areaToolStripMenuItem.Text = "Area";
            // 
            // volumeToolStripMenuItem
            // 
            this.volumeToolStripMenuItem.Name = "volumeToolStripMenuItem";
            this.volumeToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.volumeToolStripMenuItem.Text = "Volume";
            // 
            // numPtsToolStripMenuItem
            // 
            this.numPtsToolStripMenuItem.Name = "numPtsToolStripMenuItem";
            this.numPtsToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.numPtsToolStripMenuItem.Text = "NumPts";
            // 
            // numEdgesToolStripMenuItem
            // 
            this.numEdgesToolStripMenuItem.Name = "numEdgesToolStripMenuItem";
            this.numEdgesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.numEdgesToolStripMenuItem.Text = "NumEdges";
            // 
            // numFacesToolStripMenuItem
            // 
            this.numFacesToolStripMenuItem.Name = "numFacesToolStripMenuItem";
            this.numFacesToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.numFacesToolStripMenuItem.Text = "NumFaces";
            // 
            // degreeToolStripMenuItem
            // 
            this.degreeToolStripMenuItem.Name = "degreeToolStripMenuItem";
            this.degreeToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.degreeToolStripMenuItem.Text = "Degree";
            // 
            // centerPtToolStripMenuItem
            // 
            this.centerPtToolStripMenuItem.Name = "centerPtToolStripMenuItem";
            this.centerPtToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.centerPtToolStripMenuItem.Text = "CenterPt";
            // 
            // isPlanarToolStripMenuItem
            // 
            this.isPlanarToolStripMenuItem.Name = "isPlanarToolStripMenuItem";
            this.isPlanarToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.isPlanarToolStripMenuItem.Text = "IsPlanar";
            // 
            // isClosedToolStripMenuItem
            // 
            this.isClosedToolStripMenuItem.Name = "isClosedToolStripMenuItem";
            this.isClosedToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.isClosedToolStripMenuItem.Text = "IsClosed";
            // 
            // commentsToolStripMenuItem
            // 
            this.commentsToolStripMenuItem.Name = "commentsToolStripMenuItem";
            this.commentsToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.commentsToolStripMenuItem.Text = "Comments";
            // 
            // DockPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
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
            this.Name = "DockPanel";
            this.Size = new System.Drawing.Size(548, 631);
            this.msHeader.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip msHeader;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem gUIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem layerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem colorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lineTypeToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
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
        private System.Windows.Forms.ToolStripMenuItem centerPtToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isPlanarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem isClosedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentsToolStripMenuItem;
    }
}

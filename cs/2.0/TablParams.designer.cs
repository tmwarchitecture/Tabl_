namespace Tabl_
{
    partial class TablParams
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbThousand = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudDP = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbClrComma = new System.Windows.Forms.RadioButton();
            this.rbClrDash = new System.Windows.Forms.RadioButton();
            this.rbClrName = new System.Windows.Forms.RadioButton();
            this.rbAutoUpdate = new System.Windows.Forms.RadioButton();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tbUnitName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nudUnitScale = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ttipSettings = new System.Windows.Forms.ToolTip(this.components);
            this.btnMarkerClr = new System.Windows.Forms.Button();
            this.cbEnableMarker = new System.Windows.Forms.CheckBox();
            this.nudWireWt = new System.Windows.Forms.NumericUpDown();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbThreaded = new System.Windows.Forms.CheckBox();
            this.cbExHdrs = new System.Windows.Forms.CheckBox();
            this.cbSeeTot = new System.Windows.Forms.CheckBox();
            this.cbSeeUnit = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.clrPicker = new System.Windows.Forms.ColorDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDP)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnitScale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWireWt)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbThousand);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudDP);
            this.groupBox1.Location = new System.Drawing.Point(245, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 82);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Number Format";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Thousands Separator";
            // 
            // cbThousand
            // 
            this.cbThousand.FormattingEnabled = true;
            this.cbThousand.Items.AddRange(new object[] {
            "comma",
            "dot",
            "space",
            "None"});
            this.cbThousand.Location = new System.Drawing.Point(144, 49);
            this.cbThousand.Name = "cbThousand";
            this.cbThousand.Size = new System.Drawing.Size(76, 23);
            this.cbThousand.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Decimal Places";
            // 
            // nudDP
            // 
            this.nudDP.Location = new System.Drawing.Point(143, 22);
            this.nudDP.Name = "nudDP";
            this.nudDP.Size = new System.Drawing.Size(77, 21);
            this.nudDP.TabIndex = 0;
            this.nudDP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudDP.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbClrComma);
            this.groupBox2.Controls.Add(this.rbClrDash);
            this.groupBox2.Controls.Add(this.rbClrName);
            this.groupBox2.Location = new System.Drawing.Point(12, 159);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(227, 52);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Color Format";
            // 
            // rbClrComma
            // 
            this.rbClrComma.AutoSize = true;
            this.rbClrComma.Location = new System.Drawing.Point(157, 22);
            this.rbClrComma.Name = "rbClrComma";
            this.rbClrComma.Size = new System.Drawing.Size(57, 19);
            this.rbClrComma.TabIndex = 2;
            this.rbClrComma.TabStop = true;
            this.rbClrComma.Text = "R,G,B";
            this.rbClrComma.UseVisualStyleBackColor = true;
            // 
            // rbClrDash
            // 
            this.rbClrDash.AutoSize = true;
            this.rbClrDash.Location = new System.Drawing.Point(87, 22);
            this.rbClrDash.Name = "rbClrDash";
            this.rbClrDash.Size = new System.Drawing.Size(59, 19);
            this.rbClrDash.TabIndex = 1;
            this.rbClrDash.TabStop = true;
            this.rbClrDash.Text = "R-G-B";
            this.rbClrDash.UseVisualStyleBackColor = true;
            // 
            // rbClrName
            // 
            this.rbClrName.AutoSize = true;
            this.rbClrName.Location = new System.Drawing.Point(10, 22);
            this.rbClrName.Name = "rbClrName";
            this.rbClrName.Size = new System.Drawing.Size(59, 19);
            this.rbClrName.TabIndex = 0;
            this.rbClrName.TabStop = true;
            this.rbClrName.Text = "Name";
            this.rbClrName.UseVisualStyleBackColor = true;
            // 
            // rbAutoUpdate
            // 
            this.rbAutoUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbAutoUpdate.AutoSize = true;
            this.rbAutoUpdate.Location = new System.Drawing.Point(172, 76);
            this.rbAutoUpdate.Name = "rbAutoUpdate";
            this.rbAutoUpdate.Size = new System.Drawing.Size(49, 19);
            this.rbAutoUpdate.TabIndex = 1;
            this.rbAutoUpdate.TabStop = true;
            this.rbAutoUpdate.Text = "Auto";
            this.ttipSettings.SetToolTip(this.rbAutoUpdate, "any change in Rhino will trigger data refresh\r\nbeware of computing time");
            this.rbAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            this.rbManual.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbManual.AutoSize = true;
            this.rbManual.Location = new System.Drawing.Point(99, 76);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(67, 19);
            this.rbManual.TabIndex = 0;
            this.rbManual.TabStop = true;
            this.rbManual.Text = "Manual";
            this.rbManual.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tbUnitName);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.nudUnitScale);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Location = new System.Drawing.Point(245, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 78);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Custom Units";
            // 
            // tbUnitName
            // 
            this.tbUnitName.Location = new System.Drawing.Point(143, 46);
            this.tbUnitName.Name = "tbUnitName";
            this.tbUnitName.Size = new System.Drawing.Size(76, 21);
            this.tbUnitName.TabIndex = 3;
            this.ttipSettings.SetToolTip(this.tbUnitName, "leave blank to use default units");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Custom Unit Name";
            // 
            // nudUnitScale
            // 
            this.nudUnitScale.DecimalPlaces = 3;
            this.nudUnitScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudUnitScale.Location = new System.Drawing.Point(142, 20);
            this.nudUnitScale.Name = "nudUnitScale";
            this.nudUnitScale.Size = new System.Drawing.Size(77, 21);
            this.nudUnitScale.TabIndex = 1;
            this.ttipSettings.SetToolTip(this.nudUnitScale, "multiplies all numbers");
            this.nudUnitScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Scale Units";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(310, 184);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(78, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(394, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(78, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // btnMarkerClr
            // 
            this.btnMarkerClr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMarkerClr.BackColor = System.Drawing.Color.HotPink;
            this.btnMarkerClr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMarkerClr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarkerClr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMarkerClr.Location = new System.Drawing.Point(172, 109);
            this.btnMarkerClr.Name = "btnMarkerClr";
            this.btnMarkerClr.Size = new System.Drawing.Size(49, 21);
            this.btnMarkerClr.TabIndex = 1;
            this.btnMarkerClr.Text = "Color";
            this.ttipSettings.SetToolTip(this.btnMarkerClr, "viewport marker color");
            this.btnMarkerClr.UseVisualStyleBackColor = false;
            this.btnMarkerClr.Click += new System.EventHandler(this.MarkerClr_Click);
            // 
            // cbEnableMarker
            // 
            this.cbEnableMarker.AutoSize = true;
            this.cbEnableMarker.Checked = true;
            this.cbEnableMarker.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnableMarker.Location = new System.Drawing.Point(6, 111);
            this.cbEnableMarker.Name = "cbEnableMarker";
            this.cbEnableMarker.Size = new System.Drawing.Size(105, 19);
            this.cbEnableMarker.TabIndex = 4;
            this.cbEnableMarker.Text = "Mark Selected";
            this.ttipSettings.SetToolTip(this.cbEnableMarker, "marks what\'s selected in Tabl_ in the Rhino viewport");
            this.cbEnableMarker.UseVisualStyleBackColor = true;
            this.cbEnableMarker.CheckedChanged += new System.EventHandler(this.Marker_CheckedChanged);
            // 
            // nudWireWt
            // 
            this.nudWireWt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudWireWt.Location = new System.Drawing.Point(126, 109);
            this.nudWireWt.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.nudWireWt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudWireWt.Name = "nudWireWt";
            this.nudWireWt.Size = new System.Drawing.Size(40, 21);
            this.nudWireWt.TabIndex = 2;
            this.ttipSettings.SetToolTip(this.nudWireWt, "marker wire thickness");
            this.nudWireWt.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbThreaded);
            this.groupBox5.Controls.Add(this.cbEnableMarker);
            this.groupBox5.Controls.Add(this.cbExHdrs);
            this.groupBox5.Controls.Add(this.rbAutoUpdate);
            this.groupBox5.Controls.Add(this.cbSeeTot);
            this.groupBox5.Controls.Add(this.rbManual);
            this.groupBox5.Controls.Add(this.cbSeeUnit);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.btnMarkerClr);
            this.groupBox5.Controls.Add(this.nudWireWt);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(227, 141);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Spreadsheet";
            // 
            // cbThreaded
            // 
            this.cbThreaded.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbThreaded.AutoSize = true;
            this.cbThreaded.Location = new System.Drawing.Point(110, 45);
            this.cbThreaded.Name = "cbThreaded";
            this.cbThreaded.Size = new System.Drawing.Size(79, 19);
            this.cbThreaded.TabIndex = 10;
            this.cbThreaded.Text = "Threaded";
            this.ttipSettings.SetToolTip(this.cbThreaded, "beware of overhead computing cost\r\ncheck if items are vast in number");
            this.cbThreaded.UseVisualStyleBackColor = true;
            // 
            // cbExHdrs
            // 
            this.cbExHdrs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbExHdrs.AutoSize = true;
            this.cbExHdrs.Location = new System.Drawing.Point(110, 20);
            this.cbExHdrs.Name = "cbExHdrs";
            this.cbExHdrs.Size = new System.Drawing.Size(111, 19);
            this.cbExHdrs.TabIndex = 9;
            this.cbExHdrs.Text = "Export Headers";
            this.cbExHdrs.UseVisualStyleBackColor = true;
            // 
            // cbSeeTot
            // 
            this.cbSeeTot.AutoSize = true;
            this.cbSeeTot.Location = new System.Drawing.Point(6, 45);
            this.cbSeeTot.Name = "cbSeeTot";
            this.cbSeeTot.Size = new System.Drawing.Size(87, 19);
            this.cbSeeTot.TabIndex = 8;
            this.cbSeeTot.Text = "Show Total";
            this.cbSeeTot.UseVisualStyleBackColor = true;
            // 
            // cbSeeUnit
            // 
            this.cbSeeUnit.AutoSize = true;
            this.cbSeeUnit.Location = new System.Drawing.Point(6, 20);
            this.cbSeeUnit.Name = "cbSeeUnit";
            this.cbSeeUnit.Size = new System.Drawing.Size(88, 19);
            this.cbSeeUnit.TabIndex = 7;
            this.cbSeeUnit.Text = "Show Units";
            this.cbSeeUnit.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 78);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 15);
            this.label7.TabIndex = 3;
            this.label7.Text = "Refresh";
            // 
            // TablParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(484, 223);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TablParams";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDP)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnitScale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWireWt)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudDP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbThousand;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbClrComma;
        private System.Windows.Forms.RadioButton rbClrDash;
        private System.Windows.Forms.RadioButton rbClrName;
        private System.Windows.Forms.RadioButton rbAutoUpdate;
        private System.Windows.Forms.RadioButton rbManual;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tbUnitName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudUnitScale;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip ttipSettings;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.NumericUpDown nudWireWt;
        private System.Windows.Forms.Button btnMarkerClr;
        private System.Windows.Forms.ColorDialog clrPicker;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox cbEnableMarker;
        private System.Windows.Forms.CheckBox cbThreaded;
        private System.Windows.Forms.CheckBox cbExHdrs;
        private System.Windows.Forms.CheckBox cbSeeTot;
        private System.Windows.Forms.CheckBox cbSeeUnit;
    }
}
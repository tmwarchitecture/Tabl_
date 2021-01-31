namespace Tabl_
{
    partial class PlaceSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaceSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.rbFitCol = new System.Windows.Forms.RadioButton();
            this.rbEvenTablW = new System.Windows.Forms.RadioButton();
            this.rbFitTablW = new System.Windows.Forms.RadioButton();
            this.tbFitData = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbXZ = new System.Windows.Forms.RadioButton();
            this.rbPickPlane = new System.Windows.Forms.RadioButton();
            this.rbYZ = new System.Windows.Forms.RadioButton();
            this.rbXY = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.tbFontSize = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.labelFontBtn = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.nudPad = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPad)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.rbFitCol);
            this.groupBox1.Controls.Add(this.rbEvenTablW);
            this.groupBox1.Controls.Add(this.rbFitTablW);
            this.groupBox1.Controls.Add(this.tbFitData);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(151, 142);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Column";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Width";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(60, 111);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(85, 20);
            this.textBox1.TabIndex = 4;
            this.toolTip1.SetToolTip(this.textBox1, "in document unit");
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // rbFitCol
            // 
            this.rbFitCol.AutoSize = true;
            this.rbFitCol.Location = new System.Drawing.Point(6, 88);
            this.rbFitCol.Name = "rbFitCol";
            this.rbFitCol.Size = new System.Drawing.Size(115, 17);
            this.rbFitCol.TabIndex = 3;
            this.rbFitCol.Text = "Fixed column width";
            this.rbFitCol.UseVisualStyleBackColor = true;
            this.rbFitCol.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
            // 
            // rbEvenTablW
            // 
            this.rbEvenTablW.AutoSize = true;
            this.rbEvenTablW.Enabled = false;
            this.rbEvenTablW.Location = new System.Drawing.Point(6, 65);
            this.rbEvenTablW.Name = "rbEvenTablW";
            this.rbEvenTablW.Size = new System.Drawing.Size(142, 17);
            this.rbEvenTablW.TabIndex = 2;
            this.rbEvenTablW.Text = "Evenly divide table width";
            this.toolTip1.SetToolTip(this.rbEvenTablW, "no implementation yet\r\nwill fit to data");
            this.rbEvenTablW.UseVisualStyleBackColor = true;
            this.rbEvenTablW.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // rbFitTablW
            // 
            this.rbFitTablW.AutoSize = true;
            this.rbFitTablW.Enabled = false;
            this.rbFitTablW.Location = new System.Drawing.Point(6, 42);
            this.rbFitTablW.Name = "rbFitTablW";
            this.rbFitTablW.Size = new System.Drawing.Size(102, 17);
            this.rbFitTablW.TabIndex = 1;
            this.rbFitTablW.Text = "Fit to table width";
            this.toolTip1.SetToolTip(this.rbFitTablW, "no implementation yet\r\nwill fit to data");
            this.rbFitTablW.UseVisualStyleBackColor = true;
            this.rbFitTablW.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // tbFitData
            // 
            this.tbFitData.AutoSize = true;
            this.tbFitData.Checked = true;
            this.tbFitData.Location = new System.Drawing.Point(6, 19);
            this.tbFitData.Name = "tbFitData";
            this.tbFitData.Size = new System.Drawing.Size(72, 17);
            this.tbFitData.TabIndex = 0;
            this.tbFitData.TabStop = true;
            this.tbFitData.Text = "Fit to data";
            this.toolTip1.SetToolTip(this.tbFitData, "recommended");
            this.tbFitData.UseVisualStyleBackColor = true;
            this.tbFitData.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbXZ);
            this.groupBox2.Controls.Add(this.rbPickPlane);
            this.groupBox2.Controls.Add(this.rbYZ);
            this.groupBox2.Controls.Add(this.rbXY);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tbFontSize);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.labelFontBtn);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.nudPad);
            this.groupBox2.Location = new System.Drawing.Point(169, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 142);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cell";
            // 
            // rbXZ
            // 
            this.rbXZ.AutoSize = true;
            this.rbXZ.Location = new System.Drawing.Point(99, 112);
            this.rbXZ.Name = "rbXZ";
            this.rbXZ.Size = new System.Drawing.Size(39, 17);
            this.rbXZ.TabIndex = 9;
            this.rbXZ.TabStop = true;
            this.rbXZ.Text = "XZ";
            this.rbXZ.UseVisualStyleBackColor = true;
            this.rbXZ.CheckedChanged += new System.EventHandler(this.radioButton7_CheckedChanged);
            // 
            // rbPickPlane
            // 
            this.rbPickPlane.AutoSize = true;
            this.rbPickPlane.Location = new System.Drawing.Point(99, 88);
            this.rbPickPlane.Name = "rbPickPlane";
            this.rbPickPlane.Size = new System.Drawing.Size(46, 17);
            this.rbPickPlane.TabIndex = 9;
            this.rbPickPlane.TabStop = true;
            this.rbPickPlane.Text = "Pick";
            this.toolTip1.SetToolTip(this.rbPickPlane, "table will be placed at click location\r\nand parallel to this plane");
            this.rbPickPlane.UseVisualStyleBackColor = true;
            this.rbPickPlane.CheckedChanged += new System.EventHandler(this.radioButton8_CheckedChanged);
            // 
            // rbYZ
            // 
            this.rbYZ.AutoSize = true;
            this.rbYZ.Location = new System.Drawing.Point(54, 112);
            this.rbYZ.Name = "rbYZ";
            this.rbYZ.Size = new System.Drawing.Size(39, 17);
            this.rbYZ.TabIndex = 8;
            this.rbYZ.TabStop = true;
            this.rbYZ.Text = "YZ";
            this.rbYZ.UseVisualStyleBackColor = true;
            this.rbYZ.CheckedChanged += new System.EventHandler(this.radioButton6_CheckedChanged);
            // 
            // rbXY
            // 
            this.rbXY.AutoSize = true;
            this.rbXY.Location = new System.Drawing.Point(9, 112);
            this.rbXY.Name = "rbXY";
            this.rbXY.Size = new System.Drawing.Size(39, 17);
            this.rbXY.TabIndex = 7;
            this.rbXY.TabStop = true;
            this.rbXY.Text = "XY";
            this.rbXY.UseVisualStyleBackColor = true;
            this.rbXY.CheckedChanged += new System.EventHandler(this.radioButton5_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Base Plane";
            // 
            // tbFontSize
            // 
            this.tbFontSize.Location = new System.Drawing.Point(60, 64);
            this.tbFontSize.Name = "tbFontSize";
            this.tbFontSize.Size = new System.Drawing.Size(85, 20);
            this.tbFontSize.TabIndex = 5;
            this.tbFontSize.Text = "10";
            this.tbFontSize.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Font Size";
            // 
            // labelFontBtn
            // 
            this.labelFontBtn.AutoSize = true;
            this.labelFontBtn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFontBtn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelFontBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelFontBtn.Location = new System.Drawing.Point(60, 44);
            this.labelFontBtn.Name = "labelFontBtn";
            this.labelFontBtn.Size = new System.Drawing.Size(29, 15);
            this.labelFontBtn.TabIndex = 3;
            this.labelFontBtn.Text = "Arial";
            this.labelFontBtn.Click += new System.EventHandler(this.label4_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Font";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Padding";
            // 
            // nudPad
            // 
            this.nudPad.Location = new System.Drawing.Point(60, 19);
            this.nudPad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPad.Name = "nudPad";
            this.nudPad.Size = new System.Drawing.Size(85, 20);
            this.nudPad.TabIndex = 0;
            this.nudPad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPad.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 160);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(78, 160);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(60, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PlaceSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 194);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlaceSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Place Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton rbFitCol;
        private System.Windows.Forms.RadioButton rbEvenTablW;
        private System.Windows.Forms.RadioButton rbFitTablW;
        private System.Windows.Forms.RadioButton tbFitData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudPad;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FontDialog fontDialog1;
        private System.Windows.Forms.Label labelFontBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbFontSize;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rbPickPlane;
        private System.Windows.Forms.RadioButton rbYZ;
        private System.Windows.Forms.RadioButton rbXY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton rbXZ;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
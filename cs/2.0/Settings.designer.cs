namespace Tabl_
{
    partial class Settings
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
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
            this.chklTablDisplay = new System.Windows.Forms.CheckedListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDP)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnitScale)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbThousand);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.nudDP);
            this.groupBox1.Location = new System.Drawing.Point(245, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(227, 86);
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
            "comma \",\"",
            "dot \".\"",
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
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
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
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.rbAutoUpdate);
            this.groupBox3.Controls.Add(this.rbManual);
            this.groupBox3.Location = new System.Drawing.Point(12, 71);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(227, 52);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Update";
            // 
            // rbAutoUpdate
            // 
            this.rbAutoUpdate.AutoSize = true;
            this.rbAutoUpdate.Location = new System.Drawing.Point(87, 22);
            this.rbAutoUpdate.Name = "rbAutoUpdate";
            this.rbAutoUpdate.Size = new System.Drawing.Size(79, 19);
            this.rbAutoUpdate.TabIndex = 1;
            this.rbAutoUpdate.TabStop = true;
            this.rbAutoUpdate.Text = "Automatic";
            this.ttipSettings.SetToolTip(this.rbAutoUpdate, "any change in Rhino will trigger data refresh\r\nbeware of computing time");
            this.rbAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // rbManual
            // 
            this.rbManual.AutoSize = true;
            this.rbManual.Location = new System.Drawing.Point(10, 22);
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
            this.groupBox4.Location = new System.Drawing.Point(12, 130);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(227, 81);
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
            this.nudUnitScale.DecimalPlaces = 5;
            this.nudUnitScale.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudUnitScale.Location = new System.Drawing.Point(143, 16);
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
            this.label3.Location = new System.Drawing.Point(7, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Scale Units";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(245, 184);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(111, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.OK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(361, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(111, 27);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // chklTablDisplay
            // 
            this.chklTablDisplay.BackColor = System.Drawing.SystemColors.Menu;
            this.chklTablDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chklTablDisplay.CheckOnClick = true;
            this.chklTablDisplay.ColumnWidth = 110;
            this.chklTablDisplay.FormattingEnabled = true;
            this.chklTablDisplay.Items.AddRange(new object[] {
            "Show units",
            "Show total",
            "Export headers",
            "Threaded"});
            this.chklTablDisplay.Location = new System.Drawing.Point(6, 20);
            this.chklTablDisplay.MultiColumn = true;
            this.chklTablDisplay.Name = "chklTablDisplay";
            this.chklTablDisplay.Size = new System.Drawing.Size(220, 32);
            this.chklTablDisplay.TabIndex = 0;
            this.ttipSettings.SetToolTip(this.chklTablDisplay, "enabling thread increases overhead\r\nfaster only if there are myriad of line items" +
        "");
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chklTablDisplay);
            this.groupBox5.Location = new System.Drawing.Point(245, 104);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(227, 74);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Spreadsheet";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 220);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudDP)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudUnitScale)).EndInit();
            this.groupBox5.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox3;
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
        private System.Windows.Forms.CheckedListBox chklTablDisplay;
    }
}
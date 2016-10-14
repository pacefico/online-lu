namespace OnlineLU.Client.WinForms
{
    partial class Principal
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
            this.tabPrincipal = new System.Windows.Forms.TabControl();
            this.tabProcess = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.numTime = new System.Windows.Forms.NumericUpDown();
            this.txtRange = new System.Windows.Forms.TextBox();
            this.cbCreate = new System.Windows.Forms.CheckBox();
            this.cbRecursive = new System.Windows.Forms.CheckBox();
            this.btClear = new System.Windows.Forms.Button();
            this.btActivate = new System.Windows.Forms.Button();
            this.cbSummary = new System.Windows.Forms.CheckBox();
            this.listProcessing = new System.Windows.Forms.ListBox();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.listHardware = new System.Windows.Forms.ListBox();
            this.tabNewFile = new System.Windows.Forms.TabPage();
            this.lblPrecisionValue = new System.Windows.Forms.Label();
            this.numPrecision = new System.Windows.Forms.NumericUpDown();
            this.lblPrecision = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblRange = new System.Windows.Forms.Label();
            this.numRange = new System.Windows.Forms.NumericUpDown();
            this.tabPrincipal.SuspendLayout();
            this.tabProcess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).BeginInit();
            this.tabInfo.SuspendLayout();
            this.tabNewFile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRange)).BeginInit();
            this.SuspendLayout();
            // 
            // tabPrincipal
            // 
            this.tabPrincipal.Controls.Add(this.tabProcess);
            this.tabPrincipal.Controls.Add(this.tabInfo);
            this.tabPrincipal.Controls.Add(this.tabNewFile);
            this.tabPrincipal.Location = new System.Drawing.Point(3, 4);
            this.tabPrincipal.Name = "tabPrincipal";
            this.tabPrincipal.SelectedIndex = 0;
            this.tabPrincipal.Size = new System.Drawing.Size(593, 403);
            this.tabPrincipal.TabIndex = 0;
            // 
            // tabProcess
            // 
            this.tabProcess.Controls.Add(this.label1);
            this.tabProcess.Controls.Add(this.numTime);
            this.tabProcess.Controls.Add(this.txtRange);
            this.tabProcess.Controls.Add(this.cbCreate);
            this.tabProcess.Controls.Add(this.cbRecursive);
            this.tabProcess.Controls.Add(this.btClear);
            this.tabProcess.Controls.Add(this.btActivate);
            this.tabProcess.Controls.Add(this.cbSummary);
            this.tabProcess.Controls.Add(this.listProcessing);
            this.tabProcess.Location = new System.Drawing.Point(4, 22);
            this.tabProcess.Name = "tabProcess";
            this.tabProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabProcess.Size = new System.Drawing.Size(585, 377);
            this.tabProcess.TabIndex = 1;
            this.tabProcess.Text = "Processamento";
            this.tabProcess.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 349);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "ms";
            // 
            // numTime
            // 
            this.numTime.Location = new System.Drawing.Point(168, 347);
            this.numTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numTime.Name = "numTime";
            this.numTime.Size = new System.Drawing.Size(72, 20);
            this.numTime.TabIndex = 5;
            // 
            // txtRange
            // 
            this.txtRange.Location = new System.Drawing.Point(282, 346);
            this.txtRange.Name = "txtRange";
            this.txtRange.Size = new System.Drawing.Size(39, 20);
            this.txtRange.TabIndex = 4;
            this.txtRange.Text = "1000";
            this.txtRange.Visible = false;
            // 
            // cbCreate
            // 
            this.cbCreate.AutoSize = true;
            this.cbCreate.Location = new System.Drawing.Point(327, 348);
            this.cbCreate.Name = "cbCreate";
            this.cbCreate.Size = new System.Drawing.Size(63, 17);
            this.cbCreate.TabIndex = 3;
            this.cbCreate.Text = "Create?";
            this.cbCreate.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.cbCreate.UseVisualStyleBackColor = true;
            this.cbCreate.Visible = false;
            // 
            // cbRecursive
            // 
            this.cbRecursive.AutoSize = true;
            this.cbRecursive.Checked = true;
            this.cbRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRecursive.Location = new System.Drawing.Point(108, 347);
            this.cbRecursive.Name = "cbRecursive";
            this.cbRecursive.Size = new System.Drawing.Size(59, 17);
            this.cbRecursive.TabIndex = 2;
            this.cbRecursive.Text = "a cada";
            this.cbRecursive.UseVisualStyleBackColor = true;
            this.cbRecursive.CheckedChanged += new System.EventHandler(this.cbRecursive_CheckedChanged);
            // 
            // btClear
            // 
            this.btClear.Location = new System.Drawing.Point(429, 340);
            this.btClear.Name = "btClear";
            this.btClear.Size = new System.Drawing.Size(75, 23);
            this.btClear.TabIndex = 2;
            this.btClear.Text = "Limpar";
            this.btClear.UseVisualStyleBackColor = true;
            this.btClear.Click += new System.EventHandler(this.btClear_Click);
            // 
            // btActivate
            // 
            this.btActivate.Location = new System.Drawing.Point(6, 344);
            this.btActivate.Name = "btActivate";
            this.btActivate.Size = new System.Drawing.Size(96, 23);
            this.btActivate.TabIndex = 1;
            this.btActivate.Text = "Ativar";
            this.btActivate.UseVisualStyleBackColor = true;
            this.btActivate.Click += new System.EventHandler(this.Activate_Click);
            // 
            // cbSummary
            // 
            this.cbSummary.AutoSize = true;
            this.cbSummary.Location = new System.Drawing.Point(510, 344);
            this.cbSummary.Name = "cbSummary";
            this.cbSummary.Size = new System.Drawing.Size(68, 17);
            this.cbSummary.TabIndex = 1;
            this.cbSummary.Text = "Detalhes";
            this.cbSummary.UseVisualStyleBackColor = true;
            this.cbSummary.CheckedChanged += new System.EventHandler(this.cbSummary_CheckedChanged);
            // 
            // listProcessing
            // 
            this.listProcessing.FormattingEnabled = true;
            this.listProcessing.Location = new System.Drawing.Point(3, 2);
            this.listProcessing.Name = "listProcessing";
            this.listProcessing.Size = new System.Drawing.Size(579, 329);
            this.listProcessing.TabIndex = 0;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.listHardware);
            this.tabInfo.Location = new System.Drawing.Point(4, 22);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Size = new System.Drawing.Size(585, 377);
            this.tabInfo.TabIndex = 3;
            this.tabInfo.Text = "Hardware";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // listHardware
            // 
            this.listHardware.FormattingEnabled = true;
            this.listHardware.Location = new System.Drawing.Point(4, 4);
            this.listHardware.Name = "listHardware";
            this.listHardware.Size = new System.Drawing.Size(578, 368);
            this.listHardware.TabIndex = 0;
            // 
            // tabNewFile
            // 
            this.tabNewFile.Controls.Add(this.lblPrecisionValue);
            this.tabNewFile.Controls.Add(this.numPrecision);
            this.tabNewFile.Controls.Add(this.lblPrecision);
            this.tabNewFile.Controls.Add(this.txtLocation);
            this.tabNewFile.Controls.Add(this.btnGenerate);
            this.tabNewFile.Controls.Add(this.lblDestination);
            this.tabNewFile.Controls.Add(this.lblRange);
            this.tabNewFile.Controls.Add(this.numRange);
            this.tabNewFile.Location = new System.Drawing.Point(4, 22);
            this.tabNewFile.Name = "tabNewFile";
            this.tabNewFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabNewFile.Size = new System.Drawing.Size(585, 377);
            this.tabNewFile.TabIndex = 4;
            this.tabNewFile.Text = "Gerar";
            this.tabNewFile.UseVisualStyleBackColor = true;
            // 
            // lblPrecisionValue
            // 
            this.lblPrecisionValue.AutoSize = true;
            this.lblPrecisionValue.Location = new System.Drawing.Point(218, 37);
            this.lblPrecisionValue.Name = "lblPrecisionValue";
            this.lblPrecisionValue.Size = new System.Drawing.Size(79, 13);
            this.lblPrecisionValue.TabIndex = 7;
            this.lblPrecisionValue.Text = "casas decimais";
            // 
            // numPrecision
            // 
            this.numPrecision.Enabled = false;
            this.numPrecision.Location = new System.Drawing.Point(162, 35);
            this.numPrecision.Name = "numPrecision";
            this.numPrecision.Size = new System.Drawing.Size(45, 20);
            this.numPrecision.TabIndex = 6;
            this.numPrecision.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // lblPrecision
            // 
            this.lblPrecision.AutoSize = true;
            this.lblPrecision.Location = new System.Drawing.Point(159, 19);
            this.lblPrecision.Name = "lblPrecision";
            this.lblPrecision.Size = new System.Drawing.Size(48, 13);
            this.lblPrecision.TabIndex = 5;
            this.lblPrecision.Text = "Precisão";
            // 
            // txtLocation
            // 
            this.txtLocation.Enabled = false;
            this.txtLocation.Location = new System.Drawing.Point(9, 86);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(288, 20);
            this.txtLocation.TabIndex = 4;
            this.txtLocation.Text = "c:\\temp\\0.lu";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(9, 122);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Gerar";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(6, 69);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(33, 13);
            this.lblDestination.TabIndex = 2;
            this.lblDestination.Text = "Local";
            // 
            // lblRange
            // 
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(6, 19);
            this.lblRange.Name = "lblRange";
            this.lblRange.Size = new System.Drawing.Size(38, 13);
            this.lblRange.TabIndex = 1;
            this.lblRange.Text = "Ordem";
            // 
            // numRange
            // 
            this.numRange.Location = new System.Drawing.Point(9, 35);
            this.numRange.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRange.Name = "numRange";
            this.numRange.Size = new System.Drawing.Size(120, 20);
            this.numRange.TabIndex = 0;
            this.numRange.ValueChanged += new System.EventHandler(this.numRange_ValueChanged);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 408);
            this.Controls.Add(this.tabPrincipal);
            this.Name = "Principal";
            this.Text = "Nó de Processamento";
            this.Load += new System.EventHandler(this.Principal_Load);
            this.tabPrincipal.ResumeLayout(false);
            this.tabProcess.ResumeLayout(false);
            this.tabProcess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTime)).EndInit();
            this.tabInfo.ResumeLayout(false);
            this.tabNewFile.ResumeLayout(false);
            this.tabNewFile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRange)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabPrincipal;
        private System.Windows.Forms.TabPage tabProcess;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.CheckBox cbSummary;
        private System.Windows.Forms.ListBox listProcessing;
        private System.Windows.Forms.Button btActivate;
        private System.Windows.Forms.Button btClear;
        private System.Windows.Forms.CheckBox cbRecursive;
        private System.Windows.Forms.ListBox listHardware;
        private System.Windows.Forms.TextBox txtRange;
        private System.Windows.Forms.CheckBox cbCreate;
        private System.Windows.Forms.TabPage tabNewFile;
        private System.Windows.Forms.Label lblPrecisionValue;
        private System.Windows.Forms.NumericUpDown numPrecision;
        private System.Windows.Forms.Label lblPrecision;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.NumericUpDown numRange;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTime;
    }
}


namespace GradesApplication
{
    partial class ReportForm
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
            this.btnRunReport = new System.Windows.Forms.Button();
            this.comboReportCombo = new System.Windows.Forms.ComboBox();
            this.lblReportLbl = new System.Windows.Forms.Label();
            this.tbReportParam = new System.Windows.Forms.TextBox();
            this.lblreportParameter = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnRunReport
            // 
            this.btnRunReport.Location = new System.Drawing.Point(26, 138);
            this.btnRunReport.Name = "btnRunReport";
            this.btnRunReport.Size = new System.Drawing.Size(559, 23);
            this.btnRunReport.TabIndex = 0;
            this.btnRunReport.Text = "Run Report Now";
            this.btnRunReport.UseVisualStyleBackColor = true;
            this.btnRunReport.Click += new System.EventHandler(this.btnRunReport_Click);
            // 
            // comboReportCombo
            // 
            this.comboReportCombo.FormattingEnabled = true;
            this.comboReportCombo.Items.AddRange(new object[] {
            "Run Report on a StudentID",
            "Run Report on a SubjectName"});
            this.comboReportCombo.Location = new System.Drawing.Point(26, 27);
            this.comboReportCombo.Name = "comboReportCombo";
            this.comboReportCombo.Size = new System.Drawing.Size(280, 21);
            this.comboReportCombo.TabIndex = 1;
            this.comboReportCombo.Text = "---Please pick a report to run-----";
            this.comboReportCombo.SelectedIndexChanged += new System.EventHandler(this.comboReportCombo_SelectedIndexChanged);
            // 
            // lblReportLbl
            // 
            this.lblReportLbl.AutoSize = true;
            this.lblReportLbl.Location = new System.Drawing.Point(341, 27);
            this.lblReportLbl.Name = "lblReportLbl";
            this.lblReportLbl.Size = new System.Drawing.Size(182, 13);
            this.lblReportLbl.TabIndex = 2;
            this.lblReportLbl.Text = "Please Choose Type of Report to run";
            // 
            // tbReportParam
            // 
            this.tbReportParam.Location = new System.Drawing.Point(26, 80);
            this.tbReportParam.Name = "tbReportParam";
            this.tbReportParam.Size = new System.Drawing.Size(280, 20);
            this.tbReportParam.TabIndex = 3;
            // 
            // lblreportParameter
            // 
            this.lblreportParameter.AutoSize = true;
            this.lblreportParameter.Location = new System.Drawing.Point(341, 83);
            this.lblreportParameter.Name = "lblreportParameter";
            this.lblreportParameter.Size = new System.Drawing.Size(0, 13);
            this.lblreportParameter.TabIndex = 4;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 183);
            this.Controls.Add(this.lblreportParameter);
            this.Controls.Add(this.tbReportParam);
            this.Controls.Add(this.lblReportLbl);
            this.Controls.Add(this.comboReportCombo);
            this.Controls.Add(this.btnRunReport);
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.Load += new System.EventHandler(this.ReportForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnRunReport;
        private System.Windows.Forms.ComboBox comboReportCombo;
        private System.Windows.Forms.Label lblReportLbl;
        private System.Windows.Forms.TextBox tbReportParam;
        private System.Windows.Forms.Label lblreportParameter;
    }
}
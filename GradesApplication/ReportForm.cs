using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;

namespace GradesApplication
{
    public partial class ReportForm : Form
    {
        ////////////////-------Class and object declarations-------///////////////////////
        Reporting _report = new Reporting();
        ////////////////-------End of declarations-------/////////////////////////////////

        string _saveError;

        /// <summary>
        /// Form constructor
        /// </summary>
        public ReportForm()
        {
            _saveError = null;

            InitializeComponent();           
        }


        ////////////////-------Button Clicks-------//////////////////////////////////////

        /// <summary>
        /// Validates user entered data, runs sprocs, populates save dialog
        /// </summary>
        private void btnRunReport_Click(object sender, EventArgs e)
        {

            if (comboReportCombo.SelectedIndex == 0)
            {
                if (string.IsNullOrWhiteSpace(tbReportParam.Text))
                {
                    MessageBox.Show("Please enter a student ID.");
                }
                    else
                    {
                        string studentCsv = _report.StudentReport(tbReportParam.Text);
                        if (string.IsNullOrEmpty(studentCsv))
                        {
                            MessageBox.Show("An Error has occurred: " + "\n" + "\n" + _report.ErrMessage);
                            ResetForm();
                        }
                        else if (studentCsv == "notfound")
                        {
                            MessageBox.Show("OriginalStudent Not Found.");
                            ResetForm();
                        }
                        else
                        {
                            if (!SaveCsvFile(studentCsv))
                            {
                                if (_saveError != null)
                                {
                                    MessageBox.Show("The Following Error Occured: " + _saveError);
                                }
                                else
                                {
                                    MessageBox.Show("Saving File Cancelled.");
                                }
                            }
                            else
                            {
                                MessageBox.Show("The File Was Saved.");
                                ResetForm();
                            }

                        }
                    }
            }
            else if (comboReportCombo.SelectedIndex == 1)
            {
                if (string.IsNullOrWhiteSpace(tbReportParam.Text))
                {
                    MessageBox.Show("Please enter a subject name.");
                }
                else
                {
                    string subjectCsv = _report.SubjectReport(tbReportParam.Text);
                    if (string.IsNullOrEmpty(subjectCsv))
                    {
                        MessageBox.Show("An Error has occurred: " + "\n" + "\n" + _report.ErrMessage);
                        ResetForm();
                    }
                    else if (subjectCsv == "notfound")
                    {
                        MessageBox.Show("Subject Not Found.");
                        ResetForm();
                    }
                    else
                    {
                        if (!SaveCsvFile(subjectCsv))
                        {
                            if (_saveError != null)
                            {
                                MessageBox.Show("The Following Error Occured: " + _saveError);
                            }
                            else
                            {
                                MessageBox.Show("Saving File Cancelled.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("The file was saved.");
                            ResetForm();
                        }

                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please Select a report to run.");
            }
        }

        ////////////////-------End of Button Clicks-------//////////////////////////////////


        ////////////////-------Other Events-------//////////////////////////////////////////

        /// <summary>
        /// Loads form data and formats elements of form
        /// </summary>
        private void ReportForm_Load(object sender, EventArgs e)
        {
            lblreportParameter.Font = new Font(lblreportParameter.Font, FontStyle.Bold);
            lblreportParameter.ForeColor = System.Drawing.Color.Green;
            lblreportParameter.Text = "REPORT PARAMETERS GO HERE";
        }

        /// <summary>
        /// Changes label formatting based on combobox item selected
        /// </summary>
        private void comboReportCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboReportCombo.SelectedIndex == 0)
            {
                lblreportParameter.Font = new Font(lblreportParameter.Font ,FontStyle.Bold);
                lblreportParameter.ForeColor = System.Drawing.Color.Red;
                lblreportParameter.Text = "PLEASE ENTER STUDENT ID";
            }
            else if (comboReportCombo.SelectedIndex == 1)
            {
                lblreportParameter.Font = new Font(lblreportParameter.Font, FontStyle.Bold);
                lblreportParameter.ForeColor = System.Drawing.Color.Red;
                lblreportParameter.Text = "PLEASE ENTER SUBJECT NAME";
            }
            else
            {
                lblreportParameter.Font = new Font(lblreportParameter.Font, FontStyle.Bold);
                lblreportParameter.ForeColor = System.Drawing.Color.Green;
                lblreportParameter.Text = "REPORT PARAMETERS GO HERE";
            }
        }

        ////////////////-------End of Other Events-------//////////////////////////////////////////



        ////////////////-------Form Utility Methods-------/////////////////////////////////////////

        /// <summary>
        /// Populates save dialogue, limited to csv files
        /// </summary>
        public bool SaveCsvFile(string csvfile)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            try
            {
                sfd.FileOk += CheckIfFileHasCorrectExtension;
                sfd.FileName = "";
                sfd.Filter = "CSV files (*.csv)|*.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        sw.WriteLine(csvfile);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _saveError = "Error: " + e.Message;
                return false;
            }


        }

        /// <summary>
        /// Sub method for SaveCsvFile method, doesn't allow .txt, .xls extensions etc
        /// </summary>
        private void CheckIfFileHasCorrectExtension(object sender, CancelEventArgs e)
        {
            SaveFileDialog sv = (sender as SaveFileDialog);
            if (Path.GetExtension(sv.FileName).ToLower() != ".csv")
            {
                e.Cancel = true;
                MessageBox.Show("Please omit the extension or use 'CSV'");
                return;
            }
        }

        /// <summary>
        /// Resets formatting, combo box position and text
        /// </summary>
        private void ResetForm()
        {
            _saveError = null;
            tbReportParam.Clear();
            comboReportCombo.SelectedIndex = -1;
            comboReportCombo.Text = "---Please pick a report to run-----";
        }

        ////////////////-------End of Form Utility Methods-------///////////////////////////////////////////////
    }
}

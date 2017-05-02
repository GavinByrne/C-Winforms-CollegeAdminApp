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
using System.Text.RegularExpressions;

namespace GradesApplication
{
    public partial class MainForm : Form
    {
        ////////////////-------Class and object declarations-------///////////////////////
        Database _db = new Database();

        
        public List<Subject> SubjectList = new List<Subject>();
        List<Grade> _gradesList = new List<Grade>();
        DataTable _details = new DataTable();
        ////////////////-------End of declarations-------/////////////////////////////////


        /// <summary>
        /// Form constructor -->need to tidy up contents-------
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }


        ////////////////-------Button Clicks-------//////////////////////////////////////

        //click event that will open the reports form. no need to edit this code. 
        private void btnOpenReports_Click(object sender, EventArgs e)
        {
            ReportForm reports = new ReportForm();

            try
            {
                reports.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("OOPS Sorry, Cannot open form!  - " + ex.Message);
            }
        }

        /// <summary>
        /// Compares items in listbox to subjects and adds new subject if not a duplicate
        /// </summary>
        private void btnAddSubject_Click(object sender, EventArgs e)
        {
            if (comboSubject.SelectedItem == null)
                MessageBox.Show("Please add a subject");
            else if (!ListSearch(listBoxSubjects, comboSubject.SelectedItem.ToString(), 0))
                listBoxSubjects.Items.Add(comboSubject.SelectedItem.ToString());
            else
                MessageBox.Show("Subject already added");
        }

        /// <summary>
        /// Validates text boxes, checks subjects, creates student obj and inserts student/subject data to db
        /// </summary>
        private void btnAddNewStudent_Click(object sender, EventArgs e)
        {
            //DateTime Now = DateTime.Now;

            DateTime dateTime;
            if(!DateTime.TryParse(mtbDateOfBirth.Text,out dateTime))
            {
                MessageBox.Show("Please enter a valid date");
            }
            else if (!CheckStudentBlanks())
            {
                MessageBox.Show("Please ensure all fields contain valid data.");
            }
            else
            {
                //OriginalStudent student = new OriginalStudent(tbStudentNumber.Text, tbFirstName.Text, tbLastName.Text, dateTime, tbAddress.Text, Now);

                foreach (string s in listBoxSubjects.Items)
                {
                    //Grade g = new Grade();

                    var item = SubjectList.FirstOrDefault(o => o.SubjectName == s);
                    if (item != null)
                        _gradesList.Add(
                            new Grade()
                            {
                                StudentID = tbStudentNumber.Text,
                                SubjectID = item.SubjectID.ToString()
                            });
                }

                Student newStudent = new Student();
                {
                    newStudent.StudentID = tbStudentNumber.Text;
                    newStudent.FirstName = tbFirstName.Text;
                    newStudent.LastName = tbLastName.Text;
                    newStudent.DateOfBirth = dateTime;
                    newStudent.Address = tbAddress.Text;
                    newStudent.DateEntered = DateTime.Now;
                }

                if (_db.InsertStudent(newStudent) == false)
                    MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage);
                else if (_db.InsertGrades(_gradesList) == false)
                    MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage);
                else
                {
                    listBoxSubjects.Items.Clear();
                    ClearTBs();
                    _gradesList.Clear();
                    comboSubject.Text = "--Please pick a subject ---";


                    MessageBox.Show("New student details added.");
                    tbStudentNumber.Text = CreateStudentId();
                }
            }
        }

        /// <summary>
        /// Validates text boxes, checks if subject exists, checks if student doesn't take subject, adds new subject
        /// </summary>
        private void btnAddNewSubject_Click(object sender, EventArgs e)
        {
            int temp;

            if (comboSubject2.SelectedIndex < 0 || string.IsNullOrWhiteSpace(mtbGrade.Text))
            {
                MessageBox.Show("Please Select A Subject And Enter A Grade To Continue.");
                //ClearTBs();
            }
            else if (!int.TryParse(mtbGrade.Text, out temp))
            {
                MessageBox.Show("Please enter a valid Grade");
            }
            else if (temp <= 0 || temp >= 101)
            {
                MessageBox.Show("Please enter a valid Grade");
            }
            else
            {

                var item = SubjectList.FirstOrDefault(o => o.SubjectName == comboSubject2.SelectedItem.ToString());

                Grade grades = new Grade();
                {
                    grades.StudentID = _details.Rows[0][0].ToString();
                    grades.SubjectID = item.SubjectID.ToString();
                    grades.Grade1 = int.Parse(mtbGrade.Text);
                    grades.DateEntered = DateTime.Now;
                }

                int count = _db.UpdateSubjectDetails(grades); //calling a method that either adds new data, updates old data or does nothing (an error)

                if (count == -1) //An error has occurred
                {
                    MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage); //warn of error
                }
                else if (count == 0)//if the returned int "count" is zero it means a whole new subject has been added
                {
                    _details = _db.StudentDetails(grades.StudentID);//refreshes the datatable used to display student details in DGV
                    if (_details == null)//if this datatable comes back null or empty, the change has been made but something went wrong when refreshing
                    {
                        MessageBox.Show("An error has occurred refreshing the students details :" + "\n" + "\n" + _db.ErrorMessage); // Explains the error
                    }
                    else if (_details.Rows.Count == 0)
                    {
                        MessageBox.Show("Something went wrong, no details can be found for selected student.");
                    }
                    else// everything went fine ie data updated and refreshed in DGV
                    {
                        MessageBox.Show("New Subject Added.");
                        dgvStudentDetails.DataSource = _details;
                        mtbGrade.Clear();
                        comboSubject2.Text = "--Please pick a subject ---";
                    }
                }
                else//if the returned int "count" is not zero or minus one it means an existing subject has been updated
                {
                    _details = _db.StudentDetails(grades.StudentID);//refreshes the datatable used to display student details in DGV
                    if (_details == null)//if this datatable comes back null or empty, the change has been made but something went wrong when refreshing
                    {
                        MessageBox.Show("An error has occurred refreshing the students details: " + "\n" + "\n" + _db.ErrorMessage);// Explains the error
                    }
                    else if (_details.Rows.Count == 0)
                    {
                        MessageBox.Show("Something went wrong, no details can be found for selected student.");
                    }
                    else// everything went fine ie data updated and refreshed in DGV
                    {
                        MessageBox.Show("New Grade Added.");
                        dgvStudentDetails.DataSource = _details;
                        mtbGrade.Clear();
                        comboSubject2.Text = "--Please pick a subject ---";
                    }
                }
            }
        }

        /// <summary>
        /// Populates datatable with student details for dgv
        /// </summary>
        private void btnGetStudentDetails_Click(object sender, EventArgs e)
        {
            if (rbName.Checked && (string.IsNullOrWhiteSpace(tbfName.Text) || string.IsNullOrWhiteSpace(tblName.Text)))
            {
                MessageBox.Show("Please fill in the First Name and Last Name fields.");
            }
            else if (rbStudentID.Checked && string.IsNullOrWhiteSpace(tbStudentID.Text))
            {
                MessageBox.Show("Please fill in a Student Number.");
            }
            else
            {
                if (rbStudentID.Checked)
                {
                    _details = _db.StudentDetails(tbStudentID.Text);
                }
                else
                {
                    _details = _db.StudentDetails(tbfName.Text, tblName.Text);
                }


                if (_details == null)
                {
                    MessageBox.Show("An Error Has Occurred: " + "\n" + "\n" + _db.ErrorMessage);
                    ClearTBs();
                }
                else if (_details.Rows.Count == 0)
                {
                    MessageBox.Show("Student Not Found.");
                    ClearTBs();
                }
                else
                {
                    dgvStudentDetails.DataSource = _details;
                    ClearTBs();
                    btnAddNewSubject.Enabled = true;
                }

            }

        }

        /// <summary>
        /// Hides and shows the add new lecterer section in the lecturer tab
        /// </summary>
        private void btnActivateNewLecturer_Click(object sender, EventArgs e)
        {
            if (!gbxNewLecturer.Visible)
            {
                gbxNewLecturer.Visible = true;
                btnActivateNewLecturer.Text = "Hide - Add Lecturer Form";
                btnAddLecturerAndSub.Enabled = true;
            }
            else
            {
                gbxNewLecturer.Visible = false;
                btnActivateNewLecturer.Text = "Show - Add Lecturer Form";
                btnAddLecturerAndSub.Enabled = false;
            }
        }

        /// <summary>
        /// Searches for existing lecturers to assign to new subjects in lecturer tab
        /// </summary>
        private void btnSearchLecturers_Click(object sender, EventArgs e)
        {
            tbChosenLID.Clear();

            string lectureID;

            if (rbLecturerName.Checked && (string.IsNullOrWhiteSpace(tbLecturerFname.Text) || string.IsNullOrWhiteSpace(tbLecturerLname.Text)))
            {
                MessageBox.Show("Please fill in the First Name and Last Name fields.");
            }
            else if (rbLecturerId.Checked && string.IsNullOrWhiteSpace(tbLecturerId.Text))
            {
                MessageBox.Show("Please Enter a Lecturer ID.");
            }
            else
            {
                if (rbLecturerId.Checked)
                {
                    lectureID = _db.FindLecturer(tbLecturerId.Text);
                }
                else
                {
                    lectureID = _db.FindLecturer(tbLecturerFname.Text, tbLecturerLname.Text);
                }


                if (lectureID == null)
                {
                    MessageBox.Show("An Error Has Occurred: " + "\n" + "\n" + _db.ErrorMessage);
                }
                else if (lectureID == "")
                {
                    MessageBox.Show("Lecturer Not Found.");
                    tbLecturerFname.Clear();
                    tbLecturerLname.Clear();
                    tbLecturerId.Clear();
                }
                else
                {
                    tbChosenLID.Text = lectureID;
                    btnInsertNewSub.Enabled = true;

                    tbLecturerFname.Clear();
                    tbLecturerLname.Clear();
                    tbLecturerId.Clear();
                }

            }
        }

        /// <summary>
        /// Inserts new subject after validating all data is correct
        /// </summary>
        private void btnInsertNewSub_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbNewSubName.Text))
            {
                MessageBox.Show("Please Enter A Subject Name.");
            }
            else if (string.IsNullOrWhiteSpace(tbChosenLID.Text) || string.IsNullOrWhiteSpace(tbNewSubID.Text))
            {
                MessageBox.Show("Cannot Create A New Subject If Subject ID Or Chosen Lecturer ID Are Blank.");
            }
            else
            {
                Subject sub = new Subject();
                {
                    sub.SubjectName = tbNewSubName.Text;
                    sub.SubjectID = tbNewSubID.Text;
                    sub.LecturerID = tbChosenLID.Text;
                }

                int count = _db.InsertNewSubject(sub);

                if (count == -1) //An error has occurred
                {
                    MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage); //warn of error
                    ResetSubject();
                }
                else if (count > 0)
                {
                    MessageBox.Show("This Subject Name Already Exists, Please Change It And Try Again.");
                    tbNewSubName.Clear();
                }
                else
                {
                    MessageBox.Show("New Subject Added.");
                    ResetSubject();
                }
            }
        }

        /// <summary>
        /// Inserts new lecturer after validating all data is correct
        /// </summary>
        private void btnAddLecturer_Click(object sender, EventArgs e)
        {
            if (!CheckLecturerBlanks())
            {
                MessageBox.Show("Please ensure all fields contain valid data.");
            }
            else
            {
                Lecturer l = new Lecturer();
                {
                    l.LecturerID = tbNewLecturerId.Text;
                    l.FirstName = tbNEwLecturerFname.Text;
                    l.LastName = tbNewLecturerLname.Text;
                    l.Email = tbNewLecturerEmail.Text;
                    l.Phone = tbNewLecturerPhoneNo.Text;
                    l.Address = tbNewLecturerAddress.Text;
                    l.DateEntered = DateTime.Now;
                }

                if (!_db.InsertLecturer(l))
                {
                    MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage);
                }
                else
                {
                    MessageBox.Show("New Lecturer Added.");
                    ResetLecturer();
                }
            }
        }

        /// <summary>
        /// Inserts new lecturer and subject after validating all data is correct
        /// </summary>
        private void btnAddLecturerAndSub_Click(object sender, EventArgs e)
        {
            var item =
                SubjectList.FirstOrDefault(
                    o => o.SubjectName.Equals(tbNewSubName.Text, StringComparison.OrdinalIgnoreCase));

            if (!CheckLecturerBlanks() || string.IsNullOrWhiteSpace(tbNewSubName.Text))
            {
                MessageBox.Show("Please Ensure All Lecturer Fields And A New Subject Name Are Filled In.");
            }
            else if (item != null)
            {
                MessageBox.Show("This Subject Name Already Exists, Please Change It And Try Again.");
            }
            else
            {
                Lecturer l = new Lecturer();
                {
                    l.LecturerID = tbNewLecturerId.Text;
                    l.FirstName = tbNEwLecturerFname.Text;
                    l.LastName = tbNewLecturerLname.Text;
                    l.Email = tbNewLecturerEmail.Text;
                    l.Phone = tbNewLecturerPhoneNo.Text;
                    l.Address = tbNewLecturerAddress.Text;
                    l.DateEntered = DateTime.Now;
                }

                Subject sub = new Subject();
                {
                    sub.SubjectName = tbNewSubName.Text;
                    sub.SubjectID = tbNewSubID.Text;
                    sub.LecturerID = tbNewLecturerId.Text;
                }

                if (!_db.InsertLecturer(l))
                {
                    MessageBox.Show("Neither Lecturer or Subject Added Because The Following Error Occurred: " + "\n" +
                                    _db.ErrorMessage);
                    ResetLecturer();
                    ResetSubject();
                }
                else
                {
                    int count = _db.InsertNewSubject(sub);

                    if (count == -1) //An error has occurred
                    {
                        MessageBox.Show("Lecturer Added." + "\n" + "\n" + "New Subject Not Added Because The Following Error Occurred: " +
                                        _db.ErrorMessage); //warn of error
                        ResetLecturer();
                        ResetSubject();
                    }
                    else
                    {
                        MessageBox.Show("New Lecturer and Subject Added.");
                        ResetLecturer();
                        ResetSubject();
                    }
                }
            }

        }

        ////////////////-------End of Button Clicks-------//////////////////////////////////




        ////////////////-------Other Events-------//////////////////////////////////////////

        /// <summary>
        /// Loads form data and formats elements of form
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.mtbDateOfBirth.Click += new EventHandler(mtbDateOfBirth_Click);
            this.mtbGrade.Click += new EventHandler(mtbGrade_Click);

            rbStudentID.Checked = true;
            rbLecturerName.Checked = true;
            tbStudentNumber.Text = CreateStudentId();

            gbxNewLecturer.Visible = false;
            btnActivateNewLecturer.Text = "Show - Add Lecturer Form";

            if (LoadSubjects() == false)
                MessageBox.Show("The Following Error Occurred: " + _db.ErrorMessage);

            //loads ID's for lecturer tab
            NewSubjectId();
            NewLecturerId();
        }

        /// <summary>
        /// Removes selected item from listbox
        /// </summary>
        private void tsDelete_Click(object sender, EventArgs e)
        {
            listBoxSubjects.Items.RemoveAt(listBoxSubjects.SelectedIndex);
        }

        /// <summary>
        /// Controls where context menu can appear
        /// </summary>
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (listBoxSubjects.SelectedIndex == -1)
            {
                contextMenuStrip1.Enabled = false;
            }
            else
            {
                contextMenuStrip1.Enabled = true;
            }
        }

        /// <summary>
        /// Allows right click selection for deleteing listbox items
        /// </summary>
        private void listBoxSubjects_MouseUp_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = listBoxSubjects.IndexFromPoint(e.Location);
                if (item >= 0)
                {
                    listBoxSubjects.SelectedIndex = item;
                    contextMenuStrip1.Show(listBoxSubjects, e.Location);
                }
            }
        }

        /// <summary>
        /// Disables Student name field in student details part of form
        /// </summary>
        private void rbStudentID_CheckedChanged(object sender, EventArgs e)
        {
            if (rbStudentID.Checked)
            {
                gbxName.Enabled = false;
            }
            else
            {
                gbxName.Enabled = true;
            }
        }

        /// <summary>
        /// Disables Student number field in student details part of form
        /// </summary>
        private void rbName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbName.Checked)
            {
                gbxStudentID.Enabled = false;
            }
            else
            {
                gbxStudentID.Enabled = true;
            }
        }

        /// <summary>
        /// Sets start position of grade text box
        /// </summary>
        private void mtbGrade_Click(object sender, EventArgs e)
        {
            this.mtbGrade.Select(0, 0);
        }

        /// <summary>
        /// Sets start position of DOB text box
        /// </summary>
        private void mtbDateOfBirth_Click(object sender, EventArgs e)
        {
            this.mtbDateOfBirth.Select(0, 0);
        }

        /// <summary>
        /// Toggles between lecturer name and id search
        /// </summary>
        private void rbLecturerName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLecturerName.Checked)
            {
                gbxLecturerId.Enabled = false;
            }
            else
            {
                gbxLecturerId.Enabled = true;
            }
        }

        /// <summary>
        /// Toggles between lecturer name and id search
        /// </summary>
        private void rbLecturerId_CheckedChanged(object sender, EventArgs e)
        {
            if (rbLecturerId.Checked)
            {
                gbxLecturerName.Enabled = false;
            }
            else
            {
                gbxLecturerName.Enabled = true;
            }
        }

        ////////////////-------End of Other Events-------//////////////////////////////////////////



        ////////////////-------Form Utility Methods-------/////////////////////////////////////////

        /// <summary>
        /// Checks for blank textbox's
        /// </summary>
        private bool CheckStudentBlanks()
        {
            if (string.IsNullOrWhiteSpace(tbStudentNumber.Text) || string.IsNullOrWhiteSpace(tbFirstName.Text) || string.IsNullOrWhiteSpace(tbLastName.Text) ||
               string.IsNullOrWhiteSpace(mtbDateOfBirth.Text) || string.IsNullOrWhiteSpace(tbAddress.Text) || listBoxSubjects.Items.Count == 0)
                return false;
            else
                return true;

        }

        ///// <summary>
        ///// Clears the listbox and textbox's
        ///// </summary>
        private void ClearTBs()
        {
            tbFirstName.Clear();
            tbLastName.Clear();
            tbAddress.Clear();
            mtbDateOfBirth.Clear();
            //GB Comment: Clearing Add subject and grade from section B
            tbfName.Clear();
            tblName.Clear();
            tbStudentID.Clear();

            // GB Comment: work around to get default value back as suggested by Sarah
            comboSubject.SelectedIndex = -1;
            //comboSubject.SelectedText = "--Please pick a subject --";
        }

        ///// <summary>
        ///// ensures no dups in the listbox
        ///// </summary>
        private bool ListSearch(ListBox lb, string searchString, int startIndex)
        {
            for (int i = startIndex; i < lb.Items.Count; ++i)
            {
                string lbString = lb.Items[i].ToString();
                if (lbString.Contains(searchString))
                    return true;
            }
            return false;
        }

        ///// <summary>
        ///// loads a list of subjects for the subjects combobox
        ///// </summary>
        private bool LoadSubjects()
        {
            //DataTable subTable = _db.GetSubjects();

            SubjectList = _db.GetSubjects();

            if (SubjectList == null)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < SubjectList.Count; i++)
                {
                    string sName = SubjectList[i].SubjectName;

                    comboSubject.Items.Add(sName);
                    comboSubject2.Items.Add(sName);

                }
            }
            return true;
        }

        /// <summary>
        /// Generates Random Student ID
        /// </summary>
        /// <returns></returns>
        private string CreateStudentId()
        {
            Random r = new Random();
            int idNumber = r.Next(0, 9999999);

            string wholeId = "ST" + idNumber;

            return wholeId;
        }

        //##############----methods to load ID's for lecturer and subject tab (consider revision)-----#########

        /// <summary>
        /// Queries db for lecturer id, sents to newids method then set's return to relevant textbox
        /// </summary>
        private void NewSubjectId()
        {
            string temp = _db.GetLatestSubjectId();

            if (temp == null)
            {
                MessageBox.Show("The Following Error Occurred When Loading New Subject ID:" + "\n" + _db.ErrorMessage);
            }
            else
            {
                tbNewSubID.Text = NewIds(temp);
            }
        }

        /// <summary>
        /// Queries db for lecturer id, sents to newids method then set's return to relevant textbox
        /// </summary>
        private void NewLecturerId()
        {
            string temp = _db.GetLatestLecturerId();

            if (temp == null)
            {
                MessageBox.Show("The Following Error Occurred When Loading New Subject ID:" + "\n" + _db.ErrorMessage);
            }
            else
            {
                tbNewLecturerId.Text = NewIds(temp);
            }
        }

        /// <summary>
        /// Splits a string taken from db, increments and returns it
        /// </summary>
        private string NewIds(string idNumber)
        {
            int integer;
            string _string;
            string newId;

            integer = Int32.Parse(Regex.Match(idNumber, @"\d+").Value) + 1;

            _string = Regex.Replace(idNumber, @"[^A-Z]+", String.Empty);

            if (_string == "L")
            {
                if (integer < 10)
                {
                    newId = _string + "0000" + integer;
                }
                else if (integer < 100)
                {
                    newId = _string + "000" + integer;
                }
                else if (integer < 1000)
                {
                    newId = _string + "00" + integer;
                }
                else if (integer < 10000)
                {
                    newId = _string + "0" + integer;
                }
                else
                {
                    newId = _string + integer;
                }
            }
            else
            {
                if (integer < 10)
                {
                    newId = _string + "000" + integer;
                }
                else if (integer < 100)
                {
                    newId = _string + "00" + integer;
                }
                else if (integer < 1000)
                {
                    newId = _string + "0" + integer;
                }
                else
                {
                    newId = _string + integer;
                }
            }
            return newId;
        }
        //##########################################################################################

        /// <summary>
        /// Validates textboxes when adding a lecturer
        /// </summary>
        private bool CheckLecturerBlanks()
        {
            if (string.IsNullOrWhiteSpace(tbNewLecturerId.Text) || string.IsNullOrWhiteSpace(tbNEwLecturerFname.Text) || string.IsNullOrWhiteSpace(tbNewLecturerLname.Text) ||
               string.IsNullOrWhiteSpace(tbNewLecturerEmail.Text) || string.IsNullOrWhiteSpace(tbNewLecturerPhoneNo.Text) || string.IsNullOrWhiteSpace(tbNewLecturerAddress.Text))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Clears lecturer textboxes and loads a new ID
        /// </summary>
        private void ResetLecturer()
        {
            tbNewLecturerId.Clear();
            tbNEwLecturerFname.Clear();
            tbNewLecturerLname.Clear();
            tbNewLecturerEmail.Clear();
            tbNewLecturerPhoneNo.Clear();
            tbNewLecturerAddress.Clear();

            NewLecturerId();
        }

        /// <summary>
        /// Clears subject textboxes and loads a new ID
        /// </summary>
        private void ResetSubject()
        {
            tbLecturerFname.Clear();
            tbLecturerLname.Clear();
            tbLecturerId.Clear();
            tbChosenLID.Clear();
            tbNewSubName.Clear();

            NewSubjectId();

            SubjectList.Clear();
            comboSubject.Items.Clear();
            comboSubject2.Items.Clear();
            if (LoadSubjects() == false)
                MessageBox.Show("The Following Error Occurred While Refreshing The Subject List: " + _db.ErrorMessage);
        }

        ////////////////-------End of Form Utility Methods-------///////////////////////////////////////////////
    }
}

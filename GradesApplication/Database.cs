using System;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using System.Reflection;

namespace GradesApplication
{
    class Database
    {
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Stores any error messages to a string variable
        /// </summary>
        public string GetErrorMessage()
        {
            return "Database Error: " + Environment.NewLine + this.ErrorMessage;
        }

        /// <summary>
        /// LINQ method to find the last SubjectID
        /// </summary>
        /// <returns>SubjectID as string</returns>
        public string GetLatestSubjectId()
        {
            string subjectid;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    subjectid = db.Subjects
                               .OrderByDescending(s => s.SubjectID)
                               .Select(s => s.SubjectID)
                               .Take(1).FirstOrDefault();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return subjectid;
        }

        /// <summary>
        /// LINQ method to find the last LecturerID
        /// </summary>
        /// <returns>LecturerID as string</returns>
        public string GetLatestLecturerId()
        {
            string lecturerid;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    lecturerid = db.Lecturers
                                .OrderByDescending(l => l.LecturerID)
                                .Select(l => l.LecturerID)
                                .Take(1).FirstOrDefault();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return lecturerid;
        }

        /// <summary>
        /// LINQ method to pull subject info and load to a list
        /// </summary>
        /// <returns>A list of subjects(ID and Name)</returns>
        public List<Subject> GetSubjects()
        {
            List<Subject> subjectList = new List<Subject>();

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    var subjects = db.Subjects.Select(s => new { SubjectName = s.SubjectName, SubjectID = s.SubjectID });

                    foreach (var subject in subjects)
                    {
                        subjectList.Add(
                            new Subject()
                            {
                                SubjectID = subject.SubjectID,
                                SubjectName = subject.SubjectName
                            }
                            );
                    }
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return subjectList;
        }

        /// <summary>
        /// LINQ method using multiple joins to fill a list which is converted to a datatable with student information
        /// </summary>
        /// <returns>A datatable filled with Student details</returns>
        public DataTable StudentDetails(string firstName, string lastName)
        {
            DataTable dt = new DataTable();

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    var details = db.Grades.Join(db.Students, g => g.StudentID, s => s.StudentID, (g, s) => new { g = g, s = s })
                                   .Join(db.Subjects, x => x.g.SubjectID, su => su.SubjectID, (x, su) => new { x = x, su = su })
                                   .Join(db.Lecturers, y => y.su.LecturerID, l => l.LecturerID, (y, l) => new { y = y, l = l })
                                   .Where(z => (z.y.x.s.FirstName.Equals(firstName) && z.y.x.s.LastName.Equals(lastName)))
                                   .Select(z => new
                                   {
                                       StudentID = z.y.x.s.StudentID,
                                       FirstName = z.y.x.s.FirstName,
                                       LastName = z.y.x.s.LastName,
                                       DateOfBirth = z.y.x.s.DateOfBirth,
                                       Address = z.y.x.s.Address,
                                       SubjectID = z.y.x.g.SubjectID,
                                       SubjectName = z.y.su.SubjectName,
                                       Grade = z.y.x.g.Grade1,
                                       LecturerName = ((z.l.FirstName + " ") + z.l.LastName),
                                       DateEntered = z.y.x.g.DateEntered
                                   });

                    dt = LINQtoDataTable(details);
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return dt;
        }

        /// <summary>
        /// LINQ method using multiple joins to fill a list which is converted to a datatable with student information
        /// </summary>
        /// <returns>A datatable filled with Student details</returns>
        public DataTable StudentDetails(string studentId)
        {
            DataTable dt = new DataTable();

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    var details2 = db.Grades.Join(db.Students, g => g.StudentID, s => s.StudentID, (g, s) => new { g = g, s = s })
                                    .Join(db.Subjects, x => x.g.SubjectID, su => su.SubjectID, (x, su) => new { x = x, su = su })
                                    .Join(db.Lecturers, y => y.su.LecturerID, l => l.LecturerID, (y, l) => new { y = y, l = l })
                                    .Where(z => z.y.x.s.StudentID.Equals(studentId))
                                    .Select(z => new
                                    {
                                        StudentID = z.y.x.s.StudentID,
                                        FirstName = z.y.x.s.FirstName,
                                        LastName = z.y.x.s.LastName,
                                        DateOfBirth = z.y.x.s.DateOfBirth,
                                        Address = z.y.x.s.Address,
                                        SubjectID = z.y.x.g.SubjectID,
                                        SubjectName = z.y.su.SubjectName,
                                        Grade = z.y.x.g.Grade1,
                                        LecturerName = ((z.l.FirstName + " ") + z.l.LastName),
                                        DateEntered = z.y.x.g.DateEntered
                                    });

                    dt = LINQtoDataTable(details2);
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return dt;

            // Query To Datatable
            //IEnumerable<DataRow> details = (from g in db.Grades.AsEnumerable()
            //                                join s in db.Students on g.StudentID equals s.StudentID
            //                                join su in db.Subjects on g.SubjectID equals su.SubjectID
            //                                join l in db.Lecturers on su.LecturerID equals l.LecturerID
            //                                where s.StudentID.Equals(studentId)
            //                                select new { s.StudentID, s.FirstName, s.LastName, s.DateOfBirth, s.Address, g.SubjectID, su.SubjectName, g.Grade1, LecturerName = l.FirstName + " " + l.LastName, g.DateEntered })
            //                                as IEnumerable<DataRow>;


            //return details.CopyToDataTable<DataRow>();

            // Queury
            //var details = from g in db.Grades
            //              join s in db.Students on g.StudentID equals s.StudentID
            //              join su in db.Subjects on g.SubjectID equals su.SubjectID
            //              join l in db.Lecturers on su.LecturerID equals l.LecturerID
            //              where s.StudentID.Equals(studentId)
            //              select (new { s.StudentID, s.FirstName, s.LastName, s.DateOfBirth, s.Address, g.SubjectID, su.SubjectName, g.Grade1, LecturerName = l.FirstName + " " + l.LastName, g.DateEntered });

            //DataTable dt = LINQtoDataTable(details);


            //return dt;
        }

        /// <summary>
        /// Linq Method to find a lecturer id based on their name
        /// </summary>
        /// <returns>LecturerID as string</returns>
        public string FindLecturer(string firstname, string lastname)
        {
            string lecturerid;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    lecturerid = db.Lecturers
                                .Where(l => (l.FirstName.Equals(firstname) && l.LastName.Equals(lastname)))
                                .Select(l => l.LecturerID).FirstOrDefault();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return lecturerid;
        }

        /// <summary>
        /// Linq Method to find a lecturer id based on user inputted ID
        /// </summary>
        /// <returns>LecturerID as string</returns>
        public string FindLecturer(string lecturerID)
        {
            string lecturerid;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    lecturerid = db.Lecturers
                                .Where(l => l.LecturerID.Equals(lecturerID))
                                .Select(l => l.LecturerID).FirstOrDefault();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return lecturerid;
        }

        /// <summary>
        /// LINQ method to insert a new student object
        /// </summary>
        /// <returns>A bool to confirm/deny insert</returns>
        public bool InsertStudent(Student s)
        {
            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    db.Students.InsertOnSubmit(s);
                    db.SubmitChanges();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return false;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return false;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return false;
            }
            return true;

        }

        /// <summary>
        /// LINQ method to insert a list of subjects for the new student above
        /// </summary>
        /// <returns>A bool to confirm/deny insert</returns>
        public bool InsertGrades(List<Grade> gList)
        {
            try
            {
                foreach (var item in gList)
                {
                    using (LinqDBDataContext db = new LinqDBDataContext())
                    {
                        Grade g = new Grade();

                        g.StudentID = item.StudentID;
                        g.SubjectID = item.SubjectID;
                        g.DateEntered = DateTime.Now;


                        db.Grades.InsertOnSubmit(g);
                        db.SubmitChanges();
                    }
                }

            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return false;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return false;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return false;
            }
            return true;

        }

        /// <summary>
        /// LINQ method to either add a new subject and grade to a student or update the grade of 
        /// and existing subject
        /// </summary>
        /// <returns>A count in int form</returns>
        public int UpdateSubjectDetails(Grade g)
        {
            int count;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    count = db.Grades.Count(n => n.SubjectID.Equals(g.SubjectID) && n.StudentID.Equals(g.StudentID));

                    if (count == 0)
                    {
                        db.Grades.InsertOnSubmit(g);
                        db.SubmitChanges();
                    }
                    else
                    {
                        Grade grade = db.Grades.FirstOrDefault(e => e.SubjectID.Equals(g.SubjectID) && e.StudentID.Equals(g.StudentID));

                        grade.Grade1 = g.Grade1;
                        db.SubmitChanges();
                    }
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return -1;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return -1;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return -1;
            }
            return count;
        }

        /// <summary>
        /// LINQ method using multiple joins to fill a list which is converted to a datatable with subject information
        /// </summary>
        /// <returns>A datatable filled with subject details</returns>
        public DataTable SubjectDetails(string subjectName)
        {
            DataTable dt = new DataTable();

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    var details2 = db.Grades.Join(db.Students, g => g.StudentID, s => s.StudentID, (g, s) => new { g = g, s = s })
                                    .Join(db.Subjects, x => x.g.SubjectID, su => su.SubjectID, (x, su) => new { x = x, su = su })
                                    .Join(db.Lecturers, y => y.su.LecturerID, l => l.LecturerID, (y, l) => new { y = y, l = l })
                                    .Where(z => z.y.su.SubjectName.Equals(subjectName))
                                    .Select(z => new
                                    {
                                        SubjectID = z.y.x.g.SubjectID,
                                        SubjectName = z.y.su.SubjectName,
                                        StudentID = z.y.x.s.StudentID,
                                        FirstName = z.y.x.s.FirstName,
                                        LastName = z.y.x.s.LastName,
                                        Grade = z.y.x.g.Grade1,
                                        LecturerName = ((z.l.FirstName + " ") + z.l.LastName)
                                    });

                    dt = LINQtoDataTable(details2);
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return null;
            }
            return dt;
        }

        /// <summary>
        /// LINQ method to insert a new subject object
        /// </summary>
        /// <returns>int count to confirm/deny insert</returns>
        public int InsertNewSubject(Subject s)
        {
            int count;

            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    count = db.Subjects.Count(n => n.SubjectName.Equals(s.SubjectName));

                    if (count == 0)
                    {
                        db.Subjects.InsertOnSubmit(s);
                        db.SubmitChanges();
                    }

                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return -1;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return -1;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return -1;
            }
            return count;

        }

        /// <summary>
        /// LINQ method to insert a new lecturer object
        /// </summary>
        /// <returns>A bool to confirm/deny insert</returns>
        public bool InsertLecturer(Lecturer l)
        {
            try
            {
                using (LinqDBDataContext db = new LinqDBDataContext())
                {
                    db.Lecturers.InsertOnSubmit(l);
                    db.SubmitChanges();
                }
            }
            catch (SqlException sqlException)
            {
                this.ErrorMessage = "SQL Error: " + sqlException.Message;
                return false;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrorMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return false;
            }
            catch (Exception e)
            {
                this.ErrorMessage = "Error: " + e.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts IEnumerable<T> to a datatable, used in methods above that return tables
        /// </summary>
        /// <returns>A datatable filled by list parameter</returns>
        public DataTable LINQtoDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others 
                //will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace GradesApplication
{
    class Reporting
    {
        Database _db = new Database();

        int _min;
        int _max;
        double _avg;

        string _minGrade = "This is the lowest grade achieved in this report criteria:";
        string _maxGrade = "This is the highest grade achieved in this report criteria:";
        string _avgGrade = "The average grade achieved in this report criteria is:";

        public string ErrMessage { get; set; }

        /// <summary>
        /// Retrieves a datatable from a sproc and passes to TOCSV method
        /// </summary>
        public string StudentReport(string studentId)
        {
            DataTable dt = _db.StudentDetails(studentId);

            if (dt == null)
            {
                ErrMessage = _db.ErrorMessage;
                return null;
            }
            else if (dt.Rows.Count == 0)
            {
                return "notfound";
            }
            else
            {
                DataTable dtCopy = AddGradeRating(dt);
                string tempReport = ToCsv(dtCopy);

                return tempReport;
            }
        }

        /// <summary>
        /// Retrieves a datatable from a sproc and passes to TOCSV method
        /// </summary>
        public string SubjectReport(string subjectName)
        {
            DataTable dt = _db.SubjectDetails(subjectName);

            if (dt == null)
            {
                ErrMessage = _db.ErrorMessage;
                return null;
            }
            else if (dt.Rows.Count == 0)
            {
                return "notfound";
            }
            else
            {
                DataTable dtCopy = AddGradeRating(dt);
                string tempReport = ToCsv(dtCopy);

                return tempReport;
            }
        }

        /// <summary>
        /// Adds a column to the data table and fills each row based on the Grade column
        /// </summary>
        public DataTable AddGradeRating(DataTable originalTable)
        {
            originalTable.Columns.Add(new DataColumn("GradeRating", typeof (string)));

            foreach (DataRow row in originalTable.Rows)
            {
                string temp = "";
                string temp2 = row["Grade"].ToString();

                int grade;

                if (int.TryParse(row["Grade"].ToString(), out grade))
                {
                    if (string.IsNullOrWhiteSpace(temp2))
                    {
                        temp = "";
                    }
                    else if (grade < 40)
                    {
                        temp = "Fail";
                    }
                    else if (grade < 55)
                    {
                        temp = "D";
                    }
                    else if (grade < 70)
                    {
                        temp = "C";
                    }
                    else if (grade < 85)
                    {
                        temp = "B";
                    }
                    else if (grade <= 100)
                    {
                        temp = "A";
                    }
                    else
                    {
                        temp = "";
                    }
                    row["GradeRating"] = temp;
                }

            }

            return originalTable;
        }

        /// <summary>
        /// Preps summary string data and sends datatable to stringbuilder
        /// </summary>
        public string ToCsv(DataTable dtcopy)
        {

            string success;

            List<decimal> gradeListDecimals = dtcopy.AsEnumerable().Where(al => al["Grade"] != DBNull.Value).Select(al => al.Field<decimal>("Grade")).ToList();

            List<int> levels = gradeListDecimals.Select(g => Convert.ToInt32(g)).ToList();


            try
            {
                if (levels.Count >= 1)
                {
                    _min = levels.Min();
                    _max = levels.Max();
                    _avg = levels.Average();

                    success = BuildString(dtcopy);
                }
                else
                {
                    success = BuildString(dtcopy);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return success;
        }

        /// <summary>
        /// Sub Method to build string for report, removes the need for duplicate code
        /// </summary>
        private string BuildString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }
            sb.AppendLine();
            sb.AppendLine("," + _maxGrade + "," + _max);
            sb.AppendLine("," + _minGrade + "," + _min);
            sb.AppendLine("," + _avgGrade + "," + Math.Round(_avg, 2, MidpointRounding.AwayFromZero));

            return sb.ToString();
        }
    }
}

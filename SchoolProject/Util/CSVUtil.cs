using SchoolProject.Models;
using System;
using System.Collections.Generic;

namespace SchoolProject.Util
{
    public static class CSVUtil
    {
        private static List<T> parse<T>(string csvContent, Func<string[], T> mapper)
        {
            List<T> parsedList = new List<T>();
            string[] lines = csvContent.Split('\n');

            try
            {
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    line = line.Replace("\r", "");
                    string[] splitLine = line.Split(',');
                    parsedList.Add(mapper(splitLine));
                }
            } catch (Exception e)
            {
                //TODO: use actual logger class
                System.Diagnostics.Debug.WriteLine("Error occurred during mapping! " + e.Message);
            }

            return parsedList;
        }

        public static List<Student> parseCSVForStudents(string csvContent)
        {
            return parse(csvContent, (string[] data) => {
                Student student = new Student();
                student.StudentNumber = data[1];
                student.FirstMidName = data[2];
                student.LastName = data[3];
                student.hasScholarship = "Yes".Equals(data[4]);
                return student;
            });
        }

        public static List<Teacher> parseCSVForTeachers(string csvContent)
        {
            return parse(csvContent, (string[] data) => {
                Teacher teacher = new Teacher();
                teacher.FirstMidName = data[1];
                teacher.LastName = data[2];
                return teacher;
            });
        }
    }
}

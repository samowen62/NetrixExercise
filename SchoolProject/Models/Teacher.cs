using System;
using System.Collections.Generic;

namespace SchoolProject.Models
{
    public class Teacher
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public List<Student> students { get; set; }

        public int getNumberStudents()
        {
            return students != null ? students.Count : 0;
        }
    }
}
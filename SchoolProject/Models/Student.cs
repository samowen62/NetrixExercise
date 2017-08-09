using System;
using System.Collections.Generic;

namespace SchoolProject.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public string StudentNumber { get; set; }
        public bool hasScholarship { get; set; }

        public Teacher teacher { get; set; }
    }
}
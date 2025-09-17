using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManager
{
    internal class Student
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Score { get; set; }

        public override string ToString()
        {
            return $"姓名: {Name}, 年龄: {Age}, 成绩: {Score:F1}";
        }
    }
}

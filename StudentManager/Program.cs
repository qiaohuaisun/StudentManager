
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace StudentManager
{
    internal class Program
    {
        private static readonly string _filePath = "studentData.json";
        private static List<Student> _students = [];

        static void Main(string[] args)
        {
            LoadData();

            Console.WriteLine("========================================");
            Console.WriteLine("     欢迎使用学生信息管理系统 v1.0");
            Console.WriteLine("========================================");

            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        DeleteStudent();
                        break;
                    case "3":
                        UpdateStudent();
                        break;
                    case "4":
                        ViewAllStudents();
                        break;
                    case "5":
                        SortStudents();
                        break;
                    case "6":
                        ShowStatistics();
                        break;
                    case "7":
                        SaveData();
                        Console.WriteLine("✅ 数据已保存到 students.json");
                        break;
                    case "0":
                        SaveData(); // 退出前自动保存
                        Console.WriteLine("👋 感谢使用，再见！");
                        return;
                    default:
                        Console.WriteLine("❌ 无效选项，请重新选择。");
                        break;
                }

                Console.WriteLine("\n按任意键继续...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void LoadData()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("数据文件不存在，请创建");
                return;
            }
            try
            {
                string data = File.ReadAllText(_filePath, System.Text.Encoding.UTF8);
                var loaded = JsonSerializer.Deserialize<List<Student>>(data);

                if (loaded != null)
                {
                    _students = loaded;
                    Console.WriteLine($"读取到{_students.Count}条数据");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"出现错误，错误信息：{ex.Message}");
            }
        }

        private static void SaveData()
        {
            try
            {
                var option = new JsonSerializerOptions { WriteIndented = true };
                var data = JsonSerializer.Serialize( _students, option);
                File.WriteAllText( _filePath, data, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"出现错误，错误信息：{ex.Message}");
            }
        }

        private static void ShowStatistics()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("无数据可统计");
                return;
            }

            var average = _students.Average(s => s.Score);
            var max = _students.Max(s => s.Score);
            var min = _students.Min(s => s.Score);

            Console.WriteLine($"📈 平均分：{average:F2}");
            Console.WriteLine($"🔝 最高分：{max:F1}");
            Console.WriteLine($"🔻 最低分：{min:F1}");
            Console.WriteLine($"👥 学生总数：{_students.Count}");
        }

        private static void SortStudents()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("无数据可排序");
                return;
            }

            Console.Write("请选择排序方式（1-升序，2-降序）：");
            string choice = Console.ReadLine();

            IEnumerable<Student> sorted;
            if (choice == "1")
                sorted = _students.OrderBy(s => s.Score).ToList();
            else if (choice == "2")
                sorted = _students.OrderByDescending(s => s.Score).ToList();
            else
            {
                Console.WriteLine("❌ 无效选择，默认按降序排列。");
                sorted = _students.OrderByDescending(s => s.Score);
            }

            foreach (var student in sorted)
            {
                Console.WriteLine(student);
            }
        }

        private static void ViewAllStudents()
        {
            if (_students.Count == 0)
            {
                Console.WriteLine("📭 暂无学生数据。");
                return;
            }

            Console.WriteLine("\n📋 所有学生信息：");
            foreach (var student in _students)
            {
                Console.WriteLine(student);
            }
        }

        private static void UpdateStudent()
        {
            Console.Write("请输入要修改的学生姓名：");
            string name = Console.ReadLine().Trim();

            var student = _students.FirstOrDefault(s => s.Name == name);
            if (student == null)
            {
                Console.WriteLine("❌ 未找到该学生！");
                return;
            }

            Console.WriteLine($"当前信息：{student}");

            Console.Write("新年龄（直接回车跳过）：");
            string ageInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(ageInput) && int.TryParse(ageInput, out int newAge) && newAge > 0)
            {
                student.Age = newAge;
            }

            Console.Write("新成绩（直接回车跳过）：");
            string scoreInput = Console.ReadLine();
            if (!string.IsNullOrEmpty(scoreInput) && double.TryParse(scoreInput, out double newScore) && newScore >= 0 && newScore <= 100)
            {
                student.Score = newScore;
            }

            Console.WriteLine("✅ 学生信息更新成功！");
        }

        private static void DeleteStudent()
        {
            Console.Write("请输入要删除的学生姓名：");
            string name = Console.ReadLine().Trim();

            var student = _students.FirstOrDefault(s => s.Name == name);
            if (student == null)
            {
                Console.WriteLine("❌ 未找到该学生！");
                return;
            }

            _students.Remove(student);
            Console.WriteLine($"✅ 学生【{name}】已删除！");
        }

        private static void AddStudent()
        {
            Console.Write("请输入学生姓名：");
            string name = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("❌ 姓名不能为空！");
                return;
            }

            Console.Write("请输入年龄：");
            if (!int.TryParse(Console.ReadLine(), out int age) || age <= 0)
            {
                Console.WriteLine("❌ 请输入有效的年龄！");
                return;
            }

            Console.Write("请输入成绩：");
            if (!double.TryParse(Console.ReadLine(), out double score) || score < 0 || score > 100)
            {
                Console.WriteLine("❌ 成绩必须在 0~100 之间！");
                return;
            }

            _students.Add(new Student { Name = name, Age = age, Score = score });
            Console.WriteLine("✅ 学生添加成功！");
        }

        private static void ShowMenu()
        {
            Console.WriteLine("\n请选择操作：");
            Console.WriteLine("1. 添加学生");
            Console.WriteLine("2. 删除学生");
            Console.WriteLine("3. 修改学生信息");
            Console.WriteLine("4. 查看所有学生");
            Console.WriteLine("5. 按成绩排序");
            Console.WriteLine("6. 显示统计信息");
            Console.WriteLine("7. 保存数据");
            Console.WriteLine("0. 退出系统");
            Console.Write("请输入选项：");
        }
    }
}

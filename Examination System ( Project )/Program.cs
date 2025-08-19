using System;
using System.Collections.Generic;
using System.Linq;

class Course
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int MaxDegree { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
}
class Student
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Course> EnrolledCourses { get; set; } = new List<Course>();
    public Dictionary<Exam, int> Scores { get; set; } = new Dictionary<Exam, int>();
}

class Instructor
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Specialization { get; set; }
    public List<Course> TeachingCourses { get; set; } = new List<Course>();
}

abstract class Question
{
    public string Text { get; set; }
    public int Mark { get; set; }
    public abstract bool CheckAnswer(string answer);
}

class MultipleChoiceQuestion : Question
{
    public List<string> Options { get; set; }
    public string CorrectAnswer { get; set; }

    public override bool CheckAnswer(string answer)
    {
        return answer.Trim().ToLower() == CorrectAnswer.Trim().ToLower();
    }
}

class TrueFalseQuestion : Question
{
    public bool CorrectAnswer { get; set; }

    public override bool CheckAnswer(string answer)
    {
        return bool.TryParse(answer, out bool parsed) && parsed == CorrectAnswer;
    }
}

class EssayQuestion : Question
{
    public override bool CheckAnswer(string answer)
    {
        return true; 
    }
}
class Exam
{
    public string Title { get; set; }
    public Course Course { get; set; }
    public List<Question> Questions { get; set; } = new List<Question>();
    public bool Started { get; set; } = false;

    public void AddQuestion(Question q)
    {
        int totalMarks = Questions.Sum(q => q.Mark) + q.Mark;
        if (totalMarks <= Course.MaxDegree && !Started)
        {
            Questions.Add(q);
        }
    }
    public Exam DuplicateForCourse(Course newCourse)
    {
        return new Exam
        {
            Title = this.Title + " (Copy)",
            Course = newCourse,
            Questions = new List<Question>(this.Questions)
        };
    }
}
class ExamResult
{
    public string ExamTitle { get; set; }
    public string StudentName { get; set; }
    public string CourseName { get; set; }
    public int Score { get; set; }
    public bool Passed { get; set; }
}
class Program
{
    static List<Course> Courses = new();
    static List<Student> Students = new();
    static List<Instructor> Instructors = new();
    static List<Exam> Exams = new();
    static List<ExamResult> Results = new();
    static void Main()
    {
        Console.WriteLine("Welcome to the Examination System");

        while (true)
        {
            Console.WriteLine("\n1. Add Course\n2. Add Student\n3. Create Exam\n4. Take Exam\n5. Show Reports\n6. Exit");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddCourse(); break;
                case "2": AddStudent(); break;
                case "3": CreateExam(); break;
                case "4": TakeExam(); break;
                case "5": ShowReports(); break;
                case "6": return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }
    }
    static void AddCourse()
    {
        Console.Write("Enter Course Title: ");
        string title = Console.ReadLine();

        Console.Write("Enter Course Description: ");
        string desc = Console.ReadLine();

        Console.Write("Enter Maximum Degree: ");
        int max = int.Parse(Console.ReadLine());

        Courses.Add(new Course { Title = title, Description = desc, MaxDegree = max });

        Console.WriteLine("Course added successfully.");
    }
    static void AddStudent()
    {
        Console.Write("Enter Student ID: ");
        int id = int.Parse(Console.ReadLine());

        Console.Write("Enter Student Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        var student = new Student { ID = id, Name = name, Email = email };

        Console.WriteLine("Available Courses:");
        for (int i = 0; i < Courses.Count; i++)
            Console.WriteLine($"{i + 1}. {Courses[i].Title}");

        Console.Write("Enroll in course (enter number): ");
        int index = int.Parse(Console.ReadLine()) - 1;

        student.EnrolledCourses.Add(Courses[index]);

        Students.Add(student);

        Console.WriteLine("Student added and enrolled successfully.");
    }
    static void CreateExam()
    {
        Console.WriteLine("Select a Course to create an Exam for:");

        for (int i = 0; i < Courses.Count; i++)
            Console.WriteLine($"{i + 1}. {Courses[i].Title}");

        int cIndex = int.Parse(Console.ReadLine()) - 1;
        var course = Courses[cIndex];

        Console.Write("Enter Exam Title: ");
        string title = Console.ReadLine();

        var exam = new Exam { Title = title, Course = course };

        while (true)
        {
            Console.Write("Enter Question Type (mc/tf/essay) or 'done': ");
            string type = Console.ReadLine().ToLower();

            if (type == "done") break;

            Console.Write("Enter Question Text: ");
            string text = Console.ReadLine();

            Console.Write("Enter Mark: ");
            int mark = int.Parse(Console.ReadLine());

            Question question = null;

            if (type == "mc")
            {
                Console.Write("Enter Options (comma separated): ");
                var options = Console.ReadLine().Split(',').ToList();

                Console.Write("Enter Correct Answer: ");
                string correct = Console.ReadLine();

                question = new MultipleChoiceQuestion { Text = text, Mark = mark, Options = options, CorrectAnswer = correct };
            }
            else if (type == "tf")
            {
                Console.Write("Enter Correct Answer (true/false): ");
                bool correct = bool.Parse(Console.ReadLine());

                question = new TrueFalseQuestion { Text = text, Mark = mark, CorrectAnswer = correct };
            }
            else if (type == "essay")
            {
                question = new EssayQuestion { Text = text, Mark = mark };
            }

            if (question != null)
                exam.AddQuestion(question);
        }

        Exams.Add(exam);
        Console.WriteLine("Exam created successfully.");
    }
    static void TakeExam()
    {
        Console.Write("Enter your Student ID: ");
        int id = int.Parse(Console.ReadLine());

        var student = Students.Find(s => s.ID == id);
        if (student == null)
        {
            Console.WriteLine("Student not found.");
            return;
        }

        Console.WriteLine("Available Exams:");
        for (int i = 0; i < Exams.Count; i++)
        {
            if (student.EnrolledCourses.Contains(Exams[i].Course))
                Console.WriteLine($"{i + 1}. {Exams[i].Title} ({Exams[i].Course.Title})");
        }

        Console.Write("Select Exam: ");
        int eIndex = int.Parse(Console.ReadLine()) - 1;
        var exam = Exams[eIndex];

        int score = 0;
        exam.Started = true;

        foreach (var q in exam.Questions)
        {
            Console.WriteLine($"\nQuestion: {q.Text}");

            if (q is MultipleChoiceQuestion mcq)
            {
                for (int i = 0; i < mcq.Options.Count; i++)
                    Console.WriteLine($"{i + 1}. {mcq.Options[i]}");

                Console.Write("Your Answer: ");
                int ansIndex = int.Parse(Console.ReadLine()) - 1;
                if (mcq.CheckAnswer(mcq.Options[ansIndex]))
                    score += q.Mark;
            }
            else
            {
                Console.Write("Your Answer: ");
                string answer = Console.ReadLine();

                if (q.CheckAnswer(answer))
                    score += q.Mark;
            }
        }

        Results.Add(new ExamResult
        {
            ExamTitle = exam.Title,
            StudentName = student.Name,
            CourseName = exam.Course.Title,
            Score = score,
            Passed = score >= (exam.Course.MaxDegree * 0.5)
        });

        Console.WriteLine($"\nExam Finished! Your score: {score}/{exam.Course.MaxDegree}");
    }
    static void ShowReports()
    {
        Console.WriteLine("\nAll Exam Results:");
        foreach (var r in Results)
        {
            Console.WriteLine($"Student: {r.StudentName} | Exam: {r.ExamTitle} | Course: {r.CourseName} | Score: {r.Score} | {(r.Passed ? "Passed" : "Failed")}");
        }

        Console.Write("\nCompare two students by Exam Title: ");
        string title = Console.ReadLine();

        var studentsInExam = Results.Where(r => r.ExamTitle == title).ToList();

                if (studentsInExam.Count < 2)
        {
            Console.WriteLine("Not enough students took this exam to compare.");
            return;
        }

        Console.Write("Enter first student name: ");
        string name1 = Console.ReadLine();

        Console.Write("Enter second student name: ");
        string name2 = Console.ReadLine();

        var s1 = studentsInExam.FirstOrDefault(r => r.StudentName == name1);
        var s2 = studentsInExam.FirstOrDefault(r => r.StudentName == name2);

        if (s1 == null || s2 == null)
        {
            Console.WriteLine("One or both students not found in this exam.");
            return;
        }
        
        Console.WriteLine($"{s1.StudentName}: {s1.Score} vs {s2.StudentName}: {s2.Score}");

        if (s1.Score > s2.Score)
            Console.WriteLine($"{s1.StudentName} scored higher.");
        else if (s2.Score > s1.Score)
            Console.WriteLine($"{s2.StudentName} scored higher.");
        else
            Console.WriteLine("Both students have the same score.");
    }
}

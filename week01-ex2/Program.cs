using System;
namespace SAaD;

class Program
{
    static void Main()
    {
        //create a new student object
        Student alice = new Student("S001", "Alice");
        Student bob = new Student("S002", "Bob");
        Student charlie = new Student("S003", "Charlie");
        Student dave = new Student("S004", "Dave");
        //create a new module object
        // follow with creating a list titled the module
        Module programming = new Module("COM100", "Programming");
        Module physics = new Module("PHY200", "Physics");

        //enroll the student in the module
        alice.enroll(programming);
        alice.enroll(physics);
        bob.enroll(programming);
        charlie.enroll(programming);
        charlie.enroll(physics);
        dave.enroll(physics);

        //list the modules a student is enrolled in
        alice.getModules();

        //list the students enrolled in a module
        programming.getStudents().ToString();
    }
}

class Student
{
    public string studentId { get; set; }
    public string name { get; set; }
    public List<Module> Mymodules { get; set;} // List to store all modules

    public Student(string studentId, string name)
    {
        this.studentId = studentId;
        this.name = name;
        this.Mymodules = new List<Module>();
    }
    public void enroll(Module module)
    {
        // add module to myModules list
        Mymodules.Add(module);
        //call the func in the module class to add the student to the module
        module.addStudent(this);
    }
    public List<Module> getModules()
    {
        // Return a list of modules a student is enrolled in
        Console.WriteLine($"{name} is enrolled in the following modules:");
        foreach (var module in Mymodules)
        {
            Console.WriteLine($"- {module.title} ({module.code})");
        }
        return Mymodules; // redundant, need to return in one line
    }
    public string toString()
    {
        return $"Student ID: {studentId}, Name: {name}";
    }
}
class Module
{
    public string code { get; set; }
    public string title { get; set; }
    public List<Student> myStudents { get; set; } // List to store all students enrolled in the module
    public Module(string code, string title)
    {
        this.code = code;
        this.title = title;
        this.myStudents = new List<Student>();
    }
    public void addStudent(Student student)
    {
        // after module has been added to student object, add student to the module list
        myStudents.Add(student);
        //print happy path
        Console.WriteLine($"{student.name} has been added to {title}");
    }
    public List<Student> getStudents()
    {
        // Return a printed list of enrolled students
        Console.WriteLine($"Students enrolled in {title}:");
        foreach (var student in myStudents)
        {
            Console.WriteLine($"- {student.name} ({student.studentId})");
        }
        return myStudents; // same here
    }
    
}
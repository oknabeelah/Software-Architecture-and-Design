using System;
	namespace SAaD; 
		class Program 
		{ 
			static void Main() 
			{
                //create employee objects
				Employee e1 = new SalariedEmployee("E001", "John Doe", 60000);
				Employee e2 = new HourlyEmployee("E002", "Jane Smith", 20.0, 40.0);
                Employee e3 = new SalariedEmployee("E003", "Mike Johnson", 80000);
                Employee e4 = new HourlyEmployee("E004", "Emily Davis", 25.0, 35.0);
                
                //add employees to the list
                Employee.addEmployee(e1);
                Employee.addEmployee(e2);
                Employee.addEmployee(e3);
                Employee.addEmployee(e4);
                
                //show the list of employees
                Employee.getAllEmployees();

                //display employee details and pay for each employee
                e1.showEmployeeDetails();
                e2.showEmployeeDetails();
                

			}
		}
        public abstract class Employee
        {
            private string employeeId;
            private string name;
            private static List<Employee> allEmployees = new List<Employee>();
            public Employee(string employeeId, string name)
            {
                this.employeeId = employeeId;
                this.name = name;
            }

            public static void addEmployee(Employee employee)
            {
                //add to list
                allEmployees.Add(employee);
                Console.WriteLine($"Employee '{employee.name}' has been added to the list.");

            }
            public void showEmployeeDetails()
            {
                Console.WriteLine($"Here are the details of the employee:");
                Console.WriteLine($"Employee ID: {employeeId}, Name: {name}, Pay: {calculatePay()}");
            }
            public static void getAllEmployees()
            {
                Console.WriteLine("List of all employees:");
                foreach (Employee emp in allEmployees)
                {
                    emp.showEmployeeDetails();
                }
                Console.WriteLine("---------------------------------");
            }

            public abstract double calculatePay();
            
        }
        public class SalariedEmployee : Employee
        {
            private double annualSalary;
            public SalariedEmployee(string employeeId, string name, double annualSalary) : base(employeeId, name)
            {
                this.annualSalary = annualSalary;
            }
            public double getAnnualSalary() => annualSalary;

            public override double calculatePay()
            {
                return annualSalary / 12;
            }
            
        }
        public class HourlyEmployee : Employee
        {
            private double hourlyRate;
            private double hoursWorked;
            public HourlyEmployee(string employeeId, string name, double hourlyRate, double hoursWorked) : base(employeeId, name)
            {
                this.hourlyRate = hourlyRate;
                this.hoursWorked = hoursWorked;
            }
            public double getHourlyRate() => hourlyRate;
            public double getHoursWorked() => hoursWorked;

            public override double calculatePay()
            {
                return hourlyRate * hoursWorked;
            }
        }
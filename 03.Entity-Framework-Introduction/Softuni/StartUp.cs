using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(RemoveTown(context));
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var town = context.Towns.FirstOrDefault(x => x.Name == "Seattle");

            foreach (var employee in context.Employees)
            {
                if (employee.Address.Town.Name == "Seattle" && employee.AddressId.HasValue)
                {
                    employee.AddressId = null;
                }
            }

            int count = 0;
            foreach (var address in context.Addresses)
            {
                if (address.Town.Name == "Seattle")
                {
                    context.Addresses.Remove(address);
                    count++;
                }
            }

            context.Towns.Remove(town);
            context.SaveChanges();

            return $"{count} addresses in Seattle were deleted";
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Find(2);
            var employeeProject = context.EmployeesProjects.Where(x => x.ProjectId == 2);

            foreach (var item in employeeProject)
            {
                context.EmployeesProjects.Remove(item);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            StringBuilder sb = new StringBuilder();

            var projects = context.Projects.Take(10);
            foreach (var proj in projects)
            {
                sb.AppendLine(proj.Name);
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
               .OrderBy(x => x.FirstName)
               .ThenBy(x => x.LastName)
               .Where(x => x.FirstName.ToLower().StartsWith("sa"))
               .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departments = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employees = context.Employees
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Where(x => departments.Contains(x.Department.Name))
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                employee.Salary *= (decimal)1.12;
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                .OrderBy(x => x.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var pro in projects)
            {
                sb.AppendLine(pro.Name);
                sb.AppendLine(pro.Description);
                sb.AppendLine(pro.StartDate.ToString("M/d/yyyy h:mm:ss tt"));
            }

            return sb.ToString().Trim();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Include(x => x.Employees)
                .OrderBy(x => x.Employees.Count())
                .ThenBy(x => x.Name)
                .Where(x => x.Employees.Count() > 5)
                .Select(x => new
                {
                    x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Employees = x.Employees.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Select(e => new { e.FirstName, e.LastName, e.JobTitle })
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.Name} - {dep.ManagerFirstName} {dep.ManagerLastName}");
                foreach (var employee in dep.Employees)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects.OrderBy(x => x.Project.Name).Select(x => x.Project.Name).ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employee147)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                foreach (var project in employee.Projects)
                {
                    sb.AppendLine(project);
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
              .OrderByDescending(x => x.Employees.Count())
              .ThenBy(x => x.Town.Name)
              .ThenBy(x => x.AddressText)
                .Select(x => new
                {
                    AddressText = x.AddressText,
                    TownName = x.Town.Name,
                    EmployeesCount = x.Employees.Count(),
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastNane = e.Manager.LastName,
                    e.EmployeesProjects,
                    Projects = e.EmployeesProjects.Select(x => new { StartDate = x.Project.StartDate, EndDate = x.Project.EndDate, Name = x.Project.Name })
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - Manager: {employee.ManagerFirstName} {employee.ManagerLastNane}");
                foreach (var project in employee.Projects)
                {
                    var endDate = project.EndDate.HasValue ? project.EndDate.ToString() : "not finished";

                    sb.AppendLine($"--{project.Name} - {project.StartDate} - {endDate}");
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.EmployeeId)
                .Select(e => new Employee
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    MiddleName = e.MiddleName,
                    JobTitle = e.JobTitle,
                    Salary = e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.FirstName)
                .Where(x => x.Salary > 50000)
                .Select(e => new Employee
                {
                    FirstName = e.FirstName,
                    Salary = e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName)
                .Where(x => x.Department.Name == "Research and Development")
                .Select(e => new
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Department = e.Department.Name,
                    Salary = e.Salary
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} from {employee.Department} - ${employee.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var newAddress = new Address { AddressText = "Vitoshka 15", TownId = 4 };

            context.Addresses.Add(newAddress);
            context.SaveChanges();

            var employeeTarget = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            employeeTarget.Address = newAddress;
            context.SaveChanges();

            var allAddresses = context.Employees
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .Select(x => new { x.Address.AddressText })
                .ToList();


            StringBuilder sb = new StringBuilder();

            foreach (var currAddress in allAddresses)
            {
                sb.AppendLine(currAddress.AddressText);
            }

            return sb.ToString().TrimEnd();
        }
    }
}

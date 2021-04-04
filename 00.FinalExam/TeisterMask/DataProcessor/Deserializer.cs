namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var output = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ProjectXmlDto[]), new XmlRootAttribute("Projects"));

            var xmlProjects = (ProjectXmlDto[])xmlSerializer.Deserialize(new StringReader(xmlString));

            foreach (var xmlProject in xmlProjects)
            {
                if (!IsValid(xmlProject))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidOpenDate = DateTime.TryParseExact(xmlProject.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out DateTime openDate);
                if (!isValidOpenDate)
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? dueDate;
                if (!String.IsNullOrEmpty(xmlProject.DueDate))
                {
                    var isValidDueDate = DateTime.TryParseExact(xmlProject.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime tempDueDate);
                    if (!isValidOpenDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    dueDate = tempDueDate;
                }
                else dueDate = null;

                var project = new Project()
                {
                    Name = xmlProject.Name,
                    OpenDate = openDate,
                    DueDate = dueDate
                };

                foreach (var xmlTask in xmlProject.Tasks)
                {
                    if (!IsValid(xmlTask))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    var isValidTaskOpendate = DateTime.TryParseExact(xmlTask.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime taskOpendate);
                    if (!isValidOpenDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (taskOpendate < project.OpenDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    var isValidTaskDueDate = DateTime.TryParseExact(xmlTask.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, 
                        DateTimeStyles.None, out DateTime taskDueDate);

                    if (!isValidTaskDueDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (project.DueDate.HasValue && taskDueDate > project.DueDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = new Task 
                    {
                        Name = xmlTask.Name,
                        OpenDate = taskOpendate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)xmlTask.ExecutionType,
                        LabelType = (LabelType)xmlTask.LabelType
                    };

                    project.Tasks.Add(task);
                }

                context.Projects.Add(project);
                context.SaveChanges();

                output.AppendLine(string.Format(SuccessfullyImportedProject, project.Name, project.Tasks.Count));
            }

            return output.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var output = new StringBuilder();

            var jsonEmployees = JsonConvert.DeserializeObject<EmployeeJsonDto[]>(jsonString);

            foreach (var jsonEmployee in jsonEmployees)
            {
                if (!IsValid(jsonEmployee))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                var employee = new Employee
                {
                    Username = jsonEmployee.Username,
                    Email = jsonEmployee.Email,
                    Phone = jsonEmployee.Phone
                };

                foreach (var jsonTask in jsonEmployee.Tasks.Distinct())
                {
                    if (!IsValid(jsonEmployee))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    var task = context.Tasks.FirstOrDefault(x => x.Id == jsonTask);
                    if (task == null)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask() { Employee = employee, Task = task });
                }

                context.Employees.Add(employee);
                context.SaveChanges();

                output.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username, employee.EmployeesTasks.Count));
            }

            return output.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
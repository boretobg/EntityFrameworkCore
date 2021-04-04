namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var output = new StringBuilder();

            var xmlSerializer = new XmlSerializer(typeof(ProjectXmlDto[]), new XmlRootAttribute("Projects"));

            var stringWriter = new StringWriter(output);

            var projects = context.Projects
                .ToArray()
                .Where(x => x.Tasks.Count > 0)
                .Select(x => new ProjectXmlDto()
                {
                    HasEndDate = x.DueDate.HasValue ? "Yes" : "No",
                    ProjectName = x.Name,
                    TasksCount = x.Tasks.Count,
                    Tasks = x.Tasks.ToArray().Select(t => new TaskXmlDto()
                    {
                        Name = t.Name,
                        Label = t.LabelType.ToString()
                    })
                        .OrderBy(x => x.Name)
                        .ToArray()
                })
                .OrderByDescending(x => x.TasksCount)
                .ThenBy(x => x.ProjectName)
                .ToArray();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            xmlSerializer.Serialize(stringWriter, projects, namespaces);

            return output.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .ToArray()
                .Select(x => new 
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .ToArray()
                    .Where(x => x.Task.OpenDate >= date)
                    .OrderByDescending(x => x.Task.DueDate)
                    .ThenBy(x => x.Task.Name)
                    .Select(t => new 
                    {
                        TaskName = t.Task.Name,
                        OpenDate = t.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = t.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = t.Task.LabelType.ToString(),
                        ExecutionType = t.Task.ExecutionType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(x => x.Tasks.Length)
                .ThenBy(x => x.Username)
                .Take(10)
                .ToArray();

            var result = JsonConvert.SerializeObject(employees, Formatting.Indented);

            return result;
        }
    }
}
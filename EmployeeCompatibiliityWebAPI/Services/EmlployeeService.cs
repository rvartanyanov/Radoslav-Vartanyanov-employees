using System.Globalization;
using System.Collections.Generic;
using System.IO;

namespace EmployeeCompatibilityWebAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        public (int, int, int)? FindLongestWorkingPair(string fileContent)
        {
            var employeeProjects = LoadEmployeeProjects(fileContent);

            if (employeeProjects == null || employeeProjects.Count == 0)
            {
                throw new ArgumentException("No valid data found in the CSV file.");
            }

            int maxDays = 0;
            (int, int)? bestPair = null;

            for (int i = 0; i < employeeProjects.Count; i++)
            {
                for (int j = i + 1; j < employeeProjects.Count; j++)
                {
                    if (employeeProjects[i].ProjectID == employeeProjects[j].ProjectID)
                    {
                        int days = CalculateOverlapDays(employeeProjects[i], employeeProjects[j]);
                        if (days > maxDays)
                        {
                            maxDays = days;
                            bestPair = (employeeProjects[i].EmpID, employeeProjects[j].EmpID);
                        }
                    }
                }
            }

            if (bestPair.HasValue)
            {
                return (bestPair.Value.Item1, bestPair.Value.Item2, maxDays);
            }

            return null;
        }

        private List<EmployeeProject> LoadEmployeeProjects(string fileContent)
        {
            var employeeProjects = new List<EmployeeProject>();
            bool isHeader = true;
            var lines = fileContent.Split('\n');

            foreach (var line in lines)
            {
                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                var parts = line.Split(',');
                if (parts.Length < 4) continue;

                if (!int.TryParse(parts[0], out int empID) || !int.TryParse(parts[1], out int projectID) ||
                    !TryParseDate(parts[2], out DateTime dateFrom))
                {
                    continue;
                }

                DateTime dateTo = parts[3].Trim().ToUpper() == "NULL" ? DateTime.Today : TryParseDate(parts[3], out DateTime parsedDateTo) ? parsedDateTo : DateTime.Today;

                employeeProjects.Add(new EmployeeProject
                {
                    EmpID = empID,
                    ProjectID = projectID,
                    DateFrom = dateFrom,
                    DateTo = dateTo
                });
            }

            return employeeProjects;
        }

        private bool TryParseDate(string dateString, out DateTime date)
        {
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "MMM dd, yyyy", "MMMM dd, yyyy", "yyyy/MM/dd", "dd-MM-yyyy", "yyyy.MM.dd" };
            return DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private int CalculateOverlapDays(EmployeeProject e1, EmployeeProject e2)
        {
            var start = e1.DateFrom > e2.DateFrom ? e1.DateFrom : e2.DateFrom;
            var end = e1.DateTo < e2.DateTo ? e1.DateTo : e2.DateTo;

            if (start <= end)
            {
                return (end - start).Days + 1;
            }

            return 0;
        }
    }
}

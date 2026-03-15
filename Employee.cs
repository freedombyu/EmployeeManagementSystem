// Employee.cs
// Demonstrates: CLASS, VARIABLES, FUNCTIONS, EXPRESSIONS, CONDITIONALS

using System;
using System.Collections.Generic;

namespace EmployeeManagementSystem
{
    // CLASS: Represents one employee — holds data and behaviour
    public class Employee
    {
        // ── VARIABLES (auto-properties) ──────────────────────────
        public int    Id         { get; set; }
        public string Name       { get; set; }
        public string Department { get; set; }
        public double Salary     { get; set; }
        public string HireDate   { get; set; }   // "YYYY-MM-DD"
        public bool   IsActive   { get; set; }
        public ContactInfo Contact { get; set; }  // STRUCT used as a field

        // VARIABLE: list tracks every salary change (demonstrates List<T>)
        public List<SalaryRecord> SalaryHistory { get; private set; } = new();

        // ── CONSTRUCTOR ──────────────────────────────────────────
        public Employee(int id, string name, string department,
                        double salary, ContactInfo contact,
                        string hireDate = "", bool isActive = true)
        {
            Id         = id;
            Name       = name;
            Department = department;
            Salary     = salary;
            Contact    = contact;
            HireDate   = hireDate == "" ? DateTime.Now.ToString("yyyy-MM-dd") : hireDate;
            IsActive   = isActive;
        }

        // ── FUNCTIONS ────────────────────────────────────────────

        // EXPRESSION: multiply to get annual figure
        public double GetAnnualSalary() => Salary * 12;

        // CONDITIONAL: determine pay band
        public string GetSalaryGrade()
        {
            if      (Salary >= 12000) return "Executive";
            else if (Salary >= 10000) return "Senior";
            else if (Salary >= 6000)  return "Mid-Level";
            else if (Salary >= 3500)  return "Junior";
            else                      return "Entry";
        }

        // FUNCTION: Apply a percentage raise and record history
        public void ApplyRaise(double percent, string reason)
        {
            double oldSalary = Salary;
            // EXPRESSION: new salary calculation
            Salary = Math.Round(Salary * (1 + percent / 100.0), 2);
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            SalaryHistory.Add(new SalaryRecord(oldSalary, Salary, today, reason));
        }

        // FUNCTION: Calculate years of service from hire date
        public double GetYearsOfService()
        {
            // CONDITIONAL: guard against bad date strings
            if (!DateTime.TryParse(HireDate, out DateTime hired)) return 0;
            // EXPRESSION: time span in years
            return Math.Round((DateTime.Now - hired).TotalDays / 365.25, 1);
        }

        // FUNCTION: One-line summary for list views
        public override string ToString()
        {
            string status = IsActive ? "Active" : "Inactive";
            return $"[{Id,3}]  {Name,-20} | {Department,-16} | " +
                   $"${Salary,9:F2}/mo | {GetSalaryGrade(),-9} | {status}";
        }

        // ── FILE SERIALISATION ───────────────────────────────────

        // FUNCTION: Encode to a CSV line
        // Format: id,name,dept,salary,phone,email,hireDate,isActive,history
        // History entries are joined with ';'
        public string ToCsv()
        {
            // LOOP: build the history segment
            List<string> histParts = new();
            foreach (SalaryRecord r in SalaryHistory)
                histParts.Add(r.ToPipeString());

            string histSegment = string.Join(";", histParts);
            return $"{Id},{Name},{Department},{Salary}," +
                   $"{Contact.Phone},{Contact.Email}," +
                   $"{HireDate},{IsActive},{histSegment}";
        }

        // FUNCTION: Decode from a CSV line
        public static Employee FromCsv(string csvLine)
        {
            // Split on commas — limit to 9 segments so history column is untouched
            string[] parts = csvLine.Split(',', 9);

            int    id       = int.Parse(parts[0]);
            string name     = parts[1];
            string dept     = parts[2];
            double salary   = double.Parse(parts[3]);
            string phone    = parts[4];
            string email    = parts[5];
            string hireDate = parts[6];
            bool   isActive = bool.Parse(parts[7]);

            ContactInfo contact = new ContactInfo(phone, email);
            Employee emp = new Employee(id, name, dept, salary, contact, hireDate, isActive);

            // LOOP: restore salary history if present
            if (parts.Length == 9 && !string.IsNullOrWhiteSpace(parts[8]))
            {
                string[] histEntries = parts[8].Split(';');
                foreach (string entry in histEntries)
                {
                    if (!string.IsNullOrWhiteSpace(entry))
                        emp.SalaryHistory.Add(SalaryRecord.FromPipeString(entry));
                }
            }

            return emp;
        }
    }
}

// EmployeeManager.cs
// Demonstrates: FUNCTIONS, LOOPS, CONDITIONALS, FILE READ & WRITE

using System;
using System.Collections.Generic;
using System.IO;

namespace EmployeeManagementSystem
{
    public class EmployeeManager
    {
        // ── VARIABLES ────────────────────────────────────────────
        private List<Employee> employees = new List<Employee>();
        private string filePath;
        private int nextId = 1;

        public EmployeeManager(string filePath)
        {
            this.filePath = filePath;
        }

        // ════════════════════════════════════════════════════════
        //  1. ADD EMPLOYEE
        // ════════════════════════════════════════════════════════
        public void AddEmployee()
        {
            UI.Header("Add New Employee");

            Console.Write("Full Name       : ");
            string name = Console.ReadLine() ?? "";
            // CONDITIONAL: empty name guard
            if (string.IsNullOrWhiteSpace(name))
            {
                UI.Warn("Name cannot be blank. Employee not added.");
                return;
            }

            Console.Write("Department      : ");
            string dept = Console.ReadLine() ?? "";

            Console.Write("Monthly Salary  : $");
            if (!double.TryParse(Console.ReadLine(), out double salary) || salary < 0)
            {
                UI.Warn("Invalid salary. Employee not added.");
                return;
            }

            Console.Write("Phone           : ");
            string phone = Console.ReadLine() ?? "";

            Console.Write("Email           : ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Hire Date (YYYY-MM-DD, blank = today): ");
            string hireDate = Console.ReadLine() ?? "";

            // Build struct, validate contact info
            ContactInfo contact = new ContactInfo(phone, email);
            string warnings = contact.Validate();
            // CONDITIONAL: show any contact warnings but still allow save
            if (!string.IsNullOrEmpty(warnings))
                Console.Write(warnings);

            Employee emp = new Employee(nextId++, name, dept, salary, contact, hireDate);
            employees.Add(emp);

            UI.Success($"'{emp.Name}' added with ID {emp.Id} (Hired: {emp.HireDate}).");
        }

        // ════════════════════════════════════════════════════════
        //  2. VIEW ALL EMPLOYEES
        // ════════════════════════════════════════════════════════
        public void DisplayAll()
        {
            UI.Header("All Employees");

            // CONDITIONAL: empty guard
            if (employees.Count == 0) { Console.WriteLine("No employees on record."); return; }

            PrintTableHeader();

            // LOOP: print each employee row
            foreach (Employee emp in employees)
            {
                PrintRow(emp);
            }

            PrintTableFooter();
            PrintPayrollSummary();
        }

        // ════════════════════════════════════════════════════════
        //  3. SEARCH EMPLOYEE BY ID
        // ════════════════════════════════════════════════════════
        public void SearchById()
        {
            UI.Header("Search by Employee ID");
            Console.Write("Enter ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { UI.Warn("Invalid ID."); return; }

            Employee? emp = FindById(id);
            // CONDITIONAL
            if (emp == null) { Console.WriteLine($"No employee found with ID {id}."); return; }

            PrintDetailCard(emp);
        }

        // ════════════════════════════════════════════════════════
        //  4. UPDATE EMPLOYEE SALARY (flat amount)
        // ════════════════════════════════════════════════════════
        public void UpdateSalary()
        {
            UI.Header("Update Employee Salary");
            Employee? emp = PromptForEmployee();
            if (emp == null) return;

            Console.WriteLine($"  Current Salary : ${emp.Salary:F2}  ({emp.GetSalaryGrade()})");
            Console.Write("  New Salary     : $");
            if (!double.TryParse(Console.ReadLine(), out double newSalary) || newSalary < 0)
            { UI.Warn("Invalid amount. No changes made."); return; }

            Console.Write("  Reason         : ");
            string reason = Console.ReadLine() ?? "Manual adjustment";

            double old = emp.Salary;
            emp.Salary = newSalary;
            // Record in history as a manual adjustment
            emp.SalaryHistory.Add(new SalaryRecord(old, newSalary,
                DateTime.Now.ToString("yyyy-MM-dd"), reason));

            UI.Success($"Salary updated for {emp.Name}: ${old:F2} → ${newSalary:F2}. Grade: {emp.GetSalaryGrade()}");
        }

        // ════════════════════════════════════════════════════════
        //  5. REMOVE EMPLOYEE
        // ════════════════════════════════════════════════════════
        public void RemoveEmployee()
        {
            UI.Header("Remove Employee");
            Employee? emp = PromptForEmployee();
            if (emp == null) return;

            Console.Write($"  Are you sure you want to remove '{emp.Name}'? (y/n): ");
            string confirm = Console.ReadLine() ?? "";

            // CONDITIONAL: require explicit 'y'
            if (confirm.ToLower() == "y")
            {
                employees.Remove(emp);
                UI.Success($"Employee '{emp.Name}' (ID: {emp.Id}) removed.");
            }
            else
            {
                Console.WriteLine("  Removal cancelled.");
            }
        }

        // ════════════════════════════════════════════════════════
        //  6. DEPARTMENT REPORT
        // ════════════════════════════════════════════════════════
        public void DepartmentReport()
        {
            UI.Header("Department Summary Report");
            if (employees.Count == 0) { Console.WriteLine("No data."); return; }

            // VARIABLE: dictionary to group data
            Dictionary<string, (int count, double payroll, double maxSalary, double minSalary)> report = new();

            // LOOP: aggregate data per department
            foreach (Employee emp in employees)
            {
                if (!emp.IsActive) continue;   // CONDITIONAL: skip inactive

                if (!report.ContainsKey(emp.Department))
                    report[emp.Department] = (0, 0.0, double.MinValue, double.MaxValue);

                var cur = report[emp.Department];
                // EXPRESSIONS: update aggregated values
                report[emp.Department] = (
                    cur.count + 1,
                    cur.payroll + emp.Salary,
                    Math.Max(cur.maxSalary, emp.Salary),
                    Math.Min(cur.minSalary, emp.Salary)
                );
            }

            Console.WriteLine($"\n  {"Department",-18} | {"Staff",5} | {"Monthly Payroll",16} | {"Min Salary",11} | {"Max Salary",11}");
            Console.WriteLine("  " + new string('─', 74));

            // LOOP: print one row per department
            foreach (var kvp in report)
            {
                var d = kvp.Value;
                Console.WriteLine($"  {kvp.Key,-18} | {d.count,5} | ${d.payroll,15:F2} | ${d.minSalary,10:F2} | ${d.maxSalary,10:F2}");
            }
        }

        // ════════════════════════════════════════════════════════
        //  7. SEARCH EMPLOYEE BY NAME
        // ════════════════════════════════════════════════════════
        public void SearchByName()
        {
            UI.Header("Search by Name");
            Console.Write("Enter name (or partial name): ");
            string query = (Console.ReadLine() ?? "").ToLower();

            // VARIABLE: collect matches
            List<Employee> matches = new();

            // LOOP: scan all employees
            foreach (Employee emp in employees)
            {
                // CONDITIONAL + EXPRESSION: case-insensitive partial match
                if (emp.Name.ToLower().Contains(query))
                    matches.Add(emp);
            }

            // CONDITIONAL: nothing found
            if (matches.Count == 0)
            {
                Console.WriteLine($"  No employees found matching '{query}'.");
                return;
            }

            Console.WriteLine($"  Found {matches.Count} match(es):\n");
            PrintTableHeader();
            foreach (Employee emp in matches) PrintRow(emp);
            PrintTableFooter();
        }

        // ════════════════════════════════════════════════════════
        //  8. FILTER BY DEPARTMENT
        // ════════════════════════════════════════════════════════
        public void FilterByDepartment()
        {
            UI.Header("Filter by Department");

            // LOOP: collect unique department names
            List<string> depts = new();
            foreach (Employee emp in employees)
            {
                if (!depts.Contains(emp.Department))
                    depts.Add(emp.Department);
            }

            if (depts.Count == 0) { Console.WriteLine("No data."); return; }

            // LOOP: show numbered department list
            Console.WriteLine("  Available Departments:");
            for (int i = 0; i < depts.Count; i++)
                Console.WriteLine($"    {i + 1}. {depts[i]}");

            Console.Write("\n  Choose department number: ");
            if (!int.TryParse(Console.ReadLine(), out int choice) ||
                choice < 1 || choice > depts.Count)
            { UI.Warn("Invalid selection."); return; }

            string selected = depts[choice - 1];

            // LOOP: collect and display matching employees
            PrintTableHeader();
            int count = 0;
            foreach (Employee emp in employees)
            {
                if (emp.Department == selected)   // CONDITIONAL
                {
                    PrintRow(emp);
                    count++;
                }
            }
            PrintTableFooter();
            Console.WriteLine($"  {count} employee(s) in {selected}.");
        }

        // ════════════════════════════════════════════════════════
        //  9. GIVE A PERCENTAGE RAISE
        // ════════════════════════════════════════════════════════
        public void GiveRaise()
        {
            UI.Header("Apply Salary Raise");
            Console.WriteLine("  Apply raise to:");
            Console.WriteLine("    1. A single employee");
            Console.WriteLine("    2. An entire department");
            Console.Write("  Choice: ");
            string choice = Console.ReadLine() ?? "";

            Console.Write("  Raise percentage (e.g. 10 for 10%): ");
            if (!double.TryParse(Console.ReadLine(), out double pct) || pct <= 0)
            { UI.Warn("Invalid percentage."); return; }

            Console.Write("  Reason for raise: ");
            string reason = Console.ReadLine() ?? "Performance review";

            // CONDITIONAL: single vs department raise
            if (choice == "1")
            {
                Employee? emp = PromptForEmployee();
                if (emp == null) return;
                double before = emp.Salary;
                emp.ApplyRaise(pct, reason);
                UI.Success($"{emp.Name}: ${before:F2} → ${emp.Salary:F2} (+{pct}%)");
            }
            else if (choice == "2")
            {
                Console.Write("  Department name: ");
                string dept = Console.ReadLine() ?? "";
                int raised = 0;

                // LOOP: apply raise to everyone in that department
                foreach (Employee emp in employees)
                {
                    if (emp.Department.ToLower() == dept.ToLower() && emp.IsActive)
                    {
                        emp.ApplyRaise(pct, reason);
                        raised++;
                    }
                }
                UI.Success($"Raise of {pct}% applied to {raised} employee(s) in '{dept}'.");
            }
            else
            {
                UI.Warn("Invalid choice.");
            }
        }

        // ════════════════════════════════════════════════════════
        //  10. SALARY STATISTICS
        // ════════════════════════════════════════════════════════
        public void SalaryStatistics()
        {
            UI.Header("Salary Statistics");
            if (employees.Count == 0) { Console.WriteLine("No data."); return; }

            // VARIABLES: accumulators
            double total = 0, min = double.MaxValue, max = double.MinValue;
            int count = 0;

            // LOOP: compute stats
            foreach (Employee emp in employees)
            {
                if (!emp.IsActive) continue;   // CONDITIONAL
                total += emp.Salary;           // EXPRESSION
                if (emp.Salary < min) min = emp.Salary;
                if (emp.Salary > max) max = emp.Salary;
                count++;
            }

            if (count == 0) { Console.WriteLine("No active employees."); return; }

            double avg = total / count;   // EXPRESSION

            Console.WriteLine($"\n  Active Employees   : {count}");
            Console.WriteLine($"  Lowest Salary      : ${min:F2}");
            Console.WriteLine($"  Highest Salary     : ${max:F2}");
            Console.WriteLine($"  Average Salary     : ${avg:F2}");
            Console.WriteLine($"  Monthly Payroll    : ${total:F2}");
            Console.WriteLine($"  Annual Payroll     : ${total * 12:F2}");

            // LOOP + EXPRESSION: grade distribution
            Dictionary<string, int> grades = new();
            foreach (Employee emp in employees)
            {
                if (!emp.IsActive) continue;
                string grade = emp.GetSalaryGrade();
                if (!grades.ContainsKey(grade)) grades[grade] = 0;
                grades[grade]++;
            }

            Console.WriteLine("\n  Grade Distribution:");
            foreach (var g in grades)
                Console.WriteLine($"    {g.Key,-12} : {g.Value} employee(s)");
        }

        // ════════════════════════════════════════════════════════
        //  11. VIEW SALARY HISTORY
        // ════════════════════════════════════════════════════════
        public void ViewSalaryHistory()
        {
            UI.Header("Salary History");
            Employee? emp = PromptForEmployee();
            if (emp == null) return;

            Console.WriteLine($"\n  Employee  : {emp.Name}  (ID: {emp.Id})");
            Console.WriteLine($"  Current   : ${emp.Salary:F2}/mo  ({emp.GetSalaryGrade()})");
            Console.WriteLine($"  Hire Date : {emp.HireDate}  ({emp.GetYearsOfService()} yr(s) of service)");

            // CONDITIONAL: no history yet
            if (emp.SalaryHistory.Count == 0)
            {
                Console.WriteLine("\n  No salary change history recorded.");
                return;
            }

            Console.WriteLine($"\n  Change History ({emp.SalaryHistory.Count} record(s)):");
            Console.WriteLine("  " + new string('─', 65));

            // LOOP: print each record
            foreach (SalaryRecord record in emp.SalaryHistory)
                Console.WriteLine(record);
        }

        // ════════════════════════════════════════════════════════
        //  12. TOGGLE EMPLOYEE ACTIVE STATUS
        // ════════════════════════════════════════════════════════
        public void ToggleStatus()
        {
            UI.Header("Toggle Employee Active Status");
            Employee? emp = PromptForEmployee();
            if (emp == null) return;

            // EXPRESSION: flip the boolean
            emp.IsActive = !emp.IsActive;
            string newStatus = emp.IsActive ? "Active" : "Inactive";
            UI.Success($"{emp.Name} is now marked as '{newStatus}'.");
        }

        // ════════════════════════════════════════════════════════
        //  13. SORT EMPLOYEES
        // ════════════════════════════════════════════════════════
        public void SortEmployees()
        {
            UI.Header("Sort Employees");
            Console.WriteLine("  Sort by:");
            Console.WriteLine("    1. Name (A–Z)");
            Console.WriteLine("    2. Salary (High → Low)");
            Console.WriteLine("    3. Salary (Low → High)");
            Console.WriteLine("    4. Department");
            Console.WriteLine("    5. Hire Date (Newest first)");
            Console.Write("  Choice: ");

            string choice = Console.ReadLine() ?? "";

            // CONDITIONAL: pick comparator based on choice
            switch (choice)
            {
                case "1": employees.Sort((a, b) => string.Compare(a.Name, b.Name)); break;
                case "2": employees.Sort((a, b) => b.Salary.CompareTo(a.Salary));   break;
                case "3": employees.Sort((a, b) => a.Salary.CompareTo(b.Salary));   break;
                case "4": employees.Sort((a, b) => string.Compare(a.Department, b.Department)); break;
                case "5": employees.Sort((a, b) => string.Compare(b.HireDate, a.HireDate));     break;
                default: UI.Warn("Invalid choice."); return;
            }

            UI.Success("Employees sorted. Use 'View All' to see the result.");
        }

        // ════════════════════════════════════════════════════════
        //  14. EXPORT FULL REPORT TO TEXT FILE
        // ════════════════════════════════════════════════════════
        public void ExportReport()
        {
            UI.Header("Export Report to File");
            string reportPath = $"report_{DateTime.Now:yyyyMMdd_HHmm}.txt";

            try
            {
                using StreamWriter w = new StreamWriter(reportPath);

                // FILE WRITE: write report header
                w.WriteLine("╔══════════════════════════════════════════════╗");
                w.WriteLine("║        EMPLOYEE MANAGEMENT SYSTEM REPORT     ║");
                w.WriteLine($"║  Generated: {DateTime.Now:yyyy-MM-dd  HH:mm}                    ║");
                w.WriteLine("╚══════════════════════════════════════════════╝");
                w.WriteLine();

                // SECTION 1: All employees
                w.WriteLine("── ALL EMPLOYEES ─────────────────────────────────────────────");
                w.WriteLine($"  {"ID",-4} {"Name",-22} {"Department",-16} {"Salary/mo",10} {"Grade",-10} {"Hired",-12} {"Status"}");
                w.WriteLine("  " + new string('─', 85));

                double totalPayroll = 0;

                // LOOP: write each employee line
                foreach (Employee emp in employees)
                {
                    string status = emp.IsActive ? "Active" : "Inactive";
                    w.WriteLine($"  {emp.Id,-4} {emp.Name,-22} {emp.Department,-16} " +
                                $"${emp.Salary,9:F2} {emp.GetSalaryGrade(),-10} {emp.HireDate,-12} {status}");
                    // EXPRESSION: accumulate payroll
                    if (emp.IsActive) totalPayroll += emp.Salary;
                }

                w.WriteLine();
                w.WriteLine($"  Total Employees    : {employees.Count}");
                w.WriteLine($"  Active Monthly Pay : ${totalPayroll:F2}");
                w.WriteLine($"  Annual Payroll     : ${totalPayroll * 12:F2}");
                w.WriteLine();

                // SECTION 2: Department breakdown
                w.WriteLine("── DEPARTMENT BREAKDOWN ──────────────────────────────────────");
                Dictionary<string, (int cnt, double pay)> depts = new();
                foreach (Employee emp in employees)
                {
                    if (!emp.IsActive) continue;
                    if (!depts.ContainsKey(emp.Department)) depts[emp.Department] = (0, 0);
                    var cur = depts[emp.Department];
                    depts[emp.Department] = (cur.cnt + 1, cur.pay + emp.Salary);
                }
                foreach (var kvp in depts)
                    w.WriteLine($"  {kvp.Key,-20} | {kvp.Value.cnt,3} staff | ${kvp.Value.pay:F2}/mo");

                w.WriteLine();

                // SECTION 3: Salary change history
                w.WriteLine("── SALARY HISTORY ────────────────────────────────────────────");
                foreach (Employee emp in employees)
                {
                    if (emp.SalaryHistory.Count == 0) continue;
                    w.WriteLine($"  {emp.Name} (ID: {emp.Id})");
                    foreach (SalaryRecord r in emp.SalaryHistory)
                        w.WriteLine($"    {r}");
                    w.WriteLine();
                }

                w.WriteLine("── END OF REPORT ─────────────────────────────────────────────");
            }
            catch (Exception ex)
            {
                UI.Warn($"Export failed: {ex.Message}");
                return;
            }

            UI.Success($"Report exported to '{reportPath}'.");
        }

        // ════════════════════════════════════════════════════════
        //  SAVE & LOAD
        // ════════════════════════════════════════════════════════
        public void SaveToFile()
        {
            try
            {
                using StreamWriter writer = new StreamWriter(filePath);
                writer.WriteLine($"#nextId={nextId}");

                // LOOP: write one CSV line per employee
                foreach (Employee emp in employees)
                    writer.WriteLine(emp.ToCsv());

                UI.Success($"{employees.Count} employee(s) saved to '{filePath}'.");
            }
            catch (Exception ex) { UI.Warn($"Save failed: {ex.Message}"); }
        }

        public void LoadFromFile()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("  No saved data found — starting fresh.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                // LOOP: parse each line
                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // CONDITIONAL: metadata vs data line
                    if (line.StartsWith("#nextId="))
                    {
                        nextId = int.Parse(line.Substring(8));
                        continue;
                    }

                    employees.Add(Employee.FromCsv(line));
                }

                UI.Success($"Loaded {employees.Count} employee(s) from '{filePath}'.");
            }
            catch (Exception ex) { UI.Warn($"Load failed: {ex.Message}"); }
        }

        // ════════════════════════════════════════════════════════
        //  SEED DATA
        // ════════════════════════════════════════════════════════
        public void SeedData()
        {
            if (employees.Count > 0) return;

            Console.WriteLine("  🌱 Loading sample employee database...");

            var seed = new List<(string n, string d, double s, string ph, string em, string hired)>
            {
                ("Alice Johnson",   "Engineering",    12500, "555-0101", "alice.johnson@company.com",   "2019-03-15"),
                ("Bob Martinez",    "Engineering",     9800, "555-0102", "bob.martinez@company.com",    "2020-07-01"),
                ("Carol Smith",     "Marketing",       7200, "555-0103", "carol.smith@company.com",     "2021-01-10"),
                ("David Lee",       "Marketing",       4500, "555-0104", "david.lee@company.com",       "2023-05-22"),
                ("Eva Williams",    "Human Resources", 6800, "555-0105", "eva.williams@company.com",    "2020-11-30"),
                ("Frank Brown",     "Human Resources", 4200, "555-0106", "frank.brown@company.com",     "2022-08-14"),
                ("Grace Davis",     "Finance",        11000, "555-0107", "grace.davis@company.com",     "2018-06-05"),
                ("Henry Wilson",    "Finance",         8500, "555-0108", "henry.wilson@company.com",    "2021-09-17"),
                ("Isabella Moore",  "Engineering",     5500, "555-0109", "isabella.moore@company.com",  "2022-02-28"),
                ("James Taylor",    "Sales",           6100, "555-0110", "james.taylor@company.com",    "2020-04-11"),
                ("Karen Anderson",  "Sales",           4800, "555-0111", "karen.anderson@company.com",  "2023-01-03"),
                ("Liam Thomas",     "IT Support",      4100, "555-0112", "liam.thomas@company.com",     "2023-10-08"),
            };

            // LOOP: instantiate and add all seed employees
            foreach (var s in seed)
            {
                ContactInfo c = new ContactInfo(s.ph, s.em);
                employees.Add(new Employee(nextId++, s.n, s.d, s.s, c, s.hired));
            }

            UI.Success($"{employees.Count} sample employees loaded.");
        }

        // ════════════════════════════════════════════════════════
        //  PRIVATE HELPERS
        // ════════════════════════════════════════════════════════

        // FUNCTION: Ask user for an ID and return the matching employee
        private Employee? PromptForEmployee()
        {
            Console.Write("  Enter Employee ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            { UI.Warn("Invalid ID."); return null; }

            Employee? emp = FindById(id);
            if (emp == null) UI.Warn($"No employee with ID {id}.");
            return emp;
        }

        // FUNCTION: Linear search by ID (LOOP + CONDITIONAL)
        private Employee? FindById(int id)
        {
            foreach (Employee emp in employees)
            {
                if (emp.Id == id) return emp;   // EXPRESSION: equality
            }
            return null;
        }

        // FUNCTION: Print a formatted employee detail card
        private void PrintDetailCard(Employee emp)
        {
            Console.WriteLine();
            Console.WriteLine($"  ┌─ Employee #{emp.Id} ─────────────────────────────────────┐");
            Console.WriteLine($"  │  Name        : {emp.Name,-38}│");
            Console.WriteLine($"  │  Department  : {emp.Department,-38}│");
            Console.WriteLine($"  │  Salary      : ${emp.Salary,-37:F2}│");
            Console.WriteLine($"  │  Grade       : {emp.GetSalaryGrade(),-38}│");
            Console.WriteLine($"  │  Annual Pay  : ${emp.GetAnnualSalary(),-37:F2}│");
            Console.WriteLine($"  │  Hire Date   : {emp.HireDate,-38}│");
            Console.WriteLine($"  │  Service     : {emp.GetYearsOfService() + " yr(s)",-38}│");
            Console.WriteLine($"  │  Status      : {(emp.IsActive ? "Active" : "Inactive"),-38}│");
            Console.WriteLine($"  │  Phone       : {emp.Contact.Phone,-38}│");
            Console.WriteLine($"  │  Email       : {emp.Contact.Email,-38}│");
            Console.WriteLine($"  │  Changes     : {emp.SalaryHistory.Count + " salary record(s)",-38}│");
            Console.WriteLine($"  └──────────────────────────────────────────────────────┘");
        }

        // FUNCTION: Print the table header line
        private void PrintTableHeader()
        {
            Console.WriteLine();
            Console.WriteLine($"  {"ID",-4} {"Name",-22} {"Department",-16} {"Salary/mo",10} {"Grade",-10} {"Hired",-12} {"Status"}");
            Console.WriteLine("  " + new string('─', 82));
        }

        // FUNCTION: Print one employee row
        private void PrintRow(Employee emp)
        {
            string status = emp.IsActive ? "Active" : "Inactive";
            Console.WriteLine($"  {emp.Id,-4} {emp.Name,-22} {emp.Department,-16} " +
                              $"${emp.Salary,9:F2} {emp.GetSalaryGrade(),-10} {emp.HireDate,-12} {status}");
        }

        // FUNCTION: Print closing line of the table
        private void PrintTableFooter()
        {
            Console.WriteLine("  " + new string('─', 82));
        }

        // FUNCTION: Print total and average payroll
        private void PrintPayrollSummary()
        {
            double total = 0;
            int active = 0;
            // LOOP + EXPRESSION: sum active salaries
            foreach (Employee emp in employees)
            {
                if (emp.IsActive) { total += emp.Salary; active++; }
            }
            Console.WriteLine($"\n  Total: {employees.Count} employee(s) | " +
                              $"Active: {active} | Monthly Payroll: ${total:F2}");
        }
    }
}

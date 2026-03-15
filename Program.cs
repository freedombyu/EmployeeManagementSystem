// Program.cs
// Entry point — Demonstrates: VARIABLES, LOOP (main menu), CONDITIONALS (switch)

using System;

namespace EmployeeManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            UI.Banner();

            // VARIABLE: Manager owns all data and operations
            EmployeeManager manager = new EmployeeManager("employees.csv");
            manager.LoadFromFile();
            manager.SeedData();   // Loads 12 sample employees if no file exists

            // VARIABLE: loop-control flag
            bool running = true;

            // LOOP: main application loop — runs until user exits
            while (running)
            {
                UI.Menu();
                string input = Console.ReadLine() ?? "";

                // CONDITIONAL: route to the correct feature
                switch (input.Trim())
                {
                    // ── Employee Records ────────────────────────
                    case "1":  manager.AddEmployee();       break;
                    case "2":  manager.DisplayAll();        break;
                    case "3":  manager.SearchById();        break;
                    case "4":  manager.SearchByName();      break;
                    case "5":  manager.RemoveEmployee();    break;
                    case "6":  manager.ToggleStatus();      break;

                    // ── Salary ──────────────────────────────────
                    case "7":  manager.UpdateSalary();      break;
                    case "8":  manager.GiveRaise();         break;
                    case "9":  manager.ViewSalaryHistory(); break;

                    // ── Reports & Tools ─────────────────────────
                    case "10": manager.DepartmentReport();  break;
                    case "11": manager.FilterByDepartment(); break;
                    case "12": manager.SalaryStatistics();  break;
                    case "13": manager.SortEmployees();     break;
                    case "14": manager.ExportReport();      break;

                    // ── Exit ────────────────────────────────────
                    case "0":
                        manager.SaveToFile();
                        running = false;   // EXPRESSION: stop the loop
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\n  Goodbye! All data saved.\n");
                        Console.ResetColor();
                        break;

                    default:
                        UI.Warn("Invalid option. Please enter a number from 0 to 14.");
                        break;
                }
            }
        }
    }
}

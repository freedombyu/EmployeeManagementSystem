// UI.cs
// Helper class for colored console output — keeps display logic in one place

using System;

namespace EmployeeManagementSystem
{
    // CLASS: Static utility — no instances needed, just call UI.Success(...) etc.
    public static class UI
    {
        // FUNCTION: Print a section header in cyan
        public static void Header(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"  ═══ {title.ToUpper()} ═══");
            Console.ResetColor();
        }

        // FUNCTION: Print a success message in green
        public static void Success(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✔  {msg}");
            Console.ResetColor();
        }

        // FUNCTION: Print a warning in yellow
        public static void Warn(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n  ⚠  {msg}");
            Console.ResetColor();
        }

        // FUNCTION: Print the application banner
        public static void Banner()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║      EMPLOYEE  MANAGEMENT  SYSTEM  v2.0     ║");
            Console.WriteLine("║                  CSE 310  —  C#             ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        // FUNCTION: Print the main menu
        public static void Menu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n  ┌─── MAIN MENU ──────────────────────────────────┐");
            Console.WriteLine("  │                                                │");
            Console.WriteLine("  │   Employee Records                             │");
            Console.WriteLine("  │    1.  Add Employee                            │");
            Console.WriteLine("  │    2.  View All Employees                      │");
            Console.WriteLine("  │    3.  Search Employee by ID                   │");
            Console.WriteLine("  │    4.  Search Employee by Name                 │");
            Console.WriteLine("  │    5.  Remove Employee                         │");
            Console.WriteLine("  │    6.  Toggle Active / Inactive Status         │");
            Console.WriteLine("  │                                                │");
            Console.WriteLine("  │   Salary                                       │");
            Console.WriteLine("  │    7.  Update Salary (flat amount)             │");
            Console.WriteLine("  │    8.  Give Percentage Raise                   │");
            Console.WriteLine("  │    9.  View Salary History                     │");
            Console.WriteLine("  │                                                │");
            Console.WriteLine("  │   Reports & Tools                              │");
            Console.WriteLine("  │   10.  Department Report                       │");
            Console.WriteLine("  │   11.  Filter by Department                    │");
            Console.WriteLine("  │   12.  Salary Statistics                       │");
            Console.WriteLine("  │   13.  Sort Employees                          │");
            Console.WriteLine("  │   14.  Export Full Report to File              │");
            Console.WriteLine("  │                                                │");
            Console.WriteLine("  │    0.  Save & Exit                             │");
            Console.WriteLine("  │                                                │");
            Console.WriteLine("  └────────────────────────────────────────────────┘");
            Console.ResetColor();
            Console.Write("  Choose option (0–14): ");
        }
    }
}

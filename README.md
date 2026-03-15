# Employee Management System

## Overview

A fully-featured console application written in **C# (.NET 8)** for CSE 310 — Module 1.  
All 12 pre-loaded sample employees are ready to use on first launch. Data persists to a CSV file between runs.

---

## How to Run

**Requires:** [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
cd EmployeeManagementSystem
dotnet run
```

---

## Features (14 Menu Options)

### Employee Records
| # | Feature | Description |
|---|---|---|
| 1 | Add Employee | Create a record with name, dept, salary, phone, email, hire date |
| 2 | View All | Full table with grade, hire date, and active status |
| 3 | Search by ID | Look up and display a detailed employee card |
| 4 | Search by Name | Partial-name search across all records |
| 5 | Remove Employee | Delete a record (with confirmation prompt) |
| 6 | Toggle Status | Mark an employee Active or Inactive |

### Salary
| # | Feature | Description |
|---|---|---|
| 7 | Update Salary | Set a new flat salary amount (logged to history) |
| 8 | Give Raise | Apply a % raise to one employee or a whole department |
| 9 | Salary History | View every pay change with date, old/new amounts, and % delta |

### Reports & Tools
| # | Feature | Description |
|---|---|---|
| 10 | Department Report | Headcount, min/max/total payroll per department |
| 11 | Filter by Department | Show only employees from a chosen department |
| 12 | Salary Statistics | Min, max, avg, total payroll, grade distribution |
| 13 | Sort Employees | Sort by name, salary (asc/desc), department, or hire date |
| 14 | Export Report | Write a full formatted `.txt` report to disk |

---

## C# Concepts Demonstrated

| Requirement | Where |
|---|---|
| **Variables** | `bool running`, `int nextId`, `double salary`, `List<Employee>`, `Dictionary<>` |
| **Expressions** | `Salary * 12`, `Salary * (1 + pct/100)`, `totalPayroll += emp.Salary`, `emp.IsActive = !emp.IsActive` |
| **Conditionals** | `if/else if/else` (grade, validation), `switch` (menu routing), guard clauses throughout |
| **Loops** | `while` (main menu), `foreach` (search, display, aggregation), `for` (dept list) |
| **Functions** | 20+ methods: `AddEmployee()`, `GiveRaise()`, `ApplyRaise()`, `GetYearsOfService()`, `ExportReport()`, etc. |
| **Classes** | `Employee`, `EmployeeManager`, `UI` |
| **Structs** | `ContactInfo` (phone + email + validation), `SalaryRecord` (pay-change event) |
| **File Read** | `LoadFromFile()` — reads `employees.csv` with full history |
| **File Write** | `SaveToFile()` — persists all data; `ExportReport()` — writes a formatted `.txt` report |

---

## Project Structure

```
EmployeeManagementSystem/
├── Program.cs                      # Entry point — main menu loop
├── UI.cs                           # Color console helper (banner, menu, success, warn)
├── Employee.cs                     # Employee class with raise & history
├── ContactInfo.cs                  # ContactInfo struct with validation
├── SalaryRecord.cs                 # SalaryRecord struct (pay change events)
├── EmployeeManager.cs              # All 14 operations + file I/O + seed data
├── EmployeeManagementSystem.csproj
└── README.md
```

---

## Pre-loaded Sample Employees

| ID | Name | Department | Salary | Grade |
|---|---|---|---|---|
| 1 | Alice Johnson | Engineering | $12,500 | Executive |
| 2 | Bob Martinez | Engineering | $9,800 | Senior |
| 3 | Carol Smith | Marketing | $7,200 | Mid-Level |
| 4 | David Lee | Marketing | $4,500 | Junior |
| 5 | Eva Williams | Human Resources | $6,800 | Mid-Level |
| 6 | Frank Brown | Human Resources | $4,200 | Junior |
| 7 | Grace Davis | Finance | $11,000 | Senior |
| 8 | Henry Wilson | Finance | $8,500 | Mid-Level |
| 9 | Isabella Moore | Engineering | $5,500 | Junior |
| 10 | James Taylor | Sales | $6,100 | Mid-Level |
| 11 | Karen Anderson | Sales | $4,800 | Junior |
| 12 | Liam Thomas | IT Support | $4,100 | Junior |

---

## Author

CSE 310 — Module 1 Submission

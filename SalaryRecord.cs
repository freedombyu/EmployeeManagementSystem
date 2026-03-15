// SalaryRecord.cs
// Demonstrates: STRUCT — tracks a single salary change event

namespace EmployeeManagementSystem
{
    // STRUCT: Value type that records one salary change
    public struct SalaryRecord
    {
        public double OldSalary;
        public double NewSalary;
        public string ChangeDate;   // stored as string for simple CSV roundtrip
        public string Reason;

        public SalaryRecord(double oldSalary, double newSalary, string changeDate, string reason)
        {
            OldSalary  = oldSalary;
            NewSalary  = newSalary;
            ChangeDate = changeDate;
            Reason     = reason;
        }

        // FUNCTION: EXPRESSION — calculate change amount and direction
        public double Delta()       => NewSalary - OldSalary;
        public double DeltaPct()    => OldSalary == 0 ? 0 : (Delta() / OldSalary) * 100;

        public override string ToString()
        {
            string arrow = Delta() >= 0 ? "▲" : "▼";
            return $"  {ChangeDate}  |  ${OldSalary:F2}  →  ${NewSalary:F2}  " +
                   $"{arrow} {Math.Abs(DeltaPct()):F1}%  |  {Reason}";
        }

        // Serialize to pipe-delimited string (used inside CSV)
        public string ToPipeString() => $"{OldSalary}|{NewSalary}|{ChangeDate}|{Reason}";

        public static SalaryRecord FromPipeString(string s)
        {
            string[] p = s.Split('|');
            return new SalaryRecord(double.Parse(p[0]), double.Parse(p[1]), p[2], p[3]);
        }
    }
}

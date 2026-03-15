// ContactInfo.cs
// Demonstrates: STRUCT — lightweight value type for related contact fields

using System;

namespace EmployeeManagementSystem
{
    // STRUCT: Value type — stored by value, ideal for small grouped data.
    //         Unlike a class it lives on the stack and is copied on assignment.
    public struct ContactInfo
    {
        // VARIABLES: contact fields
        public string Phone;
        public string Email;

        // FUNCTION: Constructor
        public ContactInfo(string phone, string email)
        {
            Phone = phone.Trim();
            Email = email.Trim();
        }

        // FUNCTION: Basic email format check using EXPRESSIONS + CONDITIONALS
        public bool IsEmailValid()
        {
            // EXPRESSION: check that '@' and '.' both exist in the right places
            int atIndex  = Email.IndexOf('@');
            int dotIndex = Email.LastIndexOf('.');

            // CONDITIONAL
            return atIndex > 0 && dotIndex > atIndex + 1 && dotIndex < Email.Length - 1;
        }

        // FUNCTION: Basic phone check — must have at least 7 digits
        public bool IsPhoneValid()
        {
            int digitCount = 0;
            // LOOP: count digits in the phone string
            foreach (char c in Phone)
            {
                if (char.IsDigit(c)) digitCount++;
            }
            // CONDITIONAL: 7 is the minimum for a local number
            return digitCount >= 7;
        }

        // FUNCTION: Validate both fields, return a warning string or empty
        public string Validate()
        {
            string warning = "";
            if (!IsEmailValid()) warning += "  ⚠ Email format looks invalid.\n";
            if (!IsPhoneValid()) warning += "  ⚠ Phone should have at least 7 digits.\n";
            return warning;
        }

        public override string ToString()
        {
            return $"Phone: {Phone,-15}  |  Email: {Email}";
        }
    }
}

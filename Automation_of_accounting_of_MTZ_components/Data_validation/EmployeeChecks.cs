using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components.Data_validation
{
    static class EmployeeChecks
    {
        public static string CheckEmployeeData(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length > 1 && str.Length <= 30)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetter(strArray[i])) return invalidSymbols;
                    }
                }
                else return allowedLenght;
            }
            return str;
        }

        public static string CheckEmployeeLogin(string login, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (login == string.Empty) return notEntered;
            else
            {
                if (login.Length > 2 && login.Length <= 20)
                {
                    char[] loginArray = login.ToCharArray();
                    for (int i = 0; i < loginArray.Length; i++)
                    {
                        if (!char.IsLetter(loginArray[i]) && !char.IsDigit(loginArray[i]) && loginArray[i] != '_') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            }
            return login;
        }

        public static string CheckEmployeePassword(string password)
        {
            if (password == string.Empty) return "Password not entered.";
            else
            {
                if (password.Length > 2 && password.ToString().Length <= 20)
                {
                    char[] passwordArray = password.ToCharArray();
                    for (int i = 0; i < passwordArray.Length; i++)
                    {
                        if (!char.IsLetter(passwordArray[i]) && !char.IsDigit(passwordArray[i]) && passwordArray[i] != '_' && passwordArray[i] != '*') return "Password contains invalid symbols.";
                    }
                }
                else return "Allowed password length is 3-30 symbols.";
            }
            return password;
        }
    }
}

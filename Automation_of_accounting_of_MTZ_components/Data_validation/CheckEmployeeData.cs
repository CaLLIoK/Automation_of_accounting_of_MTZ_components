using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components.Data_validation
{
    static class CheckEmployeeData
    {
        public static string CheckEmployeeName(string name)
        {
            if (name == string.Empty) return "Name not entered.";
            else
            {
                if (name.Length > 1 && name.Length <= 30)
                {
                    char[] nameArray = name.ToCharArray();
                    for (int i = 0; i < nameArray.Length; i++)
                    {
                        if (!char.IsLetter(nameArray[i])) return "Name contains invalid symbols.";
                    }
                }
                else return "Allowed name length is 2-30 symbols.";
            }
            return name;
        }

        public static string CheckEmployeeSurname(string surname)
        {
            if (surname == string.Empty) return "Surname not entered.";
            else
            {
                if (surname.Length > 1 && surname.Length <= 30)
                {
                    char[] surnameArray = surname.ToCharArray();
                    for (int i = 0; i < surnameArray.Length; i++)
                    {
                        if (!char.IsLetter(surnameArray[i])) return "Surname contains invalid symbols.";
                    }
                }
                else return "Allowed surname length is 2-30 symbols.";
            }
            return surname;
        }

        public static string CheckEmployeePatronymic(string patronymic)
        {
            if (patronymic == string.Empty) return "Patronymic not entered.";
            else
            {
                if (patronymic.Length > 1 && patronymic.Length <= 30)
                {
                    char[] patronymicArray = patronymic.ToCharArray();
                    for (int i = 0; i < patronymicArray.Length; i++)
                    {
                        if (!char.IsLetter(patronymicArray[i])) return "Patronymic contains invalid symbols.";
                    }
                }
                else return "Allowed patronymic length is 2-30 symbols.";
            }
            return patronymic;
        }
    }
}

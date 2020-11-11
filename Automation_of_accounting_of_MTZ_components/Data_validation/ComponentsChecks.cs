using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components.Data_validation
{
    static class ComponentsChecks
    {
        public static string CheckComponentName(string str, /*string notEntered,*/ string invalidSymbols, string allowedLenght)
        {
            //if (str == string.Empty) return notEntered;
            //else
            //{
                if (/*str.Length > 2 && */str.Length <= 100)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != '-' && strArray[i] != ' ') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            //}
            return str;
        }

        public static string CheckComponentName(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length > 2 && str.Length <= 100)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != '-' && strArray[i] != ' ') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            }
            return str;
        }

        public static string CheckTractorBrandName(string str, /*string notEntered,*/ string invalidSymbols, string allowedLenght)
        {
            //if (str == string.Empty) return notEntered;
            //else
            //{
                if (/*str.Length > 2 && */str.Length <= 100)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != '-' && strArray[i] != ' ') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            //}
            return str;
        }

        public static string CheckTractorBrandName(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length > 2 && str.Length <= 100)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != '-' && strArray[i] != ' ') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            }
            return str;
        }

        public static string CheckComponentWeightOrCount(string str, string notEntered, string invalidSymbols, string negativeValue)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                char[] strArray = str.ToCharArray();
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (!char.IsDigit(strArray[i])) return invalidSymbols;
                }
                if (Convert.ToInt32(str) < 0) return negativeValue;
            }
            return str;
        }

        public static string CheckComponentDescription(string str, string invalidSymbols, string allowedLenght)
        {
            if (str != string.Empty)
            {
                if (str.Length > 200) return allowedLenght;
                char[] strArray = str.ToCharArray();
                for (int i = 0; i < strArray.Length - 2; i++)
                {
                    if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != ',' && strArray[i] != ' ' && strArray[i] != '-') return invalidSymbols;
                }
            }          
            return str;
        }

        public static string CheckComponentCost(string str, string notEntered, string invalidSymbols, string incorrectValue)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                char[] strArray = str.ToCharArray();
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (!char.IsDigit(strArray[i]) && strArray[i] != ',') return invalidSymbols;
                }
                if (str == ",") return incorrectValue;
            }
            return str;
        }

        public static string CheckComponentAvailabilityStatus(string str, string notEntered, string incorrectValue)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str != "Есть в наличии" && str != "Нет в наличии") return incorrectValue;
            }
            return str;
        }
    }
}

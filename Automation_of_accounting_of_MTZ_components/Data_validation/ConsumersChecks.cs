using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Automation_of_accounting_of_MTZ_components.Data_validation
{
    static class ConsumersChecks
    {
        public static string CheckConsumerName(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length > 2 && str.Length <= 80)
                {
                    char[] strArray = str.ToCharArray();
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        if (!char.IsLetterOrDigit(strArray[i]) && strArray[i] != '-' && strArray[i] != ' ' && strArray[i] != '"') return invalidSymbols;
                    }
                }
                else return allowedLenght;
            }
            return str;
        }

        public static string CheckConsumerStreet(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length > 2 && str.Length <= 60)
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

        public static string CheckConsumerHouseOrOffice(string str, string notEntered, string invalidSymbols, string negativeValue)
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

        public static string CheckConsumerPhone(string str, string notEntered, string invalidSymbols, string allowedLenght)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length == 17)
                {
                    if (!Regex.IsMatch(str.ToString(), @"(\+|)(375|)(\ |)(\(|)(17|29|25|33|44)\)\d{3}\-\d{2}\-\d{2}")) return invalidSymbols;
                }
                else return allowedLenght;
            }
            return str;
        }
    }
}
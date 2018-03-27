using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SunBlock.DataTransferObjects.Extensions
{
    public static class Validation
    {
        public static bool IsWeekend(this DateTime dateToCheck)
        {
            return dateToCheck.DayOfWeek == DayOfWeek.Saturday || dateToCheck.DayOfWeek == DayOfWeek.Sunday;
        }

        public static bool DayExistsIsInDateList(this DateTime dateToCheck, string[] dates)
        {
            return dateToCheck.DayExistsIsInDateList((from date in dates where date.IsValidDate() select Convert.ToDateTime(date)).ToList());
        }

        public static bool DayExistsIsInDateList(this DateTime dateToCheck, List<DateTime> dateArray)
        {

            var returnValue = false;

            returnValue = dateArray.Exists(dt => dt.Year == dateToCheck.Year && dt.DayOfYear == dateToCheck.DayOfYear);

            return returnValue;
        }

        public static bool IsAlpha(this string value)
        {
            const string pattern = "^([a-zA-Z]*)$";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidGuid(this string value)
        {
            const string pattern = "^((([a-f,A-F,0-9]{8}[-]){1})(([a-f,A-F,0-9]{4}[-]){3})(([a-f,A-F,0-9]{12}){1})|(([{][a-f,A-F,0-9]{8}[-]){1})(([a-f,A-F,0-9]{4}[-]){3})(([a-f,A-F,0-9]{12}){1}[}]))$";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {


                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidWholeNumber(this string value, bool allowNegative = false)
        {
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(!allowNegative ? "^([0-9]+|[0-9]{1,3}(,[0-9]{3})*)?$" : "^([-]?)([0-9]+|[0-9]{1,3}(,[0-9]{3})*)?$");
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidSuffix(this string value)
        {
            const string pattern = "^\\d{4}|\\d{2}$";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidSurname(string value)
        {
            const string pattern = "^([0-9a-zA-Z][0-9a-zA-Z,\\W])$";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsAlphaNumeric(this string value)
        {
            const string pattern = "^([\\da-zA-Z]*)$";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (regex.IsMatch(value))
                        returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidDate(this string value)
        {
            bool returnValue = false;
            try
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Convert.ToDateTime(value);
                    returnValue = true;
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidDate(string month, string day, string year)
        {
            bool returnValue = false;
            const string pattern = "^(\\d{1,2})/(\\d{1,2})/((\\d{2})|(\\d{4}))$";
            try
            {
                StringBuilder dateBuilder = new StringBuilder();
                dateBuilder.Append(month);
                dateBuilder.Append("/");
                dateBuilder.Append(day);
                dateBuilder.Append("/");
                dateBuilder.Append(year);
                if (Regex.IsMatch(dateBuilder.ToString(), pattern))
                {

                    returnValue = dateBuilder.ToString().IsValidDate();
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidDateYyyymmdd(this string value)
        {
            bool returnValue = false;
            const string pattern = "(\\d{4})(\\d{2})(\\d{2})";
            try
            {
                if (Regex.IsMatch(value, pattern))
                {

                    returnValue = Regex.Replace(value, pattern, "$1-$2-$3").IsValidDate();
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidDateMmddyyyy(this string value)
        {
            bool returnValue = false;
            const string pattern = "(\\d{2})(\\d{2})(\\d{4})";
            try
            {
                if (Regex.IsMatch(value, pattern))
                {

                    returnValue = Regex.Replace(value, pattern, "$1-$2-$3").IsValidDate();
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidTimeHhmmss(this string value)
        {
            bool returnValue = false;
            const string pattern = "(\\d{2})(\\d{2})(\\d{2})";
            try
            {
                if (Regex.IsMatch(value, pattern))
                {
                    returnValue = Regex.Replace(value, pattern, "$1:$2:$3").IsValidDate();
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }       

        public static bool IsValidEmail(this string emailAddress)
        {
            const string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            bool returnValue = false;
            try
            {
                Regex regex = new Regex(pattern);
                if (!String.IsNullOrWhiteSpace(emailAddress))
                {

                    returnValue = regex.IsMatch(emailAddress);
                }
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            const string pattern = "^(?:(?:\\+?1\\s*(?:[.-]\\s*)?)?(?:\\(\\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\\s*\\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\\s*(?:[.-]\\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\\s*(?:[.-]\\s*)?([0-9]{4})(?:\\s*(?:#|x\\.?|ext\\.?|extension)\\s*(\\d+))?$";
            bool returnValue = false;
            try
            {
                returnValue = Regex.IsMatch(phoneNumber, pattern);
            }
            catch
            {

                returnValue = false;

            }

            if (!returnValue)
            {


                const string pattern2 = "^((\\d{10})|(\\d{7}))$";
                try
                {
                    returnValue = Regex.IsMatch(phoneNumber, pattern2);
                }
                catch
                {

                    returnValue = false;

                }

            }

            return returnValue;
        }

        public static bool IsValidTaxId(this string taxId)
        {
            const string pattern = "^(\\d{3}-?\\d{2}-?\\d{4})$";
            bool returnValue;
            try
            {
                returnValue = Regex.IsMatch(taxId, pattern);
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        public static bool IsValidZipCode(this string zipcode)
        {
            const string pattern = "^((\\d{5})-?(\\d{4})?)$";
            bool returnValue;
            try
            {
                returnValue = Regex.IsMatch(zipcode, pattern);
            }
            catch
            {

                returnValue = false;

            }
            return returnValue;
        }

        /// <summary>
        /// Checks a string for character other than alpha numberic and returns
        /// true for none and false if bad characted is detected.
        /// </summary>
        /// <param name="valueToCheck">string value to check if AlphaNumberic</param>
        /// <param name="allowSpaces">boolean flag to allow spaces or not</param>
        /// <returns>String Valid = True or False</returns>
        public static bool IsAlphaNumeric(this string valueToCheck, bool allowSpaces)
        {
            Regex alphaNumericExpression = allowSpaces ? new Regex(@"^[a-zA-Z0-9\s]+$") : new Regex(@"^[a-zA-Z0-9]+$");

            return alphaNumericExpression.IsMatch(valueToCheck ?? string.Empty);
        }

        /// <summary>
        /// Trims the string and checks the trimmed length.
        /// </summary>
        /// <param name="valueToCheck">string to validate</param>
        /// <param name="maximumLength">maximum length</param>
        /// <returns></returns>
        public static bool IsLengthValid(this string valueToCheck, int maximumLength)
        {
            return valueToCheck.TrimSafe().Length <= maximumLength;
        }

        /// <summary>
        /// Checks to see if the minimum length is valid.
        /// </summary>
        /// <param name="valueToCheck"></param>
        /// <param name="minimumLength"></param>
        /// <returns></returns>
        public static bool IsMinimumLengthValid(this string valueToCheck, int minimumLength)
        {
            return valueToCheck.TrimSafe().Length >= minimumLength;
        }

        public static bool IsValidEmailAddress(this string valueToCheck)
        {
            bool returnValue = false;

            var reRegExp = new Regex(@"^[A-Z0-9!#$%&*+\-=?^_`{|}~\.]+@[A-Z0-9.-]+\.[A-Z]{2,4}$", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            try
            {
                if (!String.IsNullOrWhiteSpace(valueToCheck))
                    returnValue = reRegExp.IsMatch(valueToCheck);

            }
            catch
            {
                returnValue = false;

            }

            return returnValue;
        }

        public static bool HasInvalidHostCharacters(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Conversions.InvalidHostCharactersRegex.IsMatch(value);
        }
        public static bool HasInvalidBasicCharacters(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Conversions.InvalidBasicCharactersRegex.IsMatch(value);
        }        

        public static bool IsValidIpAddress(this string addr)
        {
            const string pattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9‌​]{2}|2[0-4][0-9]|25[0-5])$";
            IPAddress ip;
            bool valid = !string.IsNullOrEmpty(addr) && Regex.IsMatch(addr, pattern) && IPAddress.TryParse(addr, out ip);
            return valid;
        }
    }
}

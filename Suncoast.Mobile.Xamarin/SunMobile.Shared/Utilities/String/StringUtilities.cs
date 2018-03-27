using System;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SunMobile.Shared.StringUtilities
{
    public static class StringUtilities
    {
        public static string StripInvalidCurrencyChars(string inputString)
        {
			inputString = StripMultipleDecimals (inputString);
            string newString = string.Empty;
			bool isNegative = false;

			if (inputString.StartsWith("(", StringComparison.InvariantCulture))
			{
				isNegative = true;
			}

			if (inputString.StartsWith("$0.00", StringComparison.InvariantCulture))
            {
                inputString = inputString.Substring(5, inputString.Length - 5);
            }            

            foreach (char c in inputString)
            {
                if (char.IsNumber(c) || c == '.')
                {
                    newString += c;
                }
            }

			//if (newString == string.Empty) 
			//{
				//newString = "0";
			//}

			if (isNegative)
			{
				newString = "-" + newString;
			}

            return newString;
        }

		public static int GetNumberOfDecimalPlaces(decimal input)
		{
			int count = BitConverter.GetBytes(decimal.GetBits(input)[3])[2];

			return count;
		}

		public static string StripInvalidCurrencCharsAndFormat(string inputString)
		{
			inputString = StripMultipleDecimals(inputString);

			if (string.IsNullOrEmpty(inputString))
			{
				return string.Empty;
			}

			if (inputString.Length == 1)
			{
				inputString = ".0" + inputString;
			}

			string newString = string.Empty;

			foreach (char c in inputString)
			{
				if (char.IsNumber(c) || c == '.')
				{
					newString += c;
				}
			}

			var cultureInfo = new CultureInfo("en-US");

			decimal decimalValue;
			decimal.TryParse(newString, NumberStyles.Any, cultureInfo, out decimalValue);

			var decimalPlaces = GetNumberOfDecimalPlaces(decimalValue);

			if (decimalPlaces > 2)
			{
				decimalValue = decimalValue * 10;
			}

			if (decimalPlaces == 1)
			{
				decimalValue = decimalValue / 10;
			}

			newString = string.Format(cultureInfo, "{0:C}", decimalValue);

			return newString;
		}

		private static string StripMultipleDecimals(string input)
		{
			while (input.IndexOf(".", StringComparison.Ordinal) != input.LastIndexOf(".", StringComparison.Ordinal)) 
			{
				input = input.Substring (0, input.LastIndexOf(".", StringComparison.Ordinal));
			}

			if (input.Trim () == ".") 
			{
				input = "0.";
			}

			return input;
		}

		public static string FormatAsCurrency(string inputString)
		{
			if (string.IsNullOrEmpty(inputString))
			{
				inputString = "0";
			}

			decimal amount;
			decimal.TryParse(inputString, out amount);
			var cultureInfo = new CultureInfo("en-US");
			string newString = string.Format(cultureInfo, "{0:C}", amount);

			return newString;
		}

		public static bool IsNumeric(this string inputString)
		{
			int i;

			return int.TryParse(inputString, out i);
		}

		public static bool IsNumericOrEmpty(this string inputString)
		{
			int i;

			return (string.IsNullOrEmpty(inputString) || int.TryParse(inputString, out i));
		}

		public static string PadSuffix(this string suffix)
		{
			return suffix.PadLeft(4, '0');
		}

		public static string ConvertToOldSuffix(this string suffix)
		{
			var oldSuffix = suffix;

			if (oldSuffix.Length >= 4)
			{
				oldSuffix = oldSuffix.Substring(2, 2);
			}

			return oldSuffix;
		}

		public static string PadMemberId(this string memberId)
		{
			return memberId.PadLeft(10, '0');
		}

		public static string SafeEmptyNumber(string input)
		{
			var returnValue = !string.IsNullOrEmpty (input) ? input : "0";
			return returnValue;
		}

		public static bool IsValidEmail(string emailAddress)
		{
			const string pattern = "\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			bool returnValue = false;

			try
			{
				Regex regex = new Regex(pattern);

				if (!string.IsNullOrWhiteSpace(emailAddress))
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

		public static bool IsValidPhone(string phoneNumber)
		{
			bool returnValue = false;

			const string pattern2 = "^(\\d{10})$";
			try
			{
				returnValue = Regex.IsMatch(phoneNumber, pattern2);
			}
			catch
			{

				returnValue = false;

			}

			return returnValue;
		}

		public static string StripHtmlTags(string source)
		{
			var returnValue = Regex.Replace(source, "<.*?>", " ");

			// If first char is a string, remove it.
			if (returnValue.Length > 0 && returnValue[0] == ' ')
			{
				returnValue = returnValue.Substring(1);
			}

			return returnValue;
		}
    }
}
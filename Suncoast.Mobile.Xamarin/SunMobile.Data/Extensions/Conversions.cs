using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SunBlock.DataTransferObjects.Extensions
{
	public static class Conversions
	{
		internal static Regex InvalidHostCharactersRegex = new Regex(@"[~`!@#%_=|:;/,><\$\^&\*\(\)\+\b\[\]\}\{\?\.\t\r\v\f\n\e\\""]");
		//internal static Regex InvalidBasicCharactersRegex = new Regex(@"[~`><\^\+\b\}\{\t\r\v\f\n\e]");
		internal static Regex ValidBasicCharactersRegex = new Regex(@"[a-zA-Z0-9\s@\/\\\.\-_\!\?\,\;\:\\”\’\'\(\)]+");
		internal static Regex InvalidBasicCharactersRegex = new Regex(@"[^a-zA-Z0-9\s@\/\\\.\-_\!\?\,\;\:\\”\’\'\(\)]+");

		#region DateTime Extensions...

		public static DateTime GetNextBusinessDay(this DateTime startDate, int futureOrPastDays, List<DateTime> holidays = null)
		{
			var returnValue = startDate;
			returnValue.AddBusinessDays(futureOrPastDays, holidays);
			return returnValue;
		}

		public static DateTime GetNextBusinessDay(this DateTime startDate, List<DateTime> holidays = null, bool lookBackwards = false)
		{
			var returnValue = startDate.AddDays(Convert.ToDouble(lookBackwards ? -1 : 1));


			while (returnValue.IsWeekend() || returnValue.DayExistsIsInDateList(holidays))
			{
				returnValue = returnValue.AddDays(Convert.ToDouble(lookBackwards ? -1 : 1));
			}

			return returnValue;

		}

		public static DateTime AddBusinessDays(this DateTime startDate, int days, List<DateTime> holidays = null)
		{
			holidays = holidays ?? new List<DateTime>();

			bool subtract = (days < 0);

			days = Math.Abs(days);

			while (days > 0)
			{

				startDate = startDate.GetNextBusinessDay(holidays, subtract);
				days--;

			}

			return startDate;
		}

		public static DateTime EasternToUtc(this DateTime valueToConvert)
		{
			var returnValue = valueToConvert;

			try
			{
				if (valueToConvert.IsAnyKnownMinValue())
				{
					returnValue = DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
				}
				else
				{
					TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
					returnValue = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(valueToConvert, DateTimeKind.Unspecified), easternTimeZone);
				}
			}
			catch { }

			return returnValue;
		}

		public static DateTime UtcToEastern(this DateTime valueToConvert)
		{
			var returnValue = valueToConvert;

			try
			{
				if (valueToConvert.IsAnyKnownMinValue())
				{
					returnValue = DateTime.SpecifyKind(valueToConvert, DateTimeKind.Unspecified);
				}
				else
				{
					valueToConvert = DateTime.SpecifyKind(valueToConvert, DateTimeKind.Utc);
					TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
					returnValue = TimeZoneInfo.ConvertTimeFromUtc(valueToConvert, easternTimeZone);
				}
			}
			catch { }

			return returnValue;
		}

		public static DateTime DatabaseValueEasternToUtcParse(this object databaseValueToConvert)
		{
			DateTime returnValue = DateTime.MinValue.MinValueUtc();

			if (null != databaseValueToConvert && databaseValueToConvert != DBNull.Value)
			{
				returnValue = DateTime.SpecifyKind(DateTime.Parse(databaseValueToConvert.SafeGetString(returnValue.ToString(CultureInfo.InvariantCulture))), DateTimeKind.Unspecified).EasternToUtc();

			}

			return returnValue;
		}


		public static DateTime MinValueUtc(this DateTime valueToConvert)
		{
			return DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
		}

		/// <summary>
		/// Parses an EST date string into UTC
		/// </summary>
		/// <param name="easternDateTime"></param>
		/// <param name="format"></param>
		/// <returns>EST DateTime represented in UTC</returns>
		public static DateTime ParseEasternTimeStringToUtcTime(this string easternDateTime, string format)
		{
			var localDateTime = DateTime.ParseExact(easternDateTime, format, CultureInfo.InvariantCulture);
			return localDateTime.EasternToUtc();
		}

		/// <summary>
		/// Creates a local time as if it were Eastern Standard Time, for example, 9am EST becomes 9am local. Use this if you want to display EST times from within another timezone.
		/// </summary>
		/// <param name="easternUtc"></param>
		/// <returns>EST DateTime represented in local UTC</returns>
		public static DateTime LocalTimeAsEasternUtc(this DateTime easternUtc)
		{
			var utcDate = DateTime.SpecifyKind(easternUtc, DateTimeKind.Utc);

			//convert from UTC to EST time and back to UTC (this will force local time to be the same ... i.e. we will display 9AM EST as 9AM even if we are in another timezone)
			utcDate = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(easternUtc, TimeZoneInfo.Utc.Id, "Eastern Standard Time");
			return utcDate.ToUniversalTime();
		}

		public static DateTime ToDateAtMidnight(this DateTime value)
		{
			return value.ToShortDateString().ToDateTime();
		}

		/// <summary>
		/// Checks to see if the Date is MinVal or a minval passed over in Json.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static bool IsDateMinValOrJsonMinVal(this DateTime date)
		{
			return (date == DateTime.MinValue || date.Date == DateTime.MinValue.Date.ToUniversalTime().Date);
		}
		public static bool IsSqlMinVal(this DateTime date)
		{

			var minDate = DateTime.MinValue.ToSqlMinValue();
			return minDate == date || minDate.Ticks == date.Ticks || minDate.Ticks == date.ToUniversalTime().Ticks || minDate.ToShortDateString() == date.ToShortDateString();

		}

		public static bool IsAnyKnownMinValue(this DateTime date)
		{
			return date.IsSqlMinVal() || date.IsDateMinValOrJsonMinVal();
		}

		public static DateTime ToSqlMinValue(this DateTime value)
		{
			return Convert.ToDateTime("1/1/1753");
		}

		public static object UtcToEasternDatabaseValue(this DateTime valueToConvert)
		{
			return valueToConvert.UtcToEastern().ToDatabaseValue();

		}

		public static object ToDatabaseValue(this DateTime valueToConvert)
		{

			if (Convert.ToDateTime("1/1/1753").Ticks >= valueToConvert.Ticks)
				return DBNull.Value;

			return valueToConvert;

		}
		#endregion

		#region Object Extensions...
		/// <summary>
		/// Checks object for null or DBNull.Value and converts ToString or 
		/// returns defaultNullValue
		/// </summary>
		/// <param name="dataValue"></param>
		/// <param name="defaultNullValue"></param>
		/// <returns></returns>
		public static string SafeGetString(this object dataValue, string defaultNullValue = "")
		{
			string returnValue = defaultNullValue;

			try
			{
				returnValue = dataValue != DBNull.Value ? Convert.ToString(dataValue ?? defaultNullValue) : defaultNullValue;

				if (null != returnValue)
					returnValue = returnValue.Trim();
			}
			catch
			{
				returnValue = defaultNullValue;
			}
			return returnValue;
		}

		public static T ConvertUtcDatesToEastern<T>(this T objectToConvert)
		{
			return (T)((object)objectToConvert).ConvertUtcDatesToEastern();
		}

		public static object ConvertUtcDatesToEastern(this object objectToConvert)
		{
			if (objectToConvert != null)
			{
				if (objectToConvert is DateTime)
				{
					objectToConvert = ((DateTime)objectToConvert).UtcToEastern();
				}
				else if (objectToConvert.GetType().IsCollection())
				{
					int count = objectToConvert.GetCollectionCount();


					for (int index = 0; index < count; index++)
					{
						object collectionItem = objectToConvert.GetCollectionItem(index);

						if (null != collectionItem)
						{
							collectionItem.ConvertUtcDatesToEastern();
						}
					}

				}
				// is it an object?
				else if (objectToConvert.GetType().IsClass && !(objectToConvert is string))
				{
					foreach (PropertyInfo prop in objectToConvert.GetType().GetProperties())
					{
						try
						{
							if (prop.CanRead && prop.CanWrite)
							{
								//Check for a DateTime
								if (prop.PropertyType == typeof(DateTime))
								{
									DateTime input = (DateTime)prop.GetValue(objectToConvert, null);

									prop.SetValue(objectToConvert, input.UtcToEastern());
								}
								//Check for a nullable datetime
								else if (prop.PropertyType == typeof(DateTime?))
								{
									DateTime? input = (DateTime?)prop.GetValue(objectToConvert, null);

									if (input.HasValue)
									{
										prop.SetValue(objectToConvert, (DateTime?)Convert.ToDateTime(input).UtcToEastern());
									}

								}
								else if (prop.PropertyType.IsCollection())
								{
									if (null != prop.GetValue(objectToConvert, null))
									{
										int count = prop.GetCollectionCount(objectToConvert);
										for (int index = 0; index < count; index++)
										{
											object collectionItem = prop.GetCollectionItem(objectToConvert, index);

											if (null != collectionItem)
											{
												collectionItem.ConvertUtcDatesToEastern();
											}
										}
									}
								}
								// is it an object?
								else if (prop.PropertyType.IsClass && !(prop.PropertyType == typeof(string)))
								{
									if (null != prop.GetValue(objectToConvert, null))
									{
										prop.GetValue(objectToConvert, null).ConvertUtcDatesToEastern();
									}
								}

							}
						}
						catch { }
					}
				}
			}
			return objectToConvert;
		}

		public static T ConvertEasternDatesToUtc<T>(this T objectToConvert)
		{
			return (T)((object)objectToConvert).ConvertEasternDatesToUtc();
		}
		public static object ConvertEasternDatesToUtc(this object objectToConvert)
		{
			if (objectToConvert != null)
			{
				if (objectToConvert is DateTime)
				{
					objectToConvert = ((DateTime)objectToConvert).EasternToUtc();
				}
				else if (objectToConvert.GetType().IsCollection())
				{
					int count = objectToConvert.GetCollectionCount();


					for (int index = 0; index < count; index++)
					{
						object collectionItem = objectToConvert.GetCollectionItem(index);

						if (null != collectionItem)
						{
							collectionItem.ConvertEasternDatesToUtc();
						}
					}

				}
				// is it an object?
				else if (objectToConvert.GetType().IsClass && !(objectToConvert is string))
				{
					foreach (PropertyInfo prop in objectToConvert.GetType().GetProperties())
					{
						try
						{
							if (prop.CanRead && prop.CanWrite)
							{
								//Check for a DateTime
								if (prop.PropertyType == typeof(DateTime))
								{
									DateTime input = (DateTime)prop.GetValue(objectToConvert, null);

									prop.SetValue(objectToConvert, input.EasternToUtc());
								}
								//Check for a nullable datetime
								else if (prop.PropertyType == typeof(DateTime?))
								{
									DateTime? input = (DateTime?)prop.GetValue(objectToConvert, null);

									if (input.HasValue)
									{
										prop.SetValue(objectToConvert, (DateTime?)Convert.ToDateTime(input).EasternToUtc());
									}

								}
								else if (prop.PropertyType.IsCollection())
								{
									if (null != prop.GetValue(objectToConvert, null))
									{
										int count = prop.GetCollectionCount(objectToConvert);
										for (int index = 0; index < count; index++)
										{
											object collectionItem = prop.GetCollectionItem(objectToConvert, index);

											if (null != collectionItem)
											{
												collectionItem.ConvertEasternDatesToUtc();
											}
										}
									}
								}
								// is it an object?
								else if (prop.PropertyType.IsClass && !(prop.PropertyType == typeof(string)))
								{
									if (null != prop.GetValue(objectToConvert, null))
									{
										prop.GetValue(objectToConvert, null).ConvertEasternDatesToUtc();
									}
								}

							}
						}
						catch { }
					}
				}
			}
			return objectToConvert;
		}
		#endregion

		#region Decimal Extensions...

		public static string ToMoney(this decimal value)
		{
			return value.ToString("$###,###,###.#0");
		}

		public static string ToHundredthsDecimalPlace(this decimal value)
		{
			return value.ToString("#########0.00");
		}
		#endregion

		#region String Extensions...


		public static DateTime ToDateTime(this string value)
		{
			return (!String.IsNullOrEmpty(value) && value.IsValidDate()) ? Convert.ToDateTime(value) : DateTime.MinValue;

		}

		public static string RemoveInvalidHostCharacters(this string valueToCheck)
		{

			string returnValue = valueToCheck;

			try
			{
				if (!String.IsNullOrEmpty(valueToCheck))
					returnValue = InvalidHostCharactersRegex.Replace(valueToCheck.Trim(), string.Empty);

			}
			catch
			{
			}

			return returnValue;
		}

		public static string RemoveInvalidBasicCharacters(this string valueToCheck)
		{

			string returnValue = valueToCheck;

			try
			{
				if (!String.IsNullOrEmpty(valueToCheck))
					returnValue = InvalidBasicCharactersRegex.Replace(valueToCheck.Trim(), string.Empty);

			}
			catch
			{
			}

			return returnValue;
		}

		public static string RemoveNonNumericCharacters(this string valueToCheck)
		{

			string returnValue = valueToCheck;

			try
			{
				returnValue = new Regex(@"[^0-9]+").Replace(valueToCheck.TrimSafe(), string.Empty);

			}
			catch
			{
			}

			return returnValue;
		}

		public static string ToPhoneNumber(this string value)
		{
			string returnValue = value.RemoveNonNumericCharacters();

			try
			{
				returnValue = (returnValue.Length > 10) ? returnValue.Substring(0, 10) : returnValue;
				returnValue = (returnValue.Length == 10) ? Regex.Replace(returnValue, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3") : (returnValue.Length == 7) ? Regex.Replace(returnValue, @"(\d{3})(\d{4})", "$1-$2") : returnValue;

			}
			catch
			{

			}

			return returnValue;

		}

		public static string ToZipCode(this string value)
		{
			string returnValue = value.RemoveNonNumericCharacters();

			try
			{
				returnValue = (returnValue.Length > 9) ? returnValue.Substring(0, 9) : returnValue;
				returnValue = (returnValue.Length == 9) ? Regex.Replace(returnValue, @"(\d{5})(\d{4})", "$1-$2") : (returnValue.Length == 5) ? Regex.Replace(returnValue, @"(\d{5})", "$1") : returnValue;

			}
			catch
			{

			}

			return returnValue;

		}

		public static string GetInvalidBasicCharacters(this string valueToCheck)
		{

			string returnValue = valueToCheck;

			try
			{
				if (!String.IsNullOrEmpty(valueToCheck))
					returnValue = ValidBasicCharactersRegex.Replace(valueToCheck.Trim(), string.Empty);

			}
			catch
			{
			}

			return returnValue;
		}

		public static string NullToEmptyString(this string value)
		{
			return value ?? string.Empty;
		}

		public static string TrimSafe(this string value)
		{
			return value.NullToEmptyString().Trim();
		}

		public static string CleanPaymentCardNumber(this string cardNumber)
		{
			return cardNumber.TrimSafe().Replace(" ", string.Empty).Replace("-", string.Empty);
		}
		#endregion

		#region Object Extensions...

		public static T CloneObject<T>(this T objSource)
		{
			return (T)((object)objSource).CloneObject();
		}
		public static object CloneObject(this object objSource)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				if (!objSource.GetType().IsSerializable)
					return null;
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, objSource);
				stream.Position = 0;
				return formatter.Deserialize(stream);
			}
		}

		public static T CloneObjectReflection<T>(this T objSource)
		{
			return (T)((object)objSource).CloneObjectReflection();
		}

		public static object CloneObjectReflection(this object objSource)
		{

			//Get the type of source object and create a new instance of that type

			Type typeSource = objSource.GetType();

			object objTarget = Activator.CreateInstance(typeSource);



			//Get all the properties of source object type

			PropertyInfo[] propertyInfo = typeSource.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);



			//Assign all source property to taget object 's properties

			foreach (PropertyInfo property in propertyInfo)
			{

				//Check whether property can be written to 

				if (property.CanWrite)
				{

					//check whether property type is value type, enum or string type

					if (property.PropertyType.IsValueType || property.PropertyType.IsEnum || property.PropertyType.Equals(typeof(System.String)))
					{

						property.SetValue(objTarget, property.GetValue(objSource, null), null);

					}

					//else property type is object/complex types, so need to recursively call this method until the end of the tree is reached

					else
					{

						object objPropertyValue = property.GetValue(objSource, null);

						if (objPropertyValue == null)
						{

							property.SetValue(objTarget, null, null);

						}

						else
						{

							property.SetValue(objTarget, objPropertyValue.CloneObjectReflection(), null);

						}

					}

				}

			}

			return objTarget;

		}


		/// <summary>
		/// Returns the serialized JSON Representation of an object
		/// </summary>
		/// <param name="input">Object to serialize</param>
		/// <param name="defaultFailureMessage"></param>
		/// <returns>Serialized JSON String</returns>
		public static string SerializeToJsonSafe(this object input, string defaultFailureMessage = "Unable to Serialize Object to JSON")
		{
			var returnValue = defaultFailureMessage;

			try
			{
				returnValue = input.SerializeToJson();
			}
			// ReSharper disable once EmptyGeneralCatchClause
			catch (Exception) { }

			return returnValue;

		}

		/// <summary>
		/// Returns the serialized JSON Representation of an object
		/// </summary>
		/// <param name="input">Object to serialize</param>
		/// <returns>Serialized JSON String</returns>
		public static string SerializeToJson(this object input)
		{
			string returnValue;

			using (var stream = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(input.GetType());
				serializer.WriteObject(stream, input);

				stream.Position = 0;
				using (var reader = new StreamReader(stream))
				{
					returnValue = reader.ReadToEnd();
				}
			}

			return returnValue;
		}

		public static object Deserialize(this string serializedObject, Type objectType)
		{
			object returnValue = null;

			using (var stream = new MemoryStream())
			{
				var serializer = new DataContractJsonSerializer(objectType);
				var uniEncoding = new UnicodeEncoding();

				// Create the data to write to the stream.
				byte[] serializedBytes = uniEncoding.GetBytes(serializedObject);
				stream.Write(serializedBytes, 0, serializedBytes.Length);

				stream.Position = 0;
				returnValue = serializer.ReadObject(stream);
			}

			return returnValue;
		}

		#endregion

	}
}

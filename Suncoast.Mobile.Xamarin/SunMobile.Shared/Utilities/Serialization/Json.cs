using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using SunMobile.Shared.Logging;

namespace Common.Utilities.Serialization
{
	public static class Json
	{
		public static string Serialize(object input)
		{
			string returnValue = string.Empty;

			try
			{
				if (null != input)
				{
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
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "Json:Serialize");
			}

			return returnValue;
		}

		public static T Deserialize<T>(string serializedObject)
		{
			var returnValue = Activator.CreateInstance<T>();

			try
			{


				if (!string.IsNullOrEmpty(serializedObject))
				{
					using (var stream = new MemoryStream())
					{
						var serializer = new DataContractJsonSerializer(typeof(T));
						var uniEncoding = new UnicodeEncoding();

						// Create the data to write to the stream.
						byte[] serializedBytes = uniEncoding.GetBytes(serializedObject);
						stream.Write(serializedBytes, 0, serializedBytes.Length);

						stream.Position = 0;
						returnValue = (T)serializer.ReadObject(stream);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, string.Format("Json:Deserialize: {0}", serializedObject ?? string.Empty));
			}

			return returnValue;
		}
	}
}
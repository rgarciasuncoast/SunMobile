using System;
using System.IO;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;

namespace SunMobile.Shared.Utilities.Settings
{
	public class IsolatedStorage
	{
		public static void SaveData<T>(string fileName, T data)
		{
			try 
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var file = store.OpenFile(fileName, FileMode.Create, FileAccess.Write))
					{
						using (var sw = new StreamWriter(file))
						{
							string jsonData = JsonConvert.SerializeObject(data);

	                        sw.WriteLine(jsonData);
							sw.Close();
						}

						file.Close();
					}
				}
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "IsolatedStorageHelper:SaveData<T>");
			}
		}

        public static void SaveData(string fileName, string data)
        {
            try
            {
	            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
	            {
	                using (var file = store.OpenFile(fileName, FileMode.Create, FileAccess.Write))
	                {
	                    using (var sw = new StreamWriter(file))
	                    {
	                        sw.WriteLine(data);
							sw.Close();
	                    }

						file.Close();
	                }
	            }
            }
            catch(Exception ex)
            {
				Logging.Logging.Log(ex, "IsolatedStorageHelper:SaveData");
            }
        }

		public static string SaveBytesToFile(string fileName, byte[] data, string fullFileName = null)
		{
			try
			{
				if (string.IsNullOrEmpty(fullFileName))
				{
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = Path.GetFileName(Path.GetTempFileName());
                    }

					string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					fullFileName = Path.Combine(path, fileName);
				}

				File.WriteAllBytes(fullFileName, data);
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "IsolatedStorageHelper:SaveBytesToFile");
			}

			return fullFileName;
		}

		public static byte[] LoadBytesFromFile(string fileName)
		{
			byte[] returnValue = null;

			try
			{
				returnValue = File.ReadAllBytes(fileName);
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "IsolatedStorageHelper:LoadBytesFromFile");
			}

			return returnValue;
		}

		public static void DeleteFile(string fileName)
		{
			try
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					if (File.Exists(fileName))
					{
						File.Delete(fileName);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "IsolatedStorageHelper:DeleteFile");
			}
		}

		public static T LoadData<T>(string fileName) where T : new()
		{
            try 
            {
    			using (var store = IsolatedStorageFile.GetUserStoreForApplication())
    			{
    				if (!store.FileExists(fileName))
    				{
    					return new T();
    				}

    				using (var file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
    				{
    					using (var sw = new StreamReader(file))
    					{
    						string data = sw.ReadToEnd();

                        	return JsonConvert.DeserializeObject<T>(data);
    					}
    				}
    			}
            }
            catch(Exception ex)
            {
				Logging.Logging.Log(ex, "IsolatedStorageHelper:LoadData<T>");
                return new T();
            }
		}

        public static string LoadData(string fileName)
        {
            try 
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!store.FileExists(fileName))
                    {
                        return string.Empty;
                    }

                    using (var file = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var sw = new StreamReader(file))
                        {
                            string data = sw.ReadToEnd();

							if (data.EndsWith("\n", StringComparison.InvariantCulture))
							{
								data = data.Substring(0, data.Length - 1);
							}

                            return data;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
				Logging.Logging.Log(ex, "IsolatedStorageHelper:LoadData");                
                return string.Empty;
            }
        }

        public static void ClearData(string fileName)
        {
			try
			{
	            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
	            {
	                if (store.FileExists(fileName))
	                {
						GC.Collect();
						GC.WaitForPendingFinalizers();
	                    store.DeleteFile(fileName);
	                }
	            }
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "IsolatedStorageHelper:ClearData");
			}
        }
	}
}
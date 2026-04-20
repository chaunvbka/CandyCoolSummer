#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable IDE0051 // Remove unused private members

namespace Texell.CoreModule
{
    using System;
    using System.Text;
    using System.IO;
    using UnityEngine;
    using Texell.Utility;

    public class AppDataManager : IDisposable
    {
        private bool _dispose = false;
        private static AppDataManager s_Instance;
        public static AppDataManager Instance => s_Instance;

        private readonly StringBuilder _stringBuilder = new("");

        /// <summary>
        /// The path in which data file save in.
        /// </summary>
        private readonly string _path;
        private readonly Data _data;

        public AppDataManager()
        {
            if (s_Instance != null)
            {
                Debug.LogError("AppDataManager instance already exists. Cannot create a new one.");
                return;
            }
            s_Instance = this;

            _data = Resources.Load<Data>("Data");
            if (_data == null)
            {
                Debug.LogError("Load Data object fail !");
                return;
            }

            string appName = _data.AppName;
            if (string.IsNullOrEmpty(appName))
            {
                Debug.LogError("The application name is not setup!");
                return;
            }

            if (_data.Arrays == null)
            {
                _data.Arrays = new string[Enum.GetValues(typeof(DataIndex)).Length];
            }

#if UNITY_EDITOR

            _stringBuilder.Append(appName);
            _stringBuilder.Append(".dat");

#elif UNITY_ANDROID || UNITY_IOS

            _stringBuilder.Append(Application.persistentDataPath);
            _stringBuilder.Append("/");
            _stringBuilder.Append(appName);
            _stringBuilder.Append(".dat");
            
#endif

            _path = _stringBuilder.ToString();

            if (!File.Exists(_path))
            {
                FileStream fs = File.Create(_path);
                // Close file stream to release the file lock, before reading it.
                fs.Close();
            }

            // Load json data to scriptable object '_data'.
            LoadData();
        }

        void LoadData()
        {
            string json = File.ReadAllText(_path);
            JsonUtility.FromJsonOverwrite(json, _data);
            if (_data.Arrays != null && _data.Arrays.Length != Enum.GetValues(typeof(DataIndex)).Length)
            {
                _data.Arrays = new string[Enum.GetValues(typeof(DataIndex)).Length];
            }
            //Debug.Log("App data loaded.");
        }

        async void LoadDataAsync()
        {
            string json = await File.ReadAllTextAsync(_path);
            JsonUtility.FromJsonOverwrite(json, _data);
            if (_data.Arrays != null && _data.Arrays.Length != Enum.GetValues(typeof(DataIndex)).Length)
            {
                _data.Arrays = new string[Enum.GetValues(typeof(DataIndex)).Length];
            }
            //Debug.Log("App data loaded.");
        }

        /// <summary>
        /// Write json data to file.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="index"></param>
        public void WriteData(string jsonData, DataIndex index)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);
            string hex = BitConverter.ToString(bytes).Replace("-", string.Empty);
            _data.Arrays[(int)index] = hex;
            string json = JsonUtility.ToJson(_data);

            File.WriteAllText(_path, json);
        }

        /// <summary>
        /// Write json data to file asynchronously.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="index"></param>
        public async void WriteDataAsync(string jsonData, DataIndex index)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(jsonData);
            string hex = BitConverter.ToString(bytes).Replace("-", string.Empty);
            _data.Arrays[(int)index] = hex;
            string json = JsonUtility.ToJson(_data);

            await File.WriteAllTextAsync(_path, json);
        }

        /// <summary>
        /// Read json data at Arrays[index] from scriptable object. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string ReadData(DataIndex index)
        {
            string hex = _data.Arrays[(int)index];
            byte[] bytes = UtilityFunctions.HexStringToBytes(hex);
            string json = Encoding.UTF8.GetString(bytes);
            return json;
        }

        public void Dispose()
        {
            if (_dispose) return;
            _dispose = true;

            s_Instance = null;
        }

        ~AppDataManager()
        {
            Dispose();
        }
    }

}

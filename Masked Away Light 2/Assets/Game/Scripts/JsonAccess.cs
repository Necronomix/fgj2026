using System.IO;
using UnityEngine;

namespace Masked.Utils
{
    public static class JsonAccess
    {

        public static T FetchOrCreateJson<T>(T defaultData, string path) where T : class, IData
        {
            T data = null;
            if (File.Exists(path))
            {
                var file = File.ReadAllText(path);

                if (file != null && file.Length > 0)
                {
                    data = JsonUtility.FromJson<T>(file);
                }
            }
            else
            {
                data = defaultData;

                var asJson = JsonUtility.ToJson(data);
                File.WriteAllText(path, asJson);
#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Wrote to {path}");
#endif
            }

            return data;
        }

        public static void UpdateData<T>(T data, string path) where T : IData
        {
            if (!data.Changed)
            {
                return;
            }
            if (File.Exists(path))
            {
                var asJson = JsonUtility.ToJson(data);
                File.WriteAllText(path, asJson);

#if UNITY_EDITOR
                UnityEngine.Debug.Log($"Updated data at {path}");
#endif
            }
        }
    }
}

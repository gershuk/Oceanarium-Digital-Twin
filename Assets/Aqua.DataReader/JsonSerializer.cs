using System.IO;

using UnityEngine;

namespace Aqua.DataReader
{
    public static class JsonSerializer
    {
        public static T DeserializeObject<T> (string path) => JsonUtility.FromJson<T>(File.ReadAllText(path));

        public static void SerializeObject<T> (T obj, string path) => File.WriteAllText(path, JsonUtility.ToJson(obj));
    }
}
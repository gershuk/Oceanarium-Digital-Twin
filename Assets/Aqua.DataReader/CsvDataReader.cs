#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Aqua.DataReader
{
    public struct Data<T>
    {
        public Data (DateTime date, T? value)
        {
            Date = date;
            Value = value;
        }

        public DateTime Date { get; }
        public T? Value { get; }
    }

    public class ComparerDataByTime<T> : IComparer<Data<T>>
    {
        public static ComparerDataByTime<T> Instance = new();
        public int Compare (Data<T> x, Data<T> y) => x.Date.CompareTo(y.Date);
    }

    public class DataArray<T>
    {
        public string ParameterName { get; }
        public Data<T>[] Data { get; }

        public DataArray (string parameterName, Data<T>[] data)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }

    public readonly struct StringSignature
    {
        public StringSignature (string name, string type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Name { get; }
        public string Type { get; }

        public override string ToString () => $"{nameof(Name)}:'{Name}' {nameof(Type)}:'{Type}'";
    }

    public readonly struct StringData
    {
        public StringData (string date, string value)
        {
            Date = date ?? throw new ArgumentNullException(nameof(date));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Date { get; }
        public string Value { get; }

        public override string ToString () => $"{nameof(Date)}:'{Date}' {nameof(Value)}:'{Value}'";
    }

    public static class CsvDataReader
    {
        public static Dictionary<StringSignature, List<StringData>> ReadData (string filePath)
        {
            var res = new Dictionary<StringSignature, List<StringData>>();
            foreach (var str in File.ReadLines(filePath))
            {
                var subStr = Regex.Matches(str, "\"[,.$: \\w-]*\"");

                var paramName = subStr[0].Value.Replace("\"", string.Empty);
                var date = subStr[1].Value.Replace("\"", string.Empty);
                var type = subStr[2].Value.Replace("\"", string.Empty);
                var value = subStr[3].Value.Replace("\"", string.Empty);

                var signature = new StringSignature(paramName, type);
                var data = new StringData(date, value);

                if (!res.TryGetValue(signature, out var list))
                {
                    list = new List<StringData>();
                    res[signature] = list;
                }
                list.Add(data);
            }

            return res;
        }

        //SYSTEM
        //INTEGER
        //REAL
        //DISCRETE

        #region DataArray creation methods
        private static DateTime ParseDate (string date) => DateTime.ParseExact(date,
                                                                               "yyyy-MM-dd HH:mm:ss",
                                                                               System.Globalization.CultureInfo.InvariantCulture);

        public static bool TryCreateDataArrayFromReal (StringSignature signature, List<StringData> list, out DataArray<float>? dataArray)
        {
            if (signature.Type != "REAL")
            {
                Debug.LogError("Wrong signature type");
                dataArray = null;
                return false;
            }

            var array = new Data<float>[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                try
                {
                    array[i] = new Data<float>(ParseDate(list[i].Date), Convert.ToSingle(list[i].Value));
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Wrong data type. Exception: {exc.Message}");
                    dataArray = null;
                    return false;
                }
            }

            dataArray = new DataArray<float>(signature.Name, array);
            return true;
        }

        public static bool TryCreateDataArrayFromInteger (StringSignature signature, List<StringData> list, out DataArray<int>? dataArray)
        {
            if (signature.Type != "INTEGER")
            {
                Debug.LogError("Wrong signature type");
                dataArray = null;
                return false;
            }

            var array = new Data<int>[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                try
                {
                    array[i] = new Data<int>(ParseDate(list[i].Date), Convert.ToInt32(list[i].Value));
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Wrong data type. Exception: {exc.Message}");
                    dataArray = null;
                    return false;
                }
            }

            dataArray = new DataArray<int>(signature.Name, array);
            return true;
        }

        public static bool TryCreateDataArrayFromSystem (StringSignature signature, List<StringData> list, out DataArray<int>? dataArray)
        {
            if (signature.Type != "SYSTEM")
            {
                Debug.LogError("Wrong signature type");
                dataArray = null;
                return false;
            }

            var array = new Data<int>[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                try
                {
                    array[i] = new Data<int>(ParseDate(list[i].Date),Convert.ToInt32(list[i].Value));
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Wrong data type. Exception: {exc.Message}");
                    dataArray = null;
                    return false;
                }
            }

            dataArray = new DataArray<int>(signature.Name, array);
            return true;
        }

        public static bool TryCreateDataArrayFromDiscrete (StringSignature signature, List<StringData> list, out DataArray<bool>? dataArray)
        {
            if (signature.Type != "DISCRETE")
            {
                Debug.LogError("Wrong signature type");
                dataArray = null;
                return false;
            }

            var array = new Data<bool>[list.Count];

            for (var i = 0; i < list.Count; i++)
            {
                try
                {
                    array[i] = new Data<bool>(ParseDate(list[i].Date), list[i].Value switch
                                                                        {
                                                                            "0" =>false,
                                                                            "1" =>true,
                                                                            _ => throw new NotImplementedException(),
                                                                        });
                }
                catch (Exception exc)
                {
                    Debug.LogError($"Wrong data type. Exception: {exc.Message}");
                    dataArray = null;
                    return false;
                }
            }

            dataArray = new DataArray<bool>(signature.Name, array);
            return true;
        }
        #endregion
    }
}
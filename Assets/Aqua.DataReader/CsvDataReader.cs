#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Aqua.DataReader
{
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
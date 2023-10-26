#nullable enable

using System;
using System.Collections.Generic;

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
}

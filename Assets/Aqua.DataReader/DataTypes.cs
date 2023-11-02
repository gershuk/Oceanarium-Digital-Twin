#nullable enable

using System;
using System.Collections.Generic;

namespace Aqua.DataReader
{
    public readonly struct Data<T>
    {
        public DateTime Date { get; }

        public T? Value { get; }

        public Data (DateTime date, T? value)
        {
            Date = date;
            Value = value;
        }
    }

    public readonly struct StringData
    {
        public string Date { get; }

        public string Value { get; }

        public StringData (string date, string value)
        {
            Date = date ?? throw new ArgumentNullException(nameof(date));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override string ToString () => $"{nameof(Date)}:'{Date}' {nameof(Value)}:'{Value}'";
    }

    public readonly struct StringSignature
    {
        public string Name { get; }

        public string Type { get; }

        public StringSignature (string name, string type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override string ToString () => $"{nameof(Name)}:'{Name}' {nameof(Type)}:'{Type}'";
    }

    public class ComparerDataByTime<T> : IComparer<Data<T>>
    {
        public static ComparerDataByTime<T> Instance = new();

        public int Compare (Data<T> x, Data<T> y) => x.Date.CompareTo(y.Date);
    }

    public class DataArray<T>
    {
        public Data<T>[] Data { get; }
        public string ParameterName { get; }

        public DataArray (string parameterName, Data<T>[] data)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }
}
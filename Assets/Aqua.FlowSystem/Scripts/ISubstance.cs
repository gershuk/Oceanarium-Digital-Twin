#nullable enable

namespace Aqua.FlowSystem
{
    public interface ISubstance
    {
        public double PH { get; }
        public double Temperature { get; }
        public double Volume { get; }
    }

    public interface ISubstanceOperations<T> where T : struct, ISubstance
    {
        public T Combine (params T[] substances);

        public bool IsVolumeApproximatelyEqual (double value, double eps);

        public bool IsVolumeApproximatelyLess (double value, double eps);

        public T[] Separate (params double[] coefficients);
    }
}
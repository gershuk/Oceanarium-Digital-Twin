#nullable enable

namespace Aqua.FlowSystem
{
    public interface ISubstance
    {
        public double Volume { get; }
    }

    public interface ISubstanceOperations<T> where T : struct, ISubstance
    {
        public T[] Separate (params double[] coefficients);

        public T Combine (params T[] substances);

        public bool IsVolumeApproximatelyEqual (double value, double eps);

        public bool IsVolumeApproximatelyLess (double value, double eps);
    }
}
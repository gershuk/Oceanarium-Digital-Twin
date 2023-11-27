#nullable enable

namespace Aqua.FlowSystem
{
    public interface ISubstance
    {
        public double Volum { get; }
    }

    public interface ISubstanceOperations<T> where T : struct, ISubstance
    {
        public T[] Separate (params double[] coefficients);

        public T Combine (params T[] substances);

        public bool IsVolumApproximatelyEqual (double value, double eps);
    }
}

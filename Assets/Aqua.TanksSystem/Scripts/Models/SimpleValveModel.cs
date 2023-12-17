using System;

namespace Aqua.TanksSystem.Models
{
    public sealed class SimpleValveModel : Source<float>
    {
        protected override float InputDataModificationFunction (float value) =>
                                                                value is >= 0 and <= 1
                                                                ? value
                                                                : throw new ArgumentOutOfRangeException(nameof(value));

        public SimpleValveModel(float value = 0) : base(value, true)
        {

        }
    }
}
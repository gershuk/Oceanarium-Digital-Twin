#nullable enable

namespace Aqua.TanksSystem
{
    public record WaterData (float Volume, float Ph, float Temp)
    {
        public static WaterData operator + (WaterData a, WaterData b) =>
            new((a?.Volume ?? 0) + (b?.Volume ?? 0),
                (a?.Temp ?? 0 * a?.Volume ?? 0 + b?.Temp ?? 0 * b?.Temp ?? 0) / (a?.Volume ?? 0 + b?.Volume ?? 0),
                a?.Ph ?? 0 + b?.Ph ?? 0);
    }
}
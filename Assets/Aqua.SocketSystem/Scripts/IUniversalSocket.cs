#nullable enable

namespace Aqua.SocketSystem
{
    public interface IUniversalSocket<TIn, TOut> : IInputSocket<TIn>, IOutputSocket<TOut>
    {
    }
}
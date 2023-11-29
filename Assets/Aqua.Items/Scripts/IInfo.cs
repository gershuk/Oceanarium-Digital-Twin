#nullable enable

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    public interface IInfo
    {
        public IOutputSocket<string?> DescriptionSocket { get; }

        public IOutputSocket<string?> NameSocket { get; }

        public IOutputSocket<Sprite?> SpriteSocket { get; }
    }

    public class EmptyInfo : IInfo
    {
        public static EmptyInfo Instance = new();

        public IOutputSocket<string?> DescriptionSocket { get; } = new MulticonnectionSocket<string?, string>();

        public IOutputSocket<string?> NameSocket { get; } = new MulticonnectionSocket<string?, string>();

        public IOutputSocket<Sprite?> SpriteSocket { get; } = new MulticonnectionSocket<Sprite?, Sprite>();
    }
}
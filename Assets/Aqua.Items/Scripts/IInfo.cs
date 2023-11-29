using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.Items
{
    public interface IInfo
    {
        public IOutputSocket<string> DescriptionSocket { get; }

        public IOutputSocket<string> NameSocket { get; }

        public IOutputSocket<Sprite> SpriteSocket { get; }
    }
}
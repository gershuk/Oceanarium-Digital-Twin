#nullable enable

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public sealed class CursorViewModel : MonoBehaviour
    {
        [SerializeField]
        private Texture2D _cursorTexture;
        [SerializeField]
        private CursorMode _cursorMode = CursorMode.Auto;
        [SerializeField]
        private Vector2 _hotSpot = Vector2.zero;

        public Texture2D CursorTexture { get => _cursorTexture; set => _cursorTexture = value; }
        public CursorMode CursorMode { get => _cursorMode; set => _cursorMode = value; }
        public Vector2 HotSpot { get => _hotSpot; set => _hotSpot = value; }

        private void Start () => Cursor.SetCursor(CursorTexture, HotSpot, CursorMode);
    }
}

#nullable enable

using UnityEngine;

namespace Aqua.UIBaseElements
{
    public sealed class CursorViewModel : MonoBehaviour
    {
        [SerializeField]
        private CursorMode _cursorMode = CursorMode.Auto;

        [SerializeField]
        private Texture2D _cursorTexture;

        [SerializeField]
        private Vector2 _hotSpot = Vector2.zero;

        public CursorMode CursorMode { get => _cursorMode; set => _cursorMode = value; }
        public Texture2D CursorTexture { get => _cursorTexture; set => _cursorTexture = value; }
        public Vector2 HotSpot { get => _hotSpot; set => _hotSpot = value; }

        public bool IsCursorAcitve
        {
            get => Cursor.visible;
            set
            {
                Cursor.visible = value;
                Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            }
        }

        private void Start () => Cursor.SetCursor(CursorTexture, HotSpot, CursorMode);
    }
}
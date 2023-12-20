#nullable enable

using Aqua.SocketSystem;

using TMPro;

using UniRx;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class SelectFrameView : MonoBehaviour
    {
        #region UI elements

        [Header("UI elements")]
        [SerializeField]
        private Image _innerImage;

        [SerializeField]
        private Image _selectFrame;

        [SerializeField]
        private TMP_Text _text;

        #endregion UI elements

        #region Select frame sprites

        [Header("Select frame sprites")]
        [SerializeField]
        private Sprite _selectedStateSprite;

        [SerializeField]
        private Sprite _unselectedStateSprite;

        #endregion Select frame sprites

        #region Sockets
        private readonly UniversalSocket<Sprite?, Sprite?> _innerImageSpriteSocket = new();
        private readonly UniversalSocket<bool, bool> _isSelectedSocket = new();
        private readonly UniversalSocket<string?, string?> _textSocket = new();
        public IInputSocket<Sprite?> InnerImageSpriteSocket => _innerImageSpriteSocket;
        public IInputSocket<bool> IsSelectedSocket => _isSelectedSocket;
        public IInputSocket<string?> TextSocket => _textSocket;
        #endregion Sockets

        public bool IsSelected
        {
            get => _isSelectedSocket.GetValue();
            set
            {
                if (!_isSelectedSocket.TrySetValue(value))
                {
                    Debug.LogWarning("Can't set value when socket has input connection");
                }
            }
        }

        private void Awake ()
        {
            _isSelectedSocket.ReadOnlyProperty.Subscribe(SetGraphicalState).AddTo(this);
            _innerImageSpriteSocket.ReadOnlyProperty.Subscribe(SetInnerSprite).AddTo(this);
            _textSocket.ReadOnlyProperty.Subscribe(SetText).AddTo(this);
        }

        private void SetGraphicalState (bool isSelected) =>
                    _selectFrame.sprite = isSelected
                                ? _selectedStateSprite
                                : _unselectedStateSprite;

        private void SetInnerSprite (Sprite? sprite) => _innerImage.sprite = sprite;

        private void SetText (string? text) => _text.text = text ?? string.Empty;
    }
}
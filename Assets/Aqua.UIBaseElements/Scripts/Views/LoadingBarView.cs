using UniRx;

using Aqua.SocketSystem;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public sealed class LoadingBarView : MonoBehaviour
    {
        [SerializeField]
        private Image _handle;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private Image _filler;

        private readonly IUniversalSocket<float, float> _progressSocket = new UniversalSocket<float, float>(0);
        public IInputSocket<float> ProgressSocket => _progressSocket;

        private void UpdateProgress (float value)
        {
            _filler.fillAmount = value;
            _handle.rectTransform.anchoredPosition = new Vector2(_filler.rectTransform.rect.width * value, 0);
        }

        private void Awake ()
        {
            _progressSocket.ReadOnlyProperty.Subscribe(UpdateProgress).AddTo(this);
        }
    }
}

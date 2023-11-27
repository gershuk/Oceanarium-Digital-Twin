#nullable enable

using UniRx;
using UniRx.Triggers;

using UnityEngine;

using static Aqua.UIBaseElements.GUIFactory;

namespace Aqua.UIBaseElements
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class HorizontalPercentLayout : MonoBehaviour
    {
        private RectTransform[] _children;

        [SerializeField]
        private float[] _fractions;

        [SerializeField]
        [Range(0f, 1000f)]
        private float _horizontalSpace;

        private RectTransform _transform;

        private void Start ()
        {
            _fractions ??= new float[1];
            _transform = GetComponent<RectTransform>();
            _transform.OnRectTransformDimensionsChangeAsObservable().Subscribe(v => UpdateChildWidth());
        }

        [ContextMenu(nameof(UpdateChildWidth))]
        private void UpdateChildWidth ()
        {
            _children = new RectTransform[_transform.childCount];
            for (var i = 0; i < _transform.childCount; ++i)
                _children[i] = _transform.GetChild(i).GetComponent<RectTransform>();

            var offset = 0f;
            for (var i = 0; i < _children.Length; i++)
            {
                var child = _children[i];
                var newWidth = _transform.rect.width * _fractions[i];

                var parameters = new RectTransformParameters
                (
                    _transform,
                    new Vector2(0, 0.5f),
                    new Vector2(0, 0.5f),
                    new Vector2(newWidth, _transform.rect.height),
                    new Vector2(offset + (newWidth / 2), 0)
                );

                child.SetUpRectTransform(parameters);

                offset += newWidth + _horizontalSpace;
                ;
            }
        }
    }
}
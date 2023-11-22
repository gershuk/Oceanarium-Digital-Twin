#nullable enable

using UniRx;

using UnityEngine;

namespace Aqua.TanksSystem
{
    public class SimpleWaterTankObjectView : MonoBehaviour
    {
        [SerializeField]
        protected Transform _base;

        [SerializeField]
        protected Vector3 _endPosition;

        [SerializeField]
        protected float _maxLevel;

        [SerializeField]
        protected Vector3 _startPosition;

        [SerializeField]
        protected SimpleWaterTankViewModel _viewmodel;

        [SerializeField]
        protected Transform _water;

        protected void DataListener (WaterData? waterData) => SetLevel((waterData?.Volume ?? 0) / _maxLevel);

        protected void SetLevel (float value) =>
                    _water.position = _base.TransformVector(Vector3.Lerp(_startPosition, _endPosition, value));

        protected void Start ()
        {
            if (_viewmodel == null)
                _viewmodel = GetComponent<SimpleWaterTankViewModel>();

            _viewmodel.DataSocket.ReadOnlyProperty.Subscribe(DataListener).AddTo(this);
        }
    }
}
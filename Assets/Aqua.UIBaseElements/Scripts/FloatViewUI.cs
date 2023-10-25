#nullable enable

using System;
using System.Collections;

using Aqua.SocketSystem;

using TMPro;

using UniRx;
using UniRx.Triggers;

using UnityEngine;
using UnityEngine.UI;

namespace Aqua.UIBaseElements
{
    public class FloatViewUI : MonoBehaviour
    {
        #region UnityUI components

        [SerializeField]
        protected Button _decreaseButton;

        [SerializeField]
        protected Button _increaseButton;

        [SerializeField]
        protected Button _resetButton;

        [SerializeField]
        protected TMP_Text _textMesh;

        #endregion UnityUI components

        protected readonly IUniversalSocket<float, float> _universalSocket = new UniversalSocket<float, float>();
        protected float _lastActionTime = 0f;
        protected ReactiveCommand DecreaseValueCommand { get; } = new();
        protected ReactiveCommand IncreaseValueCommand { get; } = new();
        protected ReactiveCommand ResetValueCommand { get; } = new();
        protected Coroutine? ValueChangeCorutine { get; set; }
        public IInputSocket<float> Socket => _universalSocket;

        protected IEnumerator ChangeValue (Action changAction)
        {
            while (true)
            {
                changAction();
                yield return null;
            }
        }

        protected void RegisterButtons ()
        {
            _decreaseButton.OnPointerDownAsObservable().Subscribe((v) => StartChangingValueCoroutine(DecreaseValue));
            _decreaseButton.OnPointerUpAsObservable().Subscribe((v) => StopChangingValueCoroutine());

            _increaseButton.OnPointerDownAsObservable().Subscribe((v) => StartChangingValueCoroutine(IncreaseValue));
            _increaseButton.OnPointerUpAsObservable().Subscribe((v) => StopChangingValueCoroutine());

            _resetButton.OnClickAsObservable().Subscribe((v) => ResetValue());
        }

        protected virtual void Start ()
        {
            _universalSocket.ReadOnlyProperty.Subscribe((v) => _textMesh.text = v.ToString("00.00")).AddTo(this);

            RegisterButtons();
        }

        protected void StartChangingValueCoroutine (Action changAction)
        {
            if (ValueChangeCorutine != null)
                throw new Exception("Coroutine already exist");
            ValueChangeCorutine = StartCoroutine(ChangeValue(changAction));
        }

        protected void StopChangingValueCoroutine ()
        {
            if (ValueChangeCorutine == null)
                throw new Exception("Coroutine does not exist");
            StopCoroutine(ValueChangeCorutine);
            ValueChangeCorutine = null;
        }

        #region Rotation methods

        protected bool TryExecuteCommand ()
        {
            var cond = Time.time - _lastActionTime - Time.deltaTime >= float.Epsilon;
            if (cond)
                _lastActionTime = Time.time;
            return cond;
        }

        [ContextMenu(nameof(DecreaseValue))]
        public void DecreaseValue ()
        {
            if (TryExecuteCommand())
                DecreaseValueCommand.Execute();
        }

        [ContextMenu(nameof(IncreaseValue))]
        public void IncreaseValue ()
        {
            if (TryExecuteCommand())
                IncreaseValueCommand.Execute();
        }

        [ContextMenu(nameof(ResetValue))]
        public void ResetValue ()
        {
            if (TryExecuteCommand())
                ResetValueCommand.Execute();
        }

        #endregion Rotation methods
    }
}
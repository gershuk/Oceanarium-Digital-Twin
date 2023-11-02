#nullable enable

using System;

using Aqua.SocketSystem;
using Aqua.TanksSystem.Models;

using UnityEngine;

namespace Aqua.TanksSystem.ViewModels
{
    public sealed class ValveViewModel : MonoBehaviour
    {
        private SimpleValveModel SimpleValve { get; set; }

        public IOutputSocket<float> Output => SimpleValve.OutputSocket;

        private void Awake () => SimpleValve = new();

        public void SetValue (float value) => SimpleValve.Value = value;

        // ToDo : Remove with bindings
        #region Rotation method

        public void AddValue (float delta, float deltaTime)
        {
            try
            {
                SimpleValve.Value = Mathf.Min(Mathf.Max(0, SimpleValve.Value + (delta * deltaTime)), 1);
            }
            catch (ArgumentOutOfRangeException outOfRange)
            {
                Debug.Log($"The value limit has been reached. Exception: {outOfRange.Message}");
            }
        }

        [ContextMenu(nameof(Close))]
        public void Close () => AddValue(-1, Time.deltaTime);

        public void Close (float delta, float deltaTime) => AddValue(delta, deltaTime);

        [ContextMenu(nameof(Open))]
        public void Open () => AddValue(1, Time.deltaTime);

        public void Open (float delta, float deltaTime) => AddValue(delta, deltaTime);

        [ContextMenu(nameof(ResetValue))]
        public void ResetValue () => SimpleValve.Value = 0;

        #endregion Rotation method
    }
}
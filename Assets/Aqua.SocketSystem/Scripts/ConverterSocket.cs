#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public sealed class ConverterSocket<TIn, TOut> : UniversalSocket<TIn?, TOut?>, IConverterSocket<TIn?, TOut?>
    {
        public Func<TIn?, TOut?>? MainInputData�onvertingFunction { get; private set; }

        public ConverterSocket () : base()
        {
        }

        public ConverterSocket (TOut? value = default) : base(value)
        {
        }

        public ConverterSocket (ReactiveProperty<TOut?>? property = null) : base(property)
        {
        }

        protected override void ResetMainDataFunction ()
        {
            base.ResetMainDataFunction();
            MainInputData�onvertingFunction = null;
        }

        protected override void UpdateData (TIn? value)
        {
            var mValue = MainInputDataModificationFunction != null
                             ? MainInputDataModificationFunction(value)
                             : value;

            //ToDo : Refactor this
            if (mValue == null && (Property.Value == null || Property.Value.GetType().IsClass))
            {
                Property.Value = default;
                return;
            }

            Property.Value = MainInputData�onvertingFunction != null
                           ? MainInputData�onvertingFunction(value)
                           : value is TOut v
                                ? v
                                : throw new InvalidCastException();
        }

        public void SubscribeTo (IOutputSocket<TIn?> socket,
                                Func<TIn?, TOut?> inputData�onvertingFunction,
                                Func<TIn?, TIn?>? inputDataModificationFunction = null)
        {
            RegisterMainPublisher(socket);
            socket.Register(this);
            MainInputDataModificationFunction = inputDataModificationFunction;
            MainInputData�onvertingFunction = inputData�onvertingFunction;
            socket.ReadOnlyProperty.Subscribe(UpdateData).AddTo(_mainDisposable);
        }
    }
}
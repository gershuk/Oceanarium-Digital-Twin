#nullable enable

using System;

using UniRx;

namespace Aqua.SocketSystem
{
    public sealed class СombiningSocket<TIn1, TIn2, TOut> : UniversalSocket<TIn1?, TOut?>, IInputSocket<TIn2?>
    {
        private readonly CompositeDisposable _additionalDisposable = new();
        private IOutputSocket<TIn2?>? _additionalPublisher;
        public Func<TIn2?, TIn2?>? AdditionalInputDataModificationFunction { get; private set; }
        public ReactiveProperty<Func<TIn1?, TIn2?, TOut>?> CombineFunction { get; private set; }

        public СombiningSocket (ReactiveProperty<TOut?>? property = null,
                                Func<TIn1?, TIn2?, TOut>? combineFunction = null) : base(property)
        {
            CombineFunction = new ReactiveProperty<Func<TIn1?, TIn2?, TOut>?>(combineFunction ?? DefaultCombineFunction);
            CombineFunction.Subscribe((f) => UpdateData());
        }

        protected override void Dispose (bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    base.Dispose (disposing);
                    _additionalDisposable.Dispose();
                }

                _disposedValue = true;
            }
        }

        ~СombiningSocket () => Dispose(false);

        private void UpdateData ()
        {
            UpdateData(_mainPublisher != null 
                       ? _mainPublisher.ReadOnlyProperty.Value
                       : default);
        }

        protected override void UpdateData (TIn1? value)
        {
            var mainModifedValue = MainInputDataModificationFunction != null
                                   ? MainInputDataModificationFunction(value)
                                   : value;

            var additionalValue = _additionalPublisher != null 
                                  ? _additionalPublisher.ReadOnlyProperty.Value 
                                  : default;

            var additionalModifedValue = AdditionalInputDataModificationFunction!= null
                             ? AdditionalInputDataModificationFunction(additionalValue)
                             : additionalValue;

            if (CombineFunction.Value == null)
                throw new NullReferenceException(nameof(CombineFunction));

            Property.Value = CombineFunction.Value(mainModifedValue, additionalModifedValue);
        }

        public void SubscribeTo (IOutputSocket<TIn2?> socket, Func<TIn2?, TIn2?>? inputDataModificationFunction = null)
        {
            RegisterAdditionalPublisher(socket);
            socket.Register(this);
            AdditionalInputDataModificationFunction = inputDataModificationFunction;
            socket.ReadOnlyProperty.Subscribe((v)=>UpdateData()).AddTo(_additionalDisposable);
        }

        public void UnsubscribeFrom (IOutputSocket<TIn2?> socket)
        {
            UnregisterAdditionalPublisher(socket);
            socket.Unregister(this);
            ResetAdditionalDataFunction();
            _additionalDisposable.Clear();
        }

        private void RegisterAdditionalPublisher (IOutputSocket<TIn2?> socket)
        {
            if (_additionalPublisher != null)
                throw new Exception($"{nameof(_additionalPublisher)} != null");
            _additionalPublisher = socket;
        }

        private void UnregisterAdditionalPublisher (IOutputSocket<TIn2?> socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket != _additionalPublisher)
                throw new Exception($"{nameof(socket)} != {nameof(_additionalPublisher)}");

            _additionalPublisher = null;
        }

        private TOut? DefaultCombineFunction (TIn1? in1, TIn2? in2) => 
            (in1, in2) is TOut tuple 
            ? tuple 
            : throw new InvalidCastException(); 

        private void ResetAdditionalDataFunction ()
        {
            AdditionalInputDataModificationFunction = null;
        }
    }
}
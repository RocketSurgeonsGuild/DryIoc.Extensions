using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Extensions.DryIoc.Internals
{
    /// <summary>
    /// GenericObservableObservable.
    /// Implements the <see cref="System.IObservable{T}" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IObservable{T}" />
    internal class GenericObservableObservable<T> : IObservable<T>
    {
        private readonly ILogger _logger;
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericObservableObservable{T}" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public GenericObservableObservable(ILogger logger) => _logger = logger;

        /// <summary>
        /// Sends the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Send(T value)
        {
            foreach (var observer in _observers)
            {
                try
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    _logger.LogError(0, e, "Failed to execute observer");
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <param name="observer">The object that is to receive notifications.</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications before the provider has
        /// finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            return new Disposable(() => { _observers.RemoveAll(x => x == observer); });
        }
    }
}
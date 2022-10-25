using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace InGroupe.Innovation.Wpf.Bedrock.Extensions;

public static class DispatcherExtensions
{
    /// <summary>
    /// Switches the current thread or future to the UI context.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    /// <returns><see cref="SwitchToUiAwaitable"/></returns>
    public static SwitchToUiAwaitable SwitchToUi(this Dispatcher dispatcher)
        => new SwitchToUiAwaitable(dispatcher);

    public struct SwitchToUiAwaitable : INotifyCompletion
    {
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchToUiAwaitable"/> struct.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        internal SwitchToUiAwaitable(Dispatcher dispatcher)
            => this.dispatcher = dispatcher;

        public SwitchToUiAwaitable GetAwaiter() => this;

        public void GetResult()
        {
        }

        public bool IsCompleted => this.dispatcher.CheckAccess();

        public void OnCompleted(Action continuation) => this.dispatcher.BeginInvoke(continuation);
    }
}

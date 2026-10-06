using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer.Helpers
{
    /// <summary>
    /// Runs attempts one at a time across the whole server, holding each slot for a fixed interval so the
    /// total attempt rate is capped however many callers arrive at once. Guessing a secret becomes bounded by
    /// the clock rather than by how much hardware the attacker brought.
    /// </summary>
    internal sealed class AttemptThrottle(TimeSpan interval, TimeSpan maxWait)
    {
        private readonly SemaphoreSlim gate = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Waits for the gate, runs <paramref name="attempt"/>, then keeps the gate for the interval.
        /// Entered is false when the queue was still busy after maxWait: callers report that as its own
        /// outcome rather than as a failed attempt, so a flood cannot make a legitimate attempt look wrong.
        /// </summary>
        internal async Task<(bool Entered, T Result)> Run<T>(Func<Task<T>> attempt)
        {
            if (!await gate.WaitAsync(maxWait)) return (false, default);

            try
            {
                return (true, await attempt());
            }
            finally
            {
                await Task.Delay(interval);
                gate.Release();
            }
        }
    }
}

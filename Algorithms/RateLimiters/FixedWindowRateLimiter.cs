using System;
using System.Diagnostics;

namespace Algorithms.RateLimiters
{
    /// <summary>
    /// Fixed window rate limiter.
    /// </summary>
    /// <remarks>
    /// Time complexity: O(1)
    /// Space complexity: O(1)
    ///
    /// This rate limiter allows a fixed number of requests to be made in a fixed time window.
    /// For example, it can be used to limit the number of wrong password attempts to 3 per day.
    /// When the request arrives, the counter for the current time window increments.
    /// If counter reaches the quota, all requests rejected until the next time window starts.
    ///
    /// The disadvantage of this algorithm is that it vulnerable to spikes on the edge of the window
    /// (if the spike started just at the end of the window and continues in the beginning of next one,
    /// this algorithm allows 2X requests within the short period of time).
    /// </remarks>
    public class FixedWindowRateLimiter : IRateLimiter
    {
        private readonly int _requestsQuota;
        private readonly long _windowSize;

        private long _windowStartedAt;

        private int _requestsCounter = 0;

        public FixedWindowRateLimiter(int requestsQuota, TimeSpan windowSize)
        {
            _requestsQuota = requestsQuota;
            _windowSize = windowSize.Ticks;
            _windowStartedAt = Stopwatch.GetTimestamp();
        }

        public bool TryConsume(out TimeSpan timeToWait)
        {
            var now = Stopwatch.GetTimestamp();

            // restart the counter if window has expired
            if (now - _windowStartedAt >= _windowSize)
            {
                _requestsCounter = 0;
                _windowStartedAt = now;
            }

            // deny requests if we have exceeded the quota until the window restarts
            if (_requestsCounter >= _requestsQuota)
            {
                long elapsed = now - _windowStartedAt;
                timeToWait = TimeSpan.FromTicks(_windowSize - elapsed);
                return false;
            }

            // we still have the quota remaining, so consume it
            _requestsCounter++;
            timeToWait = TimeSpan.Zero;
            return true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Algorithms.RateLimiters
{
    /// <summary>
    /// Sliding window log rate limiter.
    /// </summary>
    /// <remarks>
    /// Time complexity: O(1)
    /// Space complexity: O(N) where N is the number of allowed requests per time window (quota).
    ///
    /// This rate limiter allows a maximum _quota requests to be made in any given time window.
    /// This algorithm fixes the spikes problem of the fixed window algorithm and it's very accurate.
    /// The disadvantage is that it requires more memory to store the timestamps of the 
    /// last _quota requests.
    /// </remarks>
    public class SlidingLogRateLimiter : IRateLimiter
    {
        private readonly int _requestsQuota;
        private readonly long _windowSize;

        // the queue of timestamps for the allowed requests
        private Queue<long> _timestamps = new Queue<long>();

        public SlidingLogRateLimiter(int requestsQuota, TimeSpan windowSize)
        {
            _requestsQuota = requestsQuota;
            _windowSize = windowSize.Ticks;
        }

        public bool TryConsume(out TimeSpan timeToWait)
        {
            var now = Stopwatch.GetTimestamp();

            if (_timestamps.Count >= _windowSize)
            {
                var oldest = _timestamps.Peek();
                if (now - oldest >= _windowSize)
                {
                    // the oldest request is outside of the window, so remove it
                    // and allow the new request
                    _timestamps.Dequeue();
                }
                else
                {
                    // the oldest request is still inside the window, so reject the new request
                    timeToWait = TimeSpan.FromTicks(oldest + _windowSize - now);
                    return false;
                }
            }

            // we have a quota for at least one request, so consume it and store the timestamp
            _timestamps.Enqueue(now);
            timeToWait = TimeSpan.Zero;
            return true;
        }
    }
}
using System;
using System.Diagnostics;

namespace Algorithms.RateLimiters
{
    /// <summary>
    /// Tokens bucket rate limiter.
    /// </summary>
    /// <remarks>
    /// Time complexity: O(1)
    /// Space complexity: O(1)
    ///
    /// This rate limiter emulates the bucket full of tokens. The request uses one token 
    /// to be allowed. If there is no available tokens in the bucket, request is rejected.
    /// New tokens are added to the bucket (if it's not full) one by one at the 
    /// specified rate.
    ///
    /// Configuration parameters:
    /// - Bucket capacity - tha maximum number of requests that can be processed in the
    ///   short perid of time. Too high capacity may cause requests bursts, while too
    ///   low value may increase the number of rejected requests.
    /// - Refill rate - time delay between adding two tokens to the bucket. If you want
    ///   to allow 10 requests/sec, the refill rate should be 0.1 second.
    ///
    /// Advantages:
    /// - Space efficient
    /// - Requests spikes can be handled by decreasing the bucket capacity.
    /// Disadvantages:
    /// - Choosing the optimal parameters is a challenge, especially if requests
    ///   load is variable.
    /// </remarks>
    public class TokenBucketRateLimiter : IRateLimiter
    {
        private readonly int _bucketCapacity;
        private readonly long _refillRate;

        private long _lastRefill;

        private int _remainingTokens;

        public TokenBucketRateLimiter(int bucketCapacity, TimeSpan refillRate)
        {
            _bucketCapacity = bucketCapacity;
            _refillRate = refillRate.Ticks;

            // Initially, fill the bucket:
            _remainingTokens = _bucketCapacity;
            _lastRefill = Stopwatch.GetTimestamp();
        }

        public bool TryConsume(out TimeSpan timeToWait)
        {
            Refill();

            // deny requests if we have exceeded the quota until the window restarts
            if (_remainingTokens == 0)
            {
                timeToWait = TimeSpan.FromTicks(_lastRefill + _refillRate);
                return false;
            }

            // we still have tokens, use one
            _remainingTokens--;
            timeToWait = TimeSpan.Zero;
            return true;
        }

        private void Refill()
        {
            if (_remainingTokens == _bucketCapacity)
            {
                // bucket is full
                return;
            }

            var now = Stopwatch.GetTimestamp();
            // number of tokens to add rounded down to the integer value:
            int tokensToAdd = (int) ((now - _lastRefill) / _refillRate);
            _remainingTokens = Math.Min(_bucketCapacity, _remainingTokens + tokensToAdd);

            // store the time when the latest token was added
            _lastRefill += tokensToAdd * _refillRate;
        }
    }
}
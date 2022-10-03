using System;

namespace Algorithms.RateLimiters
{
    /// <summary>
    /// Rate limiter algorithm interface.
    /// </summary>
    /// <remarks>
    /// Disign of the rate limiter is the popular system design interview question.
    /// The system design problem is quite broad and involve topics like scalability, concurrency,
    /// fault tolerance, etc. For example, see:
    /// https://liamchzh.com/tech/2020/11/18/system-design-4/
    /// https://www.linkedin.com/pulse/system-design-rate-limiter-omar-ismail
    ///
    /// This sections covers rate limiter algorithms that might be used as part of the solution
    /// and their advantages and disadvantages.
    /// All algorithms determine if the request should be allowed or rejected in O(1) time, but
    /// they differ in the space complexity and ability to handle the requests spikes.
    ///
    /// Further reading:
    /// https://en.wikipedia.org/wiki/Rate_limiting
    /// https://betterprogramming.pub/4-rate-limit-algorithms-every-developer-should-know-7472cb482f48
    /// https://medium.com/figma-design/an-alternative-approach-to-rate-limiting-f8a06cf7c94c
    /// 
    /// .NET 7 contains several rate limiter implementations out of the box:
    /// https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/ .
    /// </remarks>
    public interface IRateLimiter
    {
        /// <summary>
        /// Determines whether the request can be processed or not.
        /// </summary>
        /// <param name="timeToWait">
        /// If request denied, contains the amount of time before the next attempt.
        /// </param>
        /// <returns>True if request can be processed, false otherwise.</returns>
         bool TryConsume(out TimeSpan timeToWait);
    }
}
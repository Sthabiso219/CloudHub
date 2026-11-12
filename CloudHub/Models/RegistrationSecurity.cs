using System;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace CloudHub.Services
{
    public static class RegistrationSecurity
    {
        private class Counter { public int Count; }
        private static readonly object Sync = new object();

        // Returns true when the key has been used more than "max" times inside the window.
        // Counts live in server memory and reset when the application restarts.
        public static bool IsRateLimited(string key, int max, TimeSpan window)
        {
            lock (Sync)
            {
                Cache cache = HttpRuntime.Cache;
                Counter counter = cache.Get(key) as Counter;

                if (counter == null)
                {
                    counter = new Counter();
                    cache.Insert(key, counter, null,
                        DateTime.UtcNow.Add(window), Cache.NoSlidingExpiration);
                }

                counter.Count++;
                return counter.Count > max;
            }
        }

        // Turns 082 123 4567, 27821234567 and +27 82 123 4567 into +27821234567.
        public static string NormalizePhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            string cleaned = new string(input.Where(c => char.IsDigit(c) || c == '+').ToArray());

            if (cleaned.StartsWith("00")) cleaned = "+" + cleaned.Substring(2);
            if (cleaned.StartsWith("0") && cleaned.Length == 10) return "+27" + cleaned.Substring(1);
            if (cleaned.StartsWith("27") && cleaned.Length == 11) return "+" + cleaned;
            return cleaned;
        }
    }
}
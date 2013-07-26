using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devsurf.Security.Otp.Api
{
    public class Clock
    {
        public long CurrentInterval
        {
            get
            {
                return CurrentSeconds / interval;
            }
        }

        public long CurrentSeconds
        {
            get
            {
                TimeSpan span = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                long currentTimeSeconds = (long)Math.Floor(span.TotalSeconds);
                return currentTimeSeconds;
            }
        }

        private int interval;

        public Clock() {
            interval = 30;
        }

        public Clock(int interval) {
            this.interval = interval;
        }
    }
}

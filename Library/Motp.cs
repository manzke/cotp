using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devsurf.Security.Otp.Api;
using System.Security.Cryptography;


namespace Devsurf.Security.Otp
{
    public class Motp
    {
        private String secret;
        private Clock clock;
        private String pin;
        private static int DELAY_WINDOW = 3;

        public String Now
        {
            /**
             * Retrieves the current OTP
             *
             * @return OTP
             */
            get
            {
                return hash(secret, clock.CurrentSeconds).ToString();
            }
        }

        /**
         * Initialize an OTP instance with the shared secret generated on Registration process
         *
         * @param secret Shared secret
         */
        public Motp(String pin, String secret)
        {
            this.secret = secret;
            this.pin = pin;
            this.clock = new Clock();
        }

        public Boolean Verify(String otp)
        {
            otp = otp.ToUpper();
            long currentSeconds = clock.CurrentSeconds;

            int pastResponse = Math.Max(DELAY_WINDOW, 0) * 10;

            for (int i = pastResponse; i >= 0; i = i - 10)
            {
                String candidate = hash(this.secret, currentSeconds - i);
                if (candidate.Equals(otp))
                {
                    return true;
                }
            }
            return false;
        }

        private String hash(String secret, long epoch)
        {
            String s = (epoch / 10) + secret + pin;
            MD5 digest = MD5.Create();

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(s);
            byte[] hash = digest.ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString(0, 6).ToLower();
        }
    }
}
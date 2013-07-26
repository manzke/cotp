using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devsurf.Security.Otp.Api;

namespace Devsurf.Security.Otp
{
    public class Totp
    {
        private String secret;
        private Clock clock;
        private static int DELAY_WINDOW = 1;

        public String Now
        {
            /**
             * Retrieves the current OTP
             *
             * @return OTP
             */
            get
            {
                return hash(secret, clock.CurrentInterval).ToString();
            }
        }

        /**
         * Initialize an OTP instance with the shared secret generated on Registration process
         *
         * @param secret Shared secret
         */
        public Totp(String secret)
        {
            this.secret = secret;
            clock = new Clock();
        }

        /**
         * Initialize an OTP instance with the shared secret generated on Registration process
         *
         * @param secret Shared secret
         * @param clock  Clock responsible for retrieve the current interval
         */
        public Totp(String secret, Clock clock)
        {
            this.secret = secret;
            this.clock = clock;
        }

        /**
         * Prover - To be used only on the client side
         * Retrieves the encoded URI to generated the QRCode required by Google Authenticator
         *
         * @param name Account name
         * @return Encoded URI
         */
        public String Uri(String name)
        {
            return String.Format("otpauth://totp/{0}?secret={1}", System.Web.HttpUtility.UrlEncode(name, UnicodeEncoding.Default), secret);
        }


        /**
         * Verifier - To be used only on the server side
         * <p/>
         * Taken from Google Authenticator with small modifications from
         * {@see <a href="http://code.google.com/p/google-authenticator/source/browse/src/com/google/android/apps/authenticator/PasscodeGenerator.java?repo=android#212">PasscodeGenerator.java</a>}
         * <p/>
         * Verify a timeout code. The timeout code will be valid for a time
         * determined by the interval period and the number of adjacent intervals
         * checked.
         *
         * @param otp Timeout code
         * @return True if the timeout code is valid
         *         <p/>
         *         Author: sweis@google.com (Steve Weis)
         */
        public Boolean Verify(String otp)
        {
            long code = long.Parse(otp);
            long currentInterval = clock.CurrentInterval;

            int pastResponse = Math.Max(DELAY_WINDOW, 0);

            for (int i = pastResponse; i >= 0; --i)
            {
                int candidate = hash(this.secret, currentInterval - i);
                if (candidate == code)
                {
                    return true;
                }
            }
            return false;
        }

        private int hash(String secret, long interval)
        {
            return bytesToInt(new Hmac(Base32.Decode(secret), interval).digest());
        }

        private int bytesToInt(byte[] hash)
        {
            // put selected bytes into result int
            int offset = hash[hash.Length - 1] & 0xf;

            int binary = ((hash[offset] & 0x7f) << 24) |
                    ((hash[offset + 1] & 0xff) << 16) |
                    ((hash[offset + 2] & 0xff) << 8) |
                    (hash[offset + 3] & 0xff);

            return binary % (int)Digits.SIX;
        }
    }
}
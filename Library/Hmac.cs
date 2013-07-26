using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Devsurf.Security.Otp.Api
{
    public class Hmac
    {
        private sbyte[] secret;
        private long currentInterval;

        public Hmac(sbyte[] secret, long currentInterval) {
            this.secret = secret;
            this.currentInterval = currentInterval;
        }

        public byte[] digest()
        {
            byte[] keyBytes = new byte[secret.Length];
            System.Buffer.BlockCopy(secret, 0, keyBytes, 0, secret.Length);
            System.Security.Cryptography.HMAC mac = new HMACSHA1(keyBytes);
            mac.Initialize();

            byte[] challenge = BitConverter.GetBytes(currentInterval);
            Array.Reverse(challenge); //Java is using Big Endian so we have to convert it
            mac.ComputeHash(challenge);
            byte[] hash = mac.Hash;

            return hash;
        }
    }
}
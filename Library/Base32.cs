using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devsurf.Security.Otp.Api
{
    public class Base32
    {
        private static Base32 INSTANCE = new Base32("ABCDEFGHIJKLMNOPQRSTUVWXYZ234567"); // RFC 4648/3548
        public static Base32 Instance{
            get{
                return INSTANCE;
            }
        }

        public static string SEPARATOR {
            get{
                return "-";
            }
        }

        public static int SECRET_SIZE {
            get{
                return 10;
            }
        }

        public static DateTime JAVA_DATE_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //private static SecureRandom RANDOM = new SecureRandom();

        // 32 alpha-numeric characters.
        private String ALPHABET;
        private char[] DIGITS;
        private int MASK;
        private int SHIFT;
        private Dictionary<char, int> CHAR_MAP;

        protected Base32(String alphabet) {
            this.ALPHABET = alphabet;
            DIGITS = ALPHABET.ToCharArray();
            MASK = DIGITS.Length - 1;
            SHIFT = numberOfTrailingZeros(DIGITS.Length);
            CHAR_MAP = new Dictionary<char, int>();
            for (int i = 0; i < DIGITS.Length; i++)
            {
                CHAR_MAP.Add(DIGITS[i], i);
            }
        }

        private int numberOfTrailingZeros(int num)
        {
            string binValue = Convert.ToString(num, 2);
            if (Convert.ToInt32(binValue) == 0)
                return 1;
            int i = 0;
            while (Convert.ToInt32(binValue) % 10 == 0)
            {
                int tempNum = Convert.ToInt32(binValue);
                tempNum = tempNum / 10;
                binValue = tempNum.ToString();
                i++;
            }
            return i;
        }

        public static byte[] Decode2(String encoded)
        {
            return Instance.decodeInternal2(encoded);
        }

        public static sbyte[] Decode(String encoded) {
            return Instance.decodeInternal(encoded);
        }

        protected byte[] decodeInternal2(String encoded) {
            // Remove whitespace and separators
            encoded = encoded.Trim().Replace(SEPARATOR, "").Replace(" ", "");

            // Remove padding. Note: the padding is used as hint to determine how many
            // bits to decode from the last incomplete chunk (which is commented out
            // below, so this may have been wrong to start with).
            //TODO encoded = encoded.replaceFirst("[=]*$", "");

            // Canonicalize to all upper case
            encoded = encoded.ToUpper(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            if (encoded.Length == 0) {
                return new byte[0];
            }
            int encodedLength = encoded.Length;
            int outLength = encodedLength * SHIFT / 8;
            byte[] result = new byte[outLength];
            int buffer = 0;
            int next = 0;
            int bitsLeft = 0;
            foreach (char c in encoded.ToCharArray()) {
                if (!CHAR_MAP.ContainsKey(c)) {
                    throw new DecodingException("Illegal character: " + c);
                }
                buffer <<= SHIFT;
                buffer |= CHAR_MAP[c] & MASK;
                bitsLeft += SHIFT;
                if (bitsLeft >= 8) {
                    result[next++] = (byte) (buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
            // We'll ignore leftover bits for now.
            //
            // if (next != outLength || bitsLeft >= SHIFT) {
            //  throw new DecodingException("Bits left: " + bitsLeft);
            // }
            return result;
        }

        protected sbyte[] decodeInternal(String encoded) {
            // Remove whitespace and separators
            encoded = encoded.Trim().Replace(SEPARATOR, "").Replace(" ", "");

            // Remove padding. Note: the padding is used as hint to determine how many
            // bits to decode from the last incomplete chunk (which is commented out
            // below, so this may have been wrong to start with).
            //TODO encoded = encoded.replaceFirst("[=]*$", "");

            // Canonicalize to all upper case
            encoded = encoded.ToUpper(System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            if (encoded.Length == 0) {
                return new sbyte[0];
            }
            int encodedLength = encoded.Length;
            int outLength = encodedLength * SHIFT / 8;
            sbyte[] result = new sbyte[outLength];
            int buffer = 0;
            int next = 0;
            int bitsLeft = 0;
            foreach (char c in encoded.ToCharArray()) {
                if (!CHAR_MAP.ContainsKey(c)) {
                    throw new DecodingException("Illegal character: " + c);
                }
                buffer <<= SHIFT;
                buffer |= CHAR_MAP[c] & MASK;
                bitsLeft += SHIFT;
                if (bitsLeft >= 8) {
                    result[next++] = (sbyte) (buffer >> (bitsLeft - 8));
                    bitsLeft -= 8;
                }
            }
            // We'll ignore leftover bits for now.
            //
            // if (next != outLength || bitsLeft >= SHIFT) {
            //  throw new DecodingException("Bits left: " + bitsLeft);
            // }
            return result;
        }


        public static String encode(byte[] data) {
            return Instance.encodeInternal(data);
        }

        protected String encodeInternal(byte[] data) {
            if (data.Length == 0) {
                return "";
            }

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            if (data.Length >= (1 << 28)) {
                // The computation below will fail, so don't do it.
                throw new System.ArgumentException();
            }

            int outputLength = (data.Length * 8 + SHIFT - 1) / SHIFT;
            StringBuilder result = new StringBuilder(outputLength);

            int buffer = data[0];
            int next = 1;
            int bitsLeft = 8;
            while (bitsLeft > 0 || next < data.Length) {
                if (bitsLeft < SHIFT) {
                    if (next < data.Length) {
                        buffer <<= 8;
                        buffer |= (data[next++] & 0xff);
                        bitsLeft += 8;
                    } else {
                        int pad = SHIFT - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = MASK & (buffer >> (bitsLeft - SHIFT));
                bitsLeft -= SHIFT;
                result.Append(DIGITS[index]);
            }
            return result.ToString();
        }

        public class DecodingException : Exception {
            public DecodingException(String message) : base(message) {}
        }

        /*public static String Random() {
            // Allocating the buffer
            byte[] buffer = new byte[SECRET_SIZE];

            // Filling the buffer with random numbers.
            RANDOM.nextBytes(buffer);

            // Getting the key and converting it to Base32
            byte[] secretKey = new byte[SECRET_SIZE];
            Array.Copy(buffer, secretKey, SECRET_SIZE);
            return encode(secretKey);
        }*/
    }
}

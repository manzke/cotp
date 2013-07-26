using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Devsurf.Security.Otp;

namespace Devsurf.Security.Otp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            String pin = args[0];
            String secret = args[1];
            Motp motp = new Motp(pin, secret);
            while (true)
            {
                string Now = motp.Now;
                System.Console.WriteLine("Token " + Now + " verified: " + motp.Verify(Now));

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}

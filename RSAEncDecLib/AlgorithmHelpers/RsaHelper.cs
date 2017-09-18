namespace RSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;
    using EasySharp.NHelpers.Utils.Cryptography;

    public class RsaHelper
    {
        public static (int p, int q) GetRandomPrimesWithinRange(int start, int end)
        {
            int p = 0;
            int q = 0;

            while (p == q)
            {
                p = GetRandomPrimeWithinRange(start, end);
                q = GetRandomPrimeWithinRange(start, end);
            }

            return (p, q);
        }

        public static int GetRandomPrimeWithinRange(int start, int end)
        {
            if (!ExistsPrimeWithinRange(start, end))
                throw new ArgumentException($"There is no prime numbers for the given range: {start}..{end}");


            int value = RNGUtil.Next(start, end);

            int oscillator = value;
            bool flipFlop = true;
            int seedOffset = 1;

            while (IsNotPrime(oscillator) || oscillator.NotInRange(start, end))
            {
                flipFlop = !flipFlop; // "false" for the first time
                oscillator = value + (flipFlop ? -seedOffset : +seedOffset);
                if (flipFlop) seedOffset += 1;
            }

            return oscillator;
        }

        public static bool ExistsPrimeWithinRange(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (IsPrime(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotPrime(int number) => !IsPrime(number);

        public static bool IsPrime(int number)
        {
            if ((number & 1) == 0)
            {
                return number == 2;
            }

            int limit = (int)Math.Sqrt(number);

            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public static class PrimeExtensions
    {
        //// Random generator (thread safe)
        //private static ThreadLocal<Random> s_Gen = new ThreadLocal<Random>(() => new Random());

        //// Random generator (thread safe)
        //private static Random Gen => s_Gen.Value;

        public static bool IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            byte[] bytes = new byte[value.ToByteArray().LongLength];
            BigInteger a;

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < witnesses; i++)
                {
                    do
                    {
                        rng.GetBytes(bytes);

                        a = new BigInteger(bytes);
                    } while (a < 2 || a >= value - 2);

                    BigInteger x = BigInteger.ModPow(a, d, value);
                    if (x == 1 || x == value - 1)
                        continue;

                    for (int r = 1; r < s; r++)
                    {
                        x = BigInteger.ModPow(x, 2, value);

                        if (x == 1)
                            return false;
                        if (x == value - 1)
                            break;
                    }

                    if (x != value - 1)
                        return false;
                }
            }

            return true;
        }
    }
}
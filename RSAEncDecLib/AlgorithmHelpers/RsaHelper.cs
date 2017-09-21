namespace RSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
    using EasySharp.NHelpers.Utils.Cryptography;

    public class RsaHelper
    {
        public static (long p, long q) GetRandomPrimesWithinRange(long start, long end)
        {
            long p = 0;
            long q = 0;

            while (p == q)
            {
                p = GetRandomPrimeWithinRange(start, end);
                q = GetRandomPrimeWithinRange(start, end);
            }

            return (p, q);
        }

        public static long GetRandomPrimeWithinRange(long start, long end)
        {
            if (!ExistsPrimeWithinRange(start, end))
                throw new ArgumentException($"There is no prime numbers for the given range: {start}..{end}");


            long value = RNGUtil.Next(start, end);

            long oscillator = value;
            bool flipFlop = true;
            long seedOffset = 1;

            while (IsNotPrime(oscillator) || !(oscillator >= start && oscillator <= end))
            {
                flipFlop = !flipFlop; // "false" for the first time
                oscillator = value + (flipFlop ? -seedOffset : +seedOffset);
                if (flipFlop) seedOffset += 1;
            }

            return oscillator;
        }

        public static bool ExistsPrimeWithinRange(long start, long end)
        {
            for (long i = start; i < end; i++)
            {
                if (IsPrime(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotPrime(long number) => !IsPrime(number);

        public static bool IsPrime(long number)
        {
            if (number %2 == 0)
            {
                return number == 2;
            }

            long limit = (long) Math.Sqrt(number);

            for (long i = 3; i <= limit; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}

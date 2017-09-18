namespace RSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
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

            int limit = (int) Math.Sqrt(number);

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
}

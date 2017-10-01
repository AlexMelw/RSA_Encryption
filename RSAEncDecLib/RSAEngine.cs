namespace RSAEncDecLib
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using System.Threading.Tasks;
    using AlgorithmHelpers;
    using Interfaces;

    public class RSAEngine : IEncryptor, IDecryptor, IKeygen
    {
        private byte[] _encryptionExp;
        private byte[] _modulus;
        private byte[] _decryptionExp;

        public async Task<(byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent)>
            GenerateKeysAsync(int keySizeBits)
        {
            Stopwatch overallStopwatch = Stopwatch.StartNew();

            await Console.Out.WriteLineAsync("Key generation status: pass 1 [1/3]").ConfigureAwait(false);

            (BigInteger p, BigInteger q) = await ProducePQPrimesAsync(keySizeBits).ConfigureAwait(false);

            BigInteger modulus = ComputeModulus(p, q);
            BigInteger totient = ComputeTotient(p, q);

            await Console.Out.WriteLineAsync("Key generation status: pass 2 [2/3]").ConfigureAwait(false);
            BigInteger encryptionExponent = GenerateEncryptionExponent(totient);

            await Console.Out.WriteLineAsync("Key generation status: pass 3 [3/3]").ConfigureAwait(false);
            var decryptionExponent = GenerateDecryptionExponent(encryptionExponent, totient);

            TimeSpan elapsed = overallStopwatch.Elapsed;
            await Console.Out.WriteLineAsync($"Key generation status: operation completed in " +
                                             $"{elapsed.Minutes} min, " +
                                             $"{elapsed.Seconds} sec, " +
                                             $"{elapsed.Milliseconds} ms.").ConfigureAwait(false);

            return (modulus.ToByteArray(), encryptionExponent.ToByteArray(), decryptionExponent.ToByteArray());
        }

        private static async Task<(BigInteger p, BigInteger q)> ProducePQPrimesAsync(int keySizeBits)
        {
            (BigInteger p, BigInteger q) = default((BigInteger, BigInteger));

            switch (Environment.ProcessorCount)
            {
                case 16:
                    (p, q) = await
                        (await Task.WhenAny(
                                GeneratePQPrimesAsync(keySizeBits, 1),
                                GeneratePQPrimesAsync(keySizeBits, 2),
                                GeneratePQPrimesAsync(keySizeBits, 3),
                                GeneratePQPrimesAsync(keySizeBits, 4),
                                GeneratePQPrimesAsync(keySizeBits, 5),
                                GeneratePQPrimesAsync(keySizeBits, 6),
                                GeneratePQPrimesAsync(keySizeBits, 7),
                                GeneratePQPrimesAsync(keySizeBits, 8))
                            .ConfigureAwait(false))
                        .ConfigureAwait(false);
                    break;

                case 8:
                    (p, q) = await
                        (await Task.WhenAny(
                                GeneratePQPrimesAsync(keySizeBits, 1),
                                GeneratePQPrimesAsync(keySizeBits, 2),
                                GeneratePQPrimesAsync(keySizeBits, 3),
                                GeneratePQPrimesAsync(keySizeBits, 4))
                            .ConfigureAwait(false))
                        .ConfigureAwait(false);
                    break;

                case 4:
                    (p, q) = await
                        (await Task.WhenAny(
                                GeneratePQPrimesAsync(keySizeBits, 1),
                                GeneratePQPrimesAsync(keySizeBits, 2))
                            .ConfigureAwait(false))
                        .ConfigureAwait(false);
                    break;

                case 2:
                case 1:
                default:
                    (p, q) = await GeneratePQPrimesAsync(keySizeBits, 1)
                        .ConfigureAwait(false);
                    break;
            }
            return (p, q);
        }

        private static async Task<(BigInteger p, BigInteger q)> GeneratePQPrimesAsync(int keySizeBits, int taskId)
        {
            (BigInteger, BigInteger) pq = await Task.Run(async () =>
            {
                int keySize = keySizeBits / 2;

                BigInteger[] primes = await Task.WhenAll(
                        Task.Run(() => MaurerAlgorithm.Instance.GetProvablePrimeAsync(keySize)),
                        Task.Run(() => MaurerAlgorithm.Instance.GetProvablePrimeAsync(keySize)))
                    .ConfigureAwait(false);

                return (primes.First(), primes.Last());
            }).ConfigureAwait(false);

            return pq;
        }

        private static BigInteger GenerateEncryptionExponent(BigInteger totient)
        {
            BigInteger e;
            do

            {
                e = BigIntegerHelper.NextBigInteger(2, totient);
            } while (BigInteger.GreatestCommonDivisor(e, totient) != 1);
            return e;
        }

        private static BigInteger GenerateDecryptionExponent(BigInteger encryptionExp, BigInteger totient)
        {
            BigInteger decryptionExponent = Extended_GCD(totient, encryptionExp);
            decryptionExponent = NormalizeY(totient, decryptionExponent);
            return decryptionExponent;
        }

        private static BigInteger NormalizeY(BigInteger totient, BigInteger y) => y < 0 ? y + totient : y;

        private static BigInteger Extended_GCD(BigInteger A, BigInteger B)
        {
            bool reverse = false;
            if (A < B) //if A less than B, switch them
            {
                Swap(ref A, ref B);
                reverse = true;
            }

            BigInteger r = B;
            BigInteger q = 0;
            BigInteger x0 = 1;
            BigInteger y0 = 0;
            BigInteger x1 = 0;
            BigInteger y1 = 1;
            BigInteger x = 0;

            BigInteger y = 0;
            while (A % B != 0)
            {
                r = A % B;
                q = A / B;

                x = x0 - q * x1;
                y = y0 - q * y1;

                x0 = x1;
                y0 = y1;

                x1 = x;
                y1 = y;

                A = B;
                B = r;
            }

            var resultR = r;
            return reverse ? x : y;

            void Swap(ref BigInteger X, ref BigInteger Y)
            {
                var temp = X;
                X = Y;
                Y = temp;
            }
        }

        private static BigInteger ComputeTotient(BigInteger p, BigInteger q)
        {
            return (p - 1) * (q - 1);
        }

        private static BigInteger ComputeModulus(BigInteger p, BigInteger q)
        {
            return BigInteger.Multiply(p, q);
        }

        public byte[] EncryptData(byte[] plainText)
        {
            BigInteger m = new BigInteger(plainText);
            BigInteger e = new BigInteger(_encryptionExp);
            BigInteger n = new BigInteger(_modulus);
            byte[] encryptedData = BigInteger.ModPow(m, e, n).ToByteArray();
            return encryptedData;
        }

        public byte[] DecryptData(byte[] cipherText)
        {
            BigInteger m = new BigInteger(cipherText);
            BigInteger d = new BigInteger(_decryptionExp);
            BigInteger n = new BigInteger(_modulus);
            byte[] decryptedData = BigInteger.ModPow(m, d, n).ToByteArray();
            return decryptedData;
        }

        public void ImportPrivateKey(byte[] decryptionExp, byte[] modulus)
        {
            _decryptionExp = decryptionExp;
            _modulus = modulus;
        }

        public void ImportPrivateKey(BigInteger decryptionExp, BigInteger modulus)
        {
            ImportPrivateKey(decryptionExp.ToByteArray(), modulus.ToByteArray());
        }

        public void ImportPublicKey(byte[] encryptionExp, byte[] modulus)
        {
            _encryptionExp = encryptionExp;
            _modulus = modulus;
        }

        public void ImportPublicKey(BigInteger encryptionExp, BigInteger modulus)
        {
            ImportPublicKey(encryptionExp.ToByteArray(), modulus.ToByteArray());
        }
    }
}
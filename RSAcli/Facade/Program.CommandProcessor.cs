namespace RSAcli.Facade
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using EasySharp.NHelpers.CustomExMethods;
    using Options;
    using RSAEncDecLib;
    using RSAEncDecLib.Interfaces;

    static partial class Program
    {
        private static void ProcessGenerateRSAKeyPairCommand(GenerateRSAKeyPair options)
        {
            Task.Run(async () =>
            {
                IKeygen keygen = CryptoFactory.CreateKeygen();

                (byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent) =
                    await keygen.GenerateKeysAsync(options.KeyBitLength).ContinueOnCapturedContext();

                string timeStamp = CreateTimeStamp();

                string privateKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Private, timeStamp);
                string publicKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Public, timeStamp);

                PersistKeyToFile(publicKeyFileName, encryptionExponent, modulus);
                PersistKeyToFile(privateKeyFileName, decryptionExponent, modulus);
            }).Wait();
        }

        private static void ProcessEncryptCommand(EncryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] encryptionExponent;
            byte[] modulus;

            using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            {
                encryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
                modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            }

            IEncryptor rsaEncryptor = CryptoFactory.CreateEncryptor();
            rsaEncryptor.ImportPublicKey(encryptionExponent, modulus);
            byte[] encryptedData = rsaEncryptor.EncryptData(inputByteArray);

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(encryptedData, 0, encryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static void ProcessDecryptCommand(DecryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] decryptionExponent;
            byte[] modulus;

            using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            {
                decryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
                modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            }

            IDecryptor rsaDecryptor = CryptoFactory.CreateDecryptor();
            rsaDecryptor.ImportPrivateKey(decryptionExponent, modulus);
            byte[] decryptedData = rsaDecryptor.DecryptData(inputByteArray);

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(decryptedData, 0, decryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }
    }
}
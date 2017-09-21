namespace RSAcli
{
    using System;
    using System.Globalization;
    using System.Numerics;
    using System.Text;
    using CommandLine;
    using RSAEncDecLib.AlgorithmHelpers;

    static partial class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<GenerateRSAKeyPair, EncryptVerbOptions, DecryptVerbOptions>(args)
                .WithParsed<EncryptVerbOptions>(ProcessEncryptCommand)
                .WithParsed<DecryptVerbOptions>(ProcessDecryptCommand)
                .WithParsed<GenerateRSAKeyPair>(ProcessGenerateRSAKeyPairCommand);
        }

        private static void JustAnotherMEthod()
        {
            //n = BigInteger.Parse(
            //    "0e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f",
            //    NumberStyles.HexNumber);

            //e = BigInteger.Parse(
            //    "0a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7",
            //    NumberStyles.HexNumber);

            //d = BigInteger.Parse(
            //    "04adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7",
            //    NumberStyles.HexNumber);

            const string original = "Text to encrypt";

            RSAEngine rsaEngine = new RSAEngine();
            rsaEngine.GenerateKyes(1024, out BigInteger n, out BigInteger e, out BigInteger d);

            rsaEngine.ImportPublicKey(e, n);
            rsaEngine.ImportPrivateKey(d, n);

            byte[] encryptedRsaParams =
                rsaEngine.EncryptData(Encoding.UTF8.GetBytes(original));
            Console.Out.WriteLine("encryptedRsaParams = {0}", Encoding.UTF8.GetString(encryptedRsaParams)
                /*new BigInteger(encryptedRsaParams)*/);

            byte[] decryptedRsaParams = rsaEngine.DecryptData(encryptedRsaParams);
            Console.Out.WriteLine("decryptedRsaParams = {0}", Encoding.UTF8.GetString(decryptedRsaParams)
                /*new BigInteger(decryptedRsaParams)*/);

            Console.ReadLine();
        }
    }
}
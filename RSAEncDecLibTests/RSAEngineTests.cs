namespace RSAcli.Tests
{
    using System.Numerics;
    using NUnit.Framework;

    [TestFixture]
    public class RSAEngineTests
    {
        [Test]
        public void EncryptDataTest()
        {
            // Arrange
            RSAEngine rsaEngine = new RSAEngine(4096);
            rsaEngine.ImportPublicKey(5, 14);
            
            // Act               
            byte[] encryptedRsaParams = rsaEngine.EncryptData(new BigInteger(2).ToByteArray());

            // Assert
            Assert.That(new BigInteger(4), Is.EqualTo(new BigInteger(encryptedRsaParams)));
        }

        [Test]
        public void DecryptDataTest()
        {
            // Arrange
            RSAEngine rsaEngine = new RSAEngine(4096);
            rsaEngine.ImportPrivateKey(11, 14);

            // Act               
            byte[] decryptedRsaParams = rsaEngine.DecryptData(new BigInteger(4).ToByteArray());

            // Assert
            Assert.That(new BigInteger(2), Is.EqualTo(new BigInteger(decryptedRsaParams)));
        }
    }
}
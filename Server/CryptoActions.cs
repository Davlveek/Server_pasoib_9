using System;
using System.Text;

using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

namespace Server
{
    class CryptoActions
    {
        static public bool VerifyMsg(byte[] data, out byte[] msg)
        {
            var cms = new SignedCms();
            cms.Decode(data);

            try
            {
                cms.CheckSignature(true);
            }
            catch (CryptographicException)
            {
                msg = null;
                return false;
            }

            msg = cms.ContentInfo.Content;

            return true;
        }

        static public byte[] Decrypt(byte[] data)
        {
            var cms = new EnvelopedCms();
            cms.Decode(data);

            Console.WriteLine($"\tEncryption algorithm: {cms.ContentEncryptionAlgorithm.Oid.FriendlyName}");

            cms.Decrypt(cms.RecipientInfos[0]);
            return cms.ContentInfo.Content;
        }

        static public bool VerifySign(byte[] data)
        {
            var cms = new SignedCms();
            cms.Decode(data);

            try
            {
                cms.CheckSignature(true);
                Console.WriteLine("\tSign status: correct.");

                ParseData(cms);
            }
            catch (CryptographicException)
            {
                Console.WriteLine("\tSign status: incorrect");
                return false;
            }

            return true;
        }

        static public void ParseData(SignedCms cms)
        {
            var info = cms.SignerInfos[0];
            var certificate = info.Certificate;

            Console.WriteLine("\tCertificate");
            Console.WriteLine($"\tSubject: {certificate.Subject}");
            Console.WriteLine($"\tIssuer: {certificate.Issuer}");
            Console.WriteLine($"\tValid from: {certificate.NotBefore}");
            Console.WriteLine($"\tValid on: {certificate.NotAfter}");
            Console.WriteLine($"\tSerial number: {certificate.SerialNumber}");
            Console.WriteLine($"\tSign algorithm: {certificate.PublicKey.Oid.FriendlyName}");

            Console.WriteLine(Environment.NewLine);

            for (int i = 0; i < cms.SignerInfos[0].SignedAttributes.Count; i++)
            {
                if (cms.SignerInfos[0].SignedAttributes[i].Values[0] is Pkcs9SigningTime signingTime)
                {
                    Console.WriteLine($"\tSigning time: {signingTime.SigningTime}");
                }
            }

            var msg = cms.ContentInfo.Content;
            Console.WriteLine($"\tMessage: {new UnicodeEncoding().GetString(msg)}");
        }
    }
}

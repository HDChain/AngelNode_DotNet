using System;
using System.Security.Cryptography;
using System.Text;
using log4net;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace PartnerNode.Models {
    public class CryptoHelper {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(CryptoHelper));

        public static string RsaEncode(string privateKey, string input) {
            IAsymmetricBlockCipher engine = new RsaEngine();

            var buff = Convert.FromBase64String(privateKey);

            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(buff);
            engine.Init(true, priKey);

            byte[] txtBuff = Encoding.UTF8.GetBytes(input);
            try {
                var outBytes = engine.ProcessBlock(txtBuff, 0, txtBuff.Length);

                return Convert.ToBase64String(outBytes);
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return String.Empty;
        }

        public static string RsaDecode(string publicKey, string input) {
            IAsymmetricBlockCipher engine = new RsaEngine();

            var buff = Convert.FromBase64String(publicKey);

            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(buff);
            engine.Init(false, pubKey);

            byte[] txtBuff = Convert.FromBase64String(input);
            try {
                var outBytes = engine.ProcessBlock(txtBuff, 0, txtBuff.Length);

                return Encoding.UTF8.GetString(outBytes);
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return String.Empty;
        }

        public static string StringToMd5Hash(string inputString) {
            byte[] encryptedBytes;
            using (var md5 = new MD5CryptoServiceProvider()) {
                encryptedBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(inputString));
            }

            var sb = new StringBuilder();
            foreach (var t in encryptedBytes)
                sb.AppendFormat("{0:x2}", t);

            return sb.ToString();
        }

        public static string BypeArrayToMd5Hash(byte[] buff) {
            byte[] encryptedBytes;
            using (var md5 = new MD5CryptoServiceProvider()) {
                encryptedBytes = md5.ComputeHash(buff);
            }

            var sb = new StringBuilder();
            foreach (var t in encryptedBytes)
                sb.AppendFormat("{0:x2}", t);

            return sb.ToString();
        }

        public static string CipherKeyGenerator(int strength = 64) {
            var keyGen = new CipherKeyGenerator();
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(), strength));

            var buff = keyGen.GenerateKey();

            return Convert.ToBase64String(buff);
        }

        public static string DesEncode(string key, string input) {
            var engine = new DesEngine();

            try {
                var buff = Convert.FromBase64String(key);
                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                cipher.Init(true, new ParametersWithIV(new DesParameters(buff), buff));

                byte[] txtBuff = Encoding.UTF8.GetBytes(input);

                byte[] rv = new byte[cipher.GetOutputSize(txtBuff.Length)];
                int tam = cipher.ProcessBytes(txtBuff, 0, txtBuff.Length, rv, 0);
                cipher.DoFinal(rv, tam);

                return Convert.ToBase64String(rv);
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return String.Empty;
        }

        public static byte[] DesEncode(string key, byte[] input) {
            var engine = new DesEngine();

            try {
                var buff = Convert.FromBase64String(key);
                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                cipher.Init(true, new ParametersWithIV(new DesParameters(buff), buff));


                byte[] rv = new byte[cipher.GetOutputSize(input.Length)];
                int tam = cipher.ProcessBytes(input, 0, input.Length, rv, 0);
                cipher.DoFinal(rv, tam);

                return rv;
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return new byte[0];
        }

        public static string DesDecode(string key, string input) {
            var engine = new DesEngine();

            try {
                var buff = Convert.FromBase64String(key);
                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                cipher.Init(false, new ParametersWithIV(new DesParameters(buff), buff));

                byte[] txtBuff = Convert.FromBase64String(input);

                var rv = cipher.DoFinal(txtBuff, 0, txtBuff.Length);
                return Encoding.UTF8.GetString(rv);
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return String.Empty;
        }

        public static byte[] DesDecode(string key, byte[] input) {
            var engine = new DesEngine();

            try {
                var buff = Convert.FromBase64String(key);
                BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());
                cipher.Init(false, new ParametersWithIV(new DesParameters(buff), buff));


                var rv = cipher.DoFinal(input, 0, input.Length);

                return rv;
            } catch (Exception ex) {
                Logger.Error(ex);
            }

            return new byte[0];
        }
        
    }
}
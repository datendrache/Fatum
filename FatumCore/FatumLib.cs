//   Fatum -- Metadata Processing Library
//
//   Copyright (C) 2003-2023 Eric Knight
//   This software is distributed under the GNU Public v3 License
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.

//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.

//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace Proliferation.Fatum
{
    public static class FatumLib
    {
        public static string ByteToHex(byte value)
        {
            string tmpstring = "";

            switch ((int)(value) / 16)
            {
                case 0: tmpstring += "0"; break;
                case 1: tmpstring += "1"; break;
                case 2: tmpstring += "2"; break;
                case 3: tmpstring += "3"; break;
                case 4: tmpstring += "4"; break;
                case 5: tmpstring += "5"; break;
                case 6: tmpstring += "6"; break;
                case 7: tmpstring += "7"; break;
                case 8: tmpstring += "8"; break;
                case 9: tmpstring += "9"; break;
                case 10: tmpstring += "A"; break;
                case 11: tmpstring += "B"; break;
                case 12: tmpstring += "C"; break;
                case 13: tmpstring += "D"; break;
                case 14: tmpstring += "E"; break;
                case 15: tmpstring += "F"; break;
                default: break;
            }

            switch ((int)(value) % 16)
            {
                case 0: tmpstring += "0"; break;
                case 1: tmpstring += "1"; break;
                case 2: tmpstring += "2"; break;
                case 3: tmpstring += "3"; break;
                case 4: tmpstring += "4"; break;
                case 5: tmpstring += "5"; break;
                case 6: tmpstring += "6"; break;
                case 7: tmpstring += "7"; break;
                case 8: tmpstring += "8"; break;
                case 9: tmpstring += "9"; break;
                case 10: tmpstring += "A"; break;
                case 11: tmpstring += "B"; break;
                case 12: tmpstring += "C"; break;
                case 13: tmpstring += "D"; break;
                case 14: tmpstring += "E"; break;
                case 15: tmpstring += "F"; break;
                default: break;
            }
            return (tmpstring);
        }

        public static byte[]? HexToBytes(string hex)
        {
            byte[] buffer = new byte[hex.Length / 2];
            int buffercounter = 0;
            int length = hex.Length;
            int i = 0;

            while (i < length)
            {
                string abyte = hex.Substring(i, 2);
                buffer[buffercounter] = HexToByte(abyte);
                buffercounter++;
                i += 2;
            }

            if (buffercounter > 0)
            {
                byte[]  result = new byte[buffercounter];
                for (i = 0; i < buffercounter; i++)
                {
                    result[i] = buffer[i];
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        public static int ConvertDWORDToInt(byte[] dword)
        {
            int result = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    result = dword[i];
                }
                else
                {
                    result += (dword[i] * (256 * i));
                }
            }

            return result;
        }

        public static byte[] ConvertHexstringToDWORD(string dword)
        {
            byte[] result = new byte[4];
            int byteindex = 0;

            for (int i = 0; i < 8; i += 2)
            {
                string tmp = dword.Substring(i, i + 2);
                result[byteindex] = HexToByte(tmp);
                byteindex++;
            }

            return (result);
        }

        public static string ConvertBytesTostring(byte[] convert)
        {
            string result = "";

            int length = convert.Length;

            for (int i = 0; i < length; i++)
            {
                result += ByteToHex(convert[i]);
            }

            return (result);
        }

        public static byte ConvertBinarystringToByte(string dword)
        {
            byte result = 0;
            string convert = dword.Substring(2, dword.Length);

            for (int i = 0; i < 8; i++)
            {
                if (convert[i] == '1')
                {
                    result = (byte)((byte)result + (byte)(2 ^ (7 - i)));
                }
            }
            return (result);
        }


        public static string ConvertBytesToText(byte[] incoming, int offset)
        {
            string tmpstring = "";

            if (incoming != null)
            {
                for (int i = offset; i < incoming.Length; i++)
                {
                    byte tmpByte = incoming[i];
                    int tmpInt = (int)tmpByte;
                    if (tmpInt != 0)
                    {
                        char convertedChar = (char)tmpInt;
                        tmpstring += convertedChar;
                    }
                    else
                    {
                        i = incoming.Length;
                    }
                }
            }
            return (tmpstring);
        }

        public static byte[] ConvertTransferHexToBytes(string[] incoming)
        {
            ArrayList listVector = new ArrayList();
            byte[] unencoded = null;

            int index = 0;
            int totalBytes = 0;
            int incomingLength = incoming.Length;

            while (index < incomingLength)
            {
                string currentLine = incoming[index];
                int currentLineLength = currentLine.Length;

                byte[] bytelist = new byte[currentLineLength / 2];

                for (int i = 0; i < currentLineLength / 2; i++)
                {
                    string tmpHex = currentLine.Substring(i * 2, 2);
                    bytelist[i] = HexToByte(tmpHex);
                }
                listVector.Add(bytelist);
                totalBytes += bytelist.Length;
                index++;
            }

            unencoded = new byte[totalBytes];

            int byteindex = 0;
            int listVectorSize = listVector.Count;
            for (int i = 0; i < listVectorSize; i++)
            {
                byte[] combine = (byte[])listVector[i];
                for (int x = 0; x < combine.Length; x++)
                {
                    unencoded[byteindex] = combine[x];
                    byteindex++;
                }
            }

            listVector.Clear();
            return (unencoded);
        }

        public static string[] ConvertBytesToTransferHex(byte[] incoming)
        {
            ArrayList listVector = new ArrayList();
            string tmp = "";
            int step = 0;
            int index = 0;
            int incomingLength = incoming.Length;

            while (index < incomingLength)
            {
                tmp += ByteToHex(incoming[index]);
                if (step > 40)
                {
                    step = 0;
                    listVector.Add(tmp);
                    tmp = "";
                }
                step++;
                index++;
            }

            if (tmp.Length > 0)
            {
                listVector.Add(tmp);
            }

            int vectorSize = listVector.Count;
            string[] result = new string[vectorSize];
            for (int i = 0; i < vectorSize; i++)
            {
                result[i] = (string)listVector[i];
            }

            listVector.Clear();
            return (result);
        }

        public static Boolean ValidateEmail(string test)
        {
            Boolean CHECK = false;

            if (test == null) return (false);

            int TestLength = test.Length;
            int AtCount = 0;

            for (int i = 0; i < TestLength; i++)
            {
                if (test[i] == '@') AtCount++;
            }

            if ((AtCount) > 0)
            {
                if ((AtCount) < 2)
                {
                    CHECK = true;
                }
                else
                {
                    return (false);
                }
            }

            char[] splitter = new char[1];
            splitter[0] = '@';

            string[] Splitsville = test.Split(splitter);

            if (Splitsville.Length < 2)
            {
                return false;
            }

            string EMAIL = Splitsville[0];
            string ADDRESS = Splitsville[1];

            int EMAILlength = EMAIL.Length;

            if (EMAILlength == 0) return false;

            for (int i = 0; i < EMAILlength; i++)
            {
                char ACHAR = EMAIL[i];
                if (!Char.IsLetterOrDigit(ACHAR) && !(ACHAR == '_' | ACHAR == '-' | ACHAR == '.'))
                {
                    return (false);
                }
            }

            int totalperiods = 0;
            int ADDRESSlength = ADDRESS.Length;

            if (ADDRESSlength == 0) return false;

            for (int i = 0; i < ADDRESSlength; i++)
            {
                char ACHAR = ADDRESS[i];
                if (!Char.IsLetterOrDigit(ACHAR))
                {
                    if (ACHAR == '.')
                    {
                        totalperiods++;
                    }
                    else
                    {
                        return (false);
                    }
                }
            }

            return (CHECK);
        }

        public static string RandomPassword(int characters)
        {
            Random randomizer = new Random();
            string Password = "";

            for (int i = 0; i < characters; i++)
            {
                int value = randomizer.Next();
                int subset = value % 62;
                switch (subset)
                {
                    case 0: Password += 'a'; break;
                    case 1: Password += 'b'; break;
                    case 2: Password += 'c'; break;
                    case 3: Password += 'd'; break;
                    case 4: Password += 'e'; break;
                    case 5: Password += 'f'; break;
                    case 6: Password += 'g'; break;
                    case 7: Password += 'h'; break;
                    case 8: Password += 'i'; break;
                    case 9: Password += 'j'; break;
                    case 10: Password += 'k'; break;
                    case 11: Password += 'l'; break;
                    case 12: Password += 'm'; break;
                    case 13: Password += 'n'; break;
                    case 14: Password += 'o'; break;
                    case 15: Password += 'p'; break;
                    case 16: Password += 'q'; break;
                    case 17: Password += 'r'; break;
                    case 18: Password += 's'; break;
                    case 19: Password += 't'; break;
                    case 20: Password += 'u'; break;
                    case 21: Password += 'v'; break;
                    case 22: Password += 'w'; break;
                    case 23: Password += 'x'; break;
                    case 24: Password += 'y'; break;
                    case 25: Password += 'z'; break;
                    case 26: Password += 'A'; break;
                    case 27: Password += 'B'; break;
                    case 28: Password += 'C'; break;
                    case 29: Password += 'D'; break;
                    case 30: Password += 'E'; break;
                    case 31: Password += 'F'; break;
                    case 32: Password += 'G'; break;
                    case 33: Password += 'H'; break;
                    case 34: Password += 'I'; break;
                    case 35: Password += 'J'; break;
                    case 36: Password += 'K'; break;
                    case 37: Password += 'L'; break;
                    case 38: Password += 'M'; break;
                    case 39: Password += 'N'; break;
                    case 40: Password += 'O'; break;
                    case 41: Password += 'P'; break;
                    case 42: Password += 'Q'; break;
                    case 43: Password += 'R'; break;
                    case 44: Password += 'S'; break;
                    case 45: Password += 'T'; break;
                    case 46: Password += 'U'; break;
                    case 47: Password += 'V'; break;
                    case 48: Password += 'W'; break;
                    case 49: Password += 'X'; break;
                    case 50: Password += 'Y'; break;
                    case 51: Password += 'Z'; break;
                    case 52: Password += '0'; break;
                    case 53: Password += '1'; break;
                    case 54: Password += '2'; break;
                    case 55: Password += '3'; break;
                    case 56: Password += '4'; break;
                    case 57: Password += '5'; break;
                    case 58: Password += '6'; break;
                    case 59: Password += '7'; break;
                    case 60: Password += '8'; break;
                    case 61: Password += '9'; break;
                }
            }

        return (Password);
        }

        public static string SimplifiedDataSize(long datalength)
        {
            string result;

            if (datalength < 1000)  // Bytes
            {
                result = datalength.ToString();
            }
            else
            {
                if (datalength < 1000000)  // Kilobytes
                {
                    float temp = (float) datalength / 1000;
                    result = temp.ToString() + " Kilobytes";
                }
                else
                {
                    if (datalength < 10000000000)  // Megabytes
                    {
                        float temp = (float)datalength / 1000000;
                        result = temp.ToString() + " Megabytes";
                    }
                    else
                    {
                        if (datalength < 100000000000000)  // Gigabytes
                        {
                            float temp = (float)datalength / 1000000000;
                            result = temp.ToString() + " Gigabytes";
                        }
                        else
                        {
                            // Terabytes
                            float temp = (float) datalength / 1000000000000;
                            result = temp.ToString() + " Terabytes";
                        }
                    }
                }
            }

            return result;
        }

        public static string ToSafeString(string incoming)
        {
            string result = "";
            int incominglength = incoming.Length;

            for (int i = 0; i < incominglength; i++)
            {
                char tmp = incoming[i];

                switch (tmp)
                {
                    case ' ': result += "+"; break;
                    case '!': result += "%21"; break;
                    case '%': result += "%25"; break;
                    case '#': result += "%23"; break;
                    case '$': result += "%24"; break;
                    case '^': result += "%5E"; break;
                    case '&': result += "%26"; break;
                    case '(': result += "%28"; break;
                    case ')': result += "%29"; break;
                    case '~': result += "%7E"; break;
                    case '`': result += "%60"; break;
                    case '=': result += "%3D"; break;
                    case '+': result += "%2B"; break;
                    case '\\': result += "%5C"; break;
                    case '|': result += "%7C"; break;
                    case '{': result += "%7B"; break;
                    case '}': result += "%7D"; break;
                    case '[': result += "%5B"; break;
                    case ']': result += "%5D"; break;
                    case ':': result += "%3A"; break;
                    case '\"': result += "%22"; break;
                    case ';': result += "%3B"; break;
                    case '\'': result += "%27"; break;
                    case '<': result += "%3C"; break;
                    case '>': result += "%3E"; break;
                    case '?': result += "%3F"; break;
                    case ',': result += "%2C"; break;
                    case '/': result += "%2F"; break;
                    case '\r': result += "%0D"; break;
                    case '\n': result += "%0A"; break;
                    case '\t': result += "%09"; break;
                    case '@': result += "%40"; break;
                   
                    default: result += tmp; break;
                }
            }
            return (result);
        }

        public static byte HexToByte(string hex)
        {
            int result = 0;

            switch (hex[0])
            {
                case '0': result = 0; break;
                case '1': result = 16; break;
                case '2': result = 32; break;
                case '3': result = 48; break;
                case '4': result = 64; break;
                case '5': result = 80; break;
                case '6': result = 96; break;
                case '7': result = 112; break;
                case '8': result = 128; break;
                case '9': result = 144; break;
                case 'A': result = 160; break;
                case 'B': result = 176; break;
                case 'C': result = 192; break;
                case 'D': result = 208; break;
                case 'E': result = 224; break;
                case 'F': result = 240; break;
                default: break;
            }

            switch (hex[1])
            {
                case '0': result += 0; break;
                case '1': result += 1; break;
                case '2': result += 2; break;
                case '3': result += 3; break;
                case '4': result += 4; break;
                case '5': result += 5; break;
                case '6': result += 6; break;
                case '7': result += 7; break;
                case '8': result += 8; break;
                case '9': result += 9; break;
                case 'A': result += 10; break;
                case 'B': result += 11; break;
                case 'C': result += 12; break;
                case 'D': result += 13; break;
                case 'E': result += 14; break;
                case 'F': result += 15; break;
                default: break;
            }
            return ((byte)(result));
        }

        public static string FromSafeString(string incoming)
        {
            string result = "";
            int incominglength = incoming.Length;

            for (int i = 0; i < incominglength; i++)
            {
                Boolean found = false;

                if (incoming[i] == '%')
                {
                    string value = "";
                    i += 1;
                    if (i < incominglength)
                    {
                        value += incoming[i];
                        i++;
                        if (i < incominglength)
                        {
                            value += incoming[i];
                        }
                    }

                    byte tmpvalue = HexToByte(value);
                    int asciivalue = (byte)tmpvalue;
                    char newCharacter = (char)asciivalue;
                    result += newCharacter;
                    found = true;
                }

                if (!found)
                {
                    if (incoming[i] == '+')
                    {
                        result += ' ';
                    }
                    else
                    {
                        result += incoming[i];
                    }
                }
            }

            return (result.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("\0", string.Empty));
        }

        public static Tree? UriXmlToTree(string URI, string PostData)
        {
            Tree? result = null;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                using (MyWebClient client = new MyWebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "text/xml";
                    string s = client.UploadString(URI, PostData);
                    
                    //string s = client.DownloadString(URI);
                    result = XMLTree.ReadXmlFromString(s);
                }
            }
            catch (Exception xyz)
            {
                System.Console.Out.WriteLine(xyz.Message + "\n" + xyz.StackTrace);
            }
            return result;
        }

        public static Boolean ValidateNoSymbols(string validate)
        {
            Boolean result = true;

            for (int i=0;i<validate.Length;i++)
            {
                if (!Char.IsLetter(validate[i]))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public static string Scramble(string unscrambled, string key)
        {
            string result;

            int unscrambledLength = unscrambled.Length;

            byte[] unscram;
            unscram = Encoding.ASCII.GetBytes(unscrambled);

            for (int i = 0; i < unscrambledLength; i++)
            {
                unscram[i] = (byte)(unscram[i] + (byte) (((13+i) * i+3) % 256));
            }

            result = ConvertBytesTostring(unscram);
            result = Encrypt(result, key);
            return result;
        }

        public static string? Unscramble(string scrambled, string key)
        {
            byte[]? scram = HexToBytes(Decrypt(scrambled, key));

            if (scram != null && scram.Length > 0)
            {
                for (int i = 0; i < scram.Length; i++)
                {
                    scram[i] = (byte)(scram[i] - (byte)(((13 + i) * i + 3) % 256));
                }
                return Encoding.ASCII.GetString(scram);
            }
             return null;
        }

        public static string GenerateRandomPassword(int length)
        {
            string password = "";
            string pwchars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^*()1234567890-_=+{};:<>,.?/~`";

            Random rand = new Random();
            for (int i=0;i<length;i++)
            {
                int nextup = rand.Next() % pwchars.Length;
                password += pwchars[nextup];
            }

            return password;
        }

        public static string GenerateRandomName()
        {
            string name = "";
            string namechars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

            Random rand = new Random();
            for (int i = 0; i < 50; i++)
            {
                int nextup = rand.Next() % namechars.Length;
                name += namechars[nextup];
            }
            return name;
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public static string longToShortPresentableText(long value)
        {
            string[] sizes = { "B", "Kb", "Mb", "Gb", "Tb" };
            long len = value;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }

    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 15 * 60 * 1000;
            return w;
        }
    }
}

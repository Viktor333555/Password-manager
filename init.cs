using System.Data.SqlTypes;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace ITprojektS
{

    public class init
    {

        public static byte[] CreateVaultKey(string masterpassword, byte[] secretKey) 
            int iterations = 10000;
            using (Rfc2898DeriveBytes VaultKeyString = new Rfc2898DeriveBytes(masterpassword, secretKey, iterations))
            {
                return VaultKeyString.GetBytes(16);
            }
        }
        static byte[] kryptering(string enstring, ICryptoTransform encryptor)
        {
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(enstring);
                    }
                    byte[] encrypted = msEncrypt.ToArray();
                    return encrypted;
                }
            }
        }
        static string dekryptering(byte[] ciphertext, ICryptoTransform decryptor)
        {
            using (MemoryStream msDecrypt = new MemoryStream(ciphertext))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        string plaintext = srDecrypt.ReadToEnd();
                        return plaintext;
                    }
                }
            }
        }

        static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int length = 20;
            Random random = new Random();
            StringBuilder RandomPassword = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                RandomPassword.Append(chars[random.Next(chars.Length)]);
            }

            return RandomPassword.ToString();
        }
        public static void Init(string Clientpath, string ServerPath)
        {
            string MasterPassword = Console.ReadLine();
            Aes AES = Aes.Create();
            AES.GenerateIV();
            AES.GenerateKey();
            byte[] IV = AES.IV;
            byte[] secretKey = AES.Key;

            System.Console.WriteLine(JsonSerializer.Serialize(secretKey));
            byte[] Vaultkey = CreateVaultKey(MasterPassword, secretKey);
            Vault vault = new Vault(Vaultkey);
            string seraliserat_Vault = JsonSerializer.Serialize(vault);
            ICryptoTransform Encryption_object = AES.CreateEncryptor(Vaultkey, AES.IV);
            Byte[] Encrypted_vault = kryptering(seraliserat_Vault, Encryption_object);


            Serverfile Serverfile_info = new Serverfile(IV, Encrypted_vault);
            Serverfile.Save_To_file(Serverfile_info, ServerPath);
            Clientfile Clientfile_info = new Clientfile(secretKey);
            Clientfile.Save_To_File(Clientfile_info, Clientpath);
        }
        public static void Create(string Clientpath, string ServerPath)
        {
            string masterpass = Console.ReadLine();
            string secretkey = Console.ReadLine();

            if (Verify_Secret_Paswrd(ServerPath, masterpass, secretkey) == true)
            {
                Clientfile clientfile2 = new Clientfile(JsonSerializer.Deserialize<byte[]>(secretkey));
                Clientfile.Save_To_File(clientfile2, Clientpath);
            }
            else
            {
                System.Console.WriteLine("failed verified");
            }
        }
        static bool Verify_Secret_Paswrd(string ServerPath, string masterpass, string secretkey)
        {

            try
            {
                Aes AES = Aes.Create();
                string serialized_Serverfile = File.ReadAllText(ServerPath);
                Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
                AES.IV = serverfile.IV;
                byte[] Vaultkey = CreateVaultKey(masterpass, JsonSerializer.Deserialize<byte[]>(secretkey));
                ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
                string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public static void Get(string Clientpath, string Serverpath)
        {

            System.Console.WriteLine("Please enter your password:");
            string masterpassword = Console.ReadLine();
            string serialized_clientfile = File.ReadAllText(Clientpath);
            string serialized_Serverfile = File.ReadAllText(Serverpath);
            Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
            Clientfile clientfile = JsonSerializer.Deserialize<Clientfile>(serialized_clientfile);
            Aes AES = Aes.Create();
            byte[] Vaultkey = CreateVaultKey(masterpassword, clientfile.secretkey);
            ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
            string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);

            Vault vault = JsonSerializer.Deserialize<Vault>(serialized_Vault);
            int i = 1;
            foreach (var kvp in vault.Login_info)
            {
                var key = kvp.Key;
                System.Console.WriteLine(key);
                i++;
            }
            i = 1;
            if (vault.Login_info.Count == 0)
            {
                System.Console.WriteLine("Dictionary is empty.");
            }

        }
        public static void Get(string Clientpath, string ServerPath, string property)
        {
            string MasterPassword = Console.ReadLine();
            string serialized_clientfile = File.ReadAllText(Clientpath);
            string serialized_Serverfile = File.ReadAllText(ServerPath);
            Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
            Clientfile clientfile = JsonSerializer.Deserialize<Clientfile>(serialized_clientfile);
            Aes AES = Aes.Create();
            byte[] Vaultkey = CreateVaultKey(MasterPassword, clientfile.secretkey);
            ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
            string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);

            Vault vault = JsonSerializer.Deserialize<Vault>(serialized_Vault);
            int i = 1;
            foreach (var kvp in vault.Login_info)
            {
                var key = kvp.Key;
                if (property == key)
                {
                    var vaulu = kvp.Value;
                    System.Console.WriteLine(vaulu);
                }
                i++;
            }
            i = 1;
            if (vault.Login_info.Count == 0)
            {
                System.Console.WriteLine("Dictionary is empty.");
            }

        }
        public static void Set(string Clientpath, string ServerPath, string property)
        {

            string MasterPassword = Console.ReadLine();
            string serialized_clientfile = File.ReadAllText(Clientpath);
            string serialized_Serverfile = File.ReadAllText(ServerPath);
            Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
            Clientfile clientfile = JsonSerializer.Deserialize<Clientfile>(serialized_clientfile);
            Aes AES = Aes.Create();
            byte[] Vaultkey = CreateVaultKey(MasterPassword, clientfile.secretkey);
            ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
            string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);
            Vault vault = JsonSerializer.Deserialize<Vault>(serialized_Vault);
            bool test = false;
            System.Console.WriteLine("Your pasword?");
            string val = Console.ReadLine();
            foreach (var kvp in vault.Login_info)
            {
                if (kvp.Key == property)
                {
                    vault.Login_info[kvp.Key] = val;
                    test = true;
                }
            }
            if (test == false)
            {
                vault.Login_info.Add(property, val);
            }
            string seraliserat_Vault = JsonSerializer.Serialize(vault);
            ICryptoTransform Encryption_object = AES.CreateEncryptor(Vaultkey, serverfile.IV);
            Byte[] Encrypted_vault = kryptering(seraliserat_Vault, Encryption_object);
            Serverfile Serverfile_info = new Serverfile(serverfile.IV, Encrypted_vault);
            Serverfile.Save_To_file(Serverfile_info, ServerPath);


        }

        public static void Set(string Clientpath, string ServerPath, string property, string val)
        {

            string MasterPassword = Console.ReadLine();
            string serialized_clientfile = File.ReadAllText(Clientpath);
            string serialized_Serverfile = File.ReadAllText(ServerPath);
            Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
            Clientfile clientfile = JsonSerializer.Deserialize<Clientfile>(serialized_clientfile);
            Aes AES = Aes.Create();
            byte[] Vaultkey = CreateVaultKey(MasterPassword, clientfile.secretkey);
            ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
            string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);
            Vault vault = JsonSerializer.Deserialize<Vault>(serialized_Vault);
            string newpass;
            if (val == "-g" || val == "--generate")
            {
                newpass = GenerateRandomPassword();
                System.Console.WriteLine(newpass);
            }
            else
            {
                newpass = val;
            }
            bool test = false;
            foreach (var kvp in vault.Login_info)
            {
                if (kvp.Key == property)
                {
                    vault.Login_info[kvp.Key] = newpass;
                    test = true;
                }
            }
            if (test == false)
            {
                vault.Login_info.Add(property, newpass);
            }
            string seraliserat_Vault = JsonSerializer.Serialize(vault);
            ICryptoTransform Encryption_object = AES.CreateEncryptor(Vaultkey, serverfile.IV);
            Byte[] Encrypted_vault = kryptering(seraliserat_Vault, Encryption_object);
            Serverfile Serverfile_info = new Serverfile(serverfile.IV, Encrypted_vault);
            Serverfile.Save_To_file(Serverfile_info, ServerPath);


        }

        public static void Delete(string Clientpath, string ServerPath, string property)
        {

            System.Console.WriteLine("Please enter master password");
            string masterpassword = Console.ReadLine();

            string serialized_clientfile = File.ReadAllText(Clientpath);
            string serialized_Serverfile = File.ReadAllText(ServerPath);
            Serverfile serverfile = JsonSerializer.Deserialize<Serverfile>(serialized_Serverfile);
            Clientfile clientfile = JsonSerializer.Deserialize<Clientfile>(serialized_clientfile);
            Aes AES = Aes.Create();
            byte[] Vaultkey = CreateVaultKey(masterpassword, clientfile.secretkey);
            ICryptoTransform decryptor = AES.CreateDecryptor(Vaultkey, serverfile.IV);
            string serialized_Vault = dekryptering(serverfile.Encrypted_vault, decryptor);
            Vault vault = JsonSerializer.Deserialize<Vault>(serialized_Vault);
            bool test = false;
            foreach (var kvp in vault.Login_info)
            {
                if (kvp.Key == property)
                {
                    vault.Login_info.Remove(property);
                    test = true;
                }
            }
            if (test == false)
            {
                System.Console.WriteLine("There are no properties with that name");
            }
            else
            {
                string seraliserat_Vault = JsonSerializer.Serialize(vault);
                ICryptoTransform Encryption_object = AES.CreateEncryptor(Vaultkey, serverfile.IV);
                Byte[] Encrypted_vault = kryptering(seraliserat_Vault, Encryption_object);
                Serverfile Serverfile_info = new Serverfile(serverfile.IV, Encrypted_vault);
                Serverfile.Save_To_file(Serverfile_info, ServerPath);
            }

        }

        public static void Secret(string clientpath)
        {
            if (File.Exists(clientpath))
            {
                string serilized_clientfile = File.ReadAllText(clientpath);
                Clientfile cfile = JsonSerializer.Deserialize<Clientfile>(serilized_clientfile);
                Console.WriteLine(JsonSerializer.Serialize(cfile.secretkey));
            }

        }
    }


}







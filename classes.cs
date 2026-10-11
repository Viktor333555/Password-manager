using System.ComponentModel.DataAnnotations;
using System.Configuration.Assemblies;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace ITprojektS
{


    class Vault
    {
        public Dictionary<string, string> Login_info { get; set; }
        public byte[] Vaultkey { get; set; }
        public Vault(byte[] vaultkey)
        {
            this.Vaultkey = vaultkey;
            Login_info = new Dictionary<string, string>();
        }
    }


    //Klassen repsenterar det som sparas i serverfile
    class Serverfile
    {
        public byte[] IV { get; set; }
        public byte[] Encrypted_vault { get; set; }
        public Serverfile(byte[] IVn, byte[] Encrypted_vaultn)
        {
            IV = IVn;
            Encrypted_vault = Encrypted_vaultn;
        }

        public Serverfile() { }
        public static void Save_To_file(Serverfile objekt, string serverpath)
        {
            File.WriteAllText(serverpath, JsonSerializer.Serialize(objekt));
        }

    }

    class Clientfile
    {
        public byte[] secretkey { get; set; }
        public Clientfile(byte[] skey)
        {
            secretkey = skey;
        }
        public Clientfile()
        {

        }
        public static void Save_To_File(Clientfile objekt, string clientpath)
        {
            File.WriteAllText(clientpath, JsonSerializer.Serialize(objekt));
        }

    }



}
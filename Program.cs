using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System;

namespace ITprojektS
{

    public class Program
    {
        static public void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command provided. Please provide a command (init, create, delete).");
                return;
            }
            string command = args[0].ToLower();

            switch (command)
            {
                case "init":
                    init.Init(args[1], args[2]);
                    break;
                case "create":
                    init.Create(args[1], args[2]);
                    break;
                case "get":

                    if (args.Length == 4)
                    {
                        init.Get(args[1], args[2], args[3]);
                    }
                    else
                    {
                        init.Get(args[1], args[2]);
                    }

                    break;
                case "set":
                    if (args.Length == 5)
                    {
                        init.Set(args[1], args[2], args[3], args[4]);
                    }
                    else
                    {
                        init.Set(args[1], args[2], args[3]);
                    }
                    break;
                case "delete":
                    init.Delete(args[1], args[2], args[3]);
                    break;
                case "secret":
                    init.Secret(args[1]);
                    break;
                default:
                    Console.WriteLine("Invalid command. Please provide a valid command (init, create, delete).");
                    break;
            }

        }

    }

}


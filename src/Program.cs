namespace bakepass
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.Extensions.Configuration;


    public sealed class AppSettings
    {
        public AppSettings() { PasswordLength = 16; PasswordKey = "c0c33593cb9d4e9f920e59feb09039d6"; Silent = false; }
        public int PasswordLength { get; set; }
        public string PasswordKey { get; set; }
        public bool Silent { get; set; }
    }

    internal class Program
    {
        public static string PasswordCharacters = "abcdefghijkmnopqrstuvwxyz23456789ABCDEFGHJKLMNPQRSTUVWXYZ";

        static void Main(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("bakepass.json").Build();
            AppSettings settings = config.GetRequiredSection("Settings").Get<AppSettings>() ?? new AppSettings();
            if (!settings.Silent)
            {
                Console.WriteLine("BakePass: Generate deterministic hash passwords");
                Console.WriteLine("Copyright (c) 2024 - )|( Sanctuary Software Studio, Inc. - All rights reserved.");
            }

            // make a string out of all of the parts
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(arg);
            }
            sb.Append(settings.PasswordKey);

            // calculate a hash of the concatenated parts
            MD5 md5Hash = MD5.Create();
            byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            // generate the password from the hash
            StringBuilder pwsb = new StringBuilder();
            for (int i = 0; i < settings.PasswordLength; i++)
            {
                byte by = hash[i % hash.Length];
                pwsb.Append(PasswordCharacters[by % PasswordCharacters.Length]);
            }

            // output the password
            Console.WriteLine(pwsb.ToString());
        }
    }
}
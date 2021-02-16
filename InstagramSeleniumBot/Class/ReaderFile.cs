using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;


namespace InstagramSeleniumBot
{
    static class ReaderFile
    {
        public static Account LoadAccountFromFile()
        {
            string json = null;
            string path = Directory.GetCurrentDirectory() + @"\accounts.json";
            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadLine();
            }

            Account acc = new Account(); ;
            if (json != null)
                acc = JsonSerializer.Deserialize<Account>(json);

            return acc;
        }


        public static String LoadCstringFromFile()
        {
            string s = null;
            string path = Directory.GetCurrentDirectory() + @"\ConnectionString.ini";
            using (StreamReader sr = new StreamReader(path))
            {
                s = sr.ReadLine();
            }

            return s;
        }
    }


    struct Account
    {
        public string Login { get; set; }
        public string Pass { get; set; }
    }
}

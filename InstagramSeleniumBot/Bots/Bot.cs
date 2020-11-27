using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using SeleniumLib;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace InstagramSeleniumBot
{
    internal abstract class Bot
    {
        private protected Writer Cons;
        private protected DBmanager db;
        private protected ChromeBrowser Chrome { get; set; }
        private protected static Random rand;
        internal string Name { get; set; }
        static int profileCounter=0;

        public Bot(Writer console,ref Action EndWorkEvent,string botProfileName)
        {
            Cons = console;
            db = new DBmanager(console);
            rand = new Random();
            string chromeProfileName = (++profileCounter).ToString() + botProfileName;
            Cons.WriteLine($"Загрузка бота.");
            Chrome = new ChromeBrowser(chromeProfileName);
            Chrome.SetWindowSize(500, 1000);
            EndWorkEvent += () => Close();
        }


        internal void Autorize(string login, string password)
        {
            db.Name = login;
            int delay = rand.Next(100);
            Thread.Sleep(delay);
            CheckConnectInternet();
            Thread.Sleep(delay);
            Chrome.OpenUrl(@"https://www.instagram.com/?hl=ru");
         
            if (Chrome.FindWebElement(By.XPath("//a[@href ='/direct/inbox/']"))!=null)
            {
                Cons.WriteLine($"{Name}: Пользователь авторизован.");
                return;
            }
            
            Cons.WriteLine($"{Name}: Авторизация пользователя");
            Chrome.SendKeysXPath(@"//*[@id='loginForm']/div/div[1]/div/label/input", login);
            Chrome.SendKeysXPath(@"//*[@id='loginForm']/div/div[2]/div/label/input", password);
            Thread.Sleep(300);
            Chrome.ClickButtonXPath(@"//*[@id='loginForm']/div/div[3]/button");

            CheckAutorize();
           
        }

        private void CheckAutorize()
        {
            Cons.WriteLine($"{Name}: Проверка авторизации.");
            if (Chrome.FindWebElement(By.XPath("//a[@href ='/direct/inbox/']")) == null)
            {
                Cons.WriteLine($"{Name}: Ошибка авторизации.");
                Close();
            }
            Cons.WriteLine($"{Name}: Авторизовано.");
        }

        internal void Close()
        {
            Cons.WriteLine($"{Name}: Close.");
            Task.Run(() => Chrome.Quit());
         
        }

        internal int ParsToInt(string text)
        {
            if (!Int32.TryParse(text, out _))
            {
                text = text.Replace(" ", string.Empty);
                text = text.Replace("тыс.", "000");
                if (text.IndexOf(',') > 0)
                {
                    text = text.Replace(",", string.Empty);
                    text = text.Substring(0, text.Length - 1);
                }
            }

            if (!Int32.TryParse(text, out int num))
                Cons.WriteLine($"Ошибка парсинга строки в число - {text}");

            return num;
        }

        internal bool CheckConnectInternet()
        {
            var ping = new Ping();
            PingReply reply = ping.Send("instagram.com");

            if (Convert.ToString(reply.Status) != "Success")
            {
                reply = ping.Send("instagram.com");
                Thread.Sleep(2000);
                if (Convert.ToString(reply.Status) != "Success")
                {
                    Cons.WriteLine($"{Name}: Ошибка соеденения с сайтом");
                    Close();
                    return false;
                }
            }


            return true;
        }

        internal abstract void Start(int limit);
    }



   



}

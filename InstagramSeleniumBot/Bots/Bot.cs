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
    public abstract class Bot
    {
        private protected Writer Cons;
        private protected DBmanager db;
        private protected ChromeBrowser Chrome { get; set; }
        private protected static Random rand;
        public bool stop;
        private protected CancellationToken token;

        public Bot(Writer console, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            this.token = token;
            Cons = console;

            Cons.WriteLine($"Загрузка бота.");
            db = new DBmanager(console);
            rand = new Random();
            Chrome = new ChromeBrowser("InstagramBotProfile");
            Chrome.SetWindowSize(500, 1000);
            this.Cons.WriteLine($"Бот загружен.");
        }


        public void Autorize(string login, string password)
        {
            if (token.IsCancellationRequested)
                return;

            db.Name = login;
            int delay = rand.Next(100);
            Thread.Sleep(delay);
            CheckConnectInternet();
            Thread.Sleep(delay);
            Chrome.OpenUrl(@"https://www.instagram.com/?hl=ru");
         
            if (Chrome.FindWebElement(By.XPath("//a[@href ='/direct/inbox/']"))!=null)
            {
                Cons.WriteLine($"Пользователь авторизован.");
                return;
            }

            Cons.WriteLine($"Авторизация пользователя");
            Chrome.SendKeysXPath(@"//*[@id='loginForm']/div/div[1]/div/label/input", login);
            Chrome.SendKeysXPath(@"//*[@id='loginForm']/div/div[2]/div/label/input", password);
            Thread.Sleep(300);
            Chrome.ClickButtonXPath(@"//*[@id='loginForm']/div/div[3]/button");

            CheckAutorize();
        }

        private void CheckAutorize()
        {
            if (token.IsCancellationRequested)
                return;

            Cons.WriteLine($"Проверка авторизации.");
            if (Chrome.FindWebElement(By.XPath("//a[@href ='/direct/inbox/']")) == null)
            {
                Cons.WriteLine($"Ошибка авторизации.");
                Close();
            }
            Cons.WriteLine($"Авторизовано.");
        }

        public void Close()
        {
            if(Chrome != null)
                Chrome.Quit();
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

        public bool CheckConnectInternet()
        {
            if (token.IsCancellationRequested)
                return true;

            var ping = new Ping();
            PingReply reply = ping.Send("instagram.com");

            if (Convert.ToString(reply.Status) != "Success")
            {
                reply = ping.Send("instagram.com");
                Thread.Sleep(2000);
                if (Convert.ToString(reply.Status) != "Success")
                {
                    Cons.WriteLine($"Ошибка соеденения с сайтом");
                    Close();
                    return false;
                }
            }

            return true;
        }

        public abstract void Start(int limit);
    }



   



}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Text.Json;

namespace InstagramSeleniumBot
{
    public partial class InForm : System.Windows.Forms.Form
    {
        private readonly Writer Cons;
        List<Process> ProcessChromeDriver;
        CancellationTokenSource cTokenSource;
        CancellationToken token;
        public InForm()
        {
            InitializeComponent();
            ProcessChromeDriver = new List<Process>();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1300, 100);
            Cons = new Writer(Directory.GetCurrentDirectory(), this, textConsole);

            Cons.WriteLine("Program loaded.");
            Task.Run(() => BotWork());
            cTokenSource = new CancellationTokenSource();

        }

        private void BotWork()
        {
            token = cTokenSource.Token;
            Task.Run(() => StopAppTimer(TimeSpan.FromMinutes(50)));

            Account acc = LoadFromFile();
            Bot bot = new Bot(Cons, token);
            bot.Autorize(acc.Login, acc.Pass);
            bot.CollectingAccounts(150);
            bot.ProcessingAccounts(120);
            bot.SubscribeAccounts(13);
            bot.UnSubscribeAccounts(16);
            bot.Close();
            
            CloseProgram();
        }

        private void StopAppTimer(TimeSpan time)
        {
            List<Process> processChromeDriverOld = Process.GetProcessesByName("chromedriver").ToList();
            Thread.Sleep(TimeSpan.FromSeconds(15));
            List<Process> ProcessChromeDriverNew = Process.GetProcessesByName("chromedriver").ToList();

            foreach (Process proces in ProcessChromeDriverNew)
                if (processChromeDriverOld.Find(item => item.Id == proces.Id) == null)
                    ProcessChromeDriver.Add(proces);

            Thread.Sleep(time);
            Cons.WriteLine("Timer went off. Application closed.");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            CloseProgram();
        }

        private void CloseProgram()
        {
            Thread.Sleep(TimeSpan.FromSeconds(4));
            cTokenSource.Cancel();
            Cons.WriteLine("All bot stoped. Application closed.");
            foreach (Process proces in ProcessChromeDriver)
            {
                proces.CloseMainWindow();
            }
            Application.Exit();
        }

        private void buttonStop_Click(object sender, EventArgs e) => cTokenSource.Cancel();



        Account LoadFromFile()
        {
            string json = null;
            string path = Directory.GetCurrentDirectory() + @"\accounts.txt";
           
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    json = sr.ReadLine();
                }
            }
            catch (Exception e)
            {
                Cons.WriteLine(e.Message);
            }
            if (json != null)
            {
                Account acc = JsonSerializer.Deserialize<Account>(json);
                return acc;
            }

            return null;
        }
    }
    internal class Account
    {
        public string Login { get; set; }
        public string Pass { get; set; }
    }
}

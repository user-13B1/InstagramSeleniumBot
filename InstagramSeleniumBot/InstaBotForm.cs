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

namespace InstagramSeleniumBot
{
    public partial class Form : System.Windows.Forms.Form
    {
        private readonly Writer Cons;
        List<Process> ProcessChromeDriver;
        CancellationTokenSource cts;
        CancellationToken token;
        public Form()
        {
            InitializeComponent();
            ProcessChromeDriver = new List<Process>();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1300, 100);
            Cons = new Writer(Directory.GetCurrentDirectory(), this, textConsole);
           
            Cons.WriteLine("Program loaded.");
            Task.Run(() => BotWork());
            cts = new CancellationTokenSource();
        }

        private void BotWork()
        {
            token = cts.Token;
            Task.Run(() => Timer(TimeSpan.FromMinutes(50))); //ограничение времени работы 50минут
            Bot bot = new Bot(Cons, token);
            bot.Autorize("gurillam", "VhZTtmBKZpTqG");
           
            bot.ProcessingAccounts(120);
            bot.SubscribeAccounts(13);
            bot.UnSubscribeAccounts(16);
            bot.CollectingAccounts(120);
            bot.Close();
            
            Thread.Sleep(TimeSpan.FromSeconds(5));
            CloseProgram();
        }

        private void Timer(TimeSpan time)
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
            cts.Cancel();
            Cons.WriteLine("All bot stoped. Application closed.");
            foreach (Process proces in ProcessChromeDriver)
            {
                proces.CloseMainWindow();
            }
            Application.Exit();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            cts.Cancel();
        }
    }
}

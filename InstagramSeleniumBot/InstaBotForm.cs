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
    public partial class FormBot : Form
    {
        private readonly Writer console;
        private readonly FileReaderWriter fileReader;
        private readonly List<Bot> bots;
        public event Action EndWorkEvent = delegate { };
        List<Process> ProcessChromeDriver;

        public FormBot()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1300, 100);
            
            fileReader = new FileReaderWriter();
            Task.Run(() => Timer(TimeSpan.FromMinutes(50))); //ограничение времени работы 50минут
            console = new Writer(this, fileReader);
            bots = new List<Bot>();

           
            //Task.Run(() => Launch(new Info(console, ref EndWorkEvent, "InstaBot"), 50));
            Task.Run(() => Launch(new CollectingAccounts(console, ref EndWorkEvent,"InstaBot"), 50));
            Task.Run(() => Launch(new ProcessingAccounts(console, ref EndWorkEvent, "InstaBot"),50));
            Task.Run(() => Launch(new SubscribeAccounts(console,  ref EndWorkEvent, "InstaBot"),  5));
            Task.Run(() => Launch(new UnSubscribeAccounts(console, ref EndWorkEvent, "InstaBot"), 10));
           
        }


        private void Launch(Bot bot, int limit)
        {
            bots.Add(bot);
            bot.Autorize("gurillam", "70nCHwq");
            bot.Start(limit);
            Close(bot);
        }

        private void Close(Bot bot)
        {
            bot.Close();
            bots.Remove(bot);
            if (bots.Count == 0)
            {
                console.WriteLine("All bot stoped. Application closed.");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                CloseProgram();
            }
        }

        private void Timer(TimeSpan time)
        {
            ProcessChromeDriver = new List<Process>();
            List<Process> processChromeDriverOld = Process.GetProcessesByName("chromedriver").ToList();
            Thread.Sleep(TimeSpan.FromSeconds(15));
            List<Process> ProcessChromeDriverNew = Process.GetProcessesByName("chromedriver").ToList();

            foreach (Process proces in ProcessChromeDriverNew)
                if (processChromeDriverOld.Find(item => item.Id == proces.Id) == null)
                    ProcessChromeDriver.Add(proces);


            Thread.Sleep(time);
            console.WriteLine("Timer went off. Application closed.");

            Task.Run(() => EndWorkEvent());
            Thread.Sleep(TimeSpan.FromSeconds(30));
            CloseProgram();
        }

        private void CloseProgram()
        {
            foreach (Process proces in ProcessChromeDriver)
            {
                Console.WriteLine($"proces: {proces.Id} Close.");
                proces.CloseMainWindow();
            }
            Application.Exit();
        }

        private void FormBot_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseProgram();
        }
    }
}

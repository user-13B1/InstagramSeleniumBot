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
    public partial class InForm : System.Windows.Forms.Form
    {
        private readonly Writer Cons;
        List<Process> ProcessChromeDriver;
        CancellationTokenSource cTokenSource;
        CancellationToken token;
        public InForm()
        {
            InitializeComponent();
            Cons = new Writer(Directory.GetCurrentDirectory(), this, textConsole);
            Cons.WriteLine("Initializ");
            ProcessChromeDriver = new List<Process>();
            StartPosition = FormStartPosition.Manual;
            Location = new Point(1300, 100);
           
            cTokenSource = new CancellationTokenSource();
            Task.Run(() => CloseAppTimer(TimeSpan.FromMinutes(50)));
        }

        private void CloseAppTimer(TimeSpan time)
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
            CloseApp();
        }

        private void CloseApp()
        {
            Thread.Sleep(TimeSpan.FromSeconds(4));
            cTokenSource.Cancel();
            Cons.WriteLine("Application closed.");
            foreach (Process proces in ProcessChromeDriver)
            {
                proces.CloseMainWindow();
            }
            Application.Exit();
        }

        private void ButtonStop_Click(object sender, EventArgs e) => cTokenSource.Cancel();

        async private void InForm_Load(object sender, EventArgs e)
        {
            token = cTokenSource.Token;
            Bot bot = new Bot(Cons, token);
            await Task.Run(() => bot.Start());
            CloseApp();
        }
    }
   
}

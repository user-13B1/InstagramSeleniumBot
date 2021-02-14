using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace InstagramSeleniumBot
{
    public class Writer
    {
        private readonly Form form;
        private readonly TextWriter _writer;
        private readonly string pathMessageLogTxtFile;
        private readonly string pathErrorLogTxtFile;
        private readonly object obj;

        public Writer(string dataPathDir, Form form, TextBox textBox)
        {
            this.form = form;
            obj = new object();
            pathMessageLogTxtFile = dataPathDir + @"\log_messages.txt";
            pathErrorLogTxtFile = dataPathDir + @"\log_errors.txt";
            _writer = new TextBoxStreamWriter(textBox);
            Console.SetOut(_writer);   // Перенаправляем выходной поток консоли   
        }

        public Writer(string dataPathDir)
        {
            obj = new object();
            pathMessageLogTxtFile = dataPathDir + @"\log_messages.txt";
            pathErrorLogTxtFile = dataPathDir + @"\log_errors.txt";
        }

        public Writer()
        {
            obj = new object();
            string dataPathDir = Directory.GetCurrentDirectory();
            pathMessageLogTxtFile = dataPathDir + @"\log_messages.txt";
            pathErrorLogTxtFile = dataPathDir + @"\log_errors.txt";
        }

        public void WriteLine(object textObj, bool log = true)
        {
            if (form != null)
            {
                if (form.InvokeRequired)
                    form.BeginInvoke(new Action(() => { Console.WriteLine(textObj); }));
                else
                    Console.WriteLine(textObj);
            }

            //Запись сообщения в лог файл
            if (log)
            {
                string textStr = String.Format("{0:F}  ", DateTime.Now) + String.Format($"{textObj}");
                WriteTextToFile(textStr, pathMessageLogTxtFile);
            }
        }

        public void WriteError(object textObj)
        {
            if (form != null)
            {
                if (form.InvokeRequired)
                {
                    form.BeginInvoke(new Action(() => { Console.Write("Error: {0:F} ", DateTime.Now); }));
                    form.BeginInvoke(new Action(() => { Console.WriteLine(textObj); }));
                }
                else
                {
                    Console.Write("Error: {0:F} ", DateTime.Now);
                    Console.WriteLine(textObj);
                }
            }

            //Запись сообщения в лог файл
            string textStr = String.Format("{0:F}  Error: ", DateTime.Now) + String.Format($"{textObj}");
            WriteTextToFile(textStr, pathErrorLogTxtFile);
         
        }



        private void WriteTextToFile(string text, string filePath)
        {
            if (String.IsNullOrWhiteSpace(text))
                return;

            lock (obj)
            {
                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.Default))
                {
                    writer.WriteLine(text);
                    writer.Flush();
                }
            }
        }
       
    }
}
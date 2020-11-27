using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace InstagramSeleniumBot
{
    internal class Writer
    {
        private readonly FormBot form;
        private readonly TextWriter _writer;
        private readonly FileReaderWriter file_reader;


        public Writer(FormBot form, FileReaderWriter file_reader)
        {
            this.form = form;

            this.file_reader = file_reader;
            _writer = new TextBoxStreamWriter(form.textConsole);
            Console.SetOut(_writer);   // Перенаправляем выходной поток консоли   
            WriteLine("Консоль включена.");
        }

        public void WriteLine(object s)
        {
            if (form.InvokeRequired)
            {
                //Form.BeginInvoke(new Action(() => { Console.Write("{0:T} ", DateTime.Now); }));
                form.BeginInvoke(new Action(() => { Console.WriteLine(s); }));
            }
            else
            {
                //Console.Write("{0:T} ", DateTime.Now);
                Console.WriteLine(s);
            }

            //Запись сообщения  в лог файл
            if (file_reader != null)
                file_reader.AddBufferMessage(s);
        }
    }
}
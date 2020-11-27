using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Security.Policy;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

namespace InstagramSeleniumBot
{
    internal class FileReaderWriter
    {
        private readonly string data_path_dir;
        private readonly string message_log_txt_path;
        readonly List<string> buffer_mes_list;

        public FileReaderWriter()
        {
            data_path_dir = Directory.GetCurrentDirectory();
            buffer_mes_list = new List<string>();

            if (!Directory.Exists(data_path_dir))
            {
                try
                {
                    Directory.CreateDirectory(data_path_dir); //создаем директорию лога
                }
                catch
                {
                    MessageBox.Show("Ошибка создания директории для лога.", "Ошибка.");
                }
            }
            if (Directory.Exists(data_path_dir))
            {

                message_log_txt_path = this.data_path_dir + @"\log_message.txt";
            }

            Task.Run(() => LogWriteToFile(TimeSpan.FromSeconds(5)));
        }


        private void LogWriteToFile(TimeSpan ping)
        {
            while (true)
            {
                
                WriteBufferToFile();
                Thread.Sleep(ping);

            }
        }

        internal void WriteBufferToFile()
        {
            if (buffer_mes_list.Count == 0)
                return;
            try
            {
                if (Directory.Exists(data_path_dir))
                {
                    using (StreamWriter sw = new StreamWriter(message_log_txt_path, true, Encoding.Default))
                    {
                        var s_mes = String.Join("\n", buffer_mes_list.ToArray());
                        buffer_mes_list.Clear();
                        sw.WriteLine(s_mes);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Ошибка {e.Message}", "Ошибка.");
            }

        }

        internal void AddBufferMessage(object s)
        {
            string mess = String.Format("{0:T} ", DateTime.Now) + String.Format($"{s}");
            buffer_mes_list.Add(mess);
        }

    }
}
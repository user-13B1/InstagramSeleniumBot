using System;
using System.Data.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Data;
using System.Data.Linq.Mapping;
using System.Runtime.InteropServices.WindowsRuntime;

namespace InstagramSeleniumBot
{
    class DBmanager
    {
        readonly string connectionString = @"Data Source=SERVER\SQLEXPRESS;Initial Catalog=LoaclBD;Integrated Security=False;Connect Timeout = 7;User Id = test;Password = 123;";
        readonly Writer Cons;
        private string name;
        private readonly DataContext db;
        public DBmanager(Writer cons)
        {
            Cons = cons;
            db = new DataContext(connectionString);
        }

        public string Name { get => name; set => name = value; }

        internal bool GetUrlForSubscribe(out string url)
        {
            var query = from user in db.GetTable<ParseInstagramUsers>()
                        where user.Tomakefriend == 1
                        where user.Myname == name
                        orderby user.Date
                        select user;

            if (query.Count() == 0)
            {
                url = null;
                return false;
            }

            url = query.First().Url;
            return true;
        }

        internal bool GetUrlForUnSubscribe(out string url)
        {
            var query = from user in db.GetTable<ParseInstagramUsers>()
                        where user.Myfriend == 1
                        where user.Myname == name
                        orderby user.Date 
                        select user;




            if (query.Count() == 0)
            {
                url = null;
                return false;
            }

            url = query.First().Url;

            TimeSpan time_interval = DateTime.Now.Subtract(query.First().Date);
            Cons.WriteLine($"Unsubscribe time_interval {time_interval}");
            if (time_interval < TimeSpan.FromDays(2))
                return false;
            return true;
        }

        internal bool GetRandomUrl(out string url)
        {
            Random rand = new Random();
            //var query = from user in db.GetTable<ParseInstagramUsers>()
            //                //orderby Guid.NewGuid()
            //            where user.Donorsubs == 1
            //            select user;

            var userList = db.GetTable<ParseInstagramUsers>().Where(u => u.Donorsubs == 1).ToList();
            if (userList.Count == 0)
            {
                Cons.WriteLine($"No links found");
                url = null;
                return false;
            }
           //url = userList[rand.Next(userList.Count)].Url;
            url = userList[0].Url;
            MakeUrlNotInterest(url);
            return true;
        }

        internal void MakeUrlNotInterest(string url)
        {
            ParseInstagramUsers user = db.GetTable<ParseInstagramUsers>().Where(u => u.Url == url).FirstOrDefault();
            user.Nointerst = 1;
            user.Myfriend = 0;
            user.Tomakefriend = 0;
            user.Donorsubs = 0;
            user.Awaiting = 0;
            db.SubmitChanges();
            return;
        }

        internal bool AddUrlToFriend(string url)
        {
            ParseInstagramUsers user = db.GetTable<ParseInstagramUsers>().Where(u => u.Url == url).FirstOrDefault();
            user.Nointerst = 0;
            user.Myfriend = 1;
            user.Tomakefriend = 0;
            user.Donorsubs = 0;
            user.Awaiting = 0;
            user.Myname = name;
            user.Date = DateTime.Now;
            db.SubmitChanges();
            return true;
        }

        internal bool AddUrlInMyFutereSubs(string url)
        {
           
            ParseInstagramUsers user = db.GetTable<ParseInstagramUsers>().Where(u => u.Url == url).FirstOrDefault();
            user.Nointerst = 0;
            user.Myfriend = 0;
            user.Tomakefriend = 1;
            user.Donorsubs = 0;
            user.Awaiting = 0;
            db.SubmitChanges();
            return true;
        }

        internal bool AddUrlInGetDonorSubsList(string url)
        {
           
            ParseInstagramUsers user = db.GetTable<ParseInstagramUsers>().Where(u => u.Url == url).FirstOrDefault();
            user.Nointerst = 0;
            user.Myfriend = 0;
            user.Tomakefriend = 0;
            user.Donorsubs = 1;
            user.Awaiting = 0;
            db.SubmitChanges();
            return true;
        }

        internal bool AddNewUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
                return false;

            if (IsRepeat(url))
                return false;

            ParseInstagramUsers newUser = new ParseInstagramUsers {Url=url, Myname=name, Date=DateTime.Now, Awaiting = 1, Tomakefriend=0, Myfriend=0, Nointerst=0, Donorsubs=0 };
            db.GetTable<ParseInstagramUsers>().InsertOnSubmit(newUser);
            db.SubmitChanges();
            return true;
        }

        internal List<string> GetUrlAwaitingList(int count_acc)
        {
            List<string> urlList = new List<string>(count_acc);
            var userList = db.GetTable<ParseInstagramUsers>().Where(u => u.Awaiting == 1).Take(count_acc).ToList();

            foreach (var user in userList)
            {
                urlList.Add(user.Url);
            }

            return urlList;
        }

        internal bool IsRepeat(string url)
        {
            var query = db.GetTable<ParseInstagramUsers>().Where(u => u.Url == url);

            if (query.Count() != 0)
            {
                Cons.WriteLine($"Repeat account.");
                return true;
            }
            else
                return false;
        }

        internal void GetStatisticDB()
        {
            var query = from user in db.GetTable<ParseInstagramUsers>()
                        where user.Awaiting == 1
                        orderby user.Date
                        select user;
            Cons.WriteLine($"Аккаунтов для обработки - {query.Count()}");

            query = from user2 in db.GetTable<ParseInstagramUsers>()
                    where user2.Tomakefriend == 1
                    orderby user2.Date
                    select user2;

            Cons.WriteLine($"Аккаунтов для подписки - {query.Count()}"); 

            query = from user3 in db.GetTable<ParseInstagramUsers>()
                    where user3.Myfriend == 1
                    orderby user3.Date
                    select user3;
            Cons.WriteLine($"Аккаунтов для отписки - {query.Count()}");

            query = from user4 in db.GetTable<ParseInstagramUsers>()
                    where user4.Donorsubs == 1
                    orderby user4.Date
                    select user4;
            Cons.WriteLine($"Аккаунтов для списка подписчиков - {query.Count()}");

            query = from user5 in db.GetTable<ParseInstagramUsers>()
                    orderby user5.Date
                    select user5;

            Cons.WriteLine($"Всего - {query.Count()}");

            return;

        }

        internal void ClearDB()
        {
            var user = db.GetTable<ParseInstagramUsers>().OrderByDescending(u => u.Id);
            db.GetTable<ParseInstagramUsers>().DeleteAllOnSubmit(user);
            db.SubmitChanges();


        }
        [Table(Name = "ParseInstagramUsers")]
        public class ParseInstagramUsers
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }
            [Column(Name = "url")]
            public string Url { get; set; }
            [Column(Name = "myname")]
            public string Myname { get; set; }
            [Column(Name = "date")]
            public DateTime Date { get; set; }
            [Column(Name = "awaiting")]
            public Byte Awaiting { get; set; }
            [Column(Name = "tomakefriend")]
            public Byte Tomakefriend { get; set; }
            [Column(Name = "myfriend")]
            public Byte Myfriend { get; set; }
            [Column(Name = "nointerst")]
            public Byte Nointerst { get; set; }
            [Column(Name = "donorsubs")]
            public Byte Donorsubs { get; set; }
        }
    }
}

using OpenQA.Selenium;
using System.Threading;
using System;

namespace InstagramSeleniumBot
{

    public class CollectingAccounts : Bot
    {

        public CollectingAccounts(Writer Cons, CancellationToken token) : base(Cons, token) { }

        public override void Start(int limit)
        {
            if (token.IsCancellationRequested)
                return;

            db.GetStatisticDB();

            if (!OpenRandSubsPage())
                return;

            for (int i = 5; i < limit + 5; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                Thread.Sleep(30 + rand.Next(30));

                if (!Chrome.Scroll($@"//ul/div/li[{i}]"))
                {
                    Cons.WriteError($"Не удается проскролить.");
                    return;
                }
                db.AddNewUrl(ParseUrlAccount(i));
               
            }
            Cons.WriteLine($"Сбор завершен.");
        }
      
        internal bool OpenRandSubsPage()
        {
            for (int j = 0; j < 5; j++)
            {
                if (!db.GetRandomUrl(out string rand_url))
                    return false;

                Chrome.OpenUrl(rand_url);

                if (Chrome.ClickButtonXPath("//a[contains(.,'подписчиков')]"))
                    return true;
            }
            return false;
        }


        internal string ParseUrlAccount(int i)
        {
            if (token.IsCancellationRequested)
                return null;

            string path = $@"/html/body/div[5]/div//li[{i}]//a";
            IWebElement element = Chrome.FindWebElement(By.XPath(path));
            if (element == null)
            {
                Cons.WriteError("Ссылка для подписок не найдена.");
                return null;
            }

            string url = element.GetAttribute("href");
            if (url == null || url == "")
            {
                Cons.WriteError("Ссылка на подписчика пуста.");
                return null;
            }

            Cons.WriteLine($"Add {url.Trim()}");
            return url;
        }
    }
}
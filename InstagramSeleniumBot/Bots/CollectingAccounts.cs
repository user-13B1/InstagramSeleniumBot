using OpenQA.Selenium;
using System.Threading;
using System;

namespace InstagramSeleniumBot
{

    internal class CollectingAccounts : Bot
    {

        public CollectingAccounts(Writer Cons, ref Action EndWorkEvent,string botProfileName) : base(Cons,  ref EndWorkEvent, botProfileName)
        {
            Name = "CollectingAccountsBot";
            
        }

        internal override void Start(int limit)
        {
            if (!OpenRandSubsPage())
                return;

            for (int i = 5; i < limit + 5; i++)
            {
                Thread.Sleep(30 + rand.Next(30));
                if (!Chrome.Scroll($@"//ul/div/li[{i}]"))
                {
                    Cons.WriteLine($"{Name} Ошибка. Не удается проскролить.");
                    return;
                }
                db.AddNewUrl(GetUrlForSubs(i));
               
            }
            Cons.WriteLine($"{Name} Сбор завершен.");
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


        internal string GetUrlForSubs(int i)
        {
            string path = $@"/html/body/div[5]/div//li[{i}]//a";
            IWebElement element = Chrome.FindWebElement(By.XPath(path));
            if (element == null)
            {
                Cons.WriteLine($"{Name} GetUrlForSubs: Ссылка на подписчика не найдена.");
                return null;
            }

            string url = element.GetAttribute("href");
            if (url == null || url == "")
            {
                Cons.WriteLine($"{Name} GetUrlForSubs: Ссылка на подписчика пуста.");
                return null;
            }
            return url;
        }
    }
}
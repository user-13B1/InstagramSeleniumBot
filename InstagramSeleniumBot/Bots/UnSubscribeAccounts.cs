using OpenQA.Selenium;
using System;
using System.Threading;

namespace InstagramSeleniumBot
{
    internal class UnSubscribeAccounts : Bot
    {
        public UnSubscribeAccounts(Writer Cons,ref Action EndWorkEvent, string botProfileName) : base(Cons, ref EndWorkEvent,  botProfileName)
        {
            Name = "UnSubscribeAccounts";
        }

        internal override void Start(int limit)
        {
           
            for (int i = 0; i < limit; i++)
            {
                if(!db.GetUrlForUnSubscribe(out string url))
                    return;

                db.MakeUrlNotInterest(url);
                UnSubscribe(url,i);
            }
       
        }

        private bool UnSubscribe(string url,int i)
        {
            IWebElement element;
            Chrome.OpenUrl(url);
           
            Thread.Sleep(TimeSpan.FromSeconds(2));
            if (Chrome.IsElementPage(By.LinkText("Назад в Instagram.")))
            {
                Cons.WriteLine($"{Name} Страницы не существует.");
                return true;
            }

            string xpath = @"//*[@class='FLeXg bqE32']//button";
            Chrome.FindWebElement(By.XPath(xpath));
          
            Cons.WriteLine($"{Name} Отписка {i} - {url.Trim()}");

            Thread.Sleep(TimeSpan.FromSeconds(5 + rand.Next(10)));
            Chrome.ClickButtonXPath(xpath);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Chrome.ClickButtonXPath(@"//button[contains(.,'Отменить подписку')]");
            Thread.Sleep(TimeSpan.FromSeconds(3));

            element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Подписаться')]"));
            if (element == null)
            {
                Cons.WriteLine($"{Name} Не удалось отписаться.");
                return false;
            }
            Cons.WriteLine($"{Name} Отписаны.");
            return true;
        }

    }
}

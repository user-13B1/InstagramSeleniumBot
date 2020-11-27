using OpenQA.Selenium;
using System;
using System.Threading;


namespace InstagramSeleniumBot
{
    
    internal class SubscribeAccounts : Bot
    {
        public SubscribeAccounts(Writer Cons,ref Action EndWorkEvent, string botProfileName) : base(Cons, ref EndWorkEvent,botProfileName)
        {
            Name = "SubscribeAccounts";
        }


        internal override void Start(int limit)
        {
            for (int i = 0; i < limit; i++)
            {
                if (!db.GetUrlForSubscribe(out string url))
                    return;

                Cons.WriteLine($"{Name} {i}).Подписка на {url}".Trim());
                Chrome.OpenUrl(url);

                if (!Subscribe(url))
                    return;
            }
        }

        internal bool Subscribe(string url)
        {

#if DEBUG
            Thread.Sleep(TimeSpan.FromSeconds(1));
#else
             Thread.Sleep(TimeSpan.FromSeconds(15 + rand.Next(15)));
#endif

            IWebElement element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Подписаться')]"));
            if (element == null) 
            {
                Cons.WriteLine($"{Name} Не удалось подписаться.");
                db.MakeUrlNotInterest(url);
                return true;
            }
            //Проверка на друзей 

            element.Click();


#if DEBUG
            Thread.Sleep(TimeSpan.FromSeconds(1));
#else
             Thread.Sleep(TimeSpan.FromSeconds(6));
#endif


            element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Отправить сообщение')]"));
            if (element != null)
            {
               
                db.AddUrlToFriend(url);
                return true;
            }
            Cons.WriteLine($"{Name} Неудалось подписаться.");
            element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Сообщить о проблеме')]"));
            if (element != null)
            {
                Cons.WriteLine($"{Name} Превышен лимит подписок.");
                return false;
            }

            return true;
        }
    }
}

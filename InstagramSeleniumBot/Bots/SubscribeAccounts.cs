using OpenQA.Selenium;
using System;
using System.Threading;


namespace InstagramSeleniumBot
{

    public class SubscribeAccounts : Bot
    {
        public SubscribeAccounts(Writer Cons, CancellationToken token) : base(Cons, token) {}


        public override void Start(int limit)
        {
            if (token.IsCancellationRequested)
                return;
            db.GetStatisticDB();
            for (int i = 0; i < limit; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                if (!db.GetUrlForSubscribe(out string url))
                    return;

                Cons.WriteLine($"{i}).Подписка на {url.Trim()}");
                Chrome.OpenUrl(url);

                if (!Subscribe(url))
                    return;
            }
        }

        private bool Subscribe(string url)
        {
            if (token.IsCancellationRequested)
                return false;

#if DEBUG
            Thread.Sleep(TimeSpan.FromSeconds(1));
#else
             Thread.Sleep(TimeSpan.FromSeconds(15 + rand.Next(15)));
#endif

            IWebElement element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Подписаться')]"));
            if (element == null) 
            {
                Cons.WriteLine($"Не удалось подписаться.");
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
            Cons.WriteLine($"Не удалось подписаться.");
            element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Сообщить о проблеме')]"));
            if (element != null)
            {
                Cons.WriteLine($"Превышен лимит подписок.");
                return false;
            }

            return true;
        }
    }
}

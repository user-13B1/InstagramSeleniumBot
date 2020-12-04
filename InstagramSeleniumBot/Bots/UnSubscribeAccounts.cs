using OpenQA.Selenium;
using System;
using System.Threading;

namespace InstagramSeleniumBot
{
    public class UnSubscribeAccounts : Bot
    {
        public UnSubscribeAccounts(Writer Cons, CancellationToken token) : base(Cons, token) { }

        public override void Start(int limit)
        {
            if (token.IsCancellationRequested)
                return;
            db.GetStatisticDB();

            for (int i = 0; i < limit; i++)
            {
                if (token.IsCancellationRequested)
                    return;

                if (!db.GetUrlForUnSubscribe(out string url))
                    return;
                
                UnSubscribe(url,i);
            }
        }

        public bool UnSubscribe(string url,int i)
        {
            if (token.IsCancellationRequested)
                return true;

            db.MakeUrlNotInterest(url);

            IWebElement element;
            Chrome.OpenUrl(url);
           
            Thread.Sleep(TimeSpan.FromSeconds(2));
            if (Chrome.IsElementPage(By.LinkText("Назад в Instagram.")))
            {
                Cons.WriteLine($"Страницы не существует.");
                return true;
            }

            string xpath = @"//*[@class='FLeXg bqE32']//button";
            Chrome.FindWebElement(By.XPath(xpath));
          
            Cons.WriteLine($"Отписка {i} - {url.Trim()}");

            Thread.Sleep(TimeSpan.FromSeconds(5 + rand.Next(10)));
            Chrome.ClickButtonXPath(xpath);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Chrome.ClickButtonXPath(@"//button[contains(.,'Отменить подписку')]");
            Thread.Sleep(TimeSpan.FromSeconds(3));

            element = Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Подписаться')]"));
            if (element == null)
            {
                Cons.WriteLine($"Не удалось отписаться.");
                return false;
            }
            Cons.WriteLine($"Отписаны.");
            return true;
        }

    }
}

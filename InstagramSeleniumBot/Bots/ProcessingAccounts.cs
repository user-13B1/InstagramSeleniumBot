using OpenQA.Selenium;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace InstagramSeleniumBot
{

    internal class ProcessingAccounts : Bot
    {
        public ProcessingAccounts(Writer Cons,  ref Action EndWorkEvent, string botProfileName) : base(Cons, ref EndWorkEvent, botProfileName)
        {
            Name = "ProcessingAccounts";
        }

        internal override void Start(int limit)
        {
            List<string> url = db.GetUrlAwaitingList(limit);      //Получить список аккаунтов
            for (int i = 0; i < url.Count; i++)
            {
                Cons.WriteLine($"{i} {url[i]}");
                Chrome.OpenUrl(url[i]);
                
                if (!Parsing(url[i]))
                    break;
            }
        }


        internal bool Parsing(string url)
        {


            Thread.Sleep(5000);
            //проверка  загрузки страницы
            if (Chrome.FindWebElement(By.XPath(@"/html/body/div")) == null)
            {
                Cons.WriteLine($"{Name} Ошибка загрузки страницы.");
                return false;
            }

            //проверка существования страницы
            if (Chrome.IsElementPage(By.LinkText("Назад в Instagram.")))
            {
                Cons.WriteLine($"{Name} Страницы не существует.");
                db.MakeUrlNotInterest(url);
                return true;
            }

            //Проверка страницы на открытость
            string xPath = @"//*[@id='react-root']/section/main/div/ul/li[2]/span/span";
            if (Chrome.IsElementPage(By.XPath(xPath)))
            {
                Cons.WriteLine($"{Name} Страница закрыта.");
                db.MakeUrlNotInterest(url);
                return true;
            }

            if (Chrome.FindWebElement(By.XPath(@"//button[contains(.,'Подписаться')]"))==null)
            {
                Cons.WriteLine($"{Name} Ошибка! Кнопка подписаться не найдена.");
                db.MakeUrlNotInterest(url);
                return true;
            }

            int num_subs = ParseSpanTextToInt("подписчиков");       //Парссинг количества подписчиков
            int num_subc = ParseSpanTextToInt("подписок");          //Парссинг количества подписок
            Cons.WriteLine($"{Name} Подписчиков {num_subs} Подписок {num_subc}");

            //забираем для списка подписок
            if (num_subs > 400)
            {
                db.AddUrlInGetDonorSubsList(url);
                return true;
            }

            //забираем для подписки
            if (num_subs > 40 && num_subs < 200 && num_subs < 2000 && num_subc / num_subs >= 2)
            {
                db.AddUrlInMyFutereSubs(url);
                return true;
            }

            db.MakeUrlNotInterest(url);
            return true;
        }

        private int ParseSpanTextToInt(string s)
        {
            int num = 0;
            IWebElement element = Chrome.FindWebElement(By.XPath($@"//a[contains(.,'{s}')]/span"));
            if (element == null)
                element = Chrome.FindWebElement(By.XPath(@"//span[contains(.,'подписчиков')]/span"));
            if (element != null)
                num = ParsToInt(element.Text);
            return num;
        }
    }


}

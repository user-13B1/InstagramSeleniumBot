using OpenQA.Selenium;
using System.Threading;
using System;

namespace InstagramSeleniumBot
{

    internal class Info : Bot
    {
        public Info(Writer Cons, ref Action EndWorkEvent, string botProfileName) : base(Cons, ref EndWorkEvent, botProfileName)
        {
            Name = "InfoBot";
        }

        internal override void Start(int limit)
        {
            db.GetStatisticDB();
        }
    }
}
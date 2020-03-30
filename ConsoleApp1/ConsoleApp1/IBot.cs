using System;
using System.Collections.Generic;
using System.Text;

namespace NewsBotProject
{
    public interface IBot
    {
        public  News GetLasNewsFromSite(string link);


        public void BotForAdminPanel(News news);
    }
}

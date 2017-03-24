﻿using System.Web.Mvc;

namespace TFiN.OAuthLab.Areas.OAuth
{
    public class OAuthAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OAuth";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OAuth_default",
                "OAuth/{controller}/{action}/{id}",
                new { action = "Authorize", id = UrlParameter.Optional }
            );
        }
    }
}
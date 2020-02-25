﻿using PivotalServices.CloudFoundry.Replatform.Bootstrap.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace WinAuthSample
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AppBuilder.Instance
                //.AddCloudFoundryActuators()
                //.AddConsoleSerilogLogging(true)
                //.AddCloudFoundryMetricsForwarder()
                .AddWindowsAuthentication()
                .Build()
                .Start();
        }

        protected void Application_Stop()
        {
            AppBuilder.Instance.Stop();
        }
    }
}
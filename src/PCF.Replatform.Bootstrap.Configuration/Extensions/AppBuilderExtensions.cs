﻿using Microsoft.Extensions.Configuration;
using PivotalServices.CloudFoundry.Replatform.Bootstrap.Base;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using Steeltoe.Extensions.Configuration.ConfigServer;
using System;

namespace PivotalServices.CloudFoundry.Replatform.Bootstrap.Configuration
{
    public static class AppBuilderExtensions
    {
        const string ASPNET_ENV_VAR = "ASPNETCORE_ENVIRONMENT";

        /// <summary>
        /// Adds Json, Environment Variables and VCAP Services as configuration sources
        /// </summary>
        /// <param name="environment"></param>
        public static AppBuilder AddDefaultConfigurationProviders(this AppBuilder instance, bool jsonSettingsOptional = true, string environment = null)
        {
            environment = environment ?? GetEnvironment();

            instance.ConfigureAppConfigurationDelegates.Add((builderContext, configBuilder) => {
                configBuilder.AddWebConfiguration();
                configBuilder.AddJsonFile("appSettings.json", jsonSettingsOptional, false);
                configBuilder.AddJsonFile($"appSettings.{environment?.ToLower()}.json", true, false);
                configBuilder.AddEnvironmentVariables();
                configBuilder.AddCloudFoundry();
            });

            instance.ConfigureServicesDelegates.Add((builderContext, services) => {
                services.ConfigureCloudFoundryOptions(builderContext.Configuration);
                WebConfigurationHelper.OverrideWebConfiguration(builderContext.Configuration);
            });

            return instance;
        }

        /// <summary>
        /// Adds spring cloud config server as a configuration source
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static AppBuilder AddConfigServer(this AppBuilder instance, string environment = null)
        {
            environment = environment ?? GetEnvironment();

            instance.ConfigureAppConfigurationDelegates.Add((builderContext, configBuilder) => {
                    var clientSettings = new ConfigServerClientSettings { Environment = environment };
                    configBuilder.AddConfigServer();
            });

            instance.ConfigureServicesDelegates.Add((builderContext, services) => {
                services.ConfigureCloudFoundryOptions(builderContext.Configuration);
                WebConfigurationHelper.OverrideWebConfiguration(builderContext.Configuration);
            });

            return instance;
        }

        private static string GetEnvironment()
        {
            if (Environment.GetEnvironmentVariable(ASPNET_ENV_VAR) == null)
                Environment.SetEnvironmentVariable(ASPNET_ENV_VAR, "Development", EnvironmentVariableTarget.Machine);

            return Environment.GetEnvironmentVariable(ASPNET_ENV_VAR);
        }
    }
}
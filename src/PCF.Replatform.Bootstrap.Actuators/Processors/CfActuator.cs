﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using PivotalServices.CloudFoundry.Replatform.Bootstrap.Base;
using PivotalServices.CloudFoundry.Replatform.Bootstrap.Base.Ioc;
using Steeltoe.Common.Diagnostics;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Extensions.Logging;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Endpoint.Health.Contributor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace PivotalServices.CloudFoundry.Replatform.Bootstrap.Actuators
{
    public class CfActuator : IActuator
    {
        public void Configure()
        {
            var configuration = DependencyContainer.GetService<IConfiguration>();
            var loggerFactory = DependencyContainer.GetService<ILoggerFactory>();
            var dynamicLoggerProvider = GetDynamicLoggerProvider(configuration);
            loggerFactory.AddProvider(dynamicLoggerProvider);

            ActuatorConfigurator.UseCloudFoundryActuators(configuration,
                                                            dynamicLoggerProvider,
                                                            GetHealthContributors(),
                                                            GlobalConfiguration.Configuration.Services.GetApiExplorer(),
                                                            loggerFactory);

            ActuatorConfigurator.UseMetricsActuator(configuration, loggerFactory);
        }

        public void Start()
        {
            DiagnosticsManager.Instance.Start();
        }

        public void Stop()
        {
            DiagnosticsManager.Instance.Stop();
        }

        private IEnumerable<IHealthContributor> GetHealthContributors()
        {
            var healthContributors = DependencyContainer.GetService<IEnumerable<IHealthContributor>>().ToList();
            healthContributors.Add(new DiskSpaceContributor());
            return healthContributors;
        }

        private IDynamicLoggerProvider GetDynamicLoggerProvider(IConfiguration configuration)
        {
            return DependencyContainer.GetService<IDynamicLoggerProvider>() ?? new DynamicLoggerProvider(new ConsoleLoggerSettings().FromConfiguration(configuration));
        }
    }
}

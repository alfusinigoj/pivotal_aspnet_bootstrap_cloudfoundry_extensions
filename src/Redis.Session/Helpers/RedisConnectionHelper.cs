﻿using Microsoft.Web.Redis;
using PivotalServices.AspNet.Bootstrap.Extensions.Ioc;
using StackExchange.Redis;
using System;

namespace PivotalServices.AspNet.Bootstrap.Extensions.Cf
{
    public static class RedisConnectionHelper
    {
        static IConnectionMultiplexer connectionMultiplexer;

#pragma warning disable CS0169 //Just to protect the reference to Microsoft.Web.RedisSessionStateProvider (avoid accidental removal)
        static RedisSessionStateProvider redisSessionStateProvider;
#pragma warning restore CS0169 

        static RedisConnectionHelper()
        {
            connectionMultiplexer = DependencyContainer.GetService<IConnectionMultiplexer>() ?? throw new ArgumentNullException(nameof(IConnectionMultiplexer));
        }

        public static string GetConnectionString()
        {
            return connectionMultiplexer.Configuration;
        }
    }
}

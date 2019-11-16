﻿using AutofacSample.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AutofacSample.Controllers
{
    [Route("api/values")]
    public class ValuesController : ApiController
    {
        private readonly ICalcService service;
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ICalcService service, ILogger<ValuesController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [Route("~/api/calc/{x:int}/{y:int}/sum")]
        [HttpGet]
        public int Sum(int x, int y)
        {
            return service.Add(x, y);
        }

        [Route("~/api/calc/{x:int}/{y:int}/diff")]
        [HttpGet]
        public int Diff(int x, int y)
        {
            return service.Substract(x, y);
        }

        public IEnumerable<string> Get()
        {
            return new string[] { "api1: sum/{x}/{y}", "api2: diff/{x}/{y}" };
        }
    }
}

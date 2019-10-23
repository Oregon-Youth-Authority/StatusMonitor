using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IpDump.Controllers
{
    [Route("Home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
       public string Get()
       {
          return Request.HttpContext.Connection.RemoteIpAddress.ToString();
       }
    }
}
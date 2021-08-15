using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDS_Server.Plugins
{
    [Route("api/fop")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class FopController : ControllerBase
    {
    }
}

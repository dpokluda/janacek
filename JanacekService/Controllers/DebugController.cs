// -------------------------------------------------------------------------
// <copyright file="DebugController.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <project>Delivery Catalog Service</project>
// <summary>
//  
//    Object representing the DebugController.cs.
//  
// </summary>
// -------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;

namespace JanacekService.Controllers
{
    [Route("janacek/debug")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        // GET
        [HttpGet]
        public ActionResult<string> Get()
        {
           return ":-)";
        }
    }
}

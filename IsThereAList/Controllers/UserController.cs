using IsThereAList.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace IsThereAList.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IsThereAListContext _cxt;

        public UserController(IsThereAListContext cxt)
        {
            _cxt = cxt;
        }

        [HttpGet("[Action]")]
        public IEnumerable<User> Unauthorised()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{ownerUserId}/list/[Action]")]
        public IEnumerable<List> Owned()
        {
            throw new NotImplementedException();
        }

        [HttpPut("authorise/{userIdCsv}")]
        public void Authorise(string userIdCsv)
        {
            throw new NotImplementedException();
        }
    }
}

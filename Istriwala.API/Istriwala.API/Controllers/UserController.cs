using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Istriwala.API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        #region Fields

        IUserRepository _repository;

        #endregion Fields


        #region Constructors

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }
        #endregion Constructors


        #region Public API

        [HttpPost]
        [Route("registeruser")]
        public IHttpActionResult RegisterUser(User user)
        {
            user = _repository.Create(user);
           return Ok(user);
        }

        [HttpGet]
        [Route("getuser/{id}")]
        public IHttpActionResult GetUser(int id)
        {
            var user=_repository.Get(id);
            return Ok(user);
        }
        #endregion Public API
    }
}

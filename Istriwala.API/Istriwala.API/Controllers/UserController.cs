using Istriwala.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Istriwala.API.Controllers
{
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
    }
}

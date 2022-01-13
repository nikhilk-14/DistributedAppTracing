using Microsoft.AspNetCore.Mvc;
using SecondApi.Models;
using SecondApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SecondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        public readonly IUserService _userService;

        public CRUDController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/<CRUDController>
        [HttpGet]
        public List<User> Get()
        {
            var users = _userService.GetUsers();
            return users;
        }

        // GET api/<CRUDController>/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            var user = _userService.GetUser(id);
            return user;
        }

        // POST api/<CRUDController>
        [HttpPost]
        public int Post([FromBody] User value)
        {
            var result = _userService.CreateUser(value);
            return result;
        }

        // PUT api/<CRUDController>/5
        [HttpPut("{id}")]
        public int Put(int id, [FromBody] User value)
        {
            var result = _userService.UpdateUser(id, value);
            return result;
        }

        // DELETE api/<CRUDController>/5
        [HttpDelete("{id}")]
        public int Delete(int id)
        {
            var result = _userService.DeleteUser(id);
            return result;
        }
    }
}

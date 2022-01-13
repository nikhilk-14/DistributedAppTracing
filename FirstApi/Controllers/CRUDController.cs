using FirstApi.Models;
using FirstApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FirstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        public readonly IUserService _userService;
        public readonly IUserDataService _userDataService;

        public CRUDController(IUserService userService, IUserDataService userDataService)
        {
            _userService = userService;
            _userDataService = userDataService;
        }

        // GET: api/<CRUDController>
        [HttpGet]
        public async Task<List<User>> Get()
        {
            var users = await _userDataService.GetUsers();
            return users;
        }

        // GET api/<CRUDController>/5
        [HttpGet("{id}")]
        public async Task<User> Get(int id)
        {
            var user = await _userDataService.GetUser(id);
            return user;
        }

        // POST api/<CRUDController>
        [HttpPost]
        public async Task<int> Post([FromBody] User value)
        {
            var result = await _userDataService.CreateUser(value);
            return result;
        }

        // PUT api/<CRUDController>/5
        [HttpPut("{id}")]
        public async Task<int> Put(int id, [FromBody] User value)
        {
            var result = await _userDataService.UpdateUser(id, value);
            return result;
        }

        // DELETE api/<CRUDController>/5
        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            var result = await _userDataService.DeleteUser(id);
            return result;
        }
    }
}

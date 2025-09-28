using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Data;
using TheGrind5_EventManagement.Models;
using TheGrind5_EventManagement.Models.Entities;

namespace TheGrind5_EventManagement.Controllers
{
    //localhost:xxxx/api/user
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet] 
        public IActionResult GetAllUser()
        {
            var allUsers = dbContext.User.ToList();
            return Ok(allUsers);
        }

/*        [HttpPost]
        public IActionResult AddUser(AddUserDto addUserDto)
        {
            //ánh xạ
            var userEntity = new User()
            {
                FullName = addUserDto.FullName,
                Email = addUserDto.Email,
                PhoneNumber = addUserDto.PhoneNumber,
                Password = addUserDto.Password,
            };
            dbContext.User.Add()
        }*/
    }
}

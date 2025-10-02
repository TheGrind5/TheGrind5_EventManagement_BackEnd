using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheGrind5_EventManagement.Models;

namespace TheGrind5_EventManagement.Controllers
{
    //localhost:xxxx/api/user
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController(EventDBContext dBContext)
        {
            DBContext = dBContext;
        }

        private readonly EventDBContext DBContext;
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            DBContext.Users.ToList();
        }
    }
}
//Singleton

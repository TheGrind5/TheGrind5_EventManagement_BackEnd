using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheGrind5_EventManagement.Controllers
{
    //localhost:xxxx/api/user
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            
        }
    }
}
//Singleton

using MagicVilla_VillaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]  // But I am not a fan of using the controller like this because what happens is down the road, if for some reson you have to change the controller name or the file name here, because what happens if in funture you have to change the controller name, the route will automaticaly change for all of the connected clients. So if you endpoint is used by many consumers, you have to notify all of them that the route has changed. And that is huge pain. Because of that, I like to hardcode tha route here and that will be our API. So even if down the road, if you change the controller name, your route does not change.
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Villa> GetVillas() 
        {
            return new List<Villa> { 
            new Villa{ Id = 1,Name="Pool View"},
            new Villa{ Id = 2,Name="Beach View"}
            };
        }
    }
}

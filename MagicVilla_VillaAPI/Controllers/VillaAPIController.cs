using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]  // But I am not a fan of using the controller like this because what happens is down the road, if for some reson you have to change the controller name or the file name here, because what happens if in funture you have to change the controller name, the route will automaticaly change for all of the connected clients. So if you endpoint is used by many consumers, you have to notify all of them that the route has changed. And that is huge pain. Because of that, I like to hardcode tha route here and that will be our API. So even if down the road, if you change the controller name, your route does not change.
    [Route("api/VillaAPI")]
    [ApiController] // Model validation / DataAnnotations Depend on ApiController attribute. ApiController attribute auto check Data Annotation
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        public VillaAPIController(ILogger<VillaAPIController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas() 
        {
            _logger.LogInformation("Getting all villas");

            var villas = VillaStore.villaList;
            return Ok(villas);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        //[HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]  // We can definte Response type herdcoded.
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public ActionResult GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Get Villa Error with Id " + id);
                return BadRequest();
            }
            VillaDTO villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null) 
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            // Explicitly Check the model state when we not use [ApiController] attribute. 
            // when we add [ApiController] attribute then Data validation check ApiController before Model State. So we use [ApiController] attribute then no need to check model state.
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (VillaStore.villaList.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null ) //Custom Vaidation
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists!");
                return BadRequest(ModelState);
            }

            if (villaDTO == null) 
            { 
                return BadRequest(villaDTO); 
            }
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);

            //return Ok(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);   // call  GetVilla(int id) api
        }
        [HttpDelete("{id:int}", Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            VillaDTO villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            if (villa == null)
            {
                return NotFound(villa);
            }
            VillaStore.villaList.Remove(villa);

            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            VillaDTO villa = VillaStore.villaList.FirstOrDefault(n => n.Id == id);

            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }

        // Patch supproted for 2 Packages. there are  Microsoft.AspNetCore.JsonPatch, Microsoft.AspNetCore.Mvc.NewtonsoftJson
        // For Replace: { "op": "replace", "path": "/biscuits/0/name", "value": "Chocolate Digestive" }
        // Source Link: https://jsonpatch.com/
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(n => n.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }

            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}

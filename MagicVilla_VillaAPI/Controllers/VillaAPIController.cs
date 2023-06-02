using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //[Route("api/[controller]")]  // But I am not a fan of using the controller like this because what happens is down the road, if for some reson you have to change the controller name or the file name here, because what happens if in funture you have to change the controller name, the route will automaticaly change for all of the connected clients. So if you endpoint is used by many consumers, you have to notify all of them that the route has changed. And that is huge pain. Because of that, I like to hardcode tha route here and that will be our API. So even if down the road, if you change the controller name, your route does not change.
    [Route("api/VillaAPI")]
    [ApiController] // Model validation / DataAnnotations Depend on ApiController attribute. ApiController attribute auto check Data Annotation
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        //private readonly ApplicationDbContext _context;
        private readonly IVillaRepository _villaRepository;
        private readonly IMapper _mapper;
        public VillaAPIController(ILogger<VillaAPIController> logger, /*ApplicationDbContext context,*/ IVillaRepository villaRepository, IMapper mapper)
        {
            _logger = logger;
            //_context = context;
            _villaRepository = villaRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas() 
        {
            _logger.LogInformation("Getting all villas");

            //var villas = VillaStore.villaList;

            List<Villa> villas = await _villaRepository.GetAllAsync();

            var villaMap  = _mapper.Map<List<VillaDTO>>(villas); /*Auto Mapper*/


            return Ok(villaMap);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        //[HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(200, Type = typeof(VillaDTO))]  // We can definte Response type herdcoded.
        //[ProducesResponseType(400)]
        //[ProducesResponseType(404)]
        public async Task<ActionResult> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Get Villa Error with Id " + id);
                
                return BadRequest();
            }
            //VillaDTO villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            Villa villa = await _villaRepository.GetAsync(n => n.Id == id, tracked: false);
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
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            // Explicitly Check the model state when we not use [ApiController] attribute. 
            // when we add [ApiController] attribute then Data validation check ApiController before Model State. So we use [ApiController] attribute then no need to check model state.
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            // For List
            //if (VillaStore.villaList.FirstOrDefault(x => x.Name.ToLower() == villaDTO.Name.ToLower()) != null ) //Custom Vaidation
            //{
            //    ModelState.AddModelError("CustomError", "Villa Already Exists!");
            //    return BadRequest(ModelState);
            //}

            if (await _villaRepository.GetAsync(x => x.Name.ToLower() == createDTO.Name.ToLower()) != null) //Custom Vaidation
            {
                ModelState.AddModelError("CustomError", "Villa Already Exists!");
                return BadRequest(ModelState);
            }

            if (createDTO == null) 
            { 
                return BadRequest(createDTO); 
            }
            //if (villaDTO.Id > 0)
            //{
            //    return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            //villaDTO.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDTO);

            Villa model = _mapper.Map<Villa>(createDTO); /*Auto Mapper*/

            //Villa model = new()
            //{
            //    Name = createDTO.Name,
            //    Details = createDTO.Details,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft,
            //    ImageUrl = createDTO.ImageUrl,
            //    Amenity = createDTO.Amenity
            //};
            await _villaRepository.CreateAsync(model);

            //await _context.Villas.AddAsync(model);
            //await _context.SaveChangesAsync();

            //return Ok(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);   // call  GetVilla(int id) api
        }
        [HttpDelete("{id:int}", Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            //VillaDTO villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);

            Villa villa = await _villaRepository.GetAsync(x => x.Id == id, tracked:false);
            if (villa == null)
            {
                return NotFound(villa);
            }
            //VillaStore.villaList.Remove(villa);

            //_context.Villas.Remove(villa);
            //await _context.SaveChangesAsync();
            await _villaRepository.RemoveAsync(villa);

            return NoContent();
        }
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }
            //VillaDTO villa = VillaStore.villaList.FirstOrDefault(n => n.Id == id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;

            Villa model = _mapper.Map<Villa>(updateDTO); /*Auto Mapper*/

            //Villa model = new()
            //{
            //    Id = updateDTO.Id,
            //    Name = updateDTO.Name,
            //    Details = updateDTO.Details,
            //    Occupancy = updateDTO.Occupancy,
            //    Rate = updateDTO.Rate,
            //    Sqft = updateDTO.Sqft,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Amenity = updateDTO.Amenity
            //};
            //_context.Villas.Update(model);
            //await _context.SaveChangesAsync();

            await _villaRepository.UpdateAsync(model);

            return NoContent();
        }

        // Patch supproted for 2 Packages. there are  Microsoft.AspNetCore.JsonPatch, Microsoft.AspNetCore.Mvc.NewtonsoftJson
        // For Replace: { "op": "replace", "path": "/biscuits/0/name", "value": "Chocolate Digestive" }
        // Source Link: https://jsonpatch.com/
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]  // We can definte Response type without herdcoded.
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(n => n.Id == id);

            Villa villa = await _villaRepository.GetAsync(n => n.Id == id, tracked: false);
            if (villa == null)
            {
                return BadRequest();
            }

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa); /*Auto Mapper*/

            //VillaUpdateDTO villaDTO = new()
            //{
            //    Id = villa.Id,
            //    Name = villa.Name,
            //    Details = villa.Details,
            //    Occupancy = villa.Occupancy,
            //    Rate = villa.Rate,
            //    Sqft = villa.Sqft,
            //    ImageUrl = villa.ImageUrl,
            //    Amenity = villa.Amenity
            //};

            //patchDTO.ApplyTo(villa, ModelState);

            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO); /*Auto Mapper*/

            //Villa model = new()
            //{
            //    Id = villaDTO.Id,
            //    Name = villaDTO.Name,
            //    Details = villaDTO.Details,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Amenity = villaDTO.Amenity
            //};
            //_context.Villas.Update(model);
            //await _context.SaveChangesAsync();

            await _villaRepository.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}

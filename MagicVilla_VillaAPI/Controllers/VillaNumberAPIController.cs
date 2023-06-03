using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNumberRepository _villaNumberRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly ILogger<VillaNumberAPIController> _logger;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository, IMapper mapper, ILogger<VillaNumberAPIController> logger, IVillaRepository villaRepository)
        {
            _villaNumberRepository = villaNumberRepository;
            _mapper = mapper;
            _logger = logger;
            _response = new();
            _villaRepository = villaRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumbers()
        {
            try
            {
                _logger.LogInformation("Getting all villas");

                List<VillaNumber> villas = await _villaNumberRepository.GetAllAsync();

                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villas);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };

                return _response;
            }

        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]  
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Get Villa Number Error with Id " + id);

                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_response);
                }

                VillaNumber villaNum = await _villaNumberRepository.GetAsync(n => n.VillaNo == id, tracked: false);
                if (villaNum == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNum);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };

                return _response;
            }

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createNumDTO)
        {
            try
            {
                if (await _villaNumberRepository.GetAsync(x => x.SpecialDetails.ToLower() == createNumDTO.SpecialDetails.ToLower()) != null)
                {
                    ModelState.AddModelError("CustomError", "Villa number Already Exists!");

                    _response.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(ModelState);
                }

                if (await _villaRepository.GetAsync(n => n.Id == createNumDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomeError", "Villa Id is invalid!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                if (createNumDTO == null)
                {
                    return BadRequest(createNumDTO);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(createNumDTO);

                await _villaNumberRepository.CreateAsync(model);

                _response.Result = _mapper.Map<VillaNumberDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVillaNumber", new { id = model.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };

                return _response;
            }

        }
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                VillaNumber villaNum = await _villaNumberRepository.GetAsync(x => x.VillaNo == id, tracked: false);
                if (villaNum == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _villaNumberRepository.RemoveAsync(villaNum);

                _response.Result = _mapper.Map<VillaNumberDTO>(villaNum);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };

                return _response;
            }
        }
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateNumDTO)
        {
            try
            {
                if (updateNumDTO == null || id != updateNumDTO.VillaNo)
                {
                    return BadRequest();
                }
                if (await _villaRepository.GetAsync(n => n.Id == updateNumDTO.VillaID) == null)
                {
                    ModelState.AddModelError("CustomeError", "Villa Id is invalid!");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                VillaNumber model = _mapper.Map<VillaNumber>(updateNumDTO);

                await _villaNumberRepository.UpdateAsync(model);

                _response.Result = _mapper.Map<VillaNumberDTO>(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };

                return _response;
            }
        }
    }
}

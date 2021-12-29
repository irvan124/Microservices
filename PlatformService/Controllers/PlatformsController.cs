using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private IMapper _mapper;
        private ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository,
        IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms(){
            Console.WriteLine("--> Getting Platforms .....");
            var platformItem = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }
        [HttpGet("{id}", Name="GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id) {
            var platformItem = _repository.GetPlatformById(id);
            if(platformItem != null) {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound();
        }
        [HttpPost]
        public async Task <ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto) {
            // Mapping data Model Platform berdasarkan isi dari platform Create Dto
            var platformModel = _mapper.Map<Platform>(platformCreateDto );
            // Fungsi penambahan / POST
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();


            // Mapping platfromReadDto nya berdasarkan result / platformModel di atas
            var platformReadDto = _mapper.Map<PlatformReadDto> (platformModel);
            try
            {
                 await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                
                 Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            //Nambahin di route ini
            return CreatedAtRoute(nameof(GetPlatformById),
            // Buat object baru pada koleksi di route di atas
            new {Id=platformReadDto.Id}, platformReadDto);
        }
    }
}
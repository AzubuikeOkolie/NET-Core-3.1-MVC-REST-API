using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Commander.Models;
using Commander.Data;
using Commander.Dtos;
using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Commander.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _repository;
        private readonly IMapper _Mapper;
        public CommandsController (ICommanderRepo repository, IMapper autoMapper)
	{
            _repository = repository;
            _Mapper = autoMapper;
	}

      //  private readonly MockCommanderRepo _repository = new MockCommanderRepo();
        [HttpGet]
        public ActionResult <IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();

            return Ok(_Mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        //Get api/commands/{id}  
        [HttpGet("{id}", Name ="GetCommandById")]
        public ActionResult <CommandReadDto> GetCommandById(int id)
        {
             var commandItem = _repository.GetCommandById(id);
            if (commandItem != null)
	            {
                return Ok(_Mapper.Map<CommandReadDto>(commandItem));
	            }
           return NotFound();
        }

        //POST api/Commands
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _Mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();

            var commandReadDto = _Mapper.Map<CommandReadDto>(commandModel);
            return CreatedAtRoute(nameof(GetCommandById), new  {Id = commandReadDto.Id}, commandReadDto);
            //return Ok(commandModel);
        }

        //PUT api/commands/{id}
        [HttpPut ("{id}")]

        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
	            {
                return NotFound();
	            }
            _Mapper.Map(commandUpdateDto, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }

        //PATCH api/commands/{id}
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> pacthDoc)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
	            {
                return NotFound();
	            }

            var commandToPatch = _Mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            pacthDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
	            {
                return ValidationProblem(ModelState);
	            }
            _Mapper.Map(commandToPatch, commandModelFromRepo);
            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        //DELETE   
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
             var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
	            {
                return NotFound();
	            }
            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }

    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noteApp.backend.Data;
using noteApp.backend.Dtos;
using noteApp.backend.Helpers;
using noteApp.backend.Models;

namespace noteApp.backend.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class NoteController : Controller
    {
        private readonly JwtServices _jwtServices;
        private readonly INoteRepository _noteRepository;
        private readonly CohereServices _cohereServices;
        public NoteController(JwtServices jwtServices, INoteRepository noteRepository, CohereServices cohereServices)
        {
            _jwtServices = jwtServices;
            _noteRepository = noteRepository;
            _cohereServices = cohereServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(NoteDto dto)
        {
            var jwt = Request.Cookies["jwt"];
            var token = _jwtServices.Verify(jwt);
            Guid id = Guid.Parse(token.FindFirst("Id").Value);           

            string analyze = dto.Title + " " + dto.Content;
            CohereServices.CohereApiResponse test = await _cohereServices.GetChatResponseAsync(analyze);
            string tag = test.classifications[0].prediction;

            Note note = new() { Title = dto.Title, Content = dto.Content, UserId = id, Tag = tag };

            _noteRepository.Create(note);
            return Ok("created");
        }

        [HttpGet]
        public IActionResult GetNotes()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                if (jwt == null) return Unauthorized();
                var token = _jwtServices.Verify(jwt);
                Guid id = Guid.Parse(token.FindFirst("Id").Value);

                ICollection<Note>? notes = _noteRepository.GetByUserId(id);

                return Ok(notes);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex);
            }

        }

        [HttpPut]
        public async Task<IActionResult> UpdateNote(NoteDto dto)
        {
            var jwt = Request.Cookies["jwt"];
            var token = _jwtServices.Verify(jwt);
            Guid id = Guid.Parse(token.FindFirst("Id").Value);

            Note noteToUpdate = _noteRepository.GetById(dto.Id);

            if (noteToUpdate == null) return BadRequest();
            if (noteToUpdate.UserId != id) return Unauthorized();

            string analyze = dto.Title + " " + dto.Content;
            CohereServices.CohereApiResponse test = await _cohereServices.GetChatResponseAsync(analyze);
            string tag = test.classifications[0].prediction;

            _noteRepository.Update(noteToUpdate.Id, dto.Title, dto.Content, tag);

            return Ok("updated");
        }

        [HttpDelete("/note/{id}")]
        public IActionResult DeleteNote(int id)
        {
            var jwt = Request.Cookies["jwt"];
            var token = _jwtServices.Verify(jwt);
            Guid Id = Guid.Parse(token.FindFirst("Id").Value);

            Note? noteToDelete = _noteRepository.GetById(id);

            if (noteToDelete == null) return BadRequest();
            if (noteToDelete.UserId != Id) return Unauthorized();

            _noteRepository.Delete(noteToDelete.Id);


            return Ok("deleted");
        }

    }
}

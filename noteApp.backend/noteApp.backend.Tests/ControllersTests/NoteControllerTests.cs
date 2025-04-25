using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using noteApp.backend.Controllers;
using noteApp.backend.Data;
using noteApp.backend.Helpers;
using noteApp.backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using noteApp.backend.Dtos;
using static Azure.Core.HttpHeader;

namespace noteApp.backend.Tests.ControllersTests
{
    public class NoteControllerTests
    {
        private readonly JwtServices _jwtServices;
        private readonly INoteRepository _noteRepository;
        private readonly NoteController _noteController;
        private readonly CohereServices _cohereServices;

        public NoteControllerTests()
        {
            _jwtServices = A.Fake<JwtServices>();
            _noteRepository = A.Fake<INoteRepository>();
            _cohereServices = A.Fake<CohereServices>();
            _noteController = new NoteController(_jwtServices, _noteRepository, _cohereServices);
        }

        private void SetUpHttpContextWithJwt(Guid userId, string jwtToken)
        {
            var fakeHttpContext = A.Fake<HttpContext>();

            var fakeRequest = A.Fake<HttpRequest>();

            A.CallTo(() => fakeRequest.Cookies["jwt"]).Returns("simulated.jwt.token");

            A.CallTo(() => fakeHttpContext.Request).Returns(fakeRequest);

            _noteController.ControllerContext.HttpContext = fakeHttpContext;

            var claims = new List<Claim>
        {
            new Claim("Id", userId.ToString())
        };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            A.CallTo(() => _jwtServices.Verify(A<string>.Ignored)).Returns(claimsPrincipal);
        }

        [Fact]
        public void NoteController_GetNotes_ReturnNotes()
        {
            var userId = Guid.NewGuid(); 
            var notes = new List<Note>
        {
            new Note { Id = new Random().Next(1, 10), Title = "Test Note 1", Content = "Content 1" },
            new Note { Id = new Random().Next(11, 20), Title = "Test Note 2", Content = "Content 2" }
        };

            A.CallTo(() => _noteRepository.GetByUserId(userId)).Returns(notes);

            SetUpHttpContextWithJwt(userId, "simulated.jwt.token");

            var result = _noteController.GetNotes();

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(notes); 
        }

        [Fact]
        public void NoteController_CreateNote_ReturnString()
        {
            Guid userId = Guid.NewGuid();
            NoteDto dto = new() { Title = "test", Content = "test" };
            Note note = new() { Title = dto.Title, Content = dto.Content, UserId = userId };

            A.CallTo(() => _noteRepository.Create(note));
            SetUpHttpContextWithJwt(userId, "simulated.jwt.token");

            var result = _noteController.CreateNote(dto);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("created");
        }

        [Fact]
        public void NoteController_DeleteNote_ReturnString()
        {
            Guid userId = Guid.NewGuid();
            Note note = new() { Title = "test", Content = "test", UserId = userId };

            SetUpHttpContextWithJwt(userId, "simulated.jwt.token");

            A.CallTo(() => _noteRepository.GetById(note.Id)).Returns(note);

            A.CallTo(() => _noteRepository.Delete(note.Id));

            var result = _noteController.DeleteNote(note.Id);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("deleted");
        }

        [Fact]
        public void NoteController_UpdateNote_ReturnString()
        {
            Guid userId = Guid.NewGuid();
            NoteDto dtoEdit = new() { Title = "test1", Content = "test1" };
            Note note = new() { Title = "test", Content = "test", UserId = userId };

            SetUpHttpContextWithJwt(userId, "simulated.jwt.token");

            A.CallTo(() => _noteRepository.GetById(note.Id)).Returns(note);

            A.CallTo(() => _noteRepository.Update(note.Id, dtoEdit.Title, dtoEdit.Content));

            var result = _noteController.UpdateNote(dtoEdit);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be("updated");
        }

    }
}

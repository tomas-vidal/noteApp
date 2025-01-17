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

namespace noteApp.backend.Tests.ControllersTests
{
    public class NoteControllerTests
    {
        private readonly JwtServices _jwtServices;
        private readonly INoteRepository _noteRepository;
        private readonly NoteController _noteController;

        public NoteControllerTests()
        {
            _jwtServices = A.Fake<JwtServices>();
            _noteRepository = A.Fake<INoteRepository>();
            _noteController = new NoteController(_jwtServices, _noteRepository);
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

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal(notes, okResult.Value); 
        }

    }
}

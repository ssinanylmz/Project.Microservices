using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net;
using Project.Auth.Api.Attributes;
using Project.Auth.Application.Abstracs;
using Project.Shared.BaseControllers;
using Project.Shared.Dtos;
using Project.Shared.Dtos.Auth;
using Project.Shared.Models.Responses;
using Project.Shared.Models.Responses.Auth;

namespace Project.Auth.Api.Controllers.v1
{
    /// <summary>
    /// Provides user authentication related operations such as sign-up, sign-in, and log-out.
    /// </summary>
    [Authorize]
    [SecurityHeaders]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The logger used for logging messages in this controller.</param>
        /// <param name="authService">The authentication service which provides authentication operations.</param>
        public UserController(ILogger<UserController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Signs up a new user with provided credentials.
        /// </summary>
        /// <param name="credentials">User's sign-up credentials.</param>
        /// <returns>Returns a token on successful registration or error message on failure.</returns>
        [HttpPost]
        [Authorize]
        [Route("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserCredentialsDto credentials)
        {
            var token = await _authService.CreateUserWithEmailAndPasswordAsync(credentials.Email, credentials.Password);

            if (string.IsNullOrEmpty(token))
                return CreateActionResultInstance(Response<NoContent>.Fail("Sign up failed.", (int)HttpStatusCode.BadRequest));

            return CreateActionResultInstance(Response<dynamic>.Success(new ClientTokenResponse { AccessToken = token }, (int)HttpStatusCode.OK, "Register completed"));
        }

        /// <summary>
        /// Authenticates user and provides a token for signed-in user.
        /// </summary>
        /// <param name="credentials">User's sign-in credentials.</param>
        /// <returns>Returns a token on successful sign in or error message on failure.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserCredentialsDto credentials)
        {
            var token = await _authService.SignInWithEmailAndPasswordAsync(credentials.Email, credentials.Password);
            if (string.IsNullOrEmpty(token))
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("Sign in failed.", (int)HttpStatusCode.BadRequest));
            }

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Signs out the currently authenticated user.
        /// </summary>
        /// <returns>Returns success message on successful sign out.</returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("signinanonymously")]
        public async Task<IActionResult> SignInAnonymously()
        {
            var token = await _authService.SignInAnonymouslyAsync();
            return CreateActionResultInstance(Response<dynamic>.Success(new ClientTokenResponse { AccessToken = token }, (int)HttpStatusCode.OK, "Register completed"));
        }
    }
}

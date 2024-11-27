using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Security.Commands;
using ProjectAPI.Application.Security.Models;
using ProjectAPI.Application.Security.Queries;

namespace ProjectAPI.WebUI.Controllers;

[Route("api/security/Account")]
public class AccountController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationWebModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    [Route("LoginWeb")]
    public Task<AuthenticationWebModel> LoginWeb([FromBody] LoginCommand command) => _mediator.Send(command);

    [HttpPost]
    [Route("NewUserWeb")]
    public Task<bool> NewUserWeb([FromBody] NewUserWebCommand command) => _mediator.Send(command);




}
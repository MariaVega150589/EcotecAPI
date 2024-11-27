﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.Application.Project.Commands;
using ProjectAPI.Application.Security.Commands;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.WebUI.Controllers;
public class ProjectController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("Add")]
    public Task<bool> Add([FromBody] AddProjectCommand command) => _mediator.Send(command);


    [HttpPut]
    [Route("Update")]
    public Task<bool> Update([FromBody] UpdateProjectCommand command) => _mediator.Send(command);

    [HttpDelete]
    [Route("Delete")]
    public Task<bool> Delete([FromBody] DeleteProjectCommand command) => _mediator.Send(command);
}
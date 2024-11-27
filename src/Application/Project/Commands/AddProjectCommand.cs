using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Application.Security.Commands;
using ProjectAPI.Application.Security.Models;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Project.Commands;
public class AddProjectCommand : IRequest<bool>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Status { get; set; }
    public string AssignedTo { get; set; }
}
public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, bool>
{

    private readonly IApplicationDbContext _context;

    public AddProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AddProjectCommand request, CancellationToken cancellationToken)
    {
         Tbl_Project Tbl_Project = new Tbl_Project();
        
        try
        {
            _context.CT_Proyectos.AddRange(new Tbl_Project
            {
                Name = request.Name,
                Description = request.Description,
                Status = request.Status,
                AssignedTo = request.AssignedTo,
            });
          await  _context.SaveChangesAsync(cancellationToken);
            return true;

        }
        catch (Exception)
        {

            return false;
        }
    }

   }

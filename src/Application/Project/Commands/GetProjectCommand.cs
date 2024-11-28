using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Project.Commands;
public class GetProjectCommand : IRequest<List<Tbl_Project>>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Status { get; set; }
    public string? AssignedTo { get; set; }
}
public class GetProjectCommandHandler : IRequestHandler<GetProjectCommand, List<Tbl_Project>>
{

    private readonly IApplicationDbContext _context;

    public GetProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tbl_Project>> Handle(GetProjectCommand request, CancellationToken cancellationToken)
    {
        List<Tbl_Project> Project = new List<Tbl_Project>();

        try
        {
            Project = _context.CT_Proyectos.ToList();
            return Project;

        }
        catch (Exception)
        {

            return Project;
        }
    }

}

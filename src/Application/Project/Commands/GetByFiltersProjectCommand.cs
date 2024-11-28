using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Common.Models;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Project.Commands;
public class GetByFiltersProjectCommand : IRequest<PaginatedList<Tbl_Project>>
{
    public int PageSize { get; set; } = 5;
    public int PageNumber { get; set; } = 0;
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? Status { get; set; }
    public string? AssignedTo { get; set; }
}
public class GetProjectCommandHandler : IRequestHandler<GetByFiltersProjectCommand, PaginatedList<Tbl_Project>>
{

    private readonly IApplicationDbContext _context;

    public GetProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<Tbl_Project>> Handle(GetByFiltersProjectCommand request, CancellationToken cancellationToken)
    {

        try
        {
            var entities = await _context.CT_Proyectos
              .Where(x => x.Status == true
              || x.Id == (request.Id ?? x.Id)
              || x.Name == (request.Name ?? x.Name)
              || x.Description == (request.Description ?? x.Description)
              || x.AssignedTo == (request.AssignedTo ?? x.AssignedTo)  
              ).ToListAsync(cancellationToken);

            int count = entities.Count();

            var ResponseModel = entities.Select(entity => new Tbl_Project
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status,
                AssignedTo = entity.AssignedTo, 
            })
            .Skip(request.PageSize * request.PageNumber)
            .Take(request.PageSize)
            .ToList();

            PaginatedList<Tbl_Project> res = new(ResponseModel, count, request.PageNumber, request.PageSize);
            return res;

        }
        catch (Exception)
        {

            return null;
        }
    }

}

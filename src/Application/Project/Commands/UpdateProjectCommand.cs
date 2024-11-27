using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Project.Commands;
public class UpdateProjectCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Status { get; set; }
    public string AssignedTo { get; set; }
}
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
{

    private readonly IApplicationDbContext _context;

    public UpdateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {

        try
        {
            var model = _context.CT_Proyectos.Select(x=>x.Id == request.Id).FirstOrDefault();
            if (model != null) {
                

                var p = _context.CT_Proyectos.Find(request.Id );

                p.Name = request.Name;
                p.Description = request.Description;
                p.Status = request.Status;
                p.AssignedTo = request.AssignedTo;
                await _context.SaveChangesAsync(cancellationToken);
                return true;

            }

            return false;
        }
        catch (Exception)
        {

            return false;
        }
    }

}

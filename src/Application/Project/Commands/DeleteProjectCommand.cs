using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Domain.Entities;

namespace ProjectAPI.Application.Project.Commands;
public class DeleteProjectCommand : IRequest<bool>
{
    public int Id { get; set; }
    
}
public class DeleteCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{

    private readonly IApplicationDbContext _context;

    public DeleteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var model = _context.CT_Proyectos.Select(x => x.Id == request.Id).FirstOrDefault();
            if (model != null)
            {
                var p = _context.CT_Proyectos.Find(request.Id);

                p.Status = false;
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

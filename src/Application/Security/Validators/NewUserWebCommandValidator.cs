using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProjectAPI.Application.Common.Interfaces;
using ProjectAPI.Application.Security.Commands;

namespace ProjectAPI.Application.Security.Validators;

public class NewUserWebCommandValidator : AbstractValidator<NewUserWebCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public NewUserWebCommandValidator(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
        RuleFor(v => v.Email)
            .NotNull().WithMessage("{PropertyName} debe contener un valor")
            .NotEmpty().WithMessage("{PropertyName} no debe estar vacío")
            .MustAsync(IsValidUsername).WithMessage("Error: No se pudo crear el usuario por que el usuario ya existe");
        RuleFor(v => v.Nombre)
            .NotNull().WithMessage("{PropertyName} debe contener un valor")
            .NotEmpty().WithMessage("{PropertyName} no debe estar vacío");
        RuleFor(v => v.Password)
               .NotNull().WithMessage("{PropertyName} debe contener un valor")
               .NotEmpty().WithMessage("{PropertyName} no debe estar vacío")
               .MustAsync(IsValidPassword).WithMessage("{PropertyName} es muy débil");
        RuleFor(v => v.Permisos)
               .NotNull().WithMessage("{PropertyName} debe contener un valor")
               .NotEmpty().WithMessage("{PropertyName} no debe estar vacío")
              .MustAsync(PermisosExist);
    }

    private async Task<bool> PermisosExist(List<int> permisos, CancellationToken cancellationToken)
    {
        List<int> nonExistingIds = permisos.Except(await _context.CT_Permisos.Select(e => e.PK_Permiso).ToListAsync()).ToList();

        return nonExistingIds.Any()
            ? throw new Common.Exceptions.ValidationException($"Permisos contiene permisos que no existen: {String.Join(",", nonExistingIds)}")
            : true;
    }

    private async Task<bool> IsValidPassword(string password, CancellationToken cancellationToken)
    {
        return await _identityService.IsValidPassword(password);
    }

    private async Task<bool> IsValidUsername(string email, CancellationToken cancellationToken)
    {
        return !(await _identityService.UsernameExist(email));
    }
}
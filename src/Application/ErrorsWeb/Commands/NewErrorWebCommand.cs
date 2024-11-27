using MediatR;
using ProjectAPI.Application.Common.Interfaces;

namespace ProjectAPI.Application.ErrorsWeb.Commands;

public class NewErrorWebCommand : IRequest<bool>
{
    public string Message { get; set; }
    public string FileName { get; set; }
    public string FunctionName { get; set; }
}

public class NewErrorWebCommandHandler : IRequestHandler<NewErrorWebCommand, bool>
{

    public NewErrorWebCommandHandler()
    {
    }

    public async Task<bool> Handle(NewErrorWebCommand request, CancellationToken cancellationToken)
    {
        //var res = new ErrorWebDto();
        string bodyError = $"{request.Message} Archivo: {request.FileName} Funcion: {request.FunctionName}";


        return true;
    }
}
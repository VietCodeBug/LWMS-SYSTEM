using LWMS.Application.Common.Interfaces;
using LWMS.Domain.Entities;
using LWMS.Domain.Enums;
using MediatR;

namespace LWMS.Application.Users.Commands.Register;

public class RegisterUserCommand : IRequest<Guid>
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Staff;
    public Guid HubId { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _uow;

    public RegisterUserCommandHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            EmployeeCode = request.EmployeeCode,
            FullName = request.FullName,
            Phone = request.Phone,
            PasswordHash = request.Password, // Demo: pass trực tiếp, real prod cần hash
            Role = request.Role,
            HubId = request.HubId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}

using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface ISessionService
{
    UserDTO GetCurrentUser();
    void SetCurrentUser(UserDTO user);
}

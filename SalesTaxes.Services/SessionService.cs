using SalesTaxes.Common.DTOs;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class SessionService : ISessionService
{
    private UserDTO? _currentUser;

    public UserDTO GetCurrentUser()
    {
        return _currentUser ?? throw new ApplicationException("Current user is not set.");
    }

    public void SetCurrentUser(UserDTO user)
    {
        _currentUser = user;
    }
}

using RegAuth.Models;
using RegAuth.Models.Entities;

namespace RegAuth.Services
{
    public interface IUserService
    {
        Task<Response> ValidateUser(string email, string password);
        Task<Response> RegisterUser(string email, string password, string name);
        Task<Response> TakeAction(string action, List<User> users);
    }
}

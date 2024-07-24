using Microsoft.EntityFrameworkCore;
using RegAuth.Data;
using System.Security.Claims;
using RegAuth.Models;
using RegAuth.Models.Entities;
using RegAuth.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace RegAuth.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response> ValidateUser(string email, string password)
        {
            var response = new Response();
            var commonService=new CommonService();
            try
            {
                var UserData = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (UserData == null)
                {
                    response.Success = false;
                    response.Message = "No Account Found!";
                }
                else if (UserData.Password != commonService.DoHashing(password))
                {
                    response.Success = false;
                    response.Message = "Wrong Credentials";
                }
                else if (UserData.IsBlocked == true)
                {
                    response.Success = false;
                    response.Message = "Your Account is blocked";
                }
                else
                {
                    UserData.LastLogin = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    response.Success = true;
                    response.Message = "User Validated!";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
        public async Task<Response> RegisterUser(string email, string password, string name)
        {
            var response = new Response();
            try
            {
                var existingUser = await _dbContext.Users
.FirstOrDefaultAsync(u => u.Email == email);

                if (existingUser != null)
                {
                    response.Success = false;
                    response.Message = "An account with this email already exists.";
                }
                else
                {
                    var commonService = new CommonService();
                    var User = new User
                    {
                        Name = name,
                        Email = email,
                        Password = commonService.DoHashing(password),
                    };
                    await _dbContext.Users.AddAsync(User);
                    await _dbContext.SaveChangesAsync();
                    response.Success = true;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
        public async Task<Response> TakeAction(string action, List<User> users)
        {
            var response = new Response();
            try
            {
                if (action == "block")
                {
                    foreach (var user in users)
                    {
                        user.IsBlocked = true;
                    }
                }
                else if (action == "unblock")
                {
                    foreach (var user in users)
                    {
                        user.IsBlocked = false;
                    }
                }
                else
                {
                    _dbContext.Users.RemoveRange(users);
                }
                response.Success = true;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}

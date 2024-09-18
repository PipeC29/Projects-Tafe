using Supermarket3.Data;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace Supermarket3.Repositories
{
    public class UserRepository
    {
        private readonly    Supermarket3DBContext _context;
        public UserRepository(Supermarket3DBContext context)
        {
            _context = context;
        }

        private User GetUserByUsername(string userName)
        {
            var user = _context.Users.Where(c => c.Username.Equals(userName)).FirstOrDefault();

            return user;
        }

        public User Authenticate(LoginDTO credentials)
        {
            // Look for an account matching the provided username.
            var userDetails = GetUserByUsername(credentials.Username);
            // If no matchin username exists, return.
            if (userDetails == null)
            {
                return null;
            }
            //Check the provided password matches the hashed password for the account.
            if (BCrypt.Net.BCrypt.EnhancedVerify(credentials.Password, userDetails.PasswordHash))
            {
                return userDetails;
            }
            return null;

        }


        // TODO - Remove the salting - as the bcrypt library generates a salt automatically
        public User CreateUser(LoginDTO loginDetails, string role)
        {

            // if the string the user provides is not in the 'Roles' enum, then use the Guest role
            // this should never happen when using a dropdown list, but could be circumvented using JS
            if (!System.Enum.IsDefined(typeof(Roles), role))
            {
                role = Roles.Client.ToString();
            }
            // Checks if there is a user with the desired userrname already and retrieves it.
            var userDetails = GetUserByUsername(loginDetails.Username);
            // Username Exists go no further.
            if (userDetails != null)
            {
                return null;
            }
            //Create new user object
            User user = new User()
            {
                Username = loginDetails.Username,
                Role = role,
                PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(loginDetails.Password)
            };
            //Save the new user to the database and return it to the caller.
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

      
    }

    public enum Roles
    {
        Admin,
        Moderator,
        Client
    }
}

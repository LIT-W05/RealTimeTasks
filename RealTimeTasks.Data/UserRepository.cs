using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeTasks.Data
{
    public class UserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user, string password)
        {
            string salt = PasswordHelper.GenerateSalt();
            string passwordHash = PasswordHelper.HashPassword(password, salt);
            user.PasswordSalt = salt;
            user.PasswordHash = passwordHash;
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }

        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isCorrectPassword = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!isCorrectPassword)
            {
                return null;
            }

            return user;
        }

        public User GetByEmail(string email)
        {
            using (var context = new RealTimeTasksDataContext(_connectionString))
            {
                return context.Users.FirstOrDefault(u => u.EmailAddress == email);
            }
        }

    }
}

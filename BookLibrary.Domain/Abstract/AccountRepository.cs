using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrary.Domain.Abstract
{
    public class AccountRepository : IAccountRepository
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["LibraryConnectionString"].ConnectionString;

        public void AddUser(string userName, string userEmail)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = "INSERT INTO Users" + "(UserName, UserEmail) VALUES(@UserName, @UserEmail)";
                    SqlParameter userParam = new SqlParameter("@UserName", userName.ToLower());
                    SqlParameter emailParam = new SqlParameter("@UserEmail", userEmail.ToLower());
                    command.Parameters.Add(userParam);
                    command.Parameters.Add(emailParam);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool IsUserExistInDB(string userName, string userEmail)
        {
            string sqlExpression = "SELECT * FROM Users WHERE UserName =@UserName and UserEmail = @UserEmail";
            bool hasRow = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlExpression, connection))
                {
                    connection.Open();
                    SqlParameter userParam = new SqlParameter("@UserName", userName.ToLower().Trim());
                    command.Parameters.Add(userParam);

                    SqlParameter emailParam = new SqlParameter("@UserEmail", userEmail.ToLower().Trim());
                    command.Parameters.Add(emailParam);
                    SqlDataReader reader = command.ExecuteReader();
                    hasRow = reader.HasRows;
                }
            }
            return hasRow;
        }
    }
}

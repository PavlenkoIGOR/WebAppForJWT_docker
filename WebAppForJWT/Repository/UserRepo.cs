

namespace WebAppForJWT.Repository;
using WebAppForJWT.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Npgsql;
using WebAppForJWT.Models;
using System.Reflection.Metadata;


public interface IUserRepo
{
    public Task AddUser(User userView);
    public Task<int> AddUserAndGetId(User userView);
    public User GetUserByEmail(string email);
    public User GetUserByPassword(string password);
    public User GetUserById(int id);
    public Task EditUser(User user);
    public Task<List<User>> GetAllUsers();
    public Task DeleteUser(int userId);
}







public class UserRepo : IUserRepo
{
    public static IConfiguration? _configuration;
    private ILogger<UserRepo> _logger;
    //string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
    public UserRepo(IConfiguration configuration, ILogger<UserRepo> logg)
    {
        _configuration = configuration;
        _logger = logg;
    }





    public async Task AddUser(User userView)
    {
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                _logger.LogInformation(ex.Message);
            }
            

            //using (var command = new NpgsqlCommand(sqlExpression, connection)) /************************************ так НЕ работает **************************/
            //{
            //    command.ExecuteNonQuery();
            //}
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (name, age, role, email, password) VALUES (@name, @age, @role, @email, @password)";
                command.Parameters.AddWithValue("@name", userView.Name);
                command.Parameters.AddWithValue("@email", userView.Email);
                command.Parameters.AddWithValue("@password", /*PasswordHash.HashPassword(*/userView.Password/*)*/);
                command.Parameters.AddWithValue("@age", userView.Age);
                command.Parameters.AddWithValue("@role", "user");
                command.ExecuteNonQuery();
            }
            connection.CloseAsync().Wait();
        }
    }
    public async Task<int> AddUserAndGetId(User userView)
    {
        int userId = default;
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                _logger.LogInformation(ex.Message + "Не удалось открыть соединение");
            }
            
            //using (var command = new NpgsqlCommand(sqlExpression, connection)) /************************************ так НЕ работает **************************/
            //{
            //    command.ExecuteNonQuery();
            //}
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (name, age, role, email, password) VALUES (@name, @age, @role, @email, @password) returning id";
                command.Parameters.AddWithValue("@name", userView.Name);
                command.Parameters.AddWithValue("@email", userView.Email);
                command.Parameters.AddWithValue("@password", /*PasswordHash.HashPassword(*/userView.Password/*)*/);
                command.Parameters.AddWithValue("@age", userView.Age);
                command.Parameters.AddWithValue("@role", "user");
                userId = (int)(await command.ExecuteScalarAsync()); // Получаем id нового тега                    
            }
            await connection.CloseAsync();
        }
        return userId;
    }
    public User GetUserByEmail(string email)
    {
        string? connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        User user = new User();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string selectUserByEmail = "select * from users where email = @email";
            using (NpgsqlCommand command = new NpgsqlCommand(selectUserByEmail, connection))
            {
                command.Parameters.AddWithValue("@email", email);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);//id
                        user.Name = reader.GetString(1);//Name
                        user.Age = reader.GetInt32(2);//Age                            
                        user.Email = reader.GetString(3);//Email
                        user.Password = reader.GetString(4);//Password
                        user.Role = reader.GetString(5);//Role
                    }
                     reader.Close();
                }
            }
            connection.Close();
        }
        return user;
    }
    public User GetUserByPassword(string password)
    {
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        User user = new User();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string selectUserByEmail = "select * from users where password = @password";
            using (NpgsqlCommand command = new NpgsqlCommand(selectUserByEmail, connection))
            {
                command.Parameters.AddWithValue("@password", password);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);//id
                        user.Name = reader.GetString(1);//Name
                        user.Age = reader.GetInt32(2);//Age
                        user.Email = reader.GetString(3);//Email
                        user.Password = reader.GetString(4);//Password
                        user.Role = reader.GetString(5);//Role
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        return user;
    }
    public User GetUserById(int id)
    {
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        User user = new User();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string selectUserByEmail = "select * from users where id = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(selectUserByEmail, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user.Id = reader.GetInt32(0);//id
                        user.Name = reader.GetString(1);//Name
                        user.Age = reader.GetInt32(2);//Age
                        user.Role = reader.GetString(5);//Role
                        user.Email = reader.GetString(3);//Email
                        user.Password = reader.GetString(4);//Password                            
                    }
                    reader.Close();
                }
            }
            connection.Close();
        }
        return user;
    }

    public async Task<List<User>> GetAllUsers()
    {

        List<User> users = new List<User>();

        string connectionString = _configuration.GetConnectionString("Bethlem");

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();
            string selectUserByEmail = "select * from users";
            using (NpgsqlCommand command = new NpgsqlCommand(selectUserByEmail, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        User user = new User();
                        user.Id = reader.GetInt32(0);//id
                        user.Name = reader.GetString(1);//Name
                        user.Age = reader.GetInt32(2);//Age
                        user.Role = reader.GetString(3);//Role
                        user.Email = reader.GetString(4);//Email
                        user.Password = reader.GetString(5);//Password 
                        users.Add(user);
                    }
                }
            }
            connection.CloseAsync().Wait();
        }
        return users;
    }

    public async Task EditUser(User user)
    {
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            /*обновление данных*/
            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"update users set name = @name, age = @age, email = @email, role = @role where id = @id";
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@age", user.Age);
                command.Parameters.AddWithValue("@role", user.Role);
                command.Parameters.AddWithValue("@id", user.Id);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public async Task DeleteUser(int userId)
    {
        string connectionString = _configuration.GetConnectionString("Bethlem"); /* = "Server=localhost;Username=postgres;Port=5432;Database=Bethlem;UserId=postgres;Password=postg1234;";*/
        User user = new();
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            /*обновление данных*/
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM users where id = @id";
                command.Parameters.AddWithValue("@id", userId);
                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}

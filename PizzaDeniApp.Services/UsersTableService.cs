using PizzaDeniApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;

namespace PizzaDeniApp.Services
{
    public class UsersTableService
    {
        private DbTransaction transaction;
        private readonly string _connectionString;
        private readonly string _providerName;
        private readonly DbProviderFactory _providerFactory;

        public UsersTableService()
        {
            /*   _conactionString = 
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\ШулякД\source\repos\PizzaDeniApp.Console\PizzaDeniApp.DataAccess\PizzaDeni.mdf; Integrated Security = True";*/
          
            
            _connectionString = /*@"Server=.\SQLExpress;AttachDbFilename=C:\MyFolder\MyDataFile.mdf;Database=Database.mdf;Trusted_Connection = Yes; " +*/ 
                ConfigurationManager.ConnectionStrings["appConnectionString"].ConnectionString;
            _providerName =
                ConfigurationManager.ConnectionStrings["appConnectionString"].ProviderName;

            _providerFactory = DbProviderFactories.GetFactory(_providerName);
        }

        public bool SelectUsers(string loginUser, string passwordUser)
        {
            var data = new List<User>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    
                    command.CommandText = $"select * from Users";

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                        int id = (int)sqlDataReader["Id"];
                        string login = sqlDataReader["Login"].ToString();
                        string password = sqlDataReader["Password"].ToString();
                        long phoneNumber = (long)sqlDataReader["PhoneNumber"];

                        data.Add(new User
                        {
                            Id = id,
                            Login = login,
                            Password = password,
                            PhoneNumber = phoneNumber
                        });

                    }
                    sqlDataReader.Close();

                    for(int i = 0; i < data.Count;i++)
                    {
                        if(loginUser == data[i].Login && passwordUser == data[i].Password)
                        {
                            return true;
                        }
                    }
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }
            return false;
        }

        //command.CommandText = $"insert into Users (Login,Password,PhoneNumber) values ('{user.Login}','{user.Password}',{user.PhoneNumber})";

        public void ShowMenuPizza()
        {
            var data = new List<MenuPizza>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"select * from MenuPizza";

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                        int id = (int)sqlDataReader["Id"];
                        string typePizza = sqlDataReader["TypePizza"].ToString();
                       // int price = sqlDataReader["Password"].ToString();
                        int price = (int)sqlDataReader["Price"];
                        //  Console.WriteLine(typePizza + "   " + price);
                        data.Add(new MenuPizza
                        {
                            Id = id,
                            TypePizza = typePizza,
                            Price = price
                        });
                    }
                    sqlDataReader.Close();

                       Console.WriteLine("Вид пиццы: Цена: ");
                   // Console.WriteLine(data[0].TypePizza + "   " + data[0].Price);

                      for (int i = 0; i < data.Count; i++)
                      {
                          Console.WriteLine(data[i].Id + ")"+data[i].TypePizza + " - " + data[i].Price);
                      }
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }
           
        }

        public void InsertUser(User user)
        {

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {

                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    command.CommandText = $"insert into Users (Login,Password,PhoneNumber) values (@login, @password,@phoneNumber)";

                    var loginParameter = command.CreateParameter();
                    loginParameter.ParameterName = "@login";
                    loginParameter.DbType = System.Data.DbType.String;
                    loginParameter.Value = user.Login;



                    var passwordParameter = command.CreateParameter();
                    passwordParameter.ParameterName = "@password";
                    passwordParameter.DbType = System.Data.DbType.String;
                    passwordParameter.Value = user.Password;


                    var phoneNumberParameter = command.CreateParameter();
                    phoneNumberParameter.ParameterName = "@phoneNumber";
                    phoneNumberParameter.DbType = System.Data.DbType.Int64;
                    phoneNumberParameter.Value = user.PhoneNumber;

                    command.Parameters.Add(passwordParameter);
                    command.Parameters.Add(loginParameter);
                    command.Parameters.Add(phoneNumberParameter);

                    command.Transaction = transaction;


                    int affectedRows = command.ExecuteNonQuery();


                    if (affectedRows < 1)
                    {
                        throw new Exception("Вставка не удалась");
                    }
                    transaction.Commit();
                   
                }
                catch (DbException exception)
                {
                    //обработать
                    if (transaction != null) transaction.Rollback();
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    transaction?.Rollback();
                    throw;
                }

            }
        }

        public void InsertPurchase(string loginUser)
        {

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {

                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    transaction = connection.BeginTransaction();

                    command.CommandText = $"insert into Purchase (idUsers,Price) values (@idUsers, @price)";

                    var idUsersParameter = command.CreateParameter();
                    idUsersParameter.ParameterName = "@idUsers";
                    idUsersParameter.DbType = System.Data.DbType.Int32;
                    idUsersParameter.Value = idUsers(loginUser);

                    
                    var priceParameter = command.CreateParameter();
                    priceParameter.ParameterName = "@price";
                    priceParameter.DbType = System.Data.DbType.Int32;
                    priceParameter.Value = CostFromBasket(loginUser);
                    
                    command.Parameters.Add(idUsersParameter);
                    command.Parameters.Add(priceParameter);
                   

                    command.Transaction = transaction;


                    int affectedRows = command.ExecuteNonQuery();


                    if (affectedRows < 1)
                    {
                        throw new Exception("Вставка не удалась");
                    }
                    transaction.Commit();
                    Console.WriteLine("Оплата произошла успешно");
                    Console.WriteLine(CostFromBasket(loginUser));
                }
                catch (DbException exception)
                {
                    //обработать
                    if (transaction != null) transaction.Rollback();
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    transaction?.Rollback();
                    throw;
                }

            }
        }

        public void DeleteBasketByIdUsers(string loginUser)
        {
            var data = new List<MenuPizza>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"delete from Basket where idUsers = {idUsers(loginUser)} ";

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                       

                    }
                    sqlDataReader.Close();
             
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }

        }

        public void UpdateUser(User user)
        {

        }



        public int idUsers(string loginUser)
        {

            var data = new List<User>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"select * from Users";

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                        int id = (int)sqlDataReader["Id"];
                        string login = sqlDataReader["Login"].ToString();
                        string password = sqlDataReader["Password"].ToString();
                        long phoneNumber = (long)sqlDataReader["PhoneNumber"];

                        data.Add(new User
                        {
                            Id = id,
                            Login = login,
                            Password = password,
                            PhoneNumber = phoneNumber
                        });

                    }
                    sqlDataReader.Close();

                    for (int i = 0; i < data.Count; i++)
                    {
                        if (loginUser == data[i].Login )
                        {
                            return data[i].Id;
                        }
                    }
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }
            return 0;
        }
        public int CostFromBasket(string loginUser)
        {
            var data = new List<MenuPizza>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"select mn.TypePizza,mn.Price from Basket b join MenuPizza mn on mn.Id = b.idMenuPizza join Users us on us.Id = b.idUsers where us.id = {idUsers(loginUser)}";


                    
                    var sqlDataReader = command.ExecuteReader();
                 

                    while (sqlDataReader.Read())
                    {
                        string typePizza = sqlDataReader["TypePizza"].ToString();
                        int price = (int)sqlDataReader["Price"];
                        data.Add(new MenuPizza
                        {
                            TypePizza = typePizza,
                            Price = price
                           
                        });

                    }
                    sqlDataReader.Close();
                    int sum = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        sum += data[i].Price;

                    }
                  
                    return sum;
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }


        }
        public bool CheckIdMenuPizza(int idMenu)
        {
            var data = new List<MenuPizza>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"select id from MenuPizza";

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                        int id = (int)sqlDataReader["Id"];

                        data.Add(new MenuPizza
                        {
                            Id = id
                        });

                    }
                    sqlDataReader.Close();

                    for (int i = 0; i < data.Count; i++)
                    {
                        if (idMenu == data[i].Id)
                        {
                            return true;
                        }
                    }
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }
            return false;
        }

        public void ShowBasket(string loginUser)
        {
            var data = new List<MenuPizza>();
            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    command.CommandText = $"select mn.TypePizza,mn.Price from Basket b join MenuPizza mn on mn.Id = b.idMenuPizza join Users us on us.Id = b.idUsers where us.id = {idUsers(loginUser)}";
                        

                    // using (var command = new SqlCommand("select * from Users", connection))
                    //{
                    var sqlDataReader = command.ExecuteReader();
                    //}

                    while (sqlDataReader.Read())
                    {
                        string typePizza = sqlDataReader["TypePizza"].ToString();
                        int price = (int)sqlDataReader["Price"];
                      //  int price = (int)sqlDataReader["Price"];

                        data.Add(new MenuPizza
                        {
                           TypePizza = typePizza,
                           Price = price
                        //   Price = price
                        });

                    }
                    sqlDataReader.Close();
                    int sum = 0;
                    for (int i = 0; i < data.Count; i++)
                    {
                        //if (idUsers(loginUser) == data[i].Id)
                        //{
                            Console.WriteLine(data[i].TypePizza + " - " + data[i].Price);
                        //}
                        sum += data[i].Price;

                    }
                    Console.WriteLine($"Общая сумма: {sum}");
                }
                catch (DbException exception)
                {
                    //обработать
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    throw;
                }
            }
           

        }
        public void InsertBasket( int choiceTypePizza, string loginUser)
        {

            using (var connection = _providerFactory.CreateConnection())
            using (var command = connection.CreateCommand())
            {
                try
                {

                    if (CheckIdMenuPizza(choiceTypePizza)) {
                    connection.ConnectionString = _connectionString;
                    connection.Open();

                    transaction = connection.BeginTransaction();
                        command.CommandText = $"insert into Basket (idMenuPizza,idUsers) values (@idMenuPizza,@idUsers)";

                        var idMenuParameter = command.CreateParameter();
                        idMenuParameter.ParameterName = "@idMenuPizza";
                        idMenuParameter.DbType = System.Data.DbType.Int64;
                        idMenuParameter.Value = choiceTypePizza;


                        var idUsersParameter = command.CreateParameter();
                        idUsersParameter.ParameterName = "@idUsers";
                        idUsersParameter.DbType = System.Data.DbType.Int64;
                        idUsersParameter.Value = idUsers(loginUser);



                        command.Parameters.Add(idUsersParameter);
                        command.Parameters.Add(idMenuParameter);


                        command.Transaction = transaction;


                        int affectedRows = command.ExecuteNonQuery();


                        if (affectedRows < 1)
                        {
                            throw new Exception("Вставка не удалась");
                        }
                        Console.WriteLine("Добавлено в корзину");
                        transaction.Commit(); }
                }
                catch (DbException exception)
                {
                    //обработать
                    if (transaction != null) transaction.Rollback();
                    throw;
                }
                catch (Exception exception)
                {
                    //обработать
                    transaction?.Rollback();
                    throw;
                }

            }
        }

    }
}

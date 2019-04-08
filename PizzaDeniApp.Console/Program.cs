using PizzaDeniApp.Models;
using PizzaDeniApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PizzaDeniApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {

            while (true)
            {
                try
                {
                    System.Console.Clear();
                    System.Console.WriteLine("1) Регистрация");
                    System.Console.WriteLine("2) Вход");
                    System.Console.WriteLine("3) Выход");
                    System.Console.WriteLine("Введите действие: ");
                    int act = int.Parse(System.Console.ReadLine());

                    if (act == 1)
                    {
                        string login;
                        string password;
                        while (true)
                        {
                            System.Console.Clear();
                            System.Console.WriteLine("Регистрация");
                            System.Console.WriteLine("Введите login:");
                            login = System.Console.ReadLine();
                            int countSpace = 0;
                            for (int i = 0; i < login.Length; i++)
                            {
                                if (login[i] == ' ')
                                {
                                    countSpace++;
                                }
                            }
                            if (countSpace == 0)
                            {
                                break;
                            }
                            else
                            {
                                System.Console.WriteLine("В логине присудствуют пробелы!");
                                System.Console.ReadLine();
                            }
                        }

                        while (true)
                        {
                            System.Console.Clear();
                            System.Console.WriteLine("Регистрация");
                            System.Console.WriteLine("Введите пароль(2-8):");
                            password = System.Console.ReadLine();
                            if (password.Length >= 2 && password.Length <= 8 && !password.Contains('@') && !password.Contains(' ') && !password.Contains('"') && !password.Contains('#') && !password.Contains('$') && !password.Contains('%') && !password.Contains('^') && !password.Contains('&') && !password.Contains('*') && !password.Contains('(') && !password.Contains(')') && !password.Contains('_') && !password.Contains('+') && !password.Contains('№') && !password.Contains(';') && !password.Contains(':') && !password.Contains('?') && !password.Contains('=') && !password.Contains('-') && !password.Contains('/') && !password.Contains('\'') && !password.Contains('|') && !password.Contains('.') && !password.Contains('`') && !password.Contains('~'))
                            {
                                break;
                            }
                            else
                            {
                                System.Console.WriteLine("Не выполняются критерии пароля");
                                System.Console.ReadLine();
                            }

                        }
                        while (true)
                        {
                            System.Console.Clear();
                            System.Console.WriteLine("Регистрация");
                            System.Console.WriteLine("Введите номер телефона без кода страны(+7): ");
                            //проверка правописания


                            //Потверждение смс
                            try
                            {
                                long phoneNumber = long.Parse(System.Console.ReadLine());
                                const string accountSid = "ACe1f57554bbd7a23b0c5881591c084982";
                                const string authToken = "fefd5eef2024eaed1ce9ca0204fb9cc9";

                                TwilioClient.Init(accountSid, authToken);
                                Random rand = new Random();
                                int number1 = rand.Next(0, 9);
                                int number2 = rand.Next(0, 9);
                                int number3 = rand.Next(0, 9);
                                int number4 = rand.Next(0, 9);
                                string code = number1.ToString() + number2.ToString() + number3.ToString() + number4.ToString();//Рандомное четырехзначное число
                                var message = MessageResource.Create(
                                    body: code,
                                    from: new Twilio.Types.PhoneNumber("+19362182702"),
                                    to: new Twilio.Types.PhoneNumber("+7" + phoneNumber.ToString())
                                    );
                                System.Console.Clear();
                                System.Console.WriteLine("Регистрация");
                                System.Console.WriteLine("Вам на номер был отправлен уникальный код! Проверьте свои сообщения и впишите свой код: ");
                                string phoneCode = System.Console.ReadLine();
                                if (code == phoneCode)
                                {
                                    //Данные после регистрации лежат в Базе Данных в таблице Users
                                    User user = new User { Login = login, Password = password, PhoneNumber = phoneNumber };
                                    UsersTableService service = new UsersTableService();
                                    service.InsertUser(user);
                                    System.Console.Clear();
                                    System.Console.WriteLine("Регистрация");
                                    System.Console.WriteLine("Вы были успешно зарегестрированы!");
                                    break;
                                }
                                else
                                {
                                    System.Console.Clear();
                                    System.Console.WriteLine("Регистрация");
                                    System.Console.WriteLine("Код введен не верно!");
                                    System.Console.ReadLine();
                                }
                            }
                            catch
                            {
                                System.Console.Clear();
                                System.Console.WriteLine("Регистрация");
                                System.Console.WriteLine("Ошибка при регистрации, неправельно введенные данные или логин уже существует!");
                                System.Console.ReadLine();
                            }

                        }

                        System.Console.ReadLine();
                    }
                    else if (act == 2)
                    {
                        while (true)
                        {
                            System.Console.Clear();
                            // System.Console.WriteLine("Вход ");
                            System.Console.WriteLine("Введите login: ");
                            string login = System.Console.ReadLine();
                            //проверка на правописание
                            System.Console.WriteLine("Введите пароль: ");
                            string password = System.Console.ReadLine();
                            //проверка на правописание


                            //Пользователь вводит логин и пароль, данные сверяются с данными из базы данных 
                            UsersTableService service = new UsersTableService();
                            // service.SelectUsers(login, password); 
                            if (service.SelectUsers(login, password))
                            {
                                System.Console.Clear();
                                System.Console.WriteLine("Добро пожаловать в PizzaDeni!");
                                System.Console.WriteLine("Для продолжения нажмите Enter");
                                System.Console.ReadLine();
                                while (true)
                                {
                                    System.Console.Clear();
                                    System.Console.WriteLine("PizzaDeni");
                                    System.Console.WriteLine("1) Меню");
                                    System.Console.WriteLine("2) Корзина");
                                    System.Console.WriteLine("3) Выход");
                                    int choice = int.Parse(System.Console.ReadLine());
                                    if (choice == 1)
                                    {

                                        System.Console.Clear();
                                        System.Console.WriteLine("Меню");
                                        service.ShowMenuPizza();

                                        System.Console.WriteLine("Введите номер пиццы для добавления в корзину: ");
                                        int choicePizza = int.Parse(System.Console.ReadLine());
                                        service.InsertBasket(choicePizza, login);// Заполнение таблицы Корзина тип пиццы и пользователь
                                        System.Console.ReadLine();

                                    }
                                    else if (choice == 2)
                                    {
                                        System.Console.Clear();
                                        System.Console.WriteLine("Корзина");
                                        service.ShowBasket(login);
                                        System.Console.WriteLine("1) Оплатить");
                                        System.Console.WriteLine("2) Назад");
                                        int choicePay = int.Parse(System.Console.ReadLine());
                                        if (choicePay == 1)
                                        {

                                            //Удаление всех данных данного пользователя с Корзины и Добавление данных в таблицу Покупка
                                            System.Console.Clear();
                                            service.InsertPurchase(login);
                                            service.DeleteBasketByIdUsers(login);
                                            System.Console.WriteLine("Для завершения нажмите Enter");
                                            System.Console.ReadLine();
                                            Environment.Exit(0);
                                        }
                                        else if (choicePay == 2)
                                        {
                                            continue;
                                        }
                                        System.Console.ReadLine();
                                    }
                                    else if (choice == 3)
                                    {
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        System.Console.Clear();

                                        System.Console.WriteLine("Такого действия нет!");
                                        System.Console.ReadLine();

                                    }
                                    
                                }
                            }
                            else
                            {
                                System.Console.WriteLine("Логин или пароль не верны");
                                System.Console.ReadLine();

                            }

                        }
                        //Работа с пицерией

                        System.Console.ReadLine();

                    }
                    else if (act == 3)
                    {
                        break;
                    }

                }
                catch
                {
                    continue;
                }


}
          
        }
    }
}

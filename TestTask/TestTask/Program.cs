using System.Data;
using System.Data.SqlClient;

namespace TestTask
{
    internal class Program
    {
        //строка з'єднання може бути переміщена в окремий файл для більшої безпеки.
        private const string connectionString = "Server=192.168.0.151;Database=CostControlDB;User Id=dbuser;Password=123123"; 
        private static SqlConnection connection = new SqlConnection(connectionString);
        private static SqlCommand? command;
        private static string? login; //ім'я користувача
        static void Main(string[] args)
        {
            string? query;
            Console.Write("Введiть своє iм'я: ");
            login = Console.ReadLine();
            Console.WriteLine("Для отримання команд введiть help");
            while (true)
            {
                try
                {
                    if(connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    query = Console.ReadLine();
                    if (query != null)
                    {
                        string[] parameters = query.Split(' ');
                        if (query.ToLower() == "exit")
                        {
                            return;
                        }
                        else if (query.ToLower() == "help")
                        {
                            ShowHelp();
                        }
                        else if (query.ToLower() == "clean")
                        {
                            DeleteData();
                        }
                        else if (parameters[0].ToLower() == "add")
                        {
                            if(parameters.Length == 4)
                            {
                                AddRow(Convert.ToDateTime(parameters[1]), parameters[2], Convert.ToInt32(parameters[3]));
                            }
                            else if (parameters.Length == 3)
                            {
                                AddRow(parameters[1], Convert.ToInt32(parameters[2]));
                            }
                            else
                            {
                                Console.WriteLine("Невiрна кiлькiсть аргументiв");
                            }
                        }
                        else if (parameters[0].ToLower() == "stat")
                        {
                            if (parameters.Length == 1)
                            {
                                ShowStat();
                            }
                            else if (parameters.Length == 2)
                            {
                                ShowStat(parameters[1]);
                            }
                            else
                            {
                                Console.WriteLine("Невiрна кiлькiсть аргументiв");
                            }
                        }
                        else if (parameters[0].ToLower() == "daystat" && parameters.Length == 2)
                        {
                            ShowDayStat(Convert.ToDateTime(parameters[1]));
                        }
                        else if (parameters[0].ToLower() == "monthstat" && parameters.Length == 3)
                        {
                            ShowMonthStat(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
                        }
                        else if (parameters[0].ToLower() == "yearstat" && parameters.Length == 2)
                        {
                            ShowYearStat(Convert.ToInt32(parameters[1]));
                        }
                        else
                        {
                            Console.WriteLine("Невiрна команда або набiр атрибутiв\nДля отримання команд введiть help");
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Сталася помилка: " + ex.Message);
                    connection.Close();
                    Console.ReadLine();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Сталася помилка: " + ex.Message);
                }
            }
        }
        //Відображення довідкової інформації
        private static void ShowHelp()
        {
            Console.WriteLine("Додавання запису: add рiк(необов'язково) категорiя сумма\n" +
                "Статистика: stat категорiя(необов'язково)\n" +
                "daystat дата\n" +
                "monthstat мiсяць рiк\n" +
                "yearstat рiк\n" +
                "Видалити данi: clean\n" +
                "Закрити додаток: exit");
        }
        private static void AddRow(string category, int cost)
        {
            try
            {
                command = new SqlCommand("insert into CostControl values (@device,NULL,@category,@cost)",connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@category", category);
                command.Parameters.AddWithValue("@cost", cost);
                command.ExecuteNonQuery();
                Console.WriteLine("Операцiя виконана успiшно");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void AddRow(DateTime date, string category, int cost)
        {
            try
            {
                command = new SqlCommand("insert into CostControl values (@device,@date,@category,@cost)",connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@date",date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@category",category);
                command.Parameters.AddWithValue("@cost",cost);
                command.ExecuteNonQuery();
                Console.WriteLine("Операцiя виконана успiшно");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void ShowStat()
        {
            try
            {
                command = new SqlCommand("Select sum(cost) from CostControl where Device = @device", connection);
                command.Parameters.AddWithValue("@device", login);
                SqlDataReader statReader = command.ExecuteReader();
                statReader.Read();
                Console.WriteLine("Витрати: " + (statReader[0].ToString() == ""? 0 : statReader[0]));
                statReader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void ShowStat(string category)
        {
            try
            {
                
                command = new SqlCommand("Select sum(cost) from CostControl where Device = @device and category = @category", connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@category", category);
                SqlDataReader statReader = command.ExecuteReader();
                statReader.Read();
                Console.WriteLine("Витрати: " + (statReader[0].ToString()==""? 0: statReader[0]));
                statReader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void ShowDayStat(DateTime date)
        {
            try
            {

                command = new SqlCommand("Select sum(cost) from CostControl where Device = @device and date = @date", connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                SqlDataReader statReader = command.ExecuteReader();
                statReader.Read();
                Console.WriteLine("Витрати: " + (statReader[0].ToString() == "" ? 0 : statReader[0]));
                statReader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void ShowMonthStat(int month, int year)
        {
            try
            {

                command = new SqlCommand("Select sum(cost) from CostControl where Device = @device and month(date) = @month and year(date) = @year", connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@month", month);
                command.Parameters.AddWithValue("@year", year);
                SqlDataReader statReader = command.ExecuteReader();
                statReader.Read();
                Console.WriteLine("Витрати: " + (statReader[0].ToString() == "" ? 0 : statReader[0]));
                statReader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void ShowYearStat(int year)
        {
            try
            {

                command = new SqlCommand("Select sum(cost) from CostControl where Device = @device and year(date) = @year", connection);
                command.Parameters.AddWithValue("@device", login);
                command.Parameters.AddWithValue("@year", year);
                SqlDataReader statReader = command.ExecuteReader();
                statReader.Read();
                Console.WriteLine("Витрати: " + (statReader[0].ToString() == "" ? 0 : statReader[0]));
                statReader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
        private static void DeleteData()
        {
            try
            {
                command = new SqlCommand("delete from CostControl where Device = @device", connection);
                command.Parameters.AddWithValue("@device", login);
                command.ExecuteNonQuery();
                Console.WriteLine("Iнформацiя видалена");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Сталася помилка:" + ex.Message);
            }
        }
    }
}
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Water
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string fileName = "user_history.txt";
            List<User> users = new List<User>();
            User? currentUser = null;
            double norma = 0;
            DateTime currentDate = DateTime.Today;

          
            try
            {
                users = User.LoadAllUsers(fileName);
                if (users.Count > 0)
                {
                    Console.WriteLine("User data loaded successfully.");
                }
                else
                {
                    Console.WriteLine("No previous user data found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
            }

            bool validInput = false;
            while (!validInput)
            {
                try
                {
                    Console.WriteLine("Enter name: ");
                    var name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new ArgumentException("Name cannot be empty.");
                    }

                    Console.WriteLine("Enter weight:");
                    var weightStr = Console.ReadLine();
                    if (weightStr == null || !double.TryParse(weightStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double weight))
                    {
                        throw new ArgumentException("Weight must be a valid number.");
                    }

                    currentUser = users.FirstOrDefault(u => u.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && u.Weight == weight);
                    if (currentUser == null)
                    {
                        Console.WriteLine("No existing data found for this user. Creating a new profile.");
                        currentUser = new User(name, weight);
                        users.Add(currentUser);
                    }
                    else
                    {
                        Console.WriteLine($"Welcome back, {currentUser.Name}!");
                    }

                    norma = weight * 0.035;
                    validInput = true;
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine($"Invalid input: {ex.Message}");
                    Console.WriteLine("Please try again.\n");
                }
            }

            Console.Clear();
            if (currentUser != null)
            {
                bool inMenu = true;
                while (inMenu)
                {
                    Console.WriteLine($"Date: {currentDate.ToShortDateString()}");
                    Console.WriteLine($"Hello {currentUser.Name}");
                    Console.WriteLine($"Gundelik Norma: {norma}");
                    Console.WriteLine($"Bugun: {currentUser.Norm}");
                    Console.WriteLine();

                    Console.WriteLine("1) Su ic");
                    Console.WriteLine("2) Tarixce (History)");
                    Console.WriteLine("3) Gunu bitir");
                    Console.WriteLine("4) Cixis");
                    Console.Write("Secim edin: ");
                    var secim = Console.ReadLine();
                    if (secim != null)
                    {
                        switch (secim)
                        {
                            case "1":
                                Console.WriteLine("Enter amount of water (liters):");
                                var amountStr = Console.ReadLine();
                                if (amountStr != null)
                                {
                                    if (double.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                                    {
                                        currentUser.DrinkWater(amount, currentDate);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid input, please enter a valid number.");
                                    }
                                }
                                Console.Clear();
                                break;
                            case "2":
                                Console.Clear();
                                Console.WriteLine("History:");
                                foreach (var entry in currentUser.History)
                                {
                                    Console.WriteLine($"{entry.Key.ToShortDateString()}: {entry.Value} liters");
                                }
                                Console.WriteLine();
                                break;
                            case "3":
                                Console.Clear();
                                if (currentUser.Norm >= norma)
                                {
                                    Console.WriteLine("Tebrikler siz gundelik meqsede catmisiz");
                                }
                                else
                                {
                                    Console.WriteLine("Cox az su icmisiz diqqetli olun");
                                }
                                Console.WriteLine();
                                currentUser.ResetNorm(currentDate);
                                currentDate = currentDate.AddDays(1);
                                break;
                            case "4":
                                inMenu = false;
                                break;
                            default:
                                Console.Clear();
                                Console.WriteLine("Wrong input!!!");
                                Console.WriteLine();
                                break;
                        }
                    }
                }
                
                try
                {
                    User.SaveAllUsers(fileName, users);
                    Console.WriteLine("User data saved.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving user data: {ex.Message}");
                }
            }
        }
    }
}

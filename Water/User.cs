using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Water
{
    public class User
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public double Norm { get; set; }
        public Dictionary<DateTime, double> History { get; private set; }

        public User(string name, double weight)
        {
            Name = name;
            Weight = weight;
            Norm = 0;
            History = new Dictionary<DateTime, double>();
        }

        public void DrinkWater(double water, DateTime date)
        {
            Norm += water;
            if (History.ContainsKey(date))
            {
                History[date] += water;
            }
            else
            {
                History[date] = water;
            }
        }

        public void ResetNorm(DateTime date)
        {
            Norm = 0;
            if (!History.ContainsKey(date))
            {
                History[date] = 0;
            }
        }

        public static void SaveAllUsers(string fileName, List<User> users)
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (var user in users)
                {
                    writer.WriteLine(user.Name);
                    writer.WriteLine(user.Weight);
                    foreach (var entry in user.History)
                    {
                        writer.WriteLine($"{entry.Key.ToString("yyyy-MM-dd")},{entry.Value}");
                    }
                    writer.WriteLine("---"); 
                }
            }
        }

        public static List<User> LoadAllUsers(string fileName)
        {
            var users = new List<User>();
            if (!File.Exists(fileName))
                return users;

            using (var reader = new StreamReader(fileName))
            {
                string? line;
                User? user = null;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "---")
                    {
                        if (user != null)
                        {
                            users.Add(user);
                            user = null;
                        }
                        continue;
                    }

                    if (user == null)
                    {
                        var name = line;
                        if (!double.TryParse(reader.ReadLine(), out double weight))
                            throw new Exception("Invalid data format.");
                        user = new User(name, weight);
                    }
                    else
                    {
                        var parts = line.Split(',');
                        if (DateTime.TryParse(parts[0], out DateTime date) && double.TryParse(parts[1], out double amount))
                        {
                            user.History[date] = amount;
                        }
                    }
                }

                if (user != null) 
                    users.Add(user);
            }

            return users;
        }
    }
}

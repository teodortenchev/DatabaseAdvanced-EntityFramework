using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem3
{
    class MinionNames
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                int id = int.Parse(Console.ReadLine());

                string findIdQuery = $"SELECT Name FROM Villains WHERE Id = @id";

                using (SqlCommand command = new SqlCommand(findIdQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    string villainName = (string)command.ExecuteScalar();

                    if (villainName == null)
                    {
                        Console.WriteLine("No such villain. Try another ID");
                        return;
                    }

                    Console.WriteLine($"Villain: {villainName}");
                }


                string findMinions = @"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                         m.Name, 
                                         m.Age
                                    FROM MinionsVillains AS mv
                                    JOIN Minions As m ON mv.MinionId = m.Id
                                   WHERE mv.VillainId = @id
                                ORDER BY m.Name";



                using (SqlCommand command = new SqlCommand(findMinions, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            long rowNumber = (long)reader[0];
                            string name = (string)reader[1];
                            int age = (int)reader[2];

                            Console.WriteLine($"{rowNumber}. {name} {age}");
                        }

                        if (!reader.HasRows)
                        {
                            Console.WriteLine("no minions");
                        }
                    }
                }
            }
        }
    }
}


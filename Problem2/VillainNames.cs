using AdoNet_Exercises;
using System;
using System.Data.SqlClient;

namespace Problem2
{
    public class VillainNames
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string sqlQuery = @"  SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                            FROM Villains AS v 
                                            JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                        GROUP BY v.Id, v.Name 
                                          HAVING COUNT(mv.VillainId) > 3 
                                        ORDER BY COUNT(mv.VillainId)";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Can also reference colums with numbers e.g. reader[0] is the same as reader["Name"]
                            string name = (string)reader["Name"];
                            int minionsCount = (int)reader["MinionsCount"];

                            Console.WriteLine($"{name} - {minionsCount}");
                        }
                    }
                }
            }
        }
    }
}

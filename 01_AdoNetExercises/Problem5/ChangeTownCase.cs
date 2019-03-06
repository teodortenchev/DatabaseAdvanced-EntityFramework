using AdoNet_Exercises;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Problem5
{
    class ChangeTownCase
    {
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string toUpperTownsSql = @"UPDATE Towns
                                              SET Name = UPPER(Name)
                                               WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

                using (SqlCommand command = new SqlCommand(toUpperTownsSql, connection))
                {
                    command.Parameters.AddWithValue("@countryName", countryName);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No town names were affected.");
                    }
                    else
                    {
                        Console.WriteLine($"{rowsAffected} town names were affected.");
                        PrintTownNames(connection, countryName);
                    }

                }
            }
        }

        private static void PrintTownNames(SqlConnection connection, string countryName)
        {
            string listNamesSQL = @"SELECT t.Name 
                                      FROM Towns as t
                                      JOIN Countries AS c ON c.Id = t.CountryCode
                                     WHERE c.Name = @countryName";

            using (SqlCommand command = new SqlCommand(listNamesSQL, connection))
            {
                command.Parameters.AddWithValue("@countryName", countryName);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    List<string> towns = new List<string>();
                    
                    while (reader.Read())
                    {
                        towns.Add((string)reader[0]);
                    }

                    Console.WriteLine($"[{String.Join(", ", towns)}]");
                }
            }

        }
    }
}

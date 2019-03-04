using AdoNet_Exercises;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Prolem7
{
    class PrintMinionNames
    {
        private static List<string> minionNames;

        static void Main(string[] args)
        {
            minionNames = new List<string>();

            GetMinionNamesFromDB();

            for (int i = 0; i < minionNames.Count / 2; i++)
            {
                Console.WriteLine(minionNames[i]);
                Console.WriteLine(minionNames[minionNames.Count - 1 - i]);
            }

            if (minionNames.Count % 2 != 0)
            {
                Console.WriteLine(minionNames[minionNames.Count / 2]);
            }
        }

        private static void GetMinionNamesFromDB()
        {
            using (SqlConnection connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                string getMinionNamesSql = @"SELECT [Name] FROM Minions";

                using (SqlCommand command = new SqlCommand(getMinionNamesSql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            minionNames.Add((string)reader["Name"]);
                        }
                    }
                }
            }
        }
    }
}

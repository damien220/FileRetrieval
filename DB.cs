using System.Data.SQLite;
using System;

namespace FileRetrieval
{
    public class DB
    {
        SQLiteConnection m_dbConnection;
        string connectionString = "Data Source=Files.sqlite;Version=3;";
        public void createNewDatabase()
        {
            SQLiteConnection.CreateFile("Files.sqlite");
            using (m_dbConnection = new SQLiteConnection(connectionString))
            {
                m_dbConnection.Open();

                string createTableQuery = "CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                                                            "Name TEXT NOT NULL, Age INTEGER NOT NULL)";
                SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, m_dbConnection);
                createTableCommand.ExecuteNonQuery();
            }
        }

        // Creates a connection with our database file.
        public void connectToDatabase()
        {
            m_dbConnection = new SQLiteConnection("Data Source=Files.sqlite;Version=3;");
            m_dbConnection.Open();
        }

        // Creates a table named 'highscores' with two columns: name (a string of max 20 characters) and score (an int)
        void createTable()
        {
            string sql = "create table highscores (name varchar(20), score int)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        // Inserts some values in the highscores table.
        // As you can see, there is quite some duplicate code here, we'll solve this in part two.
        public void fillTable()
        {
            string sql = "insert into Users (Name, Age) values ('Me', 3000)";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            sql = "insert into Users (Name, Age) values ('Myself', 6000)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
            sql = "insert into Users (Name, Age) values ('And I', 9001)";
            command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        // Writes the highscores to the console sorted on score in descending order.
        public void printHighscores()
        {
            string sql = "select * from Users order by Age desc";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                Console.WriteLine("Name: " + reader["Name"] + "\tScore: " + reader["Age"]);
            Console.ReadLine();
        }
    }
}

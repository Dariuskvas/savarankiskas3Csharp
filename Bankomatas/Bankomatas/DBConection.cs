using System.Data.SQLite;

namespace Bankomatas
{
    public class DBConection
    {
        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection sqLiteConn;
            sqLiteConn = new SQLiteConnection("Data Source=database.db; Version=3; new =false; Cpmpress =True;");
            try
            {
                sqLiteConn.Open();
            }
            catch { Console.WriteLine("Error"); }
            return sqLiteConn;
        }

        public static string ReadData(SQLiteConnection conn, string dbTable, string filterCriteria, object filterValue, string returnValue)
        {
            try
            {
                string stringForReturn = string.Empty;

                SQLiteCommand sqLiteCommand = conn.CreateCommand();
                if (filterValue.GetType().Name == "Int32")
                    sqLiteCommand.CommandText = $"SELECT * FROM {dbTable} WHERE {filterCriteria}={filterValue}";
                else
                    sqLiteCommand.CommandText = $"SELECT * FROM {dbTable} WHERE {filterCriteria}='{filterValue}'";
                SQLiteDataReader sqliteReader = sqLiteCommand.ExecuteReader();

                while (sqliteReader.Read())
                {
                    stringForReturn = sqliteReader[returnValue].ToString();
                }
                return stringForReturn;
                conn.Close();

            }
            catch
            {
                Console.WriteLine("Error: negauti duomenys");
                return null;
                conn.Close();
            }
        }

        public static void ReadData(SQLiteConnection conn, string dbTable)
        {
            try
            {
                SQLiteCommand sqLiteCommand = conn.CreateCommand();
                sqLiteCommand.CommandText = $"SELECT * FROM {dbTable} ORDER BY operationDate DESC LIMIT 5";
                SQLiteDataReader sqliteReader = sqLiteCommand.ExecuteReader();
                while (sqliteReader.Read())
                {
                    Console.WriteLine($"{sqliteReader["operationDate"]} - {sqliteReader["operationAmount"]}");
                }
                conn.Close();
            }
            catch
            {
                Console.WriteLine("Nera duomenu apie operacijas");
            }
        }

        public static string ReadData(SQLiteConnection conn, string dbTable, string empty)
        {
            string stringForReturn = string.Empty;
            try
            {
                SQLiteCommand sqLiteCommand = conn.CreateCommand();
                sqLiteCommand.CommandText = $"SELECT * FROM {dbTable} ORDER BY operationDate DESC LIMIT 1";
                SQLiteDataReader sqliteReader = sqLiteCommand.ExecuteReader();
                while (sqliteReader.Read())
                {
                    stringForReturn = sqliteReader["operationDate"].ToString();
                }
                return stringForReturn;
                conn.Close();
            }
            catch
            {
                Console.WriteLine("Nera duomenu apie operacijas");
                return "2000-12-31 23:59:59";
            }
        }

        public static void UpdateData(SQLiteConnection conn, string dbTable, string filterCriteria, object filterValue, string newCriteria, object newValue)
        {
            SQLiteCommand sqLiteCommand = conn.CreateCommand();
            sqLiteCommand.CommandText = $"UPDATE {dbTable} SET {newCriteria} = {newValue} WHERE {filterCriteria} = '{filterValue}'";
            sqLiteCommand.ExecuteNonQuery();
            conn.Close();
        }

        public static void CreateTable(SQLiteConnection conn, string tableName, string cardGuid, string dateOperationDate, string operationAmount)
        {
            SQLiteCommand sqLiteCommand;
            string createSQL = $"CREATE TABLE {tableName}({cardGuid} VARCHAR(20), {dateOperationDate} VARCHAR(20), {operationAmount} int)";
            sqLiteCommand = conn.CreateCommand();
            sqLiteCommand.CommandText = createSQL;
            sqLiteCommand.ExecuteNonQuery();
        }

        public static void InsertData(SQLiteConnection con, string tableName, string guid, string operationDate, int amountForTakeOut)
        {
            SQLiteCommand sqLiteCommand;
            sqLiteCommand = con.CreateCommand();
            sqLiteCommand.CommandText = $"INSERT INTO {tableName}(CardGuid, operationDate, operationAmount) VALUES ('{guid}', '{operationDate}', {amountForTakeOut});";
            sqLiteCommand.ExecuteNonQuery();
        }

    }
}

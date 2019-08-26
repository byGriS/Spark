using System;
using System.Data.SQLite;
using System.IO;

namespace GenerateDataToSQL {
  class Program {

    private static string dbFileName = "spark.sqlite";
    private static SQLiteConnection m_dbConn = new SQLiteConnection();
    private static SQLiteCommand m_sqlCmd = new SQLiteCommand();
    private static int countData = 10000;

    static void Main(string[] args) {
      if (!File.Exists(dbFileName))
        SQLiteConnection.CreateFile(dbFileName);
      try {
        m_dbConn = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
        m_dbConn.Open();
        m_sqlCmd.Connection = m_dbConn;
      } catch (Exception e) {
        Service.Log.LogWrite(Environment.CurrentDirectory+"\\", "Ошибка подключения к базе данных", e.ToString());
      }
      m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS 'ДавлениеТестовое|атм' (id INTEGER PRIMARY KEY AUTOINCREMENT, value real, dt text )";
      m_sqlCmd.ExecuteNonQuery();
      Random r = new Random();
      DateTime time = DateTime.Now;
      for (int i = 0; i < countData; i++) {

        m_sqlCmd.CommandText = "INSERT INTO 'ДавлениеТестовое|атм' ('value', 'dt') values ('" +
                (r.Next(0,100000)/1000.0) + "' , '" +
                time + "')";
        m_sqlCmd.ExecuteNonQuery();
        time= time.AddSeconds(1);
      }
      Console.WriteLine("Данные добавлены");
      Console.Read();
    }
  }
}

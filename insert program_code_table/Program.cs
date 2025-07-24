using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main()
    {
        // 設定連接字串，根據你提供的資訊來修改
        string connectionString = "User Id=xbosprod;Password=login123;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.63.106)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=xbos2gs)))";

        Console.Write("Add New Table Name :");
        string name = Console.ReadLine();
        name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
        Console.WriteLine($"Name: {name}");

        while (true) // 一直執行直到使用者選擇退出
        {
            // 詢問操作類型
            Console.Write("Do you want to (a)dd a record or (d)elete a record? (a/d): ");
            string action = Console.ReadLine().ToLower();

            if (action == "a") // 新增資料
            {
                // 輸入 program_code 和 table_name
                Console.Write("Enter Program Code: ");
                string programCode = Console.ReadLine()?.ToUpper(); //轉大寫

                Console.Write("Enter Table Name: ");
                string tableName = Console.ReadLine()?.ToLower();  //轉小寫

                // 輸入 remark
                Console.Write("Enter Remark: ");
                string remark = Console.ReadLine();  // 這會讓使用者輸入 remark 的內容

                // 其它固定欄位
                string createUser = name; // 固定為 name

                // 插入資料到 PROGRAM_CODE_TABLE
                using (OracleConnection connection = new OracleConnection(connectionString))  // 使用正確的 OracleConnection 類型
                {
                    connection.Open(); // 開啟連接

                    string sql = @"
                        INSERT INTO PROGRAM_CODE_TABLE (program_code, table_name, remark, create_user)
                        VALUES (:program_code, :table_name, :remark, :create_user)";  // 移除 USAGE_TYPE 欄位

                    using (OracleCommand command = new OracleCommand(sql, connection))  // 使用正確的 connection
                    {
                        // 設定參數
                        command.Parameters.Add(new OracleParameter("program_code", programCode));
                        command.Parameters.Add(new OracleParameter("table_name", tableName));
                        command.Parameters.Add(new OracleParameter("remark", remark));  // 使用者輸入的 remark
                        command.Parameters.Add(new OracleParameter("create_user", createUser));

                        try
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} row(s) inserted.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }
            else if (action == "d") // 刪除資料
            {
                // 輸入 program_code 和 table_name 來刪除
                Console.Write("Enter Program Code to delete: ");
                string programCodeToDelete = Console.ReadLine()?.ToUpper(); //轉大寫

                Console.Write("Enter Table Name to delete: ");
                string tableNameToDelete = Console.ReadLine()?.ToLower(); //轉小寫

                // 刪除資料
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open(); // 開啟連接

                    string deleteSql = @"
                        DELETE FROM PROGRAM_CODE_TABLE
                        WHERE program_code = :program_code AND table_name = :table_name";

                    using (OracleCommand deleteCommand = new OracleCommand(deleteSql, connection))
                    {
                        deleteCommand.Parameters.Add(new OracleParameter("program_code", programCodeToDelete));
                        deleteCommand.Parameters.Add(new OracleParameter("table_name", tableNameToDelete));

                        try
                        {
                            int rowsDeleted = deleteCommand.ExecuteNonQuery();
                            if (rowsDeleted > 0)
                            {
                                Console.WriteLine("Record deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("No record found with the given Program Code and Table Name.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 'a' for add or 'd' for delete.");
                continue;
            }

            // 詢問是否繼續操作
            Console.Write("Do you want to continue? (y/n): ");
            string userInput = Console.ReadLine();
            if (userInput.ToLower() != "y")
            {
                break;  // 若輸入不是 'y'，則退出迴圈
            }
        }

        Console.WriteLine("Operation finished.");
    }
}

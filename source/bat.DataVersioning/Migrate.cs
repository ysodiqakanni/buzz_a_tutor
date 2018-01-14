using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using bat.data;
using DbUp;

namespace bat.DataVersioning
{
    public class Migrate
    {
        public static void Run()
        {
            using (var conn = new dbEntities())
            {
                var connpms = conn.Database.Connection.ConnectionString.Split(";".ToCharArray());

                var connstr =
                    connpms.FirstOrDefault(c => c.StartsWith("Data Source=", StringComparison.CurrentCultureIgnoreCase)) + ";" +
                    connpms.FirstOrDefault(c => c.StartsWith("Initial Catalog=", StringComparison.CurrentCultureIgnoreCase)) + ";" +
                    connpms.FirstOrDefault(c => c.StartsWith("Persist Security Info=", StringComparison.CurrentCultureIgnoreCase)) + ";" +
                    connpms.FirstOrDefault(c => c.StartsWith("User ID=", StringComparison.CurrentCultureIgnoreCase)) + ";" +
                    connpms.FirstOrDefault(c => c.StartsWith("Password=", StringComparison.CurrentCultureIgnoreCase));

                var upgrader = DeployChanges.To
                    .SqlDatabase(connstr)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

                var result = upgrader.PerformUpgrade();

                if (!result.Successful)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(result.Error);
                    Console.ResetColor();
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success!");
                Console.ResetColor();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Node.ModelLibrary.Identity;   
using System.IO;

namespace Node.ModelLibrary.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private static string GetDbPath()
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Node");

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "node.db");
        }

        private static string ConnectionString => $"Data Source={GetDbPath()}";

        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(ConnectionString);
            return new AppDbContext(optionsBuilder.Options);
        }

        public static AppDbContext Create()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite(ConnectionString);
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
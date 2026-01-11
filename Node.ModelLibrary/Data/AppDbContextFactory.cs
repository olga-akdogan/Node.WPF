using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Node.ModelLibrary.Identity;   

namespace Node.ModelLibrary.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

          
            optionsBuilder.UseSqlite("Data Source=node.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
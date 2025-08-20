using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallet.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var connStr = "Server=(localdb)\\mssqllocaldb;Database=WalletTaskDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            optionsBuilder.UseSqlServer(connStr);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

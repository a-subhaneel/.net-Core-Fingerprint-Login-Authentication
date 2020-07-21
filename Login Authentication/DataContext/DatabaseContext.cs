using Microsoft.EntityFrameworkCore;
using net_core_fingerprint_authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net_core_fingerprint_authorization.DataContext
{
    public class Databasecontext : DbContext
    {
        public DbSet<FingerprintModel> fingerPrintModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DESKTOP-N9I077O\SQLEXPRESS;Initial Catalog=FingerPrintDataBase; Integrated Security=True");
        }
    }
}

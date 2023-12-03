using CSharpWebApiSample.AppConfiguration;
using CSharpWebApiSample.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data.Common;

namespace CSharpWebApiSample.Domain
{
    public class PersonsContext : DbContext
    {
        private string connectionString;

        public DbSet<Person> Persons { get; set; }

        public PersonsContext(IOptions<DbConfiguration> options)
        {
            connectionString = options.Value.ConnectionString;
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasKey(p => p.Id);
        }
    }
}

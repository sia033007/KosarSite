using KosarSite.Models;
using Microsoft.EntityFrameworkCore;

namespace KosarSite.Data
{
    public class ApplicationDb : DbContext
    {
        public ApplicationDb(DbContextOptions<ApplicationDb> options): base(options)
        {

        }
        public DbSet<PersonModel> PersonModels { get; set; }
    }
}

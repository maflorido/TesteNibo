using NiboTest.Web.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace NiboTest.Web.Data
{
    public class Context : DbContext
    {
        public Context() : base("NiboContext")
        {            
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
            Database.SetInitializer<Context>(new DropCreateDatabaseAlways<Context>());
        }

        public DbSet<ExtratoBanco> ExtratoBanco { get; set; }

        public DbSet<Transacao> Transacao { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<ExtratoBanco>();

            modelBuilder.Entity<ExtratoBanco>().HasMany(x => x.TransacoesExtrato).WithRequired();

            modelBuilder.Entity<Transacao>().HasKey(x => new { x.CodigoBanco, x.CodigoConta, x.DataTransacao, x.TipoTransacao, x.ValorTransacao, x.NumeroCheck });
        }
    }
}
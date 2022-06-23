using Data.Model;
using Data.Model.Base;
using Data.Views;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Id użytkownika, operującego na kontekście
        /// </summary>
        public string? UserId
        {
            get;
            set;
        }

        public DbContextOptions<DatabaseContext> Options { get; set; }


        private readonly DbContextOptions<DatabaseContext> _options;
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            _options = options;
            Options = options;
        }

        public DbSet<User> User { get; set; }
        public DbSet<Page> Page { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<BoughtBooks> BoughtBooks { get; set; }
        public DbSet<Opinion> Opinion { get; set; }
        public DbSet<Visit> Visit { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }


        //VIEWS
        public DbSet<SoldBooksPerDay> SoldBooksPerDay { get; set; }
        public DbSet<BestSoldBooks> BestSoldBooks { get; set; }
        public DbSet<SoldByCategory> SoldByCategory { get; set; }
        public DbSet<BestReaders> BestReaders { get; set; }


        /// <summary>
        /// Zwraca wszystkie rekordy przekazanego typu, które są aktywne
        /// </summary>
        public IQueryable<T> AllActive<T>() where T : BaseEntity => new DatabaseContext(this._options).Set<T>().Where(row => !row.IsLocked).AsNoTracking();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Book>()
                .HasOne<Category>(row => row.Category)
                .WithMany(row => row.Books)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Book>()
                .HasMany(row => row.Opinions)
                .WithOne(row => row.OpiniedBook)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Comment>()
               .HasOne(row => row.Post)
               .WithMany(row => row.Comments)
                .OnDelete(DeleteBehavior.Restrict);



            //VIEWS

            builder.Entity<SoldBooksPerDay>()
              .HasNoKey()
              .ToView("V_SalePerDay");

            builder.Entity<BestSoldBooks>()
            .HasNoKey()
            .ToView("V_BestSoldBooks");

            builder.Entity<SoldByCategory>()
            .HasNoKey()
            .ToView("V_SalesPerCatrgory");

            builder.Entity<BestReaders>()
            .HasNoKey()
            .ToView("V_BestReaders");
        }

        /// <summary>
        /// Przy dodaniu, z automatu wypłnij datę i kto utworzył
        /// Przy edycji, datę edytowanie i kto edytował
        /// </summary>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //Weź rekordy zmodyfikowane i dodane
            this.ChangeTracker.DetectChanges();
            var addedOrModified = this.ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Added || t.State == EntityState.Modified)
                        .Select(t => new { Entity = t.Entity, State = t.State })
                        .ToArray();

            foreach (var anon in addedOrModified)
            {
                if (anon.State == EntityState.Added)
                {


                    if (anon.Entity is TimeStampEntity)
                    {
                        var track = anon.Entity as TimeStampEntity;
                        if (track != null)
                            track.CreateDate = DateTime.Now;
                    }
                    if (anon.Entity is FingerPrintEntity)
                    {
                        if (this.UserId is null)
                        {
                            throw new NullReferenceException("Użytkownik tworzący rekord nie może być nieznany");
                        }
                        var track = anon.Entity as FingerPrintEntity;
                        if (track != null)
                            track.CreateById = this.UserId;
                    }
                }

                if (anon.State == EntityState.Modified)
                {
                    if (anon.Entity is TimeStampEntity)
                    {
                        var track = anon.Entity as TimeStampEntity;
                        if (track != null)
                            track.EditDate = DateTime.Now;
                    }

                    if (anon.Entity is FingerPrintEntity)
                    {
                        if (this.UserId is null)
                        {
                            throw new NullReferenceException("Użytkownik edytujący rekord nie może być nieznany");
                        }
                        var track = anon.Entity as FingerPrintEntity;
                        if (track != null)
                            track.EditById = this.UserId;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            //Weź rekordy zmodyfikowane i dodane
            this.ChangeTracker.DetectChanges();
            var addedOrModified = this.ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Added || t.State == EntityState.Modified)
                        .Select(t => new { Entity = t.Entity, State = t.State })
                        .ToArray();

            foreach (var anon in addedOrModified)
            {
                if (anon.State == EntityState.Added)
                {


                    if (anon.Entity is TimeStampEntity)
                    {
                        var track = anon.Entity as TimeStampEntity;
                        if (track != null)
                            track.CreateDate = DateTime.Now;
                    }
                    if (anon.Entity is FingerPrintEntity)
                    {
                        if (this.UserId is null)
                        {
                            throw new NullReferenceException("Użytkownik tworzący rekord nie może być nieznany");
                        }
                        var track = anon.Entity as FingerPrintEntity;
                        if (track != null)
                            track.EditById = this.UserId;
                    }
                }

                if (anon.State == EntityState.Modified)
                {
                    if (anon.Entity is TimeStampEntity)
                    {
                        var track = anon.Entity as TimeStampEntity;
                        if (track != null)
                            track.EditDate = DateTime.Now;
                    }

                    if (anon.Entity is FingerPrintEntity)
                    {
                        if (this.UserId is null)
                        {
                            throw new NullReferenceException("Użytkownik edytujący rekord nie może być nieznany");
                        }
                        var track = anon.Entity as FingerPrintEntity;
                        if (track != null)
                            track.EditById = this.UserId;
                    }
                }
            }
            return base.SaveChanges();
        }
    }
}
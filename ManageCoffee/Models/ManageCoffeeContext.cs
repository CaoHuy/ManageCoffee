using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ManageCoffee.Models
{
    public partial class ManageCoffeeContext : DbContext
    {
        public ManageCoffeeContext()
        {
        }

        public ManageCoffeeContext(DbContextOptions<ManageCoffeeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Catalogue> Catalogues { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Detail> Details { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Table> Tables { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost;Database=ManageCoffee;uid=sa;pwd=12345678;encrypt=true;trustServerCertificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.SoftDelete)
                    .HasColumnType("datetime")
                    .HasColumnName("softDelete");
            });

            modelBuilder.Entity<Catalogue>(entity =>
            {
                entity.ToTable("Catalogue");

                entity.Property(e => e.CatalogueId).HasColumnName("catalogue_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.ToTable("Config");

                entity.Property(e => e.ConfigId).HasColumnName("config_id");

                entity.Property(e => e.Key)
                    .HasMaxLength(255)
                    .HasColumnName("key");

                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<Detail>(entity =>
            {
                entity.ToTable("Detail");

                entity.Property(e => e.DetailId).HasColumnName("detail_id");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Detail__order_id__47DBAE45");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Details)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Detail__product___48CFD27E");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Log");

                entity.Property(e => e.LogId).HasColumnName("log_id");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("action");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Object)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("object");

                entity.Property(e => e.ObjectId).HasColumnName("object_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Logs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Log__user_id__4CA06362");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Note)
                    .HasMaxLength(100)
                    .HasColumnName("note");

                entity.Property(e => e.Revision).HasColumnName("revision");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("money")
                    .HasColumnName("total_price");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("FK__Order__table_id__44FF419A");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__user_id__440B1D61");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.CatalogueId).HasColumnName("catalogue_id");

                entity.Property(e => e.Image)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("image");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("price");

                entity.Property(e => e.SoftDelete)
                    .HasColumnType("datetime")
                    .HasColumnName("softDelete");

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("unit");

                entity.HasOne(d => d.Catalogue)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CatalogueId)
                    .HasConstraintName("FK__Product__softDel__403A8C7D");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.ToTable("Table");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.AreaId).HasColumnName("area_id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Area)
                    .WithMany(p => p.Tables)
                    .HasForeignKey(d => d.AreaId)
                    .HasConstraintName("FK__Table__area_id__3D5E1FD2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.Role).HasColumnName("role");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

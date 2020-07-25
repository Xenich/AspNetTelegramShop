using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TelegramShopAsp.DB
{
    public partial class DB_A566BC_xenichContext : DbContext
    {
        public DB_A566BC_xenichContext()
        {
        }

        public DB_A566BC_xenichContext(DbContextOptions<DB_A566BC_xenichContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Buyers> Buyers { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartGoods> CartGoods { get; set; }
        public virtual DbSet<Cities> Cities { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<Goods> Goods { get; set; }
        public virtual DbSet<SellerGoods> SellerGoods { get; set; }
        public virtual DbSet<Sellers> Sellers { get; set; }
        public virtual DbSet<States> States { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<Log> Log { get; set; }

        // отдельные сеты
        //public DbSet<DB.prOutputs.CartGoodFromStoredPr> CartGoodFromStoredPr { get; set; }

        // Unable to generate entity type for table 'dbo.test'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=SQL5052.site4now.net;Database=DB_A566BC_xenich;User ID=DB_A566BC_xenich_admin; Password=3132304771z;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buyers>(entity =>
            {
                entity.HasKey(e => e.TelegramId);

                entity.Property(e => e.TelegramId).ValueGeneratedNever();

                entity.Property(e => e.adress)
                    .HasColumnName("adress")
                    .HasMaxLength(255);

                entity.Property(e => e.ChatId).HasColumnName("chatId");

                entity.Property(e => e.CityUID)
                    .HasColumnName("CityUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.currentItemEdit)
                    .HasColumnName("currentItemEdit")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.isActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20);

                entity.Property(e => e.PreviousMessage).HasColumnName("previousMessage");

                entity.Property(e => e.Region).HasColumnName("region");

                entity.Property(e => e.StateUID)
                    .IsRequired()
                    .HasColumnName("StateUID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.CityU)
                    .WithMany(p => p.Buyers)
                    .HasForeignKey(d => d.CityUID)
                    .HasConstraintName("FK_Users_Cities");
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Uid);

                entity.Property(e => e.Uid)
                    .HasColumnName("UID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.UserTelegram)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.UserTelegramId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_Users");
            });

            modelBuilder.Entity<CartGoods>(entity =>
            {
                entity.HasIndex(e => new { e.CartUid, e.GoodUid })
                    .HasName("IX_CartGoods_1")
                    .IsUnique();

                entity.Property(e => e.CartUid)
                    .IsRequired()
                    .HasColumnName("CartUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.GoodUid)
                    .IsRequired()
                    .HasColumnName("GoodUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Quantity).HasMaxLength(20);

                entity.HasOne(d => d.CartU)
                    .WithMany(p => p.CartGoods)
                    .HasForeignKey(d => d.CartUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartGoods_Cart");

                entity.HasOne(d => d.GoodU)
                    .WithMany(p => p.CartGoods)
                    .HasForeignKey(d => d.GoodUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CartGoods_Goods");
            });

            modelBuilder.Entity<Cities>(entity =>
            {
                entity.HasKey(e => e.UID);

                entity.Property(e => e.UID)
                    .HasColumnName("UID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CountryUID)
                    .IsRequired()
                    .HasColumnName("CountryUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.CountryU)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.CountryUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cities_Countries");
            });

            modelBuilder.Entity<Countries>(entity =>
            {
                entity.HasKey(e => e.UID);

                entity.Property(e => e.UID)
                    .HasColumnName("UID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Goods>(entity =>
            {
                entity.HasKey(e => e.UID);

                entity.HasIndex(e => e.Name)
                    .HasName("IX_Goods")
                    .IsUnique();

                entity.Property(e => e.UID)
                    .HasColumnName("UID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ParentUID)
                    .IsRequired()
                    .HasColumnName("ParentUID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.Goods)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK_Goods_Unit");
            });

            modelBuilder.Entity<SellerGoods>(entity =>
            {
                entity.HasKey(e => e.GoodUID);

                entity.HasIndex(e => new { e.GoodUID, e.SellerTelegramId })
                    .HasName("IX_SellerGoods")
                    .IsUnique();

                entity.Property(e => e.GoodUID)
                    .HasColumnName("GoodUID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.BasicGoodUID)
                    .IsRequired()
                    .HasColumnName("BasicGoodUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.isActive)
                    .IsRequired()
                    .HasColumnName("isActive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.MainPhotoUID)
                    .HasColumnName("MainPhotoUID")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ShopName).HasMaxLength(255);

                entity.HasOne(d => d.BasicGoodU)
                    .WithMany(p => p.SellerGoods)
                    .HasForeignKey(d => d.BasicGoodUID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SellerGoods_Goods");

                entity.HasOne(d => d.SellerTelegram)
                    .WithMany(p => p.SellerGoods)
                    .HasForeignKey(d => d.SellerTelegramId)
                    .HasConstraintName("FK_SellerGoods_Sellers");
            });

            modelBuilder.Entity<Sellers>(entity =>
            {
                entity.HasKey(e => e.TelegramId);

                entity.Property(e => e.TelegramId).ValueGeneratedNever();

                entity.Property(e => e.Adress).HasMaxLength(255);

                entity.Property(e => e.CityUID)
                    .HasColumnName("CityUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentItemEdit)
                    .HasColumnName("currentItemEdit")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.CurrentMessage).HasColumnName("currentMessage");

                entity.Property(e => e.Description).HasMaxLength(4000);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.PreviousMessage).HasColumnName("previousMessage");

                entity.Property(e => e.Region).HasColumnName("region");

                entity.Property(e => e.StateUID)
                    .IsRequired()
                    .HasColumnName("StateUID")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.HasOne(d => d.CityU)
                    .WithMany(p => p.Sellers)
                    .HasForeignKey(d => d.CityUID)
                    .HasConstraintName("FK_Sellers_Cities");
            });

            modelBuilder.Entity<States>(entity =>
            {
                entity.HasKey(e => e.UID);

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_States")
                    .IsUnique();

                entity.Property(e => e.UID)
                    .HasColumnName("UID")
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Log>(entity =>
            {

            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Name234)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Name5)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ShortName)
                    .IsRequired()
                    .HasMaxLength(10);
            });
        }
    }
}

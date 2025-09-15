using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectData.Models;

using System;
using System.Collections.Generic;

namespace ProjectData.Data;

public partial class XiangYunDbContext : DbContext
{
    public XiangYunDbContext(DbContextOptions<XiangYunDbContext> options)
        : base(options)
    {
    }

   
    public virtual DbSet<Category> Category { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<InventoryMovement> InventoryMovement { get; set; }

    public virtual DbSet<Member> Member { get; set; }

    public virtual DbSet<ModelSpec> ModelSpec { get; set; }

    public virtual DbSet<Order> Order { get; set; }

    public virtual DbSet<OrderDetail> OrderDetail { get; set; }

    public virtual DbSet<Paymethod> Paymethod { get; set; }

    public virtual DbSet<Product> Product { get; set; }

    public virtual DbSet<ProductDetail> ProductDetail { get; set; }

    public virtual DbSet<ProductModel> ProductModel { get; set; }

    public virtual DbSet<PurchaseDetails> PurchaseDetails { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrder { get; set; }

    public virtual DbSet<Quotation> Quotation { get; set; }

    public virtual DbSet<QuotationDetail> QuotationDetail { get; set; }

  

    public virtual DbSet<SalesChannel> SalesChannel { get; set; }

    public virtual DbSet<ShippingDetail> ShippingDetail { get; set; }

    public virtual DbSet<ShippingOrder> ShippingOrder { get; set; }

    public virtual DbSet<Supplier> Supplier { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- 【最終修正版】 ---
        // 這段設定告訴 XiangYunDbContext Identity 表的存在，但同時將它們從自己的遷移計畫中排除。
        modelBuilder.Entity<IdentityUser>(b =>
        {
            b.ToTable("AspNetUsers", t => t.ExcludeFromMigrations()); // <-- 新增 ExcludeFromMigrations
        });

        modelBuilder.Entity<IdentityRole>(b =>
        {
            b.ToTable("AspNetRoles", t => t.ExcludeFromMigrations()); // <-- 新增 ExcludeFromMigrations
        });

        modelBuilder.Entity<IdentityUserRole<string>>(b =>
        {
            b.ToTable("AspNetUserRoles", t => t.ExcludeFromMigrations()); // <-- 新增 ExcludeFromMigrations
            b.HasKey(ur => new { ur.UserId, ur.RoleId });
        });



        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID).HasName("PK__Category__19093A2B31EC38D6");

            entity.HasIndex(e => e.CategoryName, "UQ__Category__8517B2E07683E5D6").IsUnique();

            entity.Property(e => e.CategoryImage).HasMaxLength(200);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(20);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeID).HasName("PK__Employee__7AD04FF1A6BDA2BB");

            entity.HasIndex(e => e.EmployeeNumber, "UQ__Employee__8D663598BAB6580E").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EmployeeNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Note).HasMaxLength(200);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });


        modelBuilder.Entity<InventoryMovement>(entity =>
            {
                entity.HasKey(e => e.MovementID).HasName("PK__Inventor__D1822466A7B0C9ED");

                entity.Property(e => e.MovementDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.MovementType).HasMaxLength(20);
                entity.Property(e => e.Note).HasMaxLength(200);

                entity.HasOne(d => d.ProductDetail).WithMany(p => p.InventoryMovement)
                    .HasForeignKey(d => d.ProductDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Inventory__Produ__60A75C0F");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.HasKey(e => e.MemberID).HasName("PK__Member__0CF04B38CDDE2A25");

                entity.HasIndex(e => e.Email, "UQ__Member__A9D10534AF3EA7A2").IsUnique();

                entity.HasIndex(e => e.MemberNumber, "UQ__Member__F9D9F88E15990D39").IsUnique();

                entity.Property(e => e.Address).HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsFixedLength();
                entity.Property(e => e.MemberNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Name).HasMaxLength(50);
                entity.Property(e => e.Note).HasMaxLength(200);
                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ModelSpec>(entity =>
            {
                entity.HasKey(e => e.ModelSpecID).HasName("PK__ModelSpe__BECF39024DD7D1A6");

                entity.Property(e => e.Spec).HasMaxLength(50);
                entity.Property(e => e.SpecValue).HasMaxLength(200);

                entity.HasOne(d => d.ProductModel).WithMany(p => p.ModelSpec)
                    .HasForeignKey(d => d.ProductModelID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ModelSpec__Produ__52593CB8");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderID).HasName("PK__Order__C3905BAF9F5E47D5");

                entity.HasIndex(e => e.OrderNumber, "UQ__Order__CAC5E743ADDDFF4B").IsUnique();

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.OrderNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.HasOne(d => d.Employee).WithMany(p => p.Order)
                    .HasForeignKey(d => d.EmployeeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__EmployeeI__06CD04F7");

                entity.HasOne(d => d.Member).WithMany(p => p.Order)
                    .HasForeignKey(d => d.MemberID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__MemberID__05D8E0BE");

                entity.HasOne(d => d.Paymethod).WithMany(p => p.Order)
                    .HasForeignKey(d => d.PaymethodID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__Paymethod__07C12930");

                entity.HasOne(d => d.SalesChannel).WithMany(p => p.Order)
                    .HasForeignKey(d => d.SalesChannelID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__SalesChan__08B54D69");

            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.OrderID, e.ProductDetailID }).HasName("PK__OrderDet__905886C61C6A9F4D");

                entity.Property(e => e.Discount)
                    .HasDefaultValue(1.00m)
                    .HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__0F624AF8");

                entity.HasOne(d => d.ProductDetail).WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__10566F31");
            });

            modelBuilder.Entity<Paymethod>(entity =>
            {
                entity.HasKey(e => e.PaymethodID).HasName("PK__Paymetho__4C4FA14936E3BBD9");

                entity.Property(e => e.PaymethodName).HasMaxLength(50);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.ProductID).HasName("PK__Product__B40CC6ED7A4312A0");

                entity.HasIndex(e => e.ProductSKU, "UQ__Product__A34E50D0C67CAD1D").IsUnique();

                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
                entity.Property(e => e.ProductImage).HasMaxLength(200);
                entity.Property(e => e.ProductName).HasMaxLength(100);
                entity.Property(e => e.ProductSKU)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductModel).WithMany(p => p.Product)
                    .HasForeignKey(d => d.ProductModelID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Product__Product__571DF1D5");
            });

            modelBuilder.Entity<ProductDetail>(entity =>
            {
                entity.HasKey(e => e.ProductDetailID).HasName("PK__ProductD__3C8DD6948A2DA7B1");

                entity.HasIndex(e => e.SerialNumber, "UQ__ProductD__048A0008FA6EEC8F").IsUnique();

                entity.Property(e => e.SerialNumber).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.Price).HasPrecision(18, 2);

                entity.HasOne(d => d.Product).WithMany(p => p.ProductDetail)
                    .HasForeignKey(d => d.ProductID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductDe__Produ__5BE2A6F2");
            });

            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasKey(e => e.ProductModelID).HasName("PK__ProductM__DB7B7E9BA3181C3E");

                entity.Property(e => e.Brand).HasMaxLength(50);
                entity.Property(e => e.ProductModelName).HasMaxLength(50);

                entity.HasOne(d => d.Category).WithMany(p => p.ProductModel)
                    .HasForeignKey(d => d.CategoryID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductMo__Categ__4F7CD00D");
            });

            modelBuilder.Entity<PurchaseDetails>(entity =>
            {
                entity.HasKey(e => new { e.PurchaseOrderID, e.ProductDetailID }).HasName("PK__Purchase__50A3712DDCEDD521");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.ProductDetail).WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.ProductDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PurchaseD__Produ__6EF57B66");

                entity.HasOne(d => d.PurchaseOrder).WithMany(p => p.PurchaseDetails)
                    .HasForeignKey(d => d.PurchaseOrderID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PurchaseD__Purch__6E01572D");
            });

            modelBuilder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.PurchaseOrderID).HasName("PK__Purchase__036BAC4492ED71F7");

                entity.HasIndex(e => e.PurchaseOrderNumber, "UQ__Purchase__96241948644A61D1").IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Note).HasMaxLength(200);
                entity.Property(e => e.PurchaseOrderNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Supplier).WithMany(p => p.PurchaseOrder)
                    .HasForeignKey(d => d.SupplierID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PurchaseO__Suppl__693CA210");
            });

            modelBuilder.Entity<Quotation>(entity =>
            {
                entity.HasKey(e => e.QuotationID).HasName("PK__Quotatio__E19752B3230D0696");

                entity.HasIndex(e => e.QuotationNumber, "UQ__Quotatio__F3A63C4A41B27FA1").IsUnique();

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.LastUpdate).HasColumnType("datetime");
                entity.Property(e => e.Note).HasMaxLength(200);
                entity.Property(e => e.QuotationNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);
                entity.Property(e => e.Status).HasMaxLength(20);

                entity.HasOne(d => d.Employee).WithMany(p => p.Quotation)
                    .HasForeignKey(d => d.EmployeeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Emplo__76969D2E");

                entity.HasOne(d => d.Member).WithMany(p => p.Quotation)
                    .HasForeignKey(d => d.MemberID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Membe__75A278F5");
            });

            modelBuilder.Entity<QuotationDetail>(entity =>
            {
                entity.HasKey(e => new { e.QuotationID, e.ProductDetailID }).HasName("PK__Quotatio__B25F8FDA89C56EC6");

                entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");

                entity.Property(e => e.Discount).HasPrecision(3, 2);

                entity.HasOne(d => d.ProductDetail).WithMany(p => p.QuotationDetail)
                    .HasForeignKey(d => d.ProductDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Produ__7C4F7684");

                entity.HasOne(d => d.Quotation).WithMany(p => p.QuotationDetail)
                    .HasForeignKey(d => d.QuotationID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Quotation__Quota__7B5B524B");
            });



            modelBuilder.Entity<SalesChannel>(entity =>
            {
                entity.HasKey(e => e.SalesChannelID).HasName("PK__SalesCha__5C479447F2AE0A8B");

                entity.Property(e => e.Note).HasMaxLength(200);
                entity.Property(e => e.SalesChannelName).HasMaxLength(50);
            });

            modelBuilder.Entity<ShippingDetail>(entity =>
            {
                entity.HasKey(e => new { e.ShippingOrderID, e.ProductDetailID }).HasName("PK__Shipping__35FE0BFC20DB90E8");

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.ProductDetail).WithMany(p => p.ShippingDetail)
                    .HasForeignKey(d => d.ProductDetailID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShippingD__Produ__1BC821DD");

                entity.HasOne(d => d.ShippingOrder).WithMany(p => p.ShippingDetail)
                    .HasForeignKey(d => d.ShippingOrderID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShippingD__Shipp__1AD3FDA4");
            });

            modelBuilder.Entity<ShippingOrder>(entity =>
            {
                entity.HasKey(e => e.ShippingOrderID).HasName("PK__Shipping__6636D695D30D5797");

                entity.HasIndex(e => e.ShippingNumber, "UQ__Shipping__2C1412C2DF7F5033").IsUnique();

                entity.Property(e => e.ShippingDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ShippingNumber)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.HasOne(d => d.Employee).WithMany(p => p.ShippingOrder)
                    .HasForeignKey(d => d.EmployeeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShippingO__Emplo__160F4887");

                entity.HasOne(d => d.Order).WithMany(p => p.ShippingOrder)
                    .HasForeignKey(d => d.OrderID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShippingO__Order__151B244E");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasKey(e => e.SupplierID).HasName("PK__Supplier__4BE66694BB97AD81");

                entity.HasIndex(e => e.SupplierName, "UQ__Supplier__9C5DF66FD806EFED").IsUnique();

                entity.Property(e => e.SupplierAddress).HasMaxLength(100);
                entity.Property(e => e.SupplierName).HasMaxLength(100);
                entity.Property(e => e.SupplierTel)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }
    
        

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

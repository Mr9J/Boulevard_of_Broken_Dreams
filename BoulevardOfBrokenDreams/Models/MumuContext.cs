using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class MumuContext : DbContext
{
    public MumuContext()
    {
    }

    public MumuContext(DbContextOptions<MumuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<ActionDetail> ActionDetails { get; set; }

    public virtual DbSet<ActionType> ActionTypes { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<AuthStatus> AuthStatuses { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupDetail> GroupDetails { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<LikeDetail> LikeDetails { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberInterestProjectType> MemberInterestProjectTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentMethodId> PaymentMethodIds { get; set; }

    public virtual DbSet<PaymentStatusId> PaymentStatusIds { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectFaq> ProjectFaqs { get; set; }

    public virtual DbSet<ProjectIdtype> ProjectIdtypes { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceMessage> ServiceMessages { get; set; }

    public virtual DbSet<ShipmentStatus> ShipmentStatuses { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Mumu;Integrated Security=True;Trust Server Certificate=True");//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Mumu;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Member).WithMany(p => p.Actions)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actions_Members");

            entity.HasOne(d => d.Project).WithMany(p => p.Actions)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actions_Projects");
        });

        modelBuilder.Entity<ActionDetail>(entity =>
        {
            entity.Property(e => e.ActionDetailId)
                .ValueGeneratedNever()
                .HasColumnName("ActionDetailID");
            entity.Property(e => e.ActionId).HasColumnName("ActionID");
            entity.Property(e => e.ActionTypeId).HasColumnName("ActionTypeID");

            entity.HasOne(d => d.Action).WithMany(p => p.ActionDetails)
                .HasForeignKey(d => d.ActionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionDetails_Actions");

            entity.HasOne(d => d.ActionType).WithMany(p => p.ActionDetails)
                .HasForeignKey(d => d.ActionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionDetails_ActionTypes");
        });

        modelBuilder.Entity<ActionType>(entity =>
        {
            entity.Property(e => e.ActionTypeId).HasColumnName("ActionTypeID");
            entity.Property(e => e.ActionName).HasMaxLength(50);
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Member).WithMany(p => p.Admins)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admins_Members");
        });

        modelBuilder.Entity<AuthStatus>(entity =>
        {
            entity.ToTable("AuthStatus");

            entity.Property(e => e.AuthStatusId).HasColumnName("AuthStatusID");
            entity.Property(e => e.AuthStatus1)
                .HasMaxLength(50)
                .HasColumnName("AuthStatus");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Member).WithMany(p => p.Carts)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Members");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.Property(e => e.CartDetailId).HasColumnName("CartDetailID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetails_Carts");

            entity.HasOne(d => d.Status).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetails_Status");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Member).WithMany(p => p.Comments)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Members");

            entity.HasOne(d => d.Project).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Projects");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.GroupName).HasMaxLength(50);
        });

        modelBuilder.Entity<GroupDetail>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AuthStatusId).HasColumnName("AuthStatusID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.AuthStatus).WithMany(p => p.GroupDetails)
                .HasForeignKey(d => d.AuthStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_AuthStatus");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupDetails)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_Groups");

            entity.HasOne(d => d.Member).WithMany(p => p.GroupDetails)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_Members");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Project).WithMany(p => p.Likes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Projects");
        });

        modelBuilder.Entity<LikeDetail>(entity =>
        {
            entity.Property(e => e.LikeDetailId).HasColumnName("LikeDetailID");
            entity.Property(e => e.LikeId).HasColumnName("LikeID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Like).WithMany(p => p.LikeDetails)
                .HasForeignKey(d => d.LikeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LikeDetails_Likes");

            entity.HasOne(d => d.Member).WithMany(p => p.LikeDetails)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LikeDetails_Members");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nickname).HasMaxLength(50);
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RegistrationTime).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MemberInterestProjectType>(entity =>
        {
            entity.ToTable("MemberInterestProjectType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.ProjectTypeId).HasColumnName("ProjectTypeID");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberInterestProjectTypes)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberInterestProjectType_Members");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.MemberInterestProjectTypes)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberInterestProjectType_ProjectTypes");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PaymentStatusId).HasColumnName("PaymentStatusID");
            entity.Property(e => e.ShipDate).HasColumnType("datetime");
            entity.Property(e => e.ShipmentStatusId).HasColumnName("ShipmentStatusID");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Members");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentMethodID");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentStatusID");

            entity.HasOne(d => d.ShipmentStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShipmentStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ShipmentStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");

            entity.HasOne(d => d.Project).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Projects");
        });

        modelBuilder.Entity<PaymentMethodId>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId1);

            entity.ToTable("PaymentMethodID");

            entity.Property(e => e.PaymentMethodId1).HasColumnName("PaymentMethodID");
            entity.Property(e => e.PaymentName).HasMaxLength(50);
        });

        modelBuilder.Entity<PaymentStatusId>(entity =>
        {
            entity.HasKey(e => e.PaymentStatusId1);

            entity.ToTable("PaymentStatusID");

            entity.Property(e => e.PaymentStatusId1).HasColumnName("PaymentStatusID");
            entity.Property(e => e.PaymentStatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CurrentStock)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.OnSalePrice).HasColumnType("money");
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.ProductPrice).HasColumnType("money");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Thumbnail).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Projects");

            entity.HasOne(d => d.Status).WithMany(p => p.Products)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Status");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.MemberId).HasColumnName("MemeberID");
            entity.Property(e => e.ProjectGoal).HasColumnType("money");
            entity.Property(e => e.ProjectName).HasMaxLength(100);
            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.Thumbnail).HasMaxLength(100);

            entity.HasOne(d => d.Group).WithMany(p => p.Projects)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Projects_Groups");

            entity.HasOne(d => d.Member).WithMany(p => p.Projects)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Members");

            entity.HasOne(d => d.Status).WithMany(p => p.Projects)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Status");
        });

        modelBuilder.Entity<ProjectFaq>(entity =>
        {
            entity.ToTable("ProjectFAQ");

            entity.Property(e => e.ProjectFaqid).HasColumnName("ProjectFAQID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectFaqs)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectFAQ_Projects");
        });

        modelBuilder.Entity<ProjectIdtype>(entity =>
        {
            entity.ToTable("ProjectIDType");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.ProjectTypeId).HasColumnName("ProjectTypeID");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectIdtypes)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectIDType_Projects");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.ProjectIdtypes)
                .HasForeignKey(d => d.ProjectTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectIDType_ProjectTypes");
        });

        modelBuilder.Entity<ProjectType>(entity =>
        {
            entity.Property(e => e.ProjectTypeId).HasColumnName("ProjectTypeID");
            entity.Property(e => e.ProjectTypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Admin).WithMany(p => p.Services)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_Services_Admins");

            entity.HasOne(d => d.Member).WithMany(p => p.Services)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_Members");
        });

        modelBuilder.Entity<ServiceMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId);

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Admin).WithMany(p => p.ServiceMessages)
                .HasForeignKey(d => d.AdminId)
                .HasConstraintName("FK_ServiceMessages_Admins");

            entity.HasOne(d => d.Member).WithMany(p => p.ServiceMessages)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_ServiceMessages_Members");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceMessages)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceMessages_Services");
        });

        modelBuilder.Entity<ShipmentStatus>(entity =>
        {
            entity.ToTable("ShipmentStatus");

            entity.Property(e => e.ShipmentStatusId).HasColumnName("ShipmentStatusID");
            entity.Property(e => e.ShipmentStatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("Status");

            entity.Property(e => e.StatusId).HasColumnName("StatusID");
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

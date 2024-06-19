using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.Models;

public partial class MumuDbContext : DbContext
{
    public MumuDbContext()
    {
    }

    public MumuDbContext(DbContextOptions<MumuDbContext> options)
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

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupDetail> GroupDetails { get; set; }

    public virtual DbSet<Hobby> Hobbies { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<LikeDetail> LikeDetails { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<MemberInterestProjectType> MemberInterestProjectTypes { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<PaymentMethodId> PaymentMethodIds { get; set; }

    public virtual DbSet<PaymentStatusId> PaymentStatusIds { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostLiked> PostLikeds { get; set; }

    public virtual DbSet<PostSaved> PostSaveds { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectFaq> ProjectFaqs { get; set; }

    public virtual DbSet<ProjectIdtype> ProjectIdtypes { get; set; }

    public virtual DbSet<ProjectType> ProjectTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceMessage> ServiceMessages { get; set; }

    public virtual DbSet<ShipmentStatus> ShipmentStatuses { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Mumu;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.Actions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actions_Members");

            entity.HasOne(d => d.Project).WithMany(p => p.Actions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actions_Projects");
        });

        modelBuilder.Entity<ActionDetail>(entity =>
        {
            entity.Property(e => e.ActionDetailId).ValueGeneratedNever();

            entity.HasOne(d => d.Action).WithMany(p => p.ActionDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionDetails_Actions");

            entity.HasOne(d => d.ActionType).WithMany(p => p.ActionDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActionDetails_ActionTypes");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.Admins)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admins_Members");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.Carts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Carts_Members");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetails_Carts");

            entity.HasOne(d => d.Status).WithMany(p => p.CartDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartDetails_Status");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Members");

            entity.HasOne(d => d.Project).WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Projects");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.Property(e => e.Deadline).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Project).WithMany(p => p.Coupons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Coupons_Projects");

            entity.HasOne(d => d.Status).WithMany(p => p.Coupons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Coupons_Status");
        });

        modelBuilder.Entity<GroupDetail>(entity =>
        {
            entity.HasOne(d => d.AuthStatus).WithMany(p => p.GroupDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_AuthStatus");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_Groups");

            entity.HasOne(d => d.Member).WithMany(p => p.GroupDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupDetails_Members");
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.Hobbies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hobbies_Members");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.Hobbies)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hobbies_ProjectTypes");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Projects");
        });

        modelBuilder.Entity<LikeDetail>(entity =>
        {
            entity.HasOne(d => d.Like).WithMany(p => p.LikeDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LikeDetails_Likes");

            entity.HasOne(d => d.Member).WithMany(p => p.LikeDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LikeDetails_Members");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.Property(e => e.Eid).HasDefaultValueSql("(newid())");
            entity.Property(e => e.ResetPassword)
                .HasDefaultValue("N")
                .IsFixedLength();
            entity.Property(e => e.StatusId).HasDefaultValue(7);
            entity.Property(e => e.Thumbnail).HasDefaultValue("https://cdn.mumumsit158.com/Members/User.jpg");
            entity.Property(e => e.Verified)
                .HasDefaultValue("N")
                .IsFixedLength();
        });

        modelBuilder.Entity<MemberInterestProjectType>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.MemberInterestProjectTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberInterestProjectType_Members");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.MemberInterestProjectTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MemberInterestProjectType_ProjectTypes");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Coupon).WithMany(p => p.Orders).HasConstraintName("FK_Orders_Coupons");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Members");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentMethodID");

            entity.HasOne(d => d.PaymentStatus).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentStatusID");

            entity.HasOne(d => d.ShipmentStatus).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_ShipmentStatus");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Products");

            entity.HasOne(d => d.Project).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Projects");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126038C7576EF9");

            entity.Property(e => e.IsAnonymous)
                .HasDefaultValue("N")
                .IsFixedLength();

            entity.HasOne(d => d.Member).WithMany(p => p.Posts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Members");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.PostComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostComment_Members");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostComment_Posts");
        });

        modelBuilder.Entity<PostLiked>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.PostLikeds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostLiked_Members");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikeds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostLiked_Posts");
        });

        modelBuilder.Entity<PostSaved>(entity =>
        {
            entity.HasOne(d => d.Member).WithMany(p => p.PostSaveds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostSaved_Members");

            entity.HasOne(d => d.Post).WithMany(p => p.PostSaveds)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostSaved_Posts");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Projects");

            entity.HasOne(d => d.Status).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Status");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasOne(d => d.Group).WithMany(p => p.Projects).HasConstraintName("FK_Projects_Groups");

            entity.HasOne(d => d.Member).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Members");

            entity.HasOne(d => d.Status).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projects_Status");
        });

        modelBuilder.Entity<ProjectFaq>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.ProjectFaqs).HasConstraintName("FK_ProjectFAQ_Projects");
        });

        modelBuilder.Entity<ProjectIdtype>(entity =>
        {
            entity.HasOne(d => d.Project).WithMany(p => p.ProjectIdtypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectIDType_Projects");

            entity.HasOne(d => d.ProjectType).WithMany(p => p.ProjectIdtypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProjectIDType_ProjectTypes");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Admin).WithMany(p => p.Services).HasConstraintName("FK_Services_Admins");

            entity.HasOne(d => d.Member).WithMany(p => p.Services)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_Members");
        });

        modelBuilder.Entity<ServiceMessage>(entity =>
        {
            entity.HasOne(d => d.Admin).WithMany(p => p.ServiceMessages).HasConstraintName("FK_ServiceMessages_Admins");

            entity.HasOne(d => d.Member).WithMany(p => p.ServiceMessages).HasConstraintName("FK_ServiceMessages_Members");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceMessages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ServiceMessages_Services");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

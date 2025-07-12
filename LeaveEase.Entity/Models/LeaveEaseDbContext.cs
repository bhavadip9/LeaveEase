using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LeaveEase.Entity.Models;

public partial class LeaveEaseDbContext : DbContext
{
    public LeaveEaseDbContext()
    {
    }
    private readonly IConfiguration _configuration;
    public LeaveEaseDbContext(DbContextOptions<LeaveEaseDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<TblLeaveApprove> TblLeaveApproves { get; set; }

    public virtual DbSet<TblLeaveRequest> TblLeaveRequests { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("LeaveEaseConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__Holidays__2D35D57A0AFE322B");

            entity.Property(e => e.CreatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeletedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HolidayName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HolidayType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.HolidayCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__Holidays__Create__160F4887");

            entity.HasOne(d => d.DeletedBy).WithMany(p => p.HolidayDeletedBies)
                .HasForeignKey(d => d.DeletedById)
                .HasConstraintName("FK__Holidays__Delete__17F790F9");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.HolidayUpdatedBies)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK__Holidays__Update__17036CC0");
        });

        modelBuilder.Entity<TblLeaveApprove>(entity =>
        {
            entity.HasKey(e => e.LeaveApproveId).HasName("PK__TblLeave__E622C51B543B3DBE");

            entity.ToTable("TblLeaveApprove");

            entity.Property(e => e.CreateByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeleteByName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeleteDate).HasColumnType("datetime");
            entity.Property(e => e.Remark)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdateByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.TblLeaveApproveApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblLeaveA__Appro__4D94879B");

            entity.HasOne(d => d.CreateBy).WithMany(p => p.TblLeaveApproveCreateBies)
                .HasForeignKey(d => d.CreateById)
                .HasConstraintName("FK__TblLeaveA__Creat__4E88ABD4");

            entity.HasOne(d => d.DeleteBy).WithMany(p => p.TblLeaveApproveDeleteBies)
                .HasForeignKey(d => d.DeleteById)
                .HasConstraintName("FK__TblLeaveA__Delet__5070F446");

            entity.HasOne(d => d.LeaveRequest).WithMany(p => p.TblLeaveApproves)
                .HasForeignKey(d => d.LeaveRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblLeaveA__Leave__4CA06362");

            entity.HasOne(d => d.UpdateBy).WithMany(p => p.TblLeaveApproveUpdateBies)
                .HasForeignKey(d => d.UpdateById)
                .HasConstraintName("FK__TblLeaveA__Updat__4F7CD00D");
        });

        modelBuilder.Entity<TblLeaveRequest>(entity =>
        {
            entity.HasKey(e => e.LeaveId).HasName("PK__TblLeave__796DB959EBE27874");

            entity.ToTable("TblLeaveRequest");

            entity.Property(e => e.AppliedDate).HasColumnType("datetime");
            entity.Property(e => e.CreateByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.DeleteByName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DeleteDate).HasColumnType("datetime");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.LeaveType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("CasualLeave");
            entity.Property(e => e.Reason)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.UpdateByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreateBy).WithMany(p => p.TblLeaveRequestCreateBies)
                .HasForeignKey(d => d.CreateById)
                .HasConstraintName("FK__TblLeaveR__Creat__44FF419A");

            entity.HasOne(d => d.DeleteBy).WithMany(p => p.TblLeaveRequestDeleteBies)
                .HasForeignKey(d => d.DeleteById)
                .HasConstraintName("FK__TblLeaveR__Delet__46E78A0C");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblLeaveRequestEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblLeaveR__Emplo__440B1D61");

            entity.HasOne(d => d.UpdateBy).WithMany(p => p.TblLeaveRequestUpdateBies)
                .HasForeignKey(d => d.UpdateById)
                .HasConstraintName("FK__TblLeaveR__Updat__45F365D3");
        });

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("PK__TblPermi__EFA6FB2FCAFEAFFA");

            entity.ToTable("TblPermission");

            entity.Property(e => e.CreatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.PermissionName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.TblPermissionCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__TblPermis__Creat__0E6E26BF");

            entity.HasOne(d => d.DeletedBy).WithMany(p => p.TblPermissionDeletedBies)
                .HasForeignKey(d => d.DeletedById)
                .HasConstraintName("FK__TblPermis__Delet__10566F31");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.TblPermissions)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblPermis__IsAct__0D7A0286");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.TblPermissionUpdatedBies)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK__TblPermis__Updat__0F624AF8");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__TblRole__8AFACE1A059DF785");

            entity.ToTable("TblRole");

            entity.Property(e => e.CreatedByName).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedByName).HasMaxLength(255);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedByName).HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.TblRoleCreatedBies)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK_TblRole_CreatedBy");

            entity.HasOne(d => d.DeletedBy).WithMany(p => p.TblRoleDeletedBies)
                .HasForeignKey(d => d.DeletedById)
                .HasConstraintName("FK_TblRole_DeletedBy");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.TblRoleUpdatedBies)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK_TblRole_UpdatedBy");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__TblUser__1788CC4C47717318");

            entity.ToTable("TblUser");

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DeletedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.DeletedDate).HasColumnType("datetime");
            entity.Property(e => e.Department)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.MobileNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProfileImg)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedByName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedBy).WithMany(p => p.InverseCreatedBy)
                .HasForeignKey(d => d.CreatedById)
                .HasConstraintName("FK__TblUser__Created__3C69FB99");

            entity.HasOne(d => d.DeletedBy).WithMany(p => p.InverseDeletedBy)
                .HasForeignKey(d => d.DeletedById)
                .HasConstraintName("FK__TblUser__Deleted__3E52440B");

            entity.HasOne(d => d.ReportingPersonNavigation).WithMany(p => p.InverseReportingPersonNavigation)
                .HasForeignKey(d => d.ReportingPerson)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TblUser_ReportingPerson");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.Role)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TblUser__IsActiv__3B75D760");

            entity.HasOne(d => d.UpdatedBy).WithMany(p => p.InverseUpdatedBy)
                .HasForeignKey(d => d.UpdatedById)
                .HasConstraintName("FK__TblUser__Updated__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

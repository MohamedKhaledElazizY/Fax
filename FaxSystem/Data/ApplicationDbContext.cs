 using FaxSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace FaxSystem.Data;
public class ApplicationDbContext :DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> USERS { get; set; }
    public DbSet<Branch> BRANCHES { get; set; }
    public DbSet<Fax> FAXES { get; set; }
    public DbSet<FaxReciver> FAXERECIVERS { get; set; }
    public DbSet<Decision> DECISIONS { get; set; }
    public DbSet<FaxLink> FaxLinks { get; set; }
    public DbSet<Agency> AGENCIES { get; set; }
    public DbSet<FaxBetweenBranches> FAXBRANCHES { get; set; }
    public DbSet<BranchFaxRecivers> BRANCH_FAX_RECIVER { get; set; }
    public DbSet<Role> ROLES { get; set; }
    public DbSet<UserRoles> USER_ROLES { get; set; }
    public DbSet<Log> LOGS { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FaxReciver>().HasKey(x =>new { x.BranchID, x.FaxID  });
                 
        modelBuilder.Entity<BranchFaxRecivers>().HasKey(x => new { x.BranchID, x.FaxID });

        modelBuilder.Entity<UserRoles>().HasKey(x => new { x.UserId, x.RoleId });
                 
                      
        modelBuilder.Entity<Fax>().Property(x => x.Date).HasColumnType("date");
        modelBuilder.Entity<Fax>().Property(x => x.suspend).HasDefaultValue(true);
        modelBuilder.Entity<FaxBetweenBranches>().Property(x => x.Date).HasColumnType("date");
        modelBuilder.Entity<Branch>().HasIndex(u => u.Name).IsUnique();
        modelBuilder.Entity<Agency>().HasIndex(u => u.Name).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
        modelBuilder.Entity<Fax>().HasIndex(u => u.RegistrationNum).IsUnique();
        modelBuilder.Entity<FaxBetweenBranches>().HasIndex(u => u.RegistrationNum).IsUnique();
        modelBuilder.Entity<FaxBetweenBranches>().Property(x => x.suspend).HasDefaultValue(0);
        //forignkey fax table
        // modelBuilder.Entity<Fax>().HasOne(b => b.decision).WithOne(b => b.Fax).OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Fax>().HasOne(b => b.senderAgency).WithMany(b=>b.SendFaxes).OnDelete(DeleteBehavior.NoAction);
        ////////////

        //forignkey FaxBetweenBranches table
        modelBuilder.Entity<FaxBetweenBranches>().HasOne(b => b.senderBranch).WithMany(b => b.BranchSendFaxes).OnDelete(DeleteBehavior.NoAction);
        ////////////

        //forignkey user
        modelBuilder.Entity<User>().HasOne(b => b.branch).WithMany(b => b.Users).OnDelete(DeleteBehavior.Restrict);
        //////////

        //forignkey faxreciver
        modelBuilder.Entity<FaxReciver>().HasOne(b => b.fax).WithMany(b => b.FaxRecivers).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<FaxReciver>().HasOne(b => b.branch).WithMany(b => b.FaxRecivers).OnDelete(DeleteBehavior.NoAction);
        /////////

        //faxlink
        modelBuilder.Entity<FaxLink>().HasOne(b => b.Fax).WithMany(b => b.FaxLinks).OnDelete(DeleteBehavior.NoAction);

        //forignkey branchfaxreciver
        modelBuilder.Entity<BranchFaxRecivers>().HasOne(b => b.fax).WithMany(b => b.BranchFaxRecivers).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<BranchFaxRecivers>().HasOne(b => b.branch).WithMany(b => b.BranchFaxRecivers).OnDelete(DeleteBehavior.NoAction);
        /////////

        modelBuilder.Entity<Role>().HasData(
        new Role { RoleID = 1, RoleName = "المشرف العام" },
        new Role { RoleID = 2, RoleName = "صلاحية رئيس الفرع" },
        new Role { RoleID = 3, RoleName = "صلاحية المتابعة" },
        new Role { RoleID = 4, RoleName = "صلاحية السيد المديرر" },
        new Role { RoleID = 5, RoleName = "صلاحية نائب المدير" },
        new Role { RoleID = 6, RoleName = "رئيس فرع المتابعة" },
        new Role { RoleID = 1006, RoleName = "صلاحية التكويد" }
        );

        modelBuilder.Entity<Branch>().HasData(new Branch { ID= 1, Name="فرع النظم"});
        modelBuilder.Entity<User>().HasData(new User { ID = 1, BranchID = 1, UserName = "admin", Password = "Hossam@1983"});
        modelBuilder.Entity<UserRoles>().HasData(new UserRoles { UserId = 1, RoleId = 1 });


    }


}

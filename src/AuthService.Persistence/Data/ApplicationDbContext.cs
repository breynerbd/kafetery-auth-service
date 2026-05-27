using AuthService.Domain.Entitis;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }
    public DbSet<UserPasswordReset> UserPasswordResets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ReplaceService<
            Microsoft.EntityFrameworkCore.Migrations.IMigrationsSqlGenerator,
            Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.NpgsqlMigrationsSqlGenerator>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();

            if (!string.IsNullOrEmpty(tableName))
                entity.SetTableName(ToSnakeCase(tableName));

            foreach (var property in entity.GetProperties())
            {
                var columnName = property.GetColumnName();

                if (!string.IsNullOrEmpty(columnName))
                    property.SetColumnName(ToSnakeCase(columnName));
            }
        }

        modelBuilder.Entity<Role>().ToTable("roles");
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<UserProfile>().ToTable("user_profiles");
        modelBuilder.Entity<UserRole>().ToTable("user_roles");
        modelBuilder.Entity<UserEmail>().ToTable("user_emails");
        modelBuilder.Entity<UserPasswordReset>().ToTable("user_password_resets");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("character varying(25)")
                .HasMaxLength(25)
                .IsRequired();

            entity.Property(e => e.Surname)
                .HasColumnName("surname")
                .HasColumnType("character varying(50)")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Username)
                .HasColumnName("username")
                .HasColumnType("character varying(25)")
                .HasMaxLength(25)
                .IsRequired();

            entity.Property(e => e.Email)
                .HasColumnName("email")
                .IsRequired();

            entity.Property(e => e.Password)
                .HasColumnName("password")
                .HasColumnType("character varying(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasIndex(e => e.Username).IsUnique();

            entity.HasOne(e => e.UserProfile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UserEmail)
                .WithOne(ue => ue.User)
                .HasForeignKey<UserEmail>(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UserPasswordReset)
                .WithOne(upr => upr.User)
                .HasForeignKey<UserPasswordReset>(upr => upr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("character varying(50)")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("character varying(255)")
                .HasMaxLength(255)
                .IsRequired();

            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.RoleId)
                .HasColumnName("role_id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.AssignedAt)
                .HasColumnName("assigned_at");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");

            entity.HasIndex(e => new { e.UserId, e.RoleId })
                .IsUnique();

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserEmail>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.EmailVerified)
                .HasColumnName("email_verified");

            entity.Property(e => e.EmailVerificationToken)
                .HasColumnName("email_verification_token")
                .HasColumnType("character varying(255)")
                .HasMaxLength(255);

            entity.Property(e => e.EmailVerificationTokenExpiry)
                .HasColumnName("email_verification_token_expiry");

            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.ProfilePictureUrl)
                .HasColumnName("profile_picture_url");

            entity.Property(e => e.Bio)
                .HasColumnName("bio");

            entity.Property(e => e.DateOfBirth)
                .HasColumnName("date_of_birth");

            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });

        modelBuilder.Entity<UserPasswordReset>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("character varying(36)")
                .HasMaxLength(36)
                .IsRequired();

            entity.Property(e => e.PasswordResetToken)
                .HasColumnName("password_reset_token");

            entity.Property(e => e.PasswordResetTokenExpiry)
                .HasColumnName("password_reset_token_expiry");

            entity.HasIndex(e => e.UserId)
                .IsUnique();
        });
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return string.Concat(input.Select((x, i) =>
            i > 0 && char.IsUpper(x)
                ? "_" + x.ToString().ToLower()
                : x.ToString().ToLower()
        ));
    }
}
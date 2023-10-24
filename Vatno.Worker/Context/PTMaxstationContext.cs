#nullable disable

using Microsoft.EntityFrameworkCore;
using Vatno.Worker.Models;

namespace Vatno.Worker.Context
{
    public partial class PTMaxStationContext : DbContext, IPTMaxStationContext
    {
        public PTMaxStationContext(DbContextOptions<PTMaxStationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LogVatnoMaxme> LogVatnoMaxmes { get; set; }

        public virtual DbSet<MasBranch> MasBranches { get; set; }

        public virtual DbSet<MasCompany> MasCompanies { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=serversql-microservice.database.windows.net,1433;Database=pt-max-station-db-dev;Trusted_Connection=False;User ID=utaindbadmin@serversql-microservice;Password=Ptg2020@;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_100_CI_AI_KS_SC_UTF8");
            //modelBuilder.HasAnnotation("Relational:Collation", "Thai_CI_AS");
            //modelBuilder.UseCollation("Thai_CI_AS");

            modelBuilder.Entity<MasBranch>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode });

                entity.ToTable("MAS_BRANCH");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.BranchNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BRANCH_NO");

                entity.Property(e => e.BrnName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME");

                entity.Property(e => e.BrnStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BRN_STATUS");

                entity.Property(e => e.CloseDate)
                    .HasColumnType("date")
                    .HasColumnName("CLOSE_DATE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.District)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT");

                entity.Property(e => e.Fax)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.MapBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_BRN_CODE");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");

                entity.Property(e => e.Postcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSTCODE");

                entity.Property(e => e.ProvCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PROV_CODE");

                entity.Property(e => e.Province)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PROVINCE");

                entity.Property(e => e.SubDistrict)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUB_DISTRICT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCompany>(entity =>
            {
                entity.HasKey(e => e.CompCode);

                entity.ToTable("MAS_COMPANY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.AddressEn)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS_EN");

                entity.Property(e => e.CompImage)
                    .IsUnicode(false)
                    .HasColumnName("COMP_IMAGE");

                entity.Property(e => e.CompName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("COMP_NAME");

                entity.Property(e => e.CompNameEn)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("COMP_NAME_EN");

                entity.Property(e => e.CompSataus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_SATAUS");

                entity.Property(e => e.CompSname)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_SNAME");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.District)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.MapCompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MAP_COMP_CODE");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");

                entity.Property(e => e.Postcode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSTCODE");

                entity.Property(e => e.Province)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PROVINCE");

                entity.Property(e => e.RegisterId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REGISTER_ID");

                entity.Property(e => e.SubDistrict)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUB_DISTRICT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<LogVatnoMaxme>(entity =>
            {
                entity.ToTable("LOG_VATNO_MAXME");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.ErrorMsg)
                    .IsUnicode(false)
                    .HasColumnName("ERROR_MSG");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("FILE_NAME");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
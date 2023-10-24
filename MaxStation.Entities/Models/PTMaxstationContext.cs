using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace MaxStation.Entities.Models
{
    public partial class PTMaxstationContext : DbContext
    {
        public PTMaxstationContext()
        {
        }

        public PTMaxstationContext(DbContextOptions<PTMaxstationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AutBranchRole> AutBranchRoles { get; set; }
        public virtual DbSet<AutEmployeeRole> AutEmployeeRoles { get; set; }
        public virtual DbSet<AutPositionRole> AutPositionRoles { get; set; }
        public virtual DbSet<DopFormula> DopFormulas { get; set; }
        public virtual DbSet<DopFormulaBranch> DopFormulaBranches { get; set; }
        public virtual DbSet<DopPeriod> DopPeriods { get; set; }
        public virtual DbSet<DopPeriodCash> DopPeriodCashes { get; set; }
        public virtual DbSet<DopPeriodCashGl> DopPeriodCashGls { get; set; }
        public virtual DbSet<DopPeriodCashSum> DopPeriodCashSums { get; set; }
        public virtual DbSet<DopPeriodEmp> DopPeriodEmps { get; set; }
        public virtual DbSet<DopPeriodGl> DopPeriodGls { get; set; }
        public virtual DbSet<DopPeriodLog> DopPeriodLogs { get; set; }
        public virtual DbSet<DopPeriodMeter> DopPeriodMeters { get; set; }
        public virtual DbSet<DopPeriodTank> DopPeriodTanks { get; set; }
        public virtual DbSet<DopPeriodTankSum> DopPeriodTankSums { get; set; }
        public virtual DbSet<DopPo> DopPos { get; set; }
        public virtual DbSet<DopPosConfig> DopPosConfigs { get; set; }
        public virtual DbSet<DopPosLog> DopPosLogs { get; set; }
        public virtual DbSet<DopPostdayDt> DopPostdayDts { get; set; }
        public virtual DbSet<DopPostdayHd> DopPostdayHds { get; set; }
        public virtual DbSet<DopPostdayLog> DopPostdayLogs { get; set; }
        public virtual DbSet<DopPostdaySum> DopPostdaySums { get; set; }
        public virtual DbSet<DopPostdayValidate> DopPostdayValidates { get; set; }
        public virtual DbSet<DopValidate> DopValidates { get; set; }
        public virtual DbSet<EtlLotHd> EtlLotHds { get; set; }
        public virtual DbSet<FinBalance> FinBalances { get; set; }
        public virtual DbSet<FinReceiveDt> FinReceiveDts { get; set; }
        public virtual DbSet<FinReceiveHd> FinReceiveHds { get; set; }
        public virtual DbSet<FinReceiveLog> FinReceiveLogs { get; set; }
        public virtual DbSet<FinReceivePay> FinReceivePays { get; set; }
        public virtual DbSet<InfPoConfirmation> InfPoConfirmations { get; set; }
        public virtual DbSet<InfPoHeader> InfPoHeaders { get; set; }
        public virtual DbSet<InfPoItem> InfPoItems { get; set; }
        public virtual DbSet<InfPoType> InfPoTypes { get; set; }
        public virtual DbSet<InfPosFunction14> InfPosFunction14s { get; set; }
        public virtual DbSet<InfPosFunction2> InfPosFunction2s { get; set; }
        public virtual DbSet<InfPosFunction4> InfPosFunction4s { get; set; }
        public virtual DbSet<InfPosFunction5> InfPosFunction5s { get; set; }
        public virtual DbSet<InfSapMm036> InfSapMm036s { get; set; }
        public virtual DbSet<InfSapMm036Log> InfSapMm036Logs { get; set; }
        public virtual DbSet<InfSapMm041> InfSapMm041s { get; set; }
        public virtual DbSet<InfSapMm041Log> InfSapMm041Logs { get; set; }
        public virtual DbSet<InfSapMm042> InfSapMm042s { get; set; }
        public virtual DbSet<InfSapMm042Log> InfSapMm042Logs { get; set; }
        public virtual DbSet<InfSapMm048> InfSapMm048s { get; set; }
        public virtual DbSet<InfSapMm048Log> InfSapMm048Logs { get; set; }
        public virtual DbSet<InfSapMm051> InfSapMm051s { get; set; }
        public virtual DbSet<InfSapMm051Log> InfSapMm051Logs { get; set; }
        public virtual DbSet<InfSapMm052> InfSapMm052s { get; set; }
        public virtual DbSet<InfSapMm052Log> InfSapMm052Logs { get; set; }
        public virtual DbSet<InfSapOil02> InfSapOil02s { get; set; }
        public virtual DbSet<InfSapOil02Log> InfSapOil02Logs { get; set; }
        public virtual DbSet<InfSapZmmint01> InfSapZmmint01s { get; set; }
        public virtual DbSet<InfSapZmmint01Log> InfSapZmmint01Logs { get; set; }
        public virtual DbSet<InfSapZmmint02> InfSapZmmint02s { get; set; }
        public virtual DbSet<InfSapZmmint02Log> InfSapZmmint02Logs { get; set; }
        public virtual DbSet<InfSapZmmint03> InfSapZmmint03s { get; set; }
        public virtual DbSet<InfSapZmmint03Log> InfSapZmmint03Logs { get; set; }
        public virtual DbSet<InfSapZmmint04> InfSapZmmint04s { get; set; }
        public virtual DbSet<InfSapZmmint04Log> InfSapZmmint04Logs { get; set; }
        public virtual DbSet<InvAdjustDt> InvAdjustDts { get; set; }
        public virtual DbSet<InvAdjustHd> InvAdjustHds { get; set; }
        public virtual DbSet<InvAdjustRequestDt> InvAdjustRequestDts { get; set; }
        public virtual DbSet<InvAdjustRequestHd> InvAdjustRequestHds { get; set; }
        public virtual DbSet<InvAuditDt> InvAuditDts { get; set; }
        public virtual DbSet<InvAuditHd> InvAuditHds { get; set; }
        public virtual DbSet<InvDeliveryCtrlDt> InvDeliveryCtrlDts { get; set; }
        public virtual DbSet<InvDeliveryCtrlHd> InvDeliveryCtrlHds { get; set; }
        public virtual DbSet<InvReceiveProdDt> InvReceiveProdDts { get; set; }
        public virtual DbSet<InvReceiveProdHd> InvReceiveProdHds { get; set; }
        public virtual DbSet<InvReceiveProdLog> InvReceiveProdLogs { get; set; }
        public virtual DbSet<InvRequestDt> InvRequestDts { get; set; }
        public virtual DbSet<InvRequestHd> InvRequestHds { get; set; }
        public virtual DbSet<InvReturnOilDt> InvReturnOilDts { get; set; }
        public virtual DbSet<InvReturnOilHd> InvReturnOilHds { get; set; }
        public virtual DbSet<InvReturnSupDt> InvReturnSupDts { get; set; }
        public virtual DbSet<InvReturnSupHd> InvReturnSupHds { get; set; }
        public virtual DbSet<InvStockDaily> InvStockDailies { get; set; }
        public virtual DbSet<InvStockMonthly> InvStockMonthlies { get; set; }
        public virtual DbSet<InvSupplyRequestDt> InvSupplyRequestDts { get; set; }
        public virtual DbSet<InvSupplyRequestHd> InvSupplyRequestHds { get; set; }
        public virtual DbSet<InvTraninDt> InvTraninDts { get; set; }
        public virtual DbSet<InvTraninHd> InvTraninHds { get; set; }
        public virtual DbSet<InvTranoutDt> InvTranoutDts { get; set; }
        public virtual DbSet<InvTranoutHd> InvTranoutHds { get; set; }
        public virtual DbSet<InvTranoutLog> InvTranoutLogs { get; set; }
        public virtual DbSet<InvUnuseDt> InvUnuseDts { get; set; }
        public virtual DbSet<InvUnuseHd> InvUnuseHds { get; set; }
        public virtual DbSet<InvWithdrawDt> InvWithdrawDts { get; set; }
        public virtual DbSet<InvWithdrawHd> InvWithdrawHds { get; set; }
        public virtual DbSet<InvWithdrawLog> InvWithdrawLogs { get; set; }
        public virtual DbSet<LogError> LogErrors { get; set; }
        public virtual DbSet<LogLogin> LogLogins { get; set; }
        public virtual DbSet<LogStoreProcedure> LogStoreProcedures { get; set; }
        public virtual DbSet<LogVatnoMaxme> LogVatnoMaxmes { get; set; }
        public virtual DbSet<MasBank> MasBanks { get; set; }
        public virtual DbSet<MasBillerInfo> MasBillerInfos { get; set; }
        public virtual DbSet<MasBillerInquiryScb> MasBillerInquiryScbs { get; set; }
        public virtual DbSet<MasBillerTransactionActionScb> MasBillerTransactionActionScbs { get; set; }
        public virtual DbSet<MasBillerTransactionScb> MasBillerTransactionScbs { get; set; }
        public virtual DbSet<MasBranch> MasBranches { get; set; }
        public virtual DbSet<MasBranchCalibrate> MasBranchCalibrates { get; set; }
        public virtual DbSet<MasBranchConfig> MasBranchConfigs { get; set; }
        public virtual DbSet<MasBranchConfigDesc> MasBranchConfigDescs { get; set; }
        public virtual DbSet<MasBranchDisp> MasBranchDisps { get; set; }
        public virtual DbSet<MasBranchLog> MasBranchLogs { get; set; }
        public virtual DbSet<MasBranchMid> MasBranchMids { get; set; }
        public virtual DbSet<MasBranchPeriod> MasBranchPeriods { get; set; }
        public virtual DbSet<MasBranchTank> MasBranchTanks { get; set; }
        public virtual DbSet<MasBranchTax> MasBranchTaxes { get; set; }
        public virtual DbSet<MasCenterLog> MasCenterLogs { get; set; }
        public virtual DbSet<MasCode> MasCodes { get; set; }
        public virtual DbSet<MasCompany> MasCompanies { get; set; }
        public virtual DbSet<MasCompanyCar> MasCompanyCars { get; set; }
        public virtual DbSet<MasCompanyMapping> MasCompanyMappings { get; set; }
        public virtual DbSet<MasControl> MasControls { get; set; }
        public virtual DbSet<MasCostCenter> MasCostCenters { get; set; }
        public virtual DbSet<MasCustomer> MasCustomers { get; set; }
        public virtual DbSet<MasCustomerCar> MasCustomerCars { get; set; }
        public virtual DbSet<MasDensity> MasDensities { get; set; }
        public virtual DbSet<MasDocPattern> MasDocPatterns { get; set; }
        public virtual DbSet<MasDocPatternDt> MasDocPatternDts { get; set; }
        public virtual DbSet<MasDocumentType> MasDocumentTypes { get; set; }
        public virtual DbSet<MasEmployee> MasEmployees { get; set; }
        public virtual DbSet<MasEmployeeLevel> MasEmployeeLevels { get; set; }
        public virtual DbSet<MasGl> MasGls { get; set; }
        public virtual DbSet<MasGlAccount> MasGlAccounts { get; set; }
        public virtual DbSet<MasGlMap> MasGlMaps { get; set; }
        public virtual DbSet<MasMapping> MasMappings { get; set; }
        public virtual DbSet<MasOrganize> MasOrganizes { get; set; }
        public virtual DbSet<MasPayGroup> MasPayGroups { get; set; }
        public virtual DbSet<MasPayType> MasPayTypes { get; set; }
        public virtual DbSet<MasPayTypeDt> MasPayTypeDts { get; set; }
        public virtual DbSet<MasPosition> MasPositions { get; set; }
        public virtual DbSet<MasProduct> MasProducts { get; set; }
        public virtual DbSet<MasProductGroup> MasProductGroups { get; set; }
        public virtual DbSet<MasProductPrice> MasProductPrices { get; set; }
        public virtual DbSet<MasProductSubGroup> MasProductSubGroups { get; set; }
        public virtual DbSet<MasProductType> MasProductTypes { get; set; }
        public virtual DbSet<MasProductUnit> MasProductUnits { get; set; }
        public virtual DbSet<MasReason> MasReasons { get; set; }
        public virtual DbSet<MasReasonGroup> MasReasonGroups { get; set; }
        public virtual DbSet<MasSapCustomer> MasSapCustomers { get; set; }
        public virtual DbSet<MasSapPlant> MasSapPlants { get; set; }
        public virtual DbSet<MasSupplier> MasSuppliers { get; set; }
        public virtual DbSet<MasSupplierPay> MasSupplierPays { get; set; }
        public virtual DbSet<MasSupplierProduct> MasSupplierProducts { get; set; }
        public virtual DbSet<MasToken> MasTokens { get; set; }
        public virtual DbSet<MasUnit> MasUnits { get; set; }
        public virtual DbSet<MasUtf8> MasUtf8s { get; set; }
        public virtual DbSet<MasWarehouse> MasWarehouses { get; set; }
        public virtual DbSet<OilPromotionPriceDt> OilPromotionPriceDts { get; set; }
        public virtual DbSet<OilPromotionPriceHd> OilPromotionPriceHds { get; set; }
        public virtual DbSet<OilStandardPriceDt> OilStandardPriceDts { get; set; }
        public virtual DbSet<OilStandardPriceHd> OilStandardPriceHds { get; set; }
        public virtual DbSet<PriNonoilDt> PriNonoilDts { get; set; }
        public virtual DbSet<PriNonoilHd> PriNonoilHds { get; set; }
        public virtual DbSet<PriOilStandardDt> PriOilStandardDts { get; set; }
        public virtual DbSet<PriOilStandardHd> PriOilStandardHds { get; set; }
        public virtual DbSet<SalBillingDt> SalBillingDts { get; set; }
        public virtual DbSet<SalBillingHd> SalBillingHds { get; set; }
        public virtual DbSet<SalCashsaleDt> SalCashsaleDts { get; set; }
        public virtual DbSet<SalCashsaleHd> SalCashsaleHds { get; set; }
        public virtual DbSet<SalCashsaleLog> SalCashsaleLogs { get; set; }
        public virtual DbSet<SalCndnDt> SalCndnDts { get; set; }
        public virtual DbSet<SalCndnHd> SalCndnHds { get; set; }
        public virtual DbSet<SalCreditsaleDt> SalCreditsaleDts { get; set; }
        public virtual DbSet<SalCreditsaleHd> SalCreditsaleHds { get; set; }
        public virtual DbSet<SalCreditsaleLog> SalCreditsaleLogs { get; set; }
        public virtual DbSet<SalQuotationDt> SalQuotationDts { get; set; }
        public virtual DbSet<SalQuotationHd> SalQuotationHds { get; set; }
        public virtual DbSet<SalQuotationLog> SalQuotationLogs { get; set; }
        public virtual DbSet<SalTaxinvoiceDt> SalTaxinvoiceDts { get; set; }
        public virtual DbSet<SalTaxinvoiceHd> SalTaxinvoiceHds { get; set; }
        public virtual DbSet<SapCustomerMapping> SapCustomerMappings { get; set; }
        public virtual DbSet<SapMaterialMapping> SapMaterialMappings { get; set; }
        public virtual DbSet<SapMovementTypeMapping> SapMovementTypeMappings { get; set; }
        public virtual DbSet<SapPlantMapping> SapPlantMappings { get; set; }
        public virtual DbSet<SysApproveConfig> SysApproveConfigs { get; set; }
        public virtual DbSet<SysApproveDt> SysApproveDts { get; set; }
        public virtual DbSet<SysApproveHd> SysApproveHds { get; set; }
        public virtual DbSet<SysApproveStep> SysApproveSteps { get; set; }
        public virtual DbSet<SysApproveoilDt> SysApproveoilDts { get; set; }
        public virtual DbSet<SysApproveoilHd> SysApproveoilHds { get; set; }
        public virtual DbSet<SysBranchConfig> SysBranchConfigs { get; set; }
        public virtual DbSet<SysBranchConfigRole> SysBranchConfigRoles { get; set; }
        public virtual DbSet<SysConfigApi> SysConfigApis { get; set; }
        public virtual DbSet<SysMenu> SysMenus { get; set; }
        public virtual DbSet<SysMenuRole> SysMenuRoles { get; set; }
        public virtual DbSet<SysMessage> SysMessages { get; set; }
        public virtual DbSet<SysNotification> SysNotifications { get; set; }
        public virtual DbSet<SysReportConfig> SysReportConfigs { get; set; }
        public virtual DbSet<TmpBranchPilot> TmpBranchPilots { get; set; }
        public virtual DbSet<TrnScbLog> TrnScbLogs { get; set; }
        public virtual DbSet<VInfSapMm036> VInfSapMm036s { get; set; }
        public virtual DbSet<VInfSapMm041> VInfSapMm041s { get; set; }
        public virtual DbSet<VInfSapMm042> VInfSapMm042s { get; set; }
        public virtual DbSet<VInfSapMm048> VInfSapMm048s { get; set; }
        public virtual DbSet<VInfSapMm051> VInfSapMm051s { get; set; }
        public virtual DbSet<VInfSapMm052> VInfSapMm052s { get; set; }
        public virtual DbSet<VInfSapOil02> VInfSapOil02s { get; set; }
        public virtual DbSet<VInfSapZmmint01> VInfSapZmmint01s { get; set; }
        public virtual DbSet<VInfSapZmmint02> VInfSapZmmint02s { get; set; }
        public virtual DbSet<VInfSapZmmint03> VInfSapZmmint03s { get; set; }
        public virtual DbSet<VInfSapZmmint04> VInfSapZmmint04s { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=serversql-microservice.database.windows.net,1433;Database=pt-max-station-db-dev;Trusted_Connection=False;User ID=utaindbadmin@serversql-microservice;Password=Ptg2020@;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Thai_100_CI_AI_KS_SC_UTF8");

            modelBuilder.Entity<AutBranchRole>(entity =>
            {
                entity.HasKey(e => new { e.AuthCode, e.CompCode, e.BrnCode });

                entity.ToTable("AUT_BRANCH_ROLE");

                entity.Property(e => e.AuthCode).HasColumnName("AUTH_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.AuthName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("AUTH_NAME");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<AutEmployeeRole>(entity =>
            {
                entity.HasKey(e => e.EmpCode);

                entity.ToTable("AUT_EMPLOYEE_ROLE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.AuthCode).HasColumnName("AUTH_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_CODE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<AutPositionRole>(entity =>
            {
                entity.HasKey(e => new { e.PositionCode, e.MenuId });

                entity.ToTable("AUT_POSITION_ROLE");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_CODE");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MENU_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IsCancel)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_CANCEL")
                    .IsFixedLength(true);

                entity.Property(e => e.IsCreate)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_CREATE")
                    .IsFixedLength(true);

                entity.Property(e => e.IsEdit)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_EDIT")
                    .IsFixedLength(true);

                entity.Property(e => e.IsPrint)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_PRINT")
                    .IsFixedLength(true);

                entity.Property(e => e.IsView)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_VIEW")
                    .IsFixedLength(true);

                entity.Property(e => e.JsonData)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.PostnameThai)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("POSTNAME_THAI");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopFormula>(entity =>
            {
                entity.HasKey(e => e.FmNo);

                entity.ToTable("DOP_FORMULA");

                entity.Property(e => e.FmNo)
                    .ValueGeneratedNever()
                    .HasColumnName("FM_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DestinationType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DESTINATION_TYPE");

                entity.Property(e => e.DestinationValue)
                    .IsUnicode(false)
                    .HasColumnName("DESTINATION_VALUE");

                entity.Property(e => e.FmName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("FM_NAME");

                entity.Property(e => e.FmStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("FM_STATUS");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.SourceKey)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_KEY");

                entity.Property(e => e.SourceName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_NAME");

                entity.Property(e => e.SourceType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_TYPE");

                entity.Property(e => e.SourceValue)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_VALUE");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopFormulaBranch>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.FmNo });

                entity.ToTable("DOP_FORMULA_BRANCH");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.FmNo).HasColumnName("FM_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriod>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo });

                entity.ToTable("DOP_PERIOD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EtlLotNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ETL_LOT_NO");

                entity.Property(e => e.IsPos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_POS")
                    .IsFixedLength(true);

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.SumMeterSaleQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_METER_SALE_QTY");

                entity.Property(e => e.SumMeterTotalQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_METER_TOTAL_QTY");

                entity.Property(e => e.TimeFinish)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIME_FINISH");

                entity.Property(e => e.TimeStart)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIME_START");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodCash>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.PdId });

                entity.ToTable("DOP_PERIOD_CASH");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.CashAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CASH_AMT");

                entity.Property(e => e.CouponAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("COUPON_AMT");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CreditAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CREDIT_AMT");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.MeterAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_AMT");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.SaleAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_AMT");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodCashGl>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.SeqNo });

                entity.ToTable("DOP_PERIOD_CASH_GL");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GlAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GL_AMT");

                entity.Property(e => e.GlDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("GL_DESC");

                entity.Property(e => e.GlNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_NO");

                entity.Property(e => e.GlType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_TYPE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodCashSum>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo });

                entity.ToTable("DOP_PERIOD_CASH_SUM");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.CashAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CASH_AMT");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DepositAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DEPOSIT_AMT");

                entity.Property(e => e.DiffAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DIFF_AMT");

                entity.Property(e => e.RealAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REAL_AMT");

                entity.Property(e => e.SumCrAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_CR_AMT");

                entity.Property(e => e.SumDrAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_DR_AMT");

                entity.Property(e => e.SumSlipAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_SLIP_AMT");

                entity.Property(e => e.SumTotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_TOTAL_AMT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodEmp>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.SeqNo });

                entity.ToTable("DOP_PERIOD_EMP");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");
            });

            modelBuilder.Entity<DopPeriodGl>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.GlType, e.GlNo });

                entity.ToTable("DOP_PERIOD_GL");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.GlType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_TYPE");

                entity.Property(e => e.GlNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GlAccount)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_ACCOUNT");

                entity.Property(e => e.GlDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("GL_DESC");

                entity.Property(e => e.GlLock)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GL_LOCK")
                    .IsFixedLength(true);

                entity.Property(e => e.GlSeqNo).HasColumnName("GL_SEQ_NO");

                entity.Property(e => e.GlSlip)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GL_SLIP")
                    .IsFixedLength(true);

                entity.Property(e => e.GlStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("DOP_PERIOD_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");
            });

            modelBuilder.Entity<DopPeriodMeter>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.DispId });

                entity.ToTable("DOP_PERIOD_METER");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.DispId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DISP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DispStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DISP_STATUS");

                entity.Property(e => e.MeterFinish)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_FINISH");

                entity.Property(e => e.MeterMax)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_MAX");

                entity.Property(e => e.MeterStart)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_START");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.PeriodStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PERIOD_STATUS");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RepairAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REPAIR_AMT");

                entity.Property(e => e.RepairFinish)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REPAIR_FINISH");

                entity.Property(e => e.RepairQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REPAIR_QTY");

                entity.Property(e => e.RepairStart)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REPAIR_START");

                entity.Property(e => e.SaleAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_AMT");

                entity.Property(e => e.SaleQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_QTY");

                entity.Property(e => e.TankId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TANK_ID");

                entity.Property(e => e.TestAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TEST_AMT");

                entity.Property(e => e.TestFinish)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TEST_FINISH");

                entity.Property(e => e.TestQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TEST_QTY");

                entity.Property(e => e.TestStart)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TEST_START");

                entity.Property(e => e.TotalQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_QTY");

                entity.Property(e => e.Unitprice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNITPRICE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPeriodTank>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.TankId });

                entity.ToTable("DOP_PERIOD_TANK");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.TankId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TANK_ID");

                entity.Property(e => e.BeforeQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_QTY");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DiffQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DIFF_QTY");

                entity.Property(e => e.Height)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("HEIGHT");

                entity.Property(e => e.Hold)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("HOLD")
                    .IsFixedLength(true);

                entity.Property(e => e.HoldReasonDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("HOLD_REASON_DESC");

                entity.Property(e => e.HoldReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("HOLD_REASON_ID");

                entity.Property(e => e.IssueQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ISSUE_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.PeriodStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PERIOD_STATUS");

                entity.Property(e => e.RealQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REAL_QTY");

                entity.Property(e => e.ReceiveQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RECEIVE_QTY");

                entity.Property(e => e.RemainQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REMAIN_QTY");

                entity.Property(e => e.SaleQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_QTY");

                entity.Property(e => e.TransferQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TRANSFER_QTY");

                entity.Property(e => e.Unitprice)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("UNITPRICE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.WaterHeight)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WATER_HEIGHT");

                entity.Property(e => e.WaterQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WATER_QTY");

                entity.Property(e => e.WithdrawQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WITHDRAW_QTY");
            });

            modelBuilder.Entity<DopPeriodTankSum>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.PeriodNo, e.PdId });

                entity.ToTable("DOP_PERIOD_TANK_SUM");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.AdjustQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_QTY");

                entity.Property(e => e.BalanceQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE_QTY");

                entity.Property(e => e.BeforeQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_QTY");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IssueQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ISSUE_QTY");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.ReceiveQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RECEIVE_QTY");

                entity.Property(e => e.SaleQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_QTY");

                entity.Property(e => e.TraninQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TRANIN_QTY");

                entity.Property(e => e.TransferQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TRANSFER_QTY");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.WithdrawQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WITHDRAW_QTY");
            });

            modelBuilder.Entity<DopPo>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocDate, e.PayGroupId, e.Period })
                    .HasName("PK_DOP_POS_1");

                entity.ToTable("DOP_POS");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.PayGroupId).HasColumnName("PAY_GROUP_ID");

                entity.Property(e => e.Period).HasColumnName("PERIOD");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");
            });

            modelBuilder.Entity<DopPosConfig>(entity =>
            {
                entity.HasKey(e => new { e.DocType, e.DocDesc });

                entity.ToTable("DOP_POS_CONFIG");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocDesc)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DESC");

                entity.Property(e => e.ApiUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("API_URL");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPosLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("DOP_POS_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.PayGroupId).HasColumnName("PAY_GROUP_ID");

                entity.Property(e => e.Period).HasColumnName("PERIOD");
            });

            modelBuilder.Entity<DopPostdayDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocDate, e.DocType, e.SeqNo });

                entity.ToTable("DOP_POSTDAY_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.DocFinish).HasColumnName("DOC_FINISH");

                entity.Property(e => e.DocNo).HasColumnName("DOC_NO");

                entity.Property(e => e.DocStart).HasColumnName("DOC_START");

                entity.Property(e => e.Total).HasColumnName("TOTAL");

                entity.Property(e => e.TypeId).HasColumnName("TYPE_ID");

                entity.Property(e => e.TypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("TYPE_NAME");
            });

            modelBuilder.Entity<DopPostdayHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocDate });

                entity.ToTable("DOP_POSTDAY_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.BillPayment)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("BILL_PAYMENT")
                    .IsFixedLength(true);

                entity.Property(e => e.CashAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CASH_AMT");

                entity.Property(e => e.ChequeAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CHEQUE_AMT");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DepositAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DEPOSIT_AMT");

                entity.Property(e => e.DiffAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DIFF_AMT");

                entity.Property(e => e.EtlLotNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ETL_LOT_NO");

                entity.Property(e => e.Remark)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<DopPostdayLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("DOP_POSTDAY_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.MessageLog)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE_LOG");
            });

            modelBuilder.Entity<DopPostdaySum>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocDate, e.SeqNo });

                entity.ToTable("DOP_POSTDAY_SUM");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.FmNo).HasColumnName("FM_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.SumDetail)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_DETAIL");

                entity.Property(e => e.SumHead)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_HEAD");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<DopPostdayValidate>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocDate, e.SeqNo });

                entity.ToTable("DOP_POSTDAY_VALIDATE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ValidRemark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("VALID_REMARK");

                entity.Property(e => e.ValidResult)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("VALID_RESULT");
            });

            modelBuilder.Entity<DopValidate>(entity =>
            {
                entity.HasKey(e => e.ValidNo);

                entity.ToTable("DOP_VALIDATE");

                entity.Property(e => e.ValidNo)
                    .ValueGeneratedNever()
                    .HasColumnName("VALID_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.SourceKey)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_KEY");

                entity.Property(e => e.SourceName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_NAME");

                entity.Property(e => e.SourceType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_TYPE");

                entity.Property(e => e.SourceValue)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE_VALUE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.ValidName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("VALID_NAME");

                entity.Property(e => e.ValidSql)
                    .IsUnicode(false)
                    .HasColumnName("VALID_SQL");

                entity.Property(e => e.ValidStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VALID_STATUS");
            });

            modelBuilder.Entity<EtlLotHd>(entity =>
            {
                entity.HasKey(e => new { e.LotNo, e.LotSource })
                    .HasName("PK_ETL_LOT_HD_1");

                entity.ToTable("ETL_LOT_HD");

                entity.Property(e => e.LotNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LOT_NO");

                entity.Property(e => e.LotSource)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LOT_SOURCE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.LotStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("LOT_STATUS");

                entity.Property(e => e.LotTotal)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("LOT_TOTAL");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<FinBalance>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo });

                entity.ToTable("FIN_BALANCE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BalanceAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE_AMT");

                entity.Property(e => e.BalanceAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE_AMT_CUR");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DueDate)
                    .HasColumnType("date")
                    .HasColumnName("DUE_DATE");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<FinReceiveDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("FIN_RECEIVE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.AccountNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCOUNT_NO");

                entity.Property(e => e.ItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_AMT");

                entity.Property(e => e.ItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_AMT_CUR");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<FinReceiveHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("FIN_RECEIVE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.AccountNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCOUNT_NO");

                entity.Property(e => e.BankName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BANK_NAME");

                entity.Property(e => e.BankNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BANK_NO");

                entity.Property(e => e.BillNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BILL_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EtlLotNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ETL_LOT_NO");

                entity.Property(e => e.FeeAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("FEE_AMT");

                entity.Property(e => e.FeeAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("FEE_AMT_CUR");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.PayDate)
                    .HasColumnType("date")
                    .HasColumnName("PAY_DATE");

                entity.Property(e => e.PayNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PAY_NO");

                entity.Property(e => e.PayType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PAY_TYPE");

                entity.Property(e => e.PayTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_TYPE_ID");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.ReceiveType)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_TYPE");

                entity.Property(e => e.ReceiveTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_TYPE_ID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");

                entity.Property(e => e.WhtAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WHT_AMT");

                entity.Property(e => e.WhtAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WHT_AMT_CUR");
            });

            modelBuilder.Entity<FinReceiveLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("FIN_RECEIVE_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");
            });

            modelBuilder.Entity<FinReceivePay>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("FIN_RECEIVE_PAY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.BillBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BILL_BRN_CODE");

                entity.Property(e => e.BillNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BILL_NO");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_TYPE");

                entity.Property(e => e.PayAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("PAY_AMT");

                entity.Property(e => e.RemainAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REMAIN_AMT");

                entity.Property(e => e.TxAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TX_AMT");

                entity.Property(e => e.TxBalance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TX_BALANCE");

                entity.Property(e => e.TxBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_BRN_CODE");

                entity.Property(e => e.TxDate)
                    .HasColumnType("date")
                    .HasColumnName("TX_DATE");

                entity.Property(e => e.TxNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_NO");
            });

            modelBuilder.Entity<InfPoConfirmation>(entity =>
            {
                entity.HasKey(e => new { e.PoNumber, e.PoItem });

                entity.ToTable("INF_PO_CONFIRMATION");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PoItem)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DelivItem).HasColumnName("DELIV_ITEM");

                entity.Property(e => e.DelivNumb)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_NUMB");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("ERROR_MSG");

                entity.Property(e => e.InfStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("INF_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.IsDeleted)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_DELETED")
                    .IsFixedLength(true);

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("date")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InfPoHeader>(entity =>
            {
                entity.HasKey(e => e.PoNumber);

                entity.ToTable("INF_PO_HEADER");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREAT_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedBy1)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY_1");

                entity.Property(e => e.CreatedDate1)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE_1");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.DeleteInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("DELETE_IND")
                    .IsFixedLength(true);

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DownpayAmount)
                    .HasColumnType("decimal(28, 4)")
                    .HasColumnName("DOWNPAY_AMOUNT");

                entity.Property(e => e.DownpayDuedate)
                    .HasColumnType("date")
                    .HasColumnName("DOWNPAY_DUEDATE");

                entity.Property(e => e.DownpayPercent)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("DOWNPAY_PERCENT");

                entity.Property(e => e.DownpayType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("DOWNPAY_TYPE");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("ERROR_MSG");

                entity.Property(e => e.InfStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("INF_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.IsDeleted)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_DELETED")
                    .IsFixedLength(true);

                entity.Property(e => e.Pmnttrms)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PMNTTRMS");

                entity.Property(e => e.PurGroup)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PUR_GROUP");

                entity.Property(e => e.PurchOrg)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PURCH_ORG");

                entity.Property(e => e.ReceiveBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_BRN_CODE");

                entity.Property(e => e.ReceiveDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_DOC_NO");

                entity.Property(e => e.ReceiveLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_LOC_CODE");

                entity.Property(e => e.ReceiveStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_STATUS");

                entity.Property(e => e.ReceiveUpdate)
                    .HasColumnType("datetime")
                    .HasColumnName("RECEIVE_UPDATE");

                entity.Property(e => e.RetentionPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("RETENTION_PERCENTAGE");

                entity.Property(e => e.RetentionType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("RETENTION_TYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("date")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("VENDOR");
            });

            modelBuilder.Entity<InfPoItem>(entity =>
            {
                entity.HasKey(e => new { e.PoNumber, e.PoItem });

                entity.ToTable("INF_PO_ITEM");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PoItem)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.Acctasscat)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ACCTASSCAT");

                entity.Property(e => e.Building)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BUILDING");

                entity.Property(e => e.COName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("C_O_NAME");

                entity.Property(e => e.City)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CITY");

                entity.Property(e => e.CityNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITY_NO");

                entity.Property(e => e.Country)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COUNTRY");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DelDatcatExt)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("DEL_DATCAT_EXT");

                entity.Property(e => e.DeleteInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("DELETE_IND")
                    .IsFixedLength(true);

                entity.Property(e => e.DelivTime)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_TIME");

                entity.Property(e => e.DeliveryDate)
                    .HasColumnType("date")
                    .HasColumnName("DELIVERY_DATE");

                entity.Property(e => e.District)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT");

                entity.Property(e => e.DownpayAmount)
                    .HasColumnType("decimal(28, 4)")
                    .HasColumnName("DOWNPAY_AMOUNT");

                entity.Property(e => e.DownpayDuedate)
                    .HasColumnType("date")
                    .HasColumnName("DOWNPAY_DUEDATE");

                entity.Property(e => e.DownpayPercent)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("DOWNPAY_PERCENT");

                entity.Property(e => e.EMail)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("E_MAIL");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.ErrorMsg)
                    .HasMaxLength(4000)
                    .IsUnicode(false)
                    .HasColumnName("ERROR_MSG");

                entity.Property(e => e.Floor)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FLOOR");

                entity.Property(e => e.FreeItem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("FREE_ITEM");

                entity.Property(e => e.HouseNo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("HOUSE_NO");

                entity.Property(e => e.InfStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("INF_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.InfoRec)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("INFO_REC");

                entity.Property(e => e.IsDeleted)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_DELETED")
                    .IsFixedLength(true);

                entity.Property(e => e.ItemCat)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_CAT");

                entity.Property(e => e.Langu)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("LANGU");

                entity.Property(e => e.Location)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("LOCATION");

                entity.Property(e => e.Material)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MatlGroup)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MATL_GROUP");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME");

                entity.Property(e => e.Name2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME_2");

                entity.Property(e => e.Name3)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME_3");

                entity.Property(e => e.Name4)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("NAME_4");

                entity.Property(e => e.NetPrice)
                    .HasColumnType("decimal(38, 9)")
                    .HasColumnName("NET_PRICE");

                entity.Property(e => e.NoMoreGr)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("NO_MORE_GR");

                entity.Property(e => e.OrderprUn)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ORDERPR_UN");

                entity.Property(e => e.OverDlvTol)
                    .HasColumnType("decimal(3, 2)")
                    .HasColumnName("OVER_DLV_TOL");

                entity.Property(e => e.PlanDel).HasColumnName("PLAN_DEL");

                entity.Property(e => e.Plant)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PoDate)
                    .HasColumnType("date")
                    .HasColumnName("PO_DATE");

                entity.Property(e => e.PoItem1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM_1");

                entity.Property(e => e.PoItem2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM_2");

                entity.Property(e => e.PoUnit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PO_UNIT");

                entity.Property(e => e.PostlCod1)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSTL_COD1");

                entity.Property(e => e.PostlCod2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSTL_COD2");

                entity.Property(e => e.PreqItem)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PREQ_ITEM");

                entity.Property(e => e.PreqItem2)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PREQ_ITEM_2");

                entity.Property(e => e.PreqNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PREQ_NO");

                entity.Property(e => e.PriceUnit).HasColumnName("PRICE_UNIT");

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(14, 3)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.ReceiveFloor)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RECEIVE_FLOOR");

                entity.Property(e => e.ReceiveQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RECEIVE_QTY");

                entity.Property(e => e.ReceiveUpdate)
                    .HasColumnType("datetime")
                    .HasColumnName("RECEIVE_UPDATE");

                entity.Property(e => e.Region)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RetItem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("RET_ITEM");

                entity.Property(e => e.RetentionPercentage)
                    .HasColumnType("decimal(8, 2)")
                    .HasColumnName("RETENTION_PERCENTAGE");

                entity.Property(e => e.RfqItem)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("RFQ_ITEM");

                entity.Property(e => e.RoomNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ROOM_NO");

                entity.Property(e => e.SchedLine)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SCHED_LINE");

                entity.Property(e => e.ShortText)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHORT_TEXT");

                entity.Property(e => e.Sort1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SORT1");

                entity.Property(e => e.Sort2)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SORT2");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.StatDate)
                    .HasColumnType("date")
                    .HasColumnName("STAT_DATE");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.StrSuppl1)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("STR_SUPPL1");

                entity.Property(e => e.StrSuppl2)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("STR_SUPPL2");

                entity.Property(e => e.Street)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("STREET");

                entity.Property(e => e.StreetNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("STREET_NO");

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("TAX_CODE");

                entity.Property(e => e.Transpzone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TRANSPZONE");

                entity.Property(e => e.UnderDlvTol)
                    .HasColumnType("decimal(3, 2)")
                    .HasColumnName("UNDER_DLV_TOL");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("date")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.ValType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAL_TYPE");
            });

            modelBuilder.Entity<InfPoType>(entity =>
            {
                entity.HasKey(e => e.PoTypeId);

                entity.ToTable("INF_PO_TYPE");

                entity.Property(e => e.PoTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_TYPE_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PoTypeDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PO_TYPE_DESC");

                entity.Property(e => e.PoTypeRemark)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PO_TYPE_REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InfPosFunction14>(entity =>
            {
                entity.HasKey(e => new { e.SiteId, e.BusinessDate, e.ShiftNo, e.JournalId, e.MopCode, e.ItemNo })
                    .HasName("PK__INF_POS___C7535B98FE2824BD");

                entity.ToTable("INF_POS_FUNCTION14");

                entity.Property(e => e.SiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SITE_ID");

                entity.Property(e => e.BusinessDate)
                    .HasColumnType("datetime")
                    .HasColumnName("BUSINESS_DATE");

                entity.Property(e => e.ShiftNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHIFT_NO");

                entity.Property(e => e.JournalId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("JOURNAL_ID");

                entity.Property(e => e.MopCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MOP_CODE");

                entity.Property(e => e.ItemNo).HasColumnName("ITEM_NO");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.BranchAt)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BRANCH_AT");

                entity.Property(e => e.Bstatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BSTATUS");

                entity.Property(e => e.CardNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CARD_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.InsertTimestamp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("INSERT_TIMESTAMP");

                entity.Property(e => e.MopInfo)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("MOP_INFO");

                entity.Property(e => e.Pono)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PONO");

                entity.Property(e => e.PosId).HasColumnName("POS_ID");

                entity.Property(e => e.PosName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("POS_NAME");

                entity.Property(e => e.ShiftDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHIFT_DESC");

                entity.Property(e => e.ShiftId).HasColumnName("SHIFT_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InfPosFunction2>(entity =>
            {
                entity.HasKey(e => new { e.SiteId, e.BusinessDate, e.ShiftNo, e.HoseId });

                entity.ToTable("INF_POS_FUNCTION2");

                entity.Property(e => e.SiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SITE_ID");

                entity.Property(e => e.BusinessDate)
                    .HasColumnType("datetime")
                    .HasColumnName("BUSINESS_DATE");

                entity.Property(e => e.ShiftNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHIFT_NO");

                entity.Property(e => e.HoseId).HasColumnName("HOSE_ID");

                entity.Property(e => e.CloseMeterValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CLOSE_METER_VALUE");

                entity.Property(e => e.CloseMeterVolume)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CLOSE_METER_VOLUME");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GradeId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GRADE_ID");

                entity.Property(e => e.GradeName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("GRADE_NAME");

                entity.Property(e => e.GradeName2)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("GRADE_NAME2");

                entity.Property(e => e.OpenMeterValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("OPEN_METER_VALUE");

                entity.Property(e => e.OpenMeterVolume)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("OPEN_METER_VOLUME");

                entity.Property(e => e.PosId).HasColumnName("POS_ID");

                entity.Property(e => e.PosName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("POS_NAME");

                entity.Property(e => e.PumpId).HasColumnName("PUMP_ID");

                entity.Property(e => e.ShiftDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHIFT_DESC");

                entity.Property(e => e.ShiftId).HasColumnName("SHIFT_ID");

                entity.Property(e => e.TotalValue)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_VALUE");

                entity.Property(e => e.TotalVolume)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_VOLUME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InfPosFunction4>(entity =>
            {
                entity.HasKey(e => new { e.SiteId, e.BusinessDate, e.ShiftNo, e.JournalId })
                    .HasName("PK__INF_POS___C7535B98654645AF");

                entity.ToTable("INF_POS_FUNCTION4");

                entity.Property(e => e.SiteId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SITE_ID");

                entity.Property(e => e.BusinessDate)
                    .HasColumnType("datetime")
                    .HasColumnName("BUSINESS_DATE");

                entity.Property(e => e.ShiftNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SHIFT_NO");

                entity.Property(e => e.JournalId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("JOURNAL_ID");

                entity.Property(e => e.Billno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BILLNO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CustomerId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.JournalDate)
                    .HasColumnType("datetime")
                    .HasColumnName("JOURNAL_DATE");

                entity.Property(e => e.JournalStatus)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("JOURNAL_STATUS");

                entity.Property(e => e.LicNo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LIC_NO");

                entity.Property(e => e.MaxCardNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MAX_CARD_NUMBER");

                entity.Property(e => e.Miles)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MILES");

                entity.Property(e => e.PosId).HasColumnName("POS_ID");

                entity.Property(e => e.ShiftId).HasColumnName("SHIFT_ID");

                entity.Property(e => e.Taxinvno)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TAXINVNO");

                entity.Property(e => e.TotalDiscamt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_DISCAMT");

                entity.Property(e => e.TotalGoodsamt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_GOODSAMT");

                entity.Property(e => e.TotalPaidAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_PAID_AMT");

                entity.Property(e => e.TotalTaxamt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_TAXAMT");

                entity.Property(e => e.TransNo).HasColumnName("TRANS_NO");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.UserCardNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_CARD_NO");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("USER_ID");

                entity.Property(e => e.Username)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("USERNAME");
            });

            modelBuilder.Entity<InfPosFunction5>(entity =>
            {
                entity.HasKey(e => new { e.JournalId, e.Runno });

                entity.ToTable("INF_POS_FUNCTION5");

                entity.Property(e => e.JournalId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("JOURNAL_ID");

                entity.Property(e => e.Runno).HasColumnName("RUNNO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DeliveryId).HasColumnName("DELIVERY_ID");

                entity.Property(e => e.DeliveryType).HasColumnName("DELIVERY_TYPE");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscGroup)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DISC_GROUP");

                entity.Property(e => e.GoodsAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("GOODS_AMT");

                entity.Property(e => e.HoseId).HasColumnName("HOSE_ID");

                entity.Property(e => e.ItemCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_CODE");

                entity.Property(e => e.ItemName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NAME");

                entity.Property(e => e.ItemType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_TYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.PluNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PLU_NUMBER");

                entity.Property(e => e.PosId).HasColumnName("POS_ID");

                entity.Property(e => e.ProductCodesap)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCT_CODESAP");

                entity.Property(e => e.ProductSubType)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCT_SUB_TYPE")
                    .IsFixedLength(true);

                entity.Property(e => e.PumpId).HasColumnName("PUMP_ID");

                entity.Property(e => e.SellPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SELL_PRICE");

                entity.Property(e => e.SellQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SELL_QTY");

                entity.Property(e => e.ShiftId).HasColumnName("SHIFT_ID");

                entity.Property(e => e.TankId).HasColumnName("TANK_ID");

                entity.Property(e => e.TaxAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_AMT");

                entity.Property(e => e.TaxRate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TAX_RATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InfSapMm036>(entity =>
            {
                entity.HasKey(e => new { e.RcDocNo, e.RcBrnCode, e.RcCompCode, e.PoItem, e.MoveType });

                entity.ToTable("INF_SAP_MM036");

                entity.Property(e => e.RcDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_DOC_NO");

                entity.Property(e => e.RcBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_BRN_CODE");

                entity.Property(e => e.RcCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_COMP_CODE");

                entity.Property(e => e.PoItem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DelNumToSearch)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DEL_NUM_TO_SEARCH");

                entity.Property(e => e.DelivItem)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_ITEM");

                entity.Property(e => e.DelivItemToSearch).HasColumnName("DELIV_ITEM_TO_SEARCH");

                entity.Property(e => e.DelivNum)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_NUM");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MvtInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("MVT_IND");

                entity.Property(e => e.NoMoreGr)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("NO_MORE_GR");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RcDocDate)
                    .HasColumnType("date")
                    .HasColumnName("RC_DOC_DATE");

                entity.Property(e => e.RcPdId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RC_PD_ID");

                entity.Property(e => e.RcPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("RC_PD_NAME");

                entity.Property(e => e.RcSeqNo).HasColumnName("RC_SEQ_NO");

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm036Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM036_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapMm041>(entity =>
            {
                entity.HasKey(e => new { e.TfoDocNo, e.TfoBrnCode, e.TfoDocDate, e.TfoCompCode, e.TfoPdId, e.MoveType });

                entity.ToTable("INF_SAP_MM041");

                entity.Property(e => e.TfoDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_DOC_NO");

                entity.Property(e => e.TfoBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_BRN_CODE");

                entity.Property(e => e.TfoDocDate)
                    .HasColumnType("date")
                    .HasColumnName("TFO_DOC_DATE");

                entity.Property(e => e.TfoCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_COMP_CODE");

                entity.Property(e => e.TfoPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFO_PD_ID");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveMat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_MAT");

                entity.Property(e => e.MovePlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_PLANT");

                entity.Property(e => e.MoveStloc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_STLOC");

                entity.Property(e => e.MoveValType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_VAL_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.TfoBrnCodeTo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFO_BRN_CODE_TO");

                entity.Property(e => e.TfoLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_LOC_CODE");

                entity.Property(e => e.TfoPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TFO_PD_NAME");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm041Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM041_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.MovePlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_PLANT");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapMm042>(entity =>
            {
                entity.HasKey(e => new { e.TfiDocNo, e.TfiBrnCode, e.TfiDocDate, e.TfiCompCode, e.TfiPdId, e.MoveType });

                entity.ToTable("INF_SAP_MM042");

                entity.Property(e => e.TfiDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_DOC_NO");

                entity.Property(e => e.TfiBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_BRN_CODE");

                entity.Property(e => e.TfiDocDate)
                    .HasColumnType("date")
                    .HasColumnName("TFI_DOC_DATE");

                entity.Property(e => e.TfiCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_COMP_CODE");

                entity.Property(e => e.TfiPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFI_PD_ID");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveMat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_MAT");

                entity.Property(e => e.MovePlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_PLANT");

                entity.Property(e => e.MoveStloc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_STLOC");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.TfiLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_LOC_CODE");

                entity.Property(e => e.TfiPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TFI_PD_NAME");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm042Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM042_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapMm048>(entity =>
            {
                entity.HasKey(e => new { e.RtsDocNo, e.RtsBrnCode, e.RtsDocDate, e.RtsCompCode, e.RtsPdId, e.MoveType });

                entity.ToTable("INF_SAP_MM048");

                entity.Property(e => e.RtsDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RTS_DOC_NO");

                entity.Property(e => e.RtsBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RTS_BRN_CODE");

                entity.Property(e => e.RtsDocDate)
                    .HasColumnType("date")
                    .HasColumnName("RTS_DOC_DATE");

                entity.Property(e => e.RtsCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RTS_COMP_CODE");

                entity.Property(e => e.RtsPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RTS_PD_ID");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveReas)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_REAS");

                entity.Property(e => e.MvtInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("MVT_IND");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PoItem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.RtsLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RTS_LOC_CODE");

                entity.Property(e => e.RtsPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("RTS_PD_NAME");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm048Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM048_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapMm051>(entity =>
            {
                entity.HasKey(e => new { e.AjDocNo, e.AjBrnCode, e.AjDocDate, e.AjCompCode, e.AjPdId, e.MoveType });

                entity.ToTable("INF_SAP_MM051");

                entity.Property(e => e.AjDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AJ_DOC_NO");

                entity.Property(e => e.AjBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AJ_BRN_CODE");

                entity.Property(e => e.AjDocDate)
                    .HasColumnType("date")
                    .HasColumnName("AJ_DOC_DATE");

                entity.Property(e => e.AjCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AJ_COMP_CODE");

                entity.Property(e => e.AjPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("AJ_PD_ID");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.AjLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("AJ_LOC_CODE");

                entity.Property(e => e.AjPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("AJ_PD_NAME");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.ItemText)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_TEXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.UnloadPt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("UNLOAD_PT");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm051Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM051_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapMm052>(entity =>
            {
                entity.HasKey(e => new { e.GiDocNo, e.GiBrnCode, e.GiDocDate, e.GiCompCode, e.GiPdId, e.MoveType });

                entity.ToTable("INF_SAP_MM052");

                entity.Property(e => e.GiDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("GI_DOC_NO");

                entity.Property(e => e.GiBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("GI_BRN_CODE");

                entity.Property(e => e.GiDocDate)
                    .HasColumnType("date")
                    .HasColumnName("GI_DOC_DATE");

                entity.Property(e => e.GiCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("GI_COMP_CODE");

                entity.Property(e => e.GiPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("GI_PD_ID");

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GiLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("GI_LOC_CODE");

                entity.Property(e => e.GiPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("GI_PD_NAME");

                entity.Property(e => e.GlAccount)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_ACCOUNT");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Orderid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ORDERID");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zcoscenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ZCOSCENTER");

                entity.Property(e => e.Zcustomer)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ZCUSTOMER");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<InfSapMm052Log>(entity =>
            {
                entity.HasKey(e => new { e.BillOfLading, e.Plant, e.PstngDate, e.MoveType });

                entity.ToTable("INF_SAP_MM052_LOG");

                entity.Property(e => e.BillOfLading)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.MoveType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapOil02>(entity =>
            {
                entity.HasKey(e => new { e.PoNumber, e.RcDocNo, e.RcBrnCode, e.RcCompCode, e.PoItemNo });

                entity.ToTable("INF_SAP_OIL02");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.RcDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_DOC_NO");

                entity.Property(e => e.RcBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_BRN_CODE");

                entity.Property(e => e.RcCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RC_COMP_CODE");

                entity.Property(e => e.PoItemNo)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM_NO");

                entity.Property(e => e.ComfirmQty)
                    .HasColumnType("decimal(16, 3)")
                    .HasColumnName("COMFIRM_QTY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DelDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DEL_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DelTime)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("DEL_TIME")
                    .IsFixedLength(true);

                entity.Property(e => e.IsDeleted)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_DELETED");

                entity.Property(e => e.MatNr)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MAT_NR");

                entity.Property(e => e.MatTemp)
                    .HasColumnType("decimal(16, 1)")
                    .HasColumnName("MAT_TEMP");

                entity.Property(e => e.MatTempUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("MAT_TEMP_UOM");

                entity.Property(e => e.Mcf)
                    .HasColumnType("decimal(16, 8)")
                    .HasColumnName("MCF");

                entity.Property(e => e.PlantNr)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT_NR");

                entity.Property(e => e.RcDocDate)
                    .HasColumnType("date")
                    .HasColumnName("RC_DOC_DATE");

                entity.Property(e => e.RcPdId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("RC_PD_ID");

                entity.Property(e => e.RcPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("RC_PD_NAME");

                entity.Property(e => e.RcSeqNo).HasColumnName("RC_SEQ_NO");

                entity.Property(e => e.SlocNr)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SLOC_NR");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TestDen)
                    .HasColumnType("decimal(16, 1)")
                    .HasColumnName("TEST_DEN");

                entity.Property(e => e.TestTemp)
                    .HasColumnType("decimal(16, 8)")
                    .HasColumnName("TEST_TEMP");

                entity.Property(e => e.TestTempUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("TEST_TEMP_UOM");

                entity.Property(e => e.TrUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("TR_UOM");
            });

            modelBuilder.Entity<InfSapOil02Log>(entity =>
            {
                entity.HasKey(e => new { e.PoNumber, e.PlantNr })
                    .HasName("PK_INF_INF_SAP_OIL02_LOG");

                entity.ToTable("INF_SAP_OIL02_LOG");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PlantNr)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT_NR");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DelDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DEL_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapZmmint01>(entity =>
            {
                entity.HasKey(e => new { e.SalDocNo, e.SalBrnCode, e.SalDocDate, e.SalCompCode, e.SalPdId, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT01");

                entity.Property(e => e.SalDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAL_DOC_NO");

                entity.Property(e => e.SalBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAL_BRN_CODE");

                entity.Property(e => e.SalDocDate)
                    .HasColumnType("date")
                    .HasColumnName("SAL_DOC_DATE");

                entity.Property(e => e.SalCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAL_COMP_CODE");

                entity.Property(e => e.SalPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SAL_PD_ID");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Customer)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER");

                entity.Property(e => e.DistributeChannel)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("DISTRIBUTE_CHANNEL");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.SalLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAL_LOC_CODE");

                entity.Property(e => e.SalPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SAL_PD_NAME");

                entity.Property(e => e.SaleOrg)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SALE_ORG");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<InfSapZmmint01Log>(entity =>
            {
                entity.HasKey(e => new { e.DocumentNo, e.Plant, e.PostingDate, e.CompCode, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT01_LOG");

                entity.Property(e => e.DocumentNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.CompCode)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapZmmint02>(entity =>
            {
                entity.HasKey(e => new { e.TfoDocNo, e.TfoBrnCode, e.TfoDocDate, e.TfoCompCode, e.TfoPdId, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT02");

                entity.Property(e => e.TfoDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_DOC_NO");

                entity.Property(e => e.TfoBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_BRN_CODE");

                entity.Property(e => e.TfoDocDate)
                    .HasColumnType("date")
                    .HasColumnName("TFO_DOC_DATE");

                entity.Property(e => e.TfoCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_COMP_CODE");

                entity.Property(e => e.TfoPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFO_PD_ID");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.RefDocTfOut)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_TF_OUT");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TargetPlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("TARGET_PLANT");

                entity.Property(e => e.TfoBrnCodeTo)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFO_BRN_CODE_TO");

                entity.Property(e => e.TfoLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFO_LOC_CODE");

                entity.Property(e => e.TfoPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TFO_PD_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<InfSapZmmint02Log>(entity =>
            {
                entity.HasKey(e => new { e.DocumentNo, e.Plant, e.PostingDate, e.CompCode, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT02_LOG");

                entity.Property(e => e.DocumentNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.CompCode)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.TargetPlant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("TARGET_PLANT");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapZmmint03>(entity =>
            {
                entity.HasKey(e => new { e.TfiDocNo, e.TfiBrnCode, e.TfiDocDate, e.TfiCompCode, e.TfiPdId, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT03");

                entity.Property(e => e.TfiDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_DOC_NO");

                entity.Property(e => e.TfiBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_BRN_CODE");

                entity.Property(e => e.TfiDocDate)
                    .HasColumnType("date")
                    .HasColumnName("TFI_DOC_DATE");

                entity.Property(e => e.TfiCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_COMP_CODE");

                entity.Property(e => e.TfiPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TFI_PD_ID");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.RefDocTfIn)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_TF_IN");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TfiLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TFI_LOC_CODE");

                entity.Property(e => e.TfiPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TFI_PD_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<InfSapZmmint03Log>(entity =>
            {
                entity.HasKey(e => new { e.DocumentNo, e.Plant, e.PostingDate, e.CompCode, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT03_LOG");

                entity.Property(e => e.DocumentNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.CompCode)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.RefDocTfIn)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_TF_IN");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InfSapZmmint04>(entity =>
            {
                entity.HasKey(e => new { e.InxDocNo, e.InxBrnCode, e.InxDocDate, e.InxCompCode, e.InxPdId, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT04");

                entity.Property(e => e.InxDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("INX_DOC_NO");

                entity.Property(e => e.InxBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("INX_BRN_CODE");

                entity.Property(e => e.InxDocDate)
                    .HasColumnType("date")
                    .HasColumnName("INX_DOC_DATE");

                entity.Property(e => e.InxCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("INX_COMP_CODE");

                entity.Property(e => e.InxPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("INX_PD_ID");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.InxLocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("INX_LOC_CODE");

                entity.Property(e => e.InxPdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("INX_PD_NAME");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");

                entity.Property(e => e.Vendor)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VENDOR");
            });

            modelBuilder.Entity<InfSapZmmint04Log>(entity =>
            {
                entity.HasKey(e => new { e.DocumentNo, e.Plant, e.PostingDate, e.CompCode, e.MovementType });

                entity.ToTable("INF_SAP_ZMMINT04_LOG");

                entity.Property(e => e.DocumentNo)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.Plant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.CompCode)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.Status)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("STATUS")
                    .HasDefaultValueSql("('P')")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<InvAdjustDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo, e.SeqNo });

                entity.ToTable("INV_ADJUST_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_QTY");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitCost)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_COST");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");
            });

            modelBuilder.Entity<InvAdjustHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo });

                entity.ToTable("INV_ADJUST_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvAdjustRequestDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_ADJUST_REQUEST_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvAdjustRequestHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_ADJUST_REQUEST_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvAuditDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_AUDIT_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.AdjustQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_QTY");

                entity.Property(e => e.BalanceQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE_QTY");

                entity.Property(e => e.DiffQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DIFF_QTY");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.NoadjQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NOADJ_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitCost)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_COST");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");
            });

            modelBuilder.Entity<InvAuditHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_AUDIT_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.AuditSeq).HasColumnName("AUDIT_SEQ");

                entity.Property(e => e.AuditYear).HasColumnName("AUDIT_YEAR");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvDeliveryCtrlDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_DELIVERY_CTRL_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CtrlApi)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_API")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlApiDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_API_DESC");

                entity.Property(e => e.CtrlApiFinish)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CTRL_API_FINISH");

                entity.Property(e => e.CtrlApiStart)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CTRL_API_START");

                entity.Property(e => e.CtrlEthanol)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_ETHANOL")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlEthanolQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CTRL_ETHANOL_QTY");

                entity.Property(e => e.CtrlFull)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_FULL")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlFullContact)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_FULL_CONTACT");

                entity.Property(e => e.CtrlFullLt).HasColumnName("CTRL_FULL_LT");

                entity.Property(e => e.CtrlFullMm).HasColumnName("CTRL_FULL_MM");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");
            });

            modelBuilder.Entity<InvDeliveryCtrlHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_DELIVERY_CTRL_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CarNo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CAR_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CtrlCorrect)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_CORRECT")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlCorrectOther)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_CORRECT_OTHER");

                entity.Property(e => e.CtrlCorrectReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_CORRECT_REASON_DESC");

                entity.Property(e => e.CtrlCorrectReasonId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_CORRECT_REASON_ID");

                entity.Property(e => e.CtrlDoc)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_DOC")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlDocDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_DOC_DESC");

                entity.Property(e => e.CtrlOntime)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_ONTIME")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlOntimeLate).HasColumnName("CTRL_ONTIME_LATE");

                entity.Property(e => e.CtrlSeal)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_SEAL")
                    .IsFixedLength(true);

                entity.Property(e => e.CtrlSealFinish).HasColumnName("CTRL_SEAL_FINISH");

                entity.Property(e => e.CtrlSealStart).HasColumnName("CTRL_SEAL_START");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.RealDate)
                    .HasColumnType("datetime")
                    .HasColumnName("REAL_DATE");

                entity.Property(e => e.ReceiveNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("RECEIVE_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.WhId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WH_ID");

                entity.Property(e => e.WhName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("WH_NAME");
            });

            modelBuilder.Entity<InvReceiveProdDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo, e.SeqNo });

                entity.ToTable("INV_RECEIVE_PROD_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.Density)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DENSITY");

                entity.Property(e => e.DensityBase)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DENSITY_BASE");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscHdAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT");

                entity.Property(e => e.DiscHdAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT_CUR");

                entity.Property(e => e.IsFree).HasColumnName("IS_FREE");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.ItemRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_REMAIN");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.PoQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("PO_QTY");

                entity.Property(e => e.ReturnQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RETURN_QTY");

                entity.Property(e => e.ReturnStock)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RETURN_STOCK");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SumItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT");

                entity.Property(e => e.SumItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.Temperature)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TEMPERATURE");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UnitPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE_CUR");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");

                entity.Property(e => e.WeightPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WEIGHT_PRICE");

                entity.Property(e => e.WeightQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WEIGHT_QTY");
            });

            modelBuilder.Entity<InvReceiveProdHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo });

                entity.ToTable("INV_RECEIVE_PROD_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.DeliveryNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DELIVERY_NO");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscRate)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DISC_RATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DueDate)
                    .HasColumnType("date")
                    .HasColumnName("DUE_DATE");

                entity.Property(e => e.EtaxAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ETAX_AMT");

                entity.Property(e => e.EtaxAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ETAX_AMT_CUR");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.InvAddrId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("INV_ADDR_ID");

                entity.Property(e => e.InvAddress)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("INV_ADDRESS");

                entity.Property(e => e.InvDate)
                    .HasColumnType("date")
                    .HasColumnName("INV_DATE");

                entity.Property(e => e.InvNo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("INV_NO");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.PayAddrId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_ADDR_ID");

                entity.Property(e => e.PayAddress)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PAY_ADDRESS");

                entity.Property(e => e.PoDate)
                    .HasColumnType("date")
                    .HasColumnName("PO_DATE");

                entity.Property(e => e.PoNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NO");

                entity.Property(e => e.PoTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_TYPE_ID");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.ShippingAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SHIPPING_AMT");

                entity.Property(e => e.ShippingAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SHIPPING_AMT_CUR");

                entity.Property(e => e.Source)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SOURCE");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SupCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SUP_CODE");

                entity.Property(e => e.SupName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("SUP_NAME");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<InvReceiveProdLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("INV_RECEIVE_PROD_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");
            });

            modelBuilder.Entity<InvRequestDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocTypeId, e.DocNo, e.SeqNo });

                entity.ToTable("INV_REQUEST_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocTypeId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_ID");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvRequestHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocTypeId, e.DocNo })
                    .HasName("PK_INV_REQUEST_HD_1");

                entity.ToTable("INV_REQUEST_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocTypeId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_ID");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnCodeTo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_TO");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.BrnNameTo)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_TO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvReturnOilDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_RETURN_OIL_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_QTY");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvReturnOilHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_RETURN_OIL_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeTo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_TO");

                entity.Property(e => e.BrnNameTo)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_TO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PoNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NO");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvReturnSupDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_RETURN_SUP_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_QTY");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvReturnSupHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_RETURN_SUP_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EtlLotNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ETL_LOT_NO");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PoNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NO");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SupCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SUP_CODE");

                entity.Property(e => e.SupName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SUP_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvStockDaily>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.StockDate, e.PdId, e.UnitId });

                entity.ToTable("INV_STOCK_DAILY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.StockDate)
                    .HasColumnType("date")
                    .HasColumnName("STOCK_DATE");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.Adjust)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST");

                entity.Property(e => e.Audit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AUDIT");

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.FreeOut)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("FREE_OUT");

                entity.Property(e => e.ReceiveIn)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RECEIVE_IN");

                entity.Property(e => e.Remain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REMAIN");

                entity.Property(e => e.ReturnOut)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RETURN_OUT");

                entity.Property(e => e.SaleOut)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SALE_OUT");

                entity.Property(e => e.TransferIn)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TRANSFER_IN");

                entity.Property(e => e.TransferOut)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TRANSFER_OUT");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.WithdrawOut)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("WITHDRAW_OUT");
            });

            modelBuilder.Entity<InvStockMonthly>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.YearNo, e.MonthNo, e.PdId, e.UnitId });

                entity.ToTable("INV_STOCK_MONTHLY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.YearNo).HasColumnName("YEAR_NO");

                entity.Property(e => e.MonthNo).HasColumnName("MONTH_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.Balance)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BALANCE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");
            });

            modelBuilder.Entity<InvSupplyRequestDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_SUPPLY_REQUEST_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.RefQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_QTY");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvSupplyRequestHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_SUPPLY_REQUEST_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RequestDate)
                    .HasColumnType("date")
                    .HasColumnName("REQUEST_DATE");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvTraninDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_TRANIN_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvTraninHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_TRANIN_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDate)
                    .HasColumnType("date")
                    .HasColumnName("REF_DATE");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvTranoutDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_TRANOUT_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_QTY");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvTranoutHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_TRANOUT_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.BrnCodeTo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_TO");

                entity.Property(e => e.BrnNameTo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_TO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvTranoutLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("INV_TRANOUT_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");
            });

            modelBuilder.Entity<InvUnuseDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_UNUSE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvUnuseHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_UNUSE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .HasDefaultValueSql("('N')")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<InvWithdrawDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("INV_WITHDRAW_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");
            });

            modelBuilder.Entity<InvWithdrawHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("INV_WITHDRAW_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.UseBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("USE_BRN_CODE");

                entity.Property(e => e.UseBrnName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("USE_BRN_NAME");
            });

            modelBuilder.Entity<InvWithdrawLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("INV_WITHDRAW_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");
            });

            modelBuilder.Entity<LogError>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("LOG_ERROR");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Host)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("HOST");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.Message)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE");

                entity.Property(e => e.Method)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("METHOD");

                entity.Property(e => e.Path)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PATH");

                entity.Property(e => e.StackTrace)
                    .IsUnicode(false)
                    .HasColumnName("STACK_TRACE");
            });

            modelBuilder.Entity<LogLogin>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("LOG_LOGIN");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("IP_ADDRESS");
            });

            modelBuilder.Entity<LogStoreProcedure>(entity =>
            {
                entity.HasKey(e => e.SeqNo);

                entity.ToTable("LOG_STORE_PROCEDURE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.ErrorMsg)
                    .IsUnicode(false)
                    .HasColumnName("ERROR_MSG");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");

                entity.Property(e => e.StoreProcedureName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("STORE_PROCEDURE_NAME");
            });

            modelBuilder.Entity<LogVatnoMaxme>(entity =>
            {
                entity.ToTable("LOG_VATNO_MAXME");

                entity.Property(e => e.Id).HasColumnName("ID");

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

            modelBuilder.Entity<MasBank>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BankCode, e.AccountNo })
                    .HasName("PK_MAS_BANK_1");

                entity.ToTable("MAS_BANK");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BankCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BANK_CODE");

                entity.Property(e => e.AccountNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCOUNT_NO");

                entity.Property(e => e.BankName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BANK_NAME");

                entity.Property(e => e.BankStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BANK_STATUS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBillerInfo>(entity =>
            {
                entity.HasKey(e => e.CompanyCode);

                entity.ToTable("MAS_BILLER_INFO");

                entity.Property(e => e.CompanyCode)
                    .HasMaxLength(10)
                    .HasColumnName("COMPANY_CODE");

                entity.Property(e => e.ApiKey)
                    .HasMaxLength(255)
                    .HasColumnName("API_KEY");

                entity.Property(e => e.BankAccount)
                    .HasMaxLength(50)
                    .HasColumnName("BANK_ACCOUNT");

                entity.Property(e => e.BillerId)
                    .HasMaxLength(50)
                    .HasColumnName("BILLER_ID");

                entity.Property(e => e.Business)
                    .HasMaxLength(255)
                    .HasColumnName("BUSINESS");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("COMPANY_NAME");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Prefix).HasMaxLength(10);

                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .HasColumnName("REMARK");

                entity.Property(e => e.SecretKey)
                    .HasMaxLength(255)
                    .HasColumnName("SECRET_KEY");

                entity.Property(e => e.Suffix)
                    .HasMaxLength(10)
                    .HasColumnName("SUFFIX");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("TYPE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBillerInquiryScb>(entity =>
            {
                entity.ToTable("MAS_BILLER_INQUIRY_SCB");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Amount)
                    .HasMaxLength(255)
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.Billerid)
                    .HasMaxLength(50)
                    .HasColumnName("BILLERID");

                entity.Property(e => e.Billpaymentref1)
                    .HasMaxLength(255)
                    .HasColumnName("BILLPAYMENTREF1");

                entity.Property(e => e.Billpaymentref2)
                    .HasMaxLength(255)
                    .HasColumnName("BILLPAYMENTREF2");

                entity.Property(e => e.Billpaymentref3)
                    .HasMaxLength(255)
                    .HasColumnName("BILLPAYMENTREF3");

                entity.Property(e => e.Channelcode)
                    .HasMaxLength(255)
                    .HasColumnName("CHANNELCODE");

                entity.Property(e => e.Createby)
                    .HasMaxLength(255)
                    .HasColumnName("CREATEBY");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATEDATE");

                entity.Property(e => e.Currencycode)
                    .HasMaxLength(255)
                    .HasColumnName("CURRENCYCODE");

                entity.Property(e => e.Equivalentamount)
                    .HasMaxLength(255)
                    .HasColumnName("EQUIVALENTAMOUNT");

                entity.Property(e => e.Equivalentcurrencycode)
                    .HasMaxLength(255)
                    .HasColumnName("EQUIVALENTCURRENCYCODE");

                entity.Property(e => e.Eventcode)
                    .HasMaxLength(255)
                    .HasColumnName("EVENTCODE");

                entity.Property(e => e.Exchangerate)
                    .HasMaxLength(255)
                    .HasColumnName("EXCHANGERATE");

                entity.Property(e => e.Fasteasyslipnumber)
                    .HasMaxLength(255)
                    .HasColumnName("FASTEASYSLIPNUMBER");

                entity.Property(e => e.Partnertransactionid)
                    .HasMaxLength(255)
                    .HasColumnName("PARTNERTRANSACTIONID");

                entity.Property(e => e.Payeeaccountnumber)
                    .HasMaxLength(255)
                    .HasColumnName("PAYEEACCOUNTNUMBER");

                entity.Property(e => e.Payeename)
                    .HasMaxLength(255)
                    .HasColumnName("PAYEENAME");

                entity.Property(e => e.Payeeproxyid)
                    .HasMaxLength(255)
                    .HasColumnName("PAYEEPROXYID");

                entity.Property(e => e.Payeeproxytype)
                    .HasMaxLength(255)
                    .HasColumnName("PAYEEPROXYTYPE");

                entity.Property(e => e.Payeraccountnumber)
                    .HasMaxLength(255)
                    .HasColumnName("PAYERACCOUNTNUMBER");

                entity.Property(e => e.Payername)
                    .HasMaxLength(255)
                    .HasColumnName("PAYERNAME");

                entity.Property(e => e.Payerproxyid)
                    .HasMaxLength(255)
                    .HasColumnName("PAYERPROXYID");

                entity.Property(e => e.Payerproxytype)
                    .HasMaxLength(255)
                    .HasColumnName("PAYERPROXYTYPE");

                entity.Property(e => e.Receivingbankcode)
                    .HasMaxLength(255)
                    .HasColumnName("RECEIVINGBANKCODE");

                entity.Property(e => e.Reverseflag)
                    .HasMaxLength(255)
                    .HasColumnName("REVERSEFLAG");

                entity.Property(e => e.Sendingbankcode)
                    .HasMaxLength(255)
                    .HasColumnName("SENDINGBANKCODE");

                entity.Property(e => e.Tepacode)
                    .HasMaxLength(255)
                    .HasColumnName("TEPACODE");

                entity.Property(e => e.Transactiondateandtime)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSACTIONDATEANDTIME");

                entity.Property(e => e.Transactionid)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSACTIONID");

                entity.Property(e => e.Transactiontype)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSACTIONTYPE");
            });

            modelBuilder.Entity<MasBillerTransactionActionScb>(entity =>
            {
                entity.ToTable("MAS_BILLER_TRANSACTION_ACTION_SCB");

                entity.Property(e => e.Action)
                    .HasMaxLength(255)
                    .HasColumnName("ACTION");

                entity.Property(e => e.Billerid)
                    .HasMaxLength(50)
                    .HasColumnName("BILLERID");

                entity.Property(e => e.Createby)
                    .HasMaxLength(50)
                    .HasColumnName("CREATEBY");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATEDATE");

                entity.Property(e => e.Subname)
                    .HasMaxLength(255)
                    .HasColumnName("SUBNAME");

                entity.Property(e => e.Transref)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSREF");

                entity.Property(e => e.Type)
                    .HasMaxLength(255)
                    .HasColumnName("TYPE");

                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .HasColumnName("VALUE");
            });

            modelBuilder.Entity<MasBillerTransactionScb>(entity =>
            {
                entity.ToTable("MAS_BILLER_TRANSACTION_SCB");

                entity.Property(e => e.Amount)
                    .HasMaxLength(255)
                    .HasColumnName("AMOUNT");

                entity.Property(e => e.Billerid)
                    .HasMaxLength(50)
                    .HasColumnName("BILLERID");

                entity.Property(e => e.Countrycode)
                    .HasMaxLength(255)
                    .HasColumnName("COUNTRYCODE");

                entity.Property(e => e.Createby)
                    .HasMaxLength(50)
                    .HasColumnName("CREATEBY");

                entity.Property(e => e.Createdate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATEDATE");

                entity.Property(e => e.Receivingbank)
                    .HasMaxLength(255)
                    .HasColumnName("RECEIVINGBANK");

                entity.Property(e => e.Ref1)
                    .HasMaxLength(255)
                    .HasColumnName("REF1");

                entity.Property(e => e.Ref2)
                    .HasMaxLength(255)
                    .HasColumnName("REF2");

                entity.Property(e => e.Ref3)
                    .HasMaxLength(255)
                    .HasColumnName("REF3");

                entity.Property(e => e.Sendingbank)
                    .HasMaxLength(255)
                    .HasColumnName("SENDINGBANK");

                entity.Property(e => e.Transdate)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSDATE");

                entity.Property(e => e.Transref)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSREF");

                entity.Property(e => e.Transtime)
                    .HasMaxLength(255)
                    .HasColumnName("TRANSTIME");
            });

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

                entity.Property(e => e.PosCount).HasColumnName("POS_COUNT");

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

            modelBuilder.Entity<MasBranchCalibrate>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.TankId, e.SeqNo });

                entity.ToTable("MAS_BRANCH_CALIBRATE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.TankId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TANK_ID");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.LevelNo)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("LEVEL_NO");

                entity.Property(e => e.LevelUnit)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LEVEL_UNIT");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.TankQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TANK_QTY");

                entity.Property(e => e.TankStart)
                    .HasColumnType("date")
                    .HasColumnName("TANK_START");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBranchConfig>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode });

                entity.ToTable("MAS_BRANCH_CONFIG");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IsLockMeter)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_LOCK_METER")
                    .IsFixedLength(true);

                entity.Property(e => e.IsLockSlip)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_LOCK_SLIP")
                    .IsFixedLength(true);

                entity.Property(e => e.IsPos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_POS")
                    .IsFixedLength(true);

                entity.Property(e => e.ReportTaxType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("REPORT_TAX_TYPE");

                entity.Property(e => e.Trader)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TRADER");

                entity.Property(e => e.TraderPosition)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TRADER_POSITION");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBranchConfigDesc>(entity =>
            {
                entity.HasKey(e => e.ItemNo);

                entity.ToTable("MAS_BRANCH_CONFIG_DESC");

                entity.Property(e => e.ItemNo)
                    .ValueGeneratedNever()
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.ConfigId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CONFIG_ID");

                entity.Property(e => e.ConfigName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CONFIG_NAME");

                entity.Property(e => e.IsLockDate)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_LOCK_DATE")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<MasBranchDisp>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DispId });

                entity.ToTable("MAS_BRANCH_DISP");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DispId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DISP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DispStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DISP_STATUS");

                entity.Property(e => e.HoseId).HasColumnName("HOSE_ID");

                entity.Property(e => e.MeterMax).HasColumnName("METER_MAX");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.SerialNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SERIAL_NO");

                entity.Property(e => e.TankId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TANK_ID");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBranchLog>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.SeqNo });

                entity.ToTable("MAS_BRANCH_LOG");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");
            });

            modelBuilder.Entity<MasBranchMid>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.MidNo });

                entity.ToTable("MAS_BRANCH_MID");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.MidNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MID_NO");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATE_DATE");

                entity.Property(e => e.CreateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATE_USER");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATE_DATE");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATE_USER");
            });

            modelBuilder.Entity<MasBranchPeriod>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.PeriodNo });

                entity.ToTable("MAS_BRANCH_PERIOD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.PeriodNo).HasColumnName("PERIOD_NO");

                entity.Property(e => e.TimeFinish)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIME_FINISH");

                entity.Property(e => e.TimeStart)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TIME_START");
            });

            modelBuilder.Entity<MasBranchTank>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.TankId });

                entity.ToTable("MAS_BRANCH_TANK");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.TankId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TANK_ID");

                entity.Property(e => e.Capacity)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CAPACITY");

                entity.Property(e => e.CapacityMin)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CAPACITY_MIN");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.TankStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TANK_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasBranchTax>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.TaxId });

                entity.ToTable("MAS_BRANCH_TAX");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.TaxId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TAX_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.TaxAmt)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("TAX_AMT");

                entity.Property(e => e.TaxName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("TAX_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCenterLog>(entity =>
            {
                entity.ToTable("MAS_CENTER_LOG");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Body).HasColumnName("BODY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Ipaddress)
                    .HasMaxLength(100)
                    .HasColumnName("IPADDRESS");

                entity.Property(e => e.Method)
                    .HasMaxLength(50)
                    .HasColumnName("METHOD");

                entity.Property(e => e.Path).HasColumnName("PATH");

                entity.Property(e => e.Type)
                    .HasMaxLength(20)
                    .HasColumnName("TYPE");
            });

            modelBuilder.Entity<MasCode>(entity =>
            {
                entity.HasKey(e => e.CodeId)
                    .HasName("PK_MAS_PATTERN");

                entity.ToTable("MAS_CODE");

                entity.Property(e => e.CodeId)
                    .HasColumnName("CODE_ID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CODE");

                entity.Property(e => e.CodeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CODE_DESC");

                entity.Property(e => e.CodeType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CODE_TYPE");
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

                entity.Property(e => e.CompSname)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_SNAME");

                entity.Property(e => e.CompStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_STATUS");

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

                entity.Property(e => e.DistrictEn)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT_EN");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.IsLogin)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_LOGIN")
                    .IsFixedLength(true);

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

                entity.Property(e => e.ProvinceEn)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PROVINCE_EN");

                entity.Property(e => e.RegisterId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REGISTER_ID");

                entity.Property(e => e.SubDistrict)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUB_DISTRICT");

                entity.Property(e => e.SubDistrictEn)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUB_DISTRICT_EN");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCompanyCar>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.LicensePlate });

                entity.ToTable("MAS_COMPANY_CAR");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.CarRemark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CAR_REMARK");

                entity.Property(e => e.CarStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CAR_STATUS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCompanyMapping>(entity =>
            {
                entity.HasKey(e => new { e.CompanyCode, e.ComLegCode });

                entity.ToTable("MAS_COMPANY_MAPPING");

                entity.Property(e => e.CompanyCode)
                    .HasMaxLength(10)
                    .HasColumnName("COMPANY_CODE");

                entity.Property(e => e.ComLegCode)
                    .HasMaxLength(10)
                    .HasColumnName("COM_LEG_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasControl>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.CtrlCode });

                entity.ToTable("MAS_CONTROL");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.CtrlCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CtrlValue)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CTRL_VALUE");

                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCostCenter>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode });

                entity.ToTable("MAS_COST_CENTER");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.BrnName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME");

                entity.Property(e => e.BrnStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BRN_STATUS");

                entity.Property(e => e.CostCenter)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COST_CENTER");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.MapBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_BRN_CODE");

                entity.Property(e => e.ProfitCenter)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PROFIT_CENTER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasCustomer>(entity =>
            {
                entity.HasKey(e => e.CustCode);

                entity.ToTable("MAS_CUSTOMER");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ACCOUNT_ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.BillType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("BILL_TYPE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.ContactName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CONTACT_NAME");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COUNTRY_CODE");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("COUNTRY_NAME");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CreditLimit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CREDIT_LIMIT");

                entity.Property(e => e.CreditTerm).HasColumnName("CREDIT_TERM");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.CustPrefix)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_PREFIX");

                entity.Property(e => e.CustStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CUST_STATUS");

                entity.Property(e => e.District)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT");

                entity.Property(e => e.DueType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DUE_TYPE");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.MapCustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_CUST_CODE");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
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

                entity.Property(e => e.ProvName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PROV_NAME");

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

            modelBuilder.Entity<MasCustomerCar>(entity =>
            {
                entity.HasKey(e => new { e.CustCode, e.LicensePlate });

                entity.ToTable("MAS_CUSTOMER_CAR");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.CarStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CAR_STATUS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasDensity>(entity =>
            {
                entity.HasKey(e => e.CompCode);

                entity.ToTable("MAS_DENSITY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CalculateType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CALCULATE_TYPE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DensityBase)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DENSITY_BASE");

                entity.Property(e => e.DensityDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DENSITY_DESC");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasDocPattern>(entity =>
            {
                entity.HasKey(e => e.DocId);

                entity.ToTable("MAS_DOC_PATTERN");

                entity.Property(e => e.DocId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_ID");

                entity.Property(e => e.DocType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.Pattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PATTERN");
            });

            modelBuilder.Entity<MasDocPatternDt>(entity =>
            {
                entity.HasKey(e => new { e.DocId, e.SeqNo });

                entity.ToTable("MAS_DOC_PATTERN_DT");

                entity.Property(e => e.DocId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_ID");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.DocCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_CODE");

                entity.Property(e => e.DocValue)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_VALUE");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ITEM_ID")
                    .HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<MasDocumentType>(entity =>
            {
                entity.HasKey(e => e.DocTypeId);

                entity.ToTable("MAS_DOCUMENT_TYPE");

                entity.Property(e => e.DocTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocTypeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_DESC");

                entity.Property(e => e.DocTypeName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_NAME");

                entity.Property(e => e.DocTypeStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasEmployee>(entity =>
            {
                entity.HasKey(e => e.EmpCode);

                entity.ToTable("MAS_EMPLOYEE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.BirthDate)
                    .HasColumnType("date")
                    .HasColumnName("BIRTH_DATE");

                entity.Property(e => e.BkAccount)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BK_ACCOUNT");

                entity.Property(e => e.BkCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BK_CODE");

                entity.Property(e => e.BkName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("BK_NAME");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CodeDev)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CODE_DEV");

                entity.Property(e => e.DepartDate)
                    .HasColumnType("date")
                    .HasColumnName("DEPART_DATE");

                entity.Property(e => e.Dteupd)
                    .HasColumnType("date")
                    .HasColumnName("DTEUPD");

                entity.Property(e => e.EmployDate)
                    .HasColumnType("date")
                    .HasColumnName("EMPLOY_DATE");

                entity.Property(e => e.EmptypeCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("EMPTYPE_CODE");

                entity.Property(e => e.EmptypeDescThai)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("EMPTYPE_DESC_THAI");

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("GENDER")
                    .IsFixedLength(true);

                entity.Property(e => e.JlCode).HasColumnName("JL_CODE");

                entity.Property(e => e.JlDescThai)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("JL_DESC_THAI");

                entity.Property(e => e.Mstatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MSTATUS");

                entity.Property(e => e.OrgnameThai)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ORGNAME_THAI");

                entity.Property(e => e.PersonFnameEng)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PERSON_FNAME_ENG");

                entity.Property(e => e.PersonFnameThai)
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("PERSON_FNAME_THAI");

                entity.Property(e => e.PersonLnameEng)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PERSON_LNAME_ENG");

                entity.Property(e => e.PersonLnameThai)
                    .HasMaxLength(400)
                    .IsUnicode(false)
                    .HasColumnName("PERSON_LNAME_THAI");

                entity.Property(e => e.PlCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PL_CODE");

                entity.Property(e => e.PlDescTha)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PL_DESC_THA");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_CODE");

                entity.Property(e => e.PostnameThai)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("POSTNAME_THAI");

                entity.Property(e => e.PrefixEng)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PREFIX_ENG");

                entity.Property(e => e.PrefixThai)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PREFIX_THAI");

                entity.Property(e => e.ProbationEndDate)
                    .HasColumnType("date")
                    .HasColumnName("PROBATION_END_DATE");

                entity.Property(e => e.SocialSecurityId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SOCIAL_SECURITY_ID");

                entity.Property(e => e.TaxId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TAX_ID");

                entity.Property(e => e.WorkStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WORK_STATUS");

                entity.Property(e => e.WorkplaceThai)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("WORKPLACE_THAI");
            });

            modelBuilder.Entity<MasEmployeeLevel>(entity =>
            {
                entity.HasKey(e => e.EmpId);

                entity.ToTable("MAS_EMPLOYEE_LEVEL");

                entity.Property(e => e.EmpId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMP_ID");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(30)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.BrnName)
                    .HasMaxLength(400)
                    .HasColumnName("BRN_NAME");

                entity.Property(e => e.CompanycodeLevel1)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL1");

                entity.Property(e => e.CompanycodeLevel2)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL2");

                entity.Property(e => e.CompanycodeLevel3)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL3");

                entity.Property(e => e.CompanycodeLevel4)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL4");

                entity.Property(e => e.CompanycodeLevel5)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL5");

                entity.Property(e => e.CompanycodeLevel6)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL6");

                entity.Property(e => e.CompanycodeLevel7)
                    .HasMaxLength(100)
                    .HasColumnName("COMPANYCODE_LEVEL7");

                entity.Property(e => e.CompanynameLevel1)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL1");

                entity.Property(e => e.CompanynameLevel2)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL2");

                entity.Property(e => e.CompanynameLevel3)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL3");

                entity.Property(e => e.CompanynameLevel4)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL4");

                entity.Property(e => e.CompanynameLevel5)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL5");

                entity.Property(e => e.CompanynameLevel6)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL6");

                entity.Property(e => e.CompanynameLevel7)
                    .HasMaxLength(300)
                    .HasColumnName("COMPANYNAME_LEVEL7");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EmpEmail)
                    .HasMaxLength(100)
                    .HasColumnName("EMP_EMAIL");

                entity.Property(e => e.EmpLastnameEn)
                    .HasMaxLength(300)
                    .HasColumnName("EMP_LASTNAME_EN");

                entity.Property(e => e.EmpLastnameTh)
                    .HasMaxLength(300)
                    .HasColumnName("EMP_LASTNAME_TH");

                entity.Property(e => e.EmpNameEn)
                    .HasMaxLength(300)
                    .HasColumnName("EMP_NAME_EN");

                entity.Property(e => e.EmpNameTh)
                    .HasMaxLength(300)
                    .HasColumnName("EMP_NAME_TH");

                entity.Property(e => e.EmpSex)
                    .HasMaxLength(50)
                    .HasColumnName("EMP_SEX");

                entity.Property(e => e.EmpTitle)
                    .HasMaxLength(10)
                    .HasColumnName("EMP_TITLE")
                    .IsFixedLength(true);

                entity.Property(e => e.Head1Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HEAD1_ID");

                entity.Property(e => e.Head1LastnameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD1_LASTNAME_EN");

                entity.Property(e => e.Head1LastnameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD1_LASTNAME_TH");

                entity.Property(e => e.Head1NameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD1_NAME_EN");

                entity.Property(e => e.Head1NameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD1_NAME_TH");

                entity.Property(e => e.Head2Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HEAD2_ID");

                entity.Property(e => e.Head2LastnameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD2_LASTNAME_EN");

                entity.Property(e => e.Head2LastnameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD2_LASTNAME_TH");

                entity.Property(e => e.Head2NameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD2_NAME_EN");

                entity.Property(e => e.Head2NameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD2_NAME_TH");

                entity.Property(e => e.Head3Id)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("HEAD3_ID");

                entity.Property(e => e.Head3LastnameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD3_LASTNAME_EN");

                entity.Property(e => e.Head3LastnameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD3_LASTNAME_TH");

                entity.Property(e => e.Head3NameEn)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD3_NAME_EN");

                entity.Property(e => e.Head3NameTh)
                    .HasMaxLength(300)
                    .HasColumnName("HEAD3_NAME_TH");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(30)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LocName)
                    .HasMaxLength(400)
                    .HasColumnName("LOC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasGl>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MAS_GL");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GlAccount)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_ACCOUNT");

                entity.Property(e => e.GlDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("GL_DESC");

                entity.Property(e => e.GlNo)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_NO");

                entity.Property(e => e.GlStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_STATUS");

                entity.Property(e => e.GlType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_TYPE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasGlAccount>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.AcctCode });

                entity.ToTable("MAS_GL_ACCOUNT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.AcctCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCT_CODE");

                entity.Property(e => e.AcctName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ACCT_NAME");

                entity.Property(e => e.AcctStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCT_STATUS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasGlMap>(entity =>
            {
                entity.HasKey(e => new { e.GlNo, e.MopCode });

                entity.ToTable("MAS_GL_MAP");

                entity.Property(e => e.GlNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_NO");

                entity.Property(e => e.MopCode).HasColumnName("MOP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasMapping>(entity =>
            {
                entity.HasKey(e => new { e.MapValue, e.MapId });

                entity.ToTable("MAS_MAPPING");

                entity.Property(e => e.MapValue)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MAP_VALUE");

                entity.Property(e => e.MapId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MAP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.MapDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("MAP_DESC");

                entity.Property(e => e.MapStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MAP_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasOrganize>(entity =>
            {
                entity.HasKey(e => e.OrgCodedev);

                entity.ToTable("MAS_ORGANIZE");

                entity.Property(e => e.OrgCodedev)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORG_CODEDEV");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.MpPlant)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MP_PLANT");

                entity.Property(e => e.OrgCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORG_CODE");

                entity.Property(e => e.OrgComp)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORG_COMP");

                entity.Property(e => e.OrgName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("ORG_NAME");

                entity.Property(e => e.OrgShopid)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ORG_SHOPID");

                entity.Property(e => e.StatPosMart)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("STAT_POS_MART");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasPayGroup>(entity =>
            {
                entity.HasKey(e => e.PayGroupId);

                entity.ToTable("MAS_PAY_GROUP");

                entity.Property(e => e.PayGroupId)
                    .ValueGeneratedNever()
                    .HasColumnName("PAY_GROUP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PayGroupName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PAY_GROUP_NAME");

                entity.Property(e => e.PayGroupRemark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PAY_GROUP_REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasPayType>(entity =>
            {
                entity.HasKey(e => e.PayTypeId);

                entity.ToTable("MAS_PAY_TYPE");

                entity.Property(e => e.PayTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("PAY_TYPE_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PayGroupId).HasColumnName("PAY_GROUP_ID");

                entity.Property(e => e.PayTypeName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PAY_TYPE_NAME");

                entity.Property(e => e.PayTypeRemark)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PAY_TYPE_REMARK");

                entity.Property(e => e.PayTypeStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_TYPE_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasPayTypeDt>(entity =>
            {
                entity.HasKey(e => new { e.PayTypeId, e.PayCode });

                entity.ToTable("MAS_PAY_TYPE_DT");

                entity.Property(e => e.PayTypeId).HasColumnName("PAY_TYPE_ID");

                entity.Property(e => e.PayCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_CODE");

                entity.Property(e => e.PayName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PAY_NAME");
            });

            modelBuilder.Entity<MasPosition>(entity =>
            {
                entity.HasKey(e => e.PositionCode);

                entity.ToTable("MAS_POSITION");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.PositionDesc)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_DESC");

                entity.Property(e => e.PositionName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_NAME");

                entity.Property(e => e.PositionStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasProduct>(entity =>
            {
                entity.HasKey(e => e.PdId);

                entity.ToTable("MAS_PRODUCT");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.AcctCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("ACCT_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_ID");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.MapPdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("MAP_PD_ID");

                entity.Property(e => e.PdDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_DESC");

                entity.Property(e => e.PdImage)
                    .IsUnicode(false)
                    .HasColumnName("PD_IMAGE");

                entity.Property(e => e.PdName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.PdStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PD_STATUS");

                entity.Property(e => e.PdType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_TYPE");

                entity.Property(e => e.SubGroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUB_GROUP_ID");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<MasProductGroup>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.ToTable("MAS_PRODUCT_GROUP");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_NAME");

                entity.Property(e => e.GroupStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasProductPrice>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.PdId, e.UnitId });

                entity.ToTable("MAS_PRODUCT_PRICE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.PdStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_STATUS");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.Unitprice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNITPRICE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasProductSubGroup>(entity =>
            {
                entity.HasKey(e => e.SubGroupId);

                entity.ToTable("MAS_PRODUCT_SUB_GROUP");

                entity.Property(e => e.SubGroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUB_GROUP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.SubGroupName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("SUB_GROUP_NAME");

                entity.Property(e => e.SubGroupStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUB_GROUP_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasProductType>(entity =>
            {
                entity.HasKey(e => new { e.DocTypeId, e.GroupId });

                entity.ToTable("MAS_PRODUCT_TYPE");

                entity.Property(e => e.DocTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_ID");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasProductUnit>(entity =>
            {
                entity.HasKey(e => new { e.PdId, e.UnitId });

                entity.ToTable("MAS_PRODUCT_UNIT");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitRatio).HasColumnName("UNIT_RATIO");

                entity.Property(e => e.UnitStock).HasColumnName("UNIT_STOCK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasReason>(entity =>
            {
                entity.HasKey(e => new { e.ReasonGroup, e.ReasonId });

                entity.ToTable("MAS_REASON");

                entity.Property(e => e.ReasonGroup)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_GROUP");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IsValidate)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_VALIDATE")
                    .IsFixedLength(true);

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("REASON_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasReasonGroup>(entity =>
            {
                entity.HasKey(e => new { e.ReasonId, e.GroupId })
                    .HasName("PK_MAS_REASON_GROUP_1");

                entity.ToTable("MAS_REASON_GROUP");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.GroupId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GROUP_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.ReasonGroup)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_GROUP");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasSapCustomer>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MAS_SAP_CUSTOMER");

                entity.Property(e => e.BillingCust)
                    .HasMaxLength(255)
                    .HasColumnName("BILLING_CUST");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CustomerName)
                    .HasMaxLength(255)
                    .HasColumnName("CUSTOMER_NAME");

                entity.Property(e => e.LegCode)
                    .HasMaxLength(50)
                    .HasColumnName("LEG_CODE");

                entity.Property(e => e.SapAltCode)
                    .HasMaxLength(50)
                    .HasColumnName("SAP_ALT_CODE");

                entity.Property(e => e.SapCode)
                    .HasMaxLength(50)
                    .HasColumnName("SAP_CODE");

                entity.Property(e => e.SrcName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasSapPlant>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MAS_SAP_PLANT");

                entity.Property(e => e.BusinessArea)
                    .HasMaxLength(10)
                    .HasColumnName("BUSINESS_AREA");

                entity.Property(e => e.BusinessPlace)
                    .HasMaxLength(100)
                    .HasColumnName("BUSINESS_PLACE");

                entity.Property(e => e.Cca)
                    .HasMaxLength(50)
                    .HasColumnName("CCA");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(50)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Description)
                    .HasMaxLength(512)
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.IsDeleted)
                    .HasMaxLength(50)
                    .HasColumnName("IS_DELETED");

                entity.Property(e => e.Kokrs)
                    .HasMaxLength(20)
                    .HasColumnName("KOKRS");

                entity.Property(e => e.LegCode)
                    .HasMaxLength(50)
                    .HasColumnName("LEG_CODE");

                entity.Property(e => e.Pca)
                    .HasMaxLength(50)
                    .HasColumnName("PCA");

                entity.Property(e => e.PccaCode)
                    .HasMaxLength(50)
                    .HasColumnName("PCCA_CODE");

                entity.Property(e => e.Plant)
                    .HasMaxLength(50)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PlantName)
                    .HasMaxLength(100)
                    .HasColumnName("PLANT_NAME");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(50)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TextCsksGsber)
                    .HasMaxLength(100)
                    .HasColumnName("TEXT_CSKS_GSBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasSupplier>(entity =>
            {
                entity.HasKey(e => e.SupCode);

                entity.ToTable("MAS_SUPPLIER");

                entity.Property(e => e.SupCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SUP_CODE");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CreditTerm).HasColumnName("CREDIT_TERM");

                entity.Property(e => e.District)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("DISTRICT");

                entity.Property(e => e.Fax)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("FAX");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.MapSupCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_SUP_CODE");

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

                entity.Property(e => e.Remark)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.SubDistrict)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("SUB_DISTRICT");

                entity.Property(e => e.SupName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("SUP_NAME");

                entity.Property(e => e.SupPrefix)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("SUP_PREFIX");

                entity.Property(e => e.SupStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SUP_STATUS");

                entity.Property(e => e.TaxId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TAX_ID");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<MasSupplierPay>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.SupCode });

                entity.ToTable("MAS_SUPPLIER_PAY");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.SupCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SUP_CODE");

                entity.Property(e => e.PayAddrId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_ADDR_ID");

                entity.Property(e => e.TaxAddrId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TAX_ADDR_ID");
            });

            modelBuilder.Entity<MasSupplierProduct>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.SupCode, e.UnitBarcode })
                    .HasName("PK_MAS_SUPPLIER_PRODUCT_1");

                entity.ToTable("MAS_SUPPLIER_PRODUCT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.SupCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SUP_CODE");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitCost)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_COST");

                entity.Property(e => e.UnitPack).HasColumnName("UNIT_PACK");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");
            });

            modelBuilder.Entity<MasToken>(entity =>
            {
                entity.HasKey(e => e.Mid);

                entity.ToTable("MAS_TOKEN");

                entity.Property(e => e.Mid)
                    .HasMaxLength(20)
                    .HasColumnName("MID");

                entity.Property(e => e.AccountName)
                    .HasMaxLength(10)
                    .HasColumnName("ACCOUNT_NAME");

                entity.Property(e => e.BranchName)
                    .HasMaxLength(100)
                    .HasColumnName("BRANCH_NAME");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.SapCode)
                    .HasMaxLength(20)
                    .HasColumnName("SAP_CODE");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("STATUS");

                entity.Property(e => e.Token)
                    .HasMaxLength(100)
                    .HasColumnName("TOKEN");
            });

            modelBuilder.Entity<MasUnit>(entity =>
            {
                entity.HasKey(e => e.UnitId);

                entity.ToTable("MAS_UNIT");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.MapUnitId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MAP_UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_STATUS");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<MasUtf8>(entity =>
            {
                entity.HasKey(e => e.UtfCode);

                entity.ToTable("MAS_UTF8");

                entity.Property(e => e.UtfCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("UTF_CODE");

                entity.Property(e => e.ThCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TH_CODE");
            });

            modelBuilder.Entity<MasWarehouse>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.WhCode });

                entity.ToTable("MAS_WAREHOUSE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.WhCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("WH_CODE");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("ADDRESS");

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

                entity.Property(e => e.MapWhCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_WH_CODE");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
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

                entity.Property(e => e.WhName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("WH_NAME");

                entity.Property(e => e.WhStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("WH_STATUS");
            });

            modelBuilder.Entity<OilPromotionPriceDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo, e.PdId, e.UnitBarcode });

                entity.ToTable("OIL_PROMOTION_PRICE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.AdjustPrice)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("ADJUST_PRICE");
            });

            modelBuilder.Entity<OilPromotionPriceHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo });

                entity.ToTable("OIL_PROMOTION_PRICE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("APPROVE_DATE");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.FinishDate)
                    .HasColumnType("datetime")
                    .HasColumnName("FINISH_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<OilStandardPriceDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo, e.PdId, e.UnitBarcode });

                entity.ToTable("OIL_STANDARD_PRICE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.AdjustPrice)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("ADJUST_PRICE");

                entity.Property(e => e.BeforePrice)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("BEFORE_PRICE");

                entity.Property(e => e.CurrentPrice)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CURRENT_PRICE");
            });

            modelBuilder.Entity<OilStandardPriceHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo });

                entity.ToTable("OIL_STANDARD_PRICE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("APPROVE_DATE");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DocType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<PriNonoilDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo, e.PdId, e.UnitBarcode });

                entity.ToTable("PRI_NONOIL_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.PdId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.AdjustPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_PRICE");

                entity.Property(e => e.BeforePrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_PRICE");

                entity.Property(e => e.CurrentPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CURRENT_PRICE");
            });

            modelBuilder.Entity<PriNonoilHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocNo });

                entity.ToTable("PRI_NONOIL_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("APPROVE_DATE");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<PriOilStandardDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.DocNo, e.SeqNo })
                    .HasName("PK_PRI_OIL_STANDARD_DT_1");

                entity.ToTable("PRI_OIL_STANDARD_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");
            });

            modelBuilder.Entity<PriOilStandardHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.DocNo })
                    .HasName("PK_PRI_OIL_STANDARD_HD_1");

                entity.ToTable("PRI_OIL_STANDARD_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("APPROVE_DATE");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DocTypeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_DESC");

                entity.Property(e => e.DocTypeId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE_ID");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SalBillingDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_BILLING_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.TxAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TX_AMT");

                entity.Property(e => e.TxAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TX_AMT_CUR");

                entity.Property(e => e.TxBrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_BRN_CODE");

                entity.Property(e => e.TxDate)
                    .HasColumnType("date")
                    .HasColumnName("TX_DATE");

                entity.Property(e => e.TxNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_NO");

                entity.Property(e => e.TxType)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TX_TYPE");
            });

            modelBuilder.Entity<SalBillingHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("SAL_BILLING_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CreditLimit)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CREDIT_LIMIT");

                entity.Property(e => e.CreditTerm).HasColumnName("CREDIT_TERM");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DueDate)
                    .HasColumnType("date")
                    .HasColumnName("DUE_DATE");

                entity.Property(e => e.DueType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DUE_TYPE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SalCashsaleDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_CASHSALE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscHdAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT");

                entity.Property(e => e.DiscHdAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT_CUR");

                entity.Property(e => e.IsFree).HasColumnName("IS_FREE");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE");

                entity.Property(e => e.RefPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE_CUR");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SumItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT");

                entity.Property(e => e.SumItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UnitPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE_CUR");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<SalCashsaleHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("SAL_CASHSALE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscRate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISC_RATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.PosNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POS_NO");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.QtNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("QT_NO");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");
            });

            modelBuilder.Entity<SalCashsaleLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("SAL_CASHSALE_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.MessageLog)
                    .IsUnicode(false)
                    .HasColumnName("MESSAGE_LOG");
            });

            modelBuilder.Entity<SalCndnDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_CNDN_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.AdjustAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_AMT");

                entity.Property(e => e.AdjustAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_AMT_CUR");

                entity.Property(e => e.AdjustQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ADJUST_QTY");

                entity.Property(e => e.AfterAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AFTER_AMT");

                entity.Property(e => e.AfterAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AFTER_AMT_CUR");

                entity.Property(e => e.AfterPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AFTER_PRICE");

                entity.Property(e => e.AfterQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("AFTER_QTY");

                entity.Property(e => e.BeforeAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_AMT");

                entity.Property(e => e.BeforeAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_AMT_CUR");

                entity.Property(e => e.BeforePrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_PRICE");

                entity.Property(e => e.BeforeQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BEFORE_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<SalCndnHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo });

                entity.ToTable("SAL_CNDN_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.PrintBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRINT_BY");

                entity.Property(e => e.PrintCount).HasColumnName("PRINT_COUNT");

                entity.Property(e => e.PrintDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRINT_DATE");

                entity.Property(e => e.ReasonDesc)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REASON_DESC");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TxNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_NO");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");
            });

            modelBuilder.Entity<SalCreditsaleDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_CREDITSALE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscHdAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT");

                entity.Property(e => e.DiscHdAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT_CUR");

                entity.Property(e => e.IsFree).HasColumnName("IS_FREE");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.MeterFinish)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_FINISH");

                entity.Property(e => e.MeterStart)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("METER_START");

                entity.Property(e => e.Mile).HasColumnName("MILE");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.PoNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PO_NO");

                entity.Property(e => e.RefPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE");

                entity.Property(e => e.RefPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE_CUR");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SumItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT");

                entity.Property(e => e.SumItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UnitPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE_CUR");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<SalCreditsaleHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocType, e.DocNo });

                entity.ToTable("SAL_CREDITSALE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscRate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISC_RATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.Period)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PERIOD");

                entity.Property(e => e.PosNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POS_NO");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.QtNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("QT_NO");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.TxNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("TX_NO");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");
            });

            modelBuilder.Entity<SalCreditsaleLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("SAL_CREDITSALE_LOG");

                entity.Property(e => e.LogNo).HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsRequired()
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.RefNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");
            });

            modelBuilder.Entity<SalQuotationDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_QUOTATION_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscHdAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT");

                entity.Property(e => e.DiscHdAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT_CUR");

                entity.Property(e => e.IsFree).HasColumnName("IS_FREE");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.RefPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE");

                entity.Property(e => e.RefPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("REF_PRICE_CUR");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.StockRemain)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_REMAIN");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SumItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT");

                entity.Property(e => e.SumItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UnitPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE_CUR");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<SalQuotationHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("SAL_QUOTATION_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApprCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("APPR_CODE");

                entity.Property(e => e.BrnCodeFrom)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE_FROM");

                entity.Property(e => e.BrnNameFrom)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("BRN_NAME_FROM");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscRate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISC_RATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DocType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.FinishDate)
                    .HasColumnType("date")
                    .HasColumnName("FINISH_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.InvAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("INV_ADDR1");

                entity.Property(e => e.InvAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("INV_ADDR2");

                entity.Property(e => e.InvName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("INV_NAME");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.MaxCardId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAX_CARD_ID");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.PayCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PAY_CODE");

                entity.Property(e => e.PayTypeId).HasColumnName("PAY_TYPE_ID");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PHONE");

                entity.Property(e => e.PosPrintFlag)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POS_PRINT_FLAG");

                entity.Property(e => e.PosRewardFlag)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POS_REWARD_FLAG");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.PrintBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRINT_BY");

                entity.Property(e => e.PrintCount).HasColumnName("PRINT_COUNT");

                entity.Property(e => e.PrintDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRINT_DATE");

                entity.Property(e => e.Remark)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");
            });

            modelBuilder.Entity<SalQuotationLog>(entity =>
            {
                entity.HasKey(e => e.LogNo);

                entity.ToTable("SAL_QUOTATION_LOG");

                entity.Property(e => e.LogNo)
                    .ValueGeneratedNever()
                    .HasColumnName("LOG_NO");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.JsonData)
                    .IsUnicode(false)
                    .HasColumnName("JSON_DATA");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.LogStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOG_STATUS");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");
            });

            modelBuilder.Entity<SalTaxinvoiceDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.SeqNo });

                entity.ToTable("SAL_TAXINVOICE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscHdAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT");

                entity.Property(e => e.DiscHdAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_HD_AMT_CUR");

                entity.Property(e => e.IsFree).HasColumnName("IS_FREE");

                entity.Property(e => e.ItemQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ITEM_QTY");

                entity.Property(e => e.LicensePlate)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("LICENSE_PLATE");

                entity.Property(e => e.PdId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.PdName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("PD_NAME");

                entity.Property(e => e.StockQty)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("STOCK_QTY");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.SumItemAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT");

                entity.Property(e => e.SumItemAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUM_ITEM_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UnitBarcode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_BARCODE");

                entity.Property(e => e.UnitId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_ID");

                entity.Property(e => e.UnitName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UNIT_NAME");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.UnitPriceCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("UNIT_PRICE_CUR");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");

                entity.Property(e => e.VatType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("VAT_TYPE");
            });

            modelBuilder.Entity<SalTaxinvoiceHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("SAL_TAXINVOICE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.CitizenId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CITIZEN_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.CurRate)
                    .HasColumnType("decimal(16, 4)")
                    .HasColumnName("CUR_RATE");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CURRENCY");

                entity.Property(e => e.CustAddr1)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR1");

                entity.Property(e => e.CustAddr2)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("CUST_ADDR2");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.CustName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("CUST_NAME");

                entity.Property(e => e.DiscAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT");

                entity.Property(e => e.DiscAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("DISC_AMT_CUR");

                entity.Property(e => e.DiscRate)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DISC_RATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.DocPattern)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("DOC_PATTERN");

                entity.Property(e => e.DocStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_STATUS");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemCount).HasColumnName("ITEM_COUNT");

                entity.Property(e => e.NetAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT");

                entity.Property(e => e.NetAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("NET_AMT_CUR");

                entity.Property(e => e.Post)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("POST")
                    .IsFixedLength(true);

                entity.Property(e => e.PrintBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("PRINT_BY");

                entity.Property(e => e.PrintCount).HasColumnName("PRINT_COUNT");

                entity.Property(e => e.PrintDate)
                    .HasColumnType("datetime")
                    .HasColumnName("PRINT_DATE");

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.SubAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT");

                entity.Property(e => e.SubAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("SUB_AMT_CUR");

                entity.Property(e => e.TaxBaseAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT");

                entity.Property(e => e.TaxBaseAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TAX_BASE_AMT_CUR");

                entity.Property(e => e.TotalAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT");

                entity.Property(e => e.TotalAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("TOTAL_AMT_CUR");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");

                entity.Property(e => e.VatAmt)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT");

                entity.Property(e => e.VatAmtCur)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("VAT_AMT_CUR");

                entity.Property(e => e.VatRate).HasColumnName("VAT_RATE");
            });

            modelBuilder.Entity<SapCustomerMapping>(entity =>
            {
                entity.HasKey(e => new { e.CustCode, e.SapCustCode })
                    .HasName("PK_SAP_CUSTOMER_MP");

                entity.ToTable("SAP_CUSTOMER_MAPPING");

                entity.Property(e => e.CustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CUST_CODE");

                entity.Property(e => e.SapCustCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_CUST_CODE");

                entity.Property(e => e.ControlVersion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CONTROL_VERSION")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.SapAltCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_ALT_CODE");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<SapMaterialMapping>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SAP_MATERIAL_MAPPING");

                entity.Property(e => e.ControlVersion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CONTROL_VERSION")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.PdId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PD_ID");

                entity.Property(e => e.SapMaterialNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_MATERIAL_NO");

                entity.Property(e => e.SapUomCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_UOM_CODE");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<SapMovementTypeMapping>(entity =>
            {
                entity.HasKey(e => new { e.ReasonId, e.MovementType })
                    .HasName("PK_SAP_MOVEMENT_TYPE");

                entity.ToTable("SAP_MOVEMENT_TYPE_MAPPING");

                entity.Property(e => e.ReasonId)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("REASON_ID");

                entity.Property(e => e.MovementType)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.ControlVersion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CONTROL_VERSION")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<SapPlantMapping>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SAP_PLANT_MAPPING");

                entity.Property(e => e.BrnCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.ControlVersion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CONTROL_VERSION")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");

                entity.Property(e => e.SapBusinessPlace)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SAP_BUSINESS_PLACE");

                entity.Property(e => e.SapCcaCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_CCA_CODE");

                entity.Property(e => e.SapCompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SAP_COMP_CODE");

                entity.Property(e => e.SapPcaCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SAP_PCA_CODE");

                entity.Property(e => e.SapPlant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SAP_PLANT");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE")
                    .HasDefaultValueSql("(dateadd(hour,(7),getdate()))");
            });

            modelBuilder.Entity<SysApproveConfig>(entity =>
            {
                entity.HasKey(e => e.DocType);

                entity.ToTable("SYS_APPROVE_CONFIG");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NAME");

                entity.Property(e => e.Route)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ROUTE");

                entity.Property(e => e.StepCount).HasColumnName("STEP_COUNT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysApproveDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.DocNo, e.SeqNo });

                entity.ToTable("SYS_APPROVE_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.RefDate)
                    .HasColumnType("date")
                    .HasColumnName("REF_DATE");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.RefTypeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("REF_TYPE_DESC");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysApproveHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo });

                entity.ToTable("SYS_APPROVE_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_BY");

                entity.Property(e => e.ApproveName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_NAME");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysApproveStep>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.LocCode, e.DocNo, e.StepNo });

                entity.ToTable("SYS_APPROVE_STEP");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.LocCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("LOC_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.StepNo).HasColumnName("STEP_NO");

                entity.Property(e => e.ApprCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("APPR_CODE");

                entity.Property(e => e.ApprStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPR_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocType)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_TYPE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysApproveoilDt>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.DocNo, e.SeqNo });

                entity.ToTable("SYS_APPROVEOIL_DT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EffectiveDate)
                    .HasColumnType("datetime")
                    .HasColumnName("EFFECTIVE_DATE");

                entity.Property(e => e.RefDate)
                    .HasColumnType("date")
                    .HasColumnName("REF_DATE");

                entity.Property(e => e.RefNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("REF_NO");

                entity.Property(e => e.RefTypeDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("REF_TYPE_DESC");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysApproveoilHd>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.DocNo });

                entity.ToTable("SYS_APPROVEOIL_HD");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DOC_NO");

                entity.Property(e => e.ApproveBy)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_BY");

                entity.Property(e => e.ApproveName)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_NAME");

                entity.Property(e => e.ApproveStatus)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("APPROVE_STATUS")
                    .IsFixedLength(true);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.Guid)
                    .HasColumnName("GUID")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.RunNumber).HasColumnName("RUN_NUMBER");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysBranchConfig>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.SeqNo, e.ItemNo });

                entity.ToTable("SYS_BRANCH_CONFIG");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.ItemNo).HasColumnName("ITEM_NO");

                entity.Property(e => e.ConfigId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CONFIG_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.EmpCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("EMP_CODE");

                entity.Property(e => e.EmpName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("EMP_NAME");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.IsLock)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_LOCK")
                    .IsFixedLength(true);

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysBranchConfigRole>(entity =>
            {
                entity.HasKey(e => new { e.PositionCode, e.ItemNo });

                entity.ToTable("SYS_BRANCH_CONFIG_ROLE");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("POSITION_CODE");

                entity.Property(e => e.ItemNo).HasColumnName("ITEM_NO");

                entity.Property(e => e.ConfigId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CONFIG_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IsView)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_VIEW")
                    .IsFixedLength(true);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysConfigApi>(entity =>
            {
                entity.HasKey(e => new { e.SystemId, e.ApiId });

                entity.ToTable("SYS_CONFIG_API");

                entity.Property(e => e.SystemId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("SYSTEM_ID");

                entity.Property(e => e.ApiId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("API_ID");

                entity.Property(e => e.ApiDesc)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("API_DESC");

                entity.Property(e => e.ApiUrl)
                    .IsUnicode(false)
                    .HasColumnName("API_URL");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Method)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("METHOD");

                entity.Property(e => e.Topic)
                    .IsUnicode(false)
                    .HasColumnName("TOPIC");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysMenu>(entity =>
            {
                entity.HasKey(e => e.MenuId);

                entity.ToTable("SYS_MENU");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MENU_ID");

                entity.Property(e => e.Child)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("CHILD");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.MenuName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MENU_NAME");

                entity.Property(e => e.MenuStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MENU_STATUS");

                entity.Property(e => e.Parent)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PARENT");

                entity.Property(e => e.Route)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("ROUTE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysMenuRole>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.MenuId });

                entity.ToTable("SYS_MENU_ROLE");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.MenuId)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MENU_ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysMessage>(entity =>
            {
                entity.HasKey(e => new { e.MsgCode, e.MsgLang });

                entity.ToTable("SYS_MESSAGE");

                entity.Property(e => e.MsgCode).HasColumnName("MSG_CODE");

                entity.Property(e => e.MsgLang)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MSG_LANG");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.MsgText)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("MSG_TEXT");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysNotification>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode, e.DocDate, e.SeqNo });

                entity.ToTable("SYS_NOTIFICATION");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.DocDate)
                    .HasColumnType("date")
                    .HasColumnName("DOC_DATE");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.IsRead)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_READ")
                    .IsFixedLength(true)
                    .HasComment("N/Y");

                entity.Property(e => e.Remark)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<SysReportConfig>(entity =>
            {
                entity.HasKey(e => new { e.ReportGroup, e.SeqNo });

                entity.ToTable("SYS_REPORT_CONFIG");

                entity.Property(e => e.ReportGroup)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("REPORT_GROUP");

                entity.Property(e => e.SeqNo).HasColumnName("SEQ_NO");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.ExcelUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("EXCEL_URL");

                entity.Property(e => e.IsExcel)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_EXCEL")
                    .IsFixedLength(true);

                entity.Property(e => e.IsPdf)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("IS_PDF")
                    .IsFixedLength(true);

                entity.Property(e => e.ParameterType)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("PARAMETER_TYPE");

                entity.Property(e => e.ReportName)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REPORT_NAME");

                entity.Property(e => e.ReportStatus)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("REPORT_STATUS");

                entity.Property(e => e.ReportUrl)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REPORT_URL");

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("UPDATED_BY");

                entity.Property(e => e.UpdatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("UPDATED_DATE");
            });

            modelBuilder.Entity<TmpBranchPilot>(entity =>
            {
                entity.HasKey(e => new { e.CompCode, e.BrnCode })
                    .HasName("PK_tmp_branch_pilot");

                entity.ToTable("TMP_BRANCH_PILOT");

                entity.Property(e => e.CompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.BrnCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("BRN_CODE");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("date")
                    .HasColumnName("CREATED_DATE");

                entity.Property(e => e.Finance)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("FINANCE");

                entity.Property(e => e.Inventory)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("INVENTORY");

                entity.Property(e => e.MapCompCode)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("MAP_COMP_CODE");

                entity.Property(e => e.Meter)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("METER");

                entity.Property(e => e.MeterDate)
                    .HasColumnType("date")
                    .HasColumnName("METER_DATE");

                entity.Property(e => e.Postday)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("POSTDAY");

                entity.Property(e => e.PostdayDate)
                    .HasColumnType("date")
                    .HasColumnName("POSTDAY_DATE");

                entity.Property(e => e.Remark)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("REMARK");

                entity.Property(e => e.Sale)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("SALE");
            });

            modelBuilder.Entity<TrnScbLog>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TRN_SCB_LOG");

                entity.Property(e => e.Id)
                    .HasMaxLength(10)
                    .HasColumnName("ID")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<VInfSapMm036>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm036");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.DelNumToSearch)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DEL_NUM_TO_SEARCH");

                entity.Property(e => e.DelivItem)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_ITEM");

                entity.Property(e => e.DelivItemToSearch).HasColumnName("DELIV_ITEM_TO_SEARCH");

                entity.Property(e => e.DelivNum)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DELIV_NUM");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.MvtInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("MVT_IND");

                entity.Property(e => e.NoMoreGr)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("NO_MORE_GR");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PoItem)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapMm041>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm041");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveMat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_MAT");

                entity.Property(e => e.MovePlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_PLANT");

                entity.Property(e => e.MoveStloc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_STLOC");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.MoveValType)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_VAL_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapMm042>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm042");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveMat)
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_MAT");

                entity.Property(e => e.MovePlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_PLANT");

                entity.Property(e => e.MoveStloc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_STLOC");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapMm048>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm048");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveReas)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_REAS");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.MvtInd)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("MVT_IND");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PoItem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM");

                entity.Property(e => e.PoNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapMm051>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm051");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.ItemText)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_TEXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.UnloadPt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("UNLOAD_PT");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapMm052>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_mm052");

                entity.Property(e => e.BillOfLading)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("BILL_OF_LADING");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.DocDate)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DOC_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.EntryQnt)
                    .HasColumnType("decimal(13, 3)")
                    .HasColumnName("ENTRY_QNT");

                entity.Property(e => e.EntryUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ENTRY_UOM");

                entity.Property(e => e.GlAccount)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("GL_ACCOUNT");

                entity.Property(e => e.GmCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("GM_CODE");

                entity.Property(e => e.GrRcpt)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("GR_RCPT");

                entity.Property(e => e.HeaderTxt)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("HEADER_TXT");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MoveType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVE_TYPE");

                entity.Property(e => e.Orderid)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ORDERID");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PstngDate)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PSTNG_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.RefDocNo)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_NO");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.StgeLoc)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("STGE_LOC");

                entity.Property(e => e.VerGrGiSlip)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIP");

                entity.Property(e => e.VerGrGiSlipx)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("VER_GR_GI_SLIPX");

                entity.Property(e => e.Zcoscenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ZCOSCENTER");

                entity.Property(e => e.Zcustomer)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ZCUSTOMER");

                entity.Property(e => e.Zitem)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("ZITEM");
            });

            modelBuilder.Entity<VInfSapOil02>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_oil02");

                entity.Property(e => e.ComfirmQty)
                    .HasColumnType("decimal(16, 3)")
                    .HasColumnName("COMFIRM_QTY");

                entity.Property(e => e.DelDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DEL_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DelTime)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("DEL_TIME")
                    .IsFixedLength(true);

                entity.Property(e => e.MatNr)
                    .IsRequired()
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasColumnName("MAT_NR");

                entity.Property(e => e.MatTemp)
                    .HasColumnType("decimal(16, 1)")
                    .HasColumnName("MAT_TEMP");

                entity.Property(e => e.MatTempUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("MAT_TEMP_UOM");

                entity.Property(e => e.Mcf)
                    .HasColumnType("decimal(16, 8)")
                    .HasColumnName("MCF");

                entity.Property(e => e.PlantNr)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT_NR");

                entity.Property(e => e.PoItemNo)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasColumnName("PO_ITEM_NO");

                entity.Property(e => e.PoNumber)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PO_NUMBER");

                entity.Property(e => e.SlocNr)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SLOC_NR");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TestDen)
                    .HasColumnType("decimal(16, 1)")
                    .HasColumnName("TEST_DEN");

                entity.Property(e => e.TestTemp)
                    .HasColumnType("decimal(16, 8)")
                    .HasColumnName("TEST_TEMP");

                entity.Property(e => e.TestTempUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("TEST_TEMP_UOM");

                entity.Property(e => e.TrUom)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("TR_UOM");
            });

            modelBuilder.Entity<VInfSapZmmint01>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_zmmint01");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.Customer)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER");

                entity.Property(e => e.DistributeChannel)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("DISTRIBUTE_CHANNEL");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MovementType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.SaleOrg)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("SALE_ORG");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<VInfSapZmmint02>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_zmmint02");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MovementType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.RefDocTfOut)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_TF_OUT");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.TargetPlant)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("TARGET_PLANT");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<VInfSapZmmint03>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_zmmint03");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MovementType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.RefDocTfIn)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasColumnName("REF_DOC_TF_IN");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            modelBuilder.Entity<VInfSapZmmint04>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_inf_sap_zmmint04");

                entity.Property(e => e.CompCode)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("COMP_CODE");

                entity.Property(e => e.Costcenter)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("COSTCENTER");

                entity.Property(e => e.DocumentDate)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.DocumentNo)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("DOCUMENT_NO");

                entity.Property(e => e.ItemNo)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("ITEM_NO");

                entity.Property(e => e.Material)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("MATERIAL");

                entity.Property(e => e.MovementType)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("MOVEMENT_TYPE");

                entity.Property(e => e.Plant)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("PLANT");

                entity.Property(e => e.PostingDate)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("POSTING_DATE")
                    .IsFixedLength(true);

                entity.Property(e => e.Quantity)
                    .HasColumnType("decimal(13, 2)")
                    .HasColumnName("QUANTITY");

                entity.Property(e => e.Slog)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SLOG");

                entity.Property(e => e.SrcName)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SRC_NAME");

                entity.Property(e => e.Unit)
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("UNIT");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

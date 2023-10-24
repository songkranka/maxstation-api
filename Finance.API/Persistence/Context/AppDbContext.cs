using Finance.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.API.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<MasExpense> MasExpense { get; set; }
        public DbSet<FinExpenseHd> FinExpenseHd { get; set; }
        public DbSet<FinExpenseDt> FinExpenseDt { get; set; }
        public DbSet<FinExpenseEss> FinExpenseEss { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MasExpense>().ToTable("MAS_EXPENSE");
            builder.Entity<MasExpense>().HasKey(p => new { p.ExpenseNo });
            builder.Entity<MasExpense>().Property(x => x.ExpenseNo).HasColumnName("EXPENSE_NO").IsRequired();
            builder.Entity<MasExpense>().Property(x => x.ExpenseStatus).HasColumnName("EXPENSE_STATUS").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.ExpenseName).HasColumnName("EXPENSE_NAME").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.ExpenseQty).HasColumnName("EXPENSE_QTY").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.LockQty).HasColumnName("LOCK_QTY").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.ExpenseUnit).HasColumnName("EXPENSE_UNIT").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.ExpenseData).HasColumnName("EXPENSE_DATA").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.LockData).HasColumnName("LOCK_DATA").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.Parent).HasColumnName("PARENT").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.SeqNo).HasColumnName("SEQ_NO").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.CreatedDate).HasColumnName("CREATED_DATE").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.CreatedBy).HasColumnName("CREATED_BY").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.UpdatedDate).HasColumnName("UPDATED_DATE").IsRequired(false);
            builder.Entity<MasExpense>().Property(x => x.UpdatedBy).HasColumnName("UPDATED_BY").IsRequired(false);

            builder.Entity<FinExpenseHd>().ToTable("FIN_EXPENSE_HD");
            builder.Entity<FinExpenseHd>().HasKey(p => new { p.CompCode, p.BrnCode, p.LocCode, p.DocNo });
            builder.Entity<FinExpenseHd>().Property(x => x.CompCode).HasColumnName("COMP_CODE").IsRequired();
            builder.Entity<FinExpenseHd>().Property(x => x.BrnCode).HasColumnName("BRN_CODE").IsRequired();
            builder.Entity<FinExpenseHd>().Property(x => x.LocCode).HasColumnName("LOC_CODE").IsRequired();
            builder.Entity<FinExpenseHd>().Property(x => x.DocNo).HasColumnName("DOC_NO").IsRequired();
            builder.Entity<FinExpenseHd>().Property(x => x.DocStatus).HasColumnName("DOC_STATUS").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.DocDate).HasColumnName("DOC_DATE").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.WorkType).HasColumnName("WORK_TYPE").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.WorkStart).HasColumnName("WORK_START").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.WorkFinish).HasColumnName("WORK_FINISH").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.Remark).HasColumnName("REMARK").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.Post).HasColumnName("POST").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.RunNumber).HasColumnName("RUN_NUMBER").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.DocPattern).HasColumnName("DOC_PATTERN").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.Guid).HasColumnName("GUID").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.CreatedDate).HasColumnName("CREATED_DATE").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.CreatedBy).HasColumnName("CREATED_BY").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.UpdatedDate).HasColumnName("UPDATED_DATE").IsRequired(false);
            builder.Entity<FinExpenseHd>().Property(x => x.UpdatedBy).HasColumnName("UPDATED_BY").IsRequired(false);

            builder.Entity<FinExpenseDt>().ToTable("FIN_EXPENSE_DT");
            builder.Entity<FinExpenseDt>().HasKey(p => new { p.CompCode, p.BrnCode, p.LocCode, p.DocNo, p.SeqNo });
            builder.Entity<FinExpenseDt>().Property(x => x.CompCode).HasColumnName("COMP_CODE").IsRequired();
            builder.Entity<FinExpenseDt>().Property(x => x.BrnCode).HasColumnName("BRN_CODE").IsRequired();
            builder.Entity<FinExpenseDt>().Property(x => x.LocCode).HasColumnName("LOC_CODE").IsRequired();
            builder.Entity<FinExpenseDt>().Property(x => x.DocNo).HasColumnName("DOC_NO").IsRequired();
            builder.Entity<FinExpenseDt>().Property(x => x.SeqNo).HasColumnName("SEQ_NO").IsRequired();
            builder.Entity<FinExpenseDt>().Property(x => x.ExpenseNo).HasColumnName("EXPENSE_NO").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.Parent).HasColumnName("Parent").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.CateName).HasColumnName("CATE_NAME").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.BaseName).HasColumnName("BASE_NAME").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.BaseQty).HasColumnName("BASE_QTY").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.BaseUnit).HasColumnName("BASE_UNIT").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.ItemName).HasColumnName("ITEM_NAME").IsRequired(false);
            builder.Entity<FinExpenseDt>().Property(x => x.ItemQty).HasColumnName("ITEM_QTY").IsRequired(false);

            builder.Entity<FinExpenseEss>().ToTable("FIN_EXPENSE_ESS");
            builder.Entity<FinExpenseEss>().HasKey(p => new { p.CompCode, p.BrnCode, p.LocCode, p.DocNo, p.SeqNo });
            builder.Entity<FinExpenseEss>().Property(x => x.CompCode).HasColumnName("COMP_CODE").IsRequired();
            builder.Entity<FinExpenseEss>().Property(x => x.BrnCode).HasColumnName("BRN_CODE").IsRequired();
            builder.Entity<FinExpenseEss>().Property(x => x.LocCode).HasColumnName("LOC_CODE").IsRequired();
            builder.Entity<FinExpenseEss>().Property(x => x.DocNo).HasColumnName("DOC_NO").IsRequired();
            builder.Entity<FinExpenseEss>().Property(x => x.SeqNo).HasColumnName("SEQ_NO").IsRequired();
            builder.Entity<FinExpenseEss>().Property(x => x.EssNo).HasColumnName("ESS_NO").IsRequired(false);
            builder.Entity<FinExpenseEss>().Property(x => x.EssDesc).HasColumnName("ESS_DESC").IsRequired(false);
        }
    }
}

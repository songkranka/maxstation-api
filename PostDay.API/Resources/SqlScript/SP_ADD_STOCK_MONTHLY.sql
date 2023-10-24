declare @CompCode char(2) = '{0}'
declare @BrnCode varchar(3)='{1}'
declare @LocCode varchar(4)='{2}'
declare @CreatedBy varchar(10) = '{3}'
declare @YearNo int = {4};
declare @MonthNo int = {5};
declare @SysDate date = cast( concat(@YearNo , '-' , @MonthNo , '-' , 1) as date);
declare @Yesterday DateTime = DateAdd(D,-1 , @SysDate);
declare @PrevMonthNo int = month(@YesterDay);
declare @YesterDay2 DateTime = DateAdd(D,-1 , @yesterday);
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
delete INV_STOCK_MONTHLY where YEAR_NO = @YearNo
	and MONTH_NO = @PrevMonthNo
	and COMP_CODE = @CompCode
	and BRN_CODE = @BrnCode
	and LOC_CODE = @LocCode
insert into INV_STOCK_MONTHLY(
	[COMP_CODE]
    ,[BRN_CODE]
    ,[LOC_CODE]
    ,[YEAR_NO]
    ,[MONTH_NO]
    ,[PD_ID]
    ,[UNIT_ID]
    ,[UNIT_BARCODE]
    ,[BALANCE]
    ,[CREATED_DATE]
    ,[CREATED_BY]
)
select [COMP_CODE]
      ,[BRN_CODE]
      ,[LOC_CODE]
      ,@YearNo [YEAR_NO]
      ,@PrevMonthNo [MONTH_NO]
      ,[PD_ID]
      ,[UNIT_ID]
      ,[UNIT_BARCODE]
      ,[BALANCE]
      ,GETDATE() [CREATED_DATE]
      ,@CreatedBy [CREATED_BY] 
from INV_STOCK_DAILY 
where STOCK_DATE = @Yesterday
	and COMP_CODE = @CompCode
	and BRN_CODE = @BrnCode
	and LOC_CODE = @LocCode
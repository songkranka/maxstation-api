declare @CompCode char(2) = '{0}'
declare @BrnCode varchar(3)='{1}'
declare @LocCode varchar(4)='{2}'
declare @CreatedBy varchar(10) = '{3}'
declare @SysDate DateTime = '{4}'; --'2021-09-08';

declare @YesterDay DateTime = DateAdd(D,-1 , @SysDate);
--declare @SysDate2 DateTime = DateAdd(D,-1 , @SysDate);


SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
delete INV_STOCK_DAILY where STOCK_DATE = @SysDate
	and COMP_CODE = @CompCode 
	and BRN_CODE = @BrnCode 
	and LOC_CODE = @LocCode;
INSERT INTO [dbo].[INV_STOCK_DAILY]
	([COMP_CODE]
	,[BRN_CODE]
	,[LOC_CODE]
	,[STOCK_DATE]
	,[PD_ID]
	,[UNIT_ID]
	,[UNIT_BARCODE]
	,[BALANCE]
	,[RECEIVE_IN]
	,[TRANSFER_IN]
	,[TRANSFER_OUT]
	,[SALE_OUT]
	,[RETURN_OUT]
	,[WITHDRAW_OUT]
	,[ADJUST]
	,[REMAIN]
	,[CREATED_DATE]
	,[CREATED_BY])
select  
	@CompCode [COMP_CODE]
    ,@BrnCode [BRN_CODE]
    ,@LocCode [LOC_CODE]
    ,@SysDate [STOCK_DATE]
    ,[PD_ID]
    ,[UNIT_ID]
    ,[UNIT_BARCODE]
    ,Remain [BALANCE]
    ,ReceiveProd [RECEIVE_IN]
    ,TranferIn [TRANSFER_IN]
    ,TranferOut [TRANSFER_OUT]
    ,CashSale + CreditSale [SALE_OUT]
    ,ReturnSup + ReturnOil [RETURN_OUT]
    ,WithDraw [WITHDRAW_OUT]
    ,Adjust [ADJUST]
    ,Remain + ReceiveProd + TranferIn - TranferOut - CashSale 
	- CreditSale - ReturnSup - ReturnOil - WithDraw - Adjust  [REMAIN]
    ,GETDATE() [CREATED_DATE]
    ,@CreatedBy [CREATED_BY]
from
(
	select 	
		ISNULL( Balance.REMAIN,0) Remain,
		ISNULL( ReceiveProd.STOCK_QTY,0) ReceiveProd,
		ISNULL( TranferIn.STOCK_QTY,0) TranferIn,
		ISNULL( TranferOut.STOCK_QTY,0) TranferOut,
		ISNULL( CashSale.STOCK_QTY,0) CashSale,
		ISNULL( CreditSale.STOCK_QTY,0) CreditSale,
		ISNULL( ReturnSup.STOCK_QTY,0) ReturnSup,
		ISNULL( ReturnOil.STOCK_QTY,0) ReturnOil,
		ISNULL( WithDraw.STOCK_QTY,0) WithDraw,
		ISNULL( Adjust.STOCK_QTY,0) Adjust,
		allProductId.*
	from(
		select distinct * from(
			select PD_ID, UNIT_ID , UNIT_BARCODE from INV_STOCK_DAILY(nolock) where 
				STOCK_DATE = @Yesterday 
				and COMP_CODE = @CompCode 
				and BRN_CODE = @BrnCode 
				and LOC_CODE = @LocCode
			union 
			select PD_ID, UNIT_ID , UNIT_BARCODE from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
			on hd.BRN_CODE = dt.BRN_CODE 
				and hd.COMP_CODE = dt.COMP_CODE 
				and hd.LOC_CODE = dt.LOC_CODE
				and hd.DOC_NO = dt.DOC_NO
			where hd.DOC_DATE = @SysDate
				and hd.COMP_CODE = @CompCode 
				and hd.BRN_CODE = @BrnCode 
				and hd.LOC_CODE = @LocCode
			union 
			select PD_ID, UNIT_ID , UNIT_BARCODE from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
			on hd.BRN_CODE = dt.BRN_CODE 
				and hd.COMP_CODE = dt.COMP_CODE 
				and hd.LOC_CODE = dt.LOC_CODE
				and hd.DOC_NO = dt.DOC_NO
			where hd.DOC_DATE = @SysDate
				and hd.COMP_CODE = @CompCode 
				and hd.BRN_CODE = @BrnCode 
				and hd.LOC_CODE = @LocCode
		) ap 
	) allProductId
	outer apply(
		select REMAIN from INV_STOCK_DAILY(nolock)
		where PD_ID = allProductId.PD_ID
			and STOCK_DATE = @YesterDay
			and COMP_CODE = @CompCode 
			and BRN_CODE = @BrnCode 
			and LOC_CODE = @LocCode
	) Balance
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_RECEIVE_PROD_HD(nolock)hd join INV_RECEIVE_PROD_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
		and hd.COMP_CODE = dt.COMP_CODE 
		and hd.LOC_CODE = dt.LOC_CODE
		and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) ReceiveProd
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
		and hd.COMP_CODE = dt.COMP_CODE 
		and hd.LOC_CODE = dt.LOC_CODE
		and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) TranferIn
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_TRANIN_HD(nolock)hd join INV_TRANIN_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
		and hd.COMP_CODE = dt.COMP_CODE 
		and hd.LOC_CODE = dt.LOC_CODE
		and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) TranferOut
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from SAL_CASHSALE_HD(nolock)hd join SAL_CASHSALE_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
		and hd.COMP_CODE = dt.COMP_CODE 
		and hd.LOC_CODE = dt.LOC_CODE
		and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) CashSale
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from SAL_CREDITSALE_HD(nolock)hd join SAL_CREDITSALE_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
			and hd.COMP_CODE = dt.COMP_CODE 
			and hd.LOC_CODE = dt.LOC_CODE
			and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) CreditSale
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_RETURN_SUP_HD(nolock)hd join INV_RETURN_SUP_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
			and hd.COMP_CODE = dt.COMP_CODE 
			and hd.LOC_CODE = dt.LOC_CODE
			and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) ReturnSup
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_RETURN_OIL_HD(nolock)hd join INV_RETURN_OIL_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
			and hd.COMP_CODE = dt.COMP_CODE 
			and hd.LOC_CODE = dt.LOC_CODE
			and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) ReturnOil
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_WITHDRAW_HD(nolock)hd join INV_WITHDRAW_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
			and hd.COMP_CODE = dt.COMP_CODE 
			and hd.LOC_CODE = dt.LOC_CODE
			and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) WithDraw
	outer apply(
		select SUM(dt.STOCK_QTY) STOCK_QTY 
		from INV_ADJUST_HD(nolock)hd join INV_ADJUST_DT(nolock)dt 
		on hd.BRN_CODE = dt.BRN_CODE 
			and hd.COMP_CODE = dt.COMP_CODE 
			and hd.LOC_CODE = dt.LOC_CODE
			and hd.DOC_NO = dt.DOC_NO
		where hd.DOC_DATE = @SysDate
			and hd.COMP_CODE = @CompCode 
			and hd.BRN_CODE = @BrnCode 
			and hd.LOC_CODE = @LocCode
			and dt.PD_ID = allProductId.PD_ID
			and dt.UNIT_BARCODE = allProductId.UNIT_BARCODE
			and dt.UNIT_ID = allProductId.UNIT_ID
			and hd.DOC_STATUS <> 'Cancel'
	) Adjust
) st
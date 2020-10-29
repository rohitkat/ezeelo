--USE [Ezeelo]
GO
/****** Object:  StoredProcedure [dbo].[Purchase_Report_Select]    Script Date: 2/7/2019 7:22:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--Execute Purchase_Report_Select 347233

ALTER PROC [dbo].[Purchase_Report_Select]
(
@UserID as bigInt 

)
as 
BEGIN
DECLARE @OrderCode varchar(100)
DECLARE @OrderAmt  float
DECLARE @PaymentMode varchar(10)
DECLARE @OrderStatus varchar(10)
DECLARE @BusinessPoints float
DECLARE @OrderDate DateTime
DECLARE @ID int
DECLARE @OrdStatus nvarchar(50)
DECLARE @OrdStatusNo int

create table #Network_temp
(
    OrderCode varchar(100),
    OrderAmt float,
	paymentmode varchar(10),
	TransactionPts float,
	OrderStatus varchar(10),
	OrderDate Datetime,
	OrdStatusNo int
)


--------------Declare Cursor1----------------------------------------
DECLARE Cursor2 CURSOR READ_ONLY for Select ID from CustomerOrder where UserLoginID = @UserID
OPEN Cursor2 Fetch NEXT FROM Cursor2  INTO @ID    

IF EXISTS (SELECT 1 from MLMWalletTransaction where CustomerOrderID = @ID and UserLoginID = @UserID)
BEGIN
   
   select @OrderCode=OrderCode , @OrderAmt=OrderAmount , @PaymentMode=Paymentmode  from CustomerOrder where ID = @ID
   select @BusinessPoints=sum(BusinessPoints) from CustomerOrderDetail where CustomerOrderID = @ID
   select @OrderStatus=max(OrderStatus) from CustomerOrderDetail where CustomerOrderID = @ID
   select @OrdStatusNo= @OrderStatus-- added by amit
   Select @OrderDate=CreateDate  from CustomerOrder where OrderCode = @OrderCode


   if @OrderStatus = 8 BEGIN SELECT @BusinessPoints = 0 END
   if @OrderStatus = 9 BEGIN SELECT @BusinessPoints = 0 END

   if @OrderStatus = 1 BEGIN SELECT @OrdStatus = 'PLACED' END
   if @OrderStatus = 2 BEGIN SELECT @OrdStatus = 'CONFIRM' END
   if @OrderStatus = 3 BEGIN SELECT @OrdStatus = 'PACKED' END
   if @OrderStatus = 4 BEGIN SELECT @OrdStatus = 'DISPATCH FROM SHOP' END
   if @OrderStatus = 5 BEGIN SELECT @OrdStatus = 'IN GODOWN' END
   if @OrderStatus = 6 BEGIN SELECT @OrdStatus = 'DISPATCH FROM GODOWN' END
   if @OrderStatus = 7 BEGIN SELECT @OrdStatus = 'DELIVERED' END
   if @OrderStatus = 8 BEGIN SELECT @OrdStatus = 'RETURNED' END
   if @OrderStatus = 9 BEGIN SELECT @OrdStatus = 'CANCELLED' END
   if @OrderStatus = 10 BEGIN SELECT @OrdStatus = 'ABANDONED' END   -- added by amit
   PRINT @OrderStatus;

   IF NOT EXISTS (SELECT 1 FROM #Network_temp WHERE OrderCode = @OrderCode)
   BEGIN
      insert into #Network_temp values ( @OrderCode , @OrderAmt , @PaymentMode , @BusinessPoints , @OrdStatus , @OrderDate, @OrdStatusNo)  --added @OrdStatusNo by amit
   END

END


WHILE @@FETCH_STATUS = 0
  BEGIN
   Fetch NEXT FROM Cursor2 INTO @ID

IF EXISTS (SELECT 1 from MLMWalletTransaction where CustomerOrderID = @ID and UserLoginID = @UserID)
BEGIN
   select @OrderCode=OrderCode , @OrderAmt=OrderAmount , @PaymentMode=Paymentmode from CustomerOrder where ID = @ID
   select @BusinessPoints=sum(BusinessPoints)  from CustomerOrderDetail where CustomerOrderID = @ID
   select @OrderStatus=max(OrderStatus) from CustomerOrderDetail where CustomerOrderID = @ID
   select @OrdStatusNo=@OrderStatus  -- added by amit

   Select @OrderDate=CreateDate  from CustomerOrder where OrderCode = @OrderCode

   if @OrderStatus = 8 BEGIN SELECT @BusinessPoints = 0 END
   if @OrderStatus = 9 BEGIN SELECT @BusinessPoints = 0 END

   if @OrderStatus = 1 BEGIN SELECT @OrdStatus = 'PLACED' END
   if @OrderStatus = 2 BEGIN SELECT @OrdStatus = 'CONFIRM' END
   if @OrderStatus = 3 BEGIN SELECT @OrdStatus = 'PACKED' END
   if @OrderStatus = 4 BEGIN SELECT @OrdStatus = 'DISPATCH FROM SHOP' END
   if @OrderStatus = 5 BEGIN SELECT @OrdStatus = 'IN GODOWN' END
   if @OrderStatus = 6 BEGIN SELECT @OrdStatus = 'DISPATCH FROM GODOWN' END
   if @OrderStatus = 7 BEGIN SELECT @OrdStatus = 'DELIVERED' END
   if @OrderStatus = 8 BEGIN SELECT @OrdStatus = 'RETURNED' END
   if @OrderStatus = 9 BEGIN SELECT @OrdStatus = 'CANCELLED' END
   if @OrderStatus = 10 BEGIN SELECT @OrdStatus = 'ABANDONED' END   -- ADDED by amit
   print @OrderStatus

   IF NOT EXISTS (SELECT 1 FROM #Network_temp WHERE OrderCode = @OrderCode)
    BEGIN
       insert into #Network_temp values ( @OrderCode , @OrderAmt , @PaymentMode , @BusinessPoints , @OrdStatus , @OrderDate, @OrdStatusNo) -- added @OrdStatusNo by amit
    END
END
END

CLOSE Cursor2
DEALLOCATE Cursor2

Select * from #Network_temp 

END


--exec Purchase_Report_Select 303054

                       


              


CREATE DATABASE QuanLyQuanCafe
GO
USE QuanLyQuanCafe
GO



-- Food
-- Table
-- Food Category(Danh mục)
-- Account(thông tin tài khoản người dùng)
-- Bill
-- BillInfo

CREATE TABLE TableInfo
(
	-- có thể thêm màu bàn, số lượng người giới hạn, đặt bàn
	ID INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL DEFAULT N'Chưa có tên',
	Status NVARCHAR(100) NOT NULL DEFAULT N'Trống',     -- Trống || Có người
)
GO

CREATE TABLE Account 
(
	UserName NVARCHAR(100) PRIMARY KEY,
	DisplayName NVARCHAR(100) NOT NULL DEFAULT N'TANH',
	PassWord NVARCHAR(1000) NOT NULL DEFAULT 1,
	Type INT NOT NULL DEFAULT 0		 -- 1. Admin 0. staff
)
GO

CREATE TABLE FoodCategory 
(
	ID INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
)
GO

CREATE TABLE Food 
(
	ID INT IDENTITY PRIMARY KEY,
	Name NVARCHAR(100) NOT NULL DEFAULT N'Chưa đặt tên',
	IDCategory INT NOT NULL,
	Price FLOAT NOT NULL  DEFAULT 0,

	FOREIGN KEY (IDCategory) REFERENCES FoodCategory(ID)
) 
GO

CREATE TABLE Bill 
(
	ID INT IDENTITY PRIMARY KEY,
	DateCheckIn DATE NOT NULL DEFAULT GETDATE(),
	DateCheckOut DATE,
	IDTable INT NOT NULL,
	Status INT NOT NULL DEFAULT 0,	 -- 1. Đã thanh toán 0.Chưa thanh toán

	FOREIGN KEY (IDTable) REFERENCES TableInfo(ID) ON DELETE CASCADE
) 
GO

CREATE TABLE BillInfo 
(
	ID INT IDENTITY PRIMARY KEY,
	IDBill INT NOT NULL,
	IDFood INT NOT NULL,
	Count INT NOT NULL DEFAULT 0,

	FOREIGN KEY (IDBill) REFERENCES Bill(ID) ON DELETE CASCADE,
	FOREIGN KEY (IDFood) REFERENCES Food(ID) ON DELETE CASCADE
)
GO


-- Store producer

-- Proc Account
CREATE PROC USP_GetAccountByUserName
	@userName NVARCHAR(100)
AS
BEGIN 
	SELECT * FROM dbo.Account WHERE UserName = @userName;
END
GO
-- End proc Account

-- Proc Login
CREATE PROC USP_Login 
	@userName NVARCHAR(100),
	@passWord NVARCHAR(100)
AS 
BEGIN 
	SELECT * FROM dbo.Account WHERE UserName = @userName AND PassWord = @passWord
END
GO
-- End proc Login

-- Proc Table info
CREATE PROC USP_GetTableList
AS
BEGIN 
	SELECT * FROM dbo.TableInfo
END
GO
-- End proc tableinfo

--- Proc Insert Bill
CREATE PROC USP_InsertBill
	@iDTable INT
AS 
BEGIN 
	INSERT INTO dbo.Bill (DateCheckIn, DateCheckOut, IDTable, Status, Discount)
	VALUES (GETDATE(), NULL, @iDTable, 0, 0)
END
GO
-- End Insert Bill

-- Insert BillInfo
CREATE PROC USP_InsertBillInfo
	@idBill INT, @idFood INT, @count INT
AS
BEGIN 
	DECLARE @isExitsBillInfo INT
	DECLARE @foodCount INT = 1

	SELECT @isExitsBillInfo = ID, @foodCount = Bi.Count
	FROM dbo.BillInfo  AS BI
	WHERE IDBill = @idBill AND IDFood = @idFood

	IF (@isExitsBillInfo > 0)
	BEGIN
		DECLARE @newCount INT = @foodCount + @count
		IF(@newCount > 0)
			UPDATE dbo.BillInfo SET Count = @foodCount + @count WHERE IDFood = @idFood AND idBill = @idBill
		ELSE 
			DELETE dbo.BillInfo WHERE IDBill = @idBill AND IDFood = @idFood
	END

	ELSE
	BEGIN
		INSERT dbo.BillInfo(IDBill, IDFood, Count)
		VALUES (@idBill, @idFood, @count)
	END
END
GO
-- End Insert Bill Info


--//////////////////////////////////////////////////////////////////////////////////////////////
-- TRIGGER
CREATE TRIGGER UTG_UpdateBillInfo
ON dbo.BillInfo FOR INSERT, UPDATE
AS 
BEGIN
    -- Xử lý nhiều hàng bằng cách dùng bảng inserted
    UPDATE dbo.TableInfo
    SET Status = N'Có người'
    WHERE ID IN (
        SELECT B.IDTable
        FROM dbo.Bill B
        INNER JOIN inserted I ON B.ID = I.IDBill
        WHERE B.Status = 0
    );
END


ALTER TRIGGER UTG_DeleteBillInfo
ON dbo.BillInfo FOR DELETE
AS 
BEGIN
    -- Xử lý nhiều hàng bằng bảng deleted
    DECLARE @iDTable INT;
    
    -- Xóa tất cả các hóa đơn BillInfo
    DELETE FROM dbo.BillInfo
    WHERE IDBill IN (SELECT IDBill FROM deleted);

    -- Lấy danh sách các bảng liên quan và kiểm tra còn BillInfo hay không
    SELECT @iDTable = IDTable FROM dbo.Bill WHERE ID IN (SELECT IDBill FROM deleted);

    IF NOT EXISTS (SELECT * FROM dbo.BillInfo WHERE IDBill IN (SELECT IDBill FROM deleted))
    BEGIN
        UPDATE dbo.TableInfo
        SET Status = N'Trống'
        WHERE ID = @iDTable;

        -- Xóa bill nếu không còn BillInfo nào
        DELETE FROM dbo.Bill WHERE ID IN (SELECT IDBill FROM deleted);
    END
END


ALTER TRIGGER UTG_UpdateBill
ON dbo.Bill FOR INSERT, UPDATE
AS 
BEGIN
    -- Đối với update
    UPDATE dbo.TableInfo
    SET Status = N'Có người'
    WHERE ID IN (
        SELECT IDTable FROM inserted WHERE Status = 0
    );

	UPDATE dbo.TableInfo
    SET Status = N'Trống'
    WHERE ID IN (
        SELECT IDTable FROM inserted WHERE Status = 1
    );
END

CREATE TRIGGER UTG_DeleteBill
ON dbo.Bill FOR DELETE
AS BEGIN
    -- Đối với delete
    UPDATE dbo.TableInfo
    SET Status = N'Trống'
    WHERE ID IN (
        SELECT IDTable FROM deleted
        WHERE NOT EXISTS (SELECT 1 FROM dbo.BillInfo WHERE IDBill = deleted.ID)
    );
END

CREATE TRIGGER UTG_DeleteCategory
ON dbo.Food FOR DELETE
AS 
BEGIN
    -- Xóa tất cả BillInfo liên quan đến các Food đã bị xóa
    DELETE FROM dbo.BillInfo
    WHERE IDFood IN (SELECT ID FROM deleted);
END
--/////////////////////////////////////END TRIGGER/////////////////////////////////////////////////////////

--//////////////////////////////////////////////////////////////////////////////////////////////
 -- Switch table Proc
CREATE PROC USP_SwitchTable
	@iDTable1 INT, @iDTable2 INT
AS BEGIN
	DECLARE @iDBill1 INT
	DECLARE @iDBill2 INT

	SELECT @iDBill1 = ID FROM dbo.Bill WHERE IDTable = @iDTable1 AND Status = 0
	SELECT @iDBill2 = ID FROM dbo.Bill WHERE IDTable = @iDTable2 AND Status = 0
			

	IF(@iDBill1 IS NULL AND @iDBill2 IS NOT NULL)
	BEGIN
		-- Tạo bill mới cho bàn 1 trống
		EXEC  USP_InsertBill @iDTable = @iDTable1
		-- Lấy idbill 1 với table mới được thêm vào
		SELECT @iDBill1 = ID FROM dbo.Bill WHERE IDTable = @iDTable1
		-- Update tất cả nhưng billinfo có idbill = idbill2 = idbill1
		UPDATE dbo.BillInfo SET IDBill = @iDBill1
		WHERE IDBill = @iDBill2

		-- Xóa bill từ bàn bill2 trước đó
		DELETE FROM dbo.Bill WHERE ID = @iDBill2 
	END

	ELSE IF(@iDBill2 IS NULL AND @iDBill1 IS NOT NULL)
	BEGIN
		EXEC  USP_InsertBill @iDTable = @iDTable2
		SELECT @iDBill2 = ID FROM dbo.Bill WHERE IDTable = @iDTable2

		UPDATE dbo.BillInfo SET IDBill = @iDBill2 
		WHERE IDBill = @iDBill1

		DELETE FROM dbo.Bill WHERE ID = @iDBill1;
	END

	ELSE IF (@iDBill1 IS NOT NULL AND @iDBill2 IS NOT NULL)
	BEGIN
		-- Hoán đổi vị trí 2 idtable
		DECLARE @temp INT = @iDTable1

		UPDATE dbo.Bill SET IDTable = @iDTable2 
		WHERE IDTable = @iDTable1 AND ID = @iDBill1

		UPDATE dbo.Bill SET IDTable = @iDTable1 
		WHERE IDTable = @iDTable2 AND ID = @iDBill2
	END
END
GO
--//////////////////////////////////////////////////////////////////////////////////////////////

--//////////////////////////////////////////////////////////////////////////////////////////////
-- Proc 
ALTER PROC USP_GetlistBillByDate
	@checkIn DATE, @checkOut DATE
AS BEGIN
	SELECT TI.Name AS [Tên bàn], FORMAT(DateCheckIn, 'dd / MM / yyyy') AS [Ngày vào], FORMAT(DateCheckOut, 'dd/MM/yyyy') AS [Ngày ra], Discount AS [Giảm giá], B.TotalPrice  AS [Tổng tiền thanh toán]
	FROM dbo.Bill AS B, dbo.TableInfo AS TI
	WHERE DateCheckIn >= @checkIn AND DateCheckOut <= @checkOut AND TI.ID = B.IDTable AND B.Status = 1
END
GO

ALTER PROC USP_UpdateAccount
	@userName NVARCHAR(100), @displayName NVARCHAR(100), @passWord NVARCHAR(100), @newPassWord NVARCHAR(100)
AS BEGIN 
	DECLARE @isValidatePass INT	= 0
	SELECT @isValidatePass = COUNT(*) FROM dbo.Account
	WHERE UserName = @userName AND PassWord = @passWord

	IF(@isValidatePass > 0) 
	BEGIN
		IF(@newPassWord IS NULL OR @newPassWord = '')
		BEGIN
			UPDATE dbo.Account SET DisplayName = @displayName
			WHERE UserName = @userName
		END

		ELSE 
		BEGIN
			UPDATE dbo.Account 
			SET DisplayName = @displayName, PassWord = @newPassWord
			WHERE UserName = @userName
		END
	END 
END 
GO
--//////////////////////////////////////////////////////////////////////////////////////////////

--//////////////////////////////////////////////////////////////////////////////////////////////
-- Hàm so sánh bằng chuỗi
CREATE FUNCTION [dbo].[utf8ConvertSQL]
(
      @strInput NVARCHAR(4000)
) 
RETURNS NVARCHAR(4000)
AS
Begin
	Set @strInput=rtrim(ltrim(lower(@strInput)))
    IF @strInput IS NULL RETURN @strInput
    IF @strInput = '' RETURN @strInput
    Declare @text nvarchar(50), @i int
    Set @text='-''`~!@#$%^&*()?><:|}{,./\"''='';–'
    Select @i= PATINDEX('%['+@text+']%',@strInput ) 
    while @i > 0
        begin
	        set @strInput = replace(@strInput, substring(@strInput, @i, 1), '')
	        set @i = patindex('%['+@text+']%', @strInput)
        End
        Set @strInput =replace(@strInput,'  ',' ')
        
    DECLARE @RT NVARCHAR(4000)
    DECLARE @SIGN_CHARS NCHAR(136)
    DECLARE @UNSIGN_CHARS NCHAR (136)
    SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệế
                  ìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵý'
                  +NCHAR(272)+ NCHAR(208)
    SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeee
                  iiiiiooooooooooooooouuuuuuuuuuyyyyy'
    DECLARE @COUNTER int
    DECLARE @COUNTER1 int
    SET @COUNTER = 1
    WHILE (@COUNTER <=LEN(@strInput))
    BEGIN   
      SET @COUNTER1 = 1
       WHILE (@COUNTER1 <=LEN(@SIGN_CHARS)+1)
       BEGIN
     IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) 
            = UNICODE(SUBSTRING(@strInput,@COUNTER ,1) )
     BEGIN           
          IF @COUNTER=1
              SET @strInput = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) 
              + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)-1)                   
          ELSE
              SET @strInput = SUBSTRING(@strInput, 1, @COUNTER-1) 
              +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) 
              + SUBSTRING(@strInput, @COUNTER+1,LEN(@strInput)- @COUNTER)
              BREAK
               END
             SET @COUNTER1 = @COUNTER1 +1
       END
       SET @COUNTER = @COUNTER +1
    End
 SET @strInput = replace(@strInput,' ','-')
    RETURN lower(@strInput)
End
--//////////////////////////////////////////////////////////////////////////////////////////////





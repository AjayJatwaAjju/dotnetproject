-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Inserting @ReplaceTableName details
-- ============================================
CREATE PROCEDURE [dbo].[sp_Set@ReplacePROCEDUREName]
-- Add the parameters for the stored procedure here
@ReplaceParameter,
@Success INT OUTPUT
AS
--Handling database error
--BEGIN TRY
 
 IF(@@pkColumnName=-1)
 BEGIN
	--Inserting the @ReplaceTableName details
	 INSERT INTO @ReplaceTableName(@ReplaceInsertColumn)
	 VALUES (@ReplaceInsertParameter);
	SET @@pkColumnName=@@IDENTITY;
	SELECT @@pkColumnName,@active,1 p_status,'Record inserted successfully.'p_message;
END
IF(@@pkColumnName!=-1 and @active!=2)
BEGIN
--Updateing the @ReplaceTableName details
   UPDATE @ReplaceTableName SET
   @ReplaceUpdateParameter
  WHERE @pkColumnName=@@pkColumnName;
	SELECT @@pkColumnName,@active,1 p_status,'Record updated successfully.'p_message;
END
IF(@@pkColumnName != -1 and @active = 2)
  BEGIN
  UPDATE @ReplaceTableName SET
   active = @active,
   modifiedon = getDate()
  WHERE @pkColumnName=@@pkColumnName;
  SELECT @@pkColumnName,@active,1 p_status,'Record deleted successfully.'p_message;
END
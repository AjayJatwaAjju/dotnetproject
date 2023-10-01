-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Inserting @ReplaceTableName details
-- ============================================
CREATE PROCEDURE [dbo].[spInsert@ReplacePROCEDUREName]
-- Add the parameters for the stored procedure here
@ReplaceParameter,
@Success INT OUTPUT
AS
--Handling database error
BEGIN TRY
 SET @Success=0
--Inserting the @ReplaceTableName details
 INSERT INTO @ReplaceTableName(@ReplaceInsertColumn)
 VALUES (@ReplaceInsertParameter) 

--Set success parameter
SET @Success=1 

END TRY
BEGIN CATCH

--Inserting error into error log table.
EXEC spErrorLog

END CATCH
RETURN
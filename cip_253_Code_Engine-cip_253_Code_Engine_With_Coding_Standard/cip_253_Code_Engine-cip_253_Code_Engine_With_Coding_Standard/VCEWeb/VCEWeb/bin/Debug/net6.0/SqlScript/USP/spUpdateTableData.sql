-- ============================================
-- Author: VCE
-- Create date: 11/05/2023 1:11 PM
-- Description: Update @ReplaceTableName details
-- ============================================
CREATE PROCEDURE [dbo].[spUpdate@ReplacePROCEDUREName]
-- Add the parameters for the stored procedure here
@ReplaceParameter,
@Success INT OUTPUT 
AS
--Handling database error
BEGIN TRY
   SET @Success=0
--Updateing the @ReplaceTableName details
  UPDATE @ReplaceTableName set
  @ReplaceUpdateParameter
  @Replacewherecondition

--Set success parameter
 SET @Success=1

END TRY
BEGIN CATCH

-- Inserting error into error log table.
EXEC spErrorLog
 

END CATCH
RETURN
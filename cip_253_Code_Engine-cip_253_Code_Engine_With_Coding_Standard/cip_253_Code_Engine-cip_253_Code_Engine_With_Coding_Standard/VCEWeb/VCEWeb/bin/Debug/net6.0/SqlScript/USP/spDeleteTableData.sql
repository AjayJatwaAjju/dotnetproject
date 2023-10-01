-- ============================================ 
-- Author: VCE 
-- Create date: 11/05/2023 1:11 PM 
-- Description: Delete @ReplaceTableName details    
-- ============================================ 
CREATE PROCEDURE [dbo].[spDelete@ReplacePROCEDUREName] 
 
-- Add the parameters for the stored procedure here
@ReplaceParameter UNIQUEIDENTIFIER, 
@Success INT OUT   
 
AS 
 
BEGIN TRY  
SET @Success=0

--Seleting the @ReplaceTableName details
DELETE FROM @ReplaceTableName 
@Replacewherecondition

--Set Success parameter
SET @Success=1 

END TRY  
BEGIN CATCH  

-- Inserting error into error log table. 
EXEC spErrorLog 

END CATCH 
RETURN 
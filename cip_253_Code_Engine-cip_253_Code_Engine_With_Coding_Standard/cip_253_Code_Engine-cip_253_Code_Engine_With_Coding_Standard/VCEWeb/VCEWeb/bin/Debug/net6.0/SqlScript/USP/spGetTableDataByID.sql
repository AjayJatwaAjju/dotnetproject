-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Getting @ReplaceTableName Details by Id    
-- ============================================    
CREATE PROCEDURE [dbo].[spGet@ReplacePROCEDURENameByID]    
   
-- Add the parameters for the stored procedure here      
@ReplaceParameter

AS    
   
--Handling database error    
BEGIN TRY      
--Getting the @ReplaceTableName details
   
SELECT * FROM @ReplaceTableName
@Replacewherecondition

END TRY      
BEGIN CATCH 

-- Inserting error into error log table. 
EXEC spErrorLog   

END CATCH    
RETURN  
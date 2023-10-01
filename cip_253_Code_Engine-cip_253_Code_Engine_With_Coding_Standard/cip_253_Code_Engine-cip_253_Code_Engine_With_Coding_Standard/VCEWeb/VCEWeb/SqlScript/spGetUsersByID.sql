-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Getting Users Details by Id    
-- ============================================    
CREATE PROCEDURE [dbo].[spGetUsersByID]    
   
-- Add the parameters for the stored procedure here      
@UserGuid UNIQUEIDENTIFIER  

AS    
   
--Handling database error    
BEGIN TRY      
--Getting the Users details
   
SELECT * FROM VCE_Users WHERE UserGuid = @UserGuid 

END TRY      
BEGIN CATCH 

-- Inserting error into error log table. 
EXEC spErrorLog   

END CATCH    
RETURN  
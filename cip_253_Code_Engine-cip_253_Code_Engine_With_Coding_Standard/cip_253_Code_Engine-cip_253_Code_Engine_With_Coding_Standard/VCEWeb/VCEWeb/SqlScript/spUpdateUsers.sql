-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Update Users details          
-- ============================================    
CREATE PROCEDURE [dbo].[spUpdateUsers]    
    
-- Add the parameters for the stored procedure here
@UserGuid UNIQUEIDENTIFIER,    
@UserFirstName NVARCHAR(200),    
@UserLastName NVARCHAR(200),    
@UserCode INT,    
@UserEmailId NVARCHAR(90),    
@UserPassword NVARCHAR(200),
@Success INT OUTPUT 

AS    
    
--Handling database error     
   BEGIN TRY      
   SET @Success=0  

--Updateing the Users details      
   UPDATE VCE_Users    
   SET        
   UserFirstName = @UserFirstName,    
   UserLastName = @UserLastName,       
   UserEmailId = @UserEmailId,    
   UserPassword = @UserPassword   
   WHERE UserGuid = @UserGuid 

--Set success parameter
   SET @Success=1  

END TRY      
BEGIN CATCH

-- Inserting error into error log table.     
EXEC spErrorLog   

END CATCH    
RETURN  
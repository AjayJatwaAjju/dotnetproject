-- ============================================      
-- Author: VCE      
-- Create date: 11/05/2023 1:11 PM     
-- Description: Inserting Users details            
-- ============================================      
CREATE PROCEDURE [dbo].[spInsertUsers]      
      
-- Add the parameters for the stored procedure here
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
   SET @UserGuid = NEWID()        
          
--Inserting the Users details        
  INSERT INTO VCE_Users(UserGuid,      
   UserFirstName,      
   UserLastName,        
   UserEmailId,      
   UserPassword)      
  VALUES (@UserGuid ,      
   @UserFirstName,      
   @UserLastName,        
   @UserEmailId,      
   @UserPassword) 

--Set success parameter
SET @Success=1 

END TRY        
BEGIN CATCH

--Inserting error into error log table.       
EXEC spErrorLog 

END CATCH      
RETURN  
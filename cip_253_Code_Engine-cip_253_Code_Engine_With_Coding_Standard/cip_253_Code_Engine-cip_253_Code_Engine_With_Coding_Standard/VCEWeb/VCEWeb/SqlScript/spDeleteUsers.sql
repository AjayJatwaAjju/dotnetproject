-- ============================================  
-- Author: VCE  
-- Create date: 11/05/2023 1:11 PM 
-- Description: Delete Users details        
-- ============================================  
CREATE PROCEDURE [dbo].[spDeleteUsers]  
 
-- Add the parameters for the stored procedure here
@UserGuid UNIQUEIDENTIFIER,  
@Success INT OUT      
 
AS  
 
BEGIN TRY    
SET @Success=0

--Seleting the Users details
DELETE FROM VCE_Users WHERE UserGuid = @UserGuid  

--Set Success parameter
SET @Success=1  

END TRY    
BEGIN CATCH   

-- Inserting error into error log table.  
EXEC spErrorLog  

END CATCH  
RETURN  
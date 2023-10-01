-- ============================================      
-- Author: VCE      
-- Create date: 11/05/2023 1:11 PM      
-- Description: Getting Users Details.      
-- ============================================      
CREATE PROCEDURE [dbo].[spGetUsers]
      
-- Add the parameters for the stored procedure here     
@whereCondition NVARCHAR(MAX),     
@orderBy NVARCHAR(MAX)      
      
AS      
      
--Handling database error       
BEGIN TRY        
--Handling database error        
      
IF ISNULL(@whereCondition,'')<>''
BEGIN 
SET @whereCondition=' WHERE '+ @whereCondition 
END      
      
IF ISNULL(@orderBy,'')<>''
BEGIN 
SET @orderBy=' ORDER BY '+@orderBy
END      
      
--Getting the Users details 
EXEC ('SELECT [pkUserId]      
      ,[UserGuid]      
      ,[UserFirstName]      
      ,[UserLastName]    
      ,[UserFirstName]+'' ''+[UserLastName] as UserFullName      
      ,[UserCode]      
      ,[UserEmailId]      
      ,[UserPassword]       
       FROM [dbo].[VCE_Users] '+ @whereCondition + @orderBy )      
      
END TRY
BEGIN CATCH

-- Inserting error into error log table.
EXEC spErrorLog       
      
END CATCH
RETURN  
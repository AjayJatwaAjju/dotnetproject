
-- ============================================ 
-- Author: VCE 
-- Create date: 11/05/2023 1:11 PM 
-- Description: Getting @ReplaceTableName Details. 
-- ============================================ 
CREATE PROCEDURE [dbo].[spGet@ReplaceTableName]
 
-- Add the parameters for the stored procedure here 
@whereCondition NVARCHAR(MAX), 
@orderBy NVARCHAR(MAX),
@PageStart int=0,
@PageSize int=-1
AS 
 

IF ISNULL(@PageSize,-1)=-1
BEGIN 
SET @PageSize=50000
END 

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
 
--Getting the @ReplaceTableName details 

IF ISNULL(@PageSize,-1)=-1
BEGIN 
	EXEC ('SELECT *,COUNT(*) OVER () AS CNT FROM @ReplaceTableName '+ @whereCondition + @orderBy)
END 
ELSE
BEGIN 
   EXEC ('SELECT * ,COUNT(*) OVER () AS CNT FROM @ReplaceTableName '+ @whereCondition + @orderBy +' OFFSET '+@PageStart+
    ' ROWS FETCH NEXT '+@PageSize+' ROWS ONLY') ;
END
 
END TRY
BEGIN CATCH

-- Inserting error into error log table.
EXEC spErrorLog  
 
END CATCH
GO
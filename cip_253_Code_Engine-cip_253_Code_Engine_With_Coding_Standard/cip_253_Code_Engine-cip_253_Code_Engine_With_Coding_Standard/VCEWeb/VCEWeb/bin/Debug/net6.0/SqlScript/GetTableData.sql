declare 
--@whereCondition NVARCHAR(MAX)='@ReplaceWhereCondition',
@orderBy NVARCHAR(MAX)='@ReplaceOrderBy',
@PageStart int=@ReplacePageStart,
@PageSize int=@ReplacePageSize,
@TableName NVARCHAR(MAX)='@ReplaceTableName'


IF ISNULL(@PageSize,-1)=-1
BEGIN 
SET @PageSize=50000
END

IF ISNULL(@whereCondition,'')<>''
BEGIN 
SET @whereCondition=' WHERE '+ @whereCondition 
END

IF ISNULL(@orderBy,'')<>''
BEGIN 
SET @orderBy=' ORDER BY '+@orderBy
END
else
begin
SET @orderBy=' ORDER BY 1'
end
EXEC ('Select *,COUNT(*) OVER () AS CNT from '+@TableName+' '+ @whereCondition + 
@orderBy +
' OFFSET '+@PageStart+
' ROWS FETCH NEXT '+@PageSize+' ROWS ONLY')
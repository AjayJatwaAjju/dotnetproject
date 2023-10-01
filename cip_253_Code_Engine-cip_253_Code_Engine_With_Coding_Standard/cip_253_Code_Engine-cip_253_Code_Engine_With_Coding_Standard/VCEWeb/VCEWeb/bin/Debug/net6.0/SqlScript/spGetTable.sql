USE [VCE_DemoDatabase]
GO
/****** Object:  StoredProcedure [dbo].[spGetbase_exports]    Script Date: 7/25/2023 8:27:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ============================================ 
-- Author: VCE 
-- Create date: 11/05/2023 1:11 PM 
-- Description: Getting @ReplaceTableName Details. 
-- ============================================ 
ALTER PROCEDURE [dbo].[spGet@ReplacePROCEDUREName]
 
-- Add the parameters for the stored procedure here 
@searchtext NVARCHAR(MAX)='', 
@orderBy_column NVARCHAR(MAX), 
@orderby_dir NVARCHAR(MAX),
@PageStart INT=0,
@PageSize INT=-1,
@@pkColumnName BIGINT=-1
AS 
BEGIN

IF ISNULL(@PageSize,-1)=-1
BEGIN 
SET @PageSize=50000
END 
SELECT *,COUNT(*) OVER () AS cnt FROM @ReplaceTableName  basetable
WHERE 1 = 1
			   and active<>2
				AND (@@pkColumnName = - 1
					OR (@@pkColumnName <> - 1
						AND basetable.@pkColumnName = @@pkColumnName
						))
				AND  
					(@searchtext = ''
					OR (@searchtext <> ''
						AND (1 = 2 @ReplaceWhereCondition)
						))
                    @ReplaceOrderBY
					OFFSET @PageStart ROWS FETCH NEXT @PageSize ROWS ONLY;
END
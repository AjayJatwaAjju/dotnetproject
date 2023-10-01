-- ============================================= 
-- Author: VCE 
-- Create date: 11/05/2023 1:11 PM 
-- Description: This is for getting @ReplaceTableName based on @pkid 
-- ============================================= 
CREATE OR REPLACE FUNCTION fnGet@ReplacePROCEDURENameById
(
-- Add the parameters for the function here 
    @pkid INT
)
RETURNS text AS
$$
DECLARE
    @UserName text;
BEGIN
    -- Add the T-SQL statements to compute the return value here
    SELECT @UserName FROM @ReplaceTableName 
	@Replacewherecondition
   
    -- Return the result of the function
    RETURN @UserName;
END;
$$
LANGUAGE plpgsql;

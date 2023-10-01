-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Getting @ReplaceTableName Details by Id    
-- ============================================    
CREATE OR REPLACE FUNCTION public.spGet@ReplacePROCEDURENameByID
(
-- Add the parameters for the stored procedure here
@ReplaceParameter TEXT
)
RETURNS VOID AS
$$
BEGIN
-- Handling database error
BEGIN
-- Getting the ReplaceTableName details
EXECUTE 'SELECT * FROM ' || @ReplaceTableName || ' ' || @Replacewherecondition;
EXCEPTION
-- Inserting error into error log table.
-- EXECUTE public.spErrorLog;
END;
END;
$$
LANGUAGE plpgsql;


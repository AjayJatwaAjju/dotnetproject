-- ============================================ 
-- Author: VCE 
-- Create date: 11/05/2023 1:11 PM 
-- Description: Delete @ReplaceTableName details    
-- ============================================ 
CREATE OR REPLACE PROCEDURE "public".spDelete@ReplacePROCEDUREName(
-- Add the parameters for the stored procedure here in inputjson datatable
	inputjson TEXT,
	INOUT outjson refcursor)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE "p_@pkColumnName" BIGINT =-1;
DECLARE p_status BIGINT =-1;
DECLARE p_message CHARACTER VARYING ='';
BEGIN 
--Seleting the @ReplaceTableName details
SELECT (json_array_elements(inputJson::JSON) ->> '@pkColumnName')::BIGINT INTO "p_@pkColumnName";

IF "p_@pkColumnName"<>-1  
THEN
	SELECT 1,'Record deleted successfully.' INTO p_status,p_message;
	UPDATE "@ReplaceTableName"
	SET isactive=0 --change the column name and value according to table
	WHERE @pkColumnName="p_@pkColumnName";
END IF;
OPEN outjson
FOR (SELECT p_status,p_message);
EXCEPTION WHEN OTHERS THEN PERFORM spErrorLog('spDelete@ReplacePROCEDUREName',SQLERRM,SQLSTATE);
END;
$BODY$;

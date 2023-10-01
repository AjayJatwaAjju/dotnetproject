
-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Updating spUpdate@ReplacePROCEDUREName Details   
-- ============================================ 
CREATE OR REPLACE PROCEDURE "@ReplaceSchema".spUpdate@ReplacePROCEDUREName(
	in_inputjson TEXT,
	INOUT outjson refcursor)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE "p_@pkColumnName" BIGINT =-1;
DECLARE p_active BIGINT =-1;
DECLARE p_status BIGINT =-1;
DECLARE p_message CHARACTER VARYING ='';
BEGIN 

DROP TABLE IF EXISTS inputdata_@ReplaceTableName;

CREATE TEMP TABLE inputdata_@ReplaceTableName AS
SELECT @ReplaceParameter
FROM (SELECT in_inputJson inputJson) s;

SELECT @pkColumnName,active INTO "p_@pkColumnName",p_active FROM inputdata_@ReplaceTableName;

IF "p_@pkColumnName"<>-1 AND p_active<>2  THEN
	 SELECT 1,'Record updated successfully.' INTO p_status,p_message;
	  UPDATE "@ReplaceSchema"."@ReplaceTableName"
	  SET @ReplaceUpdateParameter
		FROM inputdata_@ReplaceTableName temptable
		WHERE "@ReplaceTableName"."@pkColumnName"=temptable."@pkColumnName"
		AND COALESCE(temptable."@pkColumnName",-1)<>-1
		;
 END IF;
		OPEN outjson
		FOR (Select "p_@pkColumnName" @pkColumnName,p_active,p_status,p_message);
EXCEPTION WHEN OTHERS THEN PERFORM spErrorLog('spUpdate@ReplacePROCEDUREName',SQLERRM,SQLSTATE);
END;
$BODY$;

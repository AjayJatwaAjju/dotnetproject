
-- ============================================    
-- Author: VCE    
-- Create date: 11/05/2023 1:11 PM   
-- Description: Creating spInsert@ReplacePROCEDUREName Details   
-- ============================================ 
CREATE OR REPLACE PROCEDURE "@ReplaceSchema".spInsert@ReplacePROCEDUREName(
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

IF "p_@pkColumnName"=-1
	 THEN 
	 SELECT 1,'Record inserted successfully.' INTO p_status,p_message;
     INSERT INTO "@ReplaceSchema"."@ReplaceTableName"(
     @ReplaceInsertColumn)
	SELECT @ReplaceInsertColumn
	FROM inputdata_@ReplaceTableName temptable WHERE COALESCE(temptable."@pkColumnName",-1)=-1
	returning @pkColumnName INTO "p_@pkColumnName";
 END IF; 
		OPEN outjson
		FOR (SELECT "p_@pkColumnName" @pkColumnName,p_active,p_status,p_message);
EXCEPTION WHEN OTHERS THEN PERFORM spErrorLog('spInsert@ReplacePROCEDUREName',SQLERRM,SQLSTATE);
END;
$BODY$;

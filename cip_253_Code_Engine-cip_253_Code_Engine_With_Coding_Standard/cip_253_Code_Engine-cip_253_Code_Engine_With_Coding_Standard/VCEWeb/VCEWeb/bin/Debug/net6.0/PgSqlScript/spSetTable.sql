
CREATE OR REPLACE PROCEDURE "@ReplaceSchema".set_@ReplacePROCEDUREName(
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
-- Create Table Procedure
CREATE TEMP TABLE inputdata_@ReplaceTableName AS
SELECT @ReplaceParameter
FROM (SELECT in_inputJson inputJson) s;
SELECT @pkColumnName,active INTO "p_@pkColumnName",p_active FROM inputdata_@ReplaceTableName;
-- Get Table Procedure
IF "p_@pkColumnName"=-1
	 THEN      
	 SELECT 1,'Record inserted successfully.' INTO p_status,p_message;
     INSERT INTO "@ReplaceSchema"."@ReplaceTableName"(
     @ReplaceInsertColumn)

	SELECT @ReplaceInsertColumn
	FROM inputdata_@ReplaceTableName temptable WHERE COALESCE(temptable."@pkColumnName",-1)=-1
	returning @pkColumnName INTO "p_@pkColumnName";
	-- Update Table Procedure
ELSIF "p_@pkColumnName"<>-1 AND p_active<>2  THEN
	 SELECT 1,'Record updated successfully.' INTO p_status,p_message;
	  UPDATE "@ReplaceSchema"."@ReplaceTableName"
	  SET @ReplaceUpdateParameter
		FROM inputdata_@ReplaceTableName temptable
		WHERE "@ReplaceTableName"."@pkColumnName"=temptable."@pkColumnName"
		AND COALESCE(temptable."@pkColumnName",-1)<>-1;
	-- Delete Table Procedure
ELSIF "p_@pkColumnName"<>-1 AND p_active=2 THEN	
	SELECT 1,'Record deleted successfully.' INTO p_status,p_message;
	 UPDATE "@ReplaceSchema"."@ReplaceTableName"

	  SET "active" = p_active
	  ,modifiedon=now()
	  WHERE @pkColumnName="p_@pkColumnName";
 END IF;
 
	   
		OPEN outjson
		FOR (SELECT "p_@pkColumnName" @pkColumnName,p_active,p_status,p_message);	 
END;
$BODY$;

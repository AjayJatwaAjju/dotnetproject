CREATE OR REPLACE PROCEDURE "@ReplaceSchema".get_@ReplacePROCEDUREName(
	in_inputjson TEXT,
	INOUT outjson refcursor)
LANGUAGE 'plpgsql'
AS $BODY$
BEGIN 

DROP TABLE IF EXISTS inputdata_@ReplaceTableName;
CREATE TEMP TABLE inputdata_@ReplaceTableName AS
SELECT 
(json_array_elements(inputJson::JSON) ->> 'length')::BIGINT "length"
,(json_array_elements(inputJson::JSON) ->> 'start')::BIGINT "start"
,((json_array_elements(inputJson::JSON) ->> 'search')::JSON ->> 'value')::CHARACTER VARYING "search"
,(json_array_elements((json_array_elements(inputJson::JSON) ->> 'order')::JSON) ->> 'column')::CHARACTER VARYING "column"
,(json_array_elements((json_array_elements(inputJson::JSON) ->> 'order')::JSON) ->> 'dir')::CHARACTER VARYING "dir", 

@ReplaceParameter 
FROM (
	SELECT 		
		in_inputJson
	inputJson
	) s;

UPDATE inputdata_@ReplaceTableName SET "@pkColumnName"=COALESCE("@pkColumnName",-1);
	   
	   
		OPEN outjson
		FOR (SELECT *,COUNT(*) OVER () AS CNT
			FROM "@ReplaceTableName" basetable
			LEFT JOIN inputdata_@ReplaceTableName fltr ON 1 = 1
			WHERE 1 = 1 AND active<>2
				AND (fltr."@pkColumnName" = - 1
					OR (fltr."@pkColumnName" <> - 1
						AND basetable."@pkColumnName" = fltr."@pkColumnName"
						))
				AND (fltr.search = ''
					OR (fltr.search <> ''
						AND (1 = 2@ReplaceWhereCondition)
						))					
					@ReplaceOrderBY
					OFFSET (SELECT MAX("start") FROM inputdata_@ReplaceTableName) LIMIT (SELECT MAX("length") FROM inputdata_@ReplaceTableName));
END;
$BODY$;

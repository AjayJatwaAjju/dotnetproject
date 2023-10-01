drop table if exists db_data ;
create temp table db_data as
 Select (json_array_elements(inputJson::json) ->> 'pkvalue')::bigint "@pkid"
 ,((json_array_elements(inputJson::json) ->> 'search')::Json ->> 'value')::CHARACTER VARYING "search"
	
     from ( Select '@replaceinputJson' inputJson
	 ) s;
	
	--Select * from db_data; 
	 
	Select *,COUNT(*) OVER () AS CNT from "@ReplaceTableName" basetable
	left join db_data fltr on 1=1
	where 1=1
	and (fltr."@pkid"= -1 or (fltr."@pkid" <>-1 and basetable."@pkid"=fltr."@pkid"))
	and (fltr.search= '' or (fltr.search <>'' and (
	     1=2
			 @replaceWhereConditions
	        )
	      )
		)
	;
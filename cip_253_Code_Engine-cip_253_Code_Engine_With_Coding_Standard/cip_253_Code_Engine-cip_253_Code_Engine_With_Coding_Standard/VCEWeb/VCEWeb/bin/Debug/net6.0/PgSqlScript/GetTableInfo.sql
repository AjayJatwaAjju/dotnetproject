SELECT 
table_schema ,
    culm.ordinal_position column_id,
    column_name ColumnName,
    culm.data_type "Datatype1",
    0 "MaxLength",
    culm.numeric_precision "precision",
    culm.numeric_scale "scale" ,
    case when culm.is_nullable='YES' then cast(1 as bit) else cast(0 as bit) end is_nullable,
	coalesce(indisprimary, false) "PrimaryKey",
    case when indisprimary =true then cast(0 as bit) else cast(1 as bit) end IsVisibale,
    case when indisprimary =true or coalesce(culm.is_generated, 'NEVER')!= 'NEVER' then cast(0 as bit) else cast(1 as bit) end Iseditable,
    case when indisprimary =true then cast(0 as bit) else cast(1 as bit) end allowfiltering,
	culm.is_generated ,
	'"'||table_schema||'"."'||culm.table_name||'"'
--select * 
from information_schema.columns as culm
left join (
	
	SELECT indisprimary,pg_class.oid column_id,
	cast(pg_class.oid::regclass as varchar(100)) "table_name",
	  pg_attribute.attname ColumnName, 
	  format_type(pg_attribute.atttypid, pg_attribute.atttypmod)  
	,nspname
	FROM pg_index, pg_class, pg_attribute, pg_namespace 
	WHERE 
	  indrelid = pg_class.oid AND 
	  pg_class.relnamespace = pg_namespace.oid AND 
	  pg_attribute.attrelid = pg_class.oid AND 
	  pg_attribute.attnum = any(pg_index.indkey)
	 AND indisprimary
	and cast(pg_class.oid::regclass as varchar(100)) like '%YourTableName%'
 ) s on s.table_name like '%'||culm.table_name||'%' and s.ColumnName=culm.column_name
 and nspname=table_schema
WHERE 1=1
and table_schema = 'Yourtableschema' 
AND culm.table_name = 'YourTableName'
order by culm.ordinal_position
;

 


  
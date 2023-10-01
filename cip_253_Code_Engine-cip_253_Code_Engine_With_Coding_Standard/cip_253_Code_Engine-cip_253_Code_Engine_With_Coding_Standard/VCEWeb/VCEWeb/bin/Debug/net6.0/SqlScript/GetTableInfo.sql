SELECT 
    c.column_id,
    c.name 'ColumnName',
    t.Name 'Datatype1',
    c.max_length 'MaxLength',
    c.precision ,
    c.scale ,
    c.is_nullable,
     ISNULL(ic.is_primary_key, 0) 'PrimaryKey',
     case when ISNULL(ic.is_primary_key, 0)=0 then cast(1 as bit) else cast(0 as bit) end IsVisibale,
     case when ISNULL(ic.is_primary_key, 0)= 1 or ISNULL(c.generated_always_type, 0)<> 0 then cast(0 as bit) else cast(1 as bit) end Iseditable,
     case when ISNULL(ic.is_primary_key, 0)=0 then cast(1 as bit) else cast(0 as bit) end allowfiltering,
	 c.generated_always_type
FROM    
    sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
LEFT OUTER JOIN (
	Select ic.object_id,ic.column_id,i.is_primary_key from sys.index_columns ic 
   LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
   where ISNULL(i.is_primary_key, 0)=1
   ) ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE
    c.object_id = OBJECT_ID('YourTableName')
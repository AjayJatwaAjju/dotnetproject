SELECT 
    c.column_id,
    c.name 'ColumnName',
    t.Name 'Datatype1',
    c.max_length 'MaxLength',
    c.precision ,
    c.scale ,
    c.is_nullable,
     ISNULL(ic.is_primary_key, 0) 'PrimaryKey',
     cast(1 as bit) IsVisibale,
     cast(1 as bit) allowfiltering
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
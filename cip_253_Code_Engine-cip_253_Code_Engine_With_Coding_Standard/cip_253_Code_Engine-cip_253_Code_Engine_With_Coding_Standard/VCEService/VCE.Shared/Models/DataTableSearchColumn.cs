
namespace VCE.Shared.Models
{
    /// <summary>
    /// Represents a column in DataTable search criteria
    /// </summary>
    public class DataTableSearchColumn
    {
        public string data { get; set; }         
        public string name { get; set; }         
        public string? dataType { get; set; }    
        public bool? bVisible { get; set; }      
        public bool searchable { get; set; }     
        public bool orderable { get; set; }      
        public Search search { get; set; }       
    }
    /// <summary>
    /// Represents the search criteria for DataTable
    /// </summary>
    public class DataTableSearchData
    {
        public int draw { get; set; }              
        public string pkid { get; set; }           
        public string pkvalue { get; set; }        
        public string tableName { get; set; }      
        public List<DataTableSearchColumn> columns { get; set; }
        public List<Order> order { get; set; }     
        public int start { get; set; }             
        public int length { get; set; }            
        public Search search { get; set; }         
    }
    /// <summary>
    /// Represents search criteria for a single column
    /// </summary>
    public class Search
    {
        public string value { get; set; }
        public bool regex { get; set; } 
    }
    /// <summary>
    /// Represents sorting order for DataTable
    /// </summary>
    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
    /// <summary>
    /// Represents the output structure for DataTable
    /// </summary>
    public class DataTableOutput
    {
        public int draw { get; set; }  
        public object data { get; set; } 
        public int recordsFiltered { get; set; }
        public int recordsTotal { get; set; }
    }
}
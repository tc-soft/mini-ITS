namespace mini_ITS.Core.Database
{
    public class SqlQueryCondition
    {
        public string Name { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }
}
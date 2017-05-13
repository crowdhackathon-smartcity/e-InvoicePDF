using SQLite.Net.Attributes;

namespace eInvoicePdf.Core.Model
{
    public class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace api.Models
{    
    public class Transaction {
        public override string ToString()
        {
            return ID;
        }
        [Key]
        [Column("transaction_id")]
        public string ID {get; set;}
        [Column("date")]
        public string Date {get; set;}
        [Column("amount")]
        public string Amount{get; set;}
        [Column("price")]
        public string Price {get; set;}
        [Column("fees")]
        public string Fees {get; set;}
        [Column("final_amount")]
        public string FinalAmount {get; set;}
        [Column("status")]
        public string Status {get; set;}
    }
}

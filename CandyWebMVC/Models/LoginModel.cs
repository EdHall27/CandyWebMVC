using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CandyWebMVC.Models
{
    [Table("Login")] // Nome da tabela no banco de dados
    public class LoginModel
    {
        [Key]
        public int LoginId { get; set; }

        [ForeignKey("User")]
        public int CPFID { get; set; }


        public string PasswordHash { get; set; }

        public virtual User? User { get; set; }
    }
}

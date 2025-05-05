using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models.CQRS
{
    public class UserRemoved
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        //public User User { get; set; }

        #region represent the alternate key

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public DateTime RemovedDate { get; set; }

        #endregion
    }
}

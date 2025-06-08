using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using UserService.Models.CQRS;

namespace UserService.Models
{
    ///
    public class User
    {
        [Required]
        public Guid Guid { get; set; } // Alternate key for the User class

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public User()
        {
            
        }

        /// <summary>
        /// These properties are used to create a relationship with Person class (1 to many relationship)
        /// </summary>
        #region Navigation Properties
        //[JsonIgnore]
        public ICollection<UserDescription> UserDescriptions { get; set; } = new List<UserDescription>();
        [JsonIgnore]
        public ICollection<UserRemoved> UserRemoved { get; set; } = new List<UserRemoved>();

        #endregion



    }
}

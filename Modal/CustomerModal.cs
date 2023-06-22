using System.ComponentModel.DataAnnotations;

namespace Dotnet7API.Modal
{
    public class CustomerModal
    {
        [Key]
        public int Code { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [StringLength(50)]
        public string? CreditLimit { get; set; }

        public bool? IsActive { get; set; }

        public int? Taxcode { get; set; }
        public string? StatusName { get; set; }
    }
}

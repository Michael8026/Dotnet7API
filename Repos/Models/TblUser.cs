using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dotnet7API.Repos.Models;

[Table("tbl_User")]
public partial class TblUser
{
    [Key]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? Password { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public bool? Isactive { get; set; }

    [StringLength(50)]
    public string? Role { get; set; }
}

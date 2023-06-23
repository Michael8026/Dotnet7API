using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dotnet7API.Repos.Models;

[Table("tbl_RefreshToken")]
public partial class TblRefreshToken
{
    [Key]
    [StringLength(50)]
    public string UserId { get; set; } = null!;

    [StringLength(50)]
    public string? TokenId { get; set; }

    public string? RefreshToken { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dotnet7API.Repos.Models;

[Keyless]
[Table("tbl.User")]
public partial class TblUser
{
    [StringLength(50)]
    public string? User { get; set; }
}

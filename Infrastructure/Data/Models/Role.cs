using System;
using System.Collections.Generic;

namespace BackPruebaTecnica.Infrastructure.Data.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}

using System;
using System.Collections.Generic;

namespace BackPruebaTecnica.Infrastructure.Data.Models;

public partial class Usuarios
{
    public int Id { get; set; }

    public string Usuario { get; set; } = null!;

    public string Pass { get; set; } = null!;

    public DateTime? FechaCreacion { get; set; }

    public int RolId { get; set; }

    public virtual ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    public virtual Role Rol { get; set; } = null!;
}

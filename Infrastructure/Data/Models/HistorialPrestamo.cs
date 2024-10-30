using System;
using System.Collections.Generic;

namespace BackPruebaTecnica.Infrastructure.Data.Models;

public partial class HistorialPrestamo
{
    public int Id { get; set; }

    public int PrestamoId { get; set; }

    public string EstadoAnterior { get; set; } = null!;

    public string EstadoNuevo { get; set; } = null!;

    public DateTime? FechaCambio { get; set; }

    public virtual Prestamo Prestamo { get; set; } = null!;
}

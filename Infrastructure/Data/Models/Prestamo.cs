using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackPruebaTecnica.Infrastructure.Data.Models;

public partial class Prestamo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(1000, 100000, ErrorMessage = "El monto debe estar entre 1000 y 100000.")]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "El plazo es obligatorio.")]
    [Range(1, 60, ErrorMessage = "El plazo debe estar entre 1 y 60 meses.")]
    public int Plazo { get; set; } // En meses

    public string Estado { get; set; } // Solicitud, Aprobado, Rechazado
    public DateTime FechaSolicitud { get; set; }
    public int UsuarioId { get; set; } // Relación con el usuario que solicitó el préstamo
    public virtual ICollection<HistorialPrestamo> HistorialPrestamos { get; set; } = new List<HistorialPrestamo>();
    public virtual Usuarios Usuario { get; set; } // Navegación al usuario
}

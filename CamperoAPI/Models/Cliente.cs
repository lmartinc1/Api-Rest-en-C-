using System;
using System.Collections.Generic;

namespace CamperoAPI.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public string? Nombre { get; set; }

    public string? Apellidos { get; set; }

    public string? Telefono { get; set; }
}

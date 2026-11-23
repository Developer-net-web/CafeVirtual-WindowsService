using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService.Entidades
{
    public class Registro
    {
        public string Nombre { get; set; }

        public string Email { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }
    }
}

using CafeVirtualService.Entidades;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeVirtualService.Interfaces
{
    public interface IRegistroRepository
    {
        void InsertarLote(List<Registro> registros);
    }
}

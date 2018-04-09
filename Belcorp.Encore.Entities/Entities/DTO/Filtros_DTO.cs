using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class Filtros_DTO
    {
		public int CodConsultora { get; set; }
		public string NombreConsultora { get; set; }
		public int DireccionConsultora { get; set; }
		public int NumeroPagina { get; set; }    //Numero de Pagina a Partir
		public int NumeroRegistros { get; set; } //Cantidad de Registros a Mostrar

	}
}

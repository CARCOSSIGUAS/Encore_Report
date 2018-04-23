using System;
using System.Collections.Generic;
using System.Text;

namespace Belcorp.Encore.Entities.Entities.DTO
{
    public class Filtros_DTO
    {
		public int CodConsultoraLogged { get; set; }
		public int CodConsultoraSearched { get; set; }
		public string NombreConsultora { get; set; }
		public int DireccionConsultora { get; set; }
		public int Period { get; set; }
		public int NumeroPagina { get; set; }    //Numero de Pagina a Partir
		public int NumeroRegistros { get; set; } //Cantidad de Registros a Mostrar
		public string TituloPago { get; set; }
		public string TituloCarrera { get; set; }
		public string Estado { get; set; }	
		public DateTime DataCadastro { get; set; }
		public decimal VentaPersonal { get; set; }
		public int Nivel { get; set; }
		public int Generation { get; set; }
		public decimal VOT { get; set; }
		public decimal VOQ { get; set; }
		public int LeftBower { get; set; }
		public int RigthBower { get; set; }
	
	}
}

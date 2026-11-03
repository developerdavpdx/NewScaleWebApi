namespace WebApiPtmHana.Models.DatosMolinos
{
    public class MolinosLineas
    {
        public int NumeroMolino { get; set; }
        public string? Proceso { get; set; }
        public decimal HorasTurno { get; set; }
        public decimal Eficiencia { get; set; }
        public decimal ObjetivoProduccion { get; set; }
        public string? Nombre { get; set; }
        public int? MolinoEnParo { get; set; }
        public DateTime? FechaDeInicioParo { get; set; }
    }

    public class MolinosRegistrosPesos
    {
        public int Molino { get; set; }
        public int Turno { get; set; }
        public DateTime FechaPesaje { get; set; }
        public string? Operador { get; set; }
        public decimal ObjetivoDeProduccion { get; set; }
        public string? Codigo { get; set; }
        public decimal PesoTotal { get; set; }
        public decimal HorasProgramadas { get; set; }
        public decimal HorasParo { get; set; }
        public int Planta { get; set; }
        public string? Folio { get; set; }
        public string? ComentariosEdiciones { get; set; }
        public string? NombreMolino { get; set; }
        public string? MotivosParo { get; set; }
        public string? ComentariosParo { get; set; }
        public int RegistroActivo { get; set; }
        public string? Material { get; set; }
        public string? Descripcion { get; set; }
    }
    public class DatosDashboardMolinos
    {
        public string? Molino { get; set; }
        public decimal PesoTot { get; set; }
        public decimal KGHoraObjetivo { get; set; }
        public decimal KGxHora { get; set; }
        public decimal HorasDif { get; set; }
    }

    public class ArticulosCodigosMolinos
    {
        public string? Codigo { get; set; }
        public string? Proceso { get; set; }
        public int Planta { get; set; }
        public string? Descripcion { get; set; }
        public string? FechaCreacion { get; set; }
        public string? Tipo { get; set; }
        public string? Familia { get; set; }
    }


    public class DatosparaRegistroMolino
    {
        public decimal HorasTurno { get; set; }
        public decimal ObjetivoProduccion { get; set; }
        public string? Proceso { get; set; }
    }

    public class MolinerosMolinos
    {
        public string? CodigoUsuario { get; set; }
        public string? Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? ACTIVO { get; set; }
    }

    public class MolinosRegistrosIngresos
    {
        public int Id { get; set; }
        public string? Codigo { get; set; }
        public string? Tipo { get; set; }
        public string? Familia { get; set; }
        public decimal Peso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Origen { get; set; }
        public int? Planta { get; set; }
    }

    public class MotivosDeParo
    {
        public int Id { get; set; }
        public string? MotivoParo { get; set; }
    }
    public class Familias
    {
        public int Id { get; set; }
        public string? Familia { get; set; }
    }
    public class TiposDeMaterial
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? Proceso { get; set; }
    }
}
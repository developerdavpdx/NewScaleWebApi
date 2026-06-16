using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiPtmHana.Datos;

namespace WebApiPtmHana.DAL.DataContext.Repositories.LineasRepo
{
    public class LineasRepositorie : IGenericRepositorieLinie<Lineas>
    {
        private readonly ApplicationDbContext _context;
        public LineasRepositorie(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AsignLineToSKU(string sku, int linie, string userId, int turno, decimal kgxhr)
        {
            try
            {
                if (!sku.IsNullOrEmpty() && linie != 0)
                {
                    var idLinea = _context.Lineas.Where(x => x.NumLinea == linie).ToList();
                    var linea = await _context.Lineas.FindAsync(idLinea[0].Id_linea);
                    if (linea == null)
                        return false;

                    if (linea.Asignado == true)
                        return false;

                    linea.Asignado = true;
                    linea.KgXHr = kgxhr;
                    linea.ItemCodeLinea = sku;
                    await _context.SaveChangesAsync();

                    BitacoraAsignacionLinia bitacora = new BitacoraAsignacionLinia()
                    {
                        Linea = linie,
                        FechaAsignacion = DateTime.Now,
                        Trabajador = userId,
                        CodigoItem = sku
                    };
                    var bitresponse = await _context.AddAsync(bitacora);
                    await _context.SaveChangesAsync();
                    return true;
                } else
                {
                    return false;
                }  
            } catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateNewLine(int linea, string proceso, string planta)
        {
            try
            {
                Lineas lineas = new Lineas()
                {
                    Descripcion = $"Linea de producción número {linea}",
                    Asignado = false,
                    Proceso = proceso,
                    Planta = int.Parse(planta),
                    NumLinea = linea
                };

                await _context.AddAsync(lineas);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public IList<Lineas> GetLineas(string planta)
        {
            IList<Lineas> lineas = _context.Lineas.Where(x => x.Planta == int.Parse(planta)).ToList();
            return lineas;

        }
        public Lineas GetCode(string planta, string linea)
        {

            Lineas CodigoLinea = _context.Lineas.FirstOrDefault(x => x.Planta == int.Parse(planta) && x.NumLinea == int.Parse(linea));

            return CodigoLinea;

        }

        public async Task<bool> UnassignLinieAndTicket(int idLinea, string planta)
        {
            var listLinea = _context.Lineas.Where(x => x.NumLinea == idLinea && x.Planta == int.Parse(planta)).ToList();
            var linea = await _context.Lineas.FindAsync(listLinea[0].Id_linea);
            if (linea == null)
                return false;

            if (linea.Asignado == false)
                return false;

            linea.Asignado = false;
            linea.KgXHr = 0;
            linea.ItemCodeLinea = null;
            linea.FolioSAP = null;
            linea.OrdenFabricacion = null;
            //_context.Remove(ticketReg);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

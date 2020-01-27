using GoTravelTour.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTravelTour.Utiles
{
    public class Utiles
    {
        private readonly GoTravelDBContext _context;

        public Utiles(GoTravelDBContext context)
        {
            _context = context;
        }
        public string GetSKUCodigo ()
        {
            bool encontrado = false;
            string sku = "";
            int longitud = 10;

            while (!encontrado)
            {
                Guid g = Guid.NewGuid();
                string token = Convert.ToBase64String(g.ToByteArray());
                token = token.Replace("=", "").Replace("+", "");
                sku = "GTT-" + token.Substring(0, longitud);
                if (!_context.Productos.Any(x=>x.SKU == sku ))
                {
                    encontrado = true;
                }
            }
            
            
            return sku;
        }
    }
}

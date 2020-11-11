using GoTravelTour.Models;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        /// <summary>
        /// Para obtener el codigo unico para el producto
        /// </summary>
        /// <returns></returns>
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

        
        public string GetCodigoOrden()
        {
           
            string sku = "";
            string numeroOrden = "000000";
            int ultimaOrden = 0;
            if (!_context.Orden.Any())
            {
                ultimaOrden = 1;
            }else
            ultimaOrden = _context.Orden.Last().OrdenId + 1;
            numeroOrden = (numeroOrden + ultimaOrden.ToString()).PadRight(7);
            sku = "GTT-" + numeroOrden;



            return sku;

        }

        public static bool SaveImages(string ImageContent, string ImageName, string path)
        {
            bool result = false;

            try
            {
                string content = ImageContent.Substring(ImageContent.LastIndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(content);
                string file = Path.Combine(path, ImageName);
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return result;
        }

        public static bool RedimensionAndSaveImages(string ImageContent, string ImageName, string path, int height, int width)
        {
            bool result = false;
            try
            {
                string content = ImageContent.Substring(ImageContent.LastIndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(content);
                string file = Path.Combine(path, ImageName);
                if (bytes.Length > 0)
                {
                    using (MemoryStream stream = new MemoryStream(bytes))
                    {
                        Image img = Image.FromStream(stream);
                        int h = img.Height;
                        int w = img.Width;
                        int newW = (w * height) / h;
                        Bitmap newImg = new Bitmap(img, newW, height);

                        if (newW > width)
                        {
                            Rectangle rectOrig = new Rectangle((newW - width) / 2, 0, width, height);
                            Bitmap bmp = new Bitmap(rectOrig.Width, rectOrig.Height);
                            Graphics g = Graphics.FromImage(bmp);
                            g.DrawImage(newImg, 0, 0, rectOrig, GraphicsUnit.Pixel);
                            using (var aux = new FileStream(file, FileMode.Create))
                            {
                                bmp.Save(aux, ImageFormat.Jpeg);
                                aux.Flush();
                            }

                        }
                        else
                        {
                            Graphics g = Graphics.FromImage(newImg);
                            g.DrawImage(img, 0, 0, newImg.Width, newImg.Height);
                            using (var aux = new FileStream(file, FileMode.Create))
                            {
                                newImg.Save(aux, ImageFormat.Jpeg);
                                aux.Flush();
                            }
                        }
                        stream.Flush();
                    }
                }
                result = true;
            }
            catch (Exception e)
            {
                string message = e.Message;
            }

            return result;
        }

        public void CrearExcel()
        {
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var productos = _context.Productos.ToList();
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("Productos");
               /* worksheet.Cells["A1"].LoadFromCollection(productos, PrintHeaders: true);
               /* for (var col = 1; col < productos.Count + 1; col++)
                {
                    worksheet.Column(col).AutoFit();
                }*/

                // Agregar formato de tabla
                var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: productos.Count + 1, toColumn: 5), "Productos");
                tabla.ShowHeader = true;
                tabla.TableStyle = TableStyles.Light6;
                tabla.ShowTotal = true;

                // File(libro.GetAsByteArray(), excelContentType, "Productos.xlsx");
            }
        }

    }
}


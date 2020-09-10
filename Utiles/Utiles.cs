using GoTravelTour.Models;
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
            string numeroOrden = "0000000000";
            int ultimaOrden = 0;
            if (!_context.Orden.Any())
            {
                ultimaOrden = 1;
            }else
            ultimaOrden = _context.Orden.Last().OrdenId + 1;
            numeroOrden = (numeroOrden + ultimaOrden.ToString()).PadRight(10);
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
            // Unos valores de muestra para rellenar
            string[,] sArray;
            sArray = new string[5, 3];
            sArray[0, 0] = "Celda 0-0";
            sArray[0, 1] = "Celda 0-1";
            sArray[0, 2] = "Celda 0-2";
            sArray[1, 0] = "Celda 1-0";
            sArray[1, 1] = "Celda 1-1";
            sArray[1, 2] = "Celda 1-2";
            sArray[2, 0] = "Celda 2-0";
            sArray[2, 1] = "Celda 2-1";
            sArray[2, 2] = "Celda 2-2";
            sArray[3, 0] = "Celda 3-0";
            sArray[3, 1] = "Celda 3-1";
            sArray[3, 2] = "Celda 3-2";
            sArray[4, 0] = "Celda 4-0";
            sArray[4, 1] = "Celda 4-1";
            sArray[4, 2] = "Celda 4-2";
            // Crear los objetos necesarios para trabajar con Exce.
            Microsoft.Office.Interop.Excel.Application obj_Excel;
            Microsoft.Office.Interop.Excel.Workbook libroexcel;
            Microsoft.Office.Interop.Excel.Worksheet hojaexcel;
            object misValue = System.Reflection.Missing.Value;
            obj_Excel = new Microsoft.Office.Interop.Excel.Application();
            libroexcel = obj_Excel.Workbooks.Add(misValue);
            hojaexcel = (Microsoft.Office.Interop.Excel.Worksheet)libroexcel.Worksheets.get_Item(1);

            obj_Excel.Visible = true; // Permite ver o no la hoja en pantalla mientras el programa trabaja con ella.

            for (int f = 1; f < 6; f++)
            {
                for (int n = 1; n < 4; n++)
                {
                    hojaexcel.Cells[f, n] = sArray[f - 1, n - 1];
                }
            }

            libroexcel.Worksheets.get_Item(1);
            string path = "../sources/excel/";
           
            Directory.CreateDirectory(Path.Combine(path));
            DirectoryInfo directory = new DirectoryInfo(path);
            if (Directory.Exists(path))
            {

               
                foreach (FileInfo file in directory.GetFiles("*.*"))
                {

                    
                        file.Delete();

                }


                //Directory.Delete(path);
            }
            libroexcel.SaveAs(Path.Combine(directory.FullName, "Libroexcel"), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal); 
            libroexcel.Close();

        }

    }
}


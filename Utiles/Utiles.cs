﻿using GoTravelTour.Models;
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
    }
}

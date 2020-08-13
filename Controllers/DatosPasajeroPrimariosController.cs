using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosPasajeroPrimariosController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public DatosPasajeroPrimariosController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/DatosPasajeroPrimarios
        [HttpGet]
        public IEnumerable<DatosPasajeroPrimario> GetDatosPasajeroPrimario()
        {
            return _context.DatosPasajeroPrimario;
        }

        // GET: api/DatosPasajeroPrimarios/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDatosPasajeroPrimario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroPrimario = await _context.DatosPasajeroPrimario.FindAsync(id);

            if (datosPasajeroPrimario == null)
            {
                return NotFound();
            }

            return Ok(datosPasajeroPrimario);
        }

        // PUT: api/DatosPasajeroPrimarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatosPasajeroPrimario([FromRoute] int id, [FromBody] DatosPasajeroPrimario datosPasajeroPrimario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != datosPasajeroPrimario.DatosPasajeroPrimarioId)
            {
                return BadRequest();
            }

            _context.Entry(datosPasajeroPrimario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DatosPasajeroPrimarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DatosPasajeroPrimarios
        [HttpPost]
        public async Task<IActionResult> PostDatosPasajeroPrimario([FromBody] DatosPasajeroPrimario datosPasajeroPrimario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DatosPasajeroPrimario.Add(datosPasajeroPrimario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatosPasajeroPrimario", new { id = datosPasajeroPrimario.DatosPasajeroPrimarioId }, datosPasajeroPrimario);
        }

        // DELETE: api/DatosPasajeroPrimarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatosPasajeroPrimario([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroPrimario = await _context.DatosPasajeroPrimario.FindAsync(id);
            if (datosPasajeroPrimario == null)
            {
                return NotFound();
            }

            _context.DatosPasajeroPrimario.Remove(datosPasajeroPrimario);
            await _context.SaveChangesAsync();

            return Ok(datosPasajeroPrimario);
        }

        private bool DatosPasajeroPrimarioExists(int id)
        {
            return _context.DatosPasajeroPrimario.Any(e => e.DatosPasajeroPrimarioId == id);
        }


        private void TransformarYSalvarImagenes(DatosPasajeroPrimario datos, string id)
        {
            string path = "../firmas/" + id + "/";
            // try
            //{
            if (Directory.Exists(path))
            {

                DirectoryInfo directory = new DirectoryInfo(path);
                foreach (FileInfo filei in directory.GetFiles("*.*"))
                {

                    
                        filei.Delete();

                }


                //Directory.Delete(path);
            }
            //  }
            //  catch(Exception ex)
            //  {
            // throw ex;
            //  }

          
                SaveImages(datos, path);
                //string ext = img.TipoImagen.Split("/")[1];
                string file = Path.Combine(path, datos.NombreImagen /*+ "." + ext*/);
                datos.ImageContent = "http://gotravelandtours.com/sources/" + id + "/" + datos.NombreImagen /*+ "." + ext*/;
                //img.ImageContent = "http://localhost/sources/" + id + "/" + img.NombreImagen /*+ "." + ext*/;
           
        }


        public static bool SaveImages(DatosPasajeroPrimario foto, string path)
        {
            bool result = false;

            try
            {

                string content = foto.ImageContent.Substring(foto.ImageContent.LastIndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(content);
                //string ext = foto.TipoImagen.Split("/")[1];
                string file = Path.Combine(path, foto.NombreImagen);

                Directory.CreateDirectory(Path.Combine(path));

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
                throw e;
                //string message = e.Message;
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


        // GET: api/DatosPasajeroPrimarios/Orden5
        [HttpGet("Orden/{id}")]
        public async Task<IActionResult> GetDatosPasajeroPrimarioByOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var datosPasajeroPrimario = await _context.DatosPasajeroPrimario.Include(x => x.ListaPasajerosSecundarios).FirstAsync(x => x.Orden.OrdenId == id);

            if (datosPasajeroPrimario == null)
            {
                return NotFound();
            }

            return Ok(datosPasajeroPrimario);
        }



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using GoTravelTour.Utiles;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlmacenImagenesController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public AlmacenImagenesController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/AlmacenImagenes
        [HttpGet]
        public IEnumerable<AlmacenImagenes> GetAlmacenImagenes()
        {
            return _context.AlmacenImagenes;
        }

        // GET: api/AlmacenImagenes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlmacenImagenes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes = await _context.AlmacenImagenes.FindAsync(id);

            if (almacenImagenes == null)
            {
                return NotFound();
            }

            return Ok(almacenImagenes);
        }

        // PUT: api/AlmacenImagenes/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutAlmacenImagenes([FromBody] List<AlmacenImagenes> almacenImagenes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*if (id != almacenImagenes.AlmacenImagenesId)
            {
                return BadRequest();
            }*/
            if(almacenImagenes.Count > 0 )
            {
                foreach (var img in almacenImagenes)
                {
                    _context.Entry(img).State = EntityState.Modified;
                }
            }
            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AlmacenImagenes
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostAlmacenImagenes(int  id, [FromBody] List<AlmacenImagenes> almacenImagenes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<string> noBorrar = new List<string>();
             
                int i = 0;
              

                while ( i < almacenImagenes.Count())
                {
                    
                    var img = almacenImagenes[i];
                    if (img.AlmacenImagenesId != 0)
                    {
                        if (img.ImageContent != null && !img.ImageContent.StartsWith("http"))
                            _context.AlmacenImagenes.Remove(_context.AlmacenImagenes.Find(img.AlmacenImagenesId));
                        else
                        {
                            if (img.ImageContent != null && img.ImageContent.StartsWith("http"))
                                noBorrar.Add(img.NombreImagen);
                            almacenImagenes.RemoveAt(i);
                            i--;
                        }
                    }
                   
                    i++;
                }

                

            
            try
            {
               // Directory.CreateDirectory("C:\\inetpub\\wwwroot\\publicEliecer\\sources");
                TransformarYSalvarImagenes(almacenImagenes, id.ToString(), noBorrar);
            }
            catch (Exception ex)
            {

                return CreatedAtAction("GetAlmacenImagenes", new { id = 0, msg = ex.Message + ex.StackTrace}, new { id = 0, msg = ex.Message +ex.StackTrace });
                throw;
            }

            


            _context.AlmacenImagenes.AddRange(almacenImagenes);
            
            
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlmacenImagenes", new { id = 0, msg = "Almacen Creado" }, almacenImagenes);
        }

        // DELETE: api/AlmacenImagenes/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAlmacenImagenes([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes = await _context.AlmacenImagenes.FindAsync(id);
            if (almacenImagenes == null)
            {
                return NotFound();
            }

            _context.AlmacenImagenes.Remove(almacenImagenes);
            await _context.SaveChangesAsync();

            return Ok(almacenImagenes);
        }

        private bool AlmacenImagenesExists(int id)
        {
            return _context.AlmacenImagenes.Any(e => e.AlmacenImagenesId == id);
        }

        // GET: api/AlmacenImagenes/Productos/5
        [Route ("Productos/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAlmacenImagenesByProducto([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes =  _context.AlmacenImagenes.Where( a => a.ProductoId == id).ToList();

            if (almacenImagenes == null)
            {
                return NotFound();
            }

            return Ok(almacenImagenes);
        }


       
        private void TransformarYSalvarImagenes(List<AlmacenImagenes> almacenImagenes, string id, List<string> imgenesANoBorrar)
        {
            string path = "../sources/" + id + "/";
           // try
            //{
                if (Directory.Exists(path))
                {

                    DirectoryInfo directory = new DirectoryInfo(path);
                    foreach (FileInfo file in directory.GetFiles("*.*"))
                    {

                     if(!imgenesANoBorrar.Any(x=>x==file.Name))
                        file.Delete();

                    }


                   //Directory.Delete(path);
                }
          //  }
          //  catch(Exception ex)
          //  {
               // throw ex;
          //  }
           
            foreach (var img in almacenImagenes)
            {
                
                SaveImages(img, path);
                //string ext = img.TipoImagen.Split("/")[1];
                string file = Path.Combine(path, img.NombreImagen /*+ "." + ext*/);
                img.ImageContent = "http://gotravelandtours.com/sources/"+id+"/"+ img.NombreImagen /*+ "." + ext*/;
                //img.ImageContent = "http://localhost/sources/" + id + "/" + img.NombreImagen /*+ "." + ext*/;
            }
        }


        public static bool SaveImages(AlmacenImagenes foto, string path)
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



        // POST: api/AlmacenImagenes
        [HttpPost]
        [Route("setmain")]
       
        public async Task<IActionResult> PostSetMain(int idProducto=0, int idImagen=0)
        {
            if (idProducto<=0 || idImagen<= 0)
            {
                return BadRequest(ModelState);
            }

            List<AlmacenImagenes> lista = _context.AlmacenImagenes.Where(x => x.ProductoId == idProducto).ToList();


            foreach (var item in lista)
            {
                if(item.AlmacenImagenesId == idImagen)
                {
                    item.Localizacion = ValoresAuxiliares.IMAGEN_LOC_MAIN;
                }
                else
                {
                    item.Localizacion = ValoresAuxiliares.IMAGEN_LOC_GALLERY;
                }

                _context.Entry(item).State = EntityState.Modified;

            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlmacenImagenes", new { id = 0, msg = "Imagen principal colocada" });
        }


        // POST: api/AlmacenImagenes
        [HttpPost]
        [Route("getmain")]

        public async Task<IActionResult> PostGetMain(int idProducto = 0)
        {
            if (idProducto <= 0 )
            {
                return BadRequest(ModelState);
            }

            List<AlmacenImagenes> lista = _context.AlmacenImagenes.Where(x => x.ProductoId == idProducto).ToList();

            AlmacenImagenes res = new AlmacenImagenes();
            foreach (var item in lista)
            {
                if (item.Localizacion == ValoresAuxiliares.IMAGEN_LOC_MAIN)
                {
                    res = item;
                }               

            }
                       
            return Ok(res);
        }

    }
}
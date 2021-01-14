using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoTravelTour.Models;
using System.IO;

namespace GoTravelTour.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherOrdensController : ControllerBase
    {
        private readonly GoTravelDBContext _context;

        public VoucherOrdensController(GoTravelDBContext context)
        {
            _context = context;
        }

        // GET: api/VoucherOrdens
        [HttpGet]
        public IEnumerable<VoucherOrden> GetVoucherOrden()
        {
            return _context.VoucherOrden;
        }

        // GET: api/VoucherOrdens/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucherOrden = await _context.VoucherOrden.FindAsync(id);

            if (voucherOrden == null)
            {
                return NotFound();
            }

            return Ok(voucherOrden);
        }

        // PUT: api/VoucherOrdens/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVoucherOrden([FromRoute] int id, [FromBody] VoucherOrden voucherOrden)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != voucherOrden.VoucherOrdenId)
            {
                return BadRequest();
            }

            TransformarYSalvarPDF(voucherOrden);

            _context.Entry(voucherOrden).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoucherOrdenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(voucherOrden);
        }

        // POST: api/VoucherOrdens
        [HttpPost]
        public async Task<IActionResult> PostVoucherOrden([FromBody] VoucherOrden voucherOrden)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            TransformarYSalvarPDF(voucherOrden);
            _context.VoucherOrden.Add(voucherOrden);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVoucherOrden", new { id = voucherOrden.VoucherOrdenId }, voucherOrden);
        }

        // DELETE: api/VoucherOrdens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucherOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var voucherOrden = await _context.VoucherOrden.FindAsync(id);
            if (voucherOrden == null)
            {
                return NotFound();
            }

            _context.VoucherOrden.Remove(voucherOrden);

            await _context.SaveChangesAsync();

            string path = "../sources/" + voucherOrden.OrdenId.ToString() + "/";
            try
            {
                if (Directory.Exists(path))
                {

                    DirectoryInfo directory = new DirectoryInfo(path);
                    foreach (FileInfo file in directory.GetFiles("*.*"))
                    {

                       
                            file.Delete();

                    }



                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(voucherOrden);
        }

        private bool VoucherOrdenExists(int id)
        {
            return _context.VoucherOrden.Any(e => e.VoucherOrdenId == id);
        }




        private void TransformarYSalvarPDF(VoucherOrden voucher)
        {
            string path = "../sources/Vouchers/" + voucher.OrdenId.ToString() + "/";
            // try
            //{
            //if (Directory.Exists(path))
            //{

            //    DirectoryInfo directory = new DirectoryInfo(path);
            //    foreach (FileInfo file in directory.GetFiles("*.*"))
            //    {

            //        if (!imgenesANoBorrar.Any(x => x == file.Name))
            //            file.Delete();

            //    }


                
            //}
            //  }
            //  catch(Exception ex)
            //  {
            // throw ex;
            //  }

           
                SavePDF(voucher, path);
                //string ext = img.TipoImagen.Split("/")[1];
                string fileUrl = Path.Combine(path, voucher.Nombre /*+ "." + ext*/);
            voucher.UrlVoucher = "https://admin.gotravelandtours.com/sources/Vouchers/" + voucher.OrdenId + "/" + voucher.Nombre /*+ "." + ext*/;
           // voucher.UrlVoucher = "http://localhost/sources/Vouchers/" + voucher.OrdenId + "/" + voucher.Nombre /*+ "." + ext*/;
            

        }


        public static bool SavePDF(VoucherOrden voucher, string path)
        {
            bool result = false;

            try
            {

                string content = voucher.UrlVoucher.Substring(voucher.UrlVoucher.LastIndexOf(',') + 1);
                byte[] bytes = Convert.FromBase64String(content);
                //string ext = foto.TipoImagen.Split("/")[1];
                string file = Path.Combine(path, voucher.Nombre);

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


        // GET: api/VoucherOrdens/Orden/5
        [Route("Orden/{id}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherOrdensByOrden([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var almacenImagenes = _context.VoucherOrden.Where(a => a.OrdenId == id).ToList();

            if (almacenImagenes == null)
            {
                return NotFound();
            }

            return Ok(almacenImagenes);
        }

    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Vigen_Repository.Models;

namespace Vigen_Repository.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly vigendbContext _context;
        private readonly IHubContext<BroadCastHub, IHubClient> _hubContext;
        public NotifyController(vigendbContext context, IHubContext<BroadCastHub, IHubClient> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        [HttpGet]
        public async Task<ActionResult<List<Notify>>> getNotifies()
        {
            List<Notify> notifies = await _context.Notifies.ToListAsync();
            return Ok(notifies);
        }

        

        [HttpGet("{id}")]
        public async Task<ActionResult<Notify>> getNotify(string id)
        {
            int idAux;
            try { idAux = int.Parse(id); }
            catch { idAux = -1; }
            Notify? notify = await _context.Notifies.FindAsync(idAux);
            if (notify == null) return NotFound();
            return Ok(notify);
        }

        [HttpPost]
        public async Task<ActionResult<Notify>> postNotify(Notify notify)
        {
            try
            {
                await _context.Notifies.AddAsync(notify);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.recibeNotify(notify);
                return Ok(notify);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Notify>> UpdateNotify(string id, Notify notify)
        {
            int idInt;
            try { idInt = int.Parse(id); }
            catch { idInt = -1; }

            if (idInt != notify.Id) return BadRequest("El id no concide");
            try
            {
                _context.Entry(notify).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(notify);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Notify>> DeleteNotify(string id)
        {
            try
            {
                Notify? notify = await _context.Notifies.FindAsync(int.Parse(id));
                if (notify == null) return NotFound();
                _context.Notifies.Remove(notify);
                await _context.SaveChangesAsync();
                return Ok(notify);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }

        [HttpGet("reporte/pdf")]
        public async Task<IActionResult> DescargarReporteNotificaciones()
        {
            var notifies = await _context.Notifies.ToListAsync();

            if (notifies.Count == 0)
            {
                return NoContent();
            }

            using (var stream = new MemoryStream())
            {
                var document = new PdfSharpCore.Pdf.PdfDocument();
                var page = document.AddPage();
                var gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                var font = new PdfSharpCore.Drawing.XFont("LiberationSans#", 10, PdfSharpCore.Drawing.XFontStyle.Regular);
                var boldFont = new PdfSharpCore.Drawing.XFont("LiberationSans#", 12, PdfSharpCore.Drawing.XFontStyle.Bold);

                double y = 40;
                double marginLeft = 40;

                // Título
                gfx.DrawString("Reporte de Notificaciones", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft, y - 20));

                // Encabezados de tabla
                gfx.DrawString("ID", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft, y));
                gfx.DrawString("Usuario", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 50, y));
                gfx.DrawString("Título", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 200, y));
                gfx.DrawString("Estado", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 400, y));
                gfx.DrawString("Fecha", boldFont, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 480, y));

                y += 25;

                foreach (var notify in notifies)
                {
                    if (y > page.Height - 40)
                    {
                        page = document.AddPage();
                        gfx = PdfSharpCore.Drawing.XGraphics.FromPdfPage(page);
                        y = 40;
                    }

                    string estadoTexto = notify.StateId switch
                    {
                        0 => "Activa",
                        1 => "En Progreso",
                        _ => "Finalizada"
                    };

                    gfx.DrawString(notify.Id.ToString(), font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft, y));
                    gfx.DrawString(notify.UserId, font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 50, y));
                    gfx.DrawString(notify.Title, font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 200, y));
                    gfx.DrawString(estadoTexto, font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 400, y));
                    gfx.DrawString(notify.Date.ToString("yyyy-MM-dd"), font, PdfSharpCore.Drawing.XBrushes.Black, new PdfSharpCore.Drawing.XPoint(marginLeft + 480, y));

                    y += 20;
                }

                document.Save(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/pdf", "reporte_alertas.pdf");
            }
        }
    }
}

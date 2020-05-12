using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VirusTracker.Business;
using VirusTracker.Models;

namespace VirusTracker.Controllers
{
    public class BusinessesController : Controller
    {
        private readonly TracingContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BusinessesController(
            TracingContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> QRCode(Guid? id)
        {
            //https://www.c-sharpcorner.com/article/generate-qr-code-in-net-core-using-bitmap/
            QRCodeCreation QR = new QRCodeCreation();
            //todo pass in the url of the business  

            Models.Business business = _context.Business.FirstOrDefault(m => m.Id == id);

            //grab the business, replace any spaces
            string place = business.BusinessName.Replace(" ", "_");

            //todo replace localhost with actual url
            //https://stackoverflow.com/questions/38437005/how-to-get-current-url-in-view-in-asp-net-core-1-0
            var location = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}");
            var url = location.AbsoluteUri;

            string path = QueryStringExtensions.AddToQueryString(url, "Place", place);

            Bitmap qrCodeImage = QR.QRGenerate(path);

            return View(BitmapToBytesCode(qrCodeImage));
        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }


        // GET: Businesses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Business.ToListAsync());
        }

        // GET: Businesses/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Business
                .FirstOrDefaultAsync(m => m.Id == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: Businesses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Businesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BusinessName,AdminName,BusinessAddress,Phone")] Models.Business business)
        {
            //todo add in the guid of the current logged in person to the class

            if (ModelState.IsValid)
            {
                //    https://stackoverflow.com/questions/30701006/how-to-get-the-current-logged-in-user-id-in-asp-net-core
                ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
                //   string userEmail = applicationUser?.Email; // will give the user's Email
                Guid userId = Guid.Parse(applicationUser?.Id); // will give the user's Email


                //var userId = _userManager.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
                //  var userName = _userManager.FindFirstValue(ClaimTypes.Name); // will give the user's userName


                business.Id = Guid.NewGuid();
                business.ASPNetUsersIdfk = userId;
                _context.Add(business);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(business);
        }

        // GET: Businesses/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Business.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }
            return View(business);
        }

        // POST: Businesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,BusinessName,AdminName,BusinessAddress,Phone")] Models.Business business)
        {
            if (id != business.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessExists(business.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(business);
        }

        // GET: Businesses/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.Business
                .FirstOrDefaultAsync(m => m.Id == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: Businesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var business = await _context.Business.FindAsync(id);
            _context.Business.Remove(business);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessExists(Guid id)
        {
            return _context.Business.Any(e => e.Id == id);
        }
    }
}

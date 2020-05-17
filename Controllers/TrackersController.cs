using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirusTracker.Models;

namespace VirusTracker.Controllers
{

    public class TrackersController : Controller
    {


        public string Place { get; set; }

        private readonly ILogger<TrackersController> _logger;
        private readonly IWebHostEnvironment _env;
        private IHttpContextAccessor _httpacc;
        private readonly TracingContext _context;
        // private readonly UserManager<ApplicationUser> _userManager;


        public TrackersController(TracingContext context, ILogger<TrackersController> logger,
            IWebHostEnvironment env,
              IHttpContextAccessor httpacc
       //      UserManager<ApplicationUser> userManager
       )
        {
            _context = context;
            _logger = logger;
            _env = env;
            _httpacc = httpacc;
            //  _userManager = userManager;
        }

        // GET: Trackers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tracker.ToListAsync());
        }

        // GET: Trackers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-3.1#how-model-binding-works
        // GET: Trackers/Create

        [HttpGet()]
        public IActionResult Create()
        {   //https://localhost:44394/Trackers/Create?Place=Vision_College
            //only update place with teh query if there is something in the string
            string nullCheck = null;
            nullCheck = HttpContext.Request.Query["Place"].ToString();

            if (!string.IsNullOrEmpty(nullCheck))
            {
                Place = HttpContext.Request.Query["Place"].ToString();

                ViewData["Place"] = Place;

            }



            return View();
        }

        // POST: Trackers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598. 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Phone, Place")] Tracker tracker)
        {

            //  tracker.BusinessName = Place;
            // string BusinessName = HttpContext.Request.Query["Place"].ToString();
            // string BusinessNameQuery = BusinessName;
            if (HttpContext.Request.Path.HasValue)
            {

                //https://localhost:44394/Trackers/Create?Place=Vision_College

                //todo process the query part to get the guid of the business class.
                //seems to get just the query part which is all I need anyway
                //   string stuff = UriHelper.GetEncodedPathAndQuery(Request);  //returns trackers/create
                //  string url = HttpContext.Request.Path;  //returns trackers/create
                //get out the business name at the end of the url


                // BusinessName = url.Contains('?') ? url.Substring(url.IndexOf('=')) : null;
            }

            //get the absolute URL 
            //  BusinessName = BusinessName;    //_httpacc.HttpContext.Request.Host.Value;
            Models.Business data = _context.Business.FirstOrDefault(m => m.BusinessName == tracker.Place);
            Guid aSPNetUsersIdfk = data.ASPNetUsersIdfk;
            tracker.ASPNetUsersIdfk = aSPNetUsersIdfk.ToString();
            tracker.Id = Guid.NewGuid();
            tracker.DateIn = DateTime.Now;
            tracker.DateOut = DateTime.Now;

            //  tracker.BusinessName = BusinessName;


            //     if (ModelState.IsValid)
            //    {

            //    https://stackoverflow.com/questions/30701006/how-to-get-the-current-logged-in-user-id-in-asp-net-core
            /*   ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
               string userEmail = applicationUser?.Email; // will give the user's Email
               Guid userId = Guid.Parse(applicationUser?.Id); // will give the user's Email
*/
            //var userId = _userManager.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            //  var userName = _userManager.FindFirstValue(ClaimTypes.Name); // will give the user's userName



            _context.Add(tracker);
            await _context.SaveChangesAsync();
            //      return RedirectToAction(nameof(Index));
            //  }

            //todo I need 2 cookies, one holds the ID of the entry ad can be removed after some hours. The other holds the Name and phone and never expires.

            //this session cookie
            //  CookieOptions cookieID = new CookieOptions();
            // cookieID.Expires = DateTime.Now.AddHours(10); //kill it after 10 hours

            //  _context.Response.Cookies.Append("key", "value", cookieID);//adds a cookie to the HTTP response and sets the name of the new cookie to "key" and its value to "value".


            //set user info into session
            LogoutTrackerModel logoutDetails = new LogoutTrackerModel();

            //    var data = _context.Tracker.FirstOrDefaultAsync(m => m.Name == tracker.Name);

            logoutDetails.ID = tracker.Id.ToString();
            logoutDetails.Name = tracker.Name;
            logoutDetails.Phone = tracker.Phone;
            HttpContext.Session.SetString("LoginDetails", JsonConvert.SerializeObject(logoutDetails));



            return View("LogoutTracker");
        }


        // GET: Trackers/LogoutTracker
        public IActionResult LogoutTracker()
        {
            //get user info from session

            var LoginDetails = JsonConvert.DeserializeObject<LogoutTrackerModel>(HttpContext.Session.GetString("LoginDetails"));



            return View();
        }





        // GET: Trackers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker.FindAsync(id);
            if (tracker == null)
            {
                return NotFound();
            }
            return View(tracker);
        }

        // POST: Trackers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ASPNetUsersIdfk, BusinessName,Name,Phone,DateIn,DateOut")] Tracker tracker)
        {
            if (id != tracker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tracker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrackerExists(tracker.Id))
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
            return View(tracker);
        }

        // GET: Trackers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tracker = await _context.Tracker
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tracker == null)
            {
                return NotFound();
            }

            return View(tracker);
        }

        // POST: Trackers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tracker = await _context.Tracker.FindAsync(id);
            _context.Tracker.Remove(tracker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrackerExists(Guid id)
        {
            return _context.Tracker.Any(e => e.Id == id);
        }
    }
}

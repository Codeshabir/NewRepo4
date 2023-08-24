using Kind_Heart_Charity.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;
using System.Diagnostics;
using System.IO.Pipelines;
using Kind_Heart_Charity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Kind_Heart_Charity.Models.Mailing;

namespace Kind_Heart_Charity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Kind_Heart_CharityContext _context;

        public HomeController(ILogger<HomeController> logger, Kind_Heart_CharityContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<DynamicPagesDBDTO> data = new List<DynamicPagesDBDTO>();
            _context.DynamicPages.Where(x=>x.IsDeleted==false).ToList().ForEach(x => data.Add(new DynamicPagesDBDTO() { Id = x.Id, PageName = x.PageName, TotalRecords = 0, SectionName = x.SectionName }));
            ViewBag.AllPages = data;
            var SectionName = data.GroupBy(x => x.SectionName).Select(x=>x.Key);
            ViewBag.SectionName = SectionName;
            var categories = _context.Categories.ToList();

            ViewBag.AllCategories = categories;
            return View();
        }

        string DomainName = "/Home/Index", sessionId;

        //[HttpPost]
        //public IActionResult CreateCheckoutSession(Donations donations)
        //{
        //    StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";


        //    var options = new PriceCreateOptions
        //    {
        //        UnitAmount = Convert.ToInt64(donations.Amount) * 100, // in cents
        //        Currency = "usd",
        //        ProductData = new PriceProductDataOptions
        //        {
        //            Name = donations.Name
        //        }
        //    };

        //    var priceService = new PriceService();
        //    Price price = priceService.Create(options);

        //    // Create a new checkout session with the price as the line item
        //    var sessionOptions = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string>
        //        {
        //            "card",
        //        },
        //        LineItems = new List<SessionLineItemOptions>
        //        {
        //            new SessionLineItemOptions
        //            {
        //                Price = price.Id,
        //                Quantity = 1,
        //            },
        //        },
        //        Mode = "payment",
        //        SuccessUrl = DomainName + "PaymentConfirmation?Name=" + donations.Name + "&Email=" + donations.Email + "&Amount=" + donations.Amount + "",
        //        CancelUrl = DomainName + "Index",
        //    };


        //    var service = new SessionService();
        //    var session = service.Create(sessionOptions);
        //    sessionId = session.Id;
        //    return Redirect(session.Url);
        //}

        //public async Task<IActionResult> PaymentConfirmation(string Name, string Email, string Amount)
        //{
        //    Donations donations = new Donations(Name, Amount, Email, 0, 0);

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(donations);
        //        await _context.SaveChangesAsync();
        //    }

        //    return Redirect("/Home/Index");
        //}




        public IActionResult CreateCheckoutSession(decimal amount, string username)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";


                var options = new PriceCreateOptions
                {
                    UnitAmount = Convert.ToInt64(amount * 100), // in cents
                    Currency = "usd",
                    ProductData = new PriceProductDataOptions
                    {
                        Name = "Monthly Plan"
                    }
                };

                var priceService = new PriceService();
                Price price = priceService.Create(options);

                // Create a new checkout session with the price as the line item
                var sessionOptions = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = price.Id,
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = Url.Action("PaymentConfirmation", "Home", new { amount = amount, username = username }, Request.Scheme),
                    CancelUrl = Url.Action("Index", "Home", null, Request.Scheme),
                };

                var service = new SessionService();
                var session = service.Create(sessionOptions);
                var sessionId = session.Id;
                return Redirect(session.Url);
            }
            catch (Exception ex)
            {
                // Log or output the exception
                Debug.WriteLine($"Exception: {ex.Message}");
                return View("Error"); // Replace "Error" with the name of your error view

            }


        }


        public async Task<IActionResult> PaymentConfirmation(decimal amount, string username)
        {
           

            return Redirect("/Home/Index");
        }
        [HttpPost]
        public IActionResult Subscribe(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var mailingEntry = new MailingList
                {
                    Email = email,
                    CreatedDate = DateTime.Now
                };

                _context.MailingLists.Add(mailingEntry);
                _context.SaveChanges();
            }

            return PartialView("_SuccessModal");
        }




    }
}
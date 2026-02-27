using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CAPTCHA_CHECK.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string RecaptchaSiteKey { get; set; }

        public void OnGet()
        {
            RecaptchaSiteKey = _configuration["RecaptchaToken:SiteKey"];
        }

        public async Task<IActionResult> OnPostAsync()
        {
            RecaptchaSiteKey = _configuration["RecaptchaToken:SiteKey"];

            var recaptchaResponse = Request.Form["g-recaptcha-response"];
            var secretKey = _configuration["RecaptchaToken:SecretKey"];

            using (var client = new HttpClient())
            {
                var result = await client
                    .GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptchaResponse}");

                var json = JObject.Parse(result);
                var success = json.Value<bool>("success");

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "CAPTCHA validation failed.");
                    return Page();
                }
            }

            // CAPTCHA SUCCESS → redirect to next page
            return RedirectToPage("Success");
        }
    }
}
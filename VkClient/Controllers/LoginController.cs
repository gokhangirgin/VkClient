using DotNetOpenAuth.AspNet;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace VkClient.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {
            DotNetOpenAuth.AspNet.Clients.VkClient client = 
                new DotNetOpenAuth.AspNet.Clients.VkClient("client_id", "client_secret", "video", "offline", "friends", "wall");
            AuthenticationResult result = client.VerifyAuthentication(this.HttpContext, new Uri(Request.Url.AbsoluteUri));
            if (!result.IsSuccessful)
                client.RequestAuthentication(this.HttpContext, new Uri(Request.Url.AbsoluteUri));
            else
            {
                ViewBag.Message = JsonConvert.SerializeObject(result.ExtraData);
            }
            return View();
        }
	}
}
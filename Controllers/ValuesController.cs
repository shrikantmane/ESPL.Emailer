using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Emailer.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var message = new MimeMessage ();
			message.From.Add (new MailboxAddress ("Bharati Shinde", "bhartishinde01@gmail.com"));
			message.To.Add (new MailboxAddress ("Mr. Shrikant Mane", "mane.shrikant55@gmail.com"));
			message.Subject = "How you doin'?";

			message.Body = new TextPart ("plain") {
				Text = @"Hey Chandler,

I just wanted to let you know that Monica and I were going to go play some paintball, you in?

-- Joey"
			};

			using (var client = new SmtpClient ()) {
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s,c,h,e) => true;

				client.Connect ("smtp.gmail.com", 587, false);

				// Note: since we don't have an OAuth2 token, disable
				// the XOAUTH2 authentication mechanism.
				client.AuthenticationMechanisms.Remove ("XOAUTH2");

				// Note: only needed if the SMTP server requires authentication
				client.Authenticate ("bhartishinde01@gmail.com", "iamstrong");

				client.Send (message);
				client.Disconnect (true);
			}
            return new string[] { "Email Sent!!", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

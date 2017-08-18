using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit; 
using Emailer.Models;
using Emailer.Helper;

namespace Emailer.Controllers
{
    
    public class MailController : Controller
    {
        [HttpPost]
        [Route("api/[controller]/send")]
        public string send([FromBody]MailWrapper mailWrapper)
        {
            MailHelper mailHelper = new MailHelper();
            bool emailSent = false;

            string[] adrs = mailWrapper.mailOptions.to.Split(',');
            if(adrs.Count() > 1)
                emailSent =mailHelper.SendMultipleEmail(mailWrapper.smtpOptions,mailWrapper.mailOptions);
            else
                emailSent =mailHelper.SendEmail(mailWrapper.smtpOptions,mailWrapper.mailOptions);

          if(emailSent)
                 return "Email Sent!!";
          else 
                 return "Can not send Email";
        }

    }
}


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
            string[] adrs = mailWrapper.mailOptions.to.Split(',');
            if(adrs.Count() > 1)
                mailHelper.SendMultipleEmailAsync(mailWrapper.smtpOptions,mailWrapper.mailOptions);
            else
                mailHelper.SendEmailAsync(mailWrapper.smtpOptions,mailWrapper.mailOptions);

          return "Email Sent!!";
        }
       
    }
}


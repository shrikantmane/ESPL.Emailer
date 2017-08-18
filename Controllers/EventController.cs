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

    public class EventController : Controller
    {
        [HttpPost]
        [Route("api/[controller]/send")]
        public string send([FromBody]EventWrapper eventWrapper)
        {
            EventHelper eventHelper = new EventHelper();
            string[] adrs = eventWrapper.eventOptions.to.Split(',');
            if (adrs.Count() > 1)
                eventHelper.SendMultipleEmailAsync(eventWrapper.smtpOptions, eventWrapper.eventOptions);
            else
                eventHelper.SendEmailAsync(eventWrapper.smtpOptions, eventWrapper.eventOptions);

            return "Email Sent!!";
        }

    }
}
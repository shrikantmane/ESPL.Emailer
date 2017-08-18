using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emailer.Models;

namespace Emailer.Models
{
public class EventWrapper
{
    public SMTPOptions smtpOptions { get; set; }
    public EventOptions eventOptions { get; set; }
}

}
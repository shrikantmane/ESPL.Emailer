using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Emailer.Models;

namespace Emailer.Models
{
public class MailWrapper
{
    public SMTPOptions smtpOptions { get; set; }
    public MailOptions mailOptions { get; set; }
}

}
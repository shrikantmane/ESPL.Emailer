using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emailer.Models
{
public class MailOptions:BasicMailOptions
{
    public string plainTextMessage{get;set;}
    public string htmlMessage{get;set;}
    
}

}
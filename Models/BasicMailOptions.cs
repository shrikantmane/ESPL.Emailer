using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Emailer.Models
{
public class BasicMailOptions
{
    public string to { get; set; }
    public string from{get;set;}
    public string subject {get;set;}
    public string replyTo{get;set;}
}
}
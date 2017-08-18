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
        // [HttpPost]
        // [Route("api/[controller]/send")]
        // public string send([FromBody]MailWrapper mailWrapper)
        // {
        //     MailHelper mailHelper = new MailHelper();
        //     bool emailSent = false;

        //     string[] adrs = mailWrapper.mailOptions.to.Split(',');
        //     if(adrs.Count() > 1)
        //         emailSent =mailHelper.SendMultipleEmail(mailWrapper.smtpOptions,mailWrapper.mailOptions);
        //     else
        //         emailSent =mailHelper.SendEmail(mailWrapper.smtpOptions,mailWrapper.mailOptions);

        //   if(emailSent)
        //          return "Email Sent!!";
        //   else 
        //          return "Can not send Email";
        // }

        [HttpPost]
        [Route("api/[controller]/send")]
        public IActionResult send([FromBody]MailWrapper mailWrapper)
        {
        try
        {
            if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.server))
            {
                 return StatusCode(404, "Please specify the email client");
            }

            if (string.IsNullOrWhiteSpace(Convert.ToString(mailWrapper.smtpOptions.port)))
            {
                return StatusCode(404, "Please specify the SMTP port");
            }
            // else
            // {
            //     if(Convert.ToInt32(mailWrapper.smtpOptions.port) !=25 && Convert.ToInt32(mailWrapper.smtpOptions.port) !=587 )
            //         return StatusCode(500, "Invalid SMTP port");
            // }

            if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.user))
            {
                return StatusCode(404,"Please specify the 'user'");
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(mailWrapper.smtpOptions.user);
                }
                catch (System.Exception ex)
                {
                   return StatusCode(400,"Invalid SMTP 'user'");
                }
            }
           
             if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.password))
            {
                return StatusCode(404,"Please specify the SMTP 'password'");
            }

            if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.to))
            {
                 return StatusCode(404, "'to' address can not be empty");
            }

            if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.from))
            {
                return StatusCode(404, "'from' can not be empty");
            }
             else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.from);
                }
                catch (System.Exception ex)
                {
                   return StatusCode(400,"Invalid 'from' address");
                }
            }

            if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.subject))
            {
                return StatusCode(404,"'subject' can not be empty");
            }

             if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.replyTo))
            {
                return StatusCode(404,"'replyTo' can not be empty");
            }
            else
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.replyTo);
                }
                catch (System.Exception ex)
                {
                   return StatusCode(400,"Invalid 'replyTo' address");
                }
            }

            MailHelper mailHelper = new MailHelper();
            bool emailSent = false;

            string[] adrs = mailWrapper.mailOptions.to.Split(',');
            if(adrs.Count() > 1)
            {
            //      int index = adrs.Length - 1;
            // System.Net.Mail.MailAddress parsedAddress = MailAddressParser.ParseAddress(adrs, false, ref index);
            // //Debug.Assert(index == -1, "The index indicates that part of the address was not parsed: " + index);
            // if(index == -1)
            //     return StatusCode(500,parsedAddress);
            // else
            }
            else if(adrs.Count() == 1)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.to);
                }
                catch (System.Exception ex)
                {
                   return StatusCode(400,"Invalid 'to' address");
                }
            }
                var m =mailHelper.generateMessage(mailWrapper.mailOptions);

if(m != null)
{
                using (var client = new SmtpClient())
            {
                try
                {
                    try
                {
                client.Connect(
                    mailWrapper.smtpOptions.server,
                    mailWrapper.smtpOptions.port,
                    mailWrapper.smtpOptions.useSSL);
                           }
            catch (System.Exception ex)
                {
                   return StatusCode(400,"Invalid SMTP Details");                
                }    
               // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                try
                {
               client.Authenticate(mailWrapper.smtpOptions.user, mailWrapper.smtpOptions.password);
                }
            catch (System.Exception ex)
                {
                   return StatusCode(500,"Authentication Failed");                
                }
                
                client.Send(m);
                client.Disconnect(true);
                return Ok("Email Sent!!");
                }
            catch (System.Exception ex)
                {
                    return StatusCode(500,"Can not send email");  
                }
           

        }
        }
        }
                 catch(System.Exception ex)
                 {
                     return StatusCode(500, "Something went wrong");
                 }
return StatusCode(500,"Can not send email");  
        }

    }
}


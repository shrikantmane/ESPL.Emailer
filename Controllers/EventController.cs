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
        // [HttpPost]
        // [Route("api/[controller]/send")]
        // public string send([FromBody]EventWrapper eventWrapper)
        // {
        //     EventHelper eventHelper = new EventHelper();
        //     bool emailSent = false;

        //     string[] adrs = eventWrapper.eventOptions.to.Split(',');
        //     if (adrs.Count() > 1)
        //         emailSent = eventHelper.SendMultipleEmail(eventWrapper.smtpOptions, eventWrapper.eventOptions);
        //     else
        //         emailSent = eventHelper.SendEmail(eventWrapper.smtpOptions, eventWrapper.eventOptions);

        //     if (emailSent)
        //         return "Email Sent!!";
        //     else
        //         return "Can not send Email";
        // }

        [HttpPost]
        [Route("api/[controller]/send")]
        public IActionResult send([FromBody]EventWrapper eventWrapper)
        {
            try
            {
                if (eventWrapper != null)
                {
                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.server))
                    {
                        return StatusCode(404, "Please specify the email client");
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(eventWrapper.smtpOptions.port)))
                    {
                        return StatusCode(404, "Please specify the SMTP port");
                    }
                    // else
                    // {
                    //     if(Convert.ToInt32(mailWrapper.smtpOptions.port) !=25 && Convert.ToInt32(mailWrapper.smtpOptions.port) !=587 )
                    //         return StatusCode(500, "Invalid SMTP port");
                    // }

                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.user))
                    {
                        return StatusCode(404, "Please specify the 'user'");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.smtpOptions.user);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid SMTP 'user'");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.password))
                    {
                        return StatusCode(404, "Please specify the SMTP 'password'");
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.to))
                    {
                        return StatusCode(404, "'to' address can not be empty");
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.from))
                    {
                        return StatusCode(404, "'from' can not be empty");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.eventOptions.from);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid 'from' address");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.subject))
                    {
                        return StatusCode(404, "'subject' can not be empty");
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.replyTo))
                    {
                        return StatusCode(404, "'replyTo' can not be empty");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.eventOptions.replyTo);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid 'replyTo' address");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.eventName))
                    {
                        return StatusCode(404, "Please specify 'eventName'");
                    }

                    if (eventWrapper.eventOptions.startTime == DateTime.MinValue)
                    {
                        return StatusCode(404, "Please specify event 'startTime'");
                    }
                    else
                    {
                        if (eventWrapper.eventOptions.startTime == null)
                            return StatusCode(400, "Invalid event 'startTime'");
                    }

                    if (eventWrapper.eventOptions.endTime == DateTime.MinValue)
                    {
                        return StatusCode(404, "Please specify event 'endTime'");
                    }
                    else
                    {
                        if (eventWrapper.eventOptions.endTime == null)
                            return StatusCode(400, "Invalid event 'endTime'");
                    }

                    if (eventWrapper.eventOptions.startTime > eventWrapper.eventOptions.endTime)
                    {
                        return StatusCode(400, "Event 'endTime' should be greater than 'startTime'");
                    }

                    // if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.eventDescription))
                    // {
                    //     return StatusCode(404, "Please specify 'eventDescription'");
                    // }
                    // if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.location))
                    // {
                    //     return StatusCode(404, "Please specify event 'location'");
                    // }

                    //valid email addresses of 'to' users
                    string[] adrs = eventWrapper.eventOptions.to.Split(',');
                    if (adrs.Count() > 1)
                    {
                        //      int index = adrs.Length - 1;
                        // System.Net.Mail.MailAddress parsedAddress = MailAddressParser.ParseAddress(adrs, false, ref index);
                        // //Debug.Assert(index == -1, "The index indicates that part of the address was not parsed: " + index);
                        // if(index == -1)
                        //     return StatusCode(500,parsedAddress);
                        // else
                    }
                    else if (adrs.Count() == 1)
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.eventOptions.to);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid 'to' address");
                        }
                    }

                    EventHelper eventHelper = new EventHelper();
                    var m = eventHelper.generateMessage(eventWrapper.eventOptions);
                    if (m != null)
                    {
                        using (var client = new SmtpClient())
                        {
                            try
                            {
                                try
                                {
                                    client.Connect(
                                        eventWrapper.smtpOptions.server,
                                        eventWrapper.smtpOptions.port,
                                        eventWrapper.smtpOptions.useSSL);
                                }
                                catch (System.Exception ex)
                                {
                                    return StatusCode(400, "Invalid SMTP Details");
                                }
                                // Note: since we don't have an OAuth2 token, disable
                                // the XOAUTH2 authentication mechanism.
                                client.AuthenticationMechanisms.Remove("XOAUTH2");

                                try
                                {
                                    client.Authenticate(eventWrapper.smtpOptions.user, eventWrapper.smtpOptions.password);
                                }
                                catch (System.Exception ex)
                                {
                                    return StatusCode(500, "Authentication Failed");
                                }

                                client.Send(m);
                                client.Disconnect(true);
                                return Ok("Email Sent!!");
                            }
                            catch (System.Exception ex)
                            {
                                return StatusCode(500, "Can not send email");
                            }


                        }
                    }
                }
                else if (eventWrapper == null)
                    return StatusCode(400, "Invalid parameters");

            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }
            return StatusCode(500, "Can not send email");
        }

    }
}
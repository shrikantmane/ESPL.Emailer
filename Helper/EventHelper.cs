using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Emailer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Emailer.Helper
{
    public class EventHelper
    {
        public EventHelper()
        {
        }

        public bool SendEmail(
                    SMTPOptions smtpOpt,
                   EventOptions eventOptions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventOptions.to))
                {
                    throw new ArgumentException("no to address provided");
                }

                if (string.IsNullOrWhiteSpace(eventOptions.from))
                {
                    throw new ArgumentException("no from address provided");
                }

                if (string.IsNullOrWhiteSpace(eventOptions.subject))
                {
                    throw new ArgumentException("no subject provided");
                }

                var m = new MimeMessage();

                m.From.Add(new MailboxAddress("", eventOptions.from));
                if (!string.IsNullOrWhiteSpace(eventOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", eventOptions.replyTo));
                }
                m.To.Add(new MailboxAddress("", eventOptions.to));
                m.Subject = eventOptions.subject;

                m.Importance = MessageImportance.Normal;

                BodyBuilder bodyBuilder = new BodyBuilder();

                // //create ics file for event
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string DateFormat = "yyyyMMddTHHmmssZ";
                string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:PUBLISH");

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + eventOptions.startTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + eventOptions.endTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("ORGANIZER;CN= " + "Bharati S" + ":MAILTO:" + "bhartishinde01@gmail.com");
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + eventOptions.eventDescription);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + eventOptions.location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + eventOptions.eventName);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");

                sb.AppendLine("END:VCALENDAR");

                var calendarBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(calendarBytes);
                bodyBuilder.Attachments.Add("event.ics", ms);
                m.Body = bodyBuilder.ToMessageBody();
                using (var client = new SmtpClient())
                {
                    try
                    {
                        client.Connect(
                        smtpOpt.server,
                        smtpOpt.port,
                        smtpOpt.useSSL);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(smtpOpt.user, smtpOpt.password);

                        client.Send(m);
                        client.Disconnect(true);
                        return true;

                    }
                    catch (System.Exception)
                    {
                        return false;
                    }
                }

            }
            catch (System.Exception)
            {
                return false;
            }

        }

        public bool SendMultipleEmail(
                    SMTPOptions smtpOpt,
                   EventOptions eventOptions)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventOptions.to))
                {
                    throw new ArgumentException("no to addresses provided");
                }

                if (string.IsNullOrWhiteSpace(eventOptions.from))
                {
                    throw new ArgumentException("no from address provided");
                }

                if (string.IsNullOrWhiteSpace(eventOptions.subject))
                {
                    throw new ArgumentException("no subject provided");
                }

                var m = new MimeMessage();
                m.From.Add(new MailboxAddress("", eventOptions.from));
                if (!string.IsNullOrWhiteSpace(eventOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", eventOptions.replyTo));
                }
                string[] adrs = eventOptions.to.Split(',');

                foreach (string item in adrs)
                {
                    if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                }

                m.Subject = eventOptions.subject;
                m.Importance = MessageImportance.Normal;

                BodyBuilder bodyBuilder = new BodyBuilder();

                //create ics file for event
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string DateFormat = "yyyyMMddTHHmmssZ";
                string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:PUBLISH");

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + eventOptions.startTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + eventOptions.endTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("ORGANIZER;CN= " + "Bharati S" + ":MAILTO:" + "bhartishinde01@gmail.com");
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + eventOptions.eventDescription);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + eventOptions.location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + eventOptions.eventName);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");

                sb.AppendLine("END:VCALENDAR");

                var calendarBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(calendarBytes);
                bodyBuilder.Attachments.Add("event.ics", ms);
                m.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    try
                    {
                        client.Connect(
                            smtpOpt.server,
                            smtpOpt.port,
                            smtpOpt.useSSL);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        client.Authenticate(
                            smtpOpt.user,
                            smtpOpt.password);

                        client.Send(m);
                        client.Disconnect(true);
                        return true;
                    }
                    catch (System.Exception)
                    {
                        return false;
                    }

                }
            }
            catch (System.Exception)
            {
                return false;
            }
        }

         public MimeMessage generateMessage(EventOptions eventOptions)
        {

            var m = new MimeMessage();
            try
            {
                m.From.Add(new MailboxAddress("", eventOptions.from));
                if (!string.IsNullOrWhiteSpace(eventOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", eventOptions.replyTo));
                }

//'to' users addition
                string[] adrs = eventOptions.to.Split(',');
                if (adrs.Count() > 1)
                {
                    foreach (string item in adrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (adrs.Count() == 1)
                {
                    m.To.Add(new MailboxAddress("", eventOptions.to));
                }

                //'cc' users addition
 string[] ccAdrs = eventOptions.cc.Split(',');
                if (ccAdrs.Count() > 1)
                {
                    foreach (string item in ccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Cc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (ccAdrs.Count() == 1)
                {
                    m.Cc.Add(new MailboxAddress("", eventOptions.cc));
                }

                //'bcc' users addition
 string[] bccAdrs = eventOptions.bcc.Split(',');
                if (bccAdrs.Count() > 1)
                {
                    foreach (string item in bccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Bcc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (bccAdrs.Count() == 1)
                {
                    m.Bcc.Add(new MailboxAddress("", eventOptions.bcc));
                }


                m.Subject = eventOptions.subject;

               m.Importance = MessageImportance.Normal;

                BodyBuilder bodyBuilder = new BodyBuilder();

                //create ics file for event
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string DateFormat = "yyyyMMddTHHmmssZ";
                string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:PUBLISH");

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + eventOptions.startTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + eventOptions.endTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("ORGANIZER;CN= " + "Bharati S" + ":MAILTO:" + "bhartishinde01@gmail.com");
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + eventOptions.eventDescription);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + eventOptions.location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + eventOptions.eventName);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");

                sb.AppendLine("END:VCALENDAR");

                var calendarBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(calendarBytes);
                bodyBuilder.Attachments.Add("event.ics", ms);
                m.Body = bodyBuilder.ToMessageBody();

                return m;
            }
            catch (System.Exception ex)
            {
                return null;
            }
            return m;
        }

    }
}
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Emailer.Models;
// using System.Text.StringBuilder;
// using System.IO.MemoryStream;

namespace Emailer.Helper
{
 public class EventHelper
    {
        public EventHelper()
        {
        }

public async Task SendEmailAsync(
            SMTPOptions smtpOpt,
           MailOptions mailOptions,
           EventOptions eventOptions)
        {
            if (string.IsNullOrWhiteSpace(mailOptions.to))
            {
                throw new ArgumentException("no to address provided");
            }

            if (string.IsNullOrWhiteSpace(mailOptions.from))
            {
                throw new ArgumentException("no from address provided");
            }

            if (string.IsNullOrWhiteSpace(mailOptions.subject))
            {
                throw new ArgumentException("no subject provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("no message provided");
            }

            var m = new MimeMessage();
          
            m.From.Add(new MailboxAddress("", mailOptions.from));
            if(!string.IsNullOrWhiteSpace(mailOptions.replyTo))
            {
                m.ReplyTo.Add(new MailboxAddress("", mailOptions.replyTo));
            }
            m.To.Add(new MailboxAddress("", mailOptions.to));
            m.Subject = mailOptions.subject;

            //m.Importance = MessageImportance.Normal;
            //Header h = new Header(HeaderId.Precedence, "Bulk");
            //m.Headers.Add()

            BodyBuilder bodyBuilder = new BodyBuilder();
            if(hasPlainText)
            {
                bodyBuilder.TextBody = mailOptions.plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = mailOptions.htmlMessage;
            }

           

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
                bodyBuilder.Attachments.Add("event.ics",ms);
            m.Body = bodyBuilder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(
                    smtpOpt.server,
                    smtpOpt.port,
                    smtpOpt.useSSL)
                    .ConfigureAwait(false);
                }
                catch (System.Exception)
                {
                   throw new ArgumentException("Can not send email.");
                }
                
               
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                // if(smtpOpt.RequiresAuthentication)
                // {
                    await client.AuthenticateAsync(smtpOpt.user, smtpOpt.password)
                        .ConfigureAwait(false);
                //}
               
                await client.SendAsync(m).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

        }

public async Task SendMultipleEmailAsync(
            SMTPOptions smtpOpt,
           MailOptions mailOptions,
           EventOptions eventOptions)
        {
            if (string.IsNullOrWhiteSpace(mailOptions.to))
            {
                throw new ArgumentException("no to addresses provided");
            }

            if (string.IsNullOrWhiteSpace(mailOptions.from))
            {
                throw new ArgumentException("no from address provided");
            }

            if (string.IsNullOrWhiteSpace(mailOptions.subject))
            {
                throw new ArgumentException("no subject provided");
            }

            var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
            var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);
            if (!hasPlainText && !hasHtml)
            {
                throw new ArgumentException("no message provided");
            }

            var m = new MimeMessage();
            m.From.Add(new MailboxAddress("", mailOptions.from));
            string[] adrs = mailOptions.to.Split(',');

            foreach (string item in adrs)
            {
                if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
            }

            m.Subject = mailOptions.subject;
            m.Importance = MessageImportance.High;
          
            BodyBuilder bodyBuilder = new BodyBuilder();
            if (hasPlainText)
            {
                bodyBuilder.TextBody = mailOptions.plainTextMessage;
            }

            if (hasHtml)
            {
                bodyBuilder.HtmlBody = mailOptions.htmlMessage;
            }

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
                bodyBuilder.Attachments.Add("event.ics",ms);
                m.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                await client.ConnectAsync(
                    smtpOpt.server,
                    smtpOpt.port,
                    smtpOpt.useSSL).ConfigureAwait(false);
                 }
                catch (System.Exception)
                {
                   throw new ArgumentException("Can not send email.");
                }
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                // if (smtpOptions.RequiresAuthentication)
                // {
                    await client.AuthenticateAsync(
                        smtpOpt.user,
                        smtpOpt.password).ConfigureAwait(false);
                //}

                await client.SendAsync(m).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }

        }

}
}
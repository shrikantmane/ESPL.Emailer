using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading.Tasks;
using Emailer.Models;

namespace Emailer.Helper
{
 public class EmailHelper
    {
        public EmailHelper()
        {
        }

public async Task SendEmailAsync(
            SMTPOptions smtpOpt,
           MailOptions mailOptions)
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
           MailOptions mailOptions)
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
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Net;
using System.Threading.Tasks;
using Emailer.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Emailer.Helper
{
    public class MailHelper
    {
        public MailHelper()
        {
        }

        public bool SendEmail(
                    SMTPOptions smtpOpt,
                   MailOptions mailOptions)
        {

            try
            {
                var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
                var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);
                // if (!hasPlainText && !hasHtml)
                // {
                //     throw new ArgumentException("no message provided");
                // }

                var m = new MimeMessage();

                m.From.Add(new MailboxAddress("", mailOptions.from));
                if (!string.IsNullOrWhiteSpace(mailOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", mailOptions.replyTo));
                }
                m.To.Add(new MailboxAddress("", mailOptions.to));
                m.Subject = mailOptions.subject;

                m.Importance = MessageImportance.Normal;

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
                        client.Connect(
                            smtpOpt.server,
                            smtpOpt.port,
                            smtpOpt.useSSL);

                        // Note: since we don't have an OAuth2 token, disable
                        // the XOAUTH2 authentication mechanism.
                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                        try
                        {
                            client.Authenticate(smtpOpt.user, smtpOpt.password);
                        }
                        catch (System.Exception ex)
                        {
                            throw new ArgumentException("Authentication Failed");
                        }

                        client.Send(m);
                        client.Disconnect(true);
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }

        public bool SendMultipleEmail(
                    SMTPOptions smtpOpt,
                   MailOptions mailOptions)
        {
            try
            {

                var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
                var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);
                // if (!hasPlainText && !hasHtml)
                // {
                //     throw new ArgumentException("no message provided");
                // }

                var m = new MimeMessage();
                m.From.Add(new MailboxAddress("", mailOptions.from));
                string[] adrs = mailOptions.to.Split(',');

                foreach (string item in adrs)
                {
                    if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                }

                m.Subject = mailOptions.subject;
                m.Importance = MessageImportance.Normal;

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
                    catch (System.Exception ex)
                    {
                        return false;
                    }
                }
            }
            catch (System.Exception ex)
            {
                return false;
            }

        }


        public MimeMessage generateMessage(MailOptions mailOptions)
        {

            var m = new MimeMessage();
            try
            {
                var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
                var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);


                m.From.Add(new MailboxAddress("", mailOptions.from));
                if (!string.IsNullOrWhiteSpace(mailOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", mailOptions.replyTo));
                }

                string[] adrs = mailOptions.to.Split(',');
                if (adrs.Count() > 1)
                {
                    foreach (string item in adrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (adrs.Count() == 1)
                {
                    m.To.Add(new MailboxAddress("", mailOptions.to));
                }

                m.Subject = mailOptions.subject;

                m.Importance = MessageImportance.Normal;

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
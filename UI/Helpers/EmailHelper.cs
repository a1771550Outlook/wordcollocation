using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BLL;
using UI.Models;

namespace UI.Helpers
{
	//public enum MailCategory
	//{
	//	Contact,
	//	Register
	//}
	public static class EmailHelper
	{
		private static SmtpClient GetSmtpClient()
		{
			SmtpClient client = new SmtpClient();

			string userId = SiteConfiguration.MailID;
			string password = SiteConfiguration.MailPassword;
			client.Credentials = new NetworkCredential(userId, password);
			client.Port = SiteConfiguration.MailPort;
			client.Host = SiteConfiguration.MailServer;
			return client;
		}
		public static void SendMail_Contact(Contact contact)
		{
			try
			{
				SmtpClient client = GetSmtpClient();
				MailMessage mail = new MailMessage { From = new MailAddress(SiteConfiguration.MailSender) };
				mail.To.Add(SiteConfiguration.MailReceiver);
				mail.Subject = SiteConfiguration.ContactMailSubject;
				mail.IsBodyHtml = true;
				string body = string.Format("<p>username: {0}</p><p>email: {1}</p><p>message: {2}</p>", contact.Username, contact.Email, contact.Message);
				mail.Body = body;
				client.Send(mail);
			}
			catch (SmtpException ex)
			{
				throw new Exception(ex.Message, ex.InnerException);
			}
		}

		/// <summary>
		/// Helper method for sending email
		/// </summary>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		/// <param name="receiverMail"></param>
		public static async Task SendMailAsnyc(string subject, string body, string receiverMail)
		{
			try
			{
				SmtpClient SmtpServer = new SmtpClient
				{
					Credentials = new NetworkCredential(SiteConfiguration.MailID, SiteConfiguration.MailPassword),
					Port = SiteConfiguration.MailPort,
					Host = SiteConfiguration.MailServer
				};
				MailMessage mail = new MailMessage { From = new MailAddress(SiteConfiguration.MailSender) };
				mail.To.Add(receiverMail);
				mail.Subject = subject;
				mail.Body = body;
				mail.IsBodyHtml = SiteConfiguration.IsMailHtml;
				await Task.Run(() => SmtpServer.Send(mail));
			}
			catch (SmtpException ex)
			{
				throw new SmtpException("Email Sending Error", ex.InnerException);
			}
		}
	}
}
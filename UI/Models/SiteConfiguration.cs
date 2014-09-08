using System;
using System.Web.UI.WebControls;

namespace UI.Models
{
	public static class SiteConfiguration
	{
		public static int MinPasswordLength { get
		{
			return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MinPasswordLength"]);
		} }
		public static int MaxPasswordLength
		{
			get
			{
				return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MaxPasswordLength"]);
			}
		}
		
		public static string CaptchaFont { get { return System.Configuration.ConfigurationManager.AppSettings["CaptchaFont"]; } }
		public static string CaptchaHeight { get { return System.Configuration.ConfigurationManager.AppSettings["CaptchaHeight"]; } }
		public static string CaptchaWidth { get { return System.Configuration.ConfigurationManager.AppSettings["CaptchaWidth"]; } }
		public static string MailSenderName { get { return System.Configuration.ConfigurationManager.AppSettings["MailSenderName"]; } }
		public static string Comments { get { return System.Configuration.ConfigurationManager.AppSettings["Comments"]; } }
		public static string CommentsC { get { return System.Configuration.ConfigurationManager.AppSettings["CommentsC"]; } }
		//public static string ReplyComments { get { return replyComments; } }
		//public static string ReplyCommentsC { get { return replyCommentsC; } }
		//public static string MemberZone { get { return memberZone; } }
		//public static string MemberZoneC { get { return memberZoneC; } }
		//public static string WordCollocationsC { get { return wordCollocationsC; } }
		//public static string WordCollocations { get { return wordCollocations; } }
		//public static string ContextualPhrasesC { get { return contextualPhrasesC; } }
		//public static string ContextualPhrases { get { return contextualPhrases; } }
		public static string HostRootPath { get { return System.Configuration.ConfigurationManager.AppSettings["HostRootPath"]; } }
		public static string MailID { get { return System.Configuration.ConfigurationManager.AppSettings["MailID"]; } }
		public static string MailServer { get { return System.Configuration.ConfigurationManager.AppSettings["MailServer"]; } }
		public static string MailSender { get { return System.Configuration.ConfigurationManager.AppSettings["MailSender"]; } }
		public static string MailReceiver { get { return System.Configuration.ConfigurationManager.AppSettings["MailReceiver"]; } }
		public static string MailPassword { get { return System.Configuration.ConfigurationManager.AppSettings["MailPassword"]; } }
		public static string ErrorLogEmail { get { return System.Configuration.ConfigurationManager.AppSettings["ErrorLogEmail"]; } }
		public static bool EnableErrorLogEmail { get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableErrorLogEmail"]); } }
		public static string ErrorMailSubject { get { return System.Configuration.ConfigurationManager.AppSettings["ErrorMailSubject"]; } }
		public static int MailPort { get { return int.Parse(System.Configuration.ConfigurationManager.AppSettings["MailPort"]); } }
		public static bool IsMailHtml { get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("IsMailHtml")); } }
		//NeedCredentials
		public static bool NeedCredentials { get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("NeedCredentials")); } }
		//UseDefaultCredentials
		public static bool UseDefaultCredentials { get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("UseDefaultCredentials")); } }

		public static string DateTimeFormat { get { return System.Configuration.ConfigurationManager.AppSettings.Get("DateTimeFormat"); } }
		public static string DateTimeFormatString { get { return System.Configuration.ConfigurationManager.AppSettings.Get("DateTimeFormatString"); } }

		public static Unit AvatarWidth { get { return Unit.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("AvatarWidth")); } }
		public static Unit AvatarHeight { get { return Unit.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("AvatarHeight")); } }
		public static string AvatarDefaultImage { get { return System.Configuration.ConfigurationManager.AppSettings.Get("AvatarDefaultImage"); } }
		public static string WebUrlRegEx { get { return System.Configuration.ConfigurationManager.AppSettings.Get("WebUrlRegEx"); } }

		public static int GravatarSize { get { return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("GravatarSize")); } }
		//public static GravatarRating GravatarRating
		//{
		//	get
		//	{
		//		string rating = System.Configuration.ConfigurationManager.AppSettings.Get("GravatarRating");
		//		GravatarRating grating = GravatarRating.Default;
		//		switch(rating)
		//		{
		//			case "G":
		//				grating = GravatarRating.G;
		//				break;
		//			case "R":
		//				grating = GravatarRating.R;
		//				break;
		//			case "X":
		//				grating = GravatarRating.X;
		//				break;
		//			case "PG":
		//				grating = GravatarRating.PG;
		//				break;
		//			case "Default":
		//				grating = GravatarRating.Default;
		//				break;
		//		}
		//		return grating;
		//	}
		//}
		//public static GravatarDefaultImageBehavior GravatarDefaultImageBehavior
		//{
		//	get 
		//	{ 
		//		string behavior = System.Configuration.ConfigurationManager.AppSettings.Get("GravatarDefaultBehavior");
		//		GravatarDefaultImageBehavior gbehavior = GravatarDefaultImageBehavior.Default;
		//		switch(behavior)
		//		{
		//			case "Retro":
		//				gbehavior = GravatarDefaultImageBehavior.Retro;
		//				break;
		//			case "MonsterId":
		//				gbehavior= GravatarDefaultImageBehavior.MonsterId;
		//				break;
		//			case "Identicon":gbehavior = GravatarDefaultImageBehavior.Identicon;
		//				break;
		//			case "MysteryMan":gbehavior = GravatarDefaultImageBehavior.MysteryMan;
		//				break;
		//			case "Default":
		//				gbehavior = GravatarDefaultImageBehavior.Default;
		//				break;
		//			case "Wavatar":
		//				gbehavior = GravatarDefaultImageBehavior.Wavatar;
		//				break;
		//		}
		//		return gbehavior;
		//	}
		//}
		public static string GravatarDefaultImage { get { return System.Configuration.ConfigurationManager.AppSettings.Get("GravatarDefaultImage"); } }
		public static string GravatarUrl { get { return System.Configuration.ConfigurationManager.AppSettings.Get("GravatarUrl"); } }
		public static string DomainName { get { return System.Configuration.ConfigurationManager.AppSettings.Get("DomainName"); } }
		public static string WcExampleSources { get { return System.Configuration.ConfigurationManager.AppSettings["WcExampleSources"]; } }
		public static string LogsPath { get { return System.Configuration.ConfigurationManager.AppSettings["LogsPath"]; } }
		public static double CacheExpiration_Minutes { get { return double.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("CacheExpiration_Minutes")); } }
		public static string IrregularVerbsXMLFile { get { return System.Configuration.ConfigurationManager.AppSettings.Get("IrregularVerbsXMLFile"); } }
		public static string RegularVerbsXMLFile { get { return System.Configuration.ConfigurationManager.AppSettings.Get("RegularVerbsXMLFile"); } }
		public static string RegularVerbsXMLFile1 { get { return System.Configuration.ConfigurationManager.AppSettings.Get("RegularVerbsXMLFile1"); } }
		
		public static string SecurityKey { get { return System.Configuration.ConfigurationManager.AppSettings.Get("SecurityKey"); } }
		public static string KeyPath { get { return System.Configuration.ConfigurationManager.AppSettings.Get("KeyPath"); } }
		public static string KeyFileName { get { return System.Configuration.ConfigurationManager.AppSettings.Get("KeyFileName"); } }
		public static string OxfordDictUrl { get { return System.Configuration.ConfigurationManager.AppSettings.Get("OxfordDictUrl"); } }
		public static string ChDictUrl { get { return System.Configuration.ConfigurationManager.AppSettings.Get("ChDictUrl"); } }
		public static string AppConfigFileName_Remote { get { return System.Configuration.ConfigurationManager.AppSettings.Get("AppConfigFileName_Remote"); } }
		public static bool EnableOAuth { get { return bool.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("EnableOAuth")); } }

		public static string ContactMailSubject { get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("ContactMailSubject");
		} }

		public static int WcColListPageSize { get { return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("WcColListPageSize")); } }

		public static int WcViewPageSize { get { return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings.Get("WcViewPageSize")); } }

		public static string DictionaryLinkZht { get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("DictionaryLinkZht");
		} }
		public static string DictionaryLinkZhs { get
		{
			return System.Configuration.ConfigurationManager.AppSettings.Get("DictionaryLinkZhs");
		} }
		public static string DictionaryLinkJap
		{
			get
			{
				return System.Configuration.ConfigurationManager.AppSettings.Get("DictionaryLinkJap");
			}
		}

		public static string SiteUrl { get { return System.Configuration.ConfigurationManager.AppSettings.Get("SiteUrl"); } }
	}
}
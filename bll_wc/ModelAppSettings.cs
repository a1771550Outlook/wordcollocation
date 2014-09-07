using System;
using System.Configuration;

namespace BLL
{
	public static class ModelAppSettings
	{
		//public static string HostRootPath { get { return ConfigurationManager.AppSettings.Get("HostRootPath"); } }
		public static string CountryXMLFile { get { return ConfigurationManager.AppSettings.Get("CountryXMLFile"); } }
		public static string TaiwanCityXMLFile { get { return ConfigurationManager.AppSettings.Get("TaiwanCityXMLFile"); } }
		public static string ChinaCityXMLFile { get { return ConfigurationManager.AppSettings.Get("ChinaCityXMLFile"); } }
		public static string JapanCityXMLFile { get { return ConfigurationManager.AppSettings.Get("JapanCityXMLFile"); } }
		public static string PrepostionXMLFile { get { return ConfigurationManager.AppSettings.Get("PrepositionXMLFile"); } }
		public static string IrregularVerbsXMLFile { get { return ConfigurationManager.AppSettings.Get("IrregularVerbsXMLFile"); } }
		public static string RegularVerbsXMLFile { get { return ConfigurationManager.AppSettings.Get("RegularVerbsXMLFile"); } }
		public static string RegularVerbsXMLFile1 { get { return ConfigurationManager.AppSettings.Get("RegularVerbsXMLFile1"); } }

		public static double CacheExpiration_Minutes { get
		{
			return Convert.ToDouble(ConfigurationManager.AppSettings.Get("CacheExpiration_Minutes"));
		} }
	}
}

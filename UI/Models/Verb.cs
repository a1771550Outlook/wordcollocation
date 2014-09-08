using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BLL;

namespace UI.Models
{
	[Serializable]
	public class Verb
	{
		public string RegularVerb { get; set; }
		public string Infinitive { get; set; }
		public string SimplePast { get; set; }
		public string PastParticiple { get; set; }

		private string filepath;

		public List<string> GetRegularVerbList()
		{
			filepath = System.Web.HttpContext.Current.Server.MapPath(ModelAppSettings.RegularVerbsXMLFile);
			XElement xelement = XElement.Load(filepath);
			IEnumerable<XElement> verbs = xelement.Elements();
			return verbs.Select(verb => verb.Value).ToList();
		}  

		public Dictionary<string, string> GetIrregluarVerbList()
		{
			Dictionary<string,string> verblist=new Dictionary<string, string>();
			filepath = System.Web.HttpContext.Current.Server.MapPath(ModelAppSettings.IrregularVerbsXMLFile);
			XElement xelement = XElement.Load(filepath);
			IEnumerable<XElement> verbs = xelement.Elements();
			foreach (var verb in verbs)
			{
				/*
				 * <Verb>
		<Infinitive>abide</Infinitive>
		<SimplePast>abided / abode</SimplePast>
		<PastParticiple>abided</PastParticiple>
	</Verb>
				 */
				Infinitive = verb.Element("Infinitive").Value.Trim();
				SimplePast = verb.Element("SimplePast").Value.Trim();
				PastParticiple = verb.Element("PastParticiple").Value.Trim();
				verblist.Add(Infinitive, string.Concat(SimplePast,"|",PastParticiple));
			}
			return verblist;
		} 
	}
}

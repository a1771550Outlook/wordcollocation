using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BLL
{
	[Serializable]
	public class Verb
	{
		public string RegularVerb { get; set; }
		public string Infinitive { get; set; }
		public string PastTense { get; set; }
		public string PastParticiple { get; set; }

		private string filepath;

		public List<string> GetRegularVerbList()
		{
			filepath = System.Web.HttpContext.Current.Server.MapPath(ModelAppSettings.RegularVerbsXMLFile);
			XElement xelement = XElement.Load(filepath);
			IEnumerable<XElement> verbs = xelement.Elements();
			return verbs.Select(verb => verb.Value).ToList();
		}  

		public Dictionary<string, string[]> GetIrregluarVerbList()
		{
			Dictionary<string,string[]> verblist=new Dictionary<string, string[]>();
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
				var xElement = verb.Element("Infinitive");
				if (xElement != null) Infinitive = xElement.Value.Trim();
				var element = verb.Element("SimplePast");
				if (element != null) PastTense = element.Value.Trim();
				var xElement1 = verb.Element("PastParticiple");
				if (xElement1 != null) PastParticiple = xElement1.Value.Trim();
				//verblist.Add(Infinitive, string.Concat(SimplePast,"|",PastParticiple));
				verblist.Add(Infinitive, new []{PastTense,PastParticiple});
			}
			return verblist;
		}

		public List<string> GetRegularVerbList1()
		{
			filepath = System.Web.HttpContext.Current.Server.MapPath(ModelAppSettings.RegularVerbsXMLFile1);
			XElement xelement = XElement.Load(filepath);
			IEnumerable<XElement> verbs = xelement.Elements();
			return verbs.Select(verb => verb.Value).ToList();
		}
	}
}

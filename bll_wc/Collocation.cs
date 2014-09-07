using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BLL.Abstract;

namespace BLL
{
	public enum CollocationPattern
	{
		noun_verb,
		verb_noun,
		adjective_noun,
		verb_preposition,
		preposition_verb, //not used, but must NOT be deleted; otherwise the current examples in the db cannot be shown...
		adverb_verb,
		phrase_noun,
		preposition_noun,
	}

	public class Collocation:WcBase,ICollocation
	{
		[Required]
		public short posId { get; set; }
		[Required]
		public short colPosId { get; set; }
		[Required]
		public long wordId { get; set; }
		[Required]
		public long colWordId { get; set; }

		public string[] PatternTextArray { get; set; }
		public string PatternTextString { get; set; }

		#region ICollocation Members

		public string Word { get; set; }

		public string ColWord { get; set; }
		

		public string ColWordZhs { get; set; }
		

		public string WordZht { get; set; }
		

		public string WordZhs { get; set; }
		

		public string ColWordZht { get; set; }
		

		public string WordJap { get; set; }
		

		public string ColWordJap { get; set; }
		

		public string Pos { get; set; }
		

		public string ColPos { get; set; }
		
		public string PosZht { get; set; }
		

		public string ColPosZht { get; set; }
		
		public string PosJap { get; set; }
		

		public string PosZhs { get; set; }
		

		public string ColPosJap { get; set; }
		

		public string ColPosZhs { get; set; }


		public List<WcExample> WcExampleList { get; set; }

		
		public CollocationPattern CollocationPattern { get; set; }

		public new long Id { get; set; }

		#endregion
	}
}

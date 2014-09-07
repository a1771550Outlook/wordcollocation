using System.Collections.Generic;
using System.Linq;

namespace BLL.Helpers
{
	public static class BLLHelper
	{
		private static string[] GetPatternTrans(CollocationPattern collocationPattern)
		{
			var culturename = CultureHelper.GetCurrentCulture();
			string[] pattern = new string[2];
			const string adj = "adjective", noun = "noun", adv = "adverb", verb = "verb", prep = "preposition", phrase = "phrase";
			var repo = new PosRepository();
			string adj_tran;
			string noun_tran;
			string adv_tran;
			string verb_tran;
			string prep_tran;
			string phrase_tran;
			if (culturename.Contains("hans"))
			{
				adj_tran = repo.GetPos_TranByEntry(adj)[1];
				noun_tran = repo.GetPos_TranByEntry(noun)[1];
				adv_tran = repo.GetPos_TranByEntry(adv)[1];
				verb_tran = repo.GetPos_TranByEntry(verb)[1];
				prep_tran = repo.GetPos_TranByEntry(prep)[1];
				phrase_tran = repo.GetPos_TranByEntry(phrase)[1];
			}
			else if (culturename.Contains("ja"))
			{
				adj_tran = repo.GetPos_TranByEntry(adj)[2];
				noun_tran = repo.GetPos_TranByEntry(noun)[2];
				adv_tran = repo.GetPos_TranByEntry(adv)[2];
				verb_tran = repo.GetPos_TranByEntry(verb)[2];
				prep_tran = repo.GetPos_TranByEntry(prep)[2];
				phrase_tran = repo.GetPos_TranByEntry(phrase)[2];
			}
			else
			{
				adj_tran = repo.GetPos_TranByEntry(adj)[0];
				noun_tran = repo.GetPos_TranByEntry(noun)[0];
				adv_tran = repo.GetPos_TranByEntry(adv)[0];
				verb_tran = repo.GetPos_TranByEntry(verb)[0];
				prep_tran = repo.GetPos_TranByEntry(prep)[0];
				phrase_tran = repo.GetPos_TranByEntry(phrase)[0];
			}

			switch (collocationPattern)
			{
				case CollocationPattern.adjective_noun:
					pattern[0] = "adjective + noun";
					pattern[1] = string.Format("({0} + {1})", adj_tran, noun_tran);
					break;
				case CollocationPattern.adverb_verb:
					pattern[0] = "adverb + verb";
					pattern[1] = string.Format("({0} + {1})", adv_tran, verb_tran);
					break;
				case CollocationPattern.noun_verb:
					pattern[0] = "noun + verb";
					pattern[1] = string.Format("({0} + {1})", noun_tran, verb_tran);
					break;
				case CollocationPattern.phrase_noun:
					pattern[0] = "phrase + noun";
					pattern[1] = string.Format("({0} + {1})", phrase_tran, noun_tran);
					break;
				case CollocationPattern.preposition_noun:
					pattern[0] = "preposition + noun";
					pattern[1] = string.Format("({0} + {1})", prep_tran, noun_tran);
					break;
				case CollocationPattern.verb_noun:
					pattern[0] = "verb + noun";
					pattern[1] = string.Format("({0} + {1})", verb_tran, noun_tran);
					break;
				case CollocationPattern.verb_preposition:
					pattern[0] = "verb + preposition";
					pattern[1] = string.Format("({0} + {1})", verb_tran, prep_tran);
					break;
			}
			return pattern;
		}
		public static string[] GetPatternArray(CollocationPattern collocationPattern)
		{
			var pattern = GetPatternTrans(collocationPattern);
			return pattern;
		}
		public static string GetPatternString(CollocationPattern collocationPattern)
		{
			var pattern = GetPatternTrans(collocationPattern);
			return string.Format("{0} {1}", pattern[0], pattern[1]);
		}

		public static List<ColWord> GetColWordListByWordPattern(string word, CollocationPattern pattern)
		{
			var collocationList = new CollocationRepository().GetCollocationListByWordPattern(word, pattern);

			return collocationList.Select(collocation => new ColWord
			{
				Entry = collocation.ColWord,
				EntryZht = collocation.ColWordZht,
				EntryZhs = collocation.ColWordZhs,
				EntryJap = collocation.ColWordJap,
				Id = collocation.colWordId.ToString(),
			}).OrderBy(cw => cw.Entry).ToList();
		}

		public static Collocation GetCollocationByCollocationId(long collocationId)
		{
			var repo = new CollocationRepository();
			return repo.GetObjectById(collocationId.ToString());
		}

		public static List<WcExample> GetWcExampleListByCollocationId(long collocationId)
		{
			var repo = new WcExampleRepository();
			return repo.GetObjectListByCollocationId(collocationId);
		}

		public static List<Collocation> GetCollocationListByWordPattern(string word, CollocationPattern collocationPattern)
		{
			var repo = new CollocationRepository();
			return repo.GetCollocationListByWordPattern(word, collocationPattern);
		}
	}
}

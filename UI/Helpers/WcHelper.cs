using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using BLL;
using BLL.Helpers;
using Microsoft.VisualBasic;
using UI.Models;
using WcResources;
using Verb = BLL.Verb;

namespace UI.Helpers
{
	/// <summary>
	/// Contains static text methods.
	/// Put this in a separate class in your project.
	/// </summary>
	public static class TextTools
	{
		/// <summary>
		/// Count occurrences of strings.
		/// </summary>
		public static int CountStringOccurrences(string text, string pattern)
		{
			// Loop through all instances of the string 'text'.
			int count = 0;
			int i = 0;
			while ((i = text.IndexOf(pattern, i)) != -1)
			{
				i += pattern.Length;
				count++;
			}
			return count;
		}

		/// <summary>
		/// Encrypt a string using dual encryption method. Return a encrypted cipher Text
		/// </summary>
		/// <param name="toEncrypt">string to be encrypted</param>
		/// <param name="useHashing">use hashing? send to for extra secirity</param>
		/// <returns></returns>
		public static string Encrypt(string toEncrypt, bool useHashing)
		{
			byte[] keyArray;
			byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

			// Get the key from config file
			string key = SiteConfiguration.SecurityKey;

			var salt = Guid.NewGuid().ToString();
			var path = Path.Combine(SiteConfiguration.KeyPath, SiteConfiguration.KeyFileName);
			StreamWriter writer = new StreamWriter(path, false);
			using (writer)
			{
				writer.WriteLine(salt);
			}
			key += salt;

			if (useHashing)
			{
				MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
				keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
				hashmd5.Clear();
			}
			else
				keyArray = Encoding.UTF8.GetBytes(key);

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
			{
				Key = keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			ICryptoTransform cTransform = tdes.CreateEncryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
			tdes.Clear();
			return Convert.ToBase64String(resultArray, 0, resultArray.Length);
		}
		/// <summary>
		/// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
		/// </summary>
		/// <param name="cipherString">encrypted string</param>
		/// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
		/// <returns></returns>
		public static string Decrypt(string cipherString, bool useHashing)
		{
			byte[] keyArray;
			byte[] toEncryptArray = Convert.FromBase64String(cipherString);

			//get the salt value
			var path = Path.Combine(SiteConfiguration.KeyPath, SiteConfiguration.KeyFileName);
			StreamReader reader = new StreamReader(path);
			string salt;
			using (reader)
			{
				salt = reader.ReadLine();
			}

			//Get your key from config file to open the lock!
			string key = SiteConfiguration.SecurityKey;
			key += salt;

			if (useHashing)
			{
				MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
				keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(key));
				hashmd5.Clear();
			}
			else
				keyArray = Encoding.UTF8.GetBytes(key);

			TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
			{
				Key = keyArray,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			};

			ICryptoTransform cTransform = tdes.CreateDecryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

			tdes.Clear();
			return Encoding.UTF8.GetString(resultArray);
		}

		public static string HashMD5(string email)
		{
			MD5 md5 = MD5.Create();
			byte[] data = md5.ComputeHash(Encoding.Default.GetBytes(email));
			StringBuilder builder = new StringBuilder();
			foreach (var d in data)
				builder.Append(d.ToString("x2"));
			return builder.ToString();
		}

		/// <summary>
		/// Uppercase first letters of all words in the string.
		/// </summary>
		public static string UpperFirst(string s)
		{
			return Regex.Replace(s, @"\b[a-z]\w+", delegate(Match match)
			{
				string v = match.ToString();
				return char.ToUpper(v[0]) + v.Substring(1);
			});
		}
	}

	/// <summary>
	/// an ad-hoc class to help formatting example with regular expression
	/// </summary>
	//class WcCapture
	//{
	//	public int Index { get; set; }
	//	public string Value { get; set; }
	//}

	//enum VerbForm
	//{
	//	regular,
	//	irregular,
	//	others
	//}
	public static class WcHelper
	{
		private const string RegularVerbListCacheName = "RegularVerbList";
		private const string RegularVerbListCacheName1 = "RegularVerbList1";
		private const string IrregularVerbListCacheName = "IrregularVerbList";

		private static readonly string DateTimeFormat = SiteConfiguration.DateTimeFormat;
		public enum PhraseStartsWithPOS
		{
			// ReSharper disable UnusedMember.Global
			verb,
			// ReSharper restore UnusedMember.Global
			noun,
			Default
		}
		public enum PhraseIncludes
		{
			ones,

			Default
		}
		public enum ModelAction
		{
			Create,
			Read,
			Update,
			Delete
		}

		public static List<WcExample> GetFormattedExamples(Collocation collocation)
		{
			string verb = collocation.Word;
			string pos = collocation.Pos;
			string colWord = collocation.ColWord;
			string colpos = collocation.ColPos;
			var repo = new WcExampleRepository();
			var examples = repo.GetObjectListByCollocationId(collocation.Id);

			foreach (var example in examples)
			{
				example.Entry = FormatExampleForView(example.Entry, verb, pos, colWord, colpos, collocation.CollocationPattern);
			}

			return examples;
		}

		/// <summary>
		/// </summary>
		/// <param name="example"></param>
		/// <param name="word"></param>
		/// <param name="pos"></param>
		/// <param name="colWord"></param>
		/// <param name="colpos"></param>
		/// <param name="collocationPattern"></param>
		/// <returns></returns>
		public static string FormatExampleForView(string example, string word, string pos, string colWord, string colpos, CollocationPattern collocationPattern)
		{
			#region Init Varibles
			string pattern;
			string replacement;
			//string prep = null;
			//string formattedWord = null; //in need when colword is preposition...
			string exampleWithFormattedWordColWord = null;
			string[] exampleWithFormattedWord;
			Regex regex;
			#endregion

			#region Set some arguments as lower case to smoothen comparsion
			//string exampleLower = example.ToLower();
			word = word.ToLower();
			colWord = colWord.ToLower();
			pos = pos.ToLower();
			colpos = colpos.ToLower();
			#endregion

			if (example.IndexOf(word, StringComparison.OrdinalIgnoreCase) == -1 ||
			    example.IndexOf(colWord, StringComparison.OrdinalIgnoreCase) == -1) return example;

			switch (collocationPattern)
			{
				case CollocationPattern.adjective_noun:
					exampleWithFormattedWord = FormatNounInExample(example, word, true);
					if (exampleWithFormattedWord == null) return null;
					pattern = string.Format(@"({0}){{1}}(,*\s*.*)({1})", colWord, exampleWithFormattedWord[1]); //colword (, and any adjective) word
					regex = new Regex(pattern, RegexOptions.IgnoreCase);
					if (regex.IsMatch(exampleWithFormattedWord[0]))
					{
						replacement = @"<span class=""colWord"">$1</span>$2$3";
						exampleWithFormattedWordColWord = regex.Replace(exampleWithFormattedWord[0], replacement);
					}
					break;
				case CollocationPattern.adverb_verb:
					//format each separately...not like that in the case above...
					exampleWithFormattedWord = FormatVerbInExample(example, word, true);
					if (exampleWithFormattedWord == null) return null;
					//@"(hastily){1}"
					pattern = string.Format("({0}){{1}}", colWord);
					regex = new Regex(pattern, RegexOptions.IgnoreCase);
					if (regex.IsMatch(exampleWithFormattedWord[0]))
					{
						replacement = @"<span class=""colWord"">$1</span>";
						exampleWithFormattedWordColWord = regex.Replace(exampleWithFormattedWord[0], replacement);
					}
					break;
				case CollocationPattern.noun_verb:
					//format each separately
					exampleWithFormattedWord = FormatNounInExample(example, word, true);
					if (exampleWithFormattedWord == null) return null;
					exampleWithFormattedWordColWord = FormatVerbInExample(exampleWithFormattedWord[0], colWord, false)[0];
					break;
				case CollocationPattern.preposition_noun:
					//eg: in sb's absence, in my absence, in her absence, in his absence, in our absence, in their absence, in your absence
					// => absence = word; in = preposition
					string prepForReplace = colWord + "|" + TextTools.UpperFirst(colWord); //in this pattern, 'prep' can have capitalization issue
					//(in|In)(\s+)(sb's|someone's|my|your|his|her|our|their){1}(\s+)([^in])*(absence){1}
					pattern = string.Format(@"({0})(\s+)(sb's|someone's|my|your|his|her|our|their){{1}}(\s+)([^{1}])*({2}){{1}}",
						prepForReplace, colWord, word);
					regex = new Regex(pattern);
					if (regex.IsMatch(example))
					{
						//const string replacement = @"$2<span class=""colWord"">$3</span>$4$5$6<span class=""word"">$7</span>"; 
						replacement = @"<span class=""colWord"">$1</span>$2$3$4$5<span class=""word"">$6</span>";
						exampleWithFormattedWordColWord = regex.Replace(example, replacement);
					}
					break;
				case CollocationPattern.verb_noun:
					//eg 1: I seem to have lost my ability to attract clients.
					//eg 2: This principle has now been effectively abandoned.
					//format each separately
					exampleWithFormattedWord = FormatNounInExample(example, word, true);
					if (exampleWithFormattedWord == null) return null;
					exampleWithFormattedWordColWord = FormatVerbInExample(exampleWithFormattedWord[0], colWord, false)[0];
					break;
				case CollocationPattern.verb_preposition: //also called phrasal verb or prepositional verb; no other words are allowed intersacted between the verb and the preposition
					//eg: agree to => I wish she would agree to my proposal.
					//eg: believe in =>	I believe in God.
					exampleWithFormattedWord = FormatVerbInExample(example, word, true);
					if (exampleWithFormattedWord == null) return null;
					pattern = string.Format(@"({0}){{1}}(\s)+({1}){{1}}", exampleWithFormattedWord[1], colWord, false);
					regex = new Regex(pattern, RegexOptions.IgnoreCase);
					if (regex.IsMatch(exampleWithFormattedWord[0]))
					{
						//<span class="word">$1</span>$2<span class="colWord">$3</span>
						replacement = @"$1$2<span class=""colWord"">$3</span>";
						exampleWithFormattedWordColWord = regex.Replace(exampleWithFormattedWord[0], replacement);
					}
					break;
				case CollocationPattern.phrase_noun:
					// a/the period of ... => Note: currently the phrase must be in the format like this
					// eg: You will not be paid for the full period of absence.

					//first remove the ' ...' from the colword:
					pattern = @"(.+)(\.\.\.)";
					regex = new Regex(pattern, RegexOptions.IgnoreCase);
					if (regex.IsMatch(colWord))
					{
						replacement = @"$1";
						colWord = regex.Replace(colWord, replacement).TrimEnd();
					}

					string[] colwordArray = colWord.Split(' ');
					if (colwordArray.Length == 3) //currently the length of the array is hard-coded...
					{
						if (colwordArray[0].Contains(@"/")) colwordArray[0] = colwordArray[0].Replace(@"/", @"|"); //convert a/the to a|the

						//(\ba|the\b)(.*\s*)(\bperiod\b)(\s+)(\bof\b)(\s+)(\babsence\b)
						pattern = string.Format(@"(\b{0}\b)(.*\s*)(\b{1}\b)(\s+)(\b{2}\b)(\s+)(\b{3}\b)", colwordArray[0], colwordArray[1],
							colwordArray[2], word);
						regex = new Regex(pattern, RegexOptions.IgnoreCase);
						if (regex.IsMatch(example))
						{
							replacement =
								@"<span class=""colWord"">$1</span>$2<span class=""colWord"">$3</span>$4<span class=""colWord"">$5</span>$6<span class=""word"">$7</span>";
							exampleWithFormattedWordColWord = regex.Replace(example, replacement);
						}
					}
					break;
			}
			return exampleWithFormattedWordColWord;
		}

		private static string[] FormatNounInExample(string example, string noun, bool isWord)
		{
			string nounForPattern;
			string[] exampleWithFormattedNoun;

			#region Countable Nouns

			#region Regular

			#region Case1#adding -s or -es
			/*
			 * Most regular countable nouns make their plurals by adding -s to the singular form, but nouns that end in -s, -z, -ch, -sh, and -s add -es to the singular form:
			 * bus / buses* boss / bosses gas / gases lass / lasses kiss / kisses mess / messes pass / passes plus / pluses 
			 * Notes:
a.	 	"Buses" is also spelled "busses." 	 	 
b.	 	Words that end in a vowel + z double the z before adding -es. eg:quiz / quizzes* whiz / whizzes*
			 */

			if (noun.EndsWith("s") || noun.EndsWith("z") || noun.EndsWith("ch") || noun.EndsWith("sh") || noun.EndsWith("s"))
			{
				nounForPattern = noun + "es";
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			else
			{
				nounForPattern = noun + "s";
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}

			//sepcial case#a: "Buses" is also spelled "busses."
			if (noun == "bus")
			{
				nounForPattern = noun + "ses";
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}

			//special case#b: Words that end in a vowel + z => double the z before adding -es eg:quiz / quizzes* whiz / whizzes*
			string pattern = @".+[aeiou]{1}z{1}";
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
			if (regex.IsMatch(noun))
			{
				nounForPattern = noun + "zes";
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			#endregion

			#region Case2#changing -y to i and adding -es
			/*
				 * Most regular countable nouns make their plurals by adding -s to the singular form, but nouns that end in a consonant + y change the y
to i and then add -es to the singular form: 
			 * baby / babies berry / berries buggy / buggies canary / canaries city / cities ferry / ferries filly / fillies
fly / flies flurry / flurries gully / gullies lily / lilies peony / peonies pony / ponies quarry / quarries query / queries salary / salaries story / stories worry / worries 
 	 	
Note: Regular nouns ending in a vowel + y do not change the y to i and add -es; instead, they simply add s:
alloy / alloys boy / boys buoy / buoys decoy / decoys display / displays donkey / donkeys envoy / envoys guy / guys key / keys monkey / monkeys play / plays
ray / rays spray / sprays stray / strays toy / toys tray / trays turkey / turkeys way / ways				 */

			pattern = @".+[^aeiou]{1}y{1}";
			regex = new Regex(pattern, RegexOptions.IgnoreCase);
			if (regex.IsMatch(noun))
			{
				nounForPattern = noun.Remove(noun.Length - 1);
				nounForPattern = nounForPattern + "ies";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}

			pattern = @".+[aeiou]{1}y{1}";
			regex = new Regex(pattern, RegexOptions.IgnoreCase);
			if (regex.IsMatch(noun))
			{
				nounForPattern = noun + "s";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}

			#endregion

			#region Case3#ending in vowels often end in a or e and adding -s

			#region SubCase1#enging in vowel except 'o'
			/*
			 * Regular countable nouns ending in vowels often end in a or e; they form their plurals by adding -s:
area / areas boa / boas cola / colas hula / hulas idea / ideas pea / peas sea / seas ace / aces bone / bones canoe / canoes cove / coves chute / chutes
ski / skis
			 */
			pattern = @".+[aeiu]{1}";
			regex = new Regex(pattern, RegexOptions.IgnoreCase);
			if (regex.IsMatch(noun))
			{
				nounForPattern = noun + "s";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			#endregion

			#region SubCase2#ending in 'o'

			if (noun.EndsWith("o"))
			{
				/*
			* A few regular countable nouns ending in o form plurals by adding -es:
echo / echoes hero / heroes potato / potatoes tomato / tomatoes
			 */
				/*
			 * Many regular countable nouns ending in o (including numerous musical terms) form their plurals by adding only -s:
albino / albinos alto / altos auto / autos basso / bassos cello / cellos duo / duos folio / folios ghetto / ghettos halo / halos kangaroo / kangaroos
			 */
				/*  	Some regular countable nouns ending in o form their plurals by adding either -s or -es, though -es is usually preferred:
grotto / grottoes (grottos) memento / mementoes (mementos) mosquito / mosquitoes (mosquitos) motto / mottoes (motts) tornado / tornadoes (tornados) volcano / volcanoes (volcanos) zero / zeroes (zeros)
*/
				//if (noun == "echo" || noun == "hero" || noun == "potato" || noun == "tomato")
				//{
				nounForPattern = noun + "es";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
				//}
				nounForPattern = noun + "s";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}

			#endregion

			#endregion

			#region Case4#Nouns ending in f

			if (noun.EndsWith("f"))
			{
				/*
			 * Several regular countable nouns ending in f have special plurals: they change the f to v and then add -es:
calf / calves elf / elves half / halves knife / knives leaf / leaves life / lives loaf / loaves scarf / scarves self / selves sheaf / sheaves shelf / shelves thief / thieves wolf / wolves
			 */
				if (noun == "calf" || noun == "elf" || noun == "half" || noun == "knife" || noun == "leaf" || noun == "life" ||
					noun == "loaf" || noun == "scarf" || noun == "self" || noun == "sheaf" || noun == "thief" || noun == "wolf")
				{
					nounForPattern = noun.Remove(noun.Length - 1);
					nounForPattern = nounForPattern + "ves";
					//formattedExample = FormatExampleWithWord(example, noun);
					//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
					exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
					if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
				}

				/* Several regular countable nouns ending in f make their plurals with either -ves or -s:
dwarf / dwarves (dwarfs) hoof / hooves (hoofs) staff / staves (staffs) wharf / wharves (wharfs)  */
				if (noun == "dwarf" || noun == "hoof" || noun == "staff" || noun == "wharf")
				{
					nounForPattern = noun.Remove(noun.Length - 1);
					nounForPattern = nounForPattern + "ves";
					//formattedExample = FormatExampleWithWord(example, nounForPattern);
					//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
					exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
					if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;

					nounForPattern = noun + "s";
					//formattedExample = FormatExampleWithWord(example, noun);
					//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
					exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
					if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
				}

				/* Many regular countable nouns ending in f make their plurals by adding -s:
belief / beliefs bluff / bluffs brief / briefs chief / chiefs cliff / cliffs cuff / cuffs gaff / gaffs muff / muffs puff / puffs roof / roofs whiff / whiffs  */
				nounForPattern = noun + "s";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}


			#endregion

			#endregion

			#region Irregular

			#region Case#1:common words
			/* common words
Here is a list of some common irregular countable nouns:
child children foot feet goose geese man men mouse mice ox oxen penny pence person people tooth teeth woman women */
			if (noun == "child")
			{
				nounForPattern = "children";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "foot")
			{
				nounForPattern = "feet";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "goose")
			{
				nounForPattern = "geese";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				return FormatExampleWithWord(example, nounForPattern, isWord);
			}
			if (noun == "man")
			{
				nounForPattern = "men";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "mouse")
			{
				nounForPattern = "mice";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "ox")
			{
				nounForPattern = "oxen";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "penny")
			{
				nounForPattern = "pence";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "person")
			{
				nounForPattern = "people";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				return FormatExampleWithWord(example, nounForPattern, isWord);
			}
			if (noun == "tooth")
			{
				nounForPattern = "teeth";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun == "woman")
			{
				nounForPattern = "women";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			#endregion

			#region Case#2: foriegn words
			/* foreign words
Here is a list of foreign words with irregular plurals:
analysis analyses bacterium bacteria cactus cacti crisis crises criterion criteria datum data diagnosis diagnoses focus foci  */
			if (noun.EndsWith("sis"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 3);
				nounForPattern = nounForPattern + "ses";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun.EndsWith("um"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 2);
				nounForPattern = nounForPattern + "a";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun.EndsWith("us"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 2);
				nounForPattern = nounForPattern + "i";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun.EndsWith("ion"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 3);
				nounForPattern = nounForPattern + "ia";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun.EndsWith("us"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 2);
				nounForPattern = nounForPattern + "i";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;

				//special case: syllabus syllabi or syllabuses
				nounForPattern = noun + "es";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			/* formula formulae fungus fungi hypothesis hypotheses medium media oasis oases phenomenon phenomena stimulus stimuli  thesis theses
vertebra vertebrae*/
			if (noun.EndsWith("a"))
			{
				nounForPattern = noun + "e";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			if (noun.EndsWith("non"))
			{
				nounForPattern = noun;
				nounForPattern = nounForPattern.Remove(nounForPattern.Length - 2);
				nounForPattern += "na";
				//formattedExample = FormatExampleWithWord(example, noun);
				//if (!string.IsNullOrEmpty(formattedExample)) return formattedExample;
				exampleWithFormattedNoun = FormatExampleWithWord(example, nounForPattern, isWord);
				if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			}
			#endregion

			#endregion

			#endregion

			#region UnCountable Nouns
			//no action is required for this type of noun => nothing to do if a noun can't be added 's'.
			#endregion

			#region Singular Noun the Simplest Form
			exampleWithFormattedNoun = FormatExampleWithWord(example, noun, isWord);
			if (exampleWithFormattedNoun != null) return exampleWithFormattedNoun;
			//string msg = string.Format("{0} cannot be found in the example", noun);
			//throw new Exception(msg);
			return null;

			#endregion
		}

		private static string[] FormatVerbInExample(string example, string verb, bool isWord)
		{
			string verbForPattern, VerbForPattern;
			string[] exampleWithFormattedVerb;

			#region Verb + s

			#region Case1
			//simple base form with 's'
			VerbForPattern = verb + "s";
			exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
			if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			#endregion

			#region Case2
			/*
					* Add - s to the base form. This is the most common spelling for the -S form and is the spelling used for most verbs.
					Notice, especially, that - s is added when the base form ends in one or more consonants + e:
					* eg:aches, bakes, breathes, cares, caches, dives, edges, fiddles, files, glares, hates, hopes, jokes, lives, makes, notes, pastes, races, spares, surprises, tastes, types, writes
					Notice that - s is also added when the base form ends in one or more consonants (but without e):
					* eg:adds, bets, beats, calls, claps, cheats, cleans, digs, drops, eats, fills, finds, fits, gets, grabs, hops, kills,
knits, links, lists, means, needs, opens, puts, quits, robs, rings, rips, sends, stops, tells, trusts, voids, wants, works, zips 
					*/
			string pattern = @"\b.+[^aeiou]+e?\b";
			Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "s";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case3
			/*
				 * In addition, notice this spelling is used with the small number of verbs ending in two vowels (including - ie):
					eg: agrees, argues, boos, coos, flees, glues, moos, sees, shoos, shoes, tees, woos, dies, lies, ties, vies
				 */
			pattern = @"\b.+[aeiou]{2}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "s";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#endregion

			#region Verb + ing

			#region Case1
			//simple base form + 'ing'

			/*
								 * If a verb ends in a stressed vowel + one or more consonants + e or ue, "drop" the e and add - ing.
Examples: abate / abating; ache / aching; bathe / bathing;beliéve / believing; bite / biting; care / caring;deléte / deleting; dive / diving; ensláve / enslaving;excíte / exciting; file / filing; gripe / griping;hope / hoping; joke / joking; live / living;make / making; paráde / parading; paste / pasting;
raise / raising; revíle / reviling; save / saving;smoothe / smoothing; taste / tasting;
								 */
			pattern = @"\b.*[aeiou]{1}[^aeiou]+e{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				verbForPattern = verb.Remove(verb.Length - 1);
				VerbForPattern = verbForPattern + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case2
			/* verb ends in 'ue' =>eg:glue / gluing; rue / ruing; sue / suing*/
			pattern = @"\b.+ue{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				verbForPattern = verb.Remove(verb.Length - 1);
				VerbForPattern = verbForPattern + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case3
			/*
			* If a verb ends in - ie, change the - ie to - y and add - ing.
			Examples:die / dying; lie / lying; tie / tying; vie / vying
			 */
			pattern = @"\b.+ie{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				verbForPattern = verb.Remove(verb.Length - 2).Insert(verb.Length - 2, "ying");
				VerbForPattern = verbForPattern;
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case4
			/*
			* If a verb ends in y, add - ing. It doesn't matter if there is a vowel or a consonant before y.
			* Examples:pry / prying spy / spying pray / praying spay / spaying
			*/
			pattern = @"\b.+y{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case5
			/*If a verb ends in a vowel + one consonant, double the consonant and add - ing.
Examples:beg / begging; chat / chatting; dig / digging;fit / fitting; grin / grinning; grip / gripping;hop / hopping; mix / mixing; nip / nipping;pin / pinning; quit / quitting; rip / ripping;sit / sitting; tip / tipping; win / winning*/
			pattern = @"\b.+[aeiou]{1}[^aeiou]{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				string consonent = Strings.Right(verb, 1);
				VerbForPattern = verb + consonent + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case6
			/* If a verb ends in a stressed vowel + er,double the r and add - ing, but if a verb ends in an unstressed vowel + er, do not double the r: just add - ing: confér / conferring		defér / deferring		refér / referring	 	ánswer / answering		óffer / offering		súffer / suffering */
			//a verb ends in a stressed vowel + er:
			pattern = @"\b.*[aeiou]{1}[^aeiou]+er{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "ring";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case7
			//a verb ends in an unstressed vowel + er:
			//eg: ánswer / answering óffer / offering súffer / suffering
			pattern = @"\b.*[aeiou]+[^aeiou]+er{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			}
			#endregion

			#region Case8
			/* If a verb ends in a vowel, add - ing.
Examples:do / doing; echo / echoing; go / going; ski / skiing */
			pattern = @"\b.+[aeiou]{1}\b";
			regEx = new Regex(pattern);
			if (regEx.IsMatch(verb))
			{
				VerbForPattern = verb + "ing";
				exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
				if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;

				//special case: dye
				if (verb == "dye")
				{
					VerbForPattern = verb + "ing";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
				}
			}
			#endregion

			#region Case9
			//add 'ing' to all other verbs...
			VerbForPattern = verb + "ing";
			exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
			if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
			#endregion

			#endregion

			#region RegularVerb Past Tense
			/* Add -d to the base form.*/
			if (CheckVerbIfRegular(verb))
			{
				#region Case1
				/*This happens when the base form ends in a vowel and one or more consonants plus e:
	ached, baked, blamed, breathed, cared, cached,
	chased. diced, dozed, dyed, edged, fiddled, filed, 
	glared, grated, hated, hoped, joked, lived,
	mired, noted, paced, pasted, raced, raised,
	sliced, spared, surprised, tasted, typed, whined. */
				pattern = @"\b.*[aeiou]{1}[^aeiou]+e{1}\b";
				regEx = new Regex(pattern);
				if (regEx.IsMatch(verb))
				{
					VerbForPattern = verb + "d";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;

					//exceptional case: dyed,typed
					if (verb == "dye" || verb == "type")
					{
						VerbForPattern = verb + "d";
						exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
						if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
					}
				}
				#endregion

				#region Case2
				/* This also happens when the base form ends in ue,oe, or ie:glued, rued, sued, hoed, toed, died, lied, tied */
				pattern = @"\b.+(ue|oe|ie){1}\b";
				regEx = new Regex(pattern);
				if (regEx.IsMatch(verb))
				{
					VerbForPattern = verb + "d";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
				}
				#endregion

				#region Case3
				/* Change -y to -i and add -ed.

	This happens when a verb ends in a consonant and y:

	apply / applied; bully / bullied; bury / buried;
	carry / carried; copy / copied; cry / cried;
	dry / dried; ferry / ferried; fry / fried;
	hurry / hurried; marry / married;
	parry / parried; pry / pried; query / queried;
	rely / relied; tarry / tarried; tidy / tidied;
	try / tried; vary / varied; worry / worried */
				pattern = @"\b.+[^aeiou]{1}y{1}\b";
				regEx = new Regex(pattern);
				if (regEx.IsMatch(verb))
				{
					VerbForPattern = verb + "d";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
				}
				#endregion

				#region Case4
				/*
				 * This does not happen when a verb ends in a vowel and y:

	annoy / annoyed; bray / brayed; destroy / destroyed;
	employ / employed; enjoy / enjoyed;
	fray / frayed; gray / grayed; obey / obeyed;
	play / played; pray / prayed; prey / preyed;
	stay / stayed; stray / strayed; sway / swayed;
	toy / toyed
				 */
				pattern = @"\b.+[aeiou]{1}y{1}\b";
				regEx = new Regex(pattern);
				if (regEx.IsMatch(verb))
				{
					VerbForPattern = verb + "ed";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
				}
				#endregion

				#region Case5
				/* Double the final consonant and add -ed if there is a single stressed vowel before the final consonant.
	ban / banned; can / canned; hem / hemmed;
	mop / mopped; pin / pinned; sip / sipped;
	trap / trapped; wad / wadded; whip / whipped;
	compél / compélled; confér / conférred;
	prefér / preférred; refér / reférred */
				pattern = @"\b[^aeiou]+[aeiou]{1}[^aeiou]{1}\b";
				regEx = new Regex(pattern);
				if (regEx.IsMatch(verb))
				{
					string consonant = Strings.Right(verb, 1);
					VerbForPattern = verb + consonant + "ed";
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;

					pattern = @"\b(com|con|pre|re){1}(pel|fer){1}\b";
					regEx = new Regex(pattern);
					if (regEx.IsMatch(verb))
					{
						consonant = Strings.Right(verb, 1);
						VerbForPattern = verb + consonant + "ed";
						exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
						if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
					}
				}
				#endregion

				#region Case6
				/* Add -ed to the base forms of all other
	regular verbs. */
				VerbForPattern = verb + "ed";
				pattern = VerbForPattern;
				regEx = new Regex(pattern, RegexOptions.IgnoreCase);
				if (regEx.IsMatch(example))
				{
					exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
					if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
				}
				#endregion
			}
			#endregion

			#region IrregularVerb Past Tense & Part Particple

			Dictionary<string, string[]> irregularVerbList;
			if (CheckVerbIfIrregular(verb, out irregularVerbList))
			{
				//try past tense first
				foreach (var key in irregularVerbList.Keys)
				{
					if (verb == key)
					{
						string pastTense = irregularVerbList[key][0];
						VerbForPattern = pastTense;
						exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
						if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
					}
				}

				//try past participle
				foreach (var key in irregularVerbList.Keys)
				{
					if (verb == key)
					{
						string pastParticiple = irregularVerbList[key][1];
						VerbForPattern = pastParticiple;
						exampleWithFormattedVerb = FormatExampleWithWord(example, VerbForPattern, isWord);
						if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;
					}
				}
			}
			#endregion

			#region Just Simple Verb
			// simple base form (no 's' 'ed' 'ing')
			//string pattern = "(" + word + "|" + TextTools.UpperFirst(word) + ")";
			exampleWithFormattedVerb = FormatExampleWithWord(example, verb, isWord);
			if (exampleWithFormattedVerb != null) return exampleWithFormattedVerb;

			//string msg = string.Format("{0} cannout be found in the example", verb);
			//throw new Exception(msg);
			return null;

			#endregion
		}

		private static string[] FormatExampleWithWord(string example, string word, bool isWord)
		{
			string pattern = "(" + word + "|" + TextTools.UpperFirst(word) + ")";
			Regex regex = new Regex(pattern);
			if (!regex.IsMatch(example)) return null;
			string replacement = isWord ? @"<span class=""word"">$1</span>" : @"<span class=""colWord"">$1</span>";
			string formattedWord = replacement.Replace("$1", word);
			string formattedExample = regex.Replace(example, formattedWord);
			return new[] { formattedExample, formattedWord };
			//return regex.Replace(example, replacement);
		}

		private static bool CheckVerbIfRegular(string verb)
		{
			List<string> regularVerbList;
			List<string> regularVerbList1;

			Verb v = new Verb();
			if (HttpContext.Current.Cache[RegularVerbListCacheName] == null)
			{
				regularVerbList = v.GetRegularVerbList();
				CacheDependency dep = new CacheDependency(HttpContext.Current.Server.MapPath(SiteConfiguration.RegularVerbsXMLFile));
				HttpContext.Current.Cache.Insert(RegularVerbListCacheName, regularVerbList, dep,
												 DateTime.Now.AddMinutes(SiteConfiguration.CacheExpiration_Minutes),
												 TimeSpan.Zero, CacheItemPriority.High, null);
			}
			else
				regularVerbList = (List<string>)HttpContext.Current.Cache[RegularVerbListCacheName];

			if (HttpContext.Current.Cache[RegularVerbListCacheName1] == null)
			{
				regularVerbList1 = v.GetRegularVerbList1();
				CacheDependency dep = new CacheDependency(HttpContext.Current.Server.MapPath(SiteConfiguration.RegularVerbsXMLFile1));
				HttpContext.Current.Cache.Insert(RegularVerbListCacheName1, regularVerbList1, dep,
												 DateTime.Now.AddMinutes(SiteConfiguration.CacheExpiration_Minutes),
												 TimeSpan.Zero, CacheItemPriority.High, null);
			}
			else
				regularVerbList1 = (List<string>)HttpContext.Current.Cache[RegularVerbListCacheName1];

			bool isRegular = false;
			foreach (string vb in regularVerbList)
			{
				if (vb.ToLower().Contains(verb))
				{
					isRegular = true;
					break;
				}
			}
			foreach (string vb in regularVerbList1)
			{
				if (vb.ToLower().Contains(verb))
				{
					isRegular = true;
					break;
				}
			}
			//return (regularVerbList.Any(vb => vb.ToLower() == verb || vb.ToLower().Contains(verb)));
			return isRegular;
		}

		private static bool CheckVerbIfIrregular(string verb, out Dictionary<string, string[]> irregularVerbList)
		{
			//Dictionary<string, string[]> irregularVerbList;
			Verb v = new Verb();
			//Dictionary<string, string[]> irregularVerbList;
			if (HttpContext.Current.Cache[IrregularVerbListCacheName] == null)
			{
				irregularVerbList = v.GetIrregluarVerbList();
				CacheDependency dep = new CacheDependency(HttpContext.Current.Server.MapPath(SiteConfiguration.IrregularVerbsXMLFile));
				HttpContext.Current.Cache.Insert(IrregularVerbListCacheName, irregularVerbList, dep,
												 DateTime.Now.AddMinutes(SiteConfiguration.CacheExpiration_Minutes),
												 TimeSpan.Zero, CacheItemPriority.High, null);
			}
			else irregularVerbList = (Dictionary<string, string[]>)HttpContext.Current.Cache[IrregularVerbListCacheName];

			return (irregularVerbList.ContainsKey(verb));
		}

		public static string GetSourceRemark(WcExample example)
		{
			var culturename = CultureHelper.GetCurrentCulture();
			string source = example.Source;
			if (!string.IsNullOrEmpty(source))
			{
				if (source.ToLower().Contains("ch"))
				{
					source = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>", SiteConfiguration.ChDictUrl, Resources.CH);
				}
				else if (source.ToLower().Contains("oxford"))
				{
					source = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>", SiteConfiguration.OxfordDictUrl, Resources.Oxford);
				}

				string remark;
				if (culturename.Contains("hans"))
					remark = example.RemarkZhs;
				else if (culturename.Contains("ja"))
					remark = example.RemarkJap;
				else
					remark = example.RemarkZht;

				if (string.IsNullOrEmpty(remark))
					return string.Format("({0}{1})", Resources.SourceText, source);
				return string.Format("({0}{1}{2} {3}{4} {5})", Resources.SourceText, source, ";",Resources.Remark, ":", remark);
			}
			source = "Web"; //default as Web for the source, so as to be more efficient when creating or editing
			return string.Format("({0}{1})", Resources.SourceText, source);
		}

		public static string GetFormattedExample(string example, Collocation collocation)
		{
			string word = collocation.Word;
			string pos = collocation.Pos;
			string colWord = collocation.ColWord;
			string colpos = collocation.ColPos;
			return FormatExampleForView(example, word, pos, colWord, colpos, collocation.CollocationPattern);
		}
	}
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Abstract
{
	public interface ICollocation
	{
		[Key]
		long Id { get; set; }
		string Word { get; set; }
		string ColWord { get; set; }
		string ColWordZhs { get; set; }
		string WordZht { get; set; }
		string WordZhs { get; set; }
		string ColWordZht { get; set; }
		string WordJap { get; set; }
		string ColWordJap { get; set; }
		string Pos { get; set; }
		string ColPos { get; set; }
		string PosZht { get; set; }
		string ColPosZht { get; set; }
		string PosJap { get; set; }
		string PosZhs { get; set; }
		string ColPosJap { get; set; }
		string ColPosZhs { get; set; }
		List<WcExample> WcExampleList { get; set; }
		CollocationPattern CollocationPattern { get; set; }
	}
}

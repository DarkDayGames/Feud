using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feud.Server.Data
{
	public class QuestionBoard
	{
		public string Name { get; set; }
		public string Question { get; set; }
		public List<QuestionAnswer> Answers { get; set; }
		public List<StrikeAnswer> Strikes { get; set; }
		public int TotalPoints => Answers.Where(x => x.AnswerVisible).Sum(x => x.Points);

		public string GuestUrl { get; set; }
	}

	public class QuestionAnswer
	{
		public int Number { get; set; }
		public string Text { get; set; }
		public int Points { get; set; }
		public bool AnswerAvailable => !string.IsNullOrEmpty(Text);
		public bool AnswerVisible { get; set; }
	}

	public class StrikeAnswer
	{
		public bool StrikeVisible { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feud.Server.Data
{
	public class QuestionBoard
	{
		public string Id { get; set;}
		public string Name { get; set; }
		public string Question { get; set; }
		public List<QuestionAnswer> Answers { get; set; }
		public List<StrikeAnswer> Strikes { get; set; }
		public int TotalPoints => Answers?.Where(x => x.AnswerVisible).Sum(x => x.Points) ?? 0;

		public int AnswerCount => Answers.Count(x => !string.IsNullOrEmpty(x.Text));

		public string GuestId { get; set; }
		public string GuestUrl => $"feud/guest/{GuestId}";

		public QuestionBoard()
		{
			Id = Guid.NewGuid().ToString();
			Answers = new List<QuestionAnswer>();
			Strikes = new List<StrikeAnswer>
			{
				new StrikeAnswer(),
				new StrikeAnswer(),
				new StrikeAnswer()
			};
			GuestId = Guid.NewGuid().ToString();
		}
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

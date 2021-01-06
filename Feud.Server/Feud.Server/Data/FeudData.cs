using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Feud.Server.Data
{

	public class FeudGame
	{
		public string BoardId { get; set; }

		public List<Player> Players { get; set; }

		public int MaxPlayers { get; set; } = 2;

		public int? IndexOfPlayerPlaying { get; set; }

		public List<int> BuzzedInPlayers { get; set; }

		public FeudGame()
		{
			Players = new List<Player>();
			BuzzedInPlayers = new List<int>();
		}
	}


	public class QuestionBoard
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Question { get; set; }
		public List<QuestionAnswer> Answers { get; set; }

		[JsonIgnore] public List<StrikeAnswer> Strikes { get; set; }

		[JsonIgnore] public int TotalPoints => Answers?.Where(x => x.AnswerVisible).Sum(x => x.Points) ?? 0;

		[JsonIgnore] public int AnswerCount => Answers.Count(x => !string.IsNullOrEmpty(x.Text));

		public string GuestId { get; set; }

		[JsonIgnore] public string GuestUrl => $"feud/gueststart/{GuestId}";

		public DateTime CreatedDate { get; set; }

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

		[JsonIgnore] public bool AnswerAvailable => !string.IsNullOrEmpty(Text);

		[JsonIgnore] public bool AnswerVisible { get; set; }
	}

	public class StrikeAnswer
	{
		public bool StrikeVisible { get; set; }
	}

	public class Player
	{
		public string Name { get; set; }
	}

	public enum BoardViewMode
	{
		Host,
		Player,
		Audience
	}
}

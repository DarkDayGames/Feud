using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feud.Server.Data;

namespace Feud.Server.Services
{
	public class BoardChangedEventArgs
	{
		public string BoardId { get; set; }
		public int ItemChangedNumber { get; set; }
	}

	public interface IFeudHostService
	{
		void AddBoard(QuestionBoard newBoard);
		List<QuestionBoard> GetBoardsForHost();

		QuestionBoard GetBoardForHost(string boardId);
		QuestionBoard GetBoardForGuest(string boardGuestId);

		void ToggleAnswer(string boardId, int number);
		void ToggleStrike(string boardId, int index);

		void Reset(string boardId);

		event EventHandler<BoardChangedEventArgs> AnswerToggled;
		event EventHandler<BoardChangedEventArgs> StrikeToggled;
		event EventHandler<BoardChangedEventArgs> BoardReset;
	}

	public class FeudHostService : IFeudHostService
	{
		public event EventHandler<BoardChangedEventArgs> AnswerToggled;
		public event EventHandler<BoardChangedEventArgs> StrikeToggled;
		public event EventHandler<BoardChangedEventArgs> BoardReset;

		public List<QuestionBoard> Boards { get; } = new List<QuestionBoard>();

		public void AddBoard(QuestionBoard newBoard)
		{
			Boards.Add(newBoard);
		}

		public List<QuestionBoard> GetBoardsForHost()
		{
			return Boards;
		}

		public QuestionBoard GetBoardForHost(string boardId)
		{
			return Boards.FirstOrDefault(x => x.Id == boardId);
		}

		public QuestionBoard GetBoardForGuest(string boardGuestId)
		{
			return Boards.FirstOrDefault(x => x.GuestId == boardGuestId);
		}

		public void ToggleAnswer(string boardId, int number)
		{
			AnswerToggled?.Invoke(this, new BoardChangedEventArgs{BoardId = boardId, ItemChangedNumber = number});
		}

		public void ToggleStrike(string boardId, int index)
		{
			StrikeToggled?.Invoke(this, new BoardChangedEventArgs { BoardId = boardId, ItemChangedNumber = index });
		}

		public void Reset(string boardId)
		{
			BoardReset?.Invoke(this, new BoardChangedEventArgs { BoardId = boardId});
		}
	}
}

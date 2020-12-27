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

		void DeleteBoard(string boardId);
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
			try
			{
				Boards.Add(newBoard);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public void DeleteBoard(string boardId)
		{
			try
			{
				var boardToDelete = GetBoardForHost(boardId);
				Boards.Remove(boardToDelete);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public List<QuestionBoard> GetBoardsForHost()
		{
			return Boards;
		}

		public QuestionBoard GetBoardForHost(string boardId)
		{
			try
			{
				return Boards.FirstOrDefault(x => x.Id == boardId);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public QuestionBoard GetBoardForGuest(string boardGuestId)
		{
			try 
			{ 
				return Boards.FirstOrDefault(x => x.GuestId == boardGuestId);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public void ToggleAnswer(string boardId, int number)
		{
			try
			{
				AnswerToggled?.Invoke(this, new BoardChangedEventArgs {BoardId = boardId, ItemChangedNumber = number});
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public void ToggleStrike(string boardId, int index)
		{
			try 
			{ 
				StrikeToggled?.Invoke(this, new BoardChangedEventArgs { BoardId = boardId, ItemChangedNumber = index });
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}

		public void Reset(string boardId)
		{
			try { 
				BoardReset?.Invoke(this, new BoardChangedEventArgs { BoardId = boardId});
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				throw;
			}
		}
	}
}

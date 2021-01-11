using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feud.Server.Data;
using Microsoft.AspNetCore.Components;

namespace Feud.Server.Services
{
	public static class BoardChangedEventActions
	{
		public const string AnswerToggled = "AnswerToggled";
		public const string StrikeToggled = "StrikeToggled";
		public const string BoardReset = "BoardReset";
		public const string StrikeBuzzOnly = "StrikeBuzzOnly";
	}

	public class BoardChangedEventArgs
	{
		public string BoardId { get; set; }
		public int ItemChangedNumber { get; set; }

		public bool NewValue { get; set; }
		public string Action { get; set; }
	}

	public interface IFeudHostService
	{
		Task LoadBoardsAsync();
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
		private readonly IBoardRepositoryService _boardRepoService;
		public event EventHandler<BoardChangedEventArgs> AnswerToggled;
		public event EventHandler<BoardChangedEventArgs> StrikeToggled;
		public event EventHandler<BoardChangedEventArgs> BoardReset;

		public List<QuestionBoard> Boards { get; } = new List<QuestionBoard>();
		
		public FeudHostService(
			IBoardRepositoryService boardRepoService)
		{
			_boardRepoService = boardRepoService;
		}

		public async Task LoadBoardsAsync()
		{
			var boards = await _boardRepoService.LoadBoardsAsync();

			foreach (var board in boards)
			{
				if (!Boards.Exists(x => x.Id == board.Id))
				{
					Boards.Add(board);
				}
			}
		}

		public void AddBoard(QuestionBoard newBoard)
		{
			Boards.Add(newBoard);
		}
		public void DeleteBoard(string boardId)
		{
			var boardToDelete = GetBoardForHost(boardId);
			Boards.Remove(boardToDelete);

			_boardRepoService.Delete(boardToDelete);
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
			var answer = GetBoardForHost(boardId).Answers.FirstOrDefault(x => x.Number == number);

			if (answer != null)
			{
				Program.Logger.Log(NLog.LogLevel.Debug,
					$"FeudHostService.ToggleAnswer - {boardId},{number} => {!answer.AnswerVisible}, {AnswerToggled?.GetInvocationList().Length ?? 0} calls to make.");

				answer.AnswerVisible = !answer.AnswerVisible;

				AnswerToggled?.Invoke(this, 
					new BoardChangedEventArgs
					{
						BoardId = boardId, 
						ItemChangedNumber = number,
						Action = BoardChangedEventActions.AnswerToggled,
						NewValue = answer.AnswerVisible
					});
			}
		}

		public void ToggleStrike(string boardId, int index)
		{
			if (index >= 0)
			{
				var strikeToToggle = GetBoardForHost(boardId).Strikes[index];

				Program.Logger.Log(NLog.LogLevel.Debug,
					$"FeudHostService.ToggleStrike - {boardId},{index} => {!strikeToToggle.StrikeVisible}, {StrikeToggled?.GetInvocationList().Length ?? 0} calls to make.");

				strikeToToggle.StrikeVisible = !strikeToToggle.StrikeVisible;

				StrikeToggled?.Invoke(this,
					new BoardChangedEventArgs
					{
						BoardId = boardId,
						ItemChangedNumber = index,
						Action = BoardChangedEventActions.StrikeToggled,
						NewValue = strikeToToggle.StrikeVisible
					});
			}
			else
			{
				Program.Logger.Log(NLog.LogLevel.Debug,
					$"FeudHostService.ToggleStrike - {boardId},{index} => buzz only, {StrikeToggled?.GetInvocationList().Length ?? 0} calls to make.");

				StrikeToggled?.Invoke(this,
					new BoardChangedEventArgs
					{
						BoardId = boardId,
						ItemChangedNumber = index,
						Action = BoardChangedEventActions.StrikeBuzzOnly
					});
			}
		}

		public void Reset(string boardId)
		{
			var board = GetBoardForHost(boardId);

			foreach (var answer in board.Answers)
			{
				if (answer.AnswerAvailable)
				{
					answer.AnswerVisible = false;
				}
			}

			foreach (var strike in board.Strikes)
			{
				strike.StrikeVisible = false;
			}

			Program.Logger.Log(NLog.LogLevel.Debug,
				$"FeudHostService.Reset - {boardId}, {BoardReset?.GetInvocationList().Length ?? 0} calls to make.");

			BoardReset?.Invoke(this, 
				new BoardChangedEventArgs
				{
					BoardId = boardId,
					Action = BoardChangedEventActions.BoardReset
				});
		}
	}
}

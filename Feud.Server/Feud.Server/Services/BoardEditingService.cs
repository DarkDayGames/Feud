using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Feud.Server.Data;
using Feud.Server.ViewModels;

namespace Feud.Server.Services
{

	public class BoardSavingEventArgs
	{
		public QuestionBoard SavedBoard { get; set; }

		public bool RunBoardAfterSaving { get; set; }
	}

	public interface IBoardEditingService
	{
		Task SaveBoard(EditBoardViewModel boardToSave, bool runBoardAfterSaving);

		void Cancel();

		event EventHandler<BoardSavingEventArgs> BoardChangesSaving;
		event EventHandler BoardChangesCancelled;
	}

	public class BoardEditingService : IBoardEditingService
	{
		private readonly IFeudHostService _feudHostService;
		private readonly IBoardRepositoryService _boardRepoService;
		private readonly IDateTimeService _dateTimeService;

		public BoardEditingService(
			IFeudHostService feudHostService,
			IBoardRepositoryService boardRepoService,
			IDateTimeService dateTimeService)
		{
			_feudHostService = feudHostService;
			_boardRepoService = boardRepoService;
			_dateTimeService = dateTimeService;
		}

		public async Task SaveBoard(EditBoardViewModel boardToSave, bool runBoardAfterSaving)
		{

			var eventArgs = new BoardSavingEventArgs
			{
				RunBoardAfterSaving = runBoardAfterSaving
			};

			if (string.IsNullOrEmpty(boardToSave.Id))
			{
				eventArgs.SavedBoard = new QuestionBoard
				{
					Name = boardToSave.Name,
					Question = boardToSave.Question,
					Answers = boardToSave.Answers,
					CreatedDate = _dateTimeService.Now
				};

				await _boardRepoService.SaveAsync(eventArgs.SavedBoard);

				_feudHostService.AddBoard(eventArgs.SavedBoard);
			}
			else
			{
				eventArgs.SavedBoard = _feudHostService.GetBoardForHost(boardToSave.Id);

				eventArgs.SavedBoard.Name = boardToSave.Name;
				eventArgs.SavedBoard.Question = boardToSave.Question;
				eventArgs.SavedBoard.Answers = boardToSave.Answers;

				await _boardRepoService.SaveAsync(eventArgs.SavedBoard);
			}
			
			BoardChangesSaving?.Invoke(this, eventArgs);
		}

		public void Cancel()
		{
			BoardChangesCancelled?.Invoke(this, new EventArgs());
		}

		public event EventHandler<BoardSavingEventArgs> BoardChangesSaving;
		public event EventHandler BoardChangesCancelled;
	}
}

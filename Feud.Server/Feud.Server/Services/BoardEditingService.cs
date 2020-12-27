using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Feud.Server.ViewModels;

namespace Feud.Server.Services
{

	public class BoardSavingEventArgs
	{
		public EditBoardViewModel BoardToSave { get; set; }

		public bool RunBoardAfterSaving { get; set; }
	}

	public interface IBoardEditingService
	{
		void SaveBoard(EditBoardViewModel boardToSave, bool runBoardAfterSaving);

		void Cancel();

		event EventHandler<BoardSavingEventArgs> BoardChangesSaving;
		event EventHandler BoardChangesCancelled;
	}

	public class BoardEditingService : IBoardEditingService
	{
		public void SaveBoard(EditBoardViewModel boardToSave, bool runBoardAfterSaving)
		{
			BoardChangesSaving?.Invoke(this, new BoardSavingEventArgs
			{
				BoardToSave = boardToSave,
				RunBoardAfterSaving = runBoardAfterSaving
			});
		}

		public void Cancel()
		{
			BoardChangesCancelled?.Invoke(this, new EventArgs());
		}

		public event EventHandler<BoardSavingEventArgs> BoardChangesSaving;
		public event EventHandler BoardChangesCancelled;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Text.Encodings.Web;
using Blazored.Modal.Services;
using Feud.Server.Data;
using Feud.Server.Services;
using Feud.Server.Shared;
using Feud.Server.ViewModels;

namespace Feud.Server.Pages
{
	public partial class FeudList : ComponentBase, IDisposable
	{
		[Inject] public NavigationManager NavigationManager { get; set; }
		[Inject] public IFeudHostService FeudHostService { get; set; }
		[Inject] public IBoardEditingService BoardEditingService { get; set; }
		[Inject] public IDateTimeService DateTimeService { get; set; }

		public EditBoardViewModel BoardToEdit { get; set; }

		public List<QuestionBoard> Boards { get; set; }

		public string CurrentEditTab { get; set; }

		[CascadingParameter] public IModalService Modal { get; set; }

		protected override async Task OnInitializedAsync()
		{
			try
			{
				BoardToEdit = new EditBoardViewModel();

				await FeudHostService.LoadBoardsAsync();

				Boards = FeudHostService.GetBoardsForHost();

				Program.Logger.Log(NLog.LogLevel.Debug, $"FeudList.OnInitialized - adding event handlers");

				BoardEditingService.BoardChangesSaving += SaveBoard;
				BoardEditingService.BoardChangesCancelled += ResetBoardAfterEditing;
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}

		}

		public void Dispose()
		{
			Program.Logger.Log(NLog.LogLevel.Debug, $"FeudList.OnInitialized - removing event handlers");

			BoardEditingService.BoardChangesSaving -= SaveBoard;
			BoardEditingService.BoardChangesCancelled -= ResetBoardAfterEditing;
		}

		public void RunBoard(string boardId)
		{
			try
			{
				NavigationManager.NavigateTo($"feud/host/{boardId}");
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		public void EditBoard(string boardId)
		{
			try
			{
				var board = FeudHostService.GetBoardForHost(boardId);

				BoardToEdit = new EditBoardViewModel
				{
					Id = board.Id,
					Name = board.Name,
					Question = board.Question
				};

				for (var i = 0; i < board.Answers.Count; i++)
				{
					BoardToEdit.Answers[i].Text = board.Answers[i].Text;
					BoardToEdit.Answers[i].Points = board.Answers[i].Points;
				}

				CurrentEditTab = Shared.EditBoard.TabKeys.Edit;

				InvokeAsync(StateHasChanged);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		public async Task DeleteBoard(string boardId)
		{
			try
			{
				var confirmModal = Modal.Show<ConfirmBoardDeletion>("Delete Board?");
				var result = await confirmModal.Result;

				if (!result.Cancelled)
				{
					FeudHostService.DeleteBoard(boardId);

					await InvokeAsync(StateHasChanged);
				}
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}


		public void SaveBoard(object sender, BoardSavingEventArgs e)
		{
			try
			{
				Program.Logger.Log(NLog.LogLevel.Debug, $"FeudList.SaveBoard");

				if (e.RunBoardAfterSaving)
				{
					NavigationManager.NavigateTo($"feud/host/{e.SavedBoard.Id}");
				}
				else
				{
					ResetBoardAfterEditing(null, null);
				}
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}

		}

		public void ResetBoardAfterEditing(object sender, EventArgs e)
		{
			try
			{
				Program.Logger.Log(NLog.LogLevel.Debug, $"FeudList.ResetBoardAfterEditing");

				BoardToEdit = new EditBoardViewModel();

				CurrentEditTab = Shared.EditBoard.TabKeys.Import;

				InvokeAsync(StateHasChanged);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}
	}
}

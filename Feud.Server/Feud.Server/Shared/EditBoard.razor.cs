﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Feud.Server.Data;
using Feud.Server.Services;
using Feud.Server.ViewModels;
using Microsoft.AspNetCore.Components;


namespace Feud.Server.Shared
{
	public partial class EditBoard : ComponentBase
	{
		[Inject] public IFeudHostService FeudHostService { get; set; }
		[Inject] public NavigationManager NavigationManager { get; set; }
		[Inject] public IBoardEditingService BoardEditingService { get; set; }
		[Inject] public HtmlEncoder HtmlEncoder { get; set; }

		[Parameter]
		public EditBoardViewModel BoardToEdit { get; set; }

		[Parameter]
		public string CurrentTab { get; set; }

		protected override void OnInitialized()
		{
			try
			{
				if (string.IsNullOrEmpty(BoardToEdit.Id))
				{
					SelectImportTab();
				}
				else
				{
					SelectEditTab();
				}
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		public void SelectImportTab()
		{
			CurrentTab = TabKeys.Import;
		}

		public void SelectEditTab()
		{
			CurrentTab = TabKeys.Edit;
		}

		public static class TabKeys
		{
			public const string Import = "import";
			public const string Edit = "edit";
		}

		protected void ImportAnswers()
		{
			try
			{
				var answerLines = BoardToEdit.AnswerBlob.Split("\n");

				var answerNumber = 1;
				var newAnswers = new List<QuestionAnswer>();

				foreach (var answerLine in answerLines)
				{
					if (answerNumber > 10)
					{
						break;
					}

					if (!string.IsNullOrEmpty(answerLine))
					{
						var values = answerLine.Split(',');

						var answer = new QuestionAnswer
						{
							Number = answerNumber++,
							Text = values[0],
							Points = values.Length > 1 ? int.Parse(values[1]) : 0
						};

						newAnswers.Add(answer);
					}
				}

				for (var i = answerNumber; i <= 10; i++)
				{
					newAnswers.Add(new QuestionAnswer
					{
						Number = i
					});
				}

				BoardToEdit.Answers = newAnswers;

				SelectEditTab();
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		protected EditBoardViewModel SanitizeBoard(EditBoardViewModel boardToSave)
		{
			boardToSave.Name = Sanitize(boardToSave.Name, 50);
			boardToSave.Question = Sanitize(boardToSave.Question, 200);

			foreach (var answer in boardToSave.Answers)
			{
				if (string.IsNullOrEmpty(answer.Text))
				{
					answer.Points = 0;
				}
				else
				{
					answer.Text = Sanitize(answer.Text, 200);
				}
			}

			return boardToSave;
		}

		private string Sanitize(string value, int maxLength)
		{
			if (!string.IsNullOrEmpty(value))
			{
				if (value.Contains('<') || value.Contains('>'))
				{
					value = HtmlEncoder.Encode(value);
				}

				if (value.Length > maxLength)
				{
					value = value.Substring(0, maxLength);
				}
			}

			return value;
		}


		protected void CreateAndRun()
		{
			try
			{
				BoardEditingService.SaveBoard(SanitizeBoard(BoardToEdit), true);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		protected void Save()
		{
			try
			{
				BoardEditingService.SaveBoard(SanitizeBoard(BoardToEdit), false);
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}

		protected void Cancel()
		{
			try
			{
				BoardEditingService.Cancel();
			}
			catch (Exception ex)
			{
				Program.Logger.Log(NLog.LogLevel.Error, ex);
				NavigationManager.NavigateTo("error");
			}
		}
	}
}

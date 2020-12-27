using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feud.Server.Data;

namespace Feud.Server.ViewModels
{
	public class EditBoardViewModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Question { get; set; }
		public string AnswerBlob { get; set; }

		public List<QuestionAnswer> Answers { get; set; }

		public EditBoardViewModel()
		{
			Answers = new List<QuestionAnswer>();
			for (var i = 1; i <= 10; i++)
			{
				Answers.Add(new QuestionAnswer { Number = i });
			}
		}
	}
}

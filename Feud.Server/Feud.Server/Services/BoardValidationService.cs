using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feud.Server.Data;
using Feud.Server.ViewModels;

namespace Feud.Server.Services
{
	public interface IBoardValidationService
	{
		List<string> Validate(EditBoardViewModel board);
	}

	public class BoardValidationService : IBoardValidationService
	{
		public List<string> Validate(EditBoardViewModel board)
		{
			var failures = new List<string>();

			if (string.IsNullOrWhiteSpace(board.Name))
			{
				failures.Add("Name is required.");
			}

			if (string.IsNullOrWhiteSpace(board.Question))
			{
				failures.Add("Question is required.");
			}

			if (board.Answers == null 
			    || !board.Answers.Any()
			    || board.Answers.All(x => string.IsNullOrWhiteSpace(x.Text)))
			{
				failures.Add("At least one Answer is required");
			}

			return failures;
		}
	}
}

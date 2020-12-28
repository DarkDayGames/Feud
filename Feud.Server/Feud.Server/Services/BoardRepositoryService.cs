using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feud.Server.Data;

namespace Feud.Server.Services
{

	public interface IBoardRepositoryService
	{
		Task<List<QuestionBoard>> LoadBoardsAsync();

		Task SaveAsync(QuestionBoard board);

		void Delete(QuestionBoard board);
	}

	public class BoardRepositoryService : IBoardRepositoryService
	{
		private const string SaveSubFolder = "savedboards";

		private string BuildBoardFileName(QuestionBoard board)
		{
			return $"Feud.{board.Id}.{board.CreatedDate:yyyyMMddHHmmss}.json";
		}

		private string BuildBoardFilePath(QuestionBoard board)
		{
			return Path.Combine(SaveFolder, BuildBoardFileName(board));
		}

		private string SaveFolder
		{
			get
			{
				var saveFolder = Path.Combine(Directory.GetCurrentDirectory(), SaveSubFolder);

				if (!Directory.Exists(saveFolder))
				{
					Directory.CreateDirectory(saveFolder);
				}

				return saveFolder;
			}
		}

		public async Task<List<QuestionBoard>> LoadBoardsAsync()
		{
			var boards = new List<QuestionBoard>();

			var fileList = Directory.GetFiles(SaveFolder);

			foreach (var file in fileList)
			{
				var json = await File.ReadAllTextAsync(file);

				var board = System.Text.Json.JsonSerializer.Deserialize<QuestionBoard>(json);

				boards.Add(board);
			}

			boards = boards.OrderBy(x => x.Name).ThenBy(x => x.CreatedDate).ToList();

			return boards;
		}

		public async Task SaveAsync(QuestionBoard board)
		{
			var json = System.Text.Json.JsonSerializer.Serialize<QuestionBoard>(board);

			var file = BuildBoardFilePath(board);

			await File.WriteAllTextAsync(BuildBoardFilePath(board),json, Encoding.UTF8);
		}

		public void Delete(QuestionBoard board)
		{
			File.Delete(BuildBoardFilePath(board));
		}
	}
}

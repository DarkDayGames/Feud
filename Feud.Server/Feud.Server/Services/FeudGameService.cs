using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feud.Server.Data;

namespace Feud.Server.Services
{
	public static class GameChangedEventActions
	{
		public const string PlayerAdded = "PlayerAdded";
		public const string PlayerBuzzedIn = "PlayerBuzzedIn";
		public const string BuzzerReset = "BuzzerReset";
		public const string PlayerChosenToPlay = "PlayerChosenToPlay";
	}

	public class GameChangedEventArgs
	{
		public string BoardId { get; set; }
		public string Action { get; set; }
		public int PlayerIndex { get; set; }

		public GameChangedEventArgs()
		{
			PlayerIndex = -1;
		}
	}

	public interface IFeudGameService
	{
		FeudGame LoadGame(string boardId);
		string LoginPlayer(string guestId, string playerName);

		void BuzzIn(string guestId, string playerName);

		void ResetBuzzer(string boardId);

		void ChoosingToPlay(string boardId, string playerName);

		event EventHandler<GameChangedEventArgs> PlayerAdded;
		event EventHandler<GameChangedEventArgs> PlayerBuzzedIn;
		event EventHandler<GameChangedEventArgs> BuzzerReset;
		event EventHandler<GameChangedEventArgs> PlayerChosenToPlay;
	}

	public class FeudGameService : IFeudGameService
	{
		private static Object _loginLock = new Object();
		private static Object _buzzInLock = new Object();


		private readonly IFeudHostService _hostService;

		public event EventHandler<GameChangedEventArgs> PlayerAdded;
		public event EventHandler<GameChangedEventArgs> PlayerBuzzedIn;
		public event EventHandler<GameChangedEventArgs> BuzzerReset;
		public event EventHandler<GameChangedEventArgs> PlayerChosenToPlay;

		public List<FeudGame> Games { get; } = new List<FeudGame>();

		public FeudGameService(
			IFeudHostService hostService)
		{
			_hostService = hostService;
		}


		public FeudGame LoadGame(string boardId)
		{
			var game = Games.FirstOrDefault(x => x.BoardId == boardId);

			if (game == null)
			{
				var board = _hostService.GetBoardForHost(boardId);

				game = new FeudGame
				{
					BoardId = board.Id
				};

				Games.Add(game);
			}

			return game;
		}

		public string LoginPlayer(string guestId, string playerName)
		{
			var board = _hostService.GetBoardForGuest(guestId);

			var game = Games.FirstOrDefault(x => x.BoardId == board.Id);

			if (game == null)
			{
				return "Invalid Room Code";
			}
			else if (string.IsNullOrEmpty(playerName))
			{
				return "Name is required.";
			}
			else
			{
				lock (_loginLock)
				{
					var player = game.Players.FirstOrDefault(x => x.Name == playerName);

					if (player != null)
					{
						return "Name is already taken.";
					}
					else if (game.Players.Count < game.MaxPlayers)
					{
						game.Players.Add(new Player {Name = playerName});

						PlayerAdded?.Invoke(this, new GameChangedEventArgs
						{
							BoardId = board.Id,
							Action = GameChangedEventActions.PlayerAdded,
							PlayerIndex = game.Players.Count - 1
						});

					}
				}

				return null;
			}
		}

		public void BuzzIn(string guestId, string playerName)
		{
			var board = _hostService.GetBoardForGuest(guestId);

			var game = Games.FirstOrDefault(x => x.BoardId == board.Id);

			if (game != null
			    && !string.IsNullOrEmpty(playerName))
			{
				lock (_buzzInLock)
				{
					if (game.BuzzedInPlayers.Count == 0)
					{
						var player = game.Players.FirstOrDefault(x => x.Name == playerName);

						if (player != null)
						{
							var indexOfBuzzingPlayer = game.Players.IndexOf(player);

							game.BuzzedInPlayers.Add(indexOfBuzzingPlayer);

							PlayerBuzzedIn?.Invoke(this, new GameChangedEventArgs
							{
								BoardId = board.Id,
								Action = GameChangedEventActions.PlayerBuzzedIn,
								PlayerIndex = indexOfBuzzingPlayer
							});

						}
					}
				}
			}
		}

		public void ResetBuzzer(string boardId)
		{
			var board = _hostService.GetBoardForHost(boardId);

			var game = Games.FirstOrDefault(x => x.BoardId == board.Id);

			if (game != null)
			{
				game.IndexOfPlayerPlaying = null;
				game.BuzzedInPlayers.Clear();

				BuzzerReset?.Invoke(this, new GameChangedEventArgs
				{
					BoardId = boardId,
					Action = GameChangedEventActions.BuzzerReset
				});
			}
		}

		public void ChoosingToPlay(string boardId, string playerName)
		{
			var board = _hostService.GetBoardForHost(boardId);

			var game = Games.FirstOrDefault(x => x.BoardId == board.Id);

			if (game != null
			    && !string.IsNullOrEmpty(playerName))
			{
				if (!game.IndexOfPlayerPlaying.HasValue)
				{
					var player = game.Players.FirstOrDefault(x => x.Name == playerName);

					if (player != null)
					{
						game.IndexOfPlayerPlaying = game.Players.IndexOf(player);

						PlayerChosenToPlay?.Invoke(this, new GameChangedEventArgs
						{
							BoardId = board.Id,
							Action = GameChangedEventActions.PlayerChosenToPlay,
							PlayerIndex = game.IndexOfPlayerPlaying.Value
						});

					}
				}
			
			}
		}

	}
}

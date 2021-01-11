using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Feud.Server.Services
{
	public interface ICurrentPlayerNameService
	{
		string Name { get; set; }
	}

	public class CurrentPlayerNameService : ICurrentPlayerNameService
	{
		private Guid Id { get; set; } = Guid.NewGuid();
		
		private string _name;
		public string Name
		{
			get
			{
				Program.Logger.Log(NLog.LogLevel.Debug, $"CURRENT NAME GET: {Id}-{_name}");

				return _name;
			}
			set
			{
				_name = value;

				Program.Logger.Log(NLog.LogLevel.Debug, $"CURRENT NAME SET: {Id}-{_name}");
			}
		}
	}
}

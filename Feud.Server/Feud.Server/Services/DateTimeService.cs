using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feud.Server.Services
{
	public interface IDateTimeService
	{
		DateTime Now { get; }
	}
	public class DateTimeService : IDateTimeService
	{
		public DateTime Now => DateTime.Now;
	}
}

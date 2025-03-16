using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB98.Modules.Tickets.Application.SeatManagement.UnlockSeat
{
	public record UnlockSeatsCommand(UnlockSeatsDto Model) : ICommand<ApiResult<object>>;
}

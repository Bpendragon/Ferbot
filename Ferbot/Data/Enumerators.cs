using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferbot.Data
{
	enum AddSuccess
	{
		Success,
		AlreadyExists,
		WriteFailure
	}

	enum RemoveSuccess
	{
		Success,
		NoSuchAlias,
		WriteFailure
	}
}

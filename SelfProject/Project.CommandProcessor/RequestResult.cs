using System.Collections.Generic;
using System.Linq;

namespace Project.CommandProcessor
{
	public abstract class RequestResult
	{
	    protected RequestResult()
	    {
	        Errors = new List<string>();
	    }

		public bool Successful { get { return !Errors.Any(); } }
		public IList<string> Errors { get; set; }
	}
}
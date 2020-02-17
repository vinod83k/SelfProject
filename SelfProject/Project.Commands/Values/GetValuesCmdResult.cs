using Project.CommandProcessor;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectWeb.Commands.Values
{
    public class GetValuesCmdResult: RequestResult
    {
        public IEnumerable<string> Values { get; set; }
    }
}

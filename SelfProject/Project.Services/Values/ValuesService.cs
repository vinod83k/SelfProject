using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Services.Values
{
    public class ValuesService : IValuesService
    {
        public IEnumerable<string> Get()
        {
            return new List<string> { "Value1", "Value2" };
        }
    }
}

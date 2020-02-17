using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Services.Values
{
    public interface IValuesService
    {
        IEnumerable<string> Get();
    }
}

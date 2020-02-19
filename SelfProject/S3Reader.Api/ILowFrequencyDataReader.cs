using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace S3Reader.Core
{
    interface ILowFrequencyDataReader
    {
        Task GetObject();
    }
}

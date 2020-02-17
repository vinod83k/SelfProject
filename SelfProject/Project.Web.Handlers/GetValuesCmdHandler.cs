using Project.CommandProcessor;
using Project.Services.Values;
using ProjectWeb.Commands.Values;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Web.Handlers
{
    public class GetValuesCmdHandler : IRequestHandler<GetValuesCmdParams, GetValuesCmdResult>
    {
        private readonly IValuesService _valuesService;
        
        public GetValuesCmdHandler(IValuesService valuesService)
        {
            _valuesService = valuesService;
        }

        public GetValuesCmdResult Handle(GetValuesCmdParams cmdParms)
        {
            return new GetValuesCmdResult
            {
                Values = _valuesService.Get()
            };
        }
    }
}

using Project.CommandProcessor;
using ProjectWeb.Commands.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Web.Handlers.Users
{
    public class GetUsersCmdHandler : IRequestHandler<GetUsersCmdParams, GetUsersCmdResult>
    {
        public GetUsersCmdResult Handle(GetUsersCmdParams cmdParms)
        {
            return new GetUsersCmdResult();
        }
    }
}

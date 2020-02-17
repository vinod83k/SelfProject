using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Project.CommandProcessor;
using ProjectWeb.Commands.Values;

namespace Project.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Project.CommandProcessor.IRequestProcessor _processor;

        public string ServiceResponse { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            Project.CommandProcessor.IRequestProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }

        public void OnGet()
        {
            var query = new GetValuesCmdParams();
            var result = _processor.Process<GetValuesCmdParams, GetValuesCmdResult>(query);

            StringBuilder sb = new StringBuilder();
            foreach (var output in result.Values) {
                sb.Append($"{output}\r\n");
            }

            ServiceResponse = sb.ToString();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Cringe.Types;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cringe.Web.Pages.Matches
{
    public class IndexModel : PageModel
    {
        private readonly BanchoApiWrapper _banchoApiWrapper;

        public IndexModel(BanchoApiWrapper banchoApiWrapper)
        {
            _banchoApiWrapper = banchoApiWrapper;
        }

        public IList<MatchModel> Matches { get; set; }

        public async Task OnGetAsync()
        {
            Matches = await _banchoApiWrapper.GetActiveMatches();
        }
    }
}

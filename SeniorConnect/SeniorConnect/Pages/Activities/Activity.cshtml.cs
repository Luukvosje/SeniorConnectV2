using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SeniorConnect.DAL;
using Seniorconnect_Luuk_deVos.Model;

namespace SeniorConnect.Pages.Activity
{
    public class ActivityModel : PageModel
    {
        private ActivityLogic logic { get; set; }
        private readonly IConfiguration _configuration;

        public List<User> Users { get; set; }

        public ActivityModel(IConfiguration configuration, ActivityLogic Logic)
        {
            _configuration = configuration;
            logic = Logic;
        }

        public void OnGet(int activityId)
        {
            if (activityId != 0)
            {
                Users = logic.GetUsers(activityId);
            }
        }
    }
}

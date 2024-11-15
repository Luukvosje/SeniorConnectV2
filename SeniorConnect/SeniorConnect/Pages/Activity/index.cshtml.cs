using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using SeniorConnect.DAL;
using SeniorConnect.Model;
using Seniorconnect_Luuk_deVos.DAL;
using Seniorconnect_Luuk_deVos.Model;
using System.Security.Claims;

namespace Seniorconnect_Luuk_deVos.Pages
{
    [Authorize]
    public class ActivityPage : PageModel
	{
        private ActivityLogic logic { get; set; }
		private readonly IConfiguration _configuration;

		public List<ActivityModel> currentActivities { get; set; }
        public List<ActivityModel> joinedActivities = new List<ActivityModel>();
        public static User user;
        public bool ShowJoinedOnly { get; private set; } = false;

        public ActivityPage(IConfiguration configuration, ActivityLogic Logic)
		{
            _configuration = configuration;
            logic = Logic;

        }

        public void OnGet()
		{
            LoadUserData();
            ShowJoinedOnly = HttpContext.Session.GetInt32("ShowJoinedOnly") == 1;
            LoadActivities();

        }
        private void LoadUserData()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;

                if (!string.IsNullOrEmpty(userId) && int.TryParse(userId, out var id))
                {
                    user = new User
                    {
                        name = name ?? "Unknown",
                        email = email ?? "",
                        userId = id
                    };
                }
            }

        }

        public IActionResult OnPostToggle()
        {
            ShowJoinedOnly = !(HttpContext.Session.GetInt32("ShowJoinedOnly") == 1);
            HttpContext.Session.SetInt32("ShowJoinedOnly", ShowJoinedOnly ? 1 : 0);
            LoadActivities();
            Console.WriteLine(ShowJoinedOnly + " show joined page");

            return Page();
        }
        public IActionResult OnPostJoinActivity(int id)
        {
            var result = logic.JoinActivity(user.userId, id);   
            return RedirectToPage();
        }
        public IActionResult OnPostLeaveActivity(int id)
        {
            var result = logic.LeaveActivity(user.userId, id);
            return RedirectToPage();
        }

        private void LoadActivities()
        {
            if (ShowJoinedOnly)
            {
                Console.WriteLine(user);
                joinedActivities = logic.GetJoinedActivities(user.userId);
                currentActivities = joinedActivities;
            }
            else
            {
                currentActivities = CheckIfJoined();
            }
        }
        private List<ActivityModel> CheckIfJoined()
        {
            List<ActivityModel> allActivities = logic.GetActivities();
            joinedActivities = logic.GetJoinedActivities(user.userId);

            foreach (var activity in allActivities)
            {
                activity.joined = joinedActivities.Any(joined => joined.ID == activity.ID);
            }

            return allActivities;
        }
    }

}


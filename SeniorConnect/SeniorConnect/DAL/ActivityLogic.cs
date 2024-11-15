using MySql.Data.MySqlClient;
using SeniorConnect.Model;
using Seniorconnect_Luuk_deVos.DAL;
using Seniorconnect_Luuk_deVos.Model;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace SeniorConnect.DAL
{
    public class ActivityLogic : BaseLogic
    {
        public ActivityLogic(MySqlDbContext dbContext, IConfiguration configuration) : base(dbContext, configuration)
        {
        }

        public List<ActivityModel> GetActivities()
        {

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<ActivityModel> activities = new List<ActivityModel>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM activities";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var activity = new ActivityModel
                            {
                                ID = reader.GetInt32("ID"),
                                Date = reader.GetDateTime("Date"),
                                Title = reader.GetString("Title"),
                                Description = reader.GetString("Description"),
                                MaxUsers = reader.GetInt32("MaxUsers"),
                                EndTime = reader.GetDateTime("EndTime"),
                                Price = reader.GetInt32("Price"),
                                StartTime = reader.GetDateTime("StartTime")
                            };
                            activities.Add(activity);
                        }
                    }
                }
                return activities;
            }
        }
        public bool JoinActivity(int userId, int acitivtyId)
        {
            if (userId == null | acitivtyId == null)
                return false;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
           
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO chosenactivities (UserId, ActivityId) VALUES (@userid, @activityid)", conn);

                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@activityid", acitivtyId);
                int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;

            }
        }

        public bool LeaveActivity(int userId, int activityId)
        {
            if (userId == 0 || activityId == 0)
                return false;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM chosenactivities WHERE UserId = @userid AND ActivityId = @activityid", conn);

                cmd.Parameters.AddWithValue("@userid", userId);
                cmd.Parameters.AddWithValue("@activityid", activityId);
                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public List<ActivityModel> GetJoinedActivities(int userId)
        {
            if (userId == 0)
                return new List<ActivityModel>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT * FROM chosenactivities ca JOIN activities a ON ca.ActivityId = a.ID WHERE ca.UserId = @userid", conn);

                cmd.Parameters.AddWithValue("@userid", userId);

                var activities = new List<ActivityModel>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var activity = new ActivityModel
                        {
                            ID = reader.GetInt32("ActivityId"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            StartTime = reader.GetDateTime("StartTime"),
                            Price = reader.GetDouble("Price"),
                            joined = true,
                        };
                        activities.Add(activity);
                    }
                }

                return activities;
            }
        }

    }
}

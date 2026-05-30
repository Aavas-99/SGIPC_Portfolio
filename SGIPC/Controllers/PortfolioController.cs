using System.Web.Mvc;
using SGIPC.Models;
using System.Data.SqlClient;
using System;
using System.Web.Security;
using System.Web.Mvc.Filters;

namespace SGIPC.Controllers
{
    public class PortfolioController : Controller
    {
        /// <summary>
        /// Restore user email to session from FormsAuthentication ticket on every request
        /// This ensures email persists across sessions when "Remember Me" is checked
        /// </summary>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            
            // If user is authenticated via FormsAuthentication but email not in session, restore it
            if (User.Identity.IsAuthenticated)
            {
                if (Session["UserEmail"] == null && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    Session["UserEmail"] = User.Identity.Name;
                }
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Committee()
        {
            return View();
        }

        public ActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signin(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IsAdmin)
                {
                    if (string.Equals(model.Email, "admin@sgipc.kuet", StringComparison.OrdinalIgnoreCase)
                        && model.Password == "admin1234")
                    {
                        FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                        Session["UserId"] = "admin";
                        Session["UserEmail"] = model.Email;
                        Session["UserRole"] = "admin";
                        EnsureAdminTables();
                        return RedirectToAction("AdminDashboard");
                    }

                    ModelState.AddModelError("", "Invalid admin email or password.");
                    return View(model);
                }

                try
                {
                    using (SqlConnection conn = DbHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "SELECT * FROM dbo.Users WHERE Email = @Email";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Email", model.Email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["Password"].ToString();
                                
                                // Check password (use BCrypt or other hashing in production)
                                if (BCrypt.Net.BCrypt.Verify(model.Password, storedPassword))
                                {
                                    // Set authentication cookie (persistent if RememberMe is checked)
                                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                                    // Also set session variables for current session
                                    Session["UserId"] = reader["Id"].ToString();
                                    Session["UserEmail"] = model.Email;
                                    Session["UserRole"] = reader["Role"]?.ToString() ?? "user";

                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid email or password.");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid email or password.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(model);
        }

        private bool IsAdminUser()
        {
            return Session["UserRole"]?.ToString() == "admin";
        }

        private void EnsureAdminTables()
        {
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string ensureAnnouncements = @"IF OBJECT_ID('dbo.Announcements', 'U') IS NULL 
BEGIN
    CREATE TABLE dbo.Announcements
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Content NVARCHAR(MAX) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
    )
END";

                string ensureResources = @"IF OBJECT_ID('dbo.Resources', 'U') IS NULL 
BEGIN
    CREATE TABLE dbo.Resources
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        FileName NVARCHAR(255) NOT NULL,
        MediaType NVARCHAR(200) NOT NULL,
        MediaData VARBINARY(MAX) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT(GETDATE())
    )
END";

                new SqlCommand(ensureAnnouncements, conn).ExecuteNonQuery();
                new SqlCommand(ensureResources, conn).ExecuteNonQuery();
            }
        }

        public ActionResult AdminDashboard()
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            EnsureAdminTables();

            var model = new AdminDashboardViewModel();

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();

                string usersQuery = "SELECT Id, Email, CreatedAt FROM dbo.Users ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(usersQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.RegisteredUsers.Add(new RegisteredUserViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Email = reader["Email"].ToString(),
                            UserName = reader["Email"].ToString().Split('@')[0],
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }

                string membersQuery = @"SELECT Id, FullName, Email, RollNumber, Department, Batch, CodeForcesHandle, AtCoderHandle, CodeChefHandle, LeetCodeHandle, VJudgeHandle, ReasonForJoin, Status, SubmittedAt
FROM dbo.ApplicationForm ORDER BY SubmittedAt DESC";
                using (SqlCommand cmd = new SqlCommand(membersQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var member = new MemberInfoViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["FullName"].ToString(),
                            Email = reader["Email"].ToString(),
                            RollNumber = reader["RollNumber"].ToString(),
                            Department = reader["Department"].ToString(),
                            Batch = reader["Batch"].ToString(),
                            CodeForcesHandle = reader["CodeForcesHandle"].ToString(),
                            AtCoderHandle = reader["AtCoderHandle"].ToString(),
                            CodeChefHandle = reader["CodeChefHandle"].ToString(),
                            LeetCodeHandle = reader["LeetCodeHandle"].ToString(),
                            VJudgeHandle = reader["VJudgeHandle"].ToString(),
                            ReasonForJoin = reader["ReasonForJoin"].ToString(),
                            Status = reader["Status"].ToString(),
                            SubmittedAt = Convert.ToDateTime(reader["SubmittedAt"])
                        };

                        if (string.Equals(member.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                        {
                            model.ApprovedMembers.Add(member);
                        }
                        else if (string.Equals(member.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(member.Status))
                        {
                            model.PendingMembers.Add(member);
                        }
                    }
                }

                string contactsQuery = "SELECT Id, FirstName, Email, Message, Status, SubmittedAt FROM dbo.ContactMessages ORDER BY SubmittedAt DESC";
                using (SqlCommand cmd = new SqlCommand(contactsQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.ContactMessages.Add(new ContactMessageViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FirstName = reader["FirstName"].ToString(),
                            Email = reader["Email"].ToString(),
                            Message = reader["Message"].ToString(),
                            Status = reader["Status"].ToString(),
                            SubmittedAt = Convert.ToDateTime(reader["SubmittedAt"])
                        });
                    }
                }

                string announcementsQuery = "SELECT Id, Title, Content, CreatedAt FROM dbo.Announcements ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(announcementsQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Announcements.Add(new AnnouncementViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"].ToString(),
                            Content = reader["Content"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }

                string resourcesQuery = "SELECT Id, Title, Description, FileName, MediaType, CreatedAt FROM dbo.Resources ORDER BY CreatedAt DESC";
                using (SqlCommand cmd = new SqlCommand(resourcesQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Resources.Add(new ResourceViewModel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Title = reader["Title"].ToString(),
                            Description = reader["Description"].ToString(),
                            FileName = reader["FileName"].ToString(),
                            MediaType = reader["MediaType"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveMember(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string updateQuery = "UPDATE dbo.ApplicationForm SET Status = 'Approved' WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectMember(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string updateQuery = "UPDATE dbo.ApplicationForm SET Status = 'Rejected' WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAnnouncement(AnnouncementInputModel model)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide both title and content for the announcement.";
                return RedirectToAction("AdminDashboard");
            }

            EnsureAdminTables();
            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string insertQuery = "INSERT INTO dbo.Announcements (Title, Content, CreatedAt) VALUES (@Title, @Content, GETDATE())";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@Title", model.Title);
                cmd.Parameters.AddWithValue("@Content", model.Content);
                cmd.ExecuteNonQuery();
            }

            TempData["SuccessMessage"] = "Announcement added successfully.";
            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResource(ResourceInputModel model)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            if (model.File == null || model.File.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please upload an image or video resource file.";
                return RedirectToAction("AdminDashboard");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please add a title and select a file.";
                return RedirectToAction("AdminDashboard");
            }

            EnsureAdminTables();
            using (var memoryStream = new System.IO.MemoryStream())
            {
                model.File.InputStream.CopyTo(memoryStream);
                byte[] mediaData = memoryStream.ToArray();

                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string insertQuery = @"INSERT INTO dbo.Resources (Title, Description, FileName, MediaType, MediaData, CreatedAt)
                                            VALUES (@Title, @Description, @FileName, @MediaType, @MediaData, GETDATE())";
                    SqlCommand cmd = new SqlCommand(insertQuery, conn);
                    cmd.Parameters.AddWithValue("@Title", model.Title);
                    cmd.Parameters.AddWithValue("@Description", model.Description ?? string.Empty);
                    cmd.Parameters.AddWithValue("@FileName", System.IO.Path.GetFileName(model.File.FileName));
                    cmd.Parameters.AddWithValue("@MediaType", model.File.ContentType);
                    cmd.Parameters.AddWithValue("@MediaData", mediaData);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["SuccessMessage"] = "Resource uploaded successfully.";
            return RedirectToAction("AdminDashboard");
        }

        public ActionResult ResourceFile(int id)
        {
            if (!IsAdminUser())
            {
                return RedirectToAction("Signin");
            }

            using (SqlConnection conn = DbHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT FileName, MediaType, MediaData FROM dbo.Resources WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var mediaType = reader["MediaType"].ToString();
                        var mediaData = (byte[])reader["MediaData"];
                        return File(mediaData, mediaType);
                    }
                }
            }

            return HttpNotFound();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.Confirm)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View(model);
                }

                try
                {
                    using (SqlConnection conn = DbHelper.GetConnection())
                    {
                        conn.Open();

                        // Check if email already exists
                        string checkQuery = "SELECT COUNT(*) FROM dbo.Users WHERE Email = @Email";
                        SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                        checkCmd.Parameters.AddWithValue("@Email", model.Email);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count > 0)
                        {
                            ModelState.AddModelError("", "Email already registered.");
                            return View(model);
                        }

                        // Hash password
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                        // Insert new user
                        string insertQuery = "INSERT INTO dbo.Users (Email, Password, Role, CreatedAt) VALUES (@Email, @Password, @Role, @CreatedAt)";
                        SqlCommand cmd = new SqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@Email", model.Email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Role", "user");
                        cmd.Parameters.AddWithValue("@CreatedAt", System.DateTime.Now);

                        cmd.ExecuteNonQuery();

                        // Account created successfully - redirect to sign in
                        TempData["SuccessMessage"] = "Account created successfully! Please sign in with your credentials.";
                        return RedirectToAction("Signin");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult ChangePassword()
        {
            if (string.IsNullOrEmpty(Session["UserEmail"]?.ToString()) && !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Signin");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var email = Session["UserEmail"]?.ToString() ?? User.Identity.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Signin");
            }

            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is required.");
            }
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "New password is required.");
            }
            if (string.IsNullOrWhiteSpace(model.ConfirmNewPassword))
            {
                ModelState.AddModelError("ConfirmNewPassword", "Confirm new password is required.");
            }
            if (!string.IsNullOrWhiteSpace(model.NewPassword) && !string.IsNullOrWhiteSpace(model.ConfirmNewPassword) && model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("", "New Password and Confirm New Password do not match.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                using (SqlConnection conn = DbHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT Password FROM dbo.Users WHERE Email = @Email";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Email", email);

                    string storedPassword = cmd.ExecuteScalar()?.ToString();
                    if (string.IsNullOrEmpty(storedPassword) || !BCrypt.Net.BCrypt.Verify(model.CurrentPassword, storedPassword))
                    {
                        ModelState.AddModelError("", "Current password is incorrect.");
                        return View(model);
                    }

                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    string updateQuery = "UPDATE dbo.Users SET Password = @Password WHERE Email = @Email";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, conn);
                    updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
                    updateCmd.Parameters.AddWithValue("@Email", email);
                    updateCmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("ChangePassword");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                return View(model);
            }
        }

        public ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Form(ApplicationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = DbHelper.GetConnection())
                    {
                        conn.Open();

                        // Insert application form into database
                        string insertQuery = @"INSERT INTO dbo.ApplicationForm 
                            (FullName, Email, RollNumber, Department, Batch, 
                            CodeForcesHandle, AtCoderHandle, CodeChefHandle, LeetCodeHandle, VJudgeHandle, 
                            ReasonForJoin, Status, SubmittedAt) 
                            VALUES 
                            (@FullName, @Email, @RollNumber, @Department, @Batch, 
                            @CodeForcesHandle, @AtCoderHandle, @CodeChefHandle, @LeetCodeHandle, @VJudgeHandle, 
                            @ReasonForJoin, 'Pending', GETDATE())";

                        SqlCommand cmd = new SqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@FullName", model.FullName ?? "");
                        cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                        cmd.Parameters.AddWithValue("@RollNumber", model.RollNumber ?? "");
                        cmd.Parameters.AddWithValue("@Department", model.Department ?? "");
                        cmd.Parameters.AddWithValue("@Batch", model.Batch ?? "");
                        cmd.Parameters.AddWithValue("@CodeForcesHandle", model.CodeForcesHandle ?? "");
                        cmd.Parameters.AddWithValue("@AtCoderHandle", model.AtCoderHandle ?? "");
                        cmd.Parameters.AddWithValue("@CodeChefHandle", model.CodeChefHandle ?? "");
                        cmd.Parameters.AddWithValue("@LeetCodeHandle", model.LeetCodeHandle ?? "");
                        cmd.Parameters.AddWithValue("@VJudgeHandle", model.VJudgeHandle ?? "");
                        cmd.Parameters.AddWithValue("@ReasonForJoin", model.ReasonForJoin ?? "");

                        cmd.ExecuteNonQuery();

                        // Set success message and redirect
                        TempData["SuccessMessage"] = "Application submitted successfully! Your application is under review. You'll be notified once the admin team reviews it.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while submitting your application: " + ex.Message);
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection conn = DbHelper.GetConnection())
                    {
                        conn.Open();

                        // Insert contact message into database
                        string insertQuery = @"INSERT INTO dbo.ContactMessages 
                            (FirstName, Email, Message, Status, SubmittedAt) 
                            VALUES 
                            (@FirstName, @Email, @Message, 'Not Replied', GETDATE())";

                        SqlCommand cmd = new SqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@FirstName", model.FirstName ?? "");
                        cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                        cmd.Parameters.AddWithValue("@Message", model.Message ?? "");

                        cmd.ExecuteNonQuery();

                        // Set success message and redirect
                        TempData["SuccessMessage"] = "Thank you for contacting us! We've received your message and will get back to you soon.";
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while sending your message: " + ex.Message);
                }
            }

            return View(model);
        }
    }
}

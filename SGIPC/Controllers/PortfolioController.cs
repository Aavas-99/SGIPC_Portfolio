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
    }
}

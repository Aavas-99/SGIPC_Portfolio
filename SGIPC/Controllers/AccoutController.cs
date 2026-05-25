using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.Web.Mvc;
using System.Web.Security;
using SGIPC.Models;

namespace SGIPC.Controllers
{
    public class AccountController : Controller
    {
        // ── GET: /Account/SignUp ──
        [HttpGet]
        public ActionResult SignUp()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // ── POST: /Account/SignUp ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if (model.Password != model.Confirm)
            {
                ViewBag.Error = "Passwords do not match.";
                return View(model);
            }

            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                // Check duplicate email
                var checkCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email = @Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", model.Email);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    ViewBag.Error = "An account with this email already exists.";
                    return View(model);
                }

                // Hash password
                string hashed = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // Insert
                var cmd = new SqlCommand(
                    "INSERT INTO Users (Email, Password) VALUES (@Email, @Pass)", conn);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Pass", hashed);
                cmd.ExecuteNonQuery();
            }

            TempData["Success"] = "Account created! Please sign in.";
            return RedirectToAction("SignIn");
        }

        // ── GET: /Account/SignIn ──
        [HttpGet]
        public ActionResult SignIn(string returnUrl = null)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // ── POST: /Account/SignIn ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model, string returnUrl = null)
        {
            using (var conn = DbHelper.GetConnection())
            {
                conn.Open();

                var cmd = new SqlCommand(
                    "SELECT Id, Email, Password, Role FROM Users WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", model.Email);

                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        ViewBag.Error = "Invalid email or password.";
                        ViewBag.ReturnUrl = returnUrl;
                        return View(model);
                    }

                    string storedHash = reader["Password"].ToString();
                    bool valid = BCrypt.Net.BCrypt.Verify(model.Password, storedHash);

                    if (!valid)
                    {
                        ViewBag.Error = "Invalid email or password.";
                        ViewBag.ReturnUrl = returnUrl;
                        return View(model);
                    }

                    string userId = reader["Id"].ToString();
                    string email = reader["Email"].ToString();
                    string role = reader["Role"].ToString();

                    // Forms Authentication Cookie
                    FormsAuthentication.SetAuthCookie(email, model.RememberMe);

                    // Session
                    Session["UserId"] = userId;
                    Session["Email"] = email;
                    Session["Role"] = role;
                }
            }

            // Redirect back or home
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ── POST: /Account/Logout ──
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout(string returnUrl = null)
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();

            // Expire the auth cookie immediately
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
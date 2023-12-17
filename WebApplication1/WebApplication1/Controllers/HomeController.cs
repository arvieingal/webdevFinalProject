using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Register(FormCollection fc)
        {
            // Retrieve form values
            string firstname = fc["firstname"];
            string lastname = fc["lastname"];
            string gender = fc["gender"];
            string email = fc["email"];
            string password = fc["password"];

            // Perform validation checks
            if (string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "<span style='color: red;'>Please fill in all the required fields.</span>";
                return View(); // Return the view with the error message
            }

            // Create a new user object
            user newUser = new user();
            newUser.firstName = firstname;
            newUser.lastName = lastname;
            newUser.gender = gender;
            newUser.email = email;
            newUser.roleId = 2;
            newUser.password = password;
            newUser.status = "Active";

            // Check if the email exists in the database
            bool isEmailAvailable = IsEmailAvailable(email);
            if (!isEmailAvailable)
            {
                ViewBag.Message = "<span style='color: red;'>Email already exists. Please choose a different email.</span>";
                return View(); // Return the view with the error message
            }

            // Add the new user to the database
            sd208Entities sd = new sd208Entities();
            sd.users.Add(newUser);
            sd.SaveChanges();

            ViewBag.Message = "<span style='color: green;'>Registration successful! Please log in.</span>";
            return RedirectToAction("Login", "Home"); // Redirect to the Login action method in the Home controller
        }

        [HttpPost]
        public JsonResult CheckEmailAvailability(string email)
        {
            bool isEmailAvailable = IsEmailAvailable(email);
            if (isEmailAvailable)
            {
                return Json("Email available!");
            }
            else
            {
                return Json("Email already exists. Please choose a different email.");
            }
        }

        private bool IsEmailAvailable(string email)
        {
            using (var sd = new sd208Entities())
            {
                return !sd.users.Any(u => u.email == email);
            }
        }

        public ActionResult Login(string email, string password) // Change parameter to individual email and password parameters
        {
            using (var dbContext = new sd208Entities())
            {
                var user = dbContext.users.FirstOrDefault(a => a.email == email && a.password == password);

                if (user != null)
                {
                    // Store user details in session
                    Session["userId"] = user.userId;
                    Session["firstName"] = user.firstName;
                    Session["lastName"] = user.lastName;
                    Session["gender"] = user.gender;
                    Session["email"] = user.email;
                    Session["password"] = user.password;
                    Session["status"] = user.status;

                    if (user.roleId == 1)
                    {
                        return RedirectToAction("Admin");
                    }
                    else if (user.roleId == 2)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }

            // Invalid login
            ViewBag.ErrorMessage = "Invalid email or password.";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }

        public ActionResult Index()
        {
            // Retrieve user information from session
            int userId = (int)Session["userId"];
            string firstName = Session["firstName"] as string;
            string lastName = Session["lastName"] as string;
            string gender = Session["gender"] as string;
            string email = Session["email"] as string;
            string password = Session["password"] as string;



            // Create a view model and assign user information
            var viewModel = new UserDetailModel
            {
                userId = userId,
                firstName = firstName,
                lastName = lastName,
                gender = gender,
                email = email,
                password = password
            };

            return View(viewModel);
        }

        public ActionResult AdminProfile()
        {
            // Retrieve user information from session
            int userId = (int)Session["userId"];
            string firstName = Session["firstName"] as string;
            string lastName = Session["lastName"] as string;
            string gender = Session["gender"] as string;
            string email = Session["email"] as string;
            string password = Session["password"] as string;



            // Create a view model and assign user information
            var viewModel = new UserDetailModel
            {
                userId = userId,
                firstName = firstName,
                lastName = lastName,
                gender = gender,
                email = email,
                password = password
            };

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Admin()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult User()
        {
            // Retrieve user information from session
            int userId = (int)Session["userId"];
            string firstName = Session["firstName"] as string;
            string lastName = Session["lastName"] as string;
            string gender = Session["gender"] as string;
            string email = Session["email"] as string;
            string password = Session["password"] as string;



            // Create a view model and assign user information
            var viewModel = new UserDetailModel
            {
                userId = userId,
                firstName = firstName,
                lastName = lastName,
                gender = gender,
                email = email,
                password = password
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditProfile(user model, string oldpassword, string newpassword)
        {
            if (ModelState.IsValid)
            {
                using (sd208Entities sd = new sd208Entities())
                {
                    var userToUpdate = sd.users.Find(model.userId);
                    if (userToUpdate != null)
                    {
                        // Check if the old password matches the user's current password
                        if (string.IsNullOrEmpty(newpassword) || oldpassword == userToUpdate.password)
                        {
                            // Update the user's information
                            if (!string.IsNullOrEmpty(model.firstName))
                                userToUpdate.firstName = model.firstName;

                            if (!string.IsNullOrEmpty(model.lastName))
                                userToUpdate.lastName = model.lastName;

                            if (!string.IsNullOrEmpty(model.gender))
                                userToUpdate.gender = model.gender;

                            // Update the user's password if a new password is provided
                            if (!string.IsNullOrEmpty(newpassword))
                            {
                                userToUpdate.password = newpassword;
                            }

                            sd.SaveChanges();

                            return Json(new { success = true });
                        }
                        else
                        {
                            return Json(new { success = false, error = "Incorrect old password" });
                        }
                    }
                }

                return Json(new { success = false, error = "User not found" });
            }

            return Json(new { success = false, error = "Invalid data" });
        }

        public ActionResult ShowUser(FormCollection fc)
        {
            sd208Entities sd = new sd208Entities();
            var userList = (from a in sd.users
                            select a).ToList();

            ViewData["UserList"] = userList;

            return View();
        }



        [HttpPost]
        public ActionResult EditUser(user model)
        {
            if (ModelState.IsValid)
            {
                using (sd208Entities sd = new sd208Entities())
                {
                    var userToUpdate = sd.users.Find(model.userId);
                    if (userToUpdate != null)
                    {
                        userToUpdate.roleId = model.roleId;
                        userToUpdate.status = model.status;
                        sd.SaveChanges();

                        return Json(new { success = true });
                    }
                }

                return Json(new { success = false, error = "User not found" });
            }

            return Json(new { success = false, error = "Invalid data" });
        }


        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            using (sd208Entities sd = new sd208Entities())
            {
                var userToDelete = sd.users.Find(id);
                if (userToDelete != null)
                {
                    sd.users.Remove(userToDelete);
                    sd.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, error = "User not found" });
        }
    }
}
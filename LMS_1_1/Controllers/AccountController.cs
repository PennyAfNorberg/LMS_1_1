using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LMS_1_1.Data;
using LMS_1_1.Models;
using LMS_1_1.Repository;
using LMS_1_1.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LMS_1_1.Controllers
{

    public class AccountController : Controller
    {

        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<LMSUser> _signInManager;
        private readonly UserManager<LMSUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IProgramRepository _programRepository;
        private readonly IUserClaimsPrincipalFactory<LMSUser> _claimsFactory;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
  

        public AccountController(ILogger<AccountController> logger,
          SignInManager<LMSUser> signInManager,
          UserManager<LMSUser> userManager,
          IConfiguration config,
          IProgramRepository programRepository
         ,IUserClaimsPrincipalFactory<LMSUser> claimsFactory
         ,IEmailSender emailSender
            , ApplicationDbContext context


            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
          
            _programRepository = programRepository;
            _claimsFactory = claimsFactory;
            _emailSender = emailSender;
            _context = context;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "App");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "App");
        }


        [HttpPost]
     // [HttpGet]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
      //  public async Task<IActionResult> CreateToken( )
        {
             if (ModelState.IsValid)
             {


                var user = await _userManager.FindByNameAsync(model.Username);

    

                if (user != null)
                {
                   var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                  //  var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        var principal = await _claimsFactory.CreateAsync(user);
                        // Create the token
                        var claims = principal.Claims.ToList();

                        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Email));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""));
                        claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? ""));
                        claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? ""));
                       // claims.Add(new Claim(JwtRegisteredClaimNames.NameId, user.Id));

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(
                          _config["Tokens:Issuer"],
                          _config["Tokens:Audience"],
                          claims.ToArray(),
                          expires: DateTime.Now.AddMinutes(30),
                          signingCredentials: creds);

                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo,
                            isTeacher = token.Claims.Where(c => c.Type == ClaimTypes.Role && c.Value == "Teacher").Select(c => c.Value).FirstOrDefault(),
                            FirstName = token.Claims.Where(c => c.Type == JwtRegisteredClaimNames.GivenName).Select(c => c.Value).FirstOrDefault(),
                            LastName = token.Claims.Where(c => c.Type == JwtRegisteredClaimNames.FamilyName).Select(c => c.Value).FirstOrDefault()
                           // Userid = token.Claims.Where(c => c.Type == JwtRegisteredClaimNames.NameId).Select(c => c.Value).FirstOrDefault()
                        };
                       // await _programRepository.AddTokenUser(results.token, user.Id);

                        return Created("", results);
                    }
                }
           }

            return BadRequest();
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Boolean>> DeleteUser([FromBody] CourseIdViewModel id)
        {

            var userid = id.CourseId;
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleteing user with ID '{userId}'.");
            }

            var tempcoursuser = _context.CourseUsers.Where(cu => cu.LMSUserId == userid);

            _context.CourseUsers.RemoveRange(tempcoursuser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);


            var res = Json(new
            {
                Ok = true
            });

            return Ok(res);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> RegisterNewUser([FromBody] RegisterUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {

                var user = new LMSUser { UserName = registerUser.Email, Email = registerUser.Email, FirstName = registerUser.FirstName, LastName = registerUser.LastName };
                var Role = registerUser.Role;
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var ok = await _userManager.AddToRoleAsync(user, Role);
                    if (!ok.Succeeded)
                    {
                        throw new Exception(string.Join("\n", ok.Errors));
                    }
                    /*  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                      var callbackUrl = Url.Page(
                          "/Account/ConfirmEmail",
                          pageHandler: null,
                          values: new { userId = user.Id, code = code },
                          protocol: Request.Scheme);

                      await _emailSender.SendEmailAsync(registerUser.Email, "Confirm your email",
                          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                          */
                    // teacher makes users so no log in.
                    // await _signInManager.SignInAsync(user, isPersistent: false);

                     var res = Json(new
                     {
                         Name = user.Id
                     });
         


                    return Created("", res);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return StatusCode(500, ModelState);

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<string>> UpdateUserAdmin([FromBody] ManageUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {


         

                var user = new LMSUser { UserName = registerUser.Email, Email = registerUser.Email, FirstName = registerUser.FirstName, LastName = registerUser.LastName };
                var Role = registerUser.Role;
                var user2 = await _userManager.FindByIdAsync(registerUser.Id);
                user2.FirstName = registerUser.FirstName;
                user2.LastName = registerUser.LastName;

                var result = await _userManager.UpdateAsync(user2);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User updated by admin");
                    
                    var roles =await _userManager.GetRolesAsync(user2);

                    if (!roles.Contains(Role))
                    {
                        _logger.LogInformation("User role updated by Admin");
                        var ok2 = await _userManager.RemoveFromRoleAsync(user2, roles[0]);
                        if (!ok2.Succeeded)
                        {
                            throw new Exception(string.Join("\n", ok2.Errors));
                        }
                        var ok = await _userManager.AddToRoleAsync(user2, Role);
                        if (!ok.Succeeded)
                        {
                            throw new Exception(string.Join("\n", ok.Errors));
                        }
                    }
                    /*  var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                      var callbackUrl = Url.Page(
                          "/Account/ConfirmEmail",
                          pageHandler: null,
                          values: new { userId = user.Id, code = code },
                          protocol: Request.Scheme);

                      await _emailSender.SendEmailAsync(registerUser.Email, "Confirm your email",
                          $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                          */
                    // teacher makes users so no log in.
                    // await _signInManager.SignInAsync(user, isPersistent: false);

                    var res = Json(new
                    {
                        status = true
                    });



                    return Ok(res);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return StatusCode(500, ModelState);

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<string>> UpdateUser([FromBody] ManageUserViewModel registerUser)
        {
            if (ModelState.IsValid)
            {
                var user2 = await _userManager.FindByNameAsync(User.Identity.Name);
                var user = new LMSUser { UserName = registerUser.Email, Email = registerUser.Email, FirstName = registerUser.FirstName, LastName = registerUser.LastName };
                // var Role = registerUser.Role;
                user2.FirstName = registerUser.FirstName;
                user2.LastName = registerUser.LastName;
                if (User.Identity.Name != registerUser.Email)
                {
                    throw new AccessViolationException("User tryed to update other user");
                }
                if (registerUser.Password != "")
                {// then try to update password

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user2, registerUser.Oldpassword, registerUser.Password);
                    if (!changePasswordResult.Succeeded)
                    {
                        _logger.LogInformation("Failed to update Password by user");
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Password updated by user");
                    }
                }
                var result = await _userManager.UpdateAsync(user2);
                if (result.Succeeded)
                {

                    _logger.LogInformation("User updated by user");

                   // var roles = await _userManager.GetRolesAsync(user2);

                    /*if (!roles.Contains(Role))
                    {
                        _logger.LogInformation("User role updated by user");
                        var ok2 = await _userManager.RemoveFromRoleAsync(user2, roles[0]);
                        if (!ok2.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, string.Join("\n", ok2.Errors));
                        }
                        var ok = await _userManager.AddToRoleAsync(user2, Role);
                        if (!ok.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, string.Join("\n", ok.Errors));
                        }
                    }*/
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                if(ModelState.Count>0)
                { // errors
                    return StatusCode(500, ModelState);
                }
                var res = Json(new
                {
                    status = true
                });



                return Ok(res);
            }
            
            return StatusCode(500, ModelState);

        }
        [HttpPost]
        public async Task<bool> IsTeacher(string token)
        {
            return await _programRepository.IsTeacherAsync(token);

        }
    }
}

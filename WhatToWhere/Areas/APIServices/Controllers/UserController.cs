using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WhatToWhere.Areas.APIServices.Models;
using WhatToWhere.Areas.APIServices.Models.DataModel;
using WhatToWhere.Filter;
using WhatToWhere.MergeUtility;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Controllers
{
    [Route("api/user/{action}")]
    [APILogFilter]
    public class UserController : ApiController
    {
        [HttpGet]
        public ResponseModel GetVerificationCodeByEmailForSignUp(string email)
        {
            ResponseModel _respModel = new ResponseModel();
            Random rnd = new Random();
            try
            {
                if (!string.IsNullOrEmpty(email))
                {
                    using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                    {
                        _respModel.issuccess = true;
                        var data = _dbcontext.tbl_user.Where(x => x.Email == email).FirstOrDefault();
                        if (data != null)
                        {
                            _respModel.data = new { UserId = data.UserId, LoginType = data.LoginType, emailcode = rnd.Next(100000, 999999).ToString(), email = email };
                        }
                        else
                        {
                            _respModel.data = new { UserId = 0, LoginType = "", emailcode = rnd.Next(100000, 999999).ToString(), email = email };
                        }
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Please enter email address";
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        //[HttpGet]
        //public ResponseModel GetVerificationCodeByMobileForSignUp(string number)
        //{
        //    ResponseModel _respModel = new ResponseModel();
        //    Random rnd = new Random();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(number))
        //        {
        //            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
        //            {
        //                _respModel.issuccess = true;
        //                var data = _dbcontext.tbl_user.Where(x => x.Mobile == number).FirstOrDefault();
        //                if (data != null)
        //                {
        //                    _respModel.data = new { UserId = data.UserId, LoginType = data.LoginType, mobilecode = rnd.Next(100000, 999999).ToString(), number = number };
        //                }
        //                else
        //                {
        //                    _respModel.data = new { UserId = 0, LoginType = "", mobilecode = rnd.Next(100000, 999999).ToString(), number = number };
        //                }
        //            }
        //        }
        //        else
        //        {
        //            _respModel.issuccess = false;
        //            _respModel.message = "Please enter phone number";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _respModel.issuccess = false;
        //        _respModel.message = "Something went wrong";
        //        _respModel.data = ex;
        //    }
        //    return _respModel;
        //}

        [SwaggerRequestExample(typeof(NewUserModel), typeof(NewUserModelExample))]
        [HttpPost]
        [ValidateModel]
        public ResponseModel SignUpUser(NewUserModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var ExistingUser = _dbcontext.tbl_user.Where(x => x.Email == model.Email && model.LoginType != "normal").FirstOrDefault();
                    if (ExistingUser != null)
                    {
                        ExistingUser.FirebaseToken = model.FirebaseToken;
                        ExistingUser.DeviceType = model.DeviceType;
                        _dbcontext.SaveChanges();

                        var data = GetFullProfile(ExistingUser, false);
                        (string token, string refreshtoken) = GetToken(ExistingUser.UserId);
                        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshtoken))
                        {
                            _respModel.issuccess = false;
                            _respModel.message = "Unable to find token";
                            return _respModel;
                        }
                        _respModel.issuccess = true;
                        _respModel.data = MergeData(data, token, refreshtoken);
                        return _respModel;
                    }
                    var isRecordExists = _dbcontext.tbl_user.Any(x => x.Email == model.Email && model.LoginType == "normal");
                    if (isRecordExists)
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Email already registered.";
                        return _respModel;
                    }

                    if (model.LoginType == "normal")
                    {
                        model.Gender = "";
                        model.ProfileImage = "www.whattowhere.net/images/default.jpg";
                    }

                    tbl_user userModel = new tbl_user
                    {
                        UserName = model.UserName,
                        Password = model.Password,
                        //FirstName = model.FirstName,
                        //LastName = model.LastName,
                        Email = model.Email,
                        Mobile = "",
                        Description = "",
                        Gender = (string.IsNullOrEmpty(model.Gender) ? "" : model.Gender),
                        Pronouns = "they",
                        ProfileImage = model.ProfileImage,
                        LoginType = model.LoginType,
                        //IsMobileVerified = false,
                        IsEmailVerified = model.IsEmailVerified,
                        //UserType = model.UserType,
                        FirebaseToken = model.FirebaseToken,
                        IsActive = true,
                        DeviceType = model.DeviceType,
                        DOB = model.DOB
                    };
                    _dbcontext.tbl_user.Add(userModel);
                    _dbcontext.SaveChanges();

                    var data1 = GetFullProfile(userModel, true);
                    (string token1, string refreshtoken1) = GetToken(userModel.UserId);
                    if (string.IsNullOrEmpty(token1) || string.IsNullOrEmpty(refreshtoken1))
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "User created But unable to find token";
                        return _respModel;
                    }
                    _respModel.issuccess = true;
                    _respModel.data = MergeData(data1, token1, refreshtoken1);
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [SwaggerRequestExample(typeof(UserSignInModel), typeof(UserSignInModelExample))]
        [HttpPost]
        [ValidateModel]
        public ResponseModel SignInUser(UserSignInModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    if (model.LoginType.ToLower() == "normal")
                    {
                        var userModel = _dbcontext.tbl_user.Where(x => x.IsActive && x.Email == model.Email && x.Password == model.Password).FirstOrDefault();
                        if (userModel != null)
                        {
                            userModel.FirebaseToken = model.FirebaseToken;
                            userModel.DeviceType = model.DeviceType;
                            _dbcontext.SaveChanges();

                            var data = GetFullProfile(userModel, false);
                            (string token, string refreshtoken) = GetToken(userModel.UserId);
                            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshtoken))
                            {
                                _respModel.issuccess = false;
                                _respModel.message = "Unable to find token";
                                return _respModel;
                            }
                            _respModel.issuccess = true;
                            _respModel.data = MergeData(data, token, refreshtoken);
                        }
                        else
                        {
                            _respModel.issuccess = false;
                            _respModel.message = "Invalid email or password.";
                        }
                    }
                    else
                    {
                        var userModel = _dbcontext.tbl_user.Where(x => x.IsActive && x.Email == model.Email).FirstOrDefault();
                        if (userModel != null)
                        {
                            userModel.FirebaseToken = model.FirebaseToken;
                            userModel.DeviceType = model.DeviceType;
                            _dbcontext.SaveChanges();

                            var data = GetFullProfile(userModel, false);
                            (string token, string refreshtoken) = GetToken(userModel.UserId);
                            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshtoken))
                            {
                                _respModel.issuccess = false;
                                _respModel.message = "Unable to find token";
                                return _respModel;
                            }
                            _respModel.issuccess = true;
                            _respModel.data = MergeData(data, token, refreshtoken);

                        }
                        else
                        {
                            _respModel.issuccess = false;
                            _respModel.message = "Invalid email.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [HttpGet]
        public ResponseModel SignOutUser(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {

                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (userModel != null)
                    {
                        userModel.FirebaseToken = null;
                        _dbcontext.SaveChanges();
                        _respModel.issuccess = true;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Invalid email.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [HttpPost]
        public ResponseModel ForgotPassword(ForgotPasswordModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    if (userModel != null)
                    {
                        if (userModel.LoginType != "normal")
                        {
                            _respModel.issuccess = false;
                            _respModel.message = "Invalid Email : Social user";
                            return _respModel;
                        }
                        userModel.Password = model.Password;
                        userModel.FirebaseToken = null;
                        _dbcontext.SaveChanges();
                        _respModel.issuccess = true;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Invalid email.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [HttpPost]
        public ResponseModel ResetPassword(ResetPasswordModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId && x.Password == model.OldPassword).FirstOrDefault();
                    if (userModel != null)
                    {
                        if (userModel.LoginType != "normal")
                        {
                            _respModel.issuccess = false;
                            _respModel.message = "Invalid Email : Social user";
                            return _respModel;
                        }
                        userModel.Password = model.Newpassword;
                        _dbcontext.SaveChanges();
                        _respModel.issuccess = true;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Invalid email or old password.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [HttpPost]
        public ResponseModel UpdateEmailAddress(UpdateEmailAddressModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var existsData = _dbcontext.tbl_user.Any(x => x.Email == model.NewEmail);
                    if (existsData)
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Email already exists";
                    }
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    if (userModel != null)
                    {
                        userModel.Email = model.NewEmail;
                        _dbcontext.SaveChanges();
                        _respModel.issuccess = true;
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Invalid user details.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        //[HttpPost]
        //public ResponseModel UpdateMobileNumber(UpdateMobileNumberModel model)
        //{
        //    ResponseModel _respModel = new ResponseModel();
        //    try
        //    {
        //        using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
        //        {
        //            var existsData = _dbcontext.tbl_user.Any(x => x.Mobile == model.NewMobileNumber);
        //            if (existsData)
        //            {
        //                _respModel.issuccess = false;
        //                _respModel.message = "Mobile number already exists";
        //            }
        //            var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId).FirstOrDefault();
        //            if (userModel != null)
        //            {
        //                userModel.Mobile = model.NewMobileNumber;
        //                _dbcontext.SaveChanges();
        //                _respModel.issuccess = true;
        //            }
        //            else
        //            {
        //                _respModel.issuccess = false;
        //                _respModel.message = "Invalid user details.";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _respModel.issuccess = false;
        //        _respModel.message = "Something went wrong";
        //        _respModel.data = ex;
        //    }
        //    return _respModel;
        //}

        [HttpPost]
        public ResponseModel UpdateProfilePicture()
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                string filePath = "";
                EditProfileModel model = new EditProfileModel();

                if (HttpContext.Current.Request.Form.AllKeys.Contains("UserId"))
                {
                    model.UserId = Convert.ToInt64(HttpContext.Current.Request.Form["UserId"]);
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "UserId parameter is missing.";
                    return _respModel;
                }

                try
                {
                    var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = model.UserId + "profilepic" + ((new Random()).Next(100, 999)).ToString() + Path.GetExtension(file.FileName);
                        var path = HttpContext.Current.Server.MapPath("~/Images/" + model.UserId + "/profilepic/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        else
                        {
                            System.IO.DirectoryInfo di = new DirectoryInfo(path);
                            foreach (FileInfo fl in di.GetFiles())
                            {
                                fl.Delete();
                            }
                        }
                        file.SaveAs(Path.Combine(path, fileName));
                        filePath = "www.whattowhere.net/Images/" + model.UserId + "/profilepic/" + fileName;

                        using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                        {
                            var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId).FirstOrDefault();
                            if (userModel != null)
                            {
                                userModel.ProfileImage = filePath;
                                _dbcontext.SaveChanges();
                                _respModel.issuccess = true;
                                _respModel.data = new { ImagePath = filePath };
                            }
                            else
                            {
                                _respModel.issuccess = false;
                                _respModel.message = "Invalid user details.";
                            }
                        }
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "File is missing";
                        return _respModel;
                    }
                }
                catch (Exception ex)
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Something went wrong";
                    _respModel.data = ex;
                    return _respModel;
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [SwaggerRequestExample(typeof(EditProfileModel), typeof(EditProfileModelExample))]
        [HttpPost]
        public ResponseModel EditUserProfile(EditProfileModel model)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == model.UserId).FirstOrDefault();
                    if (userModel != null)
                    {
                        userModel.UserName = model.UserName;
                        userModel.Description = model.Description;
                        userModel.Pronouns = model.Pronouns;
                        userModel.DOB = model.DOB;
                        userModel.Mobile = model.Mobile;
                        userModel.Gender = model.Gender;
                        _dbcontext.SaveChanges();
                        _respModel.issuccess = true; 
                        _respModel.data = GetFullProfile(userModel, false);
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "Invalid user details.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        [HttpGet]
        public ResponseModel GetUserDataByUserId(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId == UserId).FirstOrDefault();
                    if (userModel != null)
                    {
                        _respModel.issuccess = true;
                        _respModel.data = GetFullProfile(userModel, false);
                    }
                    else
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "User details not found.";
                    }
                }
            }
            catch (Exception ex)
            {
                _respModel.issuccess = false;
                _respModel.message = "Something went wrong";
                _respModel.data = ex;
            }
            return _respModel;
        }

        private object GetFullProfile(tbl_user userModel, bool isFirstLogin)
        {
            UserModel dataModel = new UserModel();
            Mapper.Map(userModel, dataModel);

            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                int followingCount = _dbcontext.tbl_follow.Where(x => x.UserId == userModel.UserId).ToList().Count();
                int followerCount = _dbcontext.tbl_follow.Where(x => x.FollowingId == userModel.UserId).ToList().Count();
                object follow = new { FollowingCount = followingCount, FollowerCount = followerCount, LoginFirst = isFirstLogin };
                object mergedData = MyTypeBuilder.Merge(dataModel, follow, "merged");
                return mergedData;
            }
        }

        private (string, string) GetToken(long userid)
        {
            string baseUrl = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.LocalPath, "");// .GetRequestContext().VirtualPathRoot;

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userid.ToString() )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(baseUrl + "/Token", content).Result; //url + 
                if (response.StatusCode.ToString() == "OK")
                {
                    var data = (JObject)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                    string token = data["access_token"].Value<string>();
                    string refreshToken = data["refresh_token"].Value<string>();
                    return (token, refreshToken);
                }
                return (null, null);
            }
        }

        public object MergeData(object data, string token, string refreshtoken)
        {
            object tkn = new { Access_Token = token, Refresh_Token = refreshtoken };
            object mergedData = MyTypeBuilder.Merge(data, tkn, "merged");
            return mergedData;
        }
    }
}

using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WhatToWhere.Filter;
using WhatToWhere.MergeUtility;
using WhatToWhere.Models;
using WhereToWhere.Data;

namespace WhatToWhere.Areas.APIServices.Controllers
{
    [Route("api/follow/{action}")]
    [APILogFilter]
    public class FollowController : ApiController
    {
        [HttpPost]
        public ResponseModel UserFollowing(long LoginUserId, long FollowingId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                if (LoginUserId <= 0 || FollowingId <= 0)
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
                }
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var ExistingFollowingUser = _dbcontext.tbl_follow.Where(x => x.UserId == LoginUserId && x.FollowingId == FollowingId).FirstOrDefault();
                    if (ExistingFollowingUser != null)
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "User already following that person.";
                        return _respModel;
                    }

                    tbl_follow userFollowing = new tbl_follow
                    {
                        UserId = LoginUserId,
                        FollowingId = FollowingId
                    };
                    _dbcontext.tbl_follow.Add(userFollowing);
                    _dbcontext.SaveChanges();
                    _respModel.issuccess = true;
                    _respModel.message = "User following that person";
                    int followingCount = _dbcontext.tbl_follow.Where(x => x.UserId == LoginUserId).ToList().Count();
                    int followerCount = _dbcontext.tbl_follow.Where(x => x.FollowingId == LoginUserId).ToList().Count();
                    _respModel.data = new { followingCount = followingCount, followerCount = followerCount };
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
        public ResponseModel UserUnfollowing(long LoginUserId, long UnfollowingId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                if (LoginUserId <= 0 || UnfollowingId <= 0)
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
                }
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var ExistingFollowingUser = _dbcontext.tbl_follow.Where(x => x.UserId == LoginUserId && x.FollowingId == UnfollowingId).FirstOrDefault();
                    if (ExistingFollowingUser == null)
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "User details not found";
                        return _respModel;
                    }

                    _dbcontext.tbl_follow.Remove(ExistingFollowingUser);
                    _dbcontext.SaveChanges();
                    _respModel.issuccess = true;
                    _respModel.message = "User  unfollowing that person";
                    int followingCount = _dbcontext.tbl_follow.Where(x => x.UserId == LoginUserId).ToList().Count();
                    int followerCount = _dbcontext.tbl_follow.Where(x => x.FollowingId == LoginUserId).ToList().Count();
                    _respModel.data = new { followingCount = followingCount, followerCount = followerCount };
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
        public ResponseModel RemoveFollower(long LoginUserId, long FollowerId)
        {
            ResponseModel _respModel = new ResponseModel();
            try
            {
                if (LoginUserId <= 0 || FollowerId <= 0)
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
                }
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var ExistingFollowingUser = _dbcontext.tbl_follow.Where(x => x.UserId == FollowerId && x.FollowingId == LoginUserId).FirstOrDefault();
                    if (ExistingFollowingUser == null)
                    {
                        _respModel.issuccess = false;
                        _respModel.message = "User details not found";
                        return _respModel;
                    }

                    _dbcontext.tbl_follow.Remove(ExistingFollowingUser);
                    _dbcontext.SaveChanges();
                    _respModel.issuccess = true;
                    _respModel.message = "User  unfollowing that person";
                    int followingCount = _dbcontext.tbl_follow.Where(x => x.UserId == LoginUserId).ToList().Count();
                    int followerCount = _dbcontext.tbl_follow.Where(x => x.FollowingId == LoginUserId).ToList().Count();
                    _respModel.data = new { followingCount = followingCount, followerCount = followerCount };
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
        public ResponseModel GetFollowerList(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            Random rnd = new Random();
            try
            {
                if (UserId >= 0)
                {
                    using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                    {
                        var data = (from usr in _dbcontext.tbl_user
                                    join fl in _dbcontext.tbl_follow on usr.UserId equals fl.UserId
                                    where fl.FollowingId == UserId
                                    select new
                                    {
                                        FollowerId = usr.UserId,
                                        usr.UserName,
                                        usr.Email,
                                        usr.ProfileImage,
                                        isFollowing = _dbcontext.tbl_follow.Where(x => x.UserId == UserId && x.FollowingId == usr.UserId).Any()
                                    }).ToList();

                        _respModel.issuccess = true;
                        _respModel.data = data;
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
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
        public ResponseModel GetFollowingList(long UserId)
        {
            ResponseModel _respModel = new ResponseModel();
            Random rnd = new Random();
            try
            {
                if (UserId >= 0)
                {
                    using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                    {
                        var data = (from usr in _dbcontext.tbl_user
                                    join fl in _dbcontext.tbl_follow on usr.UserId equals fl.FollowingId
                                    where fl.UserId == UserId
                                    select new
                                    {
                                        FollowingId = usr.UserId,
                                        usr.UserName,
                                        usr.Email,
                                        usr.ProfileImage
                                    }).ToList();

                        _respModel.issuccess = true;
                        _respModel.data = data;
                    }
                }
                else
                {
                    _respModel.issuccess = false;
                    _respModel.message = "Invalid input details";
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
        public ResponseModel SearchProfile(string str, long loginUserId)
        {
            ResponseModel _respModel = new ResponseModel();
            Random rnd = new Random();
            try
            {
                using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
                {
                    var allstrings = str.Split(' ');
                    var userModel = _dbcontext.tbl_user.Where(x => x.UserId != loginUserId &&
                    allstrings.Contains(x.UserName)
                    ).ToList();
                    if (userModel != null)
                    {
                        _respModel.issuccess = true;
                        _respModel.data = GetFullProfile(userModel, loginUserId);
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
        private object GetFullProfile(List<tbl_user> userList, long loginUserId)
        {
            List<object> finaluserList = new List<object>();
            using (WhatToWhereDBEntities _dbcontext = new WhatToWhereDBEntities())
            {
                foreach (var model in userList)
                {
                    int followingCount = _dbcontext.tbl_follow.Where(x => x.UserId == model.UserId).ToList().Count();
                    int followerCount = _dbcontext.tbl_follow.Where(x => x.FollowingId == model.UserId).ToList().Count();
                    var isFollowing = _dbcontext.tbl_follow.Where(x => x.UserId == loginUserId && x.FollowingId == model.UserId).Any();
                    var isFollower = _dbcontext.tbl_follow.Where(x => x.UserId == model.UserId && x.FollowingId == loginUserId).Any();
                    object follow = new { FollowingCount = followingCount, FollowerCount = followerCount, IsFollowing = isFollowing, IsFollower = isFollower };
                    object mergedData = MyTypeBuilder.Merge(model, follow, "merged");
                    finaluserList.Add(mergedData);
                }
                return finaluserList;
            }
        }
    }

}

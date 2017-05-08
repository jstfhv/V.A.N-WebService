﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using WebService.DatabaseContext;
using WebService.Models;
using WebService.ViewModels;

namespace WebService.Controllers {
    public class RatingController: ApiController {
        [HttpPost]
        public bool RateLocation(int id, RatingVM rating) {
            using(var ctx = new VANContext()) {
                Location location = ctx.Locations.Include("Ratings").SingleOrDefault(x => x.LocationID == id);
                if(location != null) {
                    rating.Date = DateTime.Now;
                    rating.User = Mapper.Map<UserVM>(ctx.Users.SingleOrDefault(x => x.UserId == rating.User.UserId));
                    location.Ratings.Add(Mapper.Map<Rating>(rating));
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        [HttpGet]
        public IEnumerable<RatingVM> GetAllByLocation(int id) {
            using(var ctx = new VANContext()) {
                Location location = ctx.Locations.Include("Ratings").SingleOrDefault(x => x.LocationID == id);
                if(location != null) {
                    List<Rating> ratings = new List<Rating>();
                    foreach(Rating rating in location.Ratings) {
                        ratings.Add(ctx.Ratings.Include("User").SingleOrDefault(x => x.RatingID == rating.RatingID));
                    }

                    return Mapper.Map<IEnumerable<RatingVM>>(ratings);
                }
                return null;
            }
        }

        [HttpGet]
        public IEnumerable<RatingVM> GetTop3ByLocation(int id) {
            using(var ctx = new VANContext()) {
                Location location = ctx.Locations.Include("Ratings").SingleOrDefault(x => x.LocationID == id);
                if(location != null) {
                    List<Rating> ratings = new List<Rating>();
                    foreach(Rating rating in location.Ratings) {
                        ratings.Add(ctx.Ratings.Include("User").SingleOrDefault(x => x.RatingID == rating.RatingID));
                    }
                    List<Rating> sortedList = ratings.OrderByDescending(x => x.Date).Take(3).ToList();
                    return Mapper.Map<IEnumerable<RatingVM>>(sortedList);
                }
                return null;
            }
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FeeforFreedom.Models;
using HelperLibrary;
using HelperLibrary.MongoDB;
using HelperLibrary.DynamoDB;

namespace FeeforFreedom.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDynamoDBHelper _dynamoDBHelper;
        private readonly IMongoDBHelper _mongoDBHelper;
        private bool scanIndex;
        private static Anime editedAnime;
        private bool isMongoDB = false;
        public HomeController()
        {
            this._dynamoDBHelper = new DynamoDBHelper();
            this._mongoDBHelper = new MongoDBHelper("Context", "Anime");
        }
        public IActionResult Index()
        {
            //var result = _dynamoDBHelper.GetPage<Anime>("Anime", 2, true);
            //foreach (var item in result)
            //{
            //    _dynamoDBHelper.DeleteItem<Anime>(item);
            //    _dynamoDBHelper.DeleteItem<Anime>(item);
            //    _mongoDBHelper.DeleteItem<Anime>(item);
            //}
            //_mongoDBHelper.SaveList<Anime>(result);
            //_mongoDBHelper.GetItemByDate<Anime>(DateTime.Today,DateTime.MinValue);
            //_mongoDBHelper.Save<Anime>(temp);
            //_mongoDBHelper.GetItem<Anime>("Name", "CsssS");
            //_dynamoDBHelper.GetPage<Anime>("Anime", 2, true);
            //_dynamoDBHelper.GetItem<Anime>("Category", "Değişti");
            //_dynamoDBHelper.GetItemByDate<Anime>(DateTime.Now, DateTime.MinValue);
            return View();
        }
        public void ChangeScanIndex()
        {
            scanIndex = !scanIndex;
        }
        public IActionResult Edit()
        {
            return View(editedAnime);
        }
        public IActionResult Insert()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #region AJAX FUNCTIONS
        [HttpPost]
        public JsonResult GetPage(string sort)
        {
            if (isMongoDB)
            {
                return Json(_mongoDBHelper.GetPage<Anime>("Anime", 2, bool.Parse(sort)));
            }
            else
            {
                return Json(_dynamoDBHelper.GetPage<Anime>("Anime", 2, bool.Parse(sort)));
            }
        }
        [HttpPost]
        public JsonResult SearchAnimeByName(string name)
        {
            if (isMongoDB)
            {
                return Json(_mongoDBHelper.GetItem<Anime>("Name", name));
            }
            else
            {
                return Json(_dynamoDBHelper.GetItem<Anime>("Name", name));
            }

        }
        [HttpPost]
        public JsonResult SearchAnimeByCategory(string category, string sort)
        {
            if (isMongoDB)
            {
                return Json(_mongoDBHelper.GetItem<Anime>("Category", category));
            }
            else
            {
                return Json(_dynamoDBHelper.GetItem<Anime>("Category", category));
            }
        }
        [HttpPost]
        public JsonResult ExtractAnimeGivenHTMLText(string animeText)
        {
            Anime anim = new Anime();
            string[] tempArray = animeText != null ? animeText.Split("<br>") : null;
            if (tempArray != null)
            {
                foreach (var item in tempArray)
                {
                    anim.Name = item.Split("Name = ").Length > 1 ? item.Split("Name = ")[1] : anim.Name;
                    anim.bannerLink = item.Split("src=").Length > 1 ? item.Split("src=")[1] : anim.bannerLink;
                    anim.bannerLink = anim.bannerLink != null ? anim.bannerLink.Split(">").Length > 1 ? anim.bannerLink.Split(">")[0] : anim.bannerLink : null;
                    anim.Category = item.Split("Category = ").Length > 1 ? item.Split("Category = ")[1] : anim.Category;
                    anim.ReleaseDate = item.Split("Release Date = ").Length > 1 ? item.Split("Release Date = ")[1] : anim.ReleaseDate;
                    anim.link = item.Split("href=").Length > 1 ? item.Split("href=")[1] : anim.link;
                    anim.link = anim.link != null ? anim.link.Split("target=").Length > 1 ? anim.link.Split("target=")[0] : anim.link : null;
                    anim.link = anim.link != null ? anim.link.Split(">").Length > 1 ? anim.link.Split(">")[0] : anim.link : null;
                }
                anim.link = anim.link != null ? anim.link.Replace("\"", "") : null;
                anim.bannerLink = anim.bannerLink != null ? anim.bannerLink.Replace("\"", "") : null;
                anim.bannerLink = anim.bannerLink != null ? anim.bannerLink.Replace("/\"", "") : null;
                anim.ReleaseDate = anim.ReleaseDate.Split(":").Length > 1 ? anim.ReleaseDate.Split(":")[1] : anim.ReleaseDate;
                editedAnime = anim;
                return Json(anim);
            }
            return Json(null);
        }
        public JsonResult InsertAnime(string name, string category, string releaseDate, string link, string bannerLink, string writer)
        {
            Anime anim = new Anime()
            {
                Name = name,
                Category = category,
                ReleaseDate = releaseDate,
                link = link,
                bannerLink = bannerLink,
                Writer = writer,
                Id = Guid.NewGuid()
            };

            if (isMongoDB)
            {
                try
                {
                    _mongoDBHelper.Save<Anime>(anim);
                    return Json("Success");
                }
                catch
                {
                    return Json("Fail");
                }
            }
            else
            {
                try
                {
                    _dynamoDBHelper.Save<Anime>(anim);
                    return Json("Success");
                }
                catch
                {
                    return Json("Fail");
                }
            }

            

        }
        public string PathConversion(string bannerLink)
        {
            string fileName = bannerLink.Split("/").Length > 1 ? bannerLink.Split("/")[bannerLink.Split("/").Length - 1] : null;
            string path2 = @"C:/Users/Anil Dursun/source/repos/FeeforFreedom/FeeForFreedom/wwwroot/images/" + fileName;
            System.IO.File.Copy(bannerLink, path2);
            return path2;
        }
        #endregion                                                                                       // AJAX FUNCTIONS  \\
    }
}

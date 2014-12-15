using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjectInterview.DAL;
using ProjectInterview.Models;
using ImpactWorks.FBGraph.Interfaces;
using ImpactWorks.FBGraph.Core;
using ImpactWorks.FBGraph.Connector;
using ProjectInterview.Services;

namespace ProjectInterview.Controllers
{

    [Authorize]
    public class NewsArticleController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        FacebookAuthentication auth = new FacebookAuthentication();

        // GET: NewsArticle
        public ActionResult Index()
        {
            return View(db.newsArticles.ToList());
        }

        // GET: NewsArticle/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.newsArticles.Find(id);
            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // GET: NewsArticle/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsArticle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsArticleID,Title,NewspaperName,PublishDate,Excerpt")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                db.newsArticles.Add(newsArticle);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsArticle);
        }

        // GET: NewsArticle/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.newsArticles.Find(id);
            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // POST: NewsArticle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsArticleID,Title,NewspaperName,PublishDate,Excerpt")] NewsArticle newsArticle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(newsArticle).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsArticle);
        }

        // GET: NewsArticle/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsArticle newsArticle = db.newsArticles.Find(id);
            if (newsArticle == null)
            {
                return HttpNotFound();
            }
            return View(newsArticle);
        }

        // POST: NewsArticle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NewsArticle newsArticle = db.newsArticles.Find(id);
            db.newsArticles.Remove(newsArticle);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult FacebookPostSuccess()
        {
            if (Request.QueryString["code"] != null)
            {
                string Code = Request.QueryString["code"];
                Session["facebookQueryStringValue"] = Code;
            }
            if (Session["facebookQueryStringValue"] != null)
            {
                Facebook facebook = auth.FacebookAuth("NewsArticle");
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();

                if (Session["NewsArticleId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByNewsArticleId(Convert.ToInt32(Session["NewsArticleId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["NewsArticleId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;
                }

            }
            return View();
        }

        public JsonResult PostNewsArticleOnFacebook(int? id)
        {

            Session["NewsArticleId"] = id;

            Facebook facebook = auth.FacebookAuth("NewsArticle");
            if (Session["facebookQueryStringValue"] == null)
            {
                string authLink = facebook.GetAuthorizationLink();
                return Json(authLink);
            }

            if (Session["facebookQueryStringValue"] != null)
            {
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();

                if (Session["NewsArticleId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByNewsArticleId(Convert.ToInt32(Session["NewsArticleId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["NewsArticleId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;

                }
            }
            return Json("No");
        }



        private IFeedPost CreateFacebookMessageByNewsArticleId(int? id)
        {
            //Retrieve NewsArticle data from database and to create a facebook message
            NewsArticle na = db.newsArticles.Find(id);

            //Creating a Facebook Message
            IFeedPost FBpost = new FeedPost();

            FBpost.Action = new FBAction { Name = na.Title, Link = "http://www.google.com.br" };
            FBpost.Caption = "Publish Date - " + na.PublishDate;
            FBpost.Description = na.Excerpt;
            FBpost.Url = "http://www.google.com.br";
            FBpost.Name = na.Title;
            FBpost.Message = "NewsArticle entry from " + na.NewspaperName;

            return FBpost;

        }

    }
}

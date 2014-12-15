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
using ProjectInterview.Services;
using ImpactWorks.FBGraph.Connector;
using ImpactWorks.FBGraph.Core;
using ImpactWorks.FBGraph.Interfaces;

namespace ProjectInterview.Controllers
{

    [Authorize]
    public class BlogController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        FacebookAuthentication auth = new FacebookAuthentication();


        // GET: Blog
        public ActionResult Index()
        {
            return View(db.blogs.ToList());
        }

        // GET: Blog/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogID,Title,Url,PublishDate,Excerpt")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogID,Title,Url,PublishDate,Excerpt")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.blogs.Find(id);
            db.blogs.Remove(blog);
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
                Facebook facebook = auth.FacebookAuth("Blog");
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();

                if (Session["blogId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByBlogId(Convert.ToInt32(Session["blogId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["blogId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;
                }

            }
            return View();
        }

        public JsonResult PostBlogOnFacebook(int? id)
        {

            Session["blogId"] = id;

            Facebook facebook = auth.FacebookAuth("Blog");
            if (Session["facebookQueryStringValue"] == null)
            {
                string authLink = facebook.GetAuthorizationLink();
                return Json(authLink);
            }

            if (Session["facebookQueryStringValue"] != null)
            {
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();

                if (Session["blogId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByBlogId(Convert.ToInt32(Session["blogId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["blogId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;

                }
            }
            return Json("No");
        }



        private IFeedPost CreateFacebookMessageByBlogId(int? id)
        {
            //Retrieve Blog data from database and to create a facebook message
            Blog blog = db.blogs.Find(id);

            //Creating a Facebook Message
            IFeedPost FBpost = new FeedPost();

            FBpost.Action = new FBAction { Name = blog.Title, Link = blog.Url };
            FBpost.Caption = "Publish Date - " + blog.PublishDate;
            FBpost.Description = blog.Excerpt;
            FBpost.Name = blog.Title;
            FBpost.Url = blog.Url;
            FBpost.Message = "Blog entry published by ProjectInterview";

            return FBpost;

        }


    }
}

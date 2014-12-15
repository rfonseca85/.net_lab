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
using ProjectInterview.Utils;
using ProjectInterview.Services;
using ImpactWorks.FBGraph.Connector;
using ImpactWorks.FBGraph.Core;
using ImpactWorks.FBGraph.Interfaces;

namespace ProjectInterview.Controllers
{

    [Authorize]
    public class BookController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        FacebookAuthentication auth = new FacebookAuthentication();

       [HttpGet]
        public ActionResult Index()
        {
            return View(db.books.ToList()); 
        }

        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return null;
            }
        }

        public byte[] GetImageFromDataBase(int Id)
        {
            var q = from temp in db.books where temp.BookID == Id select temp.BookCoverImage;
            byte[] cover = q.First();
            return cover;
        }


        // GET: Book/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // GET: Book/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "BookID,Title,BookCoverImage,Url,PublishDate,Excerpt")] Book book)
        {
            HttpPostedFileBase file = Request.Files["ImageData"];
            
            book.BookCoverImage = Converter.ConvertToBytes(file);

            var newBook = new Book
            {
                Title = book.Title,
                BookCoverImage = book.BookCoverImage,
                Url = book.Url,
                PublishDate = book.PublishDate,
                Excerpt = book.Excerpt
            };
            
            //Saving the book with a image in bytes
            db.books.Add(newBook);

            int i = db.SaveChanges();
            if (i == 1)
            {
                return RedirectToAction("Index");
            }
            
            return View(book);
        }


        // GET: Book/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookID,Title,BookCoverImage,Url,PublishDate,Excerpt")] Book book)
        {

            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["ImageData"];
                if (file.ContentLength > 0)
                {
                    book.BookCoverImage = Converter.ConvertToBytes(file);
                }

                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Book/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Book book = db.books.Find(id);
            db.books.Remove(book);
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
                Facebook facebook = auth.FacebookAuth("Book");
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();
                
                if (Session["bookId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByBookId(Convert.ToInt32(Session["bookId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["bookId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;
                }

            }
            return View();
        }

        public JsonResult PostBookOnFacebook(int? id)
        {

            Session["bookId"] = id;

            Facebook facebook = auth.FacebookAuth("Book");

            if (Session["facebookQueryStringValue"] == null)
            {
                string authLink = facebook.GetAuthorizationLink();
                return Json(authLink);
            }

            if (Session["facebookQueryStringValue"] != null)
            {
                facebook.GetAccessToken(Session["facebookQueryStringValue"].ToString());
                FBUser currentUser = facebook.GetLoggedInUserInfo();

                if (Session["bookId"].ToString() != "")
                {

                    IFeedPost FBpost = CreateFacebookMessageByBookId(Convert.ToInt32(Session["bookId"]));
                    var postID = facebook.PostToWall(currentUser.id.GetValueOrDefault(), FBpost);

                    //Closing 
                    Session["bookId"] = null;
                    Session["facebookQueryStringValue"] = null;
                    auth = null;
                    facebook = null;

                }
            }
            return Json("No"); 
        }



        private IFeedPost CreateFacebookMessageByBookId(int? id)
        {
            //Retrieve Book data from database and to create a facebook message
            Book book = db.books.Find(id);

            //Creating a Facebook Message
            IFeedPost FBpost = new FeedPost();

            FBpost.Action = new FBAction { Name = book.Title, Link = book.Url };
            FBpost.Caption = "Publish Date - " + book.PublishDate;
            FBpost.Description = book.Excerpt;
            FBpost.Name = book.Title;
            FBpost.Url = book.Url;
            FBpost.Message = "Book entry published by ProjectInterview";

            return FBpost;

        }


    }
}

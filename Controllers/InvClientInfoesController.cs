using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceAPI.Models;
using Newtonsoft.Json;
using MvcPaging;

namespace InvoiceAPI.Controllers
{
    public class InvClientInfoesController : Controller
    {
        private InvoiceAPIContext db = new InvoiceAPIContext();

        [HttpPost]
        public ActionResult AutoCliCn(string Term)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    if (!string.IsNullOrEmpty(Term))
                    {
                        int length = 10;// length = 回傳幾筆的參數
                        // term = 使用者輸入的term

                        var client = db.InvClientInfos.Where(C => C.AccountId == invAccount.Id && C.CompName.Contains(Term) && C.Status == 0).OrderByDescending(c => c.Id).Take(length);
                        //.Select(p => new { Id = p.ProductId, Name = p.Name }).AsEnumerable();
                        return Content(JsonConvert.SerializeObject(client.ToList()));
                    }
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult AutoCliCnAll(string Term)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    if (Term != null)
                    {
                        var client = db.InvClientInfos.Where(C => C.AccountId == C.InvAccount.Id && C.CompName.Contains(Term) && C.Status == 0).OrderByDescending(c => c.Id);
                        //.Select(p => new { Id = p.ProductId, Name = p.Name }).AsEnumerable();
                        return Content(JsonConvert.SerializeObject(client.ToList()));
                    }
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult AutoCliUn(string Term)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    if (!string.IsNullOrEmpty(Term))
                    {
                        int length = 10;// length = 回傳幾筆的參數
                        // term = 使用者輸入的term

                        var client = db.InvClientInfos.Where(C => C.AccountId == C.InvAccount.Id && C.UniformNumbers.Contains(Term) && C.Status == 0).OrderByDescending(c => c.Id).Take(length);
                        //.Select(p => new { Id = p.ProductId, Name = p.Name }).AsEnumerable();
                        return Content(JsonConvert.SerializeObject(client.ToList()));
                    }
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult AutoCliUnAll(string Term)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    if (Term != null)
                    {
                        int length = 10;// length = 回傳幾筆的參數
                        // term = 使用者輸入的term

                        var client = db.InvClientInfos.Where(C => C.AccountId == C.InvAccount.Id && C.UniformNumbers.Contains(Term) && C.Status == 0).OrderByDescending(c => c.Id).Take(length);
                        //.Select(p => new { Id = p.ProductId, Name = p.Name }).AsEnumerable();
                        return Content(JsonConvert.SerializeObject(client.ToList()));
                    }
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult CreadeCliInfo(InvClientInfo invClientInfo)
        {
            ModelState.Remove("id");
            ModelState.Remove("initdate");
            ModelState.Remove("AccountId");

            if (ModelState.IsValid)
            {
                if (Session["Guid"] != null)
                {
                    string Guid = Session["Guid"].ToString();
                    InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();
                    invClientInfo.AccountId = invAccount.Id;

                    //invClientInfo.InitDate = DateTime.Now.AddHours(15);

                    invClientInfo.Status = ClientStatusType.已建立; // =0

                    db.InvClientInfos.Add(invClientInfo);
                    db.SaveChanges();
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult CheckCliUn(string UniformNumbers)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.UniformNumbers == UniformNumbers && C.AccountId == invAccount.Id)
                        .FirstOrDefault();

                    if (invClientInfo != null)
                    {
                        return Json(true);
                    }

                    return Json(false);
                }

                return Json(false);
            }
            return Json(false);

        }


        private const int DefaultPageSize = 5;

        [HttpPost]
        public ActionResult SearchCliInfo(string Term, int Page)
        {
            //if (ModelState.IsValid)  //因為可能為空值
            //{
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (Page == null) //因為第一頁不回傳，第二頁回傳2，但系統判斷是從0開始。
                {
                    Page = 0;
                }
                else
                {
                    Page--;
                }

                var invClientInfo = db.InvClientInfos
                    .Where(C => (C.CompName.Contains(Term) || C.UniformNumbers.Contains(Term)) && C.Status == 0 && C.AccountId == invAccount.Id).ToList().OrderByDescending(C => C.Id).ToPagedList(Page, DefaultPageSize);

                return Content(JsonConvert.SerializeObject(invClientInfo));

            }
            return Json(false);
            //}
            //return Json(false);
        }


        [HttpPost]
        public ActionResult LoadCliEd(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    var invClientInfo = db.InvClientInfos.Where(C => C.Id == Id && C.AccountId == invAccount.Id).FirstOrDefault();

                    var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(invClientInfo, Formatting.Indented);

                    return Content(jsonContent, "application/json");
                }
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult UpdateCliEd(InvClientInfo invClientInfo)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    invClientInfo.AccountId = invAccount.Id;  // 單純預防，非必要
                    db.Entry(invClientInfo).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult DeleteCliInfo(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    //InvClientInfo invClientInfo = db.InvClientInfos.Find(Id); // 限主索引，且連表格會一起抓，故常與JsonIgnore並用

                    InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.AccountId == invAccount.Id && C.Id == Id).FirstOrDefault(); // 限主索引，且連表格會一起抓，故常與JsonIgnore並用
                    invClientInfo.Status = ClientStatusType.已刪除;
                    db.Entry(invClientInfo).State = EntityState.Modified;
                    //db.InvClientInfos.Remove(invClientInfo);
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }














        // GET: InvClientInfoes
        public ActionResult Index()
        {
            var invClientInfos = db.InvClientInfos.Include(i => i.InvAccount);
            return View(invClientInfos.ToList());
        }

        // GET: InvClientInfoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvClientInfo invClientInfo = db.InvClientInfos.Find(id);
            if (invClientInfo == null)
            {
                return HttpNotFound();
            }
            return View(invClientInfo);
        }

        // GET: InvClientInfoes/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid");
            return View();
        }

        // POST: InvClientInfoes/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitDate,AccountId,UniformNumbers,CompName,CompAddress,CompPhoneNumber,Status")] InvClientInfo invClientInfo)
        {
            if (ModelState.IsValid)
            {
                db.InvClientInfos.Add(invClientInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invClientInfo.AccountId);
            return View(invClientInfo);
        }

        // GET: InvClientInfoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvClientInfo invClientInfo = db.InvClientInfos.Find(id);
            if (invClientInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invClientInfo.AccountId);
            return View(invClientInfo);
        }

        // POST: InvClientInfoes/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitDate,AccountId,UniformNumbers,CompName,CompAddress,CompPhoneNumber,Status")] InvClientInfo invClientInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invClientInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invClientInfo.AccountId);
            return View(invClientInfo);
        }

        // GET: InvClientInfoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvClientInfo invClientInfo = db.InvClientInfos.Find(id);
            if (invClientInfo == null)
            {
                return HttpNotFound();
            }
            return View(invClientInfo);
        }

        // POST: InvClientInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvClientInfo invClientInfo = db.InvClientInfos.Find(id);
            db.InvClientInfos.Remove(invClientInfo);
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
    }
}

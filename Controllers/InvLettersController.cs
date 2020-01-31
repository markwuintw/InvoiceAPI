using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceAPI.Models;
using System.Text;
using MvcPaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceAPI.Controllers
{
    public class InvLettersController : Controller
    {
        private InvoiceAPIContext db = new InvoiceAPIContext();

        [HttpPost]
        public ActionResult SelectInvLet()  //todo  只顯示 票軌(?) 或 使用 發票期間 較容易選擇(?)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    int length = 10;// length = 回傳幾筆的參數

                    int month = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-1).Month);

                    if (month == 12)
                    {
                        month = 1;
                    }

                    int year = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Year) - 1911;

                    var invLetter = db.InvLetters.Where(l => l.AccountId == invAccount.Id && l.InvLetterStatus == InvLetterStatusType.可使用 && ((l.StartMonth >= month && l.Year == year) || (l.Year > year))).OrderBy(l => l.StartDate).Take(length).Select(l => new { l.Letter, l.Id });

                    //var invLetter = db.InvLetters.Where(l =>
                    //    l.AccountId == invAccount.Id && l.StartMonth >= month && l.Year >= year && l.InvLetterStatus == InvLetterStatusType.可使用).OrderBy(l=>l.StartMonth).Take(length).Select(l => l.Letter);

                    if (invLetter != null)
                    {
                        return Content(JsonConvert.SerializeObject(invLetter.ToList()));
                    }
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult CreadeInvLet(InvLetter invLetter)
        {
            ModelState.Remove("id");
            ModelState.Remove("initdate");
            ModelState.Remove("AccountId");
            ModelState.Remove("StartMonth");
            ModelState.Remove("InvLetterStatus");
            ModelState.Remove("StartDate");

            if (ModelState.IsValid)
            {
                if (Session["Guid"] != null)
                {
                    string Guid = Session["Guid"].ToString();
                    InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                    invLetter.AccountId = invAccount.Id;
                    invLetter.StartMonth = Convert.ToInt32(invLetter.Period) * 2 + 1;
                    invLetter.StartDate = Convert.ToDateTime($"{Convert.ToInt32(invLetter.Year)}" + "-" + $"{Convert.ToInt32(invLetter.Period) * 2 + 1}-1");
                    invLetter.Year = Convert.ToInt32(invLetter.Year) - 1911;
                    //invLetter.InitDate = DateTime.Now.AddHours(15);
                    invLetter.InvLetterStatus = InvLetterStatusType.可使用; // =0

                    db.InvLetters.Add(invLetter);
                    db.SaveChanges();
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }


        private const int DefaultPageSize = 5;

        [HttpPost]
        public ActionResult SearchInvLet(int StartYear, int StartPeriod, int EndYear, int EndPeriod, int Page)  //todo 麻煩區塔，做一個判斷，將已過期的字軌顯示已鎖定。
        {

            if (Session["Guid"] != null)
            {
                if (Page == null)
                {
                    Page = 0;
                }
                else
                {
                    Page--;
                }


                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                DateTime starTime = Convert.ToDateTime($"{Convert.ToInt32(StartYear) }-{Convert.ToInt32(StartPeriod) * 2 + 1}-1");

                DateTime EndTime = Convert.ToDateTime($"{Convert.ToInt32(EndYear) }-{Convert.ToInt32(EndPeriod) * 2 + 1}-1").AddMonths(2);

                var invLetter = db.InvLetters.Where(L => L.AccountId == invAccount.Id && L.StartDate >= starTime && L.StartDate < EndTime && (L.InvLetterStatus == InvLetterStatusType.可使用 || L.InvLetterStatus == InvLetterStatusType.已鎖定)).Select(L => new { L.Id, Year = (L.Year) + 1911, L.Period, L.StartNum, L.EndNum, L.InvLetterStatus, L.Letter }).OrderBy(L => L.Id).ToList().ToPagedList(Page, DefaultPageSize);

                return Content(JsonConvert.SerializeObject(invLetter));

            }
            return Json(false);

        }


        [HttpPost]
        public ActionResult DeleteInvLet(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    //InvClientInfo invClientInfo = db.InvClientInfos.Find(Id); // 限主索引，且連表格會一起抓，故常與JsonIgnore並用
                    InvLetter invLetter = db.InvLetters.Where(L => L.AccountId == invAccount.Id && L.Id == Id).FirstOrDefault();
                    invLetter.InvLetterStatus = InvLetterStatusType.已刪除;
                    db.Entry(invLetter).State = EntityState.Modified;
                    //db.InvClientInfos.Remove(invClientInfo);
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult LockInvLet(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    //InvClientInfo invClientInfo = db.InvClientInfos.Find(Id); // 限主索引，且連表格會一起抓，故常與JsonIgnore並用
                    InvLetter invLetter = db.InvLetters.Where(L => L.AccountId == invAccount.Id && L.Id == Id).FirstOrDefault();

                    if (invLetter.InvLetterStatus == InvLetterStatusType.可使用)
                    {
                        invLetter.InvLetterStatus = InvLetterStatusType.已鎖定;
                    }
                    else if (invLetter.InvLetterStatus == InvLetterStatusType.已鎖定)
                    {
                        invLetter.InvLetterStatus = InvLetterStatusType.可使用;
                    }

                    db.Entry(invLetter).State = EntityState.Modified;
                    //db.InvClientInfos.Remove(invClientInfo);
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }


        //public class ListInv
        //{
        //    public string YearPeriod { get; set; }
        //    public string InvNum { get; set; }
        //    public string Client { get; set; }
        //    public string Total { get; set; }
        //}


        [HttpPost]
        public ActionResult DetailInvLet(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    //InvClientInfo invClientInfo = db.InvClientInfos.Find(Id); // 限主索引，且連表格會一起抓，故常與JsonIgnore並用
                    InvLetter invLetter = db.InvLetters.Where(L => L.AccountId == invAccount.Id && L.Id == Id).FirstOrDefault();

                    var invTable = db.InvTables
                        .Where(T => T.AccountId == invAccount.Id && T.Letter == invLetter.Letter && (T.InvStatus == InvStatusType.已開立)).ToList(); // todo 需確認 已作廢 是否包含在內，個人認為不用，因這邊為上傳財政部使用，對財政部而言做廢發票始終不存在，以及 已鎖定須 "事後由前端或後端" 來判斷

                    //var invTables = db.InvTables
                    //    .Where(T => T.AccountId == invAccount.Id && T.Letter == invLetter.Letter).Select(T=>T.Letter).ToList();

                    StringBuilder stringBuilder = new StringBuilder();

                    foreach (var Inv in invTable)
                    {
                        stringBuilder.Append(Inv.Num + ",");
                    }

                    int Length = Convert.ToInt32(invLetter.EndNum) + 1 - Convert.ToInt32(invLetter.StartNum);

                    string[] listInvs = new string[Length];


                    int p = 0;
                    int r = 0;

                    JArray jsonOut = new JArray();

                    for (int i = Convert.ToInt32(invLetter.StartNum); i <= Convert.ToInt32(invLetter.EndNum); i++)
                    {
                        if (stringBuilder.ToString().IndexOf(i.ToString()) > -1)
                        {
                            JObject json = new JObject();
                            json.Add(new JProperty("YearPeriod", $"{invLetter.Year + 1911 } {(Convert.ToInt32(invLetter.Period) * 2 + 1)}-{(Convert.ToInt32(invLetter.Period) * 2 + 2)}月"));
                            json.Add(new JProperty("InvNum", invLetter.Letter + i));
                            json.Add(new JProperty("Client", invTable[p].Client));
                            json.Add(new JProperty("Total", invTable[p].Total.ToString()));
                            json.Add(new JProperty("Status", "已使用"));
                            jsonOut.Add(json);
                            p++;
                        }
                        else
                        {
                            JObject json = new JObject();
                            json.Add(new JProperty("YearPeriod", $"{invLetter.Year + 1911 } {(Convert.ToInt32(invLetter.Period) * 2 + 1)}-{(Convert.ToInt32(invLetter.Period) * 2 + 2)}月"));
                            json.Add(new JProperty("InvNum", invLetter.Letter + i));
                            json.Add(new JProperty("Client", ""));
                            json.Add(new JProperty("Total", ""));
                            json.Add(new JProperty("Status", "未使用"));
                            jsonOut.Add(json);
                        }

                        r++;
                    }

                    return Content(JsonConvert.SerializeObject(jsonOut));
                }
                return Json(false);
            }
            return Json(false);
        }





        //[HttpPost]
        //public ActionResult DisplayEditLetter(int Id)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        if (ModelState.IsValid)
        //        {
        //            var invClientInfo = db.InvClientInfos.Where(C => C.Id == Id && C.AccountId == invAccount.Id).FirstOrDefault();

        //            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(invClientInfo, Formatting.Indented);

        //            return Content(jsonContent, "application/json");
        //        }
        //    }
        //    return Json(false);
        //}

        //[HttpPost]
        //public ActionResult UpdateEditLetter(InvClientInfo invClientInfo)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        if (ModelState.IsValid)
        //        {
        //            invClientInfo.AccountId = invAccount.Id;
        //            db.Entry(invClientInfo).State = EntityState.Modified;
        //            db.SaveChanges();

        //            return Json(true);
        //        }
        //        return Json(false);
        //    }
        //    return Json(false);
        //}





        // GET: InvLetters
        public ActionResult Index()
        {
            var invLetters = db.InvLetters.Include(i => i.InvAccount);
            return View(invLetters.ToList());
        }

        // GET: InvLetters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvLetter invLetter = db.InvLetters.Find(id);
            if (invLetter == null)
            {
                return HttpNotFound();
            }
            return View(invLetter);
        }

        // GET: InvLetters/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid");
            return View();
        }

        // POST: InvLetters/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitDate,AccountId,Year,Period,StartMonth,Letter,StartNum,EndNum,InpLetterStatus")] InvLetter invLetter)
        {
            if (ModelState.IsValid)
            {
                db.InvLetters.Add(invLetter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invLetter.AccountId);
            return View(invLetter);
        }

        // GET: InvLetters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvLetter invLetter = db.InvLetters.Find(id);
            if (invLetter == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invLetter.AccountId);
            return View(invLetter);
        }

        // POST: InvLetters/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitDate,AccountId,Year,Period,StartMonth,Letter,StartNum,EndNum,InpLetterStatus")] InvLetter invLetter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invLetter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invLetter.AccountId);
            return View(invLetter);
        }

        // GET: InvLetters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvLetter invLetter = db.InvLetters.Find(id);
            if (invLetter == null)
            {
                return HttpNotFound();
            }
            return View(invLetter);
        }

        // POST: InvLetters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvLetter invLetter = db.InvLetters.Find(id);
            db.InvLetters.Remove(invLetter);
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

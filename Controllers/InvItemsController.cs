using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceAPI.Models;
using System.IO;
using Newtonsoft.Json;

namespace InvoiceAPI.Controllers
{
    public class InvItemsController : Controller
    {
        private InvoiceAPIContext db = new InvoiceAPIContext();

        //public class MyJson2
        //{
        //    //public string Guid { get; set; } // 抓 AccountID
        //    public int ClientId { get; set; }
        //    //public string Client { get; set; }
        //    public string Letter { get; set; } 
        //    public string Num { get; set; }
        //    public DateTime InvDate { get; set; }
        //    public Service[] Service { get; set; }
        //    public int Total { get; set; } // 需計算後再送入
        //}

        //public class MyJson
        //{
        //    public string Guid { get; set; }
        //    public string Letter { get; set; }
        //    public string Num { get; set; }
        //    public Service[] Service { get; set; }

        //}

        //public class Service
        //{
        //    public string Item { get; set; }
        //    public int Count { get; set; }
        //    public int Price { get; set; }
        //}

        //public ActionResult InvFinal(MyJson2 myJson)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();

        //        if (ModelState.IsValid)
        //        {
        //            InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();

        //            InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.Id == myJson.ClientId).FirstOrDefault();

        //            if (invAccount != null)
        //            {
        //                // 建立發票總表

        //                InvTable invTable = new InvTable();

        //                invTable.AccountId = invAccount.Id;

        //                invTable.ClientId = invClientInfo.Id;

        //                invTable.Client = invClientInfo.CompName;

        //                invTable.ClientAdress = invClientInfo.CompAddress;

        //                invTable.ClientPhoneNumber = invClientInfo.CompPhoneNumber;

        //                invTable.ClientUniformNum = invClientInfo.UniformNumbers;

        //                invTable.Letter = myJson.Letter;

        //                invTable.Num = myJson.Num;

        //                invTable.InvNum = $"{myJson.Letter}-{myJson.Num}";

        //                invTable.InvDate = myJson.InvDate;

        //                invTable.Total = myJson.Total;

        //                invTable.InvStatus = InvStatusType.已開立;

        //                db.InvTables.Add(invTable);

        //                db.SaveChanges();

        //                // 建立發票細項

        //                foreach (var item in myJson.Service)
        //                {
        //                    InvItem invItem = new InvItem();

        //                    invItem.Item = item.Item;

        //                    invItem.Count = item.Count;

        //                    invItem.Price = item.Price;

        //                    invItem.InvTableId = invTable.Id;

        //                    db.InvItems.Add(invItem);

        //                }
        //                db.SaveChanges();
        //                return Json(true);

        //            }
        //        }
        //        return Json(false);
        //    }
        //    return Json(false);
        //}

        //public ActionResult ItemTable(MyJson myJson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == myJson.Guid).FirstOrDefault();

        //        InvTable invTable = db.InvTables.Where(T => T.Letter == myJson.Letter && T.Num == myJson.Num).FirstOrDefault();

        //        if (invAccount != null)
        //        {
        //            //string Json = "[{'Item':'Berry','Count':18,'Price':100},{'ItemName':'Merry','Amount': 28,'Price':80}]";

        //            //MyJson[] myJson = JsonConvert.DeserializeObject<MyJson[]>(Json);

        //            InvItem invItem = new InvItem();

        //            foreach (var item in myJson.Service)
        //            {
        //                invItem.Item = item.Item;

        //                invItem.Count = item.Count;

        //                invItem.Price = item.Price;

        //                invItem.InvTableId = invTable.Id;

        //                //todo 

        //                db.InvItems.Add(invItem);

        //                db.SaveChanges();

        //            }

        //            return Json(true);

        //        }
        //    }
        //    return Json(false);
        //}


        //public ActionResult ItemTable(string Guid, MyJson[] myJson, string Letter, string Num)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        InvTable invTable = db.InvTables.Where(T => T.Letter == Letter && T.Num == Num).FirstOrDefault();

        //        if (invAccount != null)
        //        {
        //            //string Json = "[{'Item':'Berry','Count':18,'Price':100},{'ItemName':'Merry','Amount': 28,'Price':80}]";

        //            //MyJson[] myJson = JsonConvert.DeserializeObject<MyJson[]>(Json);

        //            InvItem invItem = new InvItem();

        //            foreach (var item in myJson)
        //            {
        //                invItem.Item = item.Item;

        //                invItem.Count = item.Count;

        //                invItem.Price = item.Price;

        //                invItem.InvTableId = invTable.Id;

        //                //todo 

        //                db.InvItems.Add(invItem);

        //                db.SaveChanges();

        //            }

        //            return Json(true);

        //        }
        //    }
        //    return Json(false);
        //}









        // GET: InvItems
        public ActionResult Index()
        {
            var invItems = db.InvItems.Include(i => i.InvTable);
            return View(invItems.ToList());
        }

        // GET: InvItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvItem invItem = db.InvItems.Find(id);
            if (invItem == null)
            {
                return HttpNotFound();
            }
            return View(invItem);
        }

        // GET: InvItems/Create
        public ActionResult Create()
        {
            ViewBag.InvTableId = new SelectList(db.InvTables, "Id", "Letter");
            return View();
        }

        // POST: InvItems/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitDate,InvTableId,Item,Count,Price")] InvItem invItem)
        {
            if (ModelState.IsValid)
            {
                db.InvItems.Add(invItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.InvTableId = new SelectList(db.InvTables, "Id", "Letter", invItem.InvTableId);
            return View(invItem);
        }

        // GET: InvItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvItem invItem = db.InvItems.Find(id);
            if (invItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.InvTableId = new SelectList(db.InvTables, "Id", "Letter", invItem.InvTableId);
            return View(invItem);
        }

        // POST: InvItems/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitDate,InvTableId,Item,Count,Price")] InvItem invItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.InvTableId = new SelectList(db.InvTables, "Id", "Letter", invItem.InvTableId);
            return View(invItem);
        }

        // GET: InvItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvItem invItem = db.InvItems.Find(id);
            if (invItem == null)
            {
                return HttpNotFound();
            }
            return View(invItem);
        }

        // POST: InvItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvItem invItem = db.InvItems.Find(id);
            db.InvItems.Remove(invItem);
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

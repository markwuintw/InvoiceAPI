using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InvoiceAPI.Models;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using MvcPaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvoiceAPI.Controllers
{
    public class InvTablesController : Controller
    {
        private InvoiceAPIContext db = new InvoiceAPIContext();

        [HttpPost]
        public ActionResult SelectInvNm(string Letter) //todo 是否改吃 LetterId(?)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    int length = 10;// length = 回傳幾筆的參數

                    var invLetter = db.InvLetters.Where(l =>
                        l.AccountId == invAccount.Id && l.Letter == Letter && l.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

                    //var invLetter = db.InvLetters.Where(l =>
                    //    l.AccountId == invAccount.Id && l.Id == Id && l.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

                    //int a = Convert.ToInt32(invLetter.StartNum);

                    List<string> invNumbersList = new List<string>(); //這邊List只能是基本型別，如 int,string...

                    //JObject json = new JObject();

                    StringBuilder stringBuilder = new StringBuilder();

                    var invTable = db.InvTables.Where(t => t.AccountId == invAccount.Id && t.Letter == Letter && t.InvStatus == InvStatusType.已開立).ToList();

                    //var invTable = db.InvTables.Where(t => t.AccountId == invAccount.Id && t.Letter == invLetter.Letter && t.InvStatus == InvStatusType.已開立).ToList();


                    foreach (var item in invTable)
                    {
                        stringBuilder.Append(item.Num + ",");
                    }


                    for (int i = Convert.ToInt32(invLetter.StartNum); i < Convert.ToInt32(invLetter.EndNum) + 1; i++)
                    {
                        if (stringBuilder.ToString().IndexOf(i.ToString() + ",") > -1)
                        {

                        }
                        else
                        {
                            invNumbersList.Add(i.ToString());
                        }

                    }
                    //json.Add(new JProperty("invNumber", invNumbersList));

                    var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(invNumbersList, Formatting.Indented);
                    return Content(jsonContent, "application/json");

                }
                return Json(false);
            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult SelectInvDt(string Letter, int Num)  //todo 是否改吃 LetterId(?)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    var invLetter = db.InvLetters.Where(l => l.AccountId == l.InvAccount.Id && l.Letter == Letter && l.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

                    string starTime = $"{(invLetter.Year) + 1911}-{Convert.ToInt32(invLetter.StartMonth)}-01";

                    DateTime starTime0 = Convert.ToDateTime(starTime);

                    // 前後一張發票的日期區間

                    var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Letter == Letter && T.InvStatus == InvStatusType.已開立).ToList();

                    DateTime LowerLimit = Convert.ToDateTime(invTable.Where(T => Convert.ToInt32(T.Num) < Num).OrderByDescending(T => T.InvDate).Select(T => T.InvDate).FirstOrDefault());

                    DateTime UpperLimit = Convert.ToDateTime(invTable.Where(T => Convert.ToInt32(T.Num) > Num).OrderBy(T => T.InvDate).Select(T => T.InvDate).FirstOrDefault());

                    if (LowerLimit == new DateTime())
                    {
                        LowerLimit = starTime0;
                    }

                    if (UpperLimit == new DateTime())
                    {
                        UpperLimit = starTime0.AddMonths(2).AddDays(-1);
                    }


                    List<DateTime> InvDateRange = new List<DateTime>(); //這邊List只能是基本型別，如 int,string...

                    InvDateRange.Add(LowerLimit);

                    InvDateRange.Add(UpperLimit);

                    var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(InvDateRange, Formatting.Indented);
                    return Content(jsonContent, "application/json");

                }
                return Json(false);
            }
            return Json(false);
        }


        //[HttpPost]
        //public ActionResult SelectInvDt(string Letter)  //todo 是否改吃 LetterId(?)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();

        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        if (invAccount != null)
        //        {
        //            var invLetter = db.InvLetters.Where(l => l.AccountId == l.InvAccount.Id && l.Letter == Letter && l.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

        //            string starTime = $"{(invLetter.Year) + 1911}-{Convert.ToInt32(invLetter.StartMonth)}-01";

        //            DateTime starTime0 = Convert.ToDateTime(starTime);

        //            List<DateTime> InvDateRange = new List<DateTime>(); //這邊List只能是基本型別，如 int,string...

        //            InvDateRange.Add(starTime0);

        //            InvDateRange.Add(starTime0.AddMonths(2));

        //            var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(InvDateRange, Formatting.Indented);
        //            return Content(jsonContent, "application/json");

        //        }
        //        return Json(false);
        //    }
        //    return Json(false);
        //}

        public class MyJson
        {
            public int TableId { get; set; }
            public int ClientId { get; set; }
            public string Letter { get; set; }
            public string Num { get; set; }
            public DateTime InvDate { get; set; }
            public Service[] Service { get; set; }
            //public int Total { get; set; } // 需計算後再送入
        }


        public class Service
        {
            public int Id { get; set; }
            public string Item { get; set; }
            public int Count { get; set; }
            public int Price { get; set; }
        }

        //public ActionResult InvFinal(MyJson myJson)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == myJson.Guid).FirstOrDefault();

        //        if (invAccount != null)
        //        {
        //            // 建立發票總表

        //            InvTable invTable = new InvTable();

        //            invTable.AccountId = invAccount.Id;

        //            invTable.Client = myJson.Client;

        //            invTable.Letter = myJson.Letter;

        //            invTable.Num = myJson.Num;

        //            invTable.ImvNum = $"{myJson.Letter}-{myJson.Num}";

        //            invTable.InvDate = myJson.InvDate;

        //            invTable.Total = myJson.Total;

        //            invTable.InpStatus = InvStatusType.已開立;

        //            db.InvTables.Add(invTable);

        //            db.SaveChanges();

        //            // 建立發票細項

        //            foreach (var item in myJson.Service)
        //            {
        //                InvItem invItem = new InvItem();

        //                invItem.Item = item.Item;

        //                invItem.Count = item.Count;

        //                invItem.Price = item.Price;

        //                invItem.InvTableId = invTable.Id;

        //                db.InvItems.Add(invItem);

        //            }
        //            db.SaveChanges();
        //            return Json(true);

        //        }
        //    }
        //    return Json(false);
        //}


        //public ActionResult InvFinal(string FormData)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();

        //        if (ModelState.IsValid)
        //        {
        //            MyJson myJson = JsonConvert.DeserializeObject<MyJson>(FormData);

        //            InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();

        //            if (invAccount != null)
        //            {
        //                // 建立發票總表

        //                InvTable invTable = new InvTable();

        //                invTable.AccountId = invAccount.Id;

        //                invTable.Client = myJson.Client;

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




        public ActionResult FinishInv(MyJson myJson) //todo 區塔那邊計算總合回傳
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                if (ModelState.IsValid)
                {
                    InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();

                    InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.Id == myJson.ClientId).FirstOrDefault();

                    InvLetter invLetter = db.InvLetters.Where(L => L.Letter == myJson.Letter && L.AccountId == invAccount.Id && L.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

                    InvTable invTableNull = db.InvTables.Where(T => T.Num == myJson.Num).FirstOrDefault();

                    if (invAccount != null && invLetter != null && invTableNull == null && invClientInfo != null)
                    {
                        // 建立發票總表

                        InvTable invTable = new InvTable();

                        invTable.AccountId = invAccount.Id;

                        invTable.ClientId = invClientInfo.Id;

                        invTable.Client = invClientInfo.CompName;

                        invTable.ClientAdress = invClientInfo.CompAddress;

                        invTable.ClientPhoneNumber = invClientInfo.CompPhoneNumber;

                        invTable.ClientUniformNum = invClientInfo.UniformNumbers;

                        invTable.Letter = myJson.Letter;

                        invTable.Num = myJson.Num;

                        invTable.InvNum = $"{myJson.Letter}{myJson.Num}";

                        invTable.InvDate = myJson.InvDate;

                        invTable.Total = 0;

                        //invTable.InitDate = DateTime.Now.AddHours(15);

                        //invTable.Total = myJson.Total; 

                        invTable.InvStatus = InvStatusType.已開立;

                        db.InvTables.Add(invTable);

                        db.SaveChanges();

                        // 建立發票細項

                        int Dollars = 0;

                        foreach (var item in myJson.Service)
                        {
                            InvItem invItem = new InvItem();

                            invItem.Item = item.Item;

                            invItem.Count = item.Count;

                            invItem.Price = item.Price;

                            invItem.InvTableId = invTable.Id;

                            //invItem.InitDate = DateTime.Now.AddHours(15);

                            Dollars += item.Count * item.Price;

                            db.InvItems.Add(invItem);

                        }
                        db.SaveChanges();

                        InvTable invTable0 = db.InvTables.Where(T => T.Id == invTable.Id).FirstOrDefault();

                        invTable0.Total = Dollars;

                        db.Entry(invTable0).State = EntityState.Modified;
                        db.SaveChanges();

                        return Json(true);

                    }
                    return Json(false);

                }
                return Json(false);
            }
            return Json(false);
        }



        //public ActionResult SearchInvInfo( DateTime? StarTime, DateTime? EndTime, String Term ,int Page)
        //{

        //    if (Session["Guid"] != null)
        //    {
        //        if (Page == null) //因為第一頁不回傳，第二頁回傳2，但系統判斷是從0開始。
        //        {
        //            Page = 0;
        //        }
        //        else
        //        {
        //            Page = Page - 1;
        //        }


        //        string Guid = Session["Guid"].ToString();
        //        InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();


        //        if (Term != "" && (StarTime != null && EndTime != null))
        //        {
        //            //todo dateTime? 型別不能用AddDays()

        //            DateTime NewStarTime = Convert.ToDateTime(StarTime);
        //            DateTime NewEndTime = Convert.ToDateTime(EndTime).AddDays(1);

        //            var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id && 
        //                                                   T.InvDate >= NewStarTime && 
        //                                                   T.InvDate < NewEndTime &&
        //                                                   T.Client.Contains(Term)
        //                ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Client, T.Total, T.InvStatus,  }).ToList().OrderByDescending(T => T.Id).ToPagedList(Page, DefaultPageSize);

        //            // todo 想做到 InpStatus 依當下時間呈現不同的效果，已鎖定

        //            return Content(JsonConvert.SerializeObject(invTable));
        //        }
        //        else if (Term != "")
        //        {
        //            var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id &&
        //                                                   T.Client.Contains(Term) 
        //                ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Client, T.Total, T.InvStatus }).ToList().OrderByDescending(T => T.Id).ToPagedList(Page, DefaultPageSize);

        //            return Content(JsonConvert.SerializeObject(invTable));
        //        }
        //        else if (StarTime != null && EndTime != null)
        //        {
        //            DateTime NewStarTime = Convert.ToDateTime(StarTime);
        //            DateTime NewEndTime = Convert.ToDateTime(EndTime).AddDays(1);

        //            var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id &&
        //                                                   T.InvDate >= NewStarTime && 
        //                                                   T.InvDate < NewEndTime
        //            ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Client, T.Total, T.InvStatus }).ToList().OrderByDescending(T => T.Id).ToPagedList(Page, DefaultPageSize);

        //            return Content(JsonConvert.SerializeObject(invTable));
        //        }
        //        else
        //        {
        //            var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id 
        //            ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Client, T.Total, T.InvStatus }).ToList().OrderByDescending(T => T.Id).ToPagedList(Page, DefaultPageSize);

        //            return Content(JsonConvert.SerializeObject(invTable));

        //        }

        //        return Json(false);

        //    }

        //    return Json(false);
        //}


        //public  ActionResult ChangeInvStatus()
        //{

        //    int ExpireYear = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-2).Year);

        //    int ExpireMonth = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-2).Month);

        //    int ExpireDay = Convert.ToInt32(DateTime.UtcNow.AddHours(8).AddMonths(-2).Day);

        //    if (ExpireDay == 1)
        //    {
        //        InvLetter invLetter = db.InvLetters.Where(L =>L.Year+1911 == ExpireYear && L.StartMonth == ExpireMonth && L.InvLetterStatus == InvLetterStatusType.可使用).FirstOrDefault();

        //        invLetter.InvLetterStatus = InvLetterStatusType.已鎖定;


        //        InvTable invTable = db.InvTables.Where(T => T.Letter == invLetter.Letter && T.InvStatus == InvStatusType.已開立).FirstOrDefault();

        //        invTable.InvStatus = InvStatusType.已鎖定;

        //        db.Entry(invLetter).State = EntityState.Modified;
        //        db.SaveChanges();

        //        db.Entry(invTable).State = EntityState.Modified;
        //        db.SaveChanges();

        //        return Json("True");

        //    }
        //    return Json("False");

        //}


        public ActionResult ChangeInvStatus(DateTime now)
        {

            int ExpireYear = Convert.ToInt32(now.AddMonths(-2).Year);

            int ExpireMonth = Convert.ToInt32(now.AddMonths(-2).Month);

            int ExpireDay = Convert.ToInt32(now.AddMonths(-2).Day);

            if (ExpireDay == 1)
            {
                var invLetter = db.InvLetters.Where(L => L.Year + 1911 == ExpireYear && L.StartMonth == ExpireMonth && L.InvLetterStatus == InvLetterStatusType.可使用).ToList();

                if (invLetter != null)
                {
                    foreach (InvLetter invLetters in invLetter)
                    {
                        invLetters.InvLetterStatus = InvLetterStatusType.已鎖定;

                        db.Entry(invLetters).State = EntityState.Modified;

                        var invTable = db.InvTables.Where(T => T.Letter == invLetters.Letter && T.InvStatus == InvStatusType.已開立).ToList();

                        foreach (InvTable invTables in invTable)
                        {
                            invTables.InvStatus = InvStatusType.已鎖定;

                            db.Entry(invTables).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                        db.SaveChanges();
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);  // 允許使用 GET

                }
                return Json(false, JsonRequestBehavior.AllowGet);  // 允許使用 GET

            }
            return Json(false, JsonRequestBehavior.AllowGet);  // 允許使用 GET

        }




        private const int DefaultPageSize = 10;


        public ActionResult SearchInvInfo(DateTime? StarTime, DateTime? EndTime, String Term, int Page)
        {

            if (Session["Guid"] != null)
            {
                if (Page == null) //因為第一頁不回傳，第二頁回傳2，但系統判斷是從0開始。
                {
                    Page = 0;
                }
                else
                {
                    Page = Page - 1;
                }


                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();

                JArray jsonOut = new JArray();

                int r = 0;

                if (Term != "" && (StarTime != null && EndTime != null))
                {
                    //todo dateTime? 型別不能用AddDays()

                    DateTime NewStarTime = Convert.ToDateTime(StarTime);
                    DateTime NewEndTime = Convert.ToDateTime(EndTime).AddDays(1);

                    var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id &&
                                                           T.InvDate >= NewStarTime &&
                                                           T.InvDate < NewEndTime &&
                                                           T.Client.Contains(Term)
                        ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Num, T.Client, T.Total, T.InvStatus, T.Letter }).ToList();


                    var invTablePage = invTable.OrderByDescending(T => T.Num).ToPagedList(Page, DefaultPageSize);

                    foreach (var Inv in invTablePage)
                    {
                        JObject json = new JObject();
                        json.Add(new JProperty("Total", invTable.Count));
                        json.Add(new JProperty("Id", Inv.Id));
                        json.Add(new JProperty("InvDate", Inv.InvDate));
                        json.Add(new JProperty("InvNum", Inv.InvNum));
                        json.Add(new JProperty("Client", Inv.Client));
                        json.Add(new JProperty("Money", Inv.Total));


                        var invLetter = db.InvLetters.Where(L => L.Letter == Inv.Letter).Select(L => L.StartDate).FirstOrDefault();

                        if (Inv.InvStatus == InvStatusType.已作廢)
                        {
                            json.Add(new JProperty("InvStatus", Inv.InvStatus));
                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(8) >= invLetter.AddMonths(2))
                            {
                                json.Add(new JProperty("InvStatus", InvStatusType.已鎖定));
                            }
                            else
                            {
                                json.Add(new JProperty("InvStatus", Inv.InvStatus));
                            }
                        }

                        r++;
                        jsonOut.Add(json);
                    }

                    // todo 想做到 InpStatus 依當下時間呈現不同的效果，已鎖定

                    return Content(JsonConvert.SerializeObject(jsonOut));
                }
                else if (Term != "")
                {
                    var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Client.Contains(Term))
                        .Select(T => new { T.Id, T.InvDate, T.InvNum, T.Num, T.Client, T.Total, T.InvStatus, T.Letter }).ToList();

                    var invTablePage = invTable.OrderByDescending(T => T.Num).ToPagedList(Page, DefaultPageSize);

                    foreach (var Inv in invTablePage)
                    {
                        JObject json = new JObject();
                        json.Add(new JProperty("Total", invTable.Count));
                        json.Add(new JProperty("Id", Inv.Id));
                        json.Add(new JProperty("InvDate", Inv.InvDate));
                        json.Add(new JProperty("InvNum", Inv.InvNum));
                        json.Add(new JProperty("Client", Inv.Client));
                        json.Add(new JProperty("Money", Inv.Total));


                        var invLetter = db.InvLetters.Where(L => L.Letter == Inv.Letter).Select(L => L.StartDate).FirstOrDefault();

                        if (Inv.InvStatus == InvStatusType.已作廢)
                        {
                            json.Add(new JProperty("InvStatus", Inv.InvStatus));
                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(8) >= invLetter.AddMonths(2))
                            {
                                json.Add(new JProperty("InvStatus", InvStatusType.已鎖定));
                            }
                            else
                            {
                                json.Add(new JProperty("InvStatus", Inv.InvStatus));
                            }
                        }

                        r++;
                        jsonOut.Add(json);
                    }

                    return Content(JsonConvert.SerializeObject(jsonOut));
                }
                else if (StarTime != null && EndTime != null)
                {
                    DateTime NewStarTime = Convert.ToDateTime(StarTime);
                    DateTime NewEndTime = Convert.ToDateTime(EndTime).AddDays(1);

                    var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id &&
                                                           T.InvDate >= NewStarTime &&
                                                           T.InvDate < NewEndTime
                    ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Num, T.Client, T.Total, T.InvStatus, T.Letter }).ToList();

                    var invTablePage = invTable.OrderByDescending(T => T.Num).ToPagedList(Page, DefaultPageSize);

                    foreach (var Inv in invTablePage)
                    {
                        JObject json = new JObject();
                        json.Add(new JProperty("Total", invTable.Count));
                        json.Add(new JProperty("Id", Inv.Id));
                        json.Add(new JProperty("InvDate", Inv.InvDate));
                        json.Add(new JProperty("InvNum", Inv.InvNum));
                        json.Add(new JProperty("Client", Inv.Client));
                        json.Add(new JProperty("Money", Inv.Total));


                        var invLetter = db.InvLetters.Where(L => L.Letter == Inv.Letter).Select(L => L.StartDate).FirstOrDefault();

                        if (Inv.InvStatus == InvStatusType.已作廢)
                        {
                            json.Add(new JProperty("InvStatus", Inv.InvStatus));
                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(8) >= invLetter.AddMonths(2))
                            {
                                json.Add(new JProperty("InvStatus", InvStatusType.已鎖定));
                            }
                            else
                            {
                                json.Add(new JProperty("InvStatus", Inv.InvStatus));
                            }
                        }

                        r++;
                        jsonOut.Add(json);
                    }

                    return Content(JsonConvert.SerializeObject(jsonOut));
                }
                else
                {
                    var invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id
                    ).Select(T => new { T.Id, T.InvDate, T.InvNum, T.Num, T.Client, T.Total, T.InvStatus, T.Letter }).ToList();

                    var invTablePage = invTable.OrderByDescending(T => T.Num).ToPagedList(Page, DefaultPageSize);

                    foreach (var Inv in invTablePage)
                    {
                        JObject json = new JObject();
                        json.Add(new JProperty("Total", invTable.Count));
                        json.Add(new JProperty("Id", Inv.Id));
                        json.Add(new JProperty("InvDate", Inv.InvDate));
                        json.Add(new JProperty("InvNum", Inv.InvNum));
                        json.Add(new JProperty("Client", Inv.Client));
                        json.Add(new JProperty("Money", Inv.Total));


                        var invLetter = db.InvLetters.Where(L => L.Letter == Inv.Letter).Select(L => L.StartDate).FirstOrDefault();

                        if (Inv.InvStatus == InvStatusType.已作廢)
                        {
                            json.Add(new JProperty("InvStatus", Inv.InvStatus));
                        }
                        else
                        {
                            if (DateTime.UtcNow.AddHours(8) >= invLetter.AddMonths(2))
                            {
                                json.Add(new JProperty("InvStatus", InvStatusType.已鎖定));
                            }
                            else
                            {
                                json.Add(new JProperty("InvStatus", Inv.InvStatus));
                            }
                        }

                        r++;
                        jsonOut.Add(json);
                    }

                    return Content(JsonConvert.SerializeObject(jsonOut));

                }

                return Json(false);

            }

            return Json(false);
        }


        [HttpPost]
        public ActionResult LoadDelInv(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    var invTable = db.InvTables.Where(T => T.Id == Id && T.AccountId == invAccount.Id && T.InvStatus == InvStatusType.已開立).Select(T => new
                    {
                        T.InvAccount.UniformNumbers,
                        T.InvAccount.CompName,
                        T.InvAccount.CompPhoneNumber,
                        T.InvAccount.CompAddress,
                        T.Client,
                        T.ClientUniformNum,
                        T.ClientPhoneNumber,
                        T.ClientAdress,
                        T.Letter,
                        T.Num,
                        T.InvDate,
                        T.Id,
                    }).FirstOrDefault();

                    return Content(JsonConvert.SerializeObject(invTable));
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult UpdateDelInv(int Id, string DropReason)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    InvTable invTable = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Id == Id && T.InvStatus == InvStatusType.已開立).FirstOrDefault();
                    invTable.InvStatus = InvStatusType.已作廢;
                    invTable.DropReason = DropReason;
                    invTable.DropTime = DateTime.UtcNow.AddHours(8);
                    db.Entry(invTable).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult LoadInvEd(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();


                if (ModelState.IsValid)
                {
                    var invTable = db.InvTables.Where(T => T.Id == Id && T.AccountId == invAccount.Id && T.InvStatus == InvStatusType.已開立).Select(T => new
                    {
                        T.InvAccount.UniformNumbers,
                        T.InvAccount.CompName,
                        T.InvAccount.CompPhoneNumber,
                        T.InvAccount.CompAddress,
                        T.Client,
                        T.ClientUniformNum,
                        T.ClientPhoneNumber,
                        T.ClientAdress,
                        T.Letter,
                        T.Num,
                        T.InvDate,
                        T.Id,
                        T.InvItems
                    }).FirstOrDefault();

                    return Content(JsonConvert.SerializeObject(invTable));

                }
                return Json(false);
            }
            return Json(false);
        }




        //[HttpPost]
        //public ActionResult EditInvoice1(int Id)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        InvTable invTable = db.InvTables.Where(T => T.Id == Id).FirstOrDefault();

        //        InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.Id == invTable.ClientId).FirstOrDefault();

        //        if (ModelState.IsValid)
        //        {
        //            //todo 接字串


        //            JObject json = new JObject();

        //            json.Add(new JProperty("AccountUN", invAccount.UniformNumbers));
        //            json.Add(new JProperty("AccountCN", invAccount.CompName));
        //            json.Add(new JProperty("AccountCA", invAccount.CompAddress));
        //            json.Add(new JProperty("AccountCP", invAccount.CompPhoneNumber));
        //            json.Add(new JProperty("ClientUN", invClientInfo.UniformNumbers));
        //            json.Add(new JProperty("ClientCN", invClientInfo.CompName));
        //            json.Add(new JProperty("ClientCA", invClientInfo.CompAddress));
        //            json.Add(new JProperty("ClientCP", invClientInfo.CompPhoneNumber));
        //            json.Add(new JProperty("Letter", invTable.Letter));
        //            json.Add(new JProperty("Num", invTable.Num));
        //            json.Add(new JProperty("Date", invTable.InvDate));

        //            return Content(JsonConvert.SerializeObject(json));

        //        }
        //        return Json(false);
        //    }
        //    return Json(false);
        //}


        //[HttpPost]
        //public ActionResult EditInvoice2(int Id)
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();
        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        InvTable invTable = db.InvTables.Where(T => T.Id == Id).FirstOrDefault();

        //        InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.Id == invTable.ClientId).FirstOrDefault();

        //        var invItem = db.InvItems.Where(I => I.InvTableId == invTable.Id).Select(I => new { I.Item, I.Count, I.Price });

        //        if (ModelState.IsValid)
        //        {
        //            return Content(JsonConvert.SerializeObject(invItem));
        //        }
        //        return Json(false);
        //    }
        //    return Json(false);
        //}



        public ActionResult UpdateInvEd(MyJson myJson)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                if (ModelState.IsValid)
                {
                    InvAccount invAccount = db.InvAccounts.Where(A => A.Guid == Guid).FirstOrDefault();

                    InvClientInfo invClientInfo = db.InvClientInfos.Where(C => C.Id == myJson.ClientId).FirstOrDefault();


                    if (invAccount != null)
                    {
                        // 建立發票總表

                        InvTable invTable = db.InvTables.Where(T => T.Id == myJson.TableId && T.AccountId == invAccount.Id && T.InvStatus == InvStatusType.已開立).FirstOrDefault();

                        invTable.AccountId = invAccount.Id;

                        invTable.ClientId = invClientInfo.Id;

                        invTable.Client = invClientInfo.CompName;

                        invTable.Letter = myJson.Letter;

                        invTable.Num = myJson.Num;

                        invTable.InvNum = $"{myJson.Letter}{myJson.Num}";

                        invTable.InvDate = myJson.InvDate;

                        invTable.Total = 0;

                        invTable.InvStatus = InvStatusType.已開立;

                        db.Entry(invTable).State = EntityState.Modified;
                        db.SaveChanges();

                        //db.InvTables.Add(invTable);

                        //db.SaveChanges();

                        // 建立發票細項

                        int Dallars = 0;

                        foreach (var item in myJson.Service)
                        {
                            InvItem invItem = db.InvItems.Where(I => I.Id == item.Id).FirstOrDefault();

                            invItem.Item = item.Item;

                            invItem.Count = item.Count;

                            invItem.Price = item.Price;

                            invItem.InvTableId = invTable.Id;

                            Dallars += item.Count * item.Price;

                            db.Entry(invItem).State = EntityState.Modified;

                        }
                        db.SaveChanges();

                        InvTable invTable0 = db.InvTables.Where(T => T.Id == invTable.Id).FirstOrDefault();

                        invTable0.Total = Dallars;

                        db.Entry(invTable0).State = EntityState.Modified;
                        db.SaveChanges();

                        return Json(true);

                    }
                }
                return Json(false);
            }
            return Json(false);
        }



        [HttpPost]
        public ActionResult CopyInvInfo(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();


                if (ModelState.IsValid)
                {
                    var invTable = db.InvTables.Where(T => T.Id == Id && T.AccountId == invAccount.Id).Select(T => new
                    {
                        T.InvAccount.UniformNumbers,
                        T.InvAccount.CompName,
                        T.InvAccount.CompPhoneNumber,
                        T.InvAccount.CompAddress,
                        T.Client,
                        T.ClientUniformNum,
                        T.ClientPhoneNumber,
                        T.ClientAdress,
                        T.InvItems
                    }).FirstOrDefault();

                    return Content(JsonConvert.SerializeObject(invTable));

                }
                return Json(false);
            }
            return Json(false);
        }


        public class UsingInv
        {
            public string UniformNumbers { get; set; }
            public string TypeInv { get; set; }
            public string Letter { get; set; }
            public string[] UnusingNum { get; set; }

        }



        [HttpPost]
        public ActionResult Pdf(int Id) // todo  輸入 Letter 或 LetterId
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    InvLetter invLetter = db.InvLetters.Where(L => L.Id == Id && L.AccountId == invAccount.Id).FirstOrDefault();

                    if (invLetter != null)
                    {
                        var invTables = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Letter == invLetter.Letter && T.InvStatus == InvStatusType.已開立).Select(T => T.Num).ToList();
                        StringBuilder stringBuilder = new StringBuilder();


                        foreach (var invNum in invTables)
                        {
                            stringBuilder.Append(invNum + ",");
                        }

                        //StringBuilder List = new StringBuilder();

                        List<string> List = new List<string>(); //這邊List只能是基本型別，如 int,string...


                        for (int i = Convert.ToInt32(invLetter.StartNum); i <= Convert.ToInt32(invLetter.EndNum); i++)
                        {
                            if (stringBuilder.ToString().IndexOf(i.ToString()) == -1)
                            {
                                List.Add(i.ToString());
                            }
                        }

                        StringBuilder InvUsing = new StringBuilder();

                        foreach (var invNum in List)
                        {
                            int forward = Convert.ToInt32(invNum) - 1;
                            int behind = Convert.ToInt32(invNum) + 1;

                            if (!List.Contains(forward.ToString()))
                            {
                                InvUsing.Append(invNum + ",");
                            }
                            if (!List.Contains(behind.ToString()))
                            {
                                InvUsing.Append(invNum + ",");
                            }
                        }

                        string ss = InvUsing.ToString();

                        int dd = ss.Length;

                        ss = ss.Remove(dd - 1, 1);

                        //InvUsing.ToString().Remove(InvUsing.Length-1 , 1);

                        string[] Final = ss.ToString().Split(',');

                        JArray outArray = new JArray();


                        for (int i = 0; i < Final.Length; i += 2)
                        {
                            JObject json = new JObject();

                            json.Add(new JProperty("UniformNumbers", invAccount.UniformNumbers));

                            json.Add(new JProperty("TypeInv", "一般稅額計算"));

                            json.Add(new JProperty("Letter", invLetter.Letter));

                            json.Add(new JProperty("Low", Final[i]));

                            json.Add(new JProperty("High", Final[i + 1]));

                            outArray.Add(json);

                        }
                        return Content(JsonConvert.SerializeObject(outArray));

                    }

                    return Json(false);
                }
                return Json(false);
            }
            return Json(false);

        }



        //[HttpPost]
        //public ActionResult Pdf(int Id) // todo  輸入 Letter 或 LetterId
        //{
        //    if (Session["Guid"] != null)
        //    {
        //        string Guid = Session["Guid"].ToString();

        //        InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

        //        if (invAccount!=null)
        //        {
        //            InvLetter invLetter = db.InvLetters.Where(L => L.Id == Id).FirstOrDefault();
        //            var invTables = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Letter == invLetter.Letter && T.InvStatus == InvStatusType.已開立).Select(T => T.Num).ToList();
        //            StringBuilder stringBuilder = new StringBuilder();


        //            foreach (var invNum in invTables)
        //            {
        //                stringBuilder.Append(invNum + ",");
        //            }

        //            //StringBuilder List = new StringBuilder();

        //            List<string> List = new List<string>(); //這邊List只能是基本型別，如 int,string...


        //            for (int i = Convert.ToInt32(invLetter.StartNum); i <= Convert.ToInt32(invLetter.EndNum); i++)
        //            {
        //                if (stringBuilder.ToString().IndexOf(i.ToString()) == -1)
        //                {
        //                    List.Add(i.ToString());
        //                }
        //            }

        //            StringBuilder InvUsing = new StringBuilder();

        //            foreach (var invNum in List)
        //            {
        //                int forward = Convert.ToInt32(invNum) - 1;
        //                int behind = Convert.ToInt32(invNum) + 1;

        //                if (!List.Contains(forward.ToString()))
        //                {
        //                    InvUsing.Append(invNum + ",");
        //                }
        //                if (!List.Contains(behind.ToString()))
        //                {
        //                    InvUsing.Append(invNum + ",");
        //                }
        //            }

        //            string ss = InvUsing.ToString();

        //            int dd = ss.Length;

        //            ss = ss.Remove(dd - 1, 1);

        //            //InvUsing.ToString().Remove(InvUsing.Length-1 , 1);

        //            string[] Final = ss.ToString().Split(',');

        //            UsingInv usingInv = new UsingInv();

        //            usingInv.UniformNumbers = invAccount.UniformNumbers;
        //            usingInv.TypeInv = "一般稅額計算";
        //            usingInv.Letter = invLetter.Letter;
        //            usingInv.UnusingNum = Final;


        //            return Content(JsonConvert.SerializeObject(usingInv));



        //        }
        //        return Json(false);
        //    }
        //    return Json(false);

        //}


        [HttpPost]
        public ActionResult Csv(int Id)
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();
                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    InvLetter invLetter = db.InvLetters.Where(L => L.Id == Id && L.AccountId == invAccount.Id).FirstOrDefault();
                    var invTables = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Letter == invLetter.Letter && T.InvStatus == InvStatusType.已開立).ToList();

                    //var invItem = db.InvItems.Where(I => I.InvTableId == invTables.).ToList();
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine(
                        $"H,{invAccount.UniformNumbers},{invAccount.CompName},{invAccount.CompAddress},{invAccount.CompPhoneNumber}");
                    foreach (var Inv in invTables)
                    {
                        //stringBuilder.AppendLine(
                        //    $"M,{Inv.InvNum},{Inv.InvDate},07,{Inv.ClientUniformNum},{Inv.Client},{Inv.ClientAdress},1,5,{Math.Round(Inv.Total/1.05)},{Inv.Total- Math.Round(Inv.Total / 1.05)},{Inv.Total},1");

                        stringBuilder.AppendLine(
                            $"M,{Inv.InvNum},{Inv.InvDate},07,{Inv.ClientUniformNum},{Inv.Client},{Inv.ClientAdress},1,5,{Inv.Total},{Math.Round(Inv.Total * 0.05)},{Math.Round(Inv.Total * 1.05)},1");


                        var invItem = db.InvItems.Where(I => I.InvTableId == Inv.Id).ToList();

                        foreach (var item in invItem)
                        {
                            //stringBuilder.AppendLine($"D,{item.Item},{item.Count},{Math.Round(item.Price / 1.05)},{Math.Round(item.Price / 1.05)* item.Count},");
                            stringBuilder.AppendLine($"D,{item.Item},{item.Count},{item.Price},{item.Price * item.Count}");

                        }
                    }
                    byte[] OutputContent = new System.Text.UTF8Encoding().GetBytes(stringBuilder.ToString());

                    return File(OutputContent, "text/csv", $"申報發票CSV-{DateTime.UtcNow.AddHours(8).ToString()}");

                    //return Content(stringBuilder.ToString());

                }
                return Json(false);

            }

            return Json(false);


        }

        [HttpPost]
        public ActionResult LandingPage(int month)
        {
            if (month == 0)
            {
                JObject jObject = new JObject();

                var invAccount = db.InvAccounts.Count();

                jObject.Add(new JProperty("AcAmount", invAccount));

                var invClientInfo = db.InvClientInfos.Count();

                jObject.Add(new JProperty("CliAmount", invClientInfo));

                var invTable = db.InvTables.Select(T => T.Total).ToList();

                int TotalSum = 0;
                foreach (var total in invTable)
                {
                    TotalSum += total;
                }

                jObject.Add(new JProperty("TotalSum", TotalSum));

                var invLetter = db.InvLetters.Count();

                jObject.Add(new JProperty("LetAmount", invLetter));

                return Content(JsonConvert.SerializeObject(jObject));

            }
            else
            {
                DateTime Now = DateTime.UtcNow.AddHours(8);
                DateTime SearchRange = Now.AddMonths(-month);


                JObject jObject = new JObject();

                var invAccount = db.InvAccounts.Where(I => I.InitDate > SearchRange).Count();

                jObject.Add(new JProperty("AcAmount", invAccount));

                var invClientInfo = db.InvClientInfos.Count();

                jObject.Add(new JProperty("CliAmount", invClientInfo));

                var invTable = db.InvTables.Where(I => I.InitDate > SearchRange).Select(T => T.Total).ToList();

                int TotalSum = 0;
                foreach (var total in invTable)
                {
                    TotalSum += total;
                }

                jObject.Add(new JProperty("TotalSum", TotalSum));

                var invLetter = db.InvLetters.Where(I => I.InitDate > SearchRange).Count();

                jObject.Add(new JProperty("LetAmount", invLetter));

                return Content(JsonConvert.SerializeObject(jObject));

            }

        }


        //[HttpPost]
        //public ActionResult Adf( string Letter )
        //{
        //    string Guid = Session["Guid"].ToString();
        //    InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();
        //    var invTables = db.InvTables.Where(T => T.AccountId == invAccount.Id && T.Letter == Letter).Select(T => T.Num).ToList();
        //    StringBuilder stringBuilder = new StringBuilder();

        //    foreach (var invNum in invTables)
        //    {
        //        int forward = Convert.ToInt32(invNum) - 1;
        //        int behind = Convert.ToInt32(invNum) + 1;

        //        if (!invTables.Contains(forward.ToString()))
        //        {
        //            stringBuilder.Append(invNum + ",");
        //        }
        //        if (!invTables.Contains(behind.ToString()))
        //        {
        //            stringBuilder.Append(invNum + ",");
        //        }
        //    }

        //    return Content(stringBuilder.ToString());
        //}





        // GET: InvTables
        public ActionResult Index()
        {
            var invTables = db.InvTables.Include(i => i.InvAccount);
            return View(invTables.ToList());
        }

        // GET: InvTables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvTable invTable = db.InvTables.Find(id);
            if (invTable == null)
            {
                return HttpNotFound();
            }
            return View(invTable);
        }

        // GET: InvTables/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid");
            return View();
        }

        // POST: InvTables/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitDate,AccountId,Letter,Num,ImpNum,InpDate,Client,Total,InpStatus,DropTime,DropReason")] InvTable invTable)
        {
            if (ModelState.IsValid)
            {
                db.InvTables.Add(invTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invTable.AccountId);
            return View(invTable);
        }

        // GET: InvTables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvTable invTable = db.InvTables.Find(id);
            if (invTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invTable.AccountId);
            return View(invTable);
        }

        // POST: InvTables/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitDate,AccountId,Letter,Num,ImpNum,InpDate,Client,Total,InpStatus,DropTime,DropReason")] InvTable invTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.InvAccounts, "Id", "Guid", invTable.AccountId);
            return View(invTable);
        }

        // GET: InvTables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvTable invTable = db.InvTables.Find(id);
            if (invTable == null)
            {
                return HttpNotFound();
            }
            return View(invTable);
        }

        // POST: InvTables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvTable invTable = db.InvTables.Find(id);
            db.InvTables.Remove(invTable);
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

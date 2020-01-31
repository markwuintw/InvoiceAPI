using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using InvoiceAPI.Models;

namespace InvoiceAPI.Controllers
{
    public class InvAccountsController : Controller
    {
        private InvoiceAPIContext db = new InvoiceAPIContext();

        // 登入API
        [HttpPost]
        public ActionResult Login(string UniformNumbers, string Password)
        {
            //if (Session["verification"] == null)
            //{
            //    return Content("機器人認證未通過(Session)");
            //}

            ////token失敗就擋掉
            //bool verification = bool.Parse(Session["verification"].ToString());

            //if (verification != true)
            //{
            //    //context.Response.Write("機器人認證未通過");
            //    return Content("機器人認證未通過(verification)");
            //}

            InvAccount invAccount = db.InvAccounts.Where(a => a.UniformNumbers == UniformNumbers).FirstOrDefault();

            if (invAccount.verif == 0)
            {
                return Content("信箱尚未認證");
            }

            if (ModelState.IsValid)
            {
                string userPwd = Password;

                string strSalt = invAccount.Salt;

                //SHA256加密
                byte[] pwdAndSalt = Encoding.UTF8.GetBytes(userPwd + strSalt);
                byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
                string hashStr = Convert.ToBase64String(hashBytes);

                if (hashStr == invAccount.Password)
                {
                    Session["Guid"] = invAccount.Guid;

                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            return Json(false);
        }

        public ActionResult CheckSession()
        {
            if (Session["Guid"] != null)
            {
                return Json(true);
            }
            return Json(false);
        }


        public ActionResult SignOut()
        {
            if (Session["Guid"] != null)
            {
                Session.Abandon();
                return Json(true);
            }
            return Json(false);
        }

        //void SetAuthenTicket(string userData, string userId)
        //{
        //    //宣告一個驗證票
        //    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userId, DateTime.Now, DateTime.Now.AddHours(3), false, userData);
        //    //加密驗證票
        //    string encryptedTicket = FormsAuthentication.Encrypt(ticket);
        //    //建立Cookie
        //    HttpCookie authenticationcookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        //    //將Cookie寫入回應
        //    Response.Cookies.Add(authenticationcookie);
        //}


        //  todo 機器人認證


        public class ResponseToken
        {
            public DateTime challenge_ts { get; set; }
            public float score { get; set; }
            public string action { get; set; }
            public bool success { get; set; }
            public string hostname { get; set; }
        }

        public ActionResult IsRobot(string hiddenToken)
        {
            string token = hiddenToken;
            string tokenContent = PostJsonContent(token);
            //取得隱藏欄位token
            ResponseToken responseToken = JsonConvert.DeserializeObject<ResponseToken>(tokenContent);
            Session["verification"] = responseToken.success;

            if (responseToken.success != true)
            {
                return Content("機器人認證失敗(IsRobot)");
            }
            return Content("success");
        }

        private static string PostJsonContent(string token)
        {
            string key = "6LfdW8kUAAAAABTrd42uQ_IQ277q9XweqO0LTDVG"; //用這串密鑰來建立網站和 reCAPTCHA 之間的通訊

            try
            {
                WebRequest request = HttpWebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
                request.Method = "POST";
                //使用 application/x-www-form-urlencoded
                request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

                //要傳送的資料內容(依字串表示)
                string postData = $"secret=6LfdW8kUAAAAABTrd42uQ_IQ277q9XweqO0LTDVG&response={token}";
                //將傳送的字串轉為 byte array
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                //告訴 server content 的長度
                request.ContentLength = byteArray.Length;
                //將 byte array 寫到 request stream 中 
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }
                using (var httpResponse = (HttpWebResponse)request.GetResponse())
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }

            catch (Exception)
            {
                return string.Empty;
            }
        }

        [HttpPost]
        public ActionResult CreadeAcInfo([Bind(Include = "CompName,UniformNumbers,Password,MngrEmail,MngrPhoneNumber,CompAddress,CompPhoneNumber,MngrName")] InvAccount invAccount)
        {
            ModelState.Remove("Id");
            ModelState.Remove("InitDate");
            ModelState.Remove("Guid");
            ModelState.Remove("Salt");
            ModelState.Remove("verif");

            //if (Session["verification"]== null)
            //{
            //    return Content("機器人認證未通過");
            //}

            ////token失敗就擋掉
            //bool verification = bool.Parse(Session["verification"].ToString());

            //if (verification != true)
            //{
            //    //context.Response.Write("機器人認證未通過");
            //    return Content("機器人認證未通過");
            //}

            if (ModelState.IsValid)
            {
                invAccount.Guid = Guid.NewGuid().ToString();

                invAccount.Salt = Guid.NewGuid().ToString();

                string userPwd = invAccount.Password;

                string strSalt = invAccount.Salt;

                //SHA256加密
                byte[] pwdAndSalt = Encoding.UTF8.GetBytes(userPwd + strSalt);
                byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
                string hashStr = Convert.ToBase64String(hashBytes);

                invAccount.Password = hashStr;

                invAccount.verif = 0; //信箱未認證

                //invAccount.InitDate = DateTime.Now.AddHours(15);

                db.InvAccounts.Add(invAccount);
                db.SaveChanges();

                StringBuilder content = new StringBuilder();
                content.AppendLine("安安,我們的電子發票小幫手系統中，收到您申請帳號的請求，該帳號若為您本人所使用，請點選下方連結進行確認，謝謝您的使用");
                content.AppendLine(
                    $@"<a href=""invoice.rocket-coding.com/InvAccounts/CheckAcEm?Guid={invAccount.Guid}""><h1>請按偶進行確認</h1></a>");

                //todo post Guid 到 驗證網站，該網址頁面待處理

                GmailTest1(content, invAccount.MngrEmail);

                return Json(true);
            }
            return Json(false);
        }

        //[HttpPost]
        //public ActionResult CreadeAcInfo([Bind(Include = "CompName,UniformNumbers,Password,MngrEmail,MngrPhoneNumber,CompAddress,CompPhoneNumber,MngrName")] InvAccount invAccount)
        //{
        //    ModelState.Remove("id");
        //    ModelState.Remove("initdate");
        //    ModelState.Remove("guid");
        //    ModelState.Remove("salt");
        //    ModelState.Remove("verif");

        //    if (ModelState.IsValid)
        //    {
        //        invAccount.Guid = Guid.NewGuid().ToString();

        //        invAccount.Salt = Guid.NewGuid().ToString();

        //        string userPwd = invAccount.Password;

        //        string strSalt = invAccount.Salt;

        //        //SHA256加密
        //        byte[] pwdAndSalt = Encoding.UTF8.GetBytes(userPwd + strSalt);
        //        byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
        //        string hashStr = Convert.ToBase64String(hashBytes);

        //        invAccount.Password = hashStr;

        //        invAccount.verif = 0; //信箱未認證

        //        //invAccount.InitDate = DateTime.Now.AddHours(15);

        //        db.InvAccounts.Add(invAccount);
        //        db.SaveChanges();

        //        StringBuilder content = new StringBuilder();
        //        content.AppendLine("安安,我們的電子發票小幫手系統中，收到您申請帳號的請求，該帳號若為您本人所使用，請點選下方連結進行確認，謝謝");
        //        content.AppendLine(
        //            $@"<a href=""invoice.rocket-coding.com/InvAccounts/CheckAcEm?Guid={invAccount.Guid}""><h1>請按偶進行確認</h1></a>");

        //        //todo post Guid 到 驗證網站，該網址頁面待處理

        //        GmailTest(content, invAccount.MngrEmail);

        //        return Json(true);
        //    }
        //    return Json(false);
        //}


        [HttpPost]
        public ActionResult CheckAcUn(string UniformNumbers)
        {
            if (ModelState.IsValid)
            {
                InvAccount invAccount = db.InvAccounts.Where(a => a.UniformNumbers == UniformNumbers).FirstOrDefault();

                //InvAccount invAccount = db.InvAccounts.Find(UniformNumbers);

                if (invAccount != null)
                {
                    return Json(true); // 帳號已存在，回傳 True
                }

                return Json(false); // 帳號不存在，回傳 False

            }
            return Json(false);
        }


        // GET
        public ActionResult CheckAcEm(string Guid)
        {
            if (ModelState.IsValid)
            {
                InvAccount invAccount = db.InvAccounts.Where(a => a.Guid == Guid).FirstOrDefault();

                invAccount.verif = AccountVerifType.已認證;

                db.Entry(invAccount).State = EntityState.Modified;
                db.SaveChanges();

                return Redirect("https://invoice.rocket-coding.com/registerSuccess.html");

                //return Json(true, JsonRequestBehavior.AllowGet);  // 允許使用 GET
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadAcEd()
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                //var invAccount = db.InvAccounts.Where(a => a.Guid == Guid).Select(a => new { a.CompName, a.UniformNumbers, a.CompAddress, a.CompPhoneNumber, a.MngrName, a.MngrPhoneNumber, a.MngrEmail }).FirstOrDefault();

                InvAccount invAccount = db.InvAccounts.Where(a => a.Guid == Guid).FirstOrDefault();

                var jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(invAccount, Formatting.Indented); //Formatting.Indented生成良好的显示格式，可读性更好  //Formatting.None会跳过不必要的空格和换行符

                return Content(jsonContent, "application/json"); // 資料庫內容全部傳至前端後，更新時在全部傳回來

            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult UpdateAcEd(InvAccount invAccount)  // 資料庫內容全部傳至前端後，更新時在全部傳回來
        {
            if (Session["Guid"] != null)
            {
                if (ModelState.IsValid)
                {
                    invAccount.Salt = Guid.NewGuid().ToString(); // 帳號資料更新時，密碼鹽亦重新建立，但 GUID 不變

                    string userPwd = invAccount.Password;

                    string strSalt = invAccount.Salt;

                    //SHA256加密
                    byte[] pwdAndSalt = Encoding.UTF8.GetBytes(userPwd + strSalt);
                    byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
                    string hashStr = Convert.ToBase64String(hashBytes);
                    invAccount.Password = hashStr;

                    db.Entry(invAccount).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult ForgetAcPw(string UniformNumbers, string MngrEmail)
        {
            InvAccount invAccount = db.InvAccounts.Where(a => a.UniformNumbers == UniformNumbers).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (invAccount != null)
                {
                    if (MngrEmail == invAccount.MngrEmail)
                    {
                        StringBuilder content = new StringBuilder();
                        content.AppendLine("安安,電子發票小幫手系統收到您密碼遺失的訊息，請點選下方連結，進行重新密碼設定");
                        content.AppendLine($@"<a href=""https://tw.yahoo.com?Guid={invAccount.Guid}""><h1>請按偶進行確認</h1></a>");
                        //todo post Guid 到 驗證網站(區塔的網頁)
                        GmailTest2(content, invAccount.MngrEmail);
                        return Json(true);
                    }
                    else
                    {
                        return Json(false);
                    }
                }
                return Json(false);

            }
            return Json(false);
        }


        [HttpPost]
        public ActionResult ResetAcPw(string Guid, string Password)
        {
            if (ModelState.IsValid)
            {
                InvAccount invAccount = db.InvAccounts.Where(a => a.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    invAccount.Salt = System.Guid.NewGuid().ToString();

                    string userPwd = Password;

                    string strSalt = invAccount.Salt;

                    //SHA256加密
                    byte[] pwdAndSalt = Encoding.UTF8.GetBytes(userPwd + strSalt);
                    byte[] hashBytes = new SHA256Managed().ComputeHash(pwdAndSalt);
                    string hashStr = Convert.ToBase64String(hashBytes);

                    invAccount.Password = hashStr;

                    db.Entry(invAccount).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(true);
                }

                return Json(false);

            }
            return Json(false);
        }

        public static bool GmailTest1(StringBuilder content, string Email)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress("mask.mark.bra@gmail.com", "電子發票小幫手系統訊息", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                mail.To.Add(Email);//可以發送給多人
                //mail.Bcc.Add("maxwuintw1989@gmail.com");//可以抄送副本給多人 
                mail.Subject = $"電子發票小幫手申請帳號確認信-{DateTime.UtcNow.AddHours(8).ToString("yyyy/M/d HH:mm:ss.fff")}";//郵件標題
                mail.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼

                mail.Body = $"{content}";//郵件內容
                mail.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                mail.IsBodyHtml = true;//是否是HTML郵件 

                //mail.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                mail.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("maxwuintw1989@gmail.com", "lumia925"); //todo 寄信信箱帳號密碼
                //client.Credentials = new System.Net.NetworkCredential("rocketcodingiscool@gmail.com", "codingiscool"); //todo 寄信信箱帳號密碼

                //GMAIL固定..................................................

                client.Host = "smtp.gmail.com";//設定smtp Server
                client.Port = 587;//設定Port
                client.EnableSsl = true;//配合gmail預設開啟驗證

                //GMAIL固定..................................................

                client.Send(mail);//寄出信件
                client.Dispose();//釋放記憶體
                return true;

            }
            catch
            {
                return false;
            }

        }

        public static bool GmailTest2(StringBuilder content, string Email)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new MailAddress("mask.mark.bra@gmail.com", "電子發票小幫手系統訊息", System.Text.Encoding.UTF8);
                /* 上面3個參數分別是發件人地址（可以隨便寫），發件人姓名，編碼*/
                mail.To.Add(Email);//可以發送給多人
                //mail.Bcc.Add("maxwuintw1989@gmail.com");//可以抄送副本給多人 
                mail.Subject = $"電子發票小幫手密碼遺失申請-{DateTime.UtcNow.AddHours(8).ToString("yyyy/M/d HH:mm:ss.fff")}";//郵件標題
                mail.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼

                mail.Body = $"{content}";//郵件內容
                mail.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
                mail.IsBodyHtml = true;//是否是HTML郵件 

                //mail.Attachments.Add(new Attachment(@"D:\test2.docx"));  //附件
                mail.Priority = MailPriority.High;//郵件優先級 

                SmtpClient client = new SmtpClient();
                client.Credentials = new System.Net.NetworkCredential("maxwuintw1989@gmail.com", "lumia925"); //todo 寄信信箱帳號密碼
                //client.Credentials = new System.Net.NetworkCredential("rocketcodingiscool@gmail.com", "codingiscool"); //todo 寄信信箱帳號密碼

                //GMAIL固定..................................................

                client.Host = "smtp.gmail.com";//設定smtp Server
                client.Port = 587;//設定Port
                client.EnableSsl = true;//配合gmail預設開啟驗證

                //GMAIL固定..................................................

                client.Send(mail);//寄出信件
                client.Dispose();//釋放記憶體
                return true;

            }
            catch
            {
                return false;
            }

        }


        [HttpPost]
        public ActionResult LoadCliInfo()  // LoadingAcInfo  發票開立頁的買方資料載入
        {
            if (Session["Guid"] != null)
            {
                string Guid = Session["Guid"].ToString();

                InvAccount invAccount = db.InvAccounts.Where(x => x.Guid == Guid).FirstOrDefault();

                if (invAccount != null)
                {
                    return Content(JsonConvert.SerializeObject(db.InvAccounts.Where(x => x.Guid == Guid).Select(x => new { x.CompName, x.CompAddress, x.CompPhoneNumber, x.UniformNumbers }).ToList()));
                }

                return Json(false);
            }
            return Json(false);
        }




        // 預設開啟頁面導至 Area 的 Admin 用
        public ActionResult login()
        {
            return Redirect("https://invoice.rocket-coding.com/index.html");
        }



        // GET: InvAccounts
        public ActionResult Index()
        {
            return View(db.InvAccounts.ToList());
        }

        // GET: InvAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvAccount invAccount = db.InvAccounts.Find(id);
            if (invAccount == null)
            {
                return HttpNotFound();
            }
            return View(invAccount);
        }

        // GET: InvAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvAccounts/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,InitDate,Guid,CompName,UniformNumbers,Password,Salt,MngrEmail,MngrPhoneNumber,CompAddress,CompPhoneNumber,verif")] InvAccount invAccount)
        {
            if (ModelState.IsValid)
            {
                db.InvAccounts.Add(invAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invAccount);
        }

        // GET: InvAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvAccount invAccount = db.InvAccounts.Find(id);
            if (invAccount == null)
            {
                return HttpNotFound();
            }
            return View(invAccount);
        }

        // POST: InvAccounts/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,InitDate,Guid,CompName,UniformNumbers,Password,Salt,MngrEmail,MngrPhoneNumber,CompAddress,CompPhoneNumber,verif")] InvAccount invAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invAccount);
        }

        // GET: InvAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvAccount invAccount = db.InvAccounts.Find(id);
            if (invAccount == null)
            {
                return HttpNotFound();
            }
            return View(invAccount);
        }

        // POST: InvAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvAccount invAccount = db.InvAccounts.Find(id);
            db.InvAccounts.Remove(invAccount);
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

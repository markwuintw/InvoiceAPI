using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceAPI.Models
{
    public enum EnableType //原 public class EnumList
    {
        關,
        開
    }

    public enum BooleanType //原 public class EnumList
    {
        否,
        是
    }

    public enum GenderType //原 public class EnumList
    {
        女,
        男
    }

    public enum ManagerType //原 public class EnumList
    {
        資訊管理版
    }

    public enum AccountVerifType //原 public class EnumList
    {
        未認證,
        已認證
    }

    public enum ClientStatusType //原 public class EnumList
    {
        已建立,
        已刪除
    }

    public enum InvLetterStatusType //原 public class EnumList
    {
        可使用,
        已鎖定,
        已刪除
    }

    public enum InvStatusType //原 public class EnumList
    {
        已開立,
        已作廢,
        已鎖定,
        未使用
    }

    public enum PeriodType //原 public class EnumList
    {
        [System.ComponentModel.Description("01-02")]
        一二月,
        [System.ComponentModel.Description("03-04")]
        三四月,
        [System.ComponentModel.Description("05-06")]
        五六月,
        [System.ComponentModel.Description("07-08")]
        七八月,
        [System.ComponentModel.Description("09-10")]
        九十月,
        [System.ComponentModel.Description("11-12")]
        十一十二月,
    }
}
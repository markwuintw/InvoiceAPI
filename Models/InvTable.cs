using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace InvoiceAPI.Models
{
    public class InvTable
    {
        [Key] //主索引，需 Int
        [Required] //必填
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //自動產生編號
        public int Id { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")] //HTML 日曆只吃yyyy-MM-dd格式
        [Display(Name = "建立日期")] //顯示的名稱，但在DB及程式碼仍用英文
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //該Computed選項指定屬性值將在首次保存時由數據庫生成，並在每次更新值時重新生成。這樣做的實際效果是，實體框架不會在INSERT或UPDATE語句中包括該屬性，而是在檢索時從數據庫中獲取計算值。
        //需 using System.ComponentModel.DataAnnotations.Schema，但 EF5 Code First 並不支援設定預設值，而此舉的意義為讓 EF 不再追蹤此屬性的任何物件變化
        public DateTime? InitDate { get; set; } //資料庫要同步改為非必填


        //..................................

        [JsonIgnore]
        [Required] //必填
        public int AccountId { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountId")]
        public virtual InvAccount InvAccount { get; set; }

        //..................................

        [Required] //必填
        [Display(Name = "字軌")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Letter { get; set; }


        [Required] //必填
        [Display(Name = "號碼")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Num { get; set; }


        [Required] //必填
        [Display(Name = "發票號碼")] //顯示的名稱，但在DB及程式碼仍用英文
        public string InvNum { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")] //HTML 日曆只吃yyyy-MM-dd格式
        [Display(Name = "發票日期")] //顯示的名稱，但在DB及程式碼仍用英文
        //該Computed選項指定屬性值將在首次保存時由數據庫生成，並在每次更新值時重新生成。這樣做的實際效果是，實體框架不會在INSERT或UPDATE語句中包括該屬性，而是在檢索時從數據庫中獲取計算值。
        //需 using System.ComponentModel.DataAnnotations.Schema，但 EF5 Code First 並不支援設定預設值，而此舉的意義為讓 EF 不再追蹤此屬性的任何物件變化
        public DateTime? InvDate { get; set; } //資料庫要同步改為非必填


        [Required] //必填
        [Display(Name = "ClientId")] //顯示的名稱，但在DB及程式碼仍用英文
        public int ClientId { get; set; }

        [Required] //必填
        [Display(Name = "買方名稱")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Client { get; set; }

        [Required] //必填
        [Display(Name = "買方地址")] //顯示的名稱，但在DB及程式碼仍用英文
        public string ClientUniformNum { get; set; }

        [Required] //必填
        [Display(Name = "買方地址")] //顯示的名稱，但在DB及程式碼仍用英文
        public string ClientAdress { get; set; }

        [Required] //必填
        [Display(Name = "買方電話")] //顯示的名稱，但在DB及程式碼仍用英文
        public string ClientPhoneNumber { get; set; }

        [Required] //必填
        [Display(Name = "發票金額")] //顯示的名稱，但在DB及程式碼仍用英文
        public int Total { get; set; }

        [Required] //必填
        [Display(Name = "發票狀態")] //顯示的名稱，但在DB及程式碼仍用英文
        public InvStatusType InvStatus { get; set; }

        [Display(Name = "作廢時間")] //顯示的名稱，但在DB及程式碼仍用英文
        public DateTime? DropTime { get; set; }

        [Display(Name = "作廢原因")] //顯示的名稱，但在DB及程式碼仍用英文
        public string DropReason { get; set; }

        public virtual ICollection<InvItem> InvItems { get; set; }  //virtual虛擬 ICollection集合代表很多筆 這類別有很多AboutLink(放父層)

    }
}
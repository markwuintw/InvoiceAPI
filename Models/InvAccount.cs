using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace InvoiceAPI.Models
{
    public class InvAccount
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


        [Required] //必填
        [Display(Name = "GUID")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Guid { get; set; }

        [Required(ErrorMessage = "{0}必填")] //必填
        [MaxLength(50)] //限定字元
        [Display(Name = "企業名稱")] //顯示的名稱，但在DB及程式碼仍用英文
        public string CompName { get; set; }


        [Required(ErrorMessage = "{0}必填")] //必填
        [Display(Name = "統一編號")] //顯示的名稱，但在DB及程式碼仍用英文
        [MaxLength(10)] //限定字元
        //[Remote("CheckAccount", "Login",HttpMethod = "POST",ErrorMessage = "此帳號已被申請過")]
        public string UniformNumbers { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Password { get; set; }


        [Required]
        [Display(Name = "密碼鹽")] //顯示的名稱，但在DB及程式碼仍用英文
        public string Salt { get; set; }


        [Required(ErrorMessage = "{0}必填")] //必填
        [EmailAddress(ErrorMessage = "{0} 格式錯誤")]
        [MaxLength(200)] //限定字元
        [DataType(DataType.EmailAddress)]
        [Display(Name = "管理者電子信箱")] //顯示的名稱，但在DB及程式碼仍用英文
        public string MngrEmail { get; set; }

        [Required(ErrorMessage = "{0}必填")] //必填
        [MaxLength(50)] //限定字元
        [Display(Name = "管理者名稱")] //顯示的名稱，但在DB及程式碼仍用英文
        public string MngrName { get; set; }

        [MaxLength(200)] //限定字元
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "管理者手機")] //顯示的名稱，但在DB及程式碼仍用英文
        public string MngrPhoneNumber { get; set; }

        [Required]
        [Display(Name = "公司地址")] //顯示的名稱，但在DB及程式碼仍用英文
        public string CompAddress { get; set; }

        [Required]
        [Display(Name = "公司電話")] //顯示的名稱，但在DB及程式碼仍用英文
        public string CompPhoneNumber { get; set; }

        [Required]
        [Display(Name = "認證狀態")] //顯示的名稱，但在DB及程式碼仍用英文
        public AccountVerifType verif { get; set; }

        [JsonIgnore]
        public virtual ICollection<InvClientInfo> InvClientInfos { get; set; }  //virtual虛擬 ICollection集合代表很多筆 這類別有很多AboutLink(放父層)

        [JsonIgnore]
        public virtual ICollection<InvLetter> InvLetters { get; set; }  //virtual虛擬 ICollection集合代表很多筆 這類別有很多AboutLink(放父層)

        [JsonIgnore]
        public virtual ICollection<InvTable> InvTables { get; set; }  //virtual虛擬 ICollection集合代表很多筆 這類別有很多AboutLink(放父層)

    }
}
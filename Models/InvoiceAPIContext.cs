namespace InvoiceAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class InvoiceAPIContext : DbContext
    {
        // 您的內容已設定為使用應用程式組態檔 (App.config 或 Web.config)
        // 中的 'InvoiceAPIContext' 連接字串。根據預設，這個連接字串的目標是
        // 您的 LocalDb 執行個體上的 'InvoiceAPI.Models.InvoiceAPIContext' 資料庫。
        // 
        // 如果您的目標是其他資料庫和 (或) 提供者，請修改
        // 應用程式組態檔中的 'InvoiceAPIContext' 連接字串。
        public InvoiceAPIContext()
            : base("name=InvoiceAPIContext")
        {
        }

        // 針對您要包含在模型中的每種實體類型新增 DbSet。如需有關設定和使用
        // Code First 模型的詳細資訊，請參閱 http://go.microsoft.com/fwlink/?LinkId=390109。

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<InvAccount> InvAccounts { get; set; } //加入DbContext
        public virtual DbSet<InvClientInfo> InvClientInfos { get; set; } //加入DbContext
        public virtual DbSet<InvLetter> InvLetters { get; set; } //加入DbContext
        public virtual DbSet<InvTable> InvTables { get; set; } //加入DbContext
        public virtual DbSet<InvItem> InvItems { get; set; } //加入DbContext
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}
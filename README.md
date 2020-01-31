# InvoiceAPI

	Controller名稱	API 功能	Action 名稱	註解
  
InvAccounts	

 	 登入	Login	
  
	確認登入狀態	CheckSession	
	
	登出	SignOut	
	
	新增帳號	CreadeAcInfo	
	
	帳號統編重複	CheckAcUn	
	
	信箱驗證(GET)*	CheckAcEm	
	
	忘記密碼	ForgetAcPw	
	
	忘記密碼-重設	ResetAcPw	
	
	帳號資訊-讀取	LoadAcEd	
	
	帳號資訊-編輯	UpdateAcEd	
	
	載入賣家資訊	LoadCliInfo	
	
	recaptcha v3*	IsRobot	
  
InvClientInfoes

  	新增顧客資訊	CreadeCliInfo	
	買家統編重複	CheckCliUn	
	查詢顧客資訊	SearchCliInfo	每頁10筆
	顯示顧客編輯	LoadCliEd	
	更新顧客編輯	UpdateCliEd	
	刪除顧客資訊	DeleteCliInfo	
	自動完成買家公司名(空字串全部)	AutoCliCnAll	(""為全部)
	自動完成買家統編	AutoCliUn	
  
InvLetters
  
  	新增票軌	CreadeInvLet	
	查詢票軌	SearchInvLet	每頁10筆
	鎖定/解鎖票軌	LockInvLet	
	刪除票軌	DeleteInvLet	
	票軌明細	DetailInvLet	
  
InvTables

  	查詢發票	SearchInvInfo	每頁10筆
	顯示作廢發票	LoadDelInv	
	新增作廢發票+原因時間	UpdateDelInv	
	顯示發票編輯	LoadInvEd	
	更新發票編輯	UpdateInvEd	
	複製發票編輯	CopyInvInfo	
	選擇票軌	SelectInvLet	
	選擇發票號碼	SelectInvNm	
	選擇發票日期(上下限)*	SelectInvDt	
	開立發票+細項新增	FinishInv	
	申報未使用發票區間*	Pdf	
	申報整月發票之CSV	Csv	
	過期票軌及發票鎖定判定(伺服器排程)(GET)*	ChangeInvStatus	
	累積用戶數累積/客戶數/累積開立金額/累積字軌數)	LandingPage	幾月內新增資料(0為全部)

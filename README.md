# 祥雲智慧銷售整合系統 (Xiang Yun Smart Sales Integration System)

一套專為中小型電腦零售業者設計的輕量級 ERP 系統，旨在整合商品、庫存、報價、訂單與顧客管理，終結 Excel 惡夢，並為未來的電商擴充打下穩固基礎。

## 專案背景

本專案源自於一家真實的電腦零售門市「小祥電腦店」。該店面臨著商品種類繁多、價格變動快、庫存管理不易、報價流程繁瑣、且缺乏系統化顧客資料等典型零售業痛點。為解決這些問題，並為未來的線上線下整合 (OMO) 佈局，此專案應運而生。

## ✨ 核心功能 (Key Features)

* **📦 專業級混合式庫存管理 (Professional Hybrid Inventory)**
    * 同時支援「序號化商品」(如主機) 與「批量商品」(如零件) 的管理。
    * 獨創的「批號管理」模型，能獨立追蹤每一批次的進貨成本與售價，並從根本上解決了資料庫唯一約束問題。
    * 完整的庫存異動紀錄 (`InventoryMovement`)，確保每一筆交易都有跡可循。

* **📄 自動化報價訂單流程 (Automated Quote-to-Order)**
    * 提供動態商品搜尋功能，業務員可快速建立報價單。
    * 支援一鍵將報價單轉換為正式訂單，並**自動、即時地扣除相應庫存**。
    * 提供報價單「作廢」功能，保留歷史紀錄而非物理刪除。

* **🧩 結構化商品目錄 (Structured Product Catalog)**
    * 採用 `分類 -> 型號 -> 規格 -> 具體商品` 的四層結構，讓商品資料管理極具彈性與擴充性。
    * 在進貨、報價等所有環節，智慧顯示帶有完整規格的產品名稱，大幅提升操作效率與準確性。

* **👥 顧客與員工權限管理 (CRM & Role Management)**
    * 整合業界標準的 **ASP.NET Core Identity** 框架，提供安全可靠的帳號管理。
    * 實現了基於角色的權限控制 (RBAC)，區分「Admin (管理員)」與「Salesperson (業務員)」的操作權限。
    * 建立了客戶主檔，可查詢每一位客戶的歷史訂單紀錄。

* **📊 即時儀表板 (Real-time Dashboard)**
    * 提供銷售數據的即時總覽，幫助管理者快速掌握營運狀況。

## 🏛️ 系統架構 (System Architecture)

本專案採用了專業、穩健的**分層式架構 (Layered Architecture)**，確保了系統的低耦合與高內聚，使其易於維護與未來擴充。

**流程示意圖:**
`[使用者] -> [Web UI (ASP.NET MVC)] -> [服務層 (Services)] -> [資料層 (EF Core)] -> [資料庫 (SQL Server)]`

* **展示層 (Web UI)**：Controller 極度輕量化，只負責接收請求與回傳畫面。
* **服務層 (Service Layer)**：封裝了所有核心商業邏輯，如訂單處理、庫存計算、檔案上傳等。
* **資料層 (Data Layer)**：透過 Entity Framework Core 進行資料庫的存取與映射。

## 🛠️ 技術棧 (Technology Stack)

* **後端**: C# 12, .NET 8
* **框架**: ASP.NET Core MVC, Entity Framework Core 8
* **資料庫**: Microsoft SQL Server
* **安全性**: ASP.NET Core Identity (帳號、角色、權限管理)
* **前端**: HTML, CSS, JavaScript, Bootstrap 5
* **核心模式**: Service Layer, ViewModel (VM), Repository Pattern (via DbContext)

在開始之前，請確保您的系統已安裝以下軟體：

* **.NET 8 SDK**
* **Visual Studio 2022**: 建議使用包含 "ASP.NET and web development" 工作負載的版本。
* **Microsoft SQL Server**: LocalDB (通常隨 Visual Studio 安裝)、Express 或 Developer Edition 皆可。

### ⚙️ 設定與執行步驟 (Setup and Run)

1.  **Clone (複製) 專案庫**
    在您的終端機或命令提示字元中，執行以下指令：
    ```bash
    git clone [https://github.com/marksman456/Project.git](https://github.com/marksman456/Project.git)
    ```

2.  **設定資料庫連線**
    * 在 Visual Studio 中打開 `Project.sln` 方案檔。
    * 找到並打開主專案 (`Project`) 中的 `appsettings.json` 檔案。
    * 在 `ConnectionStrings` 區塊，將 `DefaultConnection` 的值，修改為您本機 SQL Server 的連線字串。

    **範例:**
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=XiangYunDb_Dev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    }
    ```
    * *提示：如果您使用 LocalDB，`Server` 名稱通常是 `(localdb)\\mssqllocaldb`。如果您使用 SQL Server Express，則可能是 `.\\SQLEXPRESS`。`Database` 名稱可以自訂。*

3.  **執行資料庫遷移 (Migration)**
    * 在 Visual Studio 頂部選單，選擇 `工具 (Tools)` -> `NuGet 套件管理器 (NuGet Package Manager)` -> `套件管理器主控台 (Package Manager Console)`。
    * 確認「預設專案」的下拉選單，選擇為 **`ProjectData`**。
    * 在主控台中，執行以下指令來建立資料庫與所有資料表：
        ```powershell
        Update-Database -Context XiangYunDbContext
        ```

4. **建立初始管理者帳號 (V1.0 手動步驟)**
   
   執行專案
   
   按下 F5 或點擊 ▶ 按鈕來執行您的專案。
   
   註冊第一個帳號
   
   在開啟的網站頁面，點擊右上角的「註冊 (Register)」。
   
   建立您的第一個使用者帳號，例如 admin@example.com，並設定密碼。
   
   打開資料庫管理工具
   
   註冊成功後，請打開您的 SQL Server 資料庫管理工具 (如 SSMS)，並找到您專案的資料庫。
   
   尋找並複製 IDs
   
  * a. 尋找 UserId:
   打開 dbo.AspNetUsers 資料表，找到您剛剛註冊的 admin@example.com 帳號，並將其 Id (一長串的 GUID 字串) 複製下來。
   
  *  b. 尋找 RoleId:
   打開 dbo.AspNetRoles 資料表。
   
   如果裡面是空的，請手動新增兩筆資料：一筆的 Name 欄位為 "Admin"，另一筆為 "Salesperson"。
   
   然後，將 "Admin" 那一筆的 Id 複製下來。
   
   綁定使用者與角色 (完成步驟)
   
  * c. 綁定角色:
   打開 dbo.AspNetUserRoles 資料表，這是一張關聯表。
   
   在裡面新增一筆資料，並貼上您剛剛複製的兩個 Id：
   
   UserId 欄位： 貼上您 admin@example.com 帳號的 Id。
   完成！

   現在，請回到您的網站，用 admin@example.com 帳號重新登入一次。

   您應該就能存取所有需要 Admin 權限的頁面了（例如「員工管理」、「產品管理」等）。
   
   RoleId 欄位： 貼上 "Admin" 角色的 Id。

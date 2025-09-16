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

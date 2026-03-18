-- =============================================================
-- 図書管理システム　DDLスクリプト
-- 対象: SQL Server 2022
-- =============================================================

USE LibraryDB;
GO

-- ---- librarians ----
CREATE TABLE librarians (
    ID          INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    UserName    NVARCHAR(50)   NOT NULL UNIQUE,
    Name        NVARCHAR(100)  NOT NULL,
    Mail        NVARCHAR(254)  NULL     UNIQUE,
    Password    NVARCHAR(256)  NOT NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETDATE(),
    UpdatedAt   DATETIME2      NOT NULL DEFAULT GETDATE()
);
GO

-- ---- users ----
CREATE TABLE users (
    ID          INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(100)  NOT NULL,
    Gender      BIT            NOT NULL,              -- 0:男性 1:女性
    Birth       DATE           NOT NULL,
    Mail        NVARCHAR(254)  NOT NULL UNIQUE,
    Phone       NVARCHAR(20)   NOT NULL,
    Address     NVARCHAR(500)  NOT NULL,
    IsActive    BIT            NOT NULL DEFAULT 1,
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETDATE()
);
GO

CREATE NONCLUSTERED INDEX IX_users_Name ON users (Name);
GO

-- ---- books ----
CREATE TABLE books (
    ID          INT            NOT NULL IDENTITY(1,1) PRIMARY KEY,
    BookName    NVARCHAR(500)  NOT NULL,
    Author      NVARCHAR(200)  NOT NULL,
    Publisher   NVARCHAR(200)  NOT NULL,
    Genre       NVARCHAR(100)  NOT NULL,
    ISBN        NVARCHAR(20)   NOT NULL UNIQUE,
    Status      TINYINT        NOT NULL DEFAULT 0,    -- 0:在庫 1:貸出中 2:予約済 3:除籍
    IsLoaned    BIT            NOT NULL DEFAULT 0,
    IsReserved  BIT            NOT NULL DEFAULT 0
);
GO

CREATE NONCLUSTERED INDEX IX_books_BookName ON books (BookName);
CREATE NONCLUSTERED INDEX IX_books_Author   ON books (Author);
CREATE NONCLUSTERED INDEX IX_books_Status   ON books (Status, IsLoaned, IsReserved);
GO

-- ---- logs ----
CREATE TABLE logs (
    ID          BIGINT         NOT NULL IDENTITY(1,1) PRIMARY KEY,
    User_id     INT            NOT NULL REFERENCES users(ID),
    Book_id     INT            NOT NULL REFERENCES books(ID),
    LoanDate    DATETIME2      NOT NULL DEFAULT GETDATE(),
    ReturnDue   DATE           NOT NULL,
    ReturnDate  DATETIME2      NULL
);
GO

CREATE NONCLUSTERED INDEX IX_logs_User_active ON logs (User_id, ReturnDate);
CREATE NONCLUSTERED INDEX IX_logs_Book_active ON logs (Book_id, ReturnDate);
GO

-- アーカイブテーブル（月次バッチ移行先）
CREATE TABLE logs_archive (
    ID          BIGINT         NOT NULL PRIMARY KEY,
    User_id     INT            NOT NULL,
    Book_id     INT            NOT NULL,
    LoanDate    DATETIME2      NOT NULL,
    ReturnDue   DATE           NOT NULL,
    ReturnDate  DATETIME2      NULL,
    ArchivedAt  DATETIME2      NOT NULL DEFAULT GETDATE()
);
GO

-- ---- reservations ----
CREATE TABLE reservations (
    ID               INT        NOT NULL IDENTITY(1,1) PRIMARY KEY,
    User_id          INT        NOT NULL REFERENCES users(ID),
    Book_id          INT        NOT NULL REFERENCES books(ID),
    ReservationDate  DATETIME2  NOT NULL DEFAULT GETDATE(),
    Status           TINYINT    NOT NULL DEFAULT 0,    -- 0:有効 1:貸出済 2:キャンセル
    Notified         BIT        NOT NULL DEFAULT 0
);
GO

CREATE NONCLUSTERED INDEX IX_res_Book_Status ON reservations (Book_id, Status);
GO

-- アーカイブテーブル
CREATE TABLE reservations_archive (
    ID               INT        NOT NULL PRIMARY KEY,
    User_id          INT        NOT NULL,
    Book_id          INT        NOT NULL,
    ReservationDate  DATETIME2  NOT NULL,
    Status           TINYINT    NOT NULL,
    Notified         BIT        NOT NULL,
    ArchivedAt       DATETIME2  NOT NULL DEFAULT GETDATE()
);
GO

-- =============================================================
-- アプリケーション専用ユーザー（最小権限）
-- =============================================================
CREATE LOGIN library_app_user WITH PASSWORD = 'ChangeMe!Strong#2024';
GO
CREATE USER library_app_user FOR LOGIN library_app_user;
GO
GRANT SELECT, INSERT, UPDATE ON librarians    TO library_app_user;
GRANT SELECT, INSERT, UPDATE ON users         TO library_app_user;
GRANT SELECT, INSERT, UPDATE ON books         TO library_app_user;
GRANT SELECT, INSERT, UPDATE ON logs          TO library_app_user;
GRANT SELECT, INSERT, UPDATE ON reservations  TO library_app_user;
-- logs_archive / reservations_archive はバッチ専用ユーザーのみ
GO

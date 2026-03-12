# 図書管理システム　システム設計書

| 項目 | 内容 |
|------|------|
| 版数 | 1.3 |
| 作成日 | 2026/03/09 |
| 分類 | 社外秘 |

---

## 改訂履歴

| 版数 | 日付 | 変更内容 |
|------|------|----------|
| 1.0 | 2026/03/09 | 初版作成 |
| 1.1 | 2026/03/09 | 要件変更反映：貸出・返却を利用者操作に変更、完了画面追加、蔵書新規登録画面追加 |
| 1.2 | 2026/03/09 | DB要件変更：users.Gender を BIT(0:男性/1:女性) に変更、users.Mail/Phone/Address を NOT NULL 化、books.Publisher/Genre/ISBN を NOT NULL 化、logs.Librarian_id を削除 |
| 1.3 | 2026/03/09 | 要件変更反映：完了画面を単一テキスト「操作が完了しました」に簡略化、エラー・入力値エラーをポップアップ表示に統一 |

---

## 目次

1. [ドキュメント概要](#1-ドキュメント概要)
2. [システムアーキテクチャ](#2-システムアーキテクチャ)
3. [データベース設計](#3-データベース設計)
4. [クラス設計](#4-クラス設計)
5. [機能設計](#5-機能設計)
6. [画面設計](#6-画面設計)
7. [セキュリティ設計](#7-セキュリティ設計)
8. [非機能要件への対応](#8-非機能要件への対応)
9. [エラーハンドリング方針](#9-エラーハンドリング方針)
10. [開発・テスト方針](#10-開発テスト方針)

---

## 1. ドキュメント概要

### 1.1 目的

本書は図書管理システムのシステム設計書である。要件定義書に基づき、アーキテクチャ、データベース設計、画面設計、クラス設計、セキュリティ設計等を定義する。

### 1.2 対象範囲

- 司書ログイン機能
- 蔵書管理機能（一覧・検索・新規登録）
- 蔵書貸し出し機能（利用者が自己操作）
- 蔵書返却機能（利用者が自己操作）
- 蔵書予約機能
- 利用者管理機能
- 操作完了画面（全操作共通）

### 1.3 利用者区分

| 区分 | 説明 | 認証 |
|------|------|------|
| 司書 | システム管理・蔵書登録・利用者管理を行う | ログインID＋パスワード |
| 利用者 | 蔵書の貸出・返却・予約を自己操作する | 利用者ID入力（認証なし） |

> **要件変更（v1.1）**：貸出・返却操作は司書が代行する運用から、利用者が端末を直接操作する方式に変更した。

---

## 2. システムアーキテクチャ

### 2.1 技術スタック

| 区分 | 技術要素 | 備考 |
|------|----------|------|
| 開発言語 | C# 13 | .NET 10対応 |
| フレームワーク | Windows Forms (.NET 10) | クライアントGUIアプリ |
| アーキテクチャパターン | MVP（Model-View-Presenter） | テスタビリティ重視 |
| データベース | Microsoft SQL Server | MSSQL 2022推奨 |
| データアクセス | ADO.NET / Dapper | 軽量ORM |
| パスワードハッシュ | PBKDF2（.NET標準 Rfc2898DeriveBytes） | HMACSHA256、600,000回反復 |
| 認証 | セッション管理（インメモリ） | 司書ログイン状態保持 |
| バックアップ | SQL Server Agent ジョブ | 増分/フルバックアップ |

### 2.2 MVPパターン設計方針

本システムはMVP（Model-View-Presenter）パターンを採用する。各レイヤーの責務は以下の通り。

| レイヤー | 責務 | 実装 |
|----------|------|------|
| Model | ビジネスロジック・データアクセス・エンティティ定義 | Entities, Services, Repositories |
| View | UI表示のみ。ロジックを持たない。IViewインターフェースを実装 | WinForms Form クラス |
| Presenter | ViewとModelを仲介。UIイベント処理・バリデーション・表示制御 | Presenterクラス |

### 2.3 ソリューション構成

| プロジェクト名 | 種別 | 説明 |
|----------------|------|------|
| LibrarySystem.UI | WinForms App | Viewレイヤー（Formsクラス群） |
| LibrarySystem.Presenter | Class Library | Presenterレイヤー |
| LibrarySystem.Model | Class Library | Model層（Entities, Services, Repositories） |
| LibrarySystem.Infrastructure | Class Library | DBアクセス基盤、接続管理 |
| LibrarySystem.Tests | xUnit Test | ユニットテスト（Presenter/Service） |

---

## 3. データベース設計

### 3.1 設計方針

- 個人情報（usersテーブル）は貸出ログテーブルと分離し、結合キー以外の個人情報カラムへのアクセス権限を制限する。
- 蔵書数1,000万件規模に対応するため、検索頻度の高いカラムに適切なインデックスを設定する。
- 論理削除はusersテーブルの `IsActive` フラグで管理し、物理削除は行わない。
- 文字コードはSQL Server NVARCHAR（UTF-16）を使用する。

### 3.2 テーブル一覧

| テーブル名（論理） | テーブル名（物理） | 説明 |
|--------------------|-------------------|------|
| 司書テーブル | librarians | 司書アカウント情報 |
| 利用者テーブル | users | 利用者個人情報（分離管理） |
| 蔵書テーブル | books | 蔵書マスタ情報 |
| ログテーブル | logs | 貸出記録（現在・履歴） |
| 予約テーブル | reservations | 予約記録 |

### 3.3 テーブル定義

#### 3.3.1 司書テーブル（librarians）

| カラム名 | データ型 | NULL | 制約 | 説明 |
|----------|----------|------|------|------|
| ID | INT | NOT NULL | PK, IDENTITY(1,1) | 司書ID（自動採番） |
| UserName | NVARCHAR(50) | NOT NULL | UNIQUE | ログインID |
| Name | NVARCHAR(100) | NOT NULL | | 司書氏名 |
| Mail | NVARCHAR(254) | NULL | UNIQUE | メールアドレス |
| Password | NVARCHAR(256) | NOT NULL | | PBKDF2ハッシュ値（Salt埋め込み形式） |
| IsActive | BIT | NOT NULL | DEFAULT 1 | 有効フラグ |
| CreatedAt | DATETIME2 | NOT NULL | DEFAULT GETDATE() | 作成日時 |
| UpdatedAt | DATETIME2 | NOT NULL | DEFAULT GETDATE() | 更新日時 |

> `Password` はSaltをハッシュ値に埋め込んだ形式（ASP.NET Identity方式）で保存する。  
> フォーマット：`Base64( version[1byte] + salt[16byte] + hash[32byte] )`

#### 3.3.2 利用者テーブル（users）

| カラム名 | データ型 | NULL | 制約 | 説明 |
|----------|----------|------|------|------|
| ID | INT | NOT NULL | PK, IDENTITY(1,1) | 利用者ID（自動採番） |
| Name | NVARCHAR(100) | NOT NULL | | 氏名 |
| Gender | BIT | NOT NULL | | 性別（0:男性 1:女性） |
| Birth | DATE | NOT NULL | | 生年月日 |
| Mail | NVARCHAR(254) | NOT NULL | UNIQUE | メールアドレス |
| Phone | NVARCHAR(20) | NOT NULL | | 電話番号 |
| Address | NVARCHAR(500) | NOT NULL | | 住所 |
| IsActive | BIT | NOT NULL | DEFAULT 1 | 有効フラグ（0=論理削除） |
| CreatedAt | DATETIME2 | NOT NULL | DEFAULT GETDATE() | 作成日時 |

#### 3.3.3 蔵書テーブル（books）

| カラム名 | データ型 | NULL | 制約 | 説明 |
|----------|----------|------|------|------|
| ID | INT | NOT NULL | PK, IDENTITY(1,1) | 蔵書ID（自動採番） |
| BookName | NVARCHAR(500) | NOT NULL | | 図書名 |
| Author | NVARCHAR(200) | NOT NULL | | 著者名 |
| Publisher | NVARCHAR(200) | NOT NULL | | 出版社 |
| Genre | NVARCHAR(100) | NOT NULL | | ジャンル |
| ISBN | NVARCHAR(20) | NOT NULL | UNIQUE | ISBN番号 |
| Status | TINYINT | NOT NULL | DEFAULT 0 | 状態（0:在庫 1:貸出中 2:予約済 3:除籍） |
| IsLoaned | BIT | NOT NULL | DEFAULT 0 | 貸出中フラグ（1=貸出中） |
| IsReserved | BIT | NOT NULL | DEFAULT 0 | 予約済みフラグ（1=予約あり） |

> `IsLoaned`・`IsReserved` は `Status` と冗長だが、検索・ロジック分岐の可読性向上のため保持する。  
> 更新時は必ず `Status` と同時に整合性を保つこと。

#### 3.3.4 ログテーブル（logs）

| カラム名 | データ型 | NULL | 制約 | 説明 |
|----------|----------|------|------|------|
| ID | BIGINT | NOT NULL | PK, IDENTITY(1,1) | ログID |
| User_id | INT | NOT NULL | FK→users | 利用者ID |
| Book_id | INT | NOT NULL | FK→books | 蔵書ID |
| LoanDate | DATETIME2 | NOT NULL | DEFAULT GETDATE() | 貸出日時 |
| ReturnDue | DATE | NOT NULL | | 返却期限日 |
| ReturnDate | DATETIME2 | NULL | | 実返却日時（NULL=貸出中） |

> 3年以上経過した返却済み記録は別アーカイブテーブル（`logs_archive`）へ月次バッチで移行する。

#### 3.3.5 予約テーブル（reservations）

| カラム名 | データ型 | NULL | 制約 | 説明 |
|----------|----------|------|------|------|
| ID | INT | NOT NULL | PK, IDENTITY(1,1) | 予約ID |
| User_id | INT | NOT NULL | FK→users | 予約利用者ID |
| Book_id | INT | NOT NULL | FK→books | 蔵書ID |
| ReservationDate | DATETIME2 | NOT NULL | DEFAULT GETDATE() | 予約日時 |
| Status | TINYINT | NOT NULL | DEFAULT 0 | 0:有効 1:貸出済 2:キャンセル |
| Notified | BIT | NOT NULL | DEFAULT 0 | 返却通知済フラグ |

### 3.4 インデックス設計

| テーブル | インデックス名 | カラム | 種別 | 目的 |
|----------|----------------|--------|------|------|
| books | IX_books_BookName | BookName | 非クラスター化 | タイトル検索高速化 |
| books | IX_books_Author | Author | 非クラスター化 | 著者名検索高速化 |
| books | IX_books_Status | Status, IsLoaned, IsReserved | 非クラスター化 | ステータス絞り込み |
| logs | IX_logs_User_active | User_id, ReturnDate | 非クラスター化 | 利用者別貸出中件数取得 |
| logs | IX_logs_Book_active | Book_id, ReturnDate | 非クラスター化 | 蔵書貸出状態確認 |
| reservations | IX_res_Book_Status | Book_id, Status | 非クラスター化 | 蔵書予約状態確認 |
| users | IX_users_Name | Name | 非クラスター化 | 氏名検索 |

---

## 4. クラス設計

### 4.1 Modelレイヤー

#### 4.1.1 エンティティクラス

| クラス名 | 名前空間 | 説明 |
|----------|----------|------|
| Librarian | LibrarySystem.Model.Entities | 司書エンティティ |
| User | LibrarySystem.Model.Entities | 利用者エンティティ |
| Book | LibrarySystem.Model.Entities | 蔵書エンティティ |
| Log | LibrarySystem.Model.Entities | 貸出ログエンティティ |
| Reservation | LibrarySystem.Model.Entities | 予約エンティティ |

#### 4.1.2 サービスクラス

| クラス名/インターフェース | 主要メソッド | 説明 |
|---------------------------|-------------|------|
| IAuthService / AuthService | `Login(userName, password): Librarian`<br>`Logout()` | 司書認証・セッション管理 |
| IBookService / BookService | `Search(criteria): IList<Book>`<br>`GetById(bookId): Book`<br>`Register(dto): Book` | 蔵書検索・取得・新規登録 |
| ILoanService / LoanService | `Checkout(bookId, userId): LoanResult`<br>`Return(bookId, userId): ReturnResult`<br>`GetActiveLoans(userId): IList<Log>` | 貸出・返却処理 |
| IReservationService / ReservationService | `Reserve(bookId, userId): ReservationResult`<br>`CancelReservation(reservationId): bool`<br>`GetByBook(bookId): Reservation` | 予約管理 |
| IUserService / UserService | `CreateUser(dto): User`<br>`UpdateUser(userId, dto): User`<br>`Search(name): IList<User>` | 利用者管理 |

#### 4.1.3 リポジトリクラス

| インターフェース | 実装クラス | 説明 |
|------------------|------------|------|
| IBookRepository | BookRepository | 蔵書データアクセス（Dapper） |
| ILogRepository | LogRepository | 貸出ログデータアクセス |
| IReservationRepository | ReservationRepository | 予約データアクセス |
| IUserRepository | UserRepository | 利用者データアクセス |
| ILibrarianRepository | LibrarianRepository | 司書データアクセス |

### 4.2 Viewレイヤー（WinForms）

| Form名 | インターフェース | 機能 | 操作者 |
|--------|-----------------|------|--------|
| LoginForm | ILoginView | 司書ログイン画面 | 司書 |
| MainForm | IMainView | メインメニュー（MDI親） | 司書 |
| BookListForm | IBookListView | 蔵書一覧・検索画面 | 司書 |
| BookRegisterForm | IBookRegisterView | 蔵書新規登録画面 | 司書 |
| CheckoutForm | ICheckoutView | 蔵書貸し出し画面 | 利用者 |
| ReturnForm | IReturnView | 蔵書返却画面 | 利用者 |
| ReservationForm | IReservationView | 蔵書予約画面 | 利用者 |
| CompletionForm | ICompletionView | 操作完了画面（全操作共通） | 利用者・司書 |
| UserManageForm | IUserManageView | 利用者管理画面 | 司書 |

### 4.3 Presenterレイヤー

| Presenterクラス | 対応View | 主要処理 |
|-----------------|----------|----------|
| LoginPresenter | ILoginView | 入力バリデーション、認証呼び出し、遷移制御 |
| BookListPresenter | IBookListView | 検索条件構築、一覧表示、ステータス表示 |
| BookRegisterPresenter | IBookRegisterView | 入力バリデーション、蔵書登録実行、重複ISBN確認 |
| CheckoutPresenter | ICheckoutView | 貸出可否チェック、上限確認、貸出実行、完了画面遷移 | 
| ReturnPresenter | IReturnView | 返却処理、予約確認表示、完了画面遷移 |
| ReservationPresenter | IReservationView | 予約登録、返却予定日表示、完了画面遷移 |
| CompletionPresenter | ICompletionView | 完了メッセージ・詳細情報の表示制御、トップ画面への戻り制御 |
| UserManagePresenter | IUserManageView | 利用者CRUD、入力検証 |

---

## 5. 機能設計

### 5.1 司書ログイン機能

#### 5.1.1 処理フロー

1. LoginFormが表示される。
2. 司書がログインIDとパスワードを入力しログインボタンを押下する。
3. LoginPresenterが入力値のバリデーションを行う（空チェック）。
4. AuthServiceがDBから該当UserNameの司書レコードを取得する。
5. 取得したPasswordハッシュとPBKDF2で入力パスワードを検証する。
6. 認証成功時、セッションにLibrarianオブジェクトを格納しMainFormへ遷移する。
7. 認証失敗時、エラーメッセージを表示する（詳細は伏せる）。

#### 5.1.2 PBKDF2実装方針

- アルゴリズム：HMACSHA256
- イテレーション回数：600,000回（NIST推奨）
- ソルト：ユーザーごとにランダム生成（16バイト）、Passwordカラムに埋め込み保存
- 派生キー長：32バイト
- 使用クラス：`System.Security.Cryptography.Rfc2898DeriveBytes`
- Password保存フォーマット：`Base64( version[1byte] + salt[16byte] + hash[32byte] )`

---

### 5.2 蔵書管理（検索）機能

#### 5.2.1 検索条件

| 検索項目 | 入力形式 | 検索方式 |
|----------|----------|----------|
| 図書名 | テキストボックス | 部分一致（LIKE） |
| 著者名 | テキストボックス | 部分一致（LIKE） |
| 出版社 | テキストボックス | 部分一致（LIKE） |
| ジャンル | テキストボックス | 部分一致（LIKE） |
| 状態 | コンボボックス（複数選択） | IN句 |
| 蔵書ID | テキストボックス | 完全一致 |

#### 5.2.2 一覧表示項目

| 表示項目 | 備考 |
|----------|------|
| 蔵書ID | |
| 図書名 | |
| 著者名 | |
| 出版社 | |
| ジャンル | |
| 状態 | 在庫/貸出中/予約済/返却期限切れ を色分け表示 |
| 返却期限日 | 貸出中の場合表示。期限切れは赤字表示 |

---

### 5.3 蔵書新規登録機能（v1.1追加）

#### 5.3.1 概要

司書がログイン中のみ操作可能。BookRegisterFormから蔵書情報を入力し、booksテーブルへ登録する。

#### 5.3.2 処理フロー

1. 司書がメインメニューから「蔵書新規登録」を選択する。
2. BookRegisterFormが表示される。
3. 司書が各項目を入力し「登録」ボタンを押下する。
4. BookRegisterPresenterが入力値のバリデーションを行う。
5. ISBNが入力されている場合、重複チェックを行う（重複時はエラー表示）。
6. BookServiceがbooksテーブルへレコードを挿入する（初期Status=0:在庫）。
7. 登録成功後、CompletionFormへ遷移し「蔵書を登録しました」を表示する。

#### 5.3.3 入力項目・バリデーション

| 項目 | 必須 | バリデーション |
|------|------|----------------|
| 図書名（BookName） | 必須 | 1〜500文字 |
| 著者名（Author） | 必須 | 1〜200文字 |
| 出版社（Publisher） | 必須 | 1〜200文字 |
| ジャンル（Genre） | 必須 | 1〜100文字 |
| ISBN | 必須 | 10桁または13桁の数字、重複チェック |

---

### 5.4 蔵書貸し出し機能（v1.1変更：利用者が自己操作）

#### 5.4.1 処理フロー

1. 利用者がCheckoutFormで利用者IDと蔵書IDを入力する。
2. CheckoutPresenterが入力値のバリデーションを行う（空チェック・数値チェック）。
3. LoanServiceが利用者IDの存在確認・IsActiveチェックを行う。
4. LoanServiceが蔵書の現在ステータスを確認する。
5. LoanServiceが対象利用者の貸出中件数を確認する（上限5件チェック）。
6. LoanServiceが対象利用者に延滞中（ReturnDue < 本日 かつ ReturnDate IS NULL）の蔵書がないか確認する。
7. 延滞中の場合、警告メッセージを表示する（処理は続行可能）。
8. 貸し出し不可の場合（他者予約中・貸出中）、予約フローへ誘導する。
9. LoanServiceがlogsテーブルにレコードを挿入し、booksテーブルのStatus/IsLoanedを更新する（Librarian_id = NULL）。
10. CompletionFormへ遷移し、返却期限日（貸出日＋14日）を表示する。

#### 5.4.2 貸出可否条件

| 条件 | 結果 |
|------|------|
| booksのStatus=0（在庫） | 貸出可 |
| booksのStatus=1（貸出中） | 貸出不可→予約誘導 |
| booksのStatus=2（予約済）かつ予約者＝対象利用者 | 貸出可 |
| booksのStatus=2（予約済）かつ予約者≠対象利用者 | 貸出不可 |
| 利用者の貸出中件数が5件以上 | 貸出不可 |

---

### 5.5 蔵書返却機能（v1.1変更：利用者が自己操作）

#### 5.5.1 処理フロー

1. 利用者がReturnFormで利用者IDと蔵書IDを入力する。
2. ReturnPresenterが入力値のバリデーションを行う。
3. LoanServiceが該当蔵書・該当利用者の貸出中レコードを取得する（不一致時エラー）。
4. ReservationServiceが当該蔵書に有効な予約が存在するか確認する。
5. logsテーブルのReturnDateに現在日時を設定する。
6. 予約が存在する場合はbooksのStatus=2（予約済）、IsLoaned=0、IsReserved=1 に更新し、Notifiedフラグを立てる。
7. 予約がない場合はbooksのStatus=0（在庫）、IsLoaned=0、IsReserved=0 に更新する。
8. CompletionFormへ遷移し「返却が完了しました」を表示する。予約者が存在する場合はその旨も表示する。

---

### 5.6 蔵書予約機能

#### 5.6.1 処理フロー

1. 利用者がReservationFormで利用者IDと蔵書IDを入力する（貸出画面から遷移した場合は蔵書IDは引き継ぎ）。
2. ReservationPresenterが入力値のバリデーションを行う。
3. ReservationServiceが重複予約がないか確認する。
4. 当該蔵書の現在の貸出期限日（ReturnDue）を取得し画面に表示する。
5. reservationsテーブルにレコードを挿入する。
6. booksのStatus=2（予約済）、IsReserved=1 に更新する。
7. CompletionFormへ遷移し、予約完了と貸出期限日を表示する。
8. 返却完了時（5.5フロー）にNotifiedフラグを立て、予約者のMailへ通知する。

---

### 5.7 操作完了画面（v1.1追加）

#### 5.7.1 概要

貸出・返却・予約・蔵書登録のすべての操作が正常完了した後に表示する共通画面。  
CompletionFormは操作種別（`CompletionType` enum）と表示情報（`CompletionViewModel`）を受け取って表示を切り替える。

#### 5.7.2 表示内容

操作種別によらず「**操作が完了しました**」の単一テキストのみを表示する。追加情報（蔵書名・返却期限日等）は表示しない。

#### 5.7.3 画面遷移

- 「トップへ戻る」ボタン押下で呼び出し元の画面（利用者操作の場合はCheckoutForm/ReturnForm初期状態、司書操作の場合はMainForm）へ戻る。
- 一定時間（30秒）操作がない場合、自動でトップへ戻る。

---

### 5.8 利用者管理機能

#### 5.8.1 新規登録項目

| 項目 | 必須 | バリデーション |
|------|------|----------------|
| 氏名（Name） | 必須 | 1〜100文字 |
| 性別（Gender） | 必須 | 0（男性）または1（女性）のいずれか |
| 生年月日（Birth） | 必須 | YYYY/MM/DD形式、過去日付 |
| メールアドレス（Mail） | 必須 | RFC5322準拠フォーマット、重複チェック |
| 電話番号（Phone） | 必須 | 数字・ハイフンのみ、7〜15桁 |
| 住所（Address） | 必須 | 500文字以内 |

---

## 6. 画面設計

### 6.1 画面一覧

| 画面ID | 画面名 | 起動条件 | 操作者 | 備考 |
|--------|--------|----------|--------|------|
| SCR-001 | ログイン画面 | アプリ起動時 | 司書 | ログイン前のみ表示 |
| SCR-002 | メインメニュー | ログイン成功後 | 司書 | MDI親フォーム |
| SCR-003 | 蔵書一覧・検索 | メニューから | 司書 | DataGridView使用 |
| SCR-004 | 蔵書新規登録 | メニューから | 司書 | ログイン必須（v1.1追加） |
| SCR-005 | 蔵書貸し出し | 利用者端末起動時 | 利用者 | 利用者ID・蔵書ID入力（v1.1変更） |
| SCR-006 | 蔵書返却 | 利用者端末起動時 | 利用者 | 利用者ID・蔵書ID入力（v1.1変更） |
| SCR-007 | 蔵書予約 | 貸出不可時・直接起動 | 利用者 | 貸し出し画面から遷移可 |
| SCR-008 | 操作完了 | 各操作正常完了後 | 利用者・司書 | 全操作共通（v1.1追加） |
| SCR-009 | 利用者管理 | メニューから | 司書 | 新規登録・編集 |

### 6.2 主要画面レイアウト方針

- フォームサイズ：1280×768ピクセル（メインフォーム）
- フォント：メイリオ 9pt（日本語UI標準）
- カラースキーム：白背景、アクセントカラーは紺色（`#1E3A5F`）
- 蔵書一覧はDataGridViewを使用し、仮想モードで大量データに対応する。
- 返却期限切れの行は背景を薄赤（`#FFE4E4`）で表示する。
- 必須入力項目はラベルに「`*`」マークを付与する。

### 6.3 完了画面レイアウト（SCR-008）

```
┌─────────────────────────────────────────┐
│                                         │
│                                         │
│         ✔  操作が完了しました           │
│                                         │
│                                         │
│          [ トップへ戻る ]               │
│                                         │
│      ※30秒後に自動で戻ります           │
└─────────────────────────────────────────┘
```

### 6.4 蔵書新規登録画面レイアウト（SCR-004）

```
┌─────────────────────────────────────────┐
│  蔵書新規登録                           │
├─────────────────────────────────────────┤
│  図書名 *  [                          ] │
│  著者名 *  [                          ] │
│  出版社 *  [                          ] │
│  ジャンル* [                          ] │
│  ISBN    * [                          ] │
│                                         │
│          [ 登録 ]  [ キャンセル ]       │
└─────────────────────────────────────────┘
```

---

## 7. セキュリティ設計

### 7.1 認証・認可

- ログイン成功後、Presenterレイヤー内のセッションオブジェクト（`CurrentSession`）にログイン司書情報を格納する。
- 司書専用画面（SCR-001〜004、SCR-009）の起動時にセッションの有効性を確認し、未認証の場合はログイン画面へリダイレクトする。
- 利用者操作画面（SCR-005〜008）は認証不要だが、利用者IDの存在確認・IsActiveチェックを必ず行う。
- 一定時間（30分）操作がない場合、司書セッションを自動ログアウトする。

### 7.2 パスワード管理

- パスワードはPBKDF2（HMACSHA256、反復600,000回）でハッシュ化し、SaltをPasswordカラムに埋め込んだ形式（Base64エンコード）でDB保存する。
- パスワード平文はメモリ上でも最小限の保持とし、`SecureString` または即時クリアを行う。
- ログインID/パスワード不一致時のエラーメッセージは「IDまたはパスワードが正しくありません」とし、どちらが誤りかを特定できないようにする。

### 7.3 個人情報保護

- usersテーブルへのアクセスはアプリケーション専用のSQLユーザー（`library_app_user`）に限定し、最小権限を付与する。
- DB接続文字列は `appsettings.json` に暗号化して保存し、ソースコードにハードコードしない。
- ログファイルに個人情報（氏名・住所・メールアドレス等）を出力しない。

### 7.4 SQLインジェクション対策

- すべてのDBアクセスにパラメータ化クエリ（Dapperのパラメータ渡し）を使用する。
- ユーザー入力を直接SQL文字列に連結することを禁止する。

---

## 8. 非機能要件への対応

### 8.1 性能

| 要件 | 対応方針 |
|------|----------|
| 蔵書1,000万件登録時のパフォーマンス維持 | DataGridViewの仮想モード採用。検索クエリにインデックス適用。ページング実装（1ページ100件）。 |
| ログイン・検索1秒以内 | ログインはUNIQUEインデックス付きUserNameで単一行取得。検索は複合インデックス活用。 |
| 同時アクセス | SQL ServerのROW_LEVEL LOCKINGに依存。ADO.NET接続プーリングを有効化。 |

### 8.2 バックアップ

| バックアップ種別 | スケジュール | 保持期間 |
|------------------|-------------|----------|
| 増分バックアップ | 毎日閉館後（例：22:00） | 30日 |
| フルバックアップ | 毎月初日閉館後（例：22:00） | 12ヶ月 |

SQL Server Agentジョブで自動実行。バックアップ先は別ディスクまたはNAS。

### 8.3 履歴データ保持

- 貸出・返却・予約履歴は3年間保持する（logsテーブル・reservationsテーブル）。
- 3年経過したレコードは `logs_archive`・`reservations_archive` テーブルへ月次バッチで移行する。

### 8.4 Excelデータ移行

- 既存Excelデータのカラム名を調査し、booksテーブル・usersテーブルへのマッピング定義書を別途作成する。
- データ移行ツール（C#コンソールアプリ）を作成し、検証後に本番DBへインポートする。

---

## 9. エラーハンドリング方針

### 9.1 表示方式

すべてのエラー・警告は `MessageBox.Show()` によるポップアップダイアログで表示する。インライン表示（ラベル等への文字表示）は使用しない。

| エラー種別 | ダイアログ種別 | 例 |
|------------|---------------|----|
| 入力値エラー | MessageBoxIcon.Warning | 「図書名を入力してください」 |
| 業務エラー | MessageBoxIcon.Warning | 「貸出上限（5冊）に達しています」 |
| システムエラー | MessageBoxIcon.Error | 「システムエラーが発生しました。管理者へご連絡ください。」 |

### 9.2 実装方針

- すべてのPresenter処理はtry-catchで囲み、予期せぬ例外はログに記録した上でポップアップで通知する。
- 業務エラー（貸出上限超過・ISBN重複等）は `Result<T>` パターンで返し、Presenterがポップアップを呼び出す。例外は使用しない。
- ログはNLogを使用しファイル出力する。ログレベルは INFO / WARN / ERROR / FATAL を使い分ける。ログに個人情報は含めない。

---

## 10. 開発・テスト方針

### 10.1 開発環境

| 項目 | 内容 |
|------|------|
| IDE | Visual Studio 2022 以降 |
| フレームワーク | .NET 10 |
| 言語バージョン | C# 13 |
| DB | SQL Server 2022 Developer Edition（開発時） |
| ソース管理 | Git（GitHub / Azure DevOps） |

### 10.2 テスト方針

| テスト種別 | 対象 | ツール |
|------------|------|--------|
| ユニットテスト | Service・Presenterクラス | xUnit + Moq |
| 統合テスト | Repository（DBアクセス） | xUnit + ローカルDB |
| UIテスト | 主要業務フロー（利用者操作・司書操作） | 手動テスト |

### 10.3 主要テストケース（抜粋）

| テストID | 対象機能 | シナリオ | 期待結果 |
|----------|----------|----------|----------|
| TC-001 | ログイン | 正しいID・PWでログイン | MainFormへ遷移 |
| TC-002 | ログイン | 誤PWでログイン | エラーメッセージ表示、遷移なし |
| TC-003 | 貸出 | 在庫蔵書を貸出 | 完了画面表示、Status=1 |
| TC-004 | 貸出 | 貸出中蔵書を貸出 | エラー・予約誘導表示 |
| TC-005 | 貸出 | 貸出5冊済みの利用者が貸出 | 上限エラー表示 |
| TC-006 | 返却 | 貸出中蔵書を返却（予約なし） | 完了画面、Status=0 |
| TC-007 | 返却 | 貸出中蔵書を返却（予約あり） | 完了画面、予約者通知表示、Status=2 |
| TC-008 | 予約 | 貸出中蔵書を予約 | 完了画面、返却期限日表示 |
| TC-009 | 蔵書登録 | 正常な情報で登録 | 完了画面、booksに挿入 |
| TC-010 | 蔵書登録 | ISBN重複で登録 | 重複エラー表示 |

---

以上
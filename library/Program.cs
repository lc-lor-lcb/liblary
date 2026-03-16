// library.UI/Program.cs
using library;
using library.View;
using library.Model;
using library.Model.Repositories;
using library.Model.Repositories;
using library.Model.Services;
using library.Presenter;
using library.UI;
using library.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Windows.Forms;

namespace library.library.UI
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main()
        {
            // Windows Forms の高DPI対応設定（.NET 10）
            ApplicationConfiguration.Initialize();

            // 未処理例外ハンドラの登録
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            try
            {
                Logger.Info("図書管理システム 起動開始");

                // DIコンテナ構築
                var services = new ServiceCollection();
                ConfigureServices(services);
                var provider = services.BuildServiceProvider();

                // ログイン画面を起動
                var loginForm = provider.GetRequiredService<LoginForm>();
                Application.Run(loginForm);

                Logger.Info("図書管理システム 正常終了");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "起動時に致命的なエラーが発生しました");
                MessageBox.Show(
                    "システムエラーが発生しました。管理者へご連絡ください。",
                    "致命的なエラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        /// <summary>
        /// DIサービス登録
        /// </summary>
        private static void ConfigureServices(IServiceCollection services)
        {
            // --- Model ---
            // DB接続設定（appsettings.json から読み込み）
            services.AddSingleton<IDbConnectionFactory, SqlServerConnectionFactory>();

            // --- Repositories ---
            services.AddScoped<ILibrarianRepository, LibrarianRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // --- Services ---
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IUserService, UserService>();

            // --- Session（インメモリ、シングルトン）---
            services.AddSingleton<ISessionContext, SessionContext>();

            // --- Presenters ---
            services.AddTransient<LoginPresenter>();
            services.AddTransient<BookListPresenter>();
            services.AddTransient<BookRegisterPresenter>();
            services.AddTransient<CheckoutPresenter>();
            services.AddTransient<ReturnPresenter>();
            services.AddTransient<ReservationPresenter>();
            services.AddTransient<CompletionPresenter>();
            services.AddTransient<UserManagePresenter>();

            // --- Views（Forms）---
            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();
            services.AddTransient<BookListForm>();
            services.AddTransient<BookRegisterForm>();
            services.AddTransient<CheckoutForm>();
            services.AddTransient<ReturnForm>();
            services.AddTransient<ReservationForm>();
            services.AddTransient<CompletionForm>();
            services.AddTransient<UserManageForm>();
        }

        /// <summary>
        /// UIスレッド上の未処理例外ハンドラ
        /// </summary>
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UIスレッドで未処理の例外が発生しました");
            MessageBox.Show(
                "システムエラーが発生しました。管理者へご連絡ください。",
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// バックグラウンドスレッドの未処理例外ハンドラ
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Logger.Fatal(ex, "バックグラウンドスレッドで未処理の例外が発生しました");

            if (e.IsTerminating)
            {
                MessageBox.Show(
                    "システムエラーが発生しました。管理者へご連絡ください。",
                    "致命的なエラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
}
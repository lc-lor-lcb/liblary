using library;
using library.Model;
using library.Model.Repositories;
using library.Model.Services;
using library.Presenter;
using library.View;
using library.Views.Interfaces; // 追加
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Windows.Forms;

namespace library.UI
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            try
            {
                Logger.Info("図書管理システム 起動開始");

                var services = new ServiceCollection();
                ConfigureServices(services);
                var provider = services.BuildServiceProvider();

                // スコープを作成してアプリ終了まで保持する
                var scope = provider.CreateScope();
                var scopedProvider = scope.ServiceProvider;

                var loginForm = scopedProvider.GetRequiredService<LoginForm>();

                // 同一スコープ上で Presenter を生成（ILoginView 引数に loginForm を渡す）
                ActivatorUtilities.CreateInstance<LoginPresenter>(scopedProvider, loginForm);

                Application.Run(loginForm);

                // アプリ終了後にスコープを破棄
                scope.Dispose();

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

        private static void ConfigureServices(IServiceCollection services)
        {
            // --- Model ---
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

            // --- Session ---
            //services.AddSingleton<ISessionContext, SessionContext>();    //DB接続

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
            // LoginPresenter のコンストラクタが ILoginView を受け取るため、インターフェースをフォームにマップする
            services.AddTransient<ILoginView, LoginForm>(); // 追加

            services.AddTransient<MainForm>();
            services.AddTransient<BookListForm>();
            services.AddTransient<BookRegisterForm>();
            services.AddTransient<CheckoutForm>();
            services.AddTransient<ReturnForm>();
            services.AddTransient<ReservationForm>();
            services.AddTransient<CompletionForm>();
            services.AddTransient<UserManageForm>();
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UIスレッドで未処理の例外が発生しました");
            MessageBox.Show(
                "システムエラーが発生しました。管理者へご連絡ください。",
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

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
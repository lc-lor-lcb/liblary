using LibrarySystem.Infrastructure;
using LibrarySystem.Model.Repositories;
using LibrarySystem.Model.Services;
using LibrarySystem.Presenter.Views;
using library.View;
using Microsoft.Extensions.Configuration;
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

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();

                string connStr = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "appsettings.json に ConnectionStrings:DefaultConnection が見つかりません。");

                // ★ 全Formから参照できるよう一元管理クラスに登録
                ConnectionConfig.Initialize(connStr);

                var services = new ServiceCollection();
                ConfigureServices(services, connStr);
                var provider = services.BuildServiceProvider();

                var scope = provider.CreateScope();
                var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();

                Application.Run(loginForm);

                scope.Dispose();
                Logger.Info("図書管理システム 正常終了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "致命的なエラー（詳細）",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(IServiceCollection services, string connStr)
        {
            services.AddSingleton<IDbConnectionFactory>(
                _ => new SqlConnectionFactory(connStr));

            services.AddScoped<ILibrarianRepository, LibrarianRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IUserService, UserService>();

            services.AddTransient<LoginForm>();
            services.AddTransient<MainForm>();
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "UIスレッドで未処理の例外が発生しました");
            MessageBox.Show(e.Exception.ToString(), "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Logger.Fatal(ex, "バックグラウンドスレッドで未処理の例外が発生しました");
            if (e.IsTerminating)
                MessageBox.Show(ex?.ToString() ?? "不明なエラー", "致命的なエラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
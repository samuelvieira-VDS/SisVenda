using Microsoft.Extensions.Logging;
using Sisvenda.Database;
using Sisvenda.ViewModels;
using Sisvenda.Views;

namespace Sisvenda
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services
                .AddSingleton<DatabaseService>()
                .AddSingleton<ProdutoViewModel>()
                .AddSingleton<ProdutoListPage>()
                .AddTransient<ProdutoDetailViewModel>()
                .AddTransient<ProdutoDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

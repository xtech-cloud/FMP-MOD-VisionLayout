
using XTC.FMP.MOD.VisionLayout.App.Service;
public static class MyProgram
{
    public static void PreBuild(WebApplicationBuilder? _builder)
    {
        _builder?.Services.AddSingleton<SingletonServices>();
    }

    public static void PreRun(WebApplication? _app)
    {
    }
}



using TwitterStream.Core.Implementations;

namespace TwitterStream
{

    public class StartupHelper
    {

        public static void BindServices()
        {
            // builder.Services.AddScoped<ITestService, TestService>();
            // builder.Services.AddScoped<IStudentService, StudentService>();
        }

        public static void TestService(IServiceProvider service)
        {
            ITest t = (ITest)service.GetService(typeof(ITest));
            t.Test();
        }
    }
}
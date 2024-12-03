namespace ChatApprication.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();
            builder.Services.AddMagicOnion();

            var app = builder.Build();

            app.MapMagicOnionService();

            app.Run();
        }
    }
}

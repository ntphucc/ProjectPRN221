
[assembly: HostingStartup(typeof(ProjectRoomChat.Areas.Identity.IdentityHostingStartup))]
namespace ProjectRoomChat.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
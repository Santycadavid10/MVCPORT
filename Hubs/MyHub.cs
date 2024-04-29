using Microsoft.AspNetCore.SignalR;

namespace SignalR
{
    public class MiHub : Hub
    {
        public async Task SendData(string data)
        {
            await Clients.All.SendAsync("ReceiveData", data);
            Console.WriteLine(data);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ManageCoffee.Controllers.Hubs
{
    public class Notification : Hub
    {
        public async Task SendNotification(string str)
        {
            System.Console.WriteLine("" + str);
            await Clients.All.SendAsync("Noti", str);
        }
    }
}
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
        public async Task SendNotification(object details)
        {
            System.Console.WriteLine("" + details);
            await Clients.All.SendAsync("Noti", details);
        }
    }
}
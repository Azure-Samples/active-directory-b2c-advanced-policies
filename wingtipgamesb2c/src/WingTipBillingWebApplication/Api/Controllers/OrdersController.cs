using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WingTipBillingWebApplication.Api.Models;

namespace WingTipBillingWebApplication.Api.Controllers
{
    [Authorize(ActiveAuthenticationSchemes = Constants.AuthenticationSchemes.Bearer, Policy = Constants.AuthorizationPolicies.ReadBilling)]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        public IActionResult Get()
        {
            return Ok(new List<Order>
            {
                new Order
                {
                    Id = "567735044824186630",
                    Date = new DateTime(2017, 3, 15),
                    Description = "1 Month Games Pass",
                    Status = "Completed",
                    TotalPrice = 11.99M
                },
                new Order
                {
                    Id = "567735044507642760",
                    Date = new DateTime(2017, 2, 15),
                    Description = "1 Month Games Pass",
                    Status = "Completed",
                    TotalPrice = 11.99M
                },
                new Order
                {
                    Id = "567735044172410498",
                    Date = new DateTime(2017, 1, 15),
                    Description = "1 Month Games Pass",
                    Status = "Completed",
                    TotalPrice = 11.99M
                }
            });
        }
    }
}

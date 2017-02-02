using B2CRestApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CRestApi.Models
{
        public class ContextManager
        {

            public static DbContextBase CreateContext()
            {
                return new LoyaltyDbContext();
            }
        }
}
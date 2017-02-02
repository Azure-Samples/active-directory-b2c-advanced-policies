using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CRestApi.Models
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoyaltyModel"/> class.
    /// </summary>
    public class LoyaltyModel
    {
        /// <summary>
        /// Gets or sets the id of the object
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user id of the object
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the loyalty number
        /// </summary>
        public string LoyaltyNumber { get; set; }
    }
}
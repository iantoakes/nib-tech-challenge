using System.Collections.Generic;

namespace TechChallenge.DomainLogic.Models
{
    public static class FulfillmentStatus
    {
        public const string Error = "Error:Unfulfilled";
        public const string Success = "Fulfilled";
        public const string Pending = "Pending";

        public static List<string> ValidStatuses => new List<string> {Error, Success, Pending};
    }
}
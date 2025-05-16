using Microsoft.AspNetCore.Mvc;

namespace Cox.Cmr.Payment.Api.Models
{
    public class Header
    {
        [FromHeader]
        public required string CountryCode { get; set; }

        [FromHeader]
        public required string Source { get; set; }
    }
}

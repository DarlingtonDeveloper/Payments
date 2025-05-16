using Microsoft.AspNetCore.Mvc;

namespace Cox.Cmr.Payment.Api.Models
{
    public class QueryParameters
    {
        [FromQuery]
        public bool Mot { get; set; } = false;

        [FromQuery]
        public int Mileage { get; set; }

        [FromQuery]
        public bool Valuation { get; set; } = false;

    }
}

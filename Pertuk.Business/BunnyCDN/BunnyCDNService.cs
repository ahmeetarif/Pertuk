using BunnyCDN.Net.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Pertuk.Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pertuk.Business.BunnyCDN
{
    public class BunnyCDNService : BunnyCDNStorage
    {
        private const string ApiKey = "d5e1d437-9fa3-4da4-98d3cbb50920-d00a-4a46";
        private const string ZoneName = "pertukmedia";
        private const string ZoneRegion = "de";

        public BunnyCDNService()
            : base(ZoneName, ApiKey, ZoneRegion)
        {

        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWave4Net.Communication;

namespace ZWave4Net.Commands
{
    public class ManufacturerSpecific : CommandClass
    {
        enum manufacturerSpecificCmd
        {
            Get = 0x04,
            Report = 0x05
        }

        public ManufacturerSpecific(Node node) : base(0x72, node)
        {
        }

        protected override Enum[] Commands
        {
            get { return Enum.GetValues(typeof(manufacturerSpecificCmd)).Cast<Enum>().ToArray(); }
        }

        public async Task<ProductData> GetProductData()
        {
            var response = await Invoker.Send(new Command(ClassID, manufacturerSpecificCmd.Get));
            return ProductData.Parse(response.Payload);
        }

        protected override bool IsCorrelated(Enum request, Enum response)
        {
            if (object.Equals(request, manufacturerSpecificCmd.Get) && object.Equals(response, manufacturerSpecificCmd.Report))
                return true;

            return false;
        }

        protected override void OnEvent(Enum command, byte[] payload)
        {
            var productInfo = ProductData.Parse(payload);
            Platform.LogMessage(LogLevel.Debug, string.Format($"Event: Node = {Node}, Class = {ClassName}, Command = {command}, {productInfo}"));
        }
    }
}

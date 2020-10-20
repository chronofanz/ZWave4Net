﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ZWave.Channel;

namespace ZWave.CommandClasses
{
    public class Schedule : EndpointSupportedCommandClassBase
    {
        private const int ScheduleIdBlockMinimalProtocolVersion = 2;

        public const byte AllSchedules = 0x00;
        public const byte ScheduleIdFallback = 0xFE;
        public const byte ScheduleIdOverride = 0xFF;

        enum command
        {
            SupportedGet = 0x01,
            SupportedReport = 0x02,
            Set = 0x03,
            Get = 0x04,
            Report = 0x05,
            Remove = 0x06,
            StateSet = 0x07,
            StateGet = 0x08,
            StateReport = 0x09,
            SupportedCommandsGet = 0x0A,
            SupportedCommandsReport = 0x0B
        }

        public Schedule(Node node)
            : base(node, CommandClass.Schedule)
        { }

        internal Schedule(Node node, byte endpointId)
            : base(node, CommandClass.Schedule, endpointId)
        { }

        public Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities()
        {
            return GetSupportedFunctionalities(CancellationToken.None);
        }

        public async Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(CancellationToken cancellationToken)
        {
            var response = await Send(new Command(Class, command.SupportedGet), command.SupportedReport, cancellationToken);
            return new ScheduleSupportedFunctionalitiesReport(Node, response);
        }

        public Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(byte scheduleIdBlock)
        {
            return GetSupportedFunctionalities(scheduleIdBlock, CancellationToken.None);
        }

        public async Task<ScheduleSupportedFunctionalitiesReport> GetSupportedFunctionalities(byte scheduleIdBlock, CancellationToken cancellationToken)
        {
            if (!await IsSupportScheduleIdBlock(cancellationToken))
            {
                throw new VersionNotSupportedException($"Schedule ID blocks work with class type {Class} greater or equal to {ScheduleIdBlockMinimalProtocolVersion}.");
            }

            var response = await Send(new Command(Class, command.SupportedGet, scheduleIdBlock), command.SupportedReport, cancellationToken);
            return new ScheduleSupportedFunctionalitiesReport(Node, response);
        }

        public Task<bool> IsSupportScheduleIdBlock()
        {
            return IsSupportScheduleIdBlock(CancellationToken.None);
        }

        public async Task<bool> IsSupportScheduleIdBlock(CancellationToken cancellationToken)
        {
            var report = await Node.GetCommandClassVersionReport(Class, cancellationToken);
            return report.Version >= ScheduleIdBlockMinimalProtocolVersion;
        }
    }
}

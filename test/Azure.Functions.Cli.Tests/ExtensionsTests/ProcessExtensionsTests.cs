﻿using System.Diagnostics;
using System.Threading.Tasks;
using FluentAssertions;
using Azure.Functions.Cli.Extensions;
using Xunit;
using System.Runtime.InteropServices;

namespace Azure.Functions.Cli.Tests.ExtensionsTests
{
    public class ProcessExtensionsTests
    {
        [SkippableFact]
        public async Task WaitForExitTest()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                reason: "Unreliable on linux CI");

            Process process = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? Process.Start("cmd")
                : Process.Start("sh");
            var calledContinueWith = false;

            process.WaitForExitAsync().ContinueWith(_ =>
            {
                calledContinueWith = true;
            }).Ignore();

            process.Kill();
            for (var i = 0; !calledContinueWith && i < 5; i++)
            {
                await Task.Delay(200);
            }
            calledContinueWith.Should().BeTrue(because: "the process should have exited and called the continuation");
        }
    }
}

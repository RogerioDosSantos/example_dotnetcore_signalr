using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;
using dotnetcore_signalr;
using System.Collections.Generic;

namespace dotnetcore_signalr.Qa
{
    public class TestShellClient
    {
        ShellClient _shellClient = null;

        public TestShellClient()
        {
            ILogger logger = Mock.Of<ILogger>();            
            _shellClient = new ShellClient(new Uri("http://localhost:5001"));
        }

        [Fact]
        public void TestExample()
        {
            Assert.NotNull(_shellClient);
            Assert.True(_shellClient.SendCommand("ls"));
            List<string> commandResults = _shellClient.GetCommandResults();
            Assert.NotNull(commandResults);
            Assert.NotEmpty(commandResults);
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Sample.Scenarios
{
    [SuppressMessage("Microsoft.Performance", "CA1812", Justification = "Used.")]
    sealed class HostingScenario : IScenario
    {
        public async Task RunAsync()
        {
            await Task.Yield();

            await TerminalHost.CreateDefaultBuilder().RunTerminalAsync().ConfigureAwait(false);
        }
    }
}

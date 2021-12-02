using static System.Unix.UnixConstants;
using static System.Unix.UnixPInvoke;

namespace System.Drivers.Unix;

sealed class UnixTerminalReader : DefaultTerminalReader
{
    public int Handle { get; }

    public override bool IsRedirected => UnixTerminalUtility.IsRedirected(Handle);

    readonly object _lock;

    readonly UnixTerminalDriver _driver;

    public UnixTerminalReader(string name, int handle, object @lock, UnixTerminalDriver driver)
        : base(name)
    {
        Handle = handle;
        _lock = @lock;
        _driver = driver;
    }

    protected override unsafe void ReadCore(Span<byte> data, out int count)
    {
        if (data.IsEmpty)
        {
            count = 0;

            return;
        }

        long ret;

        lock (_lock)
        {
            while (true)
            {
                fixed (byte* p = data)
                {
                    while ((ret = read(Handle, p, (nuint)data.Length)) == -1 &&
                        Marshal.GetLastPInvokeError() == EINTR)
                    {
                        // Retry in case we get interrupted by a signal.
                    }

                    if (ret != -1)
                        break;

                    var err = Marshal.GetLastPInvokeError();

                    // The descriptor was probably redirected to a program that ended. Just silently ignore this
                    // situation.
                    //
                    // The strange condition where errno is zero happens e.g. on Linux if the process is killed while
                    // blocking in the read system call.
                    if (err is 0 or EPIPE)
                    {
                        ret = 0;

                        break;
                    }

                    // The file descriptor has been configured as non-blocking. Instead of busily trying to read over
                    // and over, poll until we can write and then try again.
                    if (_driver.PollHandle(err, Handle, POLLIN))
                        continue;

                    throw new TerminalException($"Could not read from {Name}: {new Win32Exception(err).Message}");
                }
            }
        }

        count = (int)ret;
    }
}

using System;
using System.IO;

namespace Microsoft.Extensions.Logging.Terminal
{
    public sealed class TerminalLoggerOptions
    {
        readonly ref struct Decorator
        {
            readonly TerminalWriter _writer;

            public Decorator(TerminalWriter writer, (byte R, byte G, byte B)? colors)
            {
                _writer = writer;

                if (colors is (byte r, byte g, byte b))
                    writer.ForegroundColor(r, g, b);
            }

            public void Dispose()
            {
                _writer?.ResetAttributes();
            }
        }

        LogLevel _logToStandardErrorThreshold;

        public LogLevel LogToStandardErrorThreshold
        {
            get => _logToStandardErrorThreshold;
            set
            {
                if (!Enum.IsDefined(typeof(LogLevel), value))
                    throw new ArgumentOutOfRangeException(nameof(value));

                _logToStandardErrorThreshold = value;
            }
        }

        public bool DisableColors { get; set; }

        public bool UseUtcTimestamp { get; set; }

        public TerminalLoggerWriter Writer { get; set; } = DefaultWriter;

        public static void DefaultWriter(TerminalLoggerOptions options, TerminalWriter writer,
            in TerminalLoggerEntry entry)
        {
            Decorator Decorate((byte R, byte G, byte B)? colors)
            {
                return !options.DisableColors && colors != null ? new Decorator(writer, colors.Value) : default;
            }

            writer.Write("[");

            using (_ = Decorate((127, 127, 127)))
                writer.Write(entry.Timestamp.ToString("HH:mm:ss.fff"));

            writer.Write("][");

            (byte R, byte G, byte B)? colors = entry.LogLevel switch
            {
                LogLevel.Trace => (127, 0, 127),
                LogLevel.Debug => (0, 127, 255),
                LogLevel.Information => (255, 255, 255),
                LogLevel.Warning => (255, 255, 0),
                LogLevel.Error => (255, 63, 0),
                LogLevel.Critical => (255, 0, 0),
                _ => null,
            };

            using (_ = Decorate(colors))
            {
                writer.Write(entry.LogLevel switch
                {
                    LogLevel.Trace => "TRC",
                    LogLevel.Debug => "DBG",
                    LogLevel.Information => "INF",
                    LogLevel.Warning => "WRN",
                    LogLevel.Error => "ERR",
                    LogLevel.Critical => "CRT",
                    _ => "UNK",
                });
            }

            writer.Write("][");

            using (_ = Decorate((233, 233, 233)))
                writer.Write(entry.CategoryName);

            writer.Write("][");

            using (_ = Decorate((0, 155, 155)))
                writer.Write(entry.EventId);

            writer.Write("] ");

            if (entry.Message is string m)
                writer.WriteLine("{0}", m);

            if (entry.Exception is Exception e)
                writer.WriteLine(e);
        }
    }
}
namespace Sample.Scenarios;

[SuppressMessage("Performance", "CA1812")]
sealed class AttributeScenario : Scenario
{
    public override Task RunAsync()
    {
        Terminal.Out(
            new ControlBuilder()
                .SetForegroundColor(255, 0, 0)
                .PrintLine("This text is red.")
                .ResetAttributes()
                .SetForegroundColor(0, 255, 0)
                .PrintLine("This text is green.")
                .ResetAttributes()
                .SetForegroundColor(0, 0, 255)
                .PrintLine("This text is blue.")
                .ResetAttributes()
                .SetDecorations(bold: true)
                .PrintLine("This text is bold.")
                .ResetAttributes()
                .SetDecorations(faint: true)
                .PrintLine("This text is faint.")
                .ResetAttributes()
                .SetDecorations(italic: true)
                .PrintLine("This text is in italics.")
                .ResetAttributes()
                .SetDecorations(underline: true)
                .PrintLine("This text is underlined.")
                .ResetAttributes()
                .SetDecorations(blink: true)
                .PrintLine("This text is blinking.")
                .ResetAttributes()
                .SetDecorations(invert: true)
                .PrintLine("This text is inverted.")
                .ResetAttributes()
                .SetDecorations(invisible: true)
                .PrintLine("This text is invisible.")
                .SetDecorations(strike: true)
                .PrintLine("This text is struck through.")
                .ResetAttributes()
                .SetDecorations(overline: true)
                .PrintLine("This text is overlined.")
                .ResetAttributes()
                .SetDecorations(doubleUnderline: true)
                .PrintLine("This text is doubly underlined.")
                .ResetAttributes()
                .OpenHyperlink(new("https://google.com"))
                .PrintLine("This is a Google hyperlink.")
                .CloseHyperlink()
                .OpenHyperlink(new("https://google.com"), "google")
                .PrintLine("This is a Google hyperlink with an ID.")
                .CloseHyperlink());

        return Task.CompletedTask;
    }
}

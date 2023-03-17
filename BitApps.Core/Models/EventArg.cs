namespace BitApps.Core.Models;

public class ShowBalloonEventArgs : EventArgs
{
    public string? Title { get; private set; }
    public string? Text { get; private set; }

    public ShowBalloonEventArgs(string title, string text)
    {
        Title = title;
        Text = text;
    }
}

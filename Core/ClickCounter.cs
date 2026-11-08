namespace WindowsCpsTool;

internal sealed class ClickCounter
{
    private readonly object sync = new();
    private readonly Queue<long> clicks = new();

    public void AddClick()
    {
        lock (sync)
        {
            var now = Environment.TickCount64;
            clicks.Enqueue(now);
            RemoveExpired(now);
        }
    }

    public int GetCountLastSecond()
    {
        lock (sync)
        {
            RemoveExpired(Environment.TickCount64);
            return clicks.Count;
        }
    }

    public void Clear()
    {
        lock (sync)
        {
            clicks.Clear();
        }
    }

    private void RemoveExpired(long now)
    {
        while (clicks.Count > 0 && now - clicks.Peek() > 1000)
        {
            clicks.Dequeue();
        }
    }
}

namespace FC_Solver.Utility;

public class ThreadTimeout {
    public delegate object? TimeoutThread();
    public static T? Timeout<T>(TimeoutThread thread, TimeSpan duration) {
        var span = duration;

        for (;;) {
            span = span.Subtract(TimeSpan.FromMilliseconds(50));

            var obj = thread();

            if (obj != null) return (T)obj!;
            
            Thread.Sleep(50);
            if (span.TotalMilliseconds <= 0)
                return default;

            return (T)obj!;
        }
    }
    
    public static async Task<T?> TimeoutAsync<T>(TimeoutThread thread, TimeSpan duration) {
        var span = duration;

        for (;;) {
            span = span.Subtract(TimeSpan.FromMilliseconds(50));

            var obj = thread();

            if (obj != null) return (T)obj!;
            
            Thread.Sleep(50);
            if (span.TotalMilliseconds <= 0)
                return default;

            return (T)obj!;
        }
    }
}
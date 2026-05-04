namespace WifiCdmx.Application.Extensions;

/// <summary>
/// Functional extensions for Task — enables map and parallel composition on async values.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Maps a completed Task value using a pure function.
    /// Allows building async pipelines without breaking the functional style.
    /// </summary>
    public static async Task<TResult> MapAsync<TSource, TResult>(
        this Task<TSource> task,
        Func<TSource, TResult> mapper) =>
        mapper(await task);

    /// <summary>
    /// Runs three Tasks concurrently and returns their results as a tuple.
    /// Reduces total async wait time through parallel execution.
    /// </summary>
    public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(
        this (Task<T1>, Task<T2>, Task<T3>) tasks)
    {
        await Task.WhenAll(tasks.Item1, tasks.Item2, tasks.Item3);
        return (tasks.Item1.Result, tasks.Item2.Result, tasks.Item3.Result);
    }
}

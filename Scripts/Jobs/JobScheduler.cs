using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class JobScheduler : Node
{
    readonly struct Request
    {
        public readonly IJob Job;
        public readonly TaskCompletionSource<object> TaskCompletionSource;
        public readonly CancellationToken CancellationToken;

        public Request(
            IJob job,
            TaskCompletionSource<object> taskCompletionSource,
            CancellationToken cancellationToken
        )
        {
            Job = job;
            TaskCompletionSource = taskCompletionSource;
            CancellationToken = cancellationToken;
        }
    }

    readonly HashSet<JobExecutor> _idleExecutors = new();

    readonly Queue<Request> _pendingCommonRequests = new();
    readonly Dictionary<JobExecutor, Queue<Request>> _pendingExecutorRequests = new();

    public void AddExecutor(JobExecutor executor)
    {
        _pendingExecutorRequests[executor] = new();
        ProcessIdleExecutor(executor);
    }

    public void RemoveExecutor(JobExecutor executor)
    {
        _pendingExecutorRequests.Remove(executor);
    }

    public Task Execute(IJob job, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<object>();
        ProcessPendingRequest(new Request(job, tcs, ct));
        return tcs.Task;
    }

    public Task Execute(IJob job, JobExecutor executor, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<object>();
        ProcessPendingRequest(new Request(job, tcs, ct), executor);
        return tcs.Task;
    }

    void ProcessPendingRequest(in Request request)
    {
        var executor = _idleExecutors.FirstOrDefault();
        if (executor is not null)
        {
            _idleExecutors.Remove(executor);
            _ = RunExecutor(executor, request);
        }
        else
            _pendingCommonRequests.Enqueue(request);
    }

    void ProcessPendingRequest(in Request request, JobExecutor executor)
    {
        if (_idleExecutors.Remove(executor))
            _ = RunExecutor(executor, request);
        else
            _pendingExecutorRequests[executor].Enqueue(request);
    }

    async Task RunExecutor(JobExecutor executor, Request request)
    {
        try
        {
            await executor.Execute(request.Job, request.CancellationToken);
            request.TaskCompletionSource.SetResult(null);
        }
        catch (TaskCanceledException)
        {
            request.TaskCompletionSource.SetCanceled();
        }
        finally
        {
            if (executor.CanProcess())
                ProcessIdleExecutor(executor);
        }
    }

    void ProcessIdleExecutor(JobExecutor executor)
    {
        if (TryDequeueRequestForExecutor(executor, out var request))
            _ = RunExecutor(executor, request);
        else
            ParkExecutor(executor);
    }

    bool TryDequeueRequestForExecutor(JobExecutor executor, out Request request)
    {
        var executorRequests = _pendingExecutorRequests[executor];
        while (executorRequests.TryDequeue(out var dequeuedRequest))
        {
            request = dequeuedRequest;
            return true;
        }
        while (_pendingCommonRequests.TryDequeue(out var dequeuedRequest))
        {
            request = dequeuedRequest;
            return true;
        }
        request = default;
        return false;
    }

    void ParkExecutor(JobExecutor executor)
    {
        _idleExecutors.Add(executor);
    }
}

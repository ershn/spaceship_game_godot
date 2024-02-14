using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class JobScheduler : Node
{
    readonly struct Request : IDisposable
    {
        public readonly IJob Job;
        public readonly JobExecutor Executor;
        public readonly TaskCompletionSource TaskCompletionSource;
        public readonly CancellationToken CancellationToken;
        readonly CancellationTokenRegistration _cancellationTokenRegistration;

        public Request(
            IJob job,
            TaskCompletionSource taskCompletionSource,
            CancellationToken cancellationToken,
            JobExecutor executor = null
        )
        {
            Job = job;
            Executor = executor;
            TaskCompletionSource = taskCompletionSource;
            CancellationToken = cancellationToken;

            _cancellationTokenRegistration = cancellationToken.Register(
                taskCompletionSource.SetCanceled
            );
        }

        public void Dispose()
        {
            _cancellationTokenRegistration.Dispose();
        }

        public bool Canceled => CancellationToken.IsCancellationRequested;
    }

    readonly HashSet<JobExecutor> _idleExecutors = [];

    readonly Deque<Request> _pendingCommonRequests = new();
    readonly Dictionary<JobExecutor, Deque<Request>> _pendingExecutorRequests = [];

    public Task Execute(IJob job, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource();
        ProcessRequest(new Request(job, tcs, ct));
        return tcs.Task;
    }

    public Task Execute(IJob job, JobExecutor executor, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource();
        ProcessRequest(new Request(job, tcs, ct, executor));
        return tcs.Task;
    }

    void ProcessRequest(in Request request)
    {
        if (TryAcquireExecutor(request, out var executor))
            _ = ExecuteRequest(request, executor);
        else
            EnqueueRequest(request);
    }

    async Task ExecuteRequest(Request request, JobExecutor executor)
    {
        try
        {
            await executor.Execute(request.Job, request.CancellationToken);
            request.TaskCompletionSource.SetResult();
            request.Dispose();
        }
        catch (TaskCanceledException)
        {
            var requeued = false;
            if (request.Job.Retriable && !request.Canceled)
                requeued = RequeueRequest(request);

            if (!requeued)
            {
                request.TaskCompletionSource.SetCanceled();
                request.Dispose();
            }

            throw;
        }
        finally
        {
            if (executor.CanProcess())
                ProcessExecutor(executor);
        }
    }

    void ProcessExecutor(JobExecutor executor)
    {
        if (TryDequeueRequestForExecutor(executor, out var request))
            _ = ExecuteRequest(request, executor);
        else
            ReleaseExecutor(executor);
    }

    void EnqueueRequest(in Request request)
    {
        if (request.Executor is not null)
            _pendingExecutorRequests[request.Executor].EnqueueBack(request);
        else
            _pendingCommonRequests.EnqueueBack(request);
    }

    bool RequeueRequest(in Request request)
    {
        if (request.Executor is not null)
        {
            if (!_pendingExecutorRequests.TryGetValue(request.Executor, out var requests))
                return false;

            requests.EnqueueFront(request);
            return true;
        }
        else
        {
            _pendingCommonRequests.EnqueueFront(request);
            return true;
        }
    }

    bool TryDequeueRequestForExecutor(JobExecutor executor, out Request request)
    {
        if (
            TryDequeueRequest(_pendingExecutorRequests[executor], out var dequeuedRequest)
            || TryDequeueRequest(_pendingCommonRequests, out dequeuedRequest)
        )
        {
            request = dequeuedRequest;
            return true;
        }
        else
        {
            request = default;
            return false;
        }
    }

    bool TryDequeueRequest(Deque<Request> requests, out Request request)
    {
        while (requests.TryDequeueFront(out var dequeuedRequest))
        {
            if (dequeuedRequest.Canceled)
            {
                dequeuedRequest.Dispose();
            }
            else
            {
                request = dequeuedRequest;
                return true;
            }
        }
        request = default;
        return false;
    }

    public void AddExecutor(JobExecutor executor)
    {
        _pendingExecutorRequests[executor] = new();
        ProcessExecutor(executor);
    }

    public void RemoveExecutor(JobExecutor executor)
    {
        if (_pendingExecutorRequests.Remove(executor, out var requests))
        {
            foreach (var request in requests)
            {
                request.TaskCompletionSource.TrySetCanceled();
                request.Dispose();
            }
        }
    }

    bool TryAcquireExecutor(in Request request, out JobExecutor executor)
    {
        if (request.Executor is not null)
        {
            executor = request.Executor;
            return _idleExecutors.Remove(executor);
        }
        else
        {
            executor = _idleExecutors.FirstOrDefault();
            return _idleExecutors.Remove(executor);
        }
    }

    void ReleaseExecutor(JobExecutor executor)
    {
        _idleExecutors.Add(executor);
    }
}

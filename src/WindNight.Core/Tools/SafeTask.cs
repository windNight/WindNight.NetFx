using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using WindNight.Core.ExceptionExt;
using WindNight.Core.@internal;

namespace WindNight.Core.Tools
{
    public class SafeTask : Task
    {
        public SafeTask(Action action) : base(action)
        {
        }

        public SafeTask(Action action, CancellationToken cancellationToken) : base(action, cancellationToken)
        {
        }

        public SafeTask(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, cancellationToken, creationOptions)
        {
        }

        public SafeTask(Action action, TaskCreationOptions creationOptions) : base(action, creationOptions)
        {
        }

        public SafeTask(Action<object> action, object state) : base(action, state)
        {
        }

        public SafeTask(Action<object> action, object state, CancellationToken cancellationToken) : base(action, state, cancellationToken)
        {
        }

        public SafeTask(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions)
        {
        }

        public SafeTask(Action<object> action, object state, TaskCreationOptions creationOptions) : base(action, state, creationOptions)
        {
        }


        /// <summary>Queues the specified work to run on the thread pool and returns a <see cref="T:System.Threading.Tasks.Task" /> object that represents that work.</summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> parameter was <see langword="null" />.</exception>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        public static async Task Run(Action action, string taskName, Action<string, Exception> errorHandler = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            taskName = FixTaskName(taskName);

            try
            {
                await Run(action).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

            // return CompletedTask;
        }

        /// <summary>Queues the specified work to run on the thread pool and returns a <see cref="T:System.Threading.Tasks.Task" /> object that represents that work. A cancellation token allows the work to be cancelled if it has not yet started.</summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started. <see cref="M:System.Threading.Tasks.Task.Run(System.Action,System.Threading.CancellationToken)" /> does not pass <paramref name="cancellationToken" /> to <paramref name="action" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> parameter was <see langword="null" />.</exception>
        /// <exception cref="T:System.OperationCanceledException">The task has been canceled. This exception is stored into the returned task.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> associated with <paramref name="cancellationToken" /> was disposed.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <returns>A task that represents the work queued to execute in the thread pool.</returns>
        public static async Task Run(Action action, CancellationToken cancellationToken, string taskName, Action<string, Exception> errorHandler = null)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            taskName = FixTaskName(taskName);

            try
            {
                await Run(action, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                LogHelper.Error($"{taskName} Handler Error: Operation was canceled", ex);

            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

        }

        /// <summary>Queues the specified work to run on the thread pool and returns a <see cref="T:System.Threading.Tasks.Task`1" /> object that represents that work. A cancellation token allows the work to be cancelled if it has not yet started.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <typeparam name="TResult">The return type of the task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter is <see langword="null" />.</exception>
        /// <returns>A task object that represents the work queued to execute in the thread pool.</returns>
        public static async Task<TResult> Run<TResult>(Func<TResult> function, string taskName, Action<string, Exception> errorHandler = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            taskName = FixTaskName(taskName);

            try
            {
                return await Run<TResult>(function).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"TaskName({taskName}) Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"TaskName({taskName}) Handler Error {ex.Message}", ex);
                }
            }

            return await FromResult(default(TResult));

        }

        /// <summary>Queues the specified work to run on the thread pool and returns a <see langword="Task(TResult)" /> object that represents that work.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started. <see cref="M:System.Threading.Tasks.Task.Run``1(System.Func{``0},System.Threading.CancellationToken)" /> does not pass <paramref name="cancellationToken" /> to <paramref name="action" />.</param>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter is <see langword="null" />.</exception>
        /// <exception cref="T:System.OperationCanceledException">The task has been canceled. This exception is stored into the returned task.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> associated with <paramref name="cancellationToken" /> was disposed.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <returns>A <see langword="Task(TResult)" /> that represents the work queued to execute in the thread pool.</returns>
        public static async Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken, string taskName, Action<string, Exception> errorHandler = null)
        {

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            taskName = FixTaskName(taskName);

            try
            {
                return await Run<TResult>(function, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                LogHelper.Error($"{taskName} Handler Error: Operation was canceled", ex);

            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);
                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

            return await FromResult(default(TResult));

        }



        /// <summary>Queues the specified work to run on the thread pool and returns a proxy for the task returned by <paramref name="function" />.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was <see langword="null" />.</exception>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function" />.</returns>
        public static async Task Run(Func<Task?> function, string taskName, Action<string, Exception> errorHandler = null)
        {

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            taskName = FixTaskName(taskName);

            try
            {
                await Task.Run(function).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

        }

        /// <summary>Queues the specified work to run on the thread pool and returns a proxy for the task returned by <paramref name="function" />. A cancellation token allows the work to be cancelled if it has not yet started.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started. <see cref="M:System.Threading.Tasks.Task.Run(System.Func{System.Threading.Tasks.Task},System.Threading.CancellationToken)" /> does not pass <paramref name="cancellationToken" /> to <paramref name="action" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was <see langword="null" />.</exception>
        /// <exception cref="T:System.OperationCanceledException">The task has been canceled. This exception is stored into the returned task.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> associated with <paramref name="cancellationToken" /> was disposed.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function" />.</returns>
        public static async Task Run(Func<Task?> function, CancellationToken cancellationToken, string taskName, Action<string, Exception> errorHandler = null)
        {

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            taskName = FixTaskName(taskName);

            try
            {
                await Run(function, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                LogHelper.Error($"{taskName} Handler Error: Operation was canceled", ex);

            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

        }

        /// <summary>Queues the specified work to run on the thread pool and returns a proxy for the <see langword="Task(TResult)" /> returned by <paramref name="function" />. A cancellation token allows the work to be cancelled if it has not yet started.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was <see langword="null" />.</exception>
        /// <returns>A <see langword="Task(TResult)" /> that represents a proxy for the <see langword="Task(TResult)" /> returned by <paramref name="function" />.</returns>
        public static async Task<TResult> Run<TResult>(Func<Task<TResult>?> function, string taskName, Action<string, Exception> errorHandler = null)
        {

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            taskName = FixTaskName(taskName);

            try
            {
                await Run<TResult>(function).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

            return await FromResult(default(TResult));

        }

        /// <summary>Queues the specified work to run on the thread pool and returns a proxy for the <see langword="Task(TResult)" /> returned by <paramref name="function" />.</summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started. <see cref="M:System.Threading.Tasks.Task.Run``1(System.Func{System.Threading.Tasks.Task{``0}},System.Threading.CancellationToken)" /> does not pass <paramref name="cancellationToken" /> to <paramref name="action" />.</param>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was <see langword="null" />.</exception>
        /// <exception cref="T:System.OperationCanceledException">The task has been canceled. This exception is stored into the returned task.</exception>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource" /> associated with <paramref name="cancellationToken" /> was disposed.</exception>
        /// <exception cref="T:System.Threading.Tasks.TaskCanceledException">The task has been canceled.</exception>
        /// <returns>A <see langword="Task(TResult)" /> that represents a proxy for the <see langword="Task(TResult)" /> returned by <paramref name="function" />.</returns>
        public static async Task<TResult> Run<TResult>(Func<Task<TResult>?> function, CancellationToken cancellationToken, string taskName, Action<string, Exception> errorHandler = null)
        {

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }
            taskName = FixTaskName(taskName);

            try
            {
                return await Run<TResult>(function, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                LogHelper.Error($"{taskName} Handler Error: Operation was canceled", ex);

            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.Flatten().InnerExceptions)
                {
                    LogHelper.Error($"{taskName} Handler Error: {innerEx.Message}", innerEx);
                }
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    try
                    {
                        errorHandler.Invoke(taskName, ex);

                    }
                    catch
                    {

                    }
                }
                else
                {
                    LogHelper.Error($"{taskName} Handler Error {ex.Message}", ex);
                }
            }

            return await FromResult(default(TResult));
        }

        static string FixTaskName(string taskName)
        {
            var stackTrace = new StackTrace(true);
            var list = new List<string>();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var method = frame.GetMethod();
                list.Add($"Frame {i}: {method?.DeclaringType?.Name}.{method?.Name} (in {method?.Module?.Name})");
            }

            if (taskName.IsNullOrEmpty())
            {
                try
                {
                    var stackFrame = new StackFrame(4, true); // 获取调用者的堆栈帧
                    MethodBase callerMethod = stackFrame.GetMethod();
                    if (callerMethod != null)
                    {
                        taskName = callerMethod.Name; // 获取调用方法名
                        var module = callerMethod.Module;
                        if (module != null)
                        {
                            taskName += $" (in {module.Name})"; // 添加 DLL 名
                        }
                    }
                    else
                    {
                        taskName = "Unknown_Caller"; // 备用名称
                    }
                }
                catch
                {
                    taskName = "Unknown_Caller"; // 备用名称
                }
            }

            return taskName;
        }


    }



}

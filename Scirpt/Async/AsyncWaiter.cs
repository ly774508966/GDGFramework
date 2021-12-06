using System.Collections;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GDG.Async
{
    public interface IAsyncWaiter:IDisposable
    {
        bool IsCompleted{ get; }
        bool IsFinish { get;}
        bool IsCancelled { get; }
        bool IsFaulted { get;}
        CancellationToken Token { get;}
        void CancelWait();
        Task StartWaitAsync();
    }
    public abstract class AsyncWaiter:IAsyncWaiter
    {
        public delegate void FinishEventHandle();
        public delegate void CancelledEventHandle();
        public delegate void FaultedEventHandle();
        public event FinishEventHandle Finished;
        public event CancelledEventHandle Cancelled;
        public event FaultedEventHandle Faulted;
        public object Current { get; set; }
        public bool IsCompleted{ get => IsFinish && !IsCancelled && !IsFaulted; }
        public bool IsFinish { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsFaulted { get; private set; }
        public CancellationToken Token { get; private set; }
        private CancellationTokenSource ctSource;
        private class AsyncWaiterImpl : AsyncWaiter
        {
            private Func<IEnumerator> onWait;
            public AsyncWaiterImpl(Func<IEnumerator> onWait)
            {
                this.onWait = onWait;
            }
            protected override IEnumerator OnWait()
            {
                return onWait?.Invoke();
            }
        }
        public static AsyncWaiter StartWaitAsync(Func<IEnumerator> onWait)
        {
            var waiter = (AsyncWaiter) new AsyncWaiterImpl(onWait);
            waiter.StartWaitAsync().GetAwaiter();
            return waiter;
        }
        public async Task StartWaitAsync()
        {
            IsFinish = false;
            IsCancelled = false;
            ctSource = new CancellationTokenSource();
            Token = ctSource.Token;
            await AsyncWait();
        }
        private async Task AsyncWait()
        {
            var waiter = OnWait();
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if(waiter.Current is IYieldInstruction yieldInstruction)
                        {
                            if(yieldInstruction.CanMoveNext)
                            {
                                if(!waiter.MoveNext())
                                    break;                                
                            }
                        }
                        else
                        {
                            if(!waiter.MoveNext())
                                break;                            
                        }
                        Token.ThrowIfCancellationRequested();
                        Current = waiter.Current;
                    }
                    IsFinish = true;
                });
            }
            catch (ArgumentException)
            {
                IsFinish = true;
                IsFaulted = true;
                Faulted?.Invoke();                
            }
            catch (OperationCanceledException)
            {
                IsFinish = true;
                IsCancelled = true;
                Cancelled?.Invoke();
            }
            Finished?.Invoke();
            Dispose();
        }
        public void CancelWait()
        {
            ctSource.Cancel();
        }
        protected abstract IEnumerator OnWait();

        public void Dispose()
        {
            ctSource.Dispose();
        }
    }
    public abstract class AsyncWaiter<T>:IAsyncWaiter
    {
        public delegate void FinishEventHandle();
        public delegate void CancelledEventHandle();
        public delegate void FaultedEventHandle();
        public event FinishEventHandle Finished;
        public event CancelledEventHandle Cancelled;
        public event FaultedEventHandle Faulted;
        public T Current { get; set; }
        public bool IsCompleted{ get => IsFinish && !IsCancelled && !IsFaulted; }
        public bool IsFinish { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsFaulted { get; private set; }
        public CancellationToken Token { get; private set; }
        private CancellationTokenSource ctSource;
        private class AsyncWaiterImpl<T0> : AsyncWaiter<T0>
        {
            private Func<IEnumerator<T0>> onWait;
            public AsyncWaiterImpl(Func<IEnumerator<T0>> onWait)
            {
                this.onWait = onWait;
            }
            protected override IEnumerator<T0> OnWait()
            {
                return onWait?.Invoke();
            }
        }
        public static AsyncWaiter<T0> StartWaitAsync<T0>(Func<IEnumerator<T0>> onWait)
        {
            var waiter = (AsyncWaiter<T0>) new AsyncWaiterImpl<T0>(onWait);
            waiter.StartWaitAsync().GetAwaiter();
            return waiter;
        }
        public async Task StartWaitAsync()
        {
            IsFinish = false;
            IsCancelled = false;
            ctSource = new CancellationTokenSource();
            Token = ctSource.Token;
            await AsyncWait();
        }
        private async Task AsyncWait()
        {
            var waiter = OnWait();
            try
            {
                await Task.Run(() =>
                {
                    while (true)
                    {
                        if(waiter.Current is IYieldInstruction yieldInstruction)
                        {
                            if(yieldInstruction.CanMoveNext)
                            {
                                if(!waiter.MoveNext())
                                    break;                                
                            }
                        }
                        else
                        {
                            if(!waiter.MoveNext())
                                break;                            
                        }
                        Token.ThrowIfCancellationRequested();
                        Current = waiter.Current;
                    }
                    IsFinish = true;
                });
            }
            catch (ArgumentException)
            {
                IsFinish = true;
                IsFaulted = true;
                Faulted?.Invoke();
            }
            catch (OperationCanceledException)
            {
                IsFinish = true;
                IsCancelled = true;
                Cancelled?.Invoke();
            }
            Finished?.Invoke();
            Dispose();
        }
        public void CancelWait()
        {
            ctSource.Cancel();
        }
        protected abstract IEnumerator<T> OnWait();

        public void Dispose()
        {
            ctSource?.Dispose();
        }
    }
}
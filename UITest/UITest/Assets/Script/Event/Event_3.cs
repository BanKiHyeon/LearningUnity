using System;
using System.Collections.Generic;
using System.Threading;


public delegate void EventDelegate<T1, T2, T3>(T1 t1, T2 t2, T3 t3);

public class LiveEvent<T1, T2, T3>
{
    protected readonly List<Wrapper<T1, T2, T3>> wrapperList = new List<Wrapper<T1, T2, T3>>();

    public LiveEvent()
    {

    }

    public virtual void AddObserver(object obj, EventDelegate<T1, T2, T3> observer)
    {
        wrapperList.Add(new Wrapper<T1, T2, T3>(obj, SynchronizationContext.Current, observer));
    }

    public virtual void RemoveAllObserver(object obj)
    {
        wrapperList.RemoveAll(p =>
        {
            if (!p.WeakReference.IsAlive) return true;
            if (p.WeakReference.Target.Equals(obj)) return true;
            return false;
        });
    }

    private void RemoveAllAtRelease()
    {
        wrapperList.RemoveAll(p =>
        {
            if (!p.WeakReference.IsAlive) return true;
            return false;
        });
    }

    public virtual void Post(T1 t1, T2 t2, T3 t3)
    {
        for (int i = 0; i < wrapperList.Count; ++i)
        {
            wrapperList[i].Process(t1, t2, t3);
        }
        RemoveAllAtRelease();
    }
}

public class Event<T1, T2, T3> : LiveEvent<T1, T2, T3>
{
    private T1 t1;
    public T1 First { get { return t1; } }

    private T2 t2;
    public T2 Second { get { return t2; } }

    private T3 t3;
    public T3 Third { get { return t3; } }

    public Event()
    {
        this.t1 = default;
        this.t2 = default;
        this.t3 = default;
    }

    public Event(T1 t1, T2 t2, T3 t3)
    {
        Set(t1, t2, t3);
    }

    public override void AddObserver(object obj, EventDelegate<T1, T2, T3> observer)
    {
        base.AddObserver(obj, observer);
        SynchronizationContext.Current.Post(d => observer?.Invoke(t1, t2, t3), null);
    }

    public override void Post(T1 t1, T2 t2, T3 t3)
    {
        Set(t1, t2, t3);
        base.Post(t1, t2, t3);
    }

    private void Set(T1 t1, T2 t2, T3 t3)
    {
        this.t1 = t1;
        this.t2 = t2;
        this.t3 = t3;
    }
}

public class Wrapper<T1, T2, T3>
{
    private readonly WeakReference weakReference;
    private readonly EventDelegate<T1, T2, T3> observer;
    private readonly SynchronizationContext synchronizationContext;

    public WeakReference WeakReference { get { return weakReference; } }
    public Wrapper(object obj, SynchronizationContext synchronizationContext, 
        EventDelegate<T1, T2, T3> observer)
    {
        weakReference = new WeakReference(obj);
        this.synchronizationContext = synchronizationContext;
        this.observer = observer;
    }

    public void Process(T1 t1, T2 t2, T3 t3)
    {
        if (weakReference.IsAlive)
        {
            if (synchronizationContext != null)
            {
                synchronizationContext?.Post(d => observer?.Invoke(t1, t2, t3), null);
            }
            else
            {
                observer?.Invoke(t1, t2, t3);
            }
        }
    }
}
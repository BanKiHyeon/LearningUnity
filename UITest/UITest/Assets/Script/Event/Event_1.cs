using System;
using System.Collections.Generic;
using System.Threading;


public delegate void EventDelegate<T1>(T1 t1);

public class LiveEvent<T1>
{
    protected readonly List<Wrapper<T1>> wrapperList = new List<Wrapper<T1>>();

    public LiveEvent()
    {

    }

    public virtual void AddObserver(object obj, EventDelegate<T1> observer)
    {
        wrapperList.Add(new Wrapper<T1>(obj, SynchronizationContext.Current, observer));
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

    public virtual void Post(T1 t1)
    {
        for (int i = 0; i < wrapperList.Count; ++i)
        {
            wrapperList[i].Process(t1);
        }
        RemoveAllAtRelease();
    }
}

public class Event<T1> : LiveEvent<T1>
{
    private T1 t1;
    public T1 Value { get { return t1; } }

    public Event()
    {
        this.t1 = default;
    }

    public Event(T1 t1)
    {
        Set(t1);
    }

    public override void AddObserver(object obj, EventDelegate<T1> observer)
    {
        base.AddObserver(obj, observer);
        SynchronizationContext.Current.Post(d => observer?.Invoke(t1), null);
    }

    public override void Post(T1 t1)
    {
        Set(t1);
        base.Post(t1);
    }

    private void Set(T1 t1)
    {
        this.t1 = t1;
    }
}

public class Wrapper<T1>
{
    private readonly WeakReference weakReference;
    private readonly EventDelegate<T1> observer;
    private readonly SynchronizationContext synchronizationContext;

    public WeakReference WeakReference { get { return weakReference; } }
    public Wrapper(object obj, SynchronizationContext synchronizationContext, 
        EventDelegate<T1> observer)
    {
        weakReference = new WeakReference(obj);
        this.synchronizationContext = synchronizationContext;
        this.observer = observer;
    }

    public void Process(T1 t1)
    {
        if (weakReference.IsAlive)
        {
            if (synchronizationContext != null)
            {
                synchronizationContext?.Post(d => observer?.Invoke(t1), null);
            }
            else
            {
                observer?.Invoke(t1);
            }
        }
    }
}
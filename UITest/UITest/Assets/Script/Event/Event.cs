using System;
using System.Collections.Generic;
using System.Threading;


public delegate void EventDelegate();

public class LiveEvent
{
    protected readonly List<Wrapper> wrapperList = new List<Wrapper>();

    public LiveEvent()
    {

    }

    public virtual void AddObserver(object obj, EventDelegate observer)
    {
        wrapperList.Add(new Wrapper(obj, SynchronizationContext.Current, observer));
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

    public virtual void Post()
    {
        for (int i = 0; i < wrapperList.Count; ++i)
        {
            wrapperList[i].Process();
        }
        RemoveAllAtRelease();
    }
}

public class Event : LiveEvent
{

    public Event()
    {

    }


    public override void AddObserver(object obj, EventDelegate observer)
    {
        base.AddObserver(obj, observer);
        SynchronizationContext.Current.Post(d => observer?.Invoke(), null);
    }

    public override void Post()
    {
        base.Post();
    }

}

public class Wrapper
{
    private readonly WeakReference weakReference;
    private readonly EventDelegate observer;
    private readonly SynchronizationContext synchronizationContext;

    public WeakReference WeakReference { get { return weakReference; } }
    public Wrapper(object obj, SynchronizationContext synchronizationContext, 
        EventDelegate observer)
    {
        weakReference = new WeakReference(obj);
        this.synchronizationContext = synchronizationContext;
        this.observer = observer;
    }

    public void Process()
    {
        if (weakReference.IsAlive)
        {
            if (synchronizationContext != null)
            {
                synchronizationContext?.Post(d => observer?.Invoke(), null);
            }
            else
            {
                observer?.Invoke();
            }
        }
    }
}
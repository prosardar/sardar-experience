using System;
using System.Threading;

public abstract class BaseLowJob : MarshalByRefObject
{
    public abstract int Process(string cmd);    
}
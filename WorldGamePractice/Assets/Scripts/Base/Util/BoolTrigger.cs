using System;
using UnityEngine.Events;

public class BoolTrigger
{
    private bool _value=false;
    
    public bool Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_value)
            {
                _value = false;
                OnTrue?.Invoke();
            }
        }
    }
    public event UnityAction OnTrue;
    
}

public class BoolTriggerWithParameter<T>
{
    private bool _value = false;
    private T Parameter = default(T);
    public bool Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_value)
            {
                _value = false;
                OnTrue?.Invoke(Parameter);
            }
        }
    }
    public event UnityAction<T> OnTrue;

    public BoolTriggerWithParameter(T parameter)
    {
        this.Parameter = parameter;
    }
}
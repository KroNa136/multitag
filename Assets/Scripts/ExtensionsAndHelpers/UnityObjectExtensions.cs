using System;

public static class UnityObjectExtensions
{
    public static T Bind<T>(this T obj, Action action) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke();

        return obj;
    }

    public static T Bind<T, TArg>(this T obj, Action<TArg> action, TArg arg) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2>(this T obj, Action<TArg1, TArg2> action, TArg1 arg1, TArg2 arg2) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3>(this T obj, Action<TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4>(this T obj, Action<TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3, arg4);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5>(this T obj, Action<TArg1, TArg2, TArg3, TArg4, TArg5> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this T obj, Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this T obj, Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this T obj, Action<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

        return obj;
    }

    public static T Bind<T>(this T obj, Action<T> action) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj);

        return obj;
    }

    public static T Bind<T, TArg>(this T obj, Action<T, TArg> action, TArg arg) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2>(this T obj, Action<T, TArg1, TArg2> action, TArg1 arg1, TArg2 arg2) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3>(this T obj, Action<T, TArg1, TArg2, TArg3> action, TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4>(this T obj, Action<T, TArg1, TArg2, TArg3, TArg4> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3, arg4);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5>(this T obj, Action<T, TArg1, TArg2, TArg3, TArg4, TArg5> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3, arg4, arg5);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(this T obj, Action<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3, arg4, arg5, arg6);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7>(this T obj, Action<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7);

        return obj;
    }

    public static T Bind<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8>(this T obj, Action<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8> action, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) where T : UnityEngine.Object
    {
        if (obj != null)
            action?.Invoke(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);

        return obj;
    }

    public static TResult GetOrDefault<T, TResult>(this T obj, Func<T, TResult> func) where T : UnityEngine.Object
    {
        return obj != null ? func(obj) : default;
    }

    public static TResult GetOrDefault<T, TArg, TResult>(this T obj, Func<T, TArg, TResult> func, TArg arg) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TResult>(this T obj, Func<T, TArg1, TArg2, TResult> func, TArg1 arg1, TArg2 arg2) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TArg4, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3, arg4) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3, arg4, arg5) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3, arg4, arg5, arg6) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7) : default;
    }

    public static TResult GetOrDefault<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) where T : UnityEngine.Object
    {
        return obj != null ? func(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) : default;
    }

    public static bool TryGet<T, TResult>(this T obj, Func<T, TResult> func, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj);
        return true;
    }

    public static bool TryGet<T, TArg, TResult>(this T obj, Func<T, TArg, TResult> func, TArg arg, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TResult>(this T obj, Func<T, TArg1, TArg2, TResult> func, TArg1 arg1, TArg2 arg2, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TArg4, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3, arg4);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3, arg4, arg5);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3, arg4, arg5, arg6);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        return true;
    }

    public static bool TryGet<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult>(this T obj, Func<T, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TResult> func, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8, out TResult value) where T : UnityEngine.Object
    {
        if (obj == null)
        {
            value = default;
            return false;
        }

        value = func(obj, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        return true;
    }
}

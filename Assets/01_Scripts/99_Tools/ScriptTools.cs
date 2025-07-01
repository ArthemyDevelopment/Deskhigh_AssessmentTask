using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptTools
{
    
    //Dictionary with wait for seconds, with the time as keys
    private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

    
    //Get a wait for second element in case it was already used before, optimizing its use
    public static WaitForSeconds GetWait(float time) 
    {
        if (WaitDictionary.TryGetValue(time, out var wait))return wait;

        WaitDictionary[time] = new WaitForSeconds(time);
        return WaitDictionary[time];
    }
    
    
    /// <summary>
    /// Custom Slerp implementation that allows to go the longest instead of the shortest way.
    /// </summary>
    public static Vector2 CustomSlerp(Vector2 start, Vector2 end, float pos, bool shortWay = true)
    {
        var dot = Vector2.Dot(start, end);
        if ((shortWay && dot < 0) || (!shortWay && dot > 0)) {
            start = ScalarMultiply(start, -1f);
            dot *= -1f;
        }

        dot = Mathf.Clamp(dot, -1f, 1f);
        var theta0 = Mathf.Acos(dot);
        var theta = theta0 * pos;

        var s1 = Mathf.Sin(theta)  / Mathf.Sin(theta0);
        var s0 = Mathf.Cos(theta) - dot * s1;

        var temp = Add(ScalarMultiply(start, s0), ScalarMultiply(end, s1));
        temp.Normalize();
        return Add(ScalarMultiply(start, s0), ScalarMultiply(end, s1));
    }

    /// <summary>
    /// Multiply all components by a scalar value.
    /// </summary>
    public static Vector2 ScalarMultiply(Vector2 q, float scalar)
    {
        q.x *= scalar;
        q.y *= scalar;
        return q;
    }

    /// <summary>
    /// Add two quaternions.
    /// </summary>
    public static Vector2 Add(Vector2 one, Vector2 two)
    {
        one.x += two.x;
        one.y += two.y;
        return one;
    }
    
    
    
    
    
}

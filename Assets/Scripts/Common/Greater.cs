using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class IntGreater : IComparer<int>
{
    public int Compare(int x, int y)
    {
        return y - x;
    }
}

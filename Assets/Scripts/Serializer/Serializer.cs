using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

static class Serializer
{
    public static string serialize(Vector2 _value)
    {
        return "(" + _value.x + ',' + _value.y + ')';
    }

    public static int deserialize(string _serial, out Vector2 _value)
    {
        var _serialEnd = _serial.LastIndexOf(')');

        var _commaIdx = _serial.LastIndexOf(',', _serialEnd - 1);
        _value.y = float.Parse(_serial.Substring(_commaIdx + 1, _serialEnd - _commaIdx - 1));

        var _serialStart = _serial.LastIndexOf('(', _commaIdx - 1);
        _value.x = float.Parse(_serial.Substring(_serialStart + 1, _commaIdx - _serialStart - 1));

        return _serialStart;
    }
}

using System;
using UnityEngine;

[Serializable]
public struct ObfuscatedInt
{
    [SerializeField] private int _mask;
    [SerializeField] private int _masked;


    public ObfuscatedInt(int initialValue)
    {
        _mask = 0;
        _masked = 0;

        Value = initialValue;
    }

    public static int GetRandomInteger()
    {
        return UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    public void SetMaskAndMasked(int mask, int masked)
    {
        _mask = mask;
        _masked = masked;
    }
    public readonly int GetInternalMask()
    {
        return _mask;
    }

    public int GetInternalMasked()
    {
        return _masked;
    }

    public int Value
    {
        readonly get
        {
            if (_mask == 0 && _masked != 0)
            {
                // Debug.LogError("ObfuscatedInt: Mask is zero, but value is not!");
                return 0;
            }
            return _masked ^ _mask;
        }
        set
        {
            if (_mask == 0)
            {
                _mask = GetRandomInteger();
            }
            _masked = value ^ _mask;
        }
    }

    public static implicit operator int(ObfuscatedInt obfuscatedInt) => obfuscatedInt.Value;
    public static implicit operator ObfuscatedInt(int value) => new() { Value = value };

    public static bool operator ==(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value == obfuscatedInt2.Value;

    public static bool operator !=(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value != obfuscatedInt2.Value;

    public static bool operator <(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value < obfuscatedInt2.Value;

    public static bool operator >(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value > obfuscatedInt2.Value;

    public static bool operator <=(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value <= obfuscatedInt2.Value;

    public static bool operator >=(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => obfuscatedInt1.Value >= obfuscatedInt2.Value;

    public static ObfuscatedInt operator +(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => new() { Value = obfuscatedInt1.Value + obfuscatedInt2.Value };

    public static ObfuscatedInt operator -(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => new() { Value = obfuscatedInt1.Value - obfuscatedInt2.Value };

    public static ObfuscatedInt operator *(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => new() { Value = obfuscatedInt1.Value * obfuscatedInt2.Value };

    public static ObfuscatedInt operator /(ObfuscatedInt obfuscatedInt1, ObfuscatedInt obfuscatedInt2)
    => new() { Value = obfuscatedInt1.Value / obfuscatedInt2.Value };

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override readonly bool Equals(object obj)
    {
        if (obj is ObfuscatedInt obfuscatedInt)
        {
            return Value == obfuscatedInt.Value;
        }
        return false;
    }
    public readonly bool Equals(ObfuscatedInt other)
    {
        return Value == other.Value;
    }

    public override readonly string ToString()
    {
        return Value.ToString();
    }

}

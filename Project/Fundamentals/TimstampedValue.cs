using System;
using System.Collections.Generic;
using System.Text;

namespace KeepDisplayOn.Fundamentals
{
    public class TimstampedValue<T>
    {
        // TODO: Add unit test

        public T Value { get; protected set; }

        public DateTimeOffset Timestamp { get; protected set; }

        public TimeSpan ValidInterval { get; set; } = TimeSpan.FromMinutes(10);

        public bool Invalidated { get; set; } = false;

        public bool IsValid => !Invalidated && (DateTimeOffset.UtcNow - Timestamp) < ValidInterval;

        public TimstampedValue(T value)
        {
            SetValue(value);
        }

        public TimstampedValue(T value, TimeSpan validInterval)
        {
            SetValue(value);
            ValidInterval = validInterval;
        }

        public DateTimeOffset SetValue(T newValue)
        {
            Value = newValue;
            Timestamp = DateTimeOffset.UtcNow;
            return Timestamp;
        }

        public static implicit operator T(TimstampedValue<T> v) => v.Value;
        public static explicit operator TimstampedValue<T>(T v) => new TimstampedValue<T>(v);

    }
}

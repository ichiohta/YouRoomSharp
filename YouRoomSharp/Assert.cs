using System;

namespace YouRoomSharp
{
    internal static class Assert
    {
        public static void IsNotNull(object value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void IsNotNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(parameterName);
        }

        public static void IsNotNullOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(parameterName);
        }

        public static void IsTrue(bool predicate, Func<Exception> prepareException)
        {
            if (!predicate)
                throw prepareException();
        }
    }
}

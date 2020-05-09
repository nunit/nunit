---
uid: RandomizerMethods
---

The **Randomizer** object exposed by `TestContext.CurrentContext.Random` extends `System.Random` to provide random data for a wide range of numeric types as well as enums and strings. Each test context has access to its own **Randomizer** which is used to generate random values for the `RandomAttribute` as well as for use by the user calling its methods. 

The benefit of using **Randomizer** rather than `System.Random` directly is twofold:
1. A wide range of types are provided in a uniform manner.
2. Randomizer produces repeatable values for the test run so long as no tests are changed and the same seed is used to initialize the run. A future extension is planned, which would provide repeatability at the individual method level, even if other methods are changed.

### Int (System.Int32)

##### `Next()`
Returns the next random non-negative int. (Inherited from `System.Random`)

##### `Next(int max)`
Returns the next random non-negative int less than max, which must be positive. (Inherited from `System.Random`)

##### `Next(int min, int max)`
Returns the next random int in the range min to max, excluding max. (Inherited from `System.Random`)

### UInt (System.UInt32)

##### `NextUInt()`
Returns the next random uint.

##### `NextUInt(uint max)`
Returns the next random uint less than max.

##### `NextUInt(uint min, uint max)`
Returns the next random uint in the range min to max, excluding max.

### Long (System.Int64)

##### `NextLong()`
Returns the next random non-negative long.

##### `NextLong(long max)`
Returns the next random non-negative long less than max, which must be positive.

##### `NextLong(long min, long max)`
Returns the next random long in the range min to max, excluding max.

### ULong (System.UInt64)

##### `NextULong()`
Returns the next random ulong.

##### `NextULong(ulong max)`
Returns the next random ulong less than max.

##### `NextULong(ulong min, ulong max)`
Returns the next random ulong in the range min to max, excluding max.

### Short (System.Int16)

##### `NextShort()`
Returns the next random non-negative short.

##### `NextShort(short max)`
Returns the next random non-negative short less than max, which must be positive.

##### `NextShort(short min, short max)`
Returns the next random short in the range min to max, excluding max.

### UShort (System.UInt16)

##### `NextUShort()`
Returns the next random ushort.

##### `NextUShort(ushort max)`
Returns the next random ushort less than max.

##### `NextUShort(ushort min, ushort max)`
Returns the next random ushort in the range min to max, excluding max.

### SByte (System.SByte)

##### `NextSByte()`
Returns the next random non-negative sbyte.

##### `NextSByte(sbyte max)`
Returns the next random non-negative sbyte less than max, which must be positive.

##### `NextSByte(sbyte min, sbyte max)`
Returns the next random sbyte in the range min to max, excluding max.

### Byte (System.Byte)

##### `NextByte()`
Returns the next random byte.

##### `NextByte(byte max)`
Returns the next random byte less than max.

##### `NextByte(byte min, byte max)`
Returns the next random byte in the range min to max, excluding max.

### Double (System.Double)

##### `NextDouble()`
Returns the next double in the range 0.0 to 1.0, exclusive. (Inherited from `System.Random`.)

##### `NextDouble(double max)`
Returns the next non-negative double less than max.

##### `NextDouble(double min, double max)`
Returns the next double in the range min to max, excluding max.

### Float (System.Float)

##### `NextFloat()`
Returns the next float in the range 0.0f to 1.0f, exclusive.

##### `NextFloat(float max)`
Returns the next non-negative float less than max.

##### `NextFloat(float min, float max)`
Returns the next float in the range min to max, excluding max.

### Decimal (System.Decimal)

##### `NextDecimal()`
Returns the next non-negative random decimal.

##### `NextDecimal(decimal max)`
Returns the next non-negative decimal less than max.

##### `NextDecimal(decimal min, decimal max)`
Returns the next decimal in the range min to max, excluding max.

###### Notes on Decimal Implementation
1. In the current implementation, the scale is always set to zero. That is, the values are all integral, with no decimal places. This may be enhanced in a future release by allowing the user to specify the scale.
2. In the third form, an exception is currently thrown if the range is greater than `decimal.MaxValue`. This can only occur if min is negative and max is positive.

### Bool (System.Boolean)

##### `NextBool()`
Returns a random bool with equal probability of `true` or `false`.

##### `NextBool(double probability)`
Returns a random bool with the specified probability of being `true`. Probability argument must be in the range 0.0 to 1.0, inclusive.

### Enum (System.Enum)

##### `NextEnum<T>()`
Returns a random enum value of type T. All the values of the enum are returned with equal probability. Note that this may not be useful in all cases for enums with the `FlagsAttribute` specified.

##### `NextEnum(Type type)`
Returns a random enum value of the type specified as an object, which the caller will normally cast to the specified type.

### String (System.String)

##### `GetString()`
Returns a random string of default length, composed using a default set of characters. In the current implementation, the default length is hard-coded as 25 and the default characters are "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_".

##### `GetString(int length)`
Returns a random string of the specified length.

##### `GetString(int outputLength, string allowedChars)`
Returns a random string of the specified length using the characters in the string given as the second argument.

### Guid (System.Guid)

##### `NextGuid()` (available in version 3.8)
Generates a version 4 Guid conforming the [RFC 4122](https://tools.ietf.org/html/rfc4122#section-4.4). Version 4 Guids are made of random data.
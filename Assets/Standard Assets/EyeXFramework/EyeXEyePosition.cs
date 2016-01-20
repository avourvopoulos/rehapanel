//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

/// <summary>
/// Value object for eye position data, containing the eye positions of the left
/// and right eyes and a timestamp.
/// </summary>
public sealed class EyeXEyePosition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EyeXEyePosition"/> class.
    /// </summary>
    /// <param name="leftEye">The eye position of the left eye, see <see cref="EyeXSingleEyePosition"/>.</param>
    /// <param name="leftEyeNormalized">The normalized eye position of the left eye, see <see cref="EyeXSingleEyePosition"/>.</param>
    /// <param name="rightEye">The eye position of the right eye, see <see cref="EyeXSingleEyePosition"/>.</param>
    /// <param name="rightEyeNormalized">The normalized eye position of the right eye, see <see cref="EyeXSingleEyePosition"/>.</param>
    /// <param name="timestamp">The timestamp of the eye position data.</param>
    public EyeXEyePosition(EyeXSingleEyePosition leftEye, EyeXSingleEyePosition leftEyeNormalized, 
        EyeXSingleEyePosition rightEye, EyeXSingleEyePosition rightEyeNormalized, double timestamp)
    {
        LeftEye = leftEye;
        LeftEyeNormalized = leftEyeNormalized;
        RightEye = rightEye;
        RightEyeNormalized = rightEyeNormalized;
        Timestamp = timestamp;
    }

    /// <summary>
    /// Gets a value representing an invalid EyeXEyePosition
    /// </summary>
    public static EyeXEyePosition Invalid
    {
        get
        {
            return new EyeXEyePosition(EyeXSingleEyePosition.Invalid, EyeXSingleEyePosition.Invalid, 
                EyeXSingleEyePosition.Invalid, EyeXSingleEyePosition.Invalid, double.NaN);
        }
    }

    /// <summary>
    /// Gets the position of the left eye.
    /// </summary>
    public EyeXSingleEyePosition LeftEye { get; private set; }

    /// <summary>
    /// Gets the normalized position of the left eye.
    /// </summary>
    public EyeXSingleEyePosition LeftEyeNormalized { get; private set; }

    /// <summary>
    /// Gets the position of the right eye.
    /// </summary>
    public EyeXSingleEyePosition RightEye { get; private set; }

    /// <summary>
    /// Gets the normalized position of the right eye.
    /// </summary>
    public EyeXSingleEyePosition RightEyeNormalized { get; private set; }

    /// <summary>
    /// Gets the point in time when the data point was captured. Milliseconds.
    /// </summary>
    public double Timestamp { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the data point is valid or not.
    /// </summary>
    public bool IsValid
    {
        get { return !double.IsNaN(Timestamp); }
    }
}

/// <summary>
/// Position of an eye in 3D space.
/// <para>
/// The position is taken relative to the center of the screen where the eye tracker is mounted.
/// </para>
/// </summary>
public sealed class EyeXSingleEyePosition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EyeXSingleEyePosition"/> class.
    /// </summary>
    /// <param name="isValid">Flag indicating if the eye position is valid. (Sometimes only a single eye is tracked.)</param>
    /// <param name="x">X coordinate of the eye position, in millimeters.</param>
    /// <param name="y">Y coordinate of the eye position, in millimeters.</param>
    /// <param name="z">Z coordinate of the eye position, in millimeters.</param>
    public EyeXSingleEyePosition(bool isValid, float x, float y, float z)
    {
        IsValid = isValid;
        X = x;
        Y = y;
        Z = z;
    }

    /// <summary>
    /// Gets a value representing an invalid eye position.
    /// </summary>
    public static EyeXSingleEyePosition Invalid
    {
        get
        {
            return new EyeXSingleEyePosition(false, float.NaN, float.NaN, float.NaN);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the eye position is valid. (Sometimes only a single eye is tracked.)
    /// </summary>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets the X coordinate of the eye position, in millimeters.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Gets the Y coordinate of the eye position, in millimeters.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Gets the Z coordinate of the eye position, in millimeters.
    /// </summary>
    public float Z { get; private set; }
}

//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.EyeX.Framework;

/// <summary>
/// Holds a fixation point.
/// </summary>
public class EyeXFixationPoint
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="gazePoint">The location of the fixation. See also <seealso cref="EyeXGazePoint"/>.</param>
    /// <param name="timestamp">The timestamp of the fixation.</param>
    /// <param name="eventType">The event type of the original fixation event.</param>
    public EyeXFixationPoint(EyeXGazePoint gazePoint, double timestamp, FixationDataEventType eventType)
    {
        GazePoint = gazePoint;
        Timestamp = timestamp;
        EventType = eventType;
    }

    /// <summary>
    /// Gets a value representing an invalid fixation point.
    /// </summary>
    public static EyeXFixationPoint Invalid
    {
        get
        {
            return new EyeXFixationPoint(EyeXGazePoint.Invalid, double.NaN, (FixationDataEventType)0);
        }
    }

    /// <summary>
    /// Gets the location of the fixation. See also <seealso cref="EyeXGazePoint"/>.
    /// </summary>
    public EyeXGazePoint GazePoint { get; private set; }

    /// <summary>
    /// Gets the timestamp of the fixation.
    /// <para>
    /// The timestamp can be used to uniquely identify this point from a previous point.
    /// </para>
    /// </summary>
    public double Timestamp { get; private set; }

    /// <summary>
    /// Gets a value indicating the kind of event this fixation point originates from.
    /// <para>
    /// - Begin: This is the beginning of a fixation
    /// </para><para>
    /// - Data: This is an ongoing fixation
    /// </para><para>
    /// - End: This is the end of a fixation
    /// </para>
    /// </summary>
    public FixationDataEventType EventType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the point is valid or not.
    /// </summary>
    public bool IsValid
    {
        get { return GazePoint.IsValid; }
    }
}

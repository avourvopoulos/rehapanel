//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

/// <summary>
/// Interface of an EyeX data provider.
/// </summary>
/// <typeparam name="T">Type of the provided data value object.</typeparam>
public interface IEyeXDataProvider<T>
{
    /// <summary>
    /// Gets the latest value of the data stream. The value is never null but 
    /// it might be invalid.
    /// </summary>
    T Last { get; }

    /// <summary>
    /// Starts the provider. Data will continuously be updated in the Last
    /// property as events are received from the EyeX Engine.
    /// </summary>
    void Start();

    /// <summary>
    /// Requests to stop the data provider. If there are no other clients
    /// that are currently requesting the provider to keep providing data,
    /// the provider will stop the stream of data from the EyeX Engine and
    /// stop updating the Last property.
    /// </summary>
    void Stop();
}

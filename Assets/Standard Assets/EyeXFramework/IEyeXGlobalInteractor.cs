//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using Tobii.EyeX.Client;

/// <summary>
/// Interface of a global interactor. Used by the EyeXHost.
/// </summary>
internal interface IEyeXGlobalInteractor
{
    /// <summary>
    /// Event raised when the state of the global interactor has changed
    /// in a way so that the EyeX Engine has to be updated with the new
    /// parameter settings.
    /// <para>
    /// For example: when the state has changed so the global interactor
    /// should be removed from the EyeX Engine, that is the data stream
    /// should be stopped. 
    /// </para>
    /// </summary>
    event EventHandler Updated;

    /// <summary>
    /// Gets the unique ID of the interactor that provides the data stream.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Creates an EyeX Interactor object from the properties and behaviors of
    /// this global interactor and adds it to the provided snapshot.
    /// </summary>
    /// <param name="snapshot">The <see cref="Snapshot"/> to
    /// add the interactor to.</param>
    /// <param name="forceDeletion">If true, forces the interactor to be deleted.</param>
    void AddToSnapshot(Snapshot snapshot, bool forceDeletion);

    /// <summary>
    /// Handles interaction events.
    /// </summary>
    /// <param name="event_">The <see cref="InteractionEvent"/> instance containing the event data.</param>
    /// <param name="viewportPosition">The position of the top-left corner of the viewport, in operating system coordinates.</param>
    /// <param name="viewportPixelsPerDesktopPixel">The scaling factor between the Unity viewport and operating system coordinate systems.</param>
    void HandleEvent(InteractionEvent event_, Vector2 viewportPosition, Vector2 viewportPixelsPerDesktopPixel);
}

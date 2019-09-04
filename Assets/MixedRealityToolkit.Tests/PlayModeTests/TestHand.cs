﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if !WINDOWS_UWP
// When the .NET scripting backend is enabled and C# projects are built
// The assembly that this file is part of is still built for the player,
// even though the assembly itself is marked as a test assembly (this is not
// expected because test assemblies should not be included in player builds).
// Because the .NET backend is deprecated in 2018 and removed in 2019 and this
// issue will likely persist for 2018, this issue is worked around by wrapping all
// play mode tests in this check.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Tests
{
    // Utility class to use a simulated hand
    internal class TestHand
    {
        private Handedness handedness;
        private Vector3 position;
        private Quaternion rotation = Quaternion.identity;
        private ArticulatedHandPose.GestureId gestureId = ArticulatedHandPose.GestureId.Open;
        private InputSimulationService simulationService;

        public TestHand(Handedness handedness)
        {
            this.handedness = handedness;
            simulationService = PlayModeTestUtilities.GetInputSimulationService();
        }

        public Vector3 GetVelocity()
        {
            var hand = simulationService.GetHandDevice(handedness);
            return hand.Velocity;
        }

        public IEnumerator Show(Vector3 position, bool waitForFixedUpdate = true)
        {
            this.position = position;
            yield return PlayModeTestUtilities.ShowHand(handedness, simulationService, gestureId, position);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator Hide(bool waitForFixedUpdate = true)
        {
            yield return PlayModeTestUtilities.HideHand(handedness, simulationService);
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator MoveTo(Vector3 newPosition, int numSteps = 30, bool waitForFixedUpdate = true)
        {
            Vector3 oldPosition = position;
            position = newPosition;
            for (var iter = PlayModeTestUtilities.MoveHandFromTo(oldPosition, newPosition, numSteps, gestureId, handedness, simulationService); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public IEnumerator Move(Vector3 delta, int numSteps = 30)
        {
            for (var iter = MoveTo(position + delta, numSteps); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
        }

        public IEnumerator SetRotation(Quaternion newRotation, int numSteps = 30)
        {
            Quaternion oldRotation = rotation;
            rotation = newRotation;
            yield return PlayModeTestUtilities.SetHandRotation(oldRotation, newRotation, position, gestureId, handedness, numSteps, simulationService);
        }

        public IEnumerator SetGesture(ArticulatedHandPose.GestureId newGestureId, bool waitForFixedUpdate = true)
        {
            gestureId = newGestureId;
            for (var iter = PlayModeTestUtilities.MoveHandFromTo(position, position, 1, gestureId, handedness, simulationService); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            if (waitForFixedUpdate)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        /// <summary>
        /// Combined sequence of pinching and unpinching
        /// </summary>
        /// <returns></returns>
        public IEnumerator Click()
        {
            yield return SetGesture(ArticulatedHandPose.GestureId.Pinch);
            yield return null;
            yield return SetGesture(ArticulatedHandPose.GestureId.Open);
            yield return null;
        }

        /// <summary>
        /// Combined sequence of pinching, moving, and releasing.
        /// </summary>
        /// <param name="positionToRelease">The position to which the hand moves while pinching</param>
        /// <param name="waitForFinalFixedUpdate">Wait for a final physics update after releasing</param>
        /// <param name="numSteps">Number of steps of the hand movement</param>
        public IEnumerator GrabAndThrowAt(Vector3 positionToRelease, bool waitForFinalFixedUpdate, int numSteps = 30)
        {
            for (var iter = SetGesture(ArticulatedHandPose.GestureId.Pinch); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            for (var iter = MoveTo(positionToRelease, numSteps); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
            for (var iter = SetGesture(ArticulatedHandPose.GestureId.Open, waitForFinalFixedUpdate); iter.MoveNext(); )
            {
                yield return iter.Current;
            }
        }

        public T GetPointer<T>() where T : class, IMixedRealityPointer
        {
            var hand = simulationService.GetHandDevice(handedness);
            foreach (var pointer in hand.InputSource.Pointers)
            {
                if (pointer is T)
                {
                    return pointer as T;
                }
            }
            return null;
        }
    }
}
#endif
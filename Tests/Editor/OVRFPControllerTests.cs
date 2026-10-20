namespace FuzzPhyte.XR.OVR.Tests
{
    using System.Collections.Generic;
    using System.Reflection;
    using NUnit.Framework;
    using Oculus.Interaction;
    using Oculus.Interaction.Input;
    using UnityEngine;

    public class OVRFPControllerTests
    {
        private GameObject root;
        private FPXRControllerEventManager manager;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Controller regression tests");
            root.SetActive(false);
            manager = root.AddComponent<FPXRControllerEventManager>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(root);

        [TestCase(XRHandedness.Left, XRButton.Trigger)]
        [TestCase(XRHandedness.Right, XRButton.Trigger)]
        [TestCase(XRHandedness.Left, XRButton.Grip)]
        [TestCase(XRHandedness.Right, XRButton.Grip)]
        public void AnalogHoldPublishesOnlyTransitions(XRHandedness hand, XRButton button)
        {
            var listener = root.AddComponent<OVRFPControllerListener>();
            listener.FPControllerManager = manager;
            var events = new List<XRInteractionStatus>();
            manager.ButtonPressed += (h, b) =>
            {
                Assert.That(h, Is.EqualTo(hand));
                Assert.That(b, Is.EqualTo(button));
                events.Add(XRInteractionStatus.Select);
            };
            manager.ButtonReleased += (h, b) => events.Add(XRInteractionStatus.Unselect);

            foreach (float value in new[] { 0f, 0.3f, 0.8f, 0.5f, 0f, 0f, 0.7f, 0f })
            {
                typeof(OVRFPControllerListener).GetMethod("CheckAnalogButtonState",
                    BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(listener, new object[] { null, hand, button, value });
            }

            Assert.That(events, Is.EqualTo(new[] { XRInteractionStatus.Select,
                XRInteractionStatus.Unselect, XRInteractionStatus.Select, XRInteractionStatus.Unselect }));
        }

        [TestCase(XRHandedness.Left)]
        [TestCase(XRHandedness.Right)]
        public void ButtonUnlockRestoresAllMatchesAndUnsubscribes(XRHandedness hand)
        {
            var feedback = root.AddComponent<FPXRControllerFeedback>();
            typeof(FPXRControllerEventManager).GetField(
                hand == XRHandedness.Left ? "leftControllerFeedback" : "rightControllerFeedback",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(manager, feedback);
            var locker = root.AddComponent<OVRFPControllerLocker>();
            var first = CreateSelector(ControllerButtonUsage.TriggerButton);
            var second = CreateSelector(ControllerButtonUsage.TriggerButton);
            var grip = CreateSelector(ControllerButtonUsage.GripButton);
            var opposite = CreateSelector(ControllerButtonUsage.TriggerButton);
            var selectors = hand == XRHandedness.Left ? locker.LeftControllerSelectors : locker.RightControllerSelectors;
            selectors.AddRange(new[] { first, second, grip });
            (hand == XRHandedness.Left ? locker.RightControllerSelectors : locker.LeftControllerSelectors).Add(opposite);
            locker.SetupWithData(manager);

            manager.LockControllerButton(hand, XRButton.Trigger);
            manager.LockControllerButton(hand, XRButton.Trigger);
            Assert.That(first.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.None));
            Assert.That(second.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.None));
            Assert.That(grip.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.GripButton));
            Assert.That(opposite.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.TriggerButton));

            manager.UnlockControllerButton(hand, XRButton.Trigger);
            Assert.That(first.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.TriggerButton));
            Assert.That(second.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.TriggerButton));

            manager.LockControllerButton(hand, XRButton.Trigger);
            typeof(OVRFPControllerLocker).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(locker, null);
            manager.UnlockControllerButton(hand, XRButton.Trigger);
            Assert.That(first.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.None));
            Assert.That(second.Selector.ControllerButtonUsage, Is.EqualTo(ControllerButtonUsage.None));
        }

        private OVRFPSelectorWrapper CreateSelector(ControllerButtonUsage usage)
        {
            var child = new GameObject("Selector");
            child.transform.SetParent(root.transform);
            var selector = child.AddComponent<ControllerSelector>();
            selector.ControllerButtonUsage = usage;
            var wrapper = child.AddComponent<OVRFPSelectorWrapper>();
            wrapper.SetupFromData(selector);
            return wrapper;
        }
    }
}

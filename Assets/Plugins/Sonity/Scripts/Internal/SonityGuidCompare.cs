// Created by Victor Engström
// Copyright 2026 Sonigon AB
// http://www.sonity.org/

namespace Sonity.Internal {

    public static class SonityGuidCompare {

        public static bool IsSameNotNull(SoundContainerBase objectA, SoundContainerBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundEventBase objectA, SoundEventBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundPolyGroupBase objectA, SoundPolyGroupBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundDataGroupBase objectA, SoundDataGroupBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundMixBase objectA, SoundMixBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundVolumeGroupBase objectA, SoundVolumeGroupBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundTagBase objectA, SoundTagBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        public static bool IsSameNotNull(SoundPhysicsConditionBase objectA, SoundPhysicsConditionBase objectB) {
            if (objectA == null || objectB == null) {
                return false;
            } else {
                return objectA.internals.sonityGuid == objectB.internals.sonityGuid;
            }
        }

        // SoundPreset is Editor only, skip

        // SoundAreaBase
        // SoundAreaTagBase
        // SoundReverbBase
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#pragma warning disable 0618

namespace MonkiiLoader
{
    public static class bHaptics
    {
        public static bool WasError { get => false; }

        [Obsolete("MonkiiLoader.bHaptics.IsPlaying is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPlayingAny instead.")]
        public static bool IsPlaying()
            => bHapticsLib.bHapticsManager.IsPlayingAny();
        [Obsolete("MonkiiLoader.bHaptics.IsPlaying(string) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPlaying instead.")]
        public static bool IsPlaying(string key)
            => bHapticsLib.bHapticsManager.IsPlaying(key);

        [Obsolete("MonkiiLoader.bHaptics.IsDeviceConnected(DeviceType, bool) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsDeviceConnected instead.")]
        public static bool IsDeviceConnected(DeviceType type, bool isLeft = true) => IsDeviceConnected(DeviceTypeToPositionType(type, isLeft));
        [Obsolete("MonkiiLoader.bHaptics.IsDeviceConnected(PositionType) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsDeviceConnected instead.")]
        public static bool IsDeviceConnected(PositionType type)
            => bHapticsLib.bHapticsManager.IsDeviceConnected(PositionTypeToPositionID(type));

        [Obsolete("MonkiiLoader.bHaptics.IsFeedbackRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.IsPatternRegistered instead.")]
        public static bool IsFeedbackRegistered(string key)
            => bHapticsLib.bHapticsManager.IsPatternRegistered(key);

        [Obsolete("MonkiiLoader.bHaptics.RegisterFeedback is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternFromJson instead.")]
        public static void RegisterFeedback(string key, string tactFileStr)
        {
            TinyJSON.ProxyArray proxyArray = new TinyJSON.ProxyArray();
            proxyArray["project"] = TinyJSON.Decoder.Decode(tactFileStr);
            bHapticsLib.bHapticsManager.RegisterPatternFromJson(key, TinyJSON.Encoder.Encode(proxyArray));
        }

        [Obsolete("MonkiiLoader.bHaptics.RegisterFeedbackFromTactFile is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternFromJson instead.")]
        public static void RegisterFeedbackFromTactFile(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternFromJson(key, tactFileStr);
        [Obsolete("MonkiiLoader.bHaptics.RegisterFeedbackFromTactFileReflected is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.RegisterPatternSwappedFromJson instead.")]
        public static void RegisterFeedbackFromTactFileReflected(string key, string tactFileStr)
            => bHapticsLib.bHapticsManager.RegisterPatternSwappedFromJson(key, tactFileStr);

        [Obsolete("MonkiiLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key)
            => bHapticsLib.bHapticsManager.PlayRegistered(key);
        [Obsolete("MonkiiLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, int startTimeMillis)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, startTimeMillis);
        [Obsolete("MonkiiLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, string altKey, ScaleOption option)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey,
                new bHapticsLib.ScaleOption { Duration = option.Duration, Intensity = option.Intensity });
        [Obsolete("MonkiiLoader.bHaptics.SubmitRegistered is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.PlayRegistered instead.")]
        public static void SubmitRegistered(string key, string altKey, ScaleOption sOption, RotationOption rOption)
            => bHapticsLib.bHapticsManager.PlayRegistered(key, altKey,
                new bHapticsLib.ScaleOption { Duration = sOption.Duration, Intensity = sOption.Intensity }, 
                new bHapticsLib.RotationOption { OffsetAngleX = rOption.OffsetX, OffsetY = rOption.OffsetY });

        [Obsolete("MonkiiLoader.bHaptics.TurnOff is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.StopPlayingAll instead.")]
        public static void TurnOff()
            => bHapticsLib.bHapticsManager.StopPlayingAll();
        [Obsolete("MonkiiLoader.bHaptics.TurnOff(string) is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.StopPlaying instead.")]
        public static void TurnOff(string key)
            => bHapticsLib.bHapticsManager.StopPlaying(key);

        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, byte[] bytes, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), bytes, durationMillis);
        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, byte[] bytes, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, PositionTypeToPositionID(position), bytes);


        private static Converter<DotPoint, bHapticsLib.DotPoint> DotPointConverter = new Converter<DotPoint, bHapticsLib.DotPoint>((x) 
            => new bHapticsLib.DotPoint 
            {
                Index = x.Index,
                Intensity = x.Intensity 
            });
        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, List<DotPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, List<DotPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, PositionTypeToPositionID(position), points.ConvertAll(DotPointConverter));


        private static Converter<PathPoint, bHapticsLib.PathPoint> PathPointConverter = new Converter<PathPoint, bHapticsLib.PathPoint>((x)
            => new bHapticsLib.PathPoint
            {
                X = x.X,
                Y = x.Y,
                Intensity = x.Intensity,
                MotorCount = x.MotorCount
            });
        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, DeviceType type, bool isLeft, List<PathPoint> points, int durationMillis) => Submit(key, DeviceTypeToPositionType(type, isLeft), points, durationMillis);
        [Obsolete("MonkiiLoader.bHaptics.Submit is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.Play instead.")]
        public static void Submit(string key, PositionType position, List<PathPoint> points, int durationMillis)
            => bHapticsLib.bHapticsManager.Play(key, durationMillis, PositionTypeToPositionID(position), (bHapticsLib.DotPoint[])null, points.ConvertAll(PathPointConverter));

        [Obsolete("MonkiiLoader.bHaptics.GetCurrentFeedbackStatus is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.GetDeviceStatus instead.")]
        public static FeedbackStatus GetCurrentFeedbackStatus(DeviceType type, bool isLeft = true) => GetCurrentFeedbackStatus(DeviceTypeToPositionType(type, isLeft));
        [Obsolete("MonkiiLoader.bHaptics.GetCurrentFeedbackStatus is Only Here for Compatibility Reasons. Please use bHapticsLib.bHapticsManager.GetDeviceStatus instead.")]
        public static FeedbackStatus GetCurrentFeedbackStatus(PositionType pos)
            => new FeedbackStatus { values = bHapticsLib.bHapticsManager.GetDeviceStatus(PositionTypeToPositionID(pos)) };

        [Obsolete("MonkiiLoader.bHaptics.DeviceTypeToPositionType is Only Here for Compatibility Reasons.")]
        public static PositionType DeviceTypeToPositionType(DeviceType pos, bool isLeft = true)
        => (pos) switch
        {
            DeviceType.Tactal => PositionType.Head,
            DeviceType.TactSuit => PositionType.Vest,
            DeviceType.Tactosy_arms => isLeft ? PositionType.ForearmL : PositionType.ForearmR,
            DeviceType.Tactosy_feet => isLeft ? PositionType.FootL : PositionType.FootR,
            DeviceType.Tactosy_hands => isLeft ? PositionType.HandL : PositionType.HandR,
            _ => PositionType.Head
        };

        [Obsolete("MonkiiLoader.bHaptics.DeviceType is Only Here for Compatibility Reasons.")]
        public enum DeviceType
        {
            None = 0,
            Tactal = 1,
            TactSuit = 2,
            Tactosy_arms = 3,
            Tactosy_hands = 4,
            Tactosy_feet = 5
        }

        [Obsolete("MonkiiLoader.bHaptics.PositionType is Only Here for Compatibility Reasons. Please use bHapticsLib.PositionID instead.")]
        public enum PositionType
        {
            All = 0,
            Left = 1, 
            Right = 2,
            Vest = 3,
            Head = 4,
            Racket = 5,
            HandL = 6,
            HandR = 7,
            FootL = 8, 
            FootR = 9,
            ForearmL = 10,
            ForearmR = 11,
            VestFront = 201,
            VestBack = 202,
            GloveLeft = 203,
            GloveRight = 204,
            Custom1 = 251, 
            Custom2 = 252, 
            Custom3 = 253, 
            Custom4 = 254
        }

        [Obsolete("MonkiiLoader.bHaptics.RotationOption is Only Here for Compatibility Reasons. Please use bHapticsLib.RotationOption instead.")]
        public class RotationOption
        {
            public float OffsetX, OffsetY;

            public RotationOption(float offsetX, float offsetY)
            {
                OffsetX = offsetX;
                OffsetY = offsetY;
            }

            public override string ToString() => "RotationOption { OffsetX=" + OffsetX.ToString() +
                       ", OffsetY=" + OffsetY.ToString() + " }";
        }

        [Obsolete("MonkiiLoader.bHaptics.ScaleOption is Only Here for Compatibility Reasons. Please use bHapticsLib.ScaleOption instead.")]
        public class ScaleOption
        {
            public float Intensity, Duration;

            public ScaleOption(float intensity = 1f, float duration = 1f)
            {
                Intensity = intensity;
                Duration = duration;
            }

            public override string ToString() => "ScaleOption { Intensity=" + Intensity.ToString() +
                       ", Duration=" + Duration.ToString() + " }";
        }

        [Obsolete("MonkiiLoader.bHaptics.DotPoint is Only Here for Compatibility Reasons. Please use bHapticsLib.DotPoint instead.")]
        public class DotPoint
        {
            public int Index, Intensity;

            public DotPoint(int index, int intensity = 50)
            {
                if ((index < 0) || (index > 19))
                    throw new Exception("Invalid argument index : " + index);
                Intensity = MonkiiUtils.Clamp(intensity, 0, 100);
                Index = index;
            }

            public override string ToString() => "DotPoint { Index=" + Index.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [Obsolete("MonkiiLoader.bHaptics.PathPoint is Only Here for Compatibility Reasons. Please use bHapticsLib.PathPoint instead.")]
        [StructLayout(LayoutKind.Sequential)]
        public struct PathPoint
        {
            public float X, Y;
            public int Intensity;
            public int MotorCount;

            public PathPoint(float x, float y, int intensity = 50, int motorCount = 3)
            {
                X = MonkiiUtils.Clamp(x, 0f, 1f);
                Y = MonkiiUtils.Clamp(y, 0f, 1f);
                Intensity = MonkiiUtils.Clamp(intensity, 0, 100);
                MotorCount = MonkiiUtils.Clamp(motorCount, 0, 3);
            }

            public override string ToString() => "PathPoint { X=" + X.ToString() +
                       ", Y=" + Y.ToString() +
                       ", MotorCount=" + MotorCount.ToString() +
                       ", Intensity=" + Intensity.ToString() + " }";
        }

        [Obsolete("MonkiiLoader.bHaptics.FeedbackStatus is Only Here for Compatibility Reasons.")]
        [StructLayout(LayoutKind.Sequential)]
        public struct FeedbackStatus
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public int[] values;
        };

        private static bHapticsLib.PositionID PositionTypeToPositionID(PositionType pos)
            => (pos) switch
            {
                //PositionType.All => bHapticsLib.PositionID.All,
                //PositionType.Left => bHapticsLib.PositionID.Left,
                //PositionType.Right => bHapticsLib.PositionID.Right,
                PositionType.Vest => bHapticsLib.PositionID.Vest,
                PositionType.Head => bHapticsLib.PositionID.Head,
                //PositionType.Racket => bHapticsLib.PositionID.Racket,
                PositionType.HandL => bHapticsLib.PositionID.HandLeft,
                PositionType.HandR => bHapticsLib.PositionID.HandRight,
                PositionType.FootL => bHapticsLib.PositionID.FootLeft,
                PositionType.FootR => bHapticsLib.PositionID.FootRight,
                PositionType.ForearmL => bHapticsLib.PositionID.ArmLeft,
                PositionType.ForearmR => bHapticsLib.PositionID.ArmRight,
                PositionType.VestFront => bHapticsLib.PositionID.VestFront,
                PositionType.VestBack => bHapticsLib.PositionID.VestBack,
                PositionType.GloveLeft => bHapticsLib.PositionID.GloveLeft,
                PositionType.GloveRight => bHapticsLib.PositionID.GloveRight,
                //PositionType.Custom1 => bHapticsLib.PositionID.Custom1,
                //PositionType.Custom2 => bHapticsLib.PositionID.Custom2,
                //PositionType.Custom3 => bHapticsLib.PositionID.Custom3,
                //PositionType.Custom4 => bHapticsLib.PositionID.Custom4,
                _ => bHapticsLib.PositionID.Head
            };
    }
}
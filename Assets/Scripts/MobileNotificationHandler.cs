using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
    using Unity.Notifications.Android;
    using UnityEngine.Android;
#elif UNITY_IOS
    using Unity.Notifications.iOS;
    using UnityEngine.iOS;
#endif

public class MobileNotificationHandler : MonoBehaviour
{
    private const string CHANNEL_ID = "channel_id";

    public static void SendNotifications(string title, string message)
    {
#if UNITY_ANDROID
        SendAndroidNotificatonNow(title, message);
#elif UNITY_IOS
        SendIOSNotificationNow(title, message);
#endif
    }

#if UNITY_ANDROID
    public static void SendAndroidNotificatonNow(string title, string text)
    {
        AndroidNotificationCenter.RegisterNotificationChannel(GetAndroidNotificationChannel());

        AndroidNotificationCenter.SendNotification(GetAndroidNotification(title, text), CHANNEL_ID);
    }

    private static AndroidNotificationChannel GetAndroidNotificationChannel()
    {
        return new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Description = "Generic notifications",
            Importance = Importance.Default,
        };
    }

    private static AndroidNotification GetAndroidNotification(string title, string text)
    {
        return new AndroidNotification()
        {
            Title = title,
            Text = text,
            SmallIcon = "default",
            LargeIcon = "default",
            FireTime = DateTime.Now,
        };
    }
#elif UNITY_IOS
    public static void SendIOSNotificationNow(string title, string text)
    {
        iOSNotificationCenter.ScheduleNotification(GetIOSNotification(title, text, 0));
    }

    private static iOSNotification GetIOSNotification(string title, string body, int delaySeconds)
    {
        return new iOSNotification()
        {
            Title = title,
            Subtitle = "",
            Body = body,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = new TimeSpan(0, 0, delaySeconds),
                Repeats = false
            },
        };
    }
#endif
}

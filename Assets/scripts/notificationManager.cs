using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class notificationManager : MonoBehaviour
{
    void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = "Smash Again!!!";
        notification.Text = "Let's clear one more level.";
        notification.SmallIcon = "icon_small";
        notification.LargeIcon = "icon_big";
        notification.FireTime = System.DateTime.Now.AddHours(24);
        
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");
        var notificationStatus = AndroidNotificationCenter.CheckScheduledNotificationStatus(id);

        if (notificationStatus == NotificationStatus.Scheduled)
        {
            // Replace the scheduled notification with a new notification.
            var newNotification = new AndroidNotification();
            newNotification.Title = "Smash Again!!!";
            newNotification.Text = "Let's clear one more level.";
            newNotification.SmallIcon = "icon_small";
            newNotification.LargeIcon = "icon_big";
            newNotification.FireTime = System.DateTime.Now.AddHours(24);
            AndroidNotificationCenter.UpdateScheduledNotification(id, newNotification, "channel_id");
        }
    }

}

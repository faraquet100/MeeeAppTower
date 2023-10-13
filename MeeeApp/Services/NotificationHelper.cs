namespace MeeeApp.Services;
using Plugin.LocalNotification;

public class NotificationHelper
{
    public static int NOTIFICATION_ID_CHECKIN_TEST = 100;
    public static int NOTIFICAITON_ID_CHECKOUT_TEST = 101;
    public static int NOTIFICATION_ID_CHECKIN = 200;
    public static int NOTIFICAITON_ID_CHECKOUT = 201;

    public static NotificationRequest CheckInTestNotification()
    {
        // Create the notification request
        var request = new NotificationRequest
        {
            NotificationId = NOTIFICATION_ID_CHECKIN_TEST,
            Title = "Meee",
            Description = "Time to Check-In",
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1),
            },
            Image = new NotificationImage
            {
                ResourceName = "sunup.png"
            }
        };

        return request;
    }
    
    public static NotificationRequest CheckOutTestNotification()
    {
        // Create the notification request
        var request = new NotificationRequest
        {
            NotificationId = NOTIFICAITON_ID_CHECKOUT_TEST,
            Title = "Meee",
            Description = "Time to Check-Out",
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1),
            },
            Image = new NotificationImage
            {
                ResourceName = "sundown.png"
            }
        };

        return request;
    }
    
    public static NotificationRequest CheckInNotification(Double secondsFromNow)
    {
        // Create the notification request
        var request = new NotificationRequest
        {
            NotificationId = NOTIFICATION_ID_CHECKIN,
            Title = "Meee",
            Description = "Time to Check-In",
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(secondsFromNow), // Would only work if I set the date by adding seconds
                NotifyRepeatInterval = TimeSpan.FromDays(1),
                RepeatType = NotificationRepeat.Daily,
            },
            Image = new NotificationImage
            {
                ResourceName = "sunup.png"
            }
        };

        return request;
    }
    
    public static NotificationRequest CheckOutNotification(Double secondsFromNow)
    {
        // Create the notification request
        var request = new NotificationRequest
        {
            NotificationId = NOTIFICAITON_ID_CHECKOUT,
            Title = "Meee",
            Description = "Time to Check-Out",
            CategoryType = NotificationCategoryType.Reminder,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(secondsFromNow),
                NotifyRepeatInterval = TimeSpan.FromDays(1),
                RepeatType = NotificationRepeat.Daily,
				
            },
            Image = new NotificationImage
            {
                ResourceName = "sundown.png"
            }
        };

        return request;
    }
}
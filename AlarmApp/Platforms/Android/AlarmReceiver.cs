using Android.App;
using Android.Content;
using Android.Media;
using AndroidX.Core.App;

namespace AlarmApp;

/*
 * STOPACTION - is trying to stop the alarm sound and notification implementation
 */

[BroadcastReceiver(Enabled = true, Exported = true)]
public class AlarmReceiver : BroadcastReceiver
{
    private static MediaPlayer _mediaPlayer; // STOPACTION - updated mediaplayer to static already accessable field

    public override void OnReceive(Context context, Intent intent)
    {
        // Play sound
        //_mediaPlayer = MediaPlayer.Create(context, Resource.Raw.alarm_sound);// Add sound into Resources/Raw
        var alarmUri = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
        _mediaPlayer = MediaPlayer.Create(context, alarmUri);// STOPACTION - updated to static field
        _mediaPlayer.Start();

        // STOPACTION - Send a message into the application
        MessagingCenter.Send(this, "AlarmStarted");

        // STOPACTION - Create an action to stop the alarm in the future
        var stopIntent = new Intent(context, typeof(AlarmReceiver));
        stopIntent.SetAction("STOP_ALARM");
        var stopPendingIntent = PendingIntent.GetBroadcast(context, 0, stopIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

        // Notification
        var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
        //var notification = new Notification.Builder(context, "alarm_channel")// STOPACTION - removed
        var builder = new NotificationCompat.Builder(context, "alarm_channel")// STOPACTION - added
            .SetContentTitle("Alarm!")
            .SetContentText("Time to get up!")
            .SetSmallIcon(Resource.Drawable.abc_vector_test) // Replace with own icon
            .AddAction(Resource.Drawable.abc_vector_test, "Stop", stopPendingIntent)// STOPACTION - Add button to stop the alarm in notification
            .SetAutoCancel(true);// STOPACTION - added this line to close the notification when the action is clicked
                                 //.Build();// STOPACTION - removed

        //notificationManager.Notify(1, notification);// STOPACTION - removed
        notificationManager.Notify(1, builder.Build());// STOPACTION - updated

        // If the action received is "STOP_ALARM"
        if (intent.Action == "STOP_ALARM")
        {
            StopAlarm();
        }
    }

    /// <summary>
    /// STOPACTION - Stop the alarm sound and remove the notification method
    /// a special method that allows you to stop a static media player, detached from the context of the application
    /// </summary>
    public static void StopAlarm()
    {
        if (_mediaPlayer != null && _mediaPlayer.IsPlaying)
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Release();
            _mediaPlayer = null;
        }

        // Remove the notification
        var notificationManager = (NotificationManager)Platform.CurrentActivity?.GetSystemService(Context.NotificationService);
        notificationManager?.Cancel(1);
    }


}
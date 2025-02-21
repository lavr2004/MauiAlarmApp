using Android.App;
using Android.Content;
using Android.Media;

namespace AlarmApp;

[BroadcastReceiver(Enabled = true, Exported = true)]
public class AlarmReceiver : BroadcastReceiver
{
    public override void OnReceive(Context context, Intent intent)
    {
        // Play sound
        var alarmUri = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);// Add sound into Resources/Raw
        var player = MediaPlayer.Create(context, alarmUri);
        player.Start();

        // Notification
        var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
        var notification = new Notification.Builder(context, "alarm_channel")
            .SetContentTitle("Alarm!")
            .SetContentText("Time to get up!")
            .SetSmallIcon(Resource.Drawable.abc_vector_test) // Replace with own icon
            .Build();

        notificationManager.Notify(1, notification);
    }
}
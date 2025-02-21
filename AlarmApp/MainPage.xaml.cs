#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace AlarmApp;

public partial class MainPage : ContentPage
{
#if ANDROID
    private PendingIntent _pendingIntent; // Save Android PendingIntent to cancel alarm if required
#endif

    public MainPage()
    {
        InitializeComponent();
    }

    private void SetAlarm_Clicked(object sender, EventArgs e)
    {
        var selectedTime = AlarmTimePicker.Time;
        var now = DateTime.Now;
        var alarmDateTime = new DateTime(now.Year, now.Month, now.Day, selectedTime.Hours, selectedTime.Minutes, 0);

        if (alarmDateTime < DateTime.Now)
        {
            alarmDateTime = alarmDateTime.AddDays(1); // If the time has already passed, we set it for tomorrow
        }

        if (CanScheduleExactAlarms())
        {
            SetAlarm(alarmDateTime);
            StatusLabel.Text = $"OK: alarm setting up on {alarmDateTime:HH:mm}";
        }
        else
        {
            RequestExactAlarmPermission();
        }
    }

    private void CancelAlarm_Clicked(object sender, EventArgs e)
    {
#if ANDROID
        if (_pendingIntent != null)
        {
            CancelAlarm();
            StatusLabel.Text = "OK: planned Alarm Clock cancelled";
        }
        else
        {
            StatusLabel.Text = "ER: There is no alarm set to cancel";
        }
#endif
    }

    private bool CanScheduleExactAlarms()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(31)) // Android 12+
        {
            var alarmManager = (Android.App.AlarmManager)Platform.CurrentActivity.GetSystemService(Android.Content.Context.AlarmService);
            return alarmManager.CanScheduleExactAlarms();
        }
#endif
        return true; // version under Android 12 dont require permission
    }

    private void RequestExactAlarmPermission()
    {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionRequestScheduleExactAlarm);
            intent.SetData(Android.Net.Uri.Parse("package:" + Platform.CurrentActivity.PackageName));
            Platform.CurrentActivity.StartActivity(intent);
            StatusLabel.Text = "ER: Please allow precise alarms in settings.";
        }
#endif
    }

    private void SetAlarm(DateTime alarmTime)
    {
#if ANDROID
        var context = Platform.CurrentActivity;
        var alarmManager = (Android.App.AlarmManager)context.GetSystemService(Android.Content.Context.AlarmService);
        var intent = new Android.Content.Intent(context, typeof(AlarmReceiver));
        _pendingIntent = Android.App.PendingIntent.GetBroadcast(context, 0, intent, Android.App.PendingIntentFlags.UpdateCurrent | Android.App.PendingIntentFlags.Immutable);

        var triggerTime = Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(alarmTime - DateTime.Now).TotalMilliseconds;
        alarmManager.SetExact(Android.App.AlarmType.RtcWakeup, triggerTime, _pendingIntent);
#endif
    }

    private void CancelAlarm()
    {
#if ANDROID
        var context = Platform.CurrentActivity;
        var alarmManager = (Android.App.AlarmManager)context.GetSystemService(Android.Content.Context.AlarmService);
        alarmManager.Cancel(_pendingIntent);
        _pendingIntent = null; // Reset after cancellation
#endif
    }
}
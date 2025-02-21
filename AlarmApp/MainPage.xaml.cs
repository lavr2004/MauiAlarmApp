#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;
#endif

namespace AlarmApp;
/*
 * STOPACTION - is trying to stop the alarm sound and notification implementation
 */
public partial class MainPage : ContentPage
{
#if ANDROID
    private PendingIntent _pendingIntent; // Save Android PendingIntent to cancel alarm if required
#endif

    public MainPage()
    {
        InitializeComponent();

        //STOPACTION - adding subscription to the AlarmReceiver to be able to enable the Stop button
#if ANDROID
        MessagingCenter.Subscribe<AlarmReceiver>(this, "AlarmStarted", (_) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                StopButton.IsEnabled = true;
                StatusLabel.Text = "OK: alarm is enabled!";
            });
        });
#endif
    }

    private void SetAlarm_Clicked(object sender, EventArgs e)
    {
        var selectedTime = AlarmTimePicker.Time;
        var now = DateTime.Now;
        var alarmDateTime = new DateTime(now.Year, now.Month, now.Day, selectedTime.Hours, selectedTime.Minutes, 0);

        if (alarmDateTime < DateTime.Now)
        {
            alarmDateTime = alarmDateTime.AddDays(1); // Если время уже прошло, устанавливаем на завтра
        }

        if (!CanScheduleExactAlarms())
        {
            RequestExactAlarmPermission();
            return; // Прерываем выполнение, пока пользователь не даст разрешение
        }

        // Проверка и запрос отключения оптимизации батареи
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            var powerManager = (Android.OS.PowerManager)Platform.CurrentActivity.GetSystemService(Context.PowerService);
            if (!powerManager.IsIgnoringBatteryOptimizations(Platform.CurrentActivity.PackageName))
            {
                var intent = new Intent(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + Platform.CurrentActivity.PackageName));
                Platform.CurrentActivity.StartActivity(intent);
                StatusLabel.Text = "Пожалуйста, отключите оптимизацию батареи для корректной работы будильника.";
                return; // Даём пользователю время на подтверждение
            }
        }
#endif

        // Установка будильника, если все разрешения есть
        SetAlarm(alarmDateTime);
        StatusLabel.Text = $"OK: alarm setting up on {alarmDateTime:HH:mm}";
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (CanScheduleExactAlarms())
        {
            // Если разрешения есть, можно попробовать восстановить установку
            if (StatusLabel.Text.Contains("оптимизацию батареи"))
            {
                SetAlarm_Clicked(null, null); // Повторный вызов с последним временем
            }
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

    /// <summary>
    /// STOPACTION - handle the Stop button click event and stop the alarm sound in the AlarmReceiver on Android
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StopAlarm_Clicked(object sender, EventArgs e)
    {
#if ANDROID
        AlarmReceiver.StopAlarm();
        StopButton.IsEnabled = false;
        StatusLabel.Text = "OK: alarm stopped";
        _pendingIntent = null; // We're resetting it because the alarm has already gone off.
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
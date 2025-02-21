using Microsoft.Extensions.Logging;

/*
 * MINIMAL EXAMPLE REQUIRES updates in:
 * 
 * MainPage.xaml -> add TimePicker, Button, Label
 * MainPage.xaml.cs -> add SetAlarm_Clicked(), CanScheduleExactAlarms(), RequestExactAlarmPermission() methods
 * Platforms/Android/MainApplication.cs -> add override OnCreate() method
 * Platforms/Android/AlarmReceiver.cs -> add using Android.Media; and update OnReceive() method
 * Platforms/Android/Properties/AndroidManifest.xml -> add permission android.permission.REQUEST_SCHEDULE_EXACT_ALARM, WAKE_LOCK; add <receiver android:name=".AlarmReceiver" android:exported="true" />
 * 
 * CANCELLATION PLANNED ALARM REQUIRES updates in:
 * MainPage.xaml -> add CancelAlarm Button
 * MainPage.xaml.cs -> add CancelAlarm_Clicked() method; add _pendingIntent field and CancelAlarm() method
 * 
 */

namespace AlarmApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

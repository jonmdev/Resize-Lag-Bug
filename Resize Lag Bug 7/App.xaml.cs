using System.Diagnostics;

namespace Resize_Lag_Bug_7 {
    public partial class App : Application {
        public event Action timerUpdateEvent = null;

        public App() {
            InitializeComponent();

            //TOGGLE SHADOWS
            bool addShadows = true;

            //SET APPROX FPS
            double fps = 50;

            //TIMER FUNCTION
            IDispatcherTimer timer = Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromSeconds(1 / fps);
            double time = 0;
            DateTime currentTickDateTime = DateTime.Now;
            double deltaTime = 0;
            timer.Tick += delegate {
                deltaTime = (DateTime.Now - currentTickDateTime).TotalSeconds;
                currentTickDateTime = DateTime.Now;
                time += deltaTime;
                timerUpdateEvent.Invoke();
            };
            timer.Start();

            //PAGE BUILD
            ContentPage mainPage = new();
            MainPage = mainPage;

            AbsoluteLayout rootAbs = new();
            mainPage.Content = rootAbs;

            AbsoluteLayout abs = new();
            rootAbs.Add(abs);

            VerticalStackLayout vert = new();
            vert.BackgroundColor = Colors.AliceBlue;
            abs.Add(vert);

            AbsoluteLayout abs2 = new();
            vert.Add(abs2);

            int numBorders = 3;
            List<Border> borders = new List<Border>();

            for (int i=0; i < numBorders; i++) {
                Border border = new Border();
                border.HeightRequest = 200;
                border.WidthRequest = 500;
                border.BackgroundColor = Colors.DarkOrchid;
                abs2.Add(border);
                borders.Add(border);
                if (addShadows) {
                    border.Shadow = new Shadow() { Offset = new Point(5, 5), Radius = 5 };
                };

            }

            Border borderBot = new();
            borderBot.HeightRequest = 200;
            borderBot.WidthRequest = 50;
            borderBot.BackgroundColor = Colors.DarkOrange;
            vert.Add(borderBot);

            //TIMER UPDATE FUNCTION
            timerUpdateEvent += delegate {
                double sinVal = (Math.Sin(time) + 1) * 0.5;
                double height = 20 + sinVal * 800;
                for (int i = 0; i < borders.Count; i++) {
                    borders[i].HeightRequest = height;
                }
                abs2.HeightRequest = height;
            };

            //SCREEN CHANGE FUNCTION
            mainPage.SizeChanged += delegate {
                if (mainPage.Width > 0) {
                    abs.WidthRequest = mainPage.Width;
                    abs.HeightRequest = mainPage.Height;
                    vert.WidthRequest = mainPage.Width;
                    for (int i = 0; i < borders.Count; i++) {
                        borders[i].WidthRequest = mainPage.Width;
                        borders[i].TranslationX = mainPage.Width * i;
                    }
                }
            };

            //IOS/WINDOWS FRAME RATE MONITOR
            DateTime lastUpdateDateTime = DateTime.Now;
            abs2.SizeChanged += delegate {
                if (lastUpdateDateTime != DateTime.Now) {
                    Debug.WriteLine("CURRENT FPS " + 1 / (DateTime.Now - lastUpdateDateTime).TotalSeconds);
                    lastUpdateDateTime = DateTime.Now;
                }
            };
            //ANDROID FRAME RATE MONITOR
/*#if ANDROID
            bool frameRateMonitorAttached = false;
            mainPage.HandlerChanged += delegate {
                if (!frameRateMonitorAttached) {
                    frameRateMonitorAttached = true;
                    Android.Views.View rootView = Platform.CurrentActivity.Window.DecorView.RootView;
                    rootView.ViewTreeObserver.AddOnGlobalLayoutListener(new MyLayoutListener());
                }
                
            };
            
#endif*/

        }
#if ANDROID
        public class MyLayoutListener : Java.Lang.Object, Android.Views.ViewTreeObserver.IOnGlobalLayoutListener {
            DateTime currentDateTime = DateTime.Now - TimeSpan.FromSeconds(5);
            public void OnGlobalLayout() {
                if (currentDateTime != DateTime.Now) {
                    Debug.WriteLine("CURRENT FPS " + 1 / (DateTime.Now - currentDateTime).TotalSeconds);
                    currentDateTime = DateTime.Now;
                }
            }
        }
#endif

    }

}

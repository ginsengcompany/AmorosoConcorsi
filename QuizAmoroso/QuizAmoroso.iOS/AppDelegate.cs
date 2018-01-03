using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using UIKit;
using Xamarin.Forms;
using QuizAmoroso.iOS;
using Xamarin.Forms.Platform.iOS;
using ImageCircle.Forms.Plugin.iOS;
using static QuizAmoroso.iOS.AppDelegate;
using System.Threading.Tasks;
using System.ComponentModel;
using Xfx;

[assembly: ExportRenderer(typeof(Button), typeof(MyButtonRenderer))]
namespace QuizAmoroso.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            
            XfxControls.Init();

            LoadApplication(new App());
            ImageCircleRenderer.Init();
            // Serve per non far andare l'applicazione in onSleep automaticamente
            UIApplication.SharedApplication.IdleTimerDisabled = true;
            return base.FinishedLaunching(app, options);
        }
        public class MyButtonRenderer : ButtonRenderer
        {
            protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
            {
                base.OnElementChanged(e);
                if (Control != null)
                {
                    Control.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;
                    Control.TitleLabel.LineBreakMode = UILineBreakMode.CharacterWrap;
                    Control.TitleLabel.TextAlignment = UITextAlignment.Center;
                    Control.TitleLabel.Lines = 0;
                }
            }
            protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                base.OnElementPropertyChanged(sender, e);
                if (Control != null)
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Normal);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Application);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Disabled);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Focused);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Highlighted);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Selected);
                Control.SetTitleColor(Color.White.ToUIColor(), UIControlState.Reserved);
            }
        }
    }
}

using System;
namespace MeeeApp.Controls
{
	public class CobaltImageButton : ImageButton
    {
        public static readonly BindableProperty TagProperty = BindableProperty.Create("Tag", typeof(string), typeof(CobaltImageButton), "");

        public string Tag
        {
            get => (string)GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }
        
        public async Task<bool> BounceOnPressAsync()
        {
            var currentOpacity = this.Opacity;
            this.Opacity = currentOpacity * 0.8;
            await this.ScaleTo(0.95, 70, Easing.BounceOut);
            await this.ScaleTo(1, 70, Easing.BounceOut);
            this.Opacity = currentOpacity;

            return true;
        }
    }
}


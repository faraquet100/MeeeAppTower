using System;
namespace MeeeApp.Controls
{
	public class CobaltImageButton : ImageButton
	{
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


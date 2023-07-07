using System;
namespace MeeeApp.Controls
{
	public class CobaltButton : Button
	{
        public async Task<bool> BounceOnPressAsync()
        {
            await this.ScaleTo(0.95, 100, Easing.BounceOut);
            await this.ScaleTo(1, 100, Easing.BounceOut);
            return true;
        }
    }

	
}


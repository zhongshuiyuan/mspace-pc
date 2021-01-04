using Gvitech.CityMaker.RenderControl;
using Mmc.Platform.Core;
using System;

namespace Mmc.Platform.Commands
{
	public class AreaCmd : SimpleCommandWithMap
	{
		public override void Execute(object parameter)
		{
			bool state = parameter.ParseTo(false);
			if (state)
			{
				base.MapControl.InteractMode = gviInteractMode.gviInteractMeasurement;
				base.MapControl.MeasurementMode = gviMeasurementMode.gviMeasureArea;
			}
			else
			{
				base.MapControl.InteractMode = gviInteractMode.gviInteractNormal;
			}
		}
	}
}
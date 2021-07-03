using Sandbox.Common;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.Utils;
using VRageMath;

namespace Nomad.LaserAntennaGridFirmware
{
	public class LaserAntennaTerminal
	{
		internal static bool controlsCreated;

		internal static void createControls()
		{
			if (LaserAntennaTerminal.controlsCreated)
			{
				return;
			}

			LaserAntennaTerminal.createSeparator();
			LaserAntennaTerminal.createLaserToggleCheckbox();
			LaserAntennaTerminal.createLaserColor();
			LaserAntennaTerminal.createSeparator();
			LaserAntennaTerminal.createConnectGridToggleCheckbox();
			LaserAntennaTerminal.controlsCreated = true;
		}

		internal static void createSeparator()
		{
			IMyTerminalControlSeparator separator = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyLaserAntenna>("laser_antenna_grid_firmware_separator");
			MyAPIGateway.TerminalControls.AddControl<IMyLaserAntenna>(separator);
		}

		internal static void createLaserToggleCheckbox()
		{
			IMyTerminalControlCheckbox laserCheckbox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCheckbox, IMyLaserAntenna>("laser_antenna_grid_firmware_show_laser");

			laserCheckbox.Title = MyStringId.GetOrCompute("Show Connection Laser");
			laserCheckbox.Tooltip = MyStringId.GetOrCompute("Show or hide the laser when connected");
			laserCheckbox.SupportsMultipleBlocks = true;

			laserCheckbox.Getter = (IMyTerminalBlock block) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware logic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (logic != null)
				{
					return logic.Settings.ShowLaser;
				}
				return true;
			};

			laserCheckbox.Setter = (IMyTerminalBlock block, bool value) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware sourcelogic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (sourcelogic != null)
				{
					sourcelogic.Settings.ShowLaser = value;
					sourcelogic.SyncWithServer = true;
				}
				LaserAntennaGridFirmware targetlogic = sourcelogic.GetTargetLogic();
				if (targetlogic != null)
				{
					targetlogic.Settings.ShowLaser = value;
				}
			};

			MyAPIGateway.TerminalControls.AddControl<IMyLaserAntenna>(laserCheckbox);
		}

		internal static void createLaserColor()
		{
			IMyTerminalControlColor laserColor = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlColor, IMyLaserAntenna>("laser_antenna_grid_firmware_laser_color");

			laserColor.Title = MyStringId.GetOrCompute("Colour");
			laserColor.Tooltip = MyStringId.GetOrCompute("Specify the laser colour");
			laserColor.SupportsMultipleBlocks = true;

			laserColor.Getter = (IMyTerminalBlock block) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware logic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (logic != null)
				{
					return logic.Settings.LaserColor;
				}
				return Color.Red.ToVector4();
			};

			laserColor.Setter = (IMyTerminalBlock block, Color value) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware sourcelogic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (sourcelogic != null)
				{
					sourcelogic.Settings.LaserColor = value.ToVector4();
					sourcelogic.SyncWithServer = true;
				}
				LaserAntennaGridFirmware targetlogic = sourcelogic.GetTargetLogic();
				if (targetlogic != null)
				{
					targetlogic.Settings.LaserColor = value.ToVector4();
				}
			};

			MyAPIGateway.TerminalControls.AddControl<IMyLaserAntenna>(laserColor);
		}

		internal static void createConnectGridToggleCheckbox()
		{
			IMyTerminalControlCheckbox connectGridCheckbox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCheckbox, IMyLaserAntenna>("laser_antenna_grid_firmware_connect_grid");

			connectGridCheckbox.Title = MyStringId.GetOrCompute("Connect To Receiver Grid");
			connectGridCheckbox.Tooltip = MyStringId.GetOrCompute("Connect grids on successful antenna connection");
			connectGridCheckbox.SupportsMultipleBlocks = true;

			connectGridCheckbox.Getter = (IMyTerminalBlock block) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware logic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (logic != null)
				{
					return logic.Settings.GroupGridOnConnect;
				}
				return true;
			};

			connectGridCheckbox.Setter = (IMyTerminalBlock block, bool value) => {
				IMyLaserAntenna source = (IMyLaserAntenna) block;
				LaserAntennaGridFirmware sourcelogic = source.GameLogic.GetAs<LaserAntennaGridFirmware>();
				if (sourcelogic != null)
				{
					sourcelogic.Settings.GroupGridOnConnect = value;
					sourcelogic.SyncWithServer = true;
				}
				LaserAntennaGridFirmware targetlogic = sourcelogic.GetTargetLogic();
				if (targetlogic != null)
				{
					targetlogic.Settings.GroupGridOnConnect = value;
				}
			};

			MyAPIGateway.TerminalControls.AddControl<IMyLaserAntenna>(connectGridCheckbox);
		}
	}
}

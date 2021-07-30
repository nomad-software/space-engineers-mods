using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Weapons;
using SpaceEngineers.Game.ModAPI;
using System;
using VRage;
using VRage.Common.Utils;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Library.Utils;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

using BlendTypeEnum = VRageRender.MyBillboard.BlendTypeEnum;
using MyLaserAntennaStatus = Sandbox.ModAPI.Ingame.MyLaserAntennaStatus;


namespace Nomad.LaserAntennaGridFirmware
{
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_LaserAntenna), false)]
	public class LaserAntennaGridFirmware : MyGameLogicComponent
	{
		protected readonly Guid SETTINGS_GUID = new Guid("05C8D1AB-A6AE-4FA4-A6D6-4C28A108F92D");
		protected readonly double LASER_DRAW_DISTANCE = 350.0; // Metres

		protected IMyCamera camera;
		protected Random rng;

		protected MyStringId laserMaterial;
		protected MyStringId laserFlareBaseMaterial;
		protected MyStringId laserFlareMaterial;
		protected float laserWidth;
		protected Vector3D laserStart;
		protected Vector3D laserEnd;
		protected Vector3D sourceLaserFlarePosition;
		protected Vector3D targetLaserFlarePosition;
		protected MatrixD sourceLaserFlareMatrix;
		protected MatrixD targetLaserFlareMatrix;
		protected bool laserStartInRange;
		protected bool laserEndInRange;

		protected MyEntitySubpart sourceTurret;
		protected MyEntitySubpart targetTurret;
		protected MatrixD smallAntennaWorldMatrix;
		protected Vector3D smallAntennaUpVector;

		protected IMyLaserAntenna source;
		protected MyLaserAntennaDefinition sourceBlockDefinition;
		protected bool sourceIsLargeGrid;
		protected bool sourceIsConnected;
		protected IMyLaserAntenna target;
		protected LaserAntennaGridFirmware targetLogic;
		protected bool targetIsLargeGrid;
		protected bool targetIsWorking;

		protected MySoundPair sound;
		protected MyEntity3DSoundEmitter soundEmitter;

		public bool ConnectedToGrid;
		public LaserAntennaSettings Settings;
		public bool SyncWithServer;

		public override void Init(MyObjectBuilder_EntityBase objectBuilder)
		{
			base.Init(objectBuilder);

			this.NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
			this.camera = MyAPIGateway.Session.Camera;
			this.rng = new Random();


			this.source = (IMyLaserAntenna) this.Entity;
			this.sourceBlockDefinition = (MyLaserAntennaDefinition) this.source.SlimBlock.BlockDefinition;
			// Check SerializableDefinitionId for sub type.
			this.sourceIsLargeGrid = (this.source.BlockDefinition.SubtypeId == "LargeBlockLaserAntenna");

			this.sound = new MySoundPair("BlockProjectorOff");
			this.soundEmitter = new MyEntity3DSoundEmitter((MyEntity) this.source);
			this.soundEmitter.CustomVolume = 1.0f;
			this.soundEmitter.CustomMaxDistance = 8.0f;

			this.laserMaterial = MyStringId.GetOrCompute("LaserAntennaLaser");
			this.laserFlareBaseMaterial = MyStringId.GetOrCompute("LaserAntennaLaserFlareBase");
			this.laserFlareMaterial = MyStringId.GetOrCompute("LaserAntennaLaserFlare");

			MyResourceSinkComponent sink = (MyResourceSinkComponent) this.source.ResourceSink;
			sink?.SetRequiredInputFuncByType(MyResourceDistributorComponent.ElectricityId, this.computePowerRequirements);

			this.Settings = new LaserAntennaSettings();
			this.LoadSettings();
			this.SaveSettings();
		}

		public override void UpdateOnceBeforeFrame()
		{
			LaserAntennaTerminal.createControls();
			this.NeedsUpdate = MyEntityUpdateEnum.EACH_FRAME | MyEntityUpdateEnum.EACH_10TH_FRAME | MyEntityUpdateEnum.EACH_100TH_FRAME;
		}

		public override void UpdateBeforeSimulation()
		{
		}

		public override void UpdateBeforeSimulation10()
		{
		}

		public override void UpdateBeforeSimulation100()
		{
		}

		public override void UpdateAfterSimulation()
		{
			this.handleDrawingLaser();
		}

		public override void UpdateAfterSimulation10()
		{
			this.handleState();
			this.handleSound();
		}

		public override void UpdateAfterSimulation100()
		{
			if (this.SyncWithServer && MyAPIGateway.Multiplayer.MultiplayerActive && ! MyAPIGateway.Multiplayer.IsServer)
			{
				this.Settings.NetworkSenderId = MyAPIGateway.Multiplayer.MyId;
				this.Settings.NetworkLaserAntennaId = this.source.EntityId;
				var bytes = MyAPIGateway.Utilities.SerializeToBinary(this.Settings);
				MyAPIGateway.Multiplayer.SendMessageToServer(LaserAntennaNetworkSession.CHANNEL_ID, bytes);
				this.SyncWithServer = false;
			}
		}

		public override void UpdatingStopped()
		{
		}

		public override void Close()
		{
			this.disconnectGrid();
			this.soundEmitter.Cleanup();
		}

		public override bool IsSerialized()
		{
			this.SaveSettings();
			return base.IsSerialized();
		}

		public void SaveSettings()
		{
			try
			{
				if (this.source == null)
				{
					return;
				}

				if(this.source.Storage == null)
				{
					this.source.Storage = new MyModStorageComponent();
				}

				this.source.Storage.SetValue(this.SETTINGS_GUID, Convert.ToBase64String(MyAPIGateway.Utilities.SerializeToBinary(this.Settings)));
			}
			catch(Exception e)
			{
				return;
			}
		}

		public void LoadSettings()
		{
			this.Settings.ShowLaser = true;
			this.Settings.LaserColor = Color.Red.ToVector4();
			this.Settings.GroupGridOnConnect = false;

			if(this.source.Storage == null)
			{
				return;
			}

			try
			{
				string data;

				if(! this.source.Storage.TryGetValue(this.SETTINGS_GUID, out data))
				{
					return;
				}

				LaserAntennaSettings settings = MyAPIGateway.Utilities.SerializeFromBinary<LaserAntennaSettings>(Convert.FromBase64String(data));

				if(settings != null)
				{
					this.Settings.ShowLaser = settings.ShowLaser;
					this.Settings.LaserColor = settings.LaserColor;
					this.Settings.GroupGridOnConnect = settings.GroupGridOnConnect;
				}
			}
			catch(Exception e)
			{
				return;
			}
		}

		public LaserAntennaGridFirmware GetTargetLogic()
		{
			if (this.targetLogic != null)
			{
				return this.targetLogic;
			}

			if (this.target != null)
			{
				this.targetLogic = this.target?.GameLogic?.GetAs<LaserAntennaGridFirmware>();
				return this.targetLogic;
			}

			return null;
		}

		protected float computePowerRequirements()
		{
			float power = 0.0f;

			if (! this.source.Enabled || ! this.source.IsFunctional)
			{
				return power;
			}

			switch (this.source.Status)
			{
				case MyLaserAntennaStatus.Idle:
					power = this.sourceBlockDefinition.PowerInputIdle; // 0.0001 (100 watts)
					break;

				case MyLaserAntennaStatus.RotatingToTarget:
					power = this.sourceBlockDefinition.PowerInputTurning; // 0.001 (1 Kilowatt)
					break;

				case MyLaserAntennaStatus.OutOfRange:
				case MyLaserAntennaStatus.SearchingTargetForAntenna:
				case MyLaserAntennaStatus.Connecting:
				case MyLaserAntennaStatus.Connected:
					if (this.source.TargetCoords != null)
					{
						float distance = (float) Vector3D.Distance(this.source.GetPosition(), this.source.TargetCoords);

						if (distance <= 200000.0f)
						{
							power = this.sourceBlockDefinition.PowerInputLasing * distance;
							power = power / 1000000.0f;
						}
						else
						{
							power = this.sourceBlockDefinition.PowerInputLasing / 2.0f / 200000.0f;
							power = power * (distance * distance) + 1000000.0f;
							power = power / 1000000.0f;
						}
					}
					if (this.ConnectedToGrid)
					{
						power = power * 50.0f;
					}
					break;

				default:
					power = this.sourceBlockDefinition.PowerInputIdle;
					break;
			}

			return power;
		}

		// Do a check to only connect grids from one of the paired antenna.
		// Otherwise you get a race condition and a null reference error when
		// using the cube grids.
		protected bool isMaster()
		{
			if (this.target == null)
			{
				return false;
			}
			return this.source.EntityId > this.target.EntityId;
		}

		// Try and initialise the target antenna. If we are connected properly
		// we should get it. Also syncronise any mismatched settings.
		protected void initTarget()
		{
			if (this.target == null)
			{
				if (this.source.Other != null)
				{
					this.target = this.source.Other;
					this.targetIsLargeGrid = (this.target.BlockDefinition.SubtypeId == "LargeBlockLaserAntenna");

					LaserAntennaGridFirmware targetlogic = this.GetTargetLogic();
					if (targetlogic != null)
					{
						targetlogic.Settings.ShowLaser = this.Settings.ShowLaser;
						targetlogic.Settings.LaserColor = this.Settings.LaserColor;
						targetlogic.Settings.GroupGridOnConnect = this.Settings.GroupGridOnConnect;
					}
				}
			}
		}

		protected void handleState()
		{
			this.sourceIsConnected = (this.source.Status == MyLaserAntennaStatus.Connected);

			if (this.source.IsWorking && this.sourceIsConnected)
			{
				this.initTarget();

				if (this.Settings.GroupGridOnConnect)
				{
					this.connectGrid();
				}
				else
				{
					this.disconnectGrid();
				}
			}
			else
			{
				this.fullDisconnect();
			}

			this.targetIsWorking = (this.target != null && this.target.IsWorking);

			if (! this.targetIsWorking)
			{
				this.fullDisconnect();
			}
		}

		protected void handleSound()
		{
			if (this.source.IsWorking && this.sourceIsConnected && this.targetIsWorking && this.ConnectedToGrid)
			{
				if (! this.soundEmitter.IsPlaying)
				{
					this.soundEmitter.PlaySound(this.sound, skipIntro: true);
				}
			}
			else
			{
				if (this.soundEmitter.IsPlaying)
				{
					this.soundEmitter.StopSound(false);
				}
			}
		}

		protected void handleDrawingLaser()
		{
			if (this.Settings.ShowLaser && this.source.IsWorking && this.sourceIsConnected && this.targetIsWorking)
			{
				// Only the master takes care of drawing the laser to make sure
				// everything is in sync and only happens once.
				if (! this.isMaster())
				{
					return;
				}

				// Sometimes the source and/or target maybe invalid here. This
				// happens because this method is being called far more often
				// than the main logic method, so booleans used here may not
				// have been updated yet. Because of this, the code below might
				// fail and throw exceptions.
				try
				{
					this.sourceTurret = this.source.GetSubpart("LaserComTurret").GetSubpart("LaserCom");
					this.targetTurret = this.target.GetSubpart("LaserComTurret").GetSubpart("LaserCom");
					this.laserStart = this.sourceTurret.WorldMatrix.Translation;
					this.laserEnd = this.targetTurret.WorldMatrix.Translation;

					// Only draw the elements that are in range.
					this.laserStartInRange = (this.camera.GetDistanceWithFOV(this.laserStart) <= this.LASER_DRAW_DISTANCE);
					this.laserEndInRange = (this.camera.GetDistanceWithFOV(this.laserEnd) <= this.LASER_DRAW_DISTANCE);

					// Hitch the start point up a bit on small grid laser
					// antenna because their lens is slightly off centre.
					if (this.laserStartInRange && ! this.sourceIsLargeGrid)
					{
						this.smallAntennaWorldMatrix = this.sourceTurret.PositionComp.LocalMatrixRef * this.source.CubeGrid.WorldMatrix;
						this.smallAntennaUpVector = Vector3D.Cross(this.laserStart - this.laserEnd, this.smallAntennaWorldMatrix.Right);
						Vector3D.Normalize(ref this.smallAntennaUpVector, out this.smallAntennaUpVector);
						this.laserStart += (this.smallAntennaUpVector * 0.16);
					}

					// Hitch the start point up a bit on small grid laser
					// antenna because their lens is slightly off centre.
					if (this.laserEndInRange && ! this.targetIsLargeGrid)
					{
						this.smallAntennaWorldMatrix = this.targetTurret.PositionComp.LocalMatrixRef * this.target.CubeGrid.WorldMatrix;
						this.smallAntennaUpVector = Vector3D.Cross(this.laserEnd - this.laserStart, this.smallAntennaWorldMatrix.Right);
						Vector3D.Normalize(ref this.smallAntennaUpVector, out this.smallAntennaUpVector);
						this.laserEnd += (this.smallAntennaUpVector * 0.16);
					}

					// -----------------------
					// SOURCE -> TARGET LASER
					// -----------------------
					if (this.laserStartInRange || this.laserEndInRange)
					{
						// Add some jitter to the laser.
						this.laserWidth = (float) this.rng.NextDouble() * (0.12f - 0.06f) + 0.06f;

						// Draw the laser.
						MyTransparentGeometry.AddLineBillboard(this.laserMaterial, this.Settings.LaserColor, this.laserStart, this.laserEnd - this.laserStart, 1.0f, this.laserWidth, BlendTypeEnum.PostPP);
					}

					// -----------------------
					// SOURCE FLARE
					// -----------------------
					if (this.laserStartInRange)
					{
						// Calculate the source flare offset.
						Vector3D.Lerp(ref this.laserStart, ref this.laserEnd, ((this.sourceIsLargeGrid ? 0.91 : 0.475) / (this.laserStart - this.laserEnd).Length()), out this.sourceLaserFlarePosition);

						// Create a source matrix from the lasers direction.
						this.sourceLaserFlareMatrix = MatrixD.CreateFromDir(this.laserStart - this.laserEnd);

						// Draw a source flare flat on the face of the laser antenna
						// turrent to hide clipping of the following flare.
						MyTransparentGeometry.AddBillboardOriented(this.laserFlareBaseMaterial, this.Settings.LaserColor, this.sourceLaserFlarePosition, this.sourceLaserFlareMatrix.Left, this.sourceLaserFlareMatrix.Up, this.laserWidth * (this.sourceIsLargeGrid ? 10.0f : 7.0f), BlendTypeEnum.PostPP);

						// Draw a source flare always facing the camera.
						MyTransparentGeometry.AddPointBillboard(this.laserFlareMaterial, this.Settings.LaserColor, this.sourceLaserFlarePosition, uint.MaxValue, ref MatrixD.Identity, this.laserWidth * (this.sourceIsLargeGrid ? 10.0f : 7.0f), 0.0f, -1, BlendTypeEnum.PostPP);
					}

					// -----------------------
					// TARGET FLARE
					// -----------------------
					if (this.laserEndInRange)
					{
						// Calculate the target flare offset.
						Vector3D.Lerp(ref this.laserEnd, ref this.laserStart, ((this.targetIsLargeGrid ? 0.91 : 0.475) / (this.laserEnd - this.laserStart).Length()), out this.targetLaserFlarePosition);

						// Create a target matrix from the lasers direction.
						this.targetLaserFlareMatrix = MatrixD.CreateFromDir(this.laserEnd - this.laserStart);

						// Draw a target flare flat on the face of the laser antenna
						// turrent to hide clipping of the following flare.
						MyTransparentGeometry.AddBillboardOriented(this.laserFlareBaseMaterial, this.Settings.LaserColor, this.targetLaserFlarePosition, this.targetLaserFlareMatrix.Left, this.targetLaserFlareMatrix.Up, this.laserWidth * (this.targetIsLargeGrid ? 10.0f : 7.0f), BlendTypeEnum.PostPP);

						// Draw a target flare always facing the camera.
						MyTransparentGeometry.AddPointBillboard(this.laserFlareMaterial, this.Settings.LaserColor, this.targetLaserFlarePosition, uint.MaxValue, ref MatrixD.Identity, this.laserWidth * (this.targetIsLargeGrid ? 10.0f : 7.0f), 0.0f, -1, BlendTypeEnum.PostPP);
					}
				}
				catch (Exception e)
				{
					return;
				}
			}
		}

		// Connect the grids of the antenna.
		protected void connectGrid()
		{
			if (this.target != null)
			{
				if (! this.target.IsInSameLogicalGroupAs(this.source))
				{
					if (this.isMaster())
					{
						// MyAPIGateway.Utilities.ShowMessage("Debug", String.Format("Update: Antenna:{0} on {1}:{2} connecting", this.source.EntityId, this.source.CubeGrid.DisplayName, this.source.CubeGrid.EntityId));
						MyCubeGrid.CreateGridGroupLink(GridLinkTypeEnum.Logical, this.source.EntityId, (MyCubeGrid) this.source.CubeGrid, (MyCubeGrid) this.target.CubeGrid);
						MyCubeGrid.CreateGridGroupLink(GridLinkTypeEnum.Electrical, this.source.EntityId, (MyCubeGrid) this.source.CubeGrid, (MyCubeGrid) this.target.CubeGrid);

						this.ConnectedToGrid = true;

						LaserAntennaGridFirmware targetlogic = this.GetTargetLogic();
						if (targetlogic != null)
						{
							targetlogic.ConnectedToGrid = true;
						}
					}
				}
			}
		}

		// Disconnect the grids.
		protected void disconnectGrid()
		{
			if (this.target != null)
			{
				if (this.target.IsInSameLogicalGroupAs(this.source))
				{
					if (this.isMaster())
					{
						// MyAPIGateway.Utilities.ShowMessage("Debug", String.Format("Update: Antenna:{0} on {1}:{2} disconnecting", this.source.EntityId, this.source.CubeGrid.DisplayName, this.source.CubeGrid.EntityId));
						MyCubeGrid.BreakGridGroupLink(GridLinkTypeEnum.Logical, this.source.EntityId, (MyCubeGrid) this.source.CubeGrid, (MyCubeGrid) this.target.CubeGrid);
						MyCubeGrid.BreakGridGroupLink(GridLinkTypeEnum.Electrical, this.source.EntityId, (MyCubeGrid) this.source.CubeGrid, (MyCubeGrid) this.target.CubeGrid);

						this.ConnectedToGrid = false;

						LaserAntennaGridFirmware targetlogic = this.GetTargetLogic();
						if (targetlogic != null)
						{
							targetlogic.ConnectedToGrid = false;
						}
					}
				}
			}
		}

		// Disconnect the grids and reset the target.
		protected void fullDisconnect()
		{
			this.disconnectGrid();
			this.target = null;
			this.targetLogic = null;
		}
	}
}

using ProtoBuf;
using System;
using VRageMath;


namespace Nomad.LaserAntennaGridFirmware
{
	[ProtoContract(UseProtoMembersOnly = true)]
	public class LaserAntennaSettings
	{
		[ProtoMember(1)]
		public bool ShowLaser;

		[ProtoMember(2)]
		public Vector4 LaserColor;

		[ProtoMember(3)]
		public bool GroupGridOnConnect;

		[ProtoMember(4)]
		public long NetworkLaserAntennaId;

		[ProtoMember(5)]
		public ulong NetworkSenderId;
	}
}

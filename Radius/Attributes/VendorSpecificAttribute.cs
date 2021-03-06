﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
	public class VendorSpecificAttribute : RadiusAttribute
	{
		#region Constants
		private const uint VSA_ID_INDEX = 2;
		private const byte VSA_TYPE_INDEX = 6;
		private const byte VSA_LENGTH_INDEX = 7;
		private const byte VSA_DATA_INDEX = 8;
		#endregion

		#region Properties
		public byte VendorSpecificType { get; private set; }
		public byte VendorSpecificLength { get; private set; }
		public uint VendorId { get; private set; }
		#endregion

		#region Constructor
		public VendorSpecificAttribute(uint vendorId, byte vendorSpecificType, byte[] vendorSpecificData) : base (RadiusAttributeType.VENDOR_SPECIFIC)
		{
			Data = vendorSpecificData;
			
			//Length is the actual data plus all the vendor specific header info
			Length = (byte)(Data.Length + VSA_DATA_INDEX);

			RawData = new byte[Length];

			RawData[0] = (byte)Type;
			RawData[1] = Length;

			// Set the Private Enterprise Number for this attribute
			// http://www.iana.org/assignments/enterprise-numbers
			byte[] vendorIdArray = BitConverter.GetBytes(vendorId);
			Array.Reverse(vendorIdArray);
			Array.Copy(vendorIdArray, 0, RawData, ATTRIBUTE_HEADER_SIZE, sizeof(uint));

			RawData[VSA_TYPE_INDEX] = vendorSpecificType;

			RawData[VSA_LENGTH_INDEX] = (byte)(Data.Length + ATTRIBUTE_HEADER_SIZE);

			Array.Copy(vendorSpecificData, 0, RawData, VSA_DATA_INDEX, vendorSpecificData.Length);
		}

		public VendorSpecificAttribute(byte[] rawData, int offset) : base (RadiusAttributeType.VENDOR_SPECIFIC)
		{
			byte[] vendorIDArray = new byte[sizeof(uint)];
			Array.Copy(rawData, offset + VSA_ID_INDEX, vendorIDArray, 0, sizeof(uint));
			Array.Reverse(vendorIDArray);
			VendorId = BitConverter.ToUInt32(vendorIDArray, 0);
			VendorSpecificType = rawData[VSA_TYPE_INDEX];
			VendorSpecificLength = rawData[VSA_LENGTH_INDEX];
			Data = new byte[VendorSpecificLength - 2];
			Array.Copy(rawData, offset + VSA_DATA_INDEX, Data, 0, VendorSpecificLength - 2);
		}
		#endregion
	}
}

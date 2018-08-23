using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

namespace App_121GW
{
	public class Range121GW
	{
		public string mUnits;
		public string mLabel;
		public int[] mValues;
		public string mNotation;

		public Range121GW(string pUnits, string pLabel, int[] pValues, string pNotation)
		{
			mNotation = pNotation;
			mUnits = pUnits;
			mLabel = pLabel;
			mValues = pValues;
		}
	}
	//TODO : Hold logging when HOLD is enabled.
	public class Packet121GW
	{
		byte[] mData;

		public enum eMode
		{
			Low_Z = 0,
			DCV = 1,
			ACV = 2,
			DCmV = 3,
			ACmV = 4,
			Temp = 5,
			Hz = 6,
			mS = 7,
			Duty = 8,
			Resistor = 9,
			Continuity = 10,
			Diode = 11,
			Capacitor = 12,
			ACuVA = 13,
			ACmVA = 14,
			ACVA = 15,
			ACuA = 16,
			DCuA = 17,
			ACmA = 18,
			DCmA = 19,
			ACA = 20,
			DCA = 21,
			DCuVA = 22,
			DCmVA = 23,
			DCVA = 24,

			TempC = 25,
			TempF = 26,

			_TempC = 100,
			_TempF = 105,
			_Battery = 110,
			_APO_On = 120,
			_APO_Off = 125,
			_YEAR = 130,
			_DATE = 135,
			_TIME = 140,
			_BURDEN_VOLTAGE = 150,
			_LCD = 160,
			_dBm = 180,
			_Interval = 190
		}
		public enum eSign
		{
			ePositive = 0,
			eNegative = 1
		}
		public enum eAD_DC
		{
			eNone = 0,
			eDC,
			eAC,
			eACDC
		}
		public enum eBarRange
		{
			e5 = 0,
			e50 = 1,
			e500 = 2,
			e1000 = 3
		}

		readonly Range121GW[] mRangeLookup =
		{
			new Range121GW("V",     "Voltage Low Z (V)",    new int[]{4}            ," "        ),      //0
			new Range121GW("V",     "Voltage DC (V)",       new int[]{1,2,3,4}      ,"    "     ),      //1
			new Range121GW("V",     "Voltage AC (V)",       new int[]{1,2,3,4}      ,"    "     ),      //2
			new Range121GW("mV",    "Voltage DC (V)",       new int[]{2,3}          ,"mm"       ),      //3 
			new Range121GW("mV",    "Voltage AC (V)",       new int[]{2,3}          ,"mm"       ),      //4
			new Range121GW("°C",    "Temp (°C)",            new int[]{4}            ," "        ),      //5
			new Range121GW("KHz",   "Frequency (Hz)",       new int[]{2,3,1,2,3}    ,"  kkk"    ),      //6
			new Range121GW("ms",    "Period (s)",           new int[]{1,2,3}        ,"   "      ),      //7
			new Range121GW("%",     "Duty (%)",             new int[]{4}            ," "        ),      //8
			new Range121GW("KΩ",    "Resistance (Ω)",       new int[]{2,3,1,2,3,1,2},"  kkkMM"  ),      //9
			new Range121GW("KΩ",    "Continuity (Ω)",       new int[]{3}            ," "        ),      //10
			new Range121GW("V",     "Diode (V)",            new int[]{1,2}          ,"  "       ),      //11
			new Range121GW("ms",    "Capacitance (F)",      new int[]{3,4,2,3,4,5}  ,"nnuuuu"   ),      //12
			new Range121GW("uVA",   "Power AC (VA)",        new int[]{3,4,4,5}      ,"    "     ),      //13
			new Range121GW("mVA",   "Power AC (VA)",        new int[]{2,3,3,4}      ,"mm  "     ),      //14
			new Range121GW("mVA",   "Power AC (VA)",        new int[]{4,5,2,3}      ,"mm  "     ),      //15
			new Range121GW("uA",    "Current AC (A)",       new int[]{2,3}          ,"  "       ),      //16
			new Range121GW("uA",    "Current DC (A)",       new int[]{2,3}          ,"  "       ),      //17
			new Range121GW("mA",    "Current AC (A)",       new int[]{1,2}          ,"mm"       ),      //18
			new Range121GW("mA",    "Current DC (A)",       new int[]{1,2}          ,"mm"       ),      //19
			new Range121GW("A",     "Current AC (A)",       new int[]{3,1,2}        ,"m  "      ),      //20
			new Range121GW("A",     "Current DC (A)",       new int[]{3,1,2}        ,"m  "      ),      //21
			new Range121GW("uVA",   "Power DC (VA)",        new int[]{3,4,4,5}      ,"    "     ),      //22
			new Range121GW("mVA",   "Power DC (VA)",        new int[]{2,3,3,4}      ,"mm  "     ),      //23
			new Range121GW("VA",    "Power DC (VA)",        new int[]{4,5,2,3}      ,"mm  "     ),      //24
			new Range121GW("°C",    "Temp (°C)",            new int[]{4}            ," "        ),      //25
			new Range121GW("°F",    "Temp (°F)",            new int[]{4}            ," "        ),      //26
		};

		public int BoolToInt(bool value) => (value) ? 1 : 0;

		public byte Nibble(int pIndex, bool pHigh)
		{
			var data = mData[pIndex];
			if (pHigh) data >>= 4;
			data &= 0xf;

			return data;
		}

		//Start decimal coded nibbles protocol
		//Don't know why this is a seperate protocol...
		public int Year => mData[1] + 2000;
		public int Month => Nibble(2, true);
		public int Serial =>
			Nibble(2, false) * 10000 +
			Nibble(3, true) * 1000 +
			Nibble(3, false) * 100 +
			Nibble(4, true) * 10 +
			Nibble(4, false);

		//Start hex coded bytes protocol
		public eMode Mode
		{
			get
			{
				var output = (eMode)(mData[5] & 0x1F);
				if (output == eMode.Temp)
				{
					if (MainC) return eMode.TempC;
					if (MainF) return eMode.TempF;
				}
				return output;
			}
		}
		public bool MainOverload => (Nibble(6, true) & 0x8) > 0;
		public eSign MainSign => (eSign)BoolToInt((Nibble(6, true) & 0x4) > 0);
		public bool MainC => (Nibble(6, true) & 0x2) > 0;
		public bool MainF => (Nibble(6, true) & 0x1) > 0;

		public Range121GW MainRange => mRangeLookup[(int)Mode];
		public int MainRangeIndex => Nibble(6, false);
		public int MainRangeValue => MainRange.mValues[MainRangeIndex];
		public char MainRangeUnits => MainRange.mNotation[MainRangeIndex];
		public int MainIntValue => (((mData[5] >> 6) & 0x3) << 16) | (mData[7] << 8) | mData[8];

		public eMode        SubMode         => (eMode)mData[9];
        public bool         SubOverload     => (Nibble(10, true) & 0x8) != 0;
        public eSign        SubSign         => ((eSign)BoolToInt((Nibble(10, true) & 0x4) > 0));
        public bool         SubK            => ((Nibble(10, true) & 0x2) > 0);
        public bool         SubHz           => ((Nibble(10, true) & 0x1) > 0);
        public int          SubPoint        => Nibble(10, false);

        public int          SubIntValue
        {
            get
            {
                uint MSB = mData[11u];
                uint LSB = mData[12u];
                return (int)((MSB * 256u) + LSB);
            }
        }

        public bool         SubCurrentMode => SubMode == eMode.ACA || SubMode == eMode.ACmA || SubMode == eMode.ACuA || SubMode == eMode.DCA || SubMode == eMode.DCmA || SubMode == eMode.DCuA;
        public bool         Subm
        {
            get
            {
                switch (Mode)
                {
                    case eMode.ACVA:
                    case eMode.DCVA:
                    case eMode.ACmVA:
                    case eMode.DCmVA:
                    case eMode.ACuVA:
                    case eMode.DCuVA:
                        if (SubCurrentMode)
                        {
                            switch (MainRangeUnits)
                            {
                                case 'm':
                                case 'n':
                                case 'u':
                                    return true;
                            }
                            return (SubMode == eMode.ACmA || SubMode == eMode.ACuA || SubMode == eMode.DCmA || SubMode == eMode.DCuA);
                        }
                        break;
                }
                return false;
            }
        }


        public bool         BarOn           => ((Nibble(13, true) & 0x1) == 0);
        public bool         Bar0_150        => ((Nibble(13, false) & 0x8) != 0);
        public eSign        BarSign         => (eSign)BoolToInt((Nibble(13, false) & 0x4) == 0);
        public eBarRange    Bar1000_500     => (eBarRange)(Nibble(13, false) & 3);
        public int          BarValue        => mData[14] & 0x1F;
        public byte         Status1         => mData[15];
        public byte         Status2         => mData[16];
        public byte         Status3         => mData[17];

        public bool         Status1KHz      => (Status1 & 0x40) != 0;
        public bool         Status1ms       => (Status1 & 0x20) != 0;
        public eAD_DC       StatusAC_DC     => (eAD_DC)((Status1 >> 3) & 3);
        public bool         StatusAuto      => (Status1 & 0x4) != 0;
        public bool         StatusAPO       => (Status1 & 0x2) != 0;
        public bool         StatusBAT       => (Status1 & 0x1) != 0;

        public bool         StatusBT        => (Status2 & 0x40) != 0;
        public bool         StatusArrow     => (Status2 & 0x20) != 0;
        public bool         StatusRel       => (Status2 & 0x10) != 0;
        public bool         StatusdBm       => (Status2 & 0x8) != 0;
        public int          StatusMinMax    => (Status2 & 0x7);

        public bool         StatusTest      => (Status3 & 0x40) != 0;
        public int          StatusMem       => (Status3 & 0x30) >> 4;
        public int          StatusAHold     => (Status3 >> 2 ) & 3;
        public bool         StatusAC        => (Status3 & 0x2) != 0;
        public bool         StatusDC        => (Status3 & 0x1) != 0;

        public Packet121GW()
        {
            mData = new byte[19];
        }


        public float MainValue
        {
            get
            {
                var val = (float) MainIntValue / (float) Math.Pow(10f, 5 - MainRangeValue);
                return (MainSign == eSign.eNegative) ? -val : val;
            }
        }
        public float SubValue
        {
            get
            {
                var val = SubIntValue / (float)Math.Pow((float)10, (float)SubPoint);
                return (SubSign == eSign.eNegative)  ? - val : val;
            }
        }
        public string		MainRangeLabel	=>  MainRange?.mLabel ?? "";

		public double MainRangeMultiple
		{
			get
			{
				switch (MainRangeUnits)
				{
					case 'n':
						return 1.0 / 1000000000.0;
					case 'u':
						return 1.0 / 1000000.0;
					case 'm':
						return 1.0 / 1000.0;
					case 'K':
					case 'k':
						return 1000.0;
					case 'M':
						return 1000000.0;
				}
				return 1;
			}
		}

        //Note the above properties should not be read until this subroutine
        // completes
        static public bool Checksum( byte[] input )
        {
            byte output = 0;
            for ( uint i = 0; i < input.Length - 1; ++i )
                output ^= (byte)input[ i ];
            return ( output == input[ input.Length - 1u ] );
        }

		bool IsValid(byte[] input) => Checksum(input) && (input.Length == 19u);
		public bool ProcessPacket( byte[] pInput )
		{
            if ( IsValid( pInput ) )
            {
                mData = pInput;
                return true;
            }
            return false;
        }

		private static readonly byte[] KEYCODE_RANGE		= { 0xF4, 0x30, 0x31, 0x30, 0x31 };
		private static readonly byte[] KEYCODE_HOLD			= { 0xF4, 0x30, 0x32, 0x30, 0x32 };
		private static readonly byte[] KEYCODE_REL			= { 0xF4, 0x30, 0x33, 0x30, 0x33 };
		private static readonly byte[] KEYCODE_PEAK			= { 0xF4, 0x30, 0x34, 0x30, 0x34 };
		private static readonly byte[] KEYCODE_MODE			= { 0xF4, 0x30, 0x35, 0x30, 0x35 };
		private static readonly byte[] KEYCODE_MINMAX		= { 0xF4, 0x30, 0x36, 0x30, 0x36 };
		private static readonly byte[] KEYCODE_MEM			= { 0xF4, 0x30, 0x37, 0x30, 0x37 };
		private static readonly byte[] KEYCODE_SETUP		= { 0xF4, 0x30, 0x38, 0x30, 0x38 };
		private static readonly byte[] KEYCODE_LONG_RANGE	= { 0xF4, 0x38, 0x31, 0x38, 0x31 };
		private static readonly byte[] KEYCODE_LONG_HOLD	= { 0xF4, 0x38, 0x32, 0x38, 0x32 };
		private static readonly byte[] KEYCODE_LONG_REL		= { 0xF4, 0x38, 0x33, 0x38, 0x33 };
		private static readonly byte[] KEYCODE_LONG_PEAK	= { 0xF4, 0x38, 0x34, 0x38, 0x34 };
		private static readonly byte[] KEYCODE_LONG_MODE	= { 0xF4, 0x38, 0x35, 0x38, 0x35 };
		private static readonly byte[] KEYCODE_LONG_MINMAX	= { 0xF4, 0x38, 0x36, 0x38, 0x36 };
		private static readonly byte[] KEYCODE_LONG_MEM		= { 0xF4, 0x38, 0x37, 0x38, 0x37 };
		private static readonly byte[] KEYCODE_LONG_SETUP	= { 0xF4, 0x38, 0x38, 0x38, 0x38 };
		
		public enum Keycode
		{
			RANGE,
			HOLD,
			REL,
			PEAK,
			MODE,
			MINMAX,
			MEM,
			SETUP,
			LONG_RANGE,
			LONG_HOLD,
			LONG_REL,
			LONG_PEAK,
			LONG_MINMAX,
			LONG_MEM,
			LONG_SETUP
		}
		public static byte[] GetKeycode(Keycode Input)
		{
			switch (Input)
			{
				case Keycode.RANGE:
					return KEYCODE_RANGE;
				case Keycode.HOLD:
					return KEYCODE_HOLD;
				case Keycode.REL:
					return KEYCODE_REL;
				case Keycode.PEAK:
					return KEYCODE_PEAK;
				case Keycode.MODE:
					return KEYCODE_MODE;
				case Keycode.MINMAX:
					return KEYCODE_MINMAX;
				case Keycode.MEM:
					return KEYCODE_MEM;
				case Keycode.SETUP:
					return KEYCODE_SETUP;
				case Keycode.LONG_RANGE:
					return KEYCODE_LONG_RANGE;
				case Keycode.LONG_HOLD:
					return KEYCODE_LONG_HOLD;
				case Keycode.LONG_REL:
					return KEYCODE_LONG_REL;
				case Keycode.LONG_PEAK:
					return KEYCODE_LONG_PEAK;
				case Keycode.LONG_MINMAX:
					return KEYCODE_LONG_MINMAX;
				case Keycode.LONG_MEM:
					return KEYCODE_LONG_MEM;
				case Keycode.LONG_SETUP:
					return KEYCODE_LONG_SETUP;
			}
			return null;
		}
	}
}

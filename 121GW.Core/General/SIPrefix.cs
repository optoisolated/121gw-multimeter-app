using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace App_121GW
{
	public static class SIPrefix
	{
		private static List<Tuple<double, string>> Units = new List<Tuple<double, string>>()
		{
			new Tuple<double, string>( 1e-12 ,"p"), //0
			new Tuple<double, string>( 1e-9  ,"n"), //1
			new Tuple<double, string>( 1e-6  ,"u"), //2
			new Tuple<double, string>( 1e-3  ,"m"), //3
			new Tuple<double, string>( 1	 ," "), //4
			new Tuple<double, string>( 1e3   ,"k"), //5
			new Tuple<double, string>( 1e6   ,"M"), //6
			new Tuple<double, string>( 1e9   ,"G"), //7
			new Tuple<double, string>( 1e12  ,"T")  //8
		};

		public static string SignificantFigure(double Value, int Figures)
		{
			int sign = (Value > 0) ? 1: -1;
			Value = Math.Abs(Value);

			int int_value = (int)Value;
			int mag_length = int_value.ToString().Length;
			Figures -= mag_length;

			int decimals = (int)((Value - int_value) * Math.Pow(10, Figures));
			if (int_value == 0 && decimals == 0)
				return "0.0";
			return (sign * int_value).ToString() + "." + decimals.ToString().PadLeft(Figures, '0');
		}

		public static string ToString(double Value)
		{
			if (Value == 0.0)
				return "0.0";

			foreach (var unit in Units)
			{
				var range = unit.Item1 * 1000;
				if (range > Math.Abs(Value))
				{
					var label = unit.Item2;
					var outval = (Value * 1000) / range;

					var str = SignificantFigure(outval, 4);
					if (str != "0.0")
						return str + label;
					return str;
				}
			}
			return "0.0";
		}
	}

    public class SIValue
    {
		private char[]  UnitLookup	=	{ 'p','n','u','m',' ','k','M','G','T' };
		
		public double	Value			{ get; }
		public int		SigFigures		{ get; }
		public double	Power		=>	Math.Floor(Math.Log10(Math.Abs(Value)));
		public double	Multiply	=>	Math.Pow(10, -SI_Power);

		public double	SI_Power	=>	Math.Floor(Power / 3.0) * 3.0;
		public int		SI_Index	=>	( int )(SI_Power / 3) + (UnitLookup.Length / 2);
		public char		SI_Unit		=>	UnitLookup[ SI_Index ];
		public double	SI_Value	=>	Value * Multiply;

		public int		SI_Ceiling	=> (int)Math.Ceiling(SI_Value);
		public int		SI_Floor	=> (int)Math.Floor(SI_Value);

		public double	SI_ValueSF
		{
			get
			{
				if ( Value == 0 )
					return 0;
				else
				{
					var si_value = SI_Value;
					if (si_value == 0 )
						return 0;
					else
					{
						var scale = Math.Pow( 10, Math.Floor( Math.Log10( Math.Abs(si_value) ) ) + 1 );
						return ( scale * Math.Round(si_value / scale, SigFigures ) );
					}
				}
			}
		}
		
		public SIValue(float pValue, uint pSignificantFigures = 3)
		{
			Value = pValue;
			SigFigures = (int)pSignificantFigures;
		}
		public override string ToString()
        {
            if (Value == 0) return "0";
            return SI_ValueSF.ToString() + " " + SI_Unit;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace App_121GW
{
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
		
		public SIValue(double pValue, uint pSignificantFigures = 3)
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

	public static class SIPrefix
	{
		public static string ToString(double Value, uint pSignificantFigures = 3) => new SIValue(Value, pSignificantFigures).ToString();
	}
}
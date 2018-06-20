using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;

namespace App_121GW
{
	public abstract class ASmartData : ASmartElement
	{
		public SmartChart		Parent  = null;
		public ASmartAxisPair   Axis	= null;

		protected SKPaint DataPaint = MakeDefaultPaint(Globals.TextColor, 1, Globals.MajorFontSize, Globals.Typeface, IsStroke:true);

		public IObservableList<SKPoint> Points;

		public void Reset()
		{
            Axis.Reset();
            Points.Clear();
		}

		private List<SKPoint> PointsList => Points.ToList();
		private SKPoint[] PointsArray => Points.ToList().ToArray();

		protected (SKPath, SKRect) Path
		{
			get
			{
				var path = new SKPath();
				path.AddPoly(PointsArray, false);
				var bounds = path.Bounds;
				return (path, bounds);
			}
		}

		public string GetCSV()
		{
            //This prevents list from being updated during a copy
            List<SKPoint> points; 
            lock (PointsList)
			    points = new List<SKPoint>(PointsList);

			if (points.Count > 1)
			{
				//The fallback values of axis labels are X, Y
				string horozontal_label = "Time (s)";
				string vertical_label = Parent.Title;

				//The header row of the CSV
				string output = horozontal_label + ", " + vertical_label + "\r\n";

				//Print the rows of the CSV to the string.
				foreach (var item in points) output += item.X.ToString() + ", " + item.Y.ToString() + "\r\n";

				//Return output ;) troll comment
				return output;
			}
			return "";
		}

		public abstract void Draw(SKCanvas Canvas, SKSize dimension, SKSize view);
		
		public ASmartData(ASmartAxisPair pAxis)
		{
			Axis = pAxis;

			pAxis.Horizontal.Range.Set(0f, 0.1f);
			pAxis.Vertical.Range.Set(-0.1f, 0.1f);
		}
	}
	public class SmartData : ASmartData
	{
		public override void Draw(SKCanvas Canvas, SKSize dimension, SKSize view)
		{
			if (Points.Count == 0)
				return;

			(var path, var bounds) = Path;
			path.Transform  (Axis.Transform(dimension));    //Transform the path to fit limits
															//This only draws the path in the render region (between axis)
			Canvas.Save();
			Canvas.ClipRect(Axis.AxisClip(dimension));
			Canvas.DrawPath(path, DataPaint);               //Render scaled and shifted path
			Canvas.Restore();
			Axis.Set		(bounds);					    //Set the axis limits
			Axis.Draw	    (Canvas, dimension, view);		//Render the axis with limits
		}
		public SmartData(ASmartAxisPair pAxis, IObservableList<SKPoint> pData) : base(pAxis)
		{
			DataPaint.IsStroke  = true;
            DataPaint.StrokeWidth = 3;

            DataPaint.Color = Globals.UniqueColor().ToSKColor();
			Axis.Parent	    = this;
			Points          = pData;
			Points.CollectionChanged += (s, e) => Parent.InvalidateSurface();
		}
	}
}
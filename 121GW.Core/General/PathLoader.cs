using System.IO;
using SkiaSharp;
using SkiaSharp.Extended;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace App_121GW
{
	class PathLoader : ResourceLoader
	{
		public delegate bool ProcessImage(string Name, Polycurve Image);
		private ProcessImage mImageFunction;

		public static SKMatrix BuildTransformMatrix(string Transform)
		{
			var Output = SKMatrix.MakeIdentity();

			char[] delim = { ',', ' ', '(', ')' };
			var v_s = Transform.Split(delim, StringSplitOptions.RemoveEmptyEntries);

			if (v_s.Length == 0)
				throw (new Exception("Malformed transform."));

			//Get the name of the transform type
			int i = 0;
			var Type = v_s[i++].ToLower();

			//Switch to correct transform type
			switch (Type)
			{
				case "matrix":
					var a = float.Parse(v_s[i++]);
					var b = float.Parse(v_s[i++]);
					var c = float.Parse(v_s[i++]);
					var d = float.Parse(v_s[i++]);
					var e = float.Parse(v_s[i++]);
					var f = float.Parse(v_s[i++]);


					Output.ScaleX = a;
					Output.ScaleY = d;
					Output.SkewX = c;
					Output.SkewY = b;
					Output.TransX = e;
					Output.TransY = f;

					break;
				case "translate":
					var tx = float.Parse(v_s[i++]);

					var ty = 0.0f;
					if (i < v_s.Length)
						ty = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeTranslation(tx, ty);
					break;
				case "scale":
					var sx = float.Parse(v_s[i++]);

					var sy = 0.0f;
					if (i < v_s.Length)
						sy = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeScale(sx, sy);
					break;
				case "rotate":
					//
					var angle = float.Parse(v_s[i++]);

					//
					var cx = 0.0f;
					if (i < v_s.Length)
						cx = float.Parse(v_s[i++]);

					//
					var cy = 0.0f;
					if (i < v_s.Length)
						cy = float.Parse(v_s[i++]);

					//
					Output = SKMatrix.MakeRotationDegrees(angle, cx, cy);
					break;
				case "skewX":
					var sk_x_angle = float.Parse(v_s[i++]);
					var anglx_radians = ((float)Math.PI / 180.0f) * sk_x_angle;
					Output = SKMatrix.MakeSkew((float)Math.Tan(anglx_radians), 0);
					break;
				case "skewY":
					var sk_y_angle = float.Parse(v_s[i++]);
					var angly_radians = ((float)Math.PI / 180.0f) * sk_y_angle;
					Output = SKMatrix.MakeSkew(0, (float)Math.Tan(angly_radians));
					break;
			};

			// SVG always have these settings
			Output.Persp0 = 0;
			Output.Persp1 = 0;
			Output.Persp2 = 1;
			return Output;
		}

		public List<Polycurve> Curves;
		Polycurve LastCurve
		{
			get
			{
				if (Curves == null)
					return null;
				if (Curves.Count == 0)
					return null;

				return Curves[Curves.Count - 1];
			}
		}
		SKMatrix _CTM = new SKMatrix();
		SKMatrix _LocalTransform = SKMatrix.MakeIdentity();
		SKMatrix _GlobalTransform = SKMatrix.MakeIdentity();

		void UpdateCTM()
		{
			_CTM = SKMatrix.MakeIdentity();
			SKMatrix.Concat(ref _CTM, _GlobalTransform, _LocalTransform);
		}
		SKMatrix LocalTransform
		{
			set
			{
				_LocalTransform = value;
				UpdateCTM();
			}
			get
			{
				return _LocalTransform;
			}
		}
		SKMatrix GlobalTransform
		{
			set
			{
				_GlobalTransform = value;
				UpdateCTM();
			}
			get
			{
				return _GlobalTransform;
			}
		}
		bool ParsePath(string Name, string Path, SKMatrix Transform)
		{
			var path = SKPath.ParseSvgPathData(Path);
			path.Transform(Transform);
			LastCurve.AddPath(path);
			return true;
		}
		bool ProcessSVG(string Name, Stream InputStream)
		{
            MemoryStream ms = new MemoryStream();
			InputStream.CopyTo(ms);
			byte[] data = ms.ToArray();

			MemoryStream ms1 = new MemoryStream(data);
			MemoryStream ms2 = new MemoryStream(data);

			var xdoc = new XDocument();
			xdoc = XDocument.Load(ms1);
			var temp = xdoc.Descendants();
			foreach (var t in temp)
			{
				switch (t.Name.LocalName.ToString())
				{
					case "g":
						//Get and apply global transform
						var gtransform = t.Attribute("transform");
						if (gtransform != null)
							GlobalTransform = BuildTransformMatrix(gtransform.Value);
						else
							GlobalTransform = SKMatrix.MakeIdentity();

						Curves.Add(new Polycurve(Name));
						break;
					case "path":
						//Get and apply local transformation
						var ptransform = t.Attribute("transform");
						if (ptransform != null)
							LocalTransform = BuildTransformMatrix(ptransform.Value);
						else
							LocalTransform = SKMatrix.MakeIdentity();

						var attribute = t.Attribute("d");
						if (attribute != null)
							ParsePath(Name, attribute.Value, _CTM);

						break;
					default:
						break;
				}
			}

            //Use SKSVG to get viewbox
            SkiaSharp.Extended.Svg.SKSvg Imag = new SkiaSharp.Extended.Svg.SKSvg();
			Imag.Load(ms2);

			LastCurve.CanvasSize = Imag.ViewBox.Size;
			LastCurve.Update();

            mImageFunction(Name, LastCurve);

			ms1.Dispose();
			ms2.Dispose();
			ms1 = null;
			ms2 = null;
			Imag = null;

			return true;
		}

		public PathLoader(ProcessImage pLoaderFunction)
		{
			mImageFunction = pLoaderFunction;

			//Convert commands to a path
			Curves = new List<Polycurve>();

			//Immediately triggers loading of all SVG files through the Process SVG function above
			var loader = new GeneralLoader(ProcessSVG, "svg");
		}
	}
}

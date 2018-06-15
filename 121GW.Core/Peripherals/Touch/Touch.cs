using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.ComponentModel;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Platform;
using System.Diagnostics;

using System.Collections;
using System.Threading;

namespace App_121GW
{
	public class		TouchPoint
	{
		public enum eTouchType
		{
			eMoved,
			eHover,
			ePressed,
			eReleased,
            eScroll
		};

		public Point			Position;
		public eTouchType	   TouchType
		{
			set;
			get;
		}

		public TouchPoint(Point pInput, eTouchType pTouchType)
		{
			Position	= pInput;
			TouchType   = pTouchType;
		}
	}
	public static class TouchPointFactory
	{
		public static TouchPoint Moved(Point pInput)
		{
			return new TouchPoint(pInput, TouchPoint.eTouchType.eMoved);
		}
		public static TouchPoint Hover(Point pInput)
		{
			return new TouchPoint(pInput, TouchPoint.eTouchType.eHover);
		}
		public static TouchPoint Pressed(Point pInput)
		{
			return new TouchPoint(pInput, TouchPoint.eTouchType.ePressed);
		}
		public static TouchPoint Released(Point pInput)
		{
			return new TouchPoint(pInput, TouchPoint.eTouchType.eReleased);
		}
        public static TouchPoint Scroll(Point pInput)
        {
            return new TouchPoint(pInput, TouchPoint.eTouchType.eScroll);
        }
    }
	public class		TouchActionEventArgs : EventArgs
	{
        public int ScrollDelta { get; private set; }
		public TouchActionEventArgs(TouchPoint pLocation, uint pID, int delta = 0)
		{
            ScrollDelta = delta;
            Location = pLocation;
			ID = pID;
		}
		public uint ID
		{
			private set; get;
		}
		public TouchPoint Location
		{
			private set; get;
		}
	}

	public class TouchPinch
	{
		public Point PointA { get; private set; }
		public Point PointB { get; private set; }

		private float  _XDistance;
		public float   XDistance
		{
			get	=> _XDistance;
			private set
			{
				var dist = Math.Abs(value);
				XDistanceDelta = dist - _XDistance;
				_XDistance = dist;
				ZoomX = 1 + XDistanceDelta / _XDistance;

				if (float.IsInfinity(ZoomX)) ZoomX = 1;
				if (ZoomX < 0) ZoomX = 1;
			}
		}
		private float  _YDistance;
		public float   YDistance
		{
			get	=> _YDistance;
			private set
			{
				var dist = Math.Abs(value);
				YDistanceDelta = dist - _YDistance;
				_YDistance = dist;
				ZoomY = 1 + YDistanceDelta / _YDistance;

				if (float.IsInfinity(ZoomY)) ZoomY = 1;
				if (ZoomY < 0) ZoomY = 1;
			}
		}
        public float Distance => (float)Math.Sqrt(XDistance * XDistance + YDistance * YDistance);
        public float Angle => (float)Math.Atan(YDistance / XDistance);
		public Point Center => new Point((PointA.X + PointB.X) / 2, (PointA.Y + PointB.Y) / 2);
		public float ZoomX { get; private set; }
		public float ZoomY { get; private set; }
		public bool	 Threshold =>  (XDistanceDelta != 0 || YDistanceDelta != 0);

		private bool	Ready;
		public float   XDistanceDelta	{ get; private set; }
		public float   YDistanceDelta	{ get; private set; }
		public float   DistanceDelta => ( float )Math.Sqrt(XDistanceDelta * XDistanceDelta + YDistanceDelta * YDistanceDelta);
		public bool Set(Point A, Point B)
		{
			////////////////////////
			PointA = A;
			PointB = B;
			////////////////////////
			XDistance = (float)Math.Abs(B.X - A.X);
			YDistance = (float)Math.Abs(B.Y - A.Y);
			////////////////////////
			if (Ready)  return true;
			else		Ready = true;
			////////////////////////
			return false;
			////////////////////////
		}
		public void Clear() => Ready = false;
		public TouchPinch () => Ready = false;
	}
	public class TouchPinchActionEventArgs : EventArgs
	{
		public TouchPinchActionEventArgs(TouchPinch pPinch)
		{
			Pinch = pPinch;
		}
		public TouchPinch Pinch
		{
			private set; get;
		}
	}
	public class TouchPanActionEventArgs : EventArgs
	{
		public float Dx, Dy;
        public float Distance => (float)Math.Sqrt(Dx * Dx + Dy * Dy);


        public TouchPanActionEventArgs(float dx, float dy)
		{
			Dx = dx;
			Dy = dy;
		}
		public TouchPanActionEventArgs()
		{}
	}
	public class TouchCursor
	{
		private TouchPoint Point;
		
		public Point Position
		{
			get
			{
				return Point.Position;
			}
			set
			{
				var NewVal = value;
				var OldVal = Point.Position;
				var DeltaX = NewVal.X - OldVal.X;
				var DeltaY = NewVal.Y - OldVal.Y;
				_Delta = (float)Math.Sqrt(DeltaX * DeltaX + DeltaY * DeltaY);
				Point.Position = value;
			}
		}
		public TouchPoint.eTouchType TouchType
		{
			get
			{
				return Point.TouchType;
			}
			set
			{
				Point.TouchType = value;
			}
		}
		private float _Delta;
		public float Delta
		{
			get
			{
				return _Delta;   
			}
			set
			{
				_Delta = value;
			}
		}
		
		public TouchCursor (TouchPoint pPoint)
		{
			Point = pPoint;
		}
	}

    public class ScrollActionEventArgs : EventArgs
    {
        private int     _Steps;
        public  int     Steps => _Steps;
        private Point   _About;
        public  Point   About => _About;

        public ScrollActionEventArgs(Point pAbout, int Steps)
        {
            _About = pAbout;
            _Steps = Steps;
        }
    }

    public class TouchDoubleTapEventArgs : EventArgs
	{

	}
	public class TouchTapEventArgs : EventArgs
	{

	}

	public class Touch : RoutingEffect
	{
		public delegate void	TouchActionEventHandler		    (object sender, TouchActionEventArgs		args);
		public delegate void	TouchPinchActionEventHandler	(object sender, TouchPinchActionEventArgs   args);
		public delegate void	TouchPanActionEventHandler	    (object sender, TouchPanActionEventArgs	    args);
        public delegate void    ScrollActionEventHandler        (object sender, ScrollActionEventArgs       args);
        public delegate void	TouchDoubleTapActionEventHandler(object sender, TouchDoubleTapEventArgs	    args);
		public delegate void	TouchTapEventArgs			    (object sender, TouchTapEventArgs		    args);

		public event TouchActionEventHandler		Pressed;
		public event TouchActionEventHandler		Released;
		public event TouchActionEventHandler		Hover;
		public event TouchActionEventHandler		PressedMoved;

        public event ScrollActionEventHandler		Scroll;
		public event TouchPanActionEventHandler		Pan;
		public event TouchPinchActionEventHandler	Pinch;

		private float									GestureThreshold;
		CancellableTimer								TapTimer;
		private int									    TapTimeout;
		private int									    TapCount;
		public event TouchTapEventArgs				    Tap;
		public event TouchDoubleTapActionEventHandler   DoubleTap;
		private void TapTimer_Expired()
		{
			if (Mode == TouchMode.Normal)
			{
				if (TapCount == 1)
					Tap?.Invoke(this, null);
				else if (TapCount >= 2)
					DoubleTap?.Invoke(this, null);

				TapCount = 0;
			}
		}
		private void TapProcess()
		{
			TapTimer.Start();

			//Increment tap regardless
			TapCount++;
		}
		private void TapCancel()
		{
			TapTimer.Cancel();
			TapCount = 0;
		}

		public bool Capture { set; get; }

		Dictionary<uint, TouchCursor> Cursors;
		TouchPinch PinchProcessor = new TouchPinch();

		//Adds a cursor to the map if it doesn't already exist
		enum TouchMode
		{
			Normal,
			Pinch,
			Pan
		}
		TouchMode Mode = TouchMode.Normal;
		void ProcessCursor  ( TouchActionEventArgs args )
		{
			if ( !Cursors.ContainsKey( args.ID ) )
			{
				Cursors.Add(args.ID, new TouchCursor( args.Location ));
				if (Cursors.Count >= 2)
					Mode = TouchMode.Pinch;
			}
			else
			{
				var item1 = Cursors[ args.ID ];
				TouchCursor item2 = item1;
				foreach ( var cursor in Cursors )
					if ( cursor.Key != args.ID )
						item2 = cursor.Value;

				var dx = (float)(args.Location.Position.X - item1.Position.X);
				var dy = (float)(args.Location.Position.Y - item1.Position.Y);

				item1.Position = args.Location.Position;
				switch ( Cursors.Count )
				{
					case 0:
						Mode = TouchMode.Normal;
						break;
					case 1:
						{
							var temp = new TouchPanActionEventArgs(dx, dy);
							var dist = temp.Distance;
							if (dist > 0)
							{
								if (Mode == TouchMode.Pan || temp.Distance > GestureThreshold)
								{
									TapCancel();
									Debug.WriteLine("Pan Enabled");
									Mode = TouchMode.Pan;
									Pan?.Invoke(this, temp);
								}
							}
						}   break;
					case 2:
						{
							var temp = new TouchPinchActionEventArgs(PinchProcessor);
							if (temp.Pinch.DistanceDelta >= 0)
							{
								Mode = TouchMode.Pinch;
								if (PinchProcessor.Set(item1.Position, item2.Position))
								{
									TapCancel();
									Pinch?.Invoke(this, temp);
								}
							}
						}   break;
					default:
						Debug.WriteLine("To many cursors.");
						break;
				}
			}
		}
		void RemoveCursor   (TouchActionEventArgs args)
		{
			Cursors.Remove(args.ID);
			if (Mode == TouchMode.Pinch)
			{
				PinchProcessor.Clear();

				if (Cursors.Count == 1)
					Mode = TouchMode.Pan;
			}
			if (Cursors.Count == 0)
				Mode = TouchMode.Normal;
		}

		//Triggers a threadsafe event
		private void SafeEvent(TouchActionEventHandler EventFunction, Element element, TouchActionEventArgs args)
		{
			if (EventFunction != null)
				Device.BeginInvokeOnMainThread(() => {
					EventFunction?.Invoke(element, args);
				});
		}
		private void PlainEvent(TouchActionEventHandler EventFunction, Element element, TouchActionEventArgs args)
		{
			if (Mode == TouchMode.Normal)
				SafeEvent(EventFunction, element, args);
		}


		//Calls the safe event function for the event
		private void SafeHover	  (Element element, TouchActionEventArgs args)
		{
			PlainEvent(Hover, element, args);
		}
        private void SafePressed	(Element element, TouchActionEventArgs args)
		{
			//Start the tap timer
			TapProcess();
			PlainEvent(Pressed, element, args);
		}
		private void SafeReleased   (Element element, TouchActionEventArgs args)
		{
			//Stop the tap timer
			RemoveCursor(args);
			PlainEvent(Released, element, args);
		}
		private void SafePressedMove(Element element, TouchActionEventArgs args)
		{
			ProcessCursor(args);
			SafeEvent(PressedMoved, element, args);
		}

		//Processes platform agnostic input
		private TouchPoint.eTouchType PreviousType;
		public void OnTouchAction(Element e, TouchActionEventArgs a)
		{
			var type = a.Location.TouchType;
			var change = type != PreviousType;
			switch (type)
			{
				case TouchPoint.eTouchType.eMoved:
					switch (PreviousType)
					{
						case TouchPoint.eTouchType.eReleased:
							SafeHover(e, a);
							break;
						case TouchPoint.eTouchType.ePressed:
							SafePressedMove(e, a);
							break;
						default:
							throw (new Exception("Cannot determine previous state. It must be pressed or released"));
					}
					break;
				case TouchPoint.eTouchType.ePressed:
					if (change)
						SafePressed(e, a);
					break;
				case TouchPoint.eTouchType.eReleased:
					SafeReleased(e, a);
					break;
				default:
					throw (new Exception("Do not issue commands other than pressed, released and move from a device."));
			}

			//Moved is not a state change
			if (type != TouchPoint.eTouchType.eMoved)
				PreviousType = type;
		}

		//Common handlers
		public void RaiseAction(TouchActionEventArgs Action)
		{
			OnTouchAction(Element, Action);
        }
        public void ScrollHandler(object sender, Point pt, int delta, uint ID)
        {
            Device.BeginInvokeOnMainThread(() => {
                Scroll?.Invoke(sender, new ScrollActionEventArgs(TouchPointFactory.Released(pt).Position, delta));
            });
        }
        public void ReleasedHandler(object sender, Point pt, uint ID)
		{
			RaiseAction(new TouchActionEventArgs(TouchPointFactory.Released(pt), ID));
		}
		public void MoveHandler(object sender, Point pt, uint ID)
		{
			RaiseAction(new TouchActionEventArgs(TouchPointFactory.Moved(pt), ID));
		}
		public void PressedHandler(object sender, Point pt, uint ID)
		{
			RaiseAction(new TouchActionEventArgs(TouchPointFactory.Pressed(pt), ID));
		}

		//Initialise class and base
		public Touch(int pTapTimeout = 500, float pThreshold = 4) : base("App_121GW.Touch")
		{
			GestureThreshold = pThreshold;
			TapTimeout = pTapTimeout;

			Mode = TouchMode.Normal;
			TapTimer = new CancellableTimer(TimeSpan.FromMilliseconds(TapTimeout), TapTimer_Expired);

			PreviousType = TouchPoint.eTouchType.eReleased;
			Cursors = new Dictionary<uint, TouchCursor>();
		}
	}
}

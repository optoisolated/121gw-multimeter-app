using System;
using Xamarin.Forms;
using System.Threading;

namespace App_121GW
{
	class Loading : GeneralView
	{
		private static readonly TimeSpan Now	= new TimeSpan(0, 0, 0, 0, 0);
		private static readonly TimeSpan Period	= new TimeSpan(0, 0, 0, 0, 250);

		private Timer			mTimer			= null;
		const	string			mDotsString		= ".....";
		private int				mDots			= 0;
		private readonly string	mText			= "";
		private GeneralLabel	mLoadingText	= new GeneralLabel();

		private void Update()
		{
			Globals.RunMainThread(() =>
			{
				var dot_string = mDotsString.Substring(mDotsString.Length - mDots);
				mLoadingText.Text = mText + dot_string;
				mDots++; if (mDots >= mDotsString.Length) mDots = 0;
			});
		}

		private Timer	MakeTimer	=> new Timer((obj) => { Update(); }, null, Now, Period);
		public	void	Run()		=> mTimer = MakeTimer;
		public	void	Stop()		=> mTimer = null;

		public Loading(string pText)
		{
			Content = mLoadingText;
			mText = pText;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace rMultiplatform
{
	public class PacketProcessor
	{
		public delegate void ProcessPacket(byte[] pPacket);
		public  event ProcessPacket mCallback;
		private bool mStartFound;
		private byte mStart;
		private int mLength;
		private List<byte> mBuffer;

		public PacketProcessor(byte start, int length)
		{ 
			mBuffer	 = new List<byte>();
			mStart	  = start;
			mLength	 = length;
			mStartFound = false;
		}
		public void Reset()
		{
			mBuffer.Clear();
			mStartFound = false;
		}
		public void Recieve( byte[] pBytes )
		{
            foreach ( var byt in pBytes )
			{
                var character = (char) byt;
                if ( character == mStart )
                {
                    if ( mStartFound )
                    {
                        if ( mBuffer.Count >= mLength )
                        {
                            mCallback?.Invoke( mBuffer.ToArray() );
                            
                            Debug.WriteLine(BitConverter.ToString(mBuffer.ToArray()));

                            mBuffer.Clear( );
                        }
                    }
                    else
                        Debug.WriteLine("FAILED : " + BitConverter.ToString(mBuffer.ToArray()));

                    mStartFound = true;
                    mBuffer.Clear( );
                    mBuffer.Add( byt );

                } else if ( mStartFound )
                    mBuffer.Add( byt );
            }
		}
	}
}

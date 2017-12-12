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
		public void Recieve(byte[] pBytes)
		{
            var debug_readable = Encoding.UTF8.GetString(pBytes);

            Debug.WriteLine("Recieved : " + debug_readable);

            foreach (var byt in pBytes)
			{
                bool IsLetterOrDigitFix(char input)
                {
                    bool lower = 'a' <= input && input <= 'z';
                    bool upper = 'A' <= input && input <= 'Z';
                    bool number = '0' <= input && input <= '9';
                    return lower || upper || number;
                }

                //Add byte
                if (mStartFound)
                {
                    //Currently replacing Char.IsLetterOrNumber which let through some unicode characters
                    if (IsLetterOrDigitFix((char)byt))
                    {
                        mBuffer.Add(byt);
                        if (mBuffer.Count >= mLength)
                        {
                            mCallback?.Invoke(mBuffer.ToArray());
                            mBuffer.Clear();
                            mStartFound = false;
                        }
                    }
                }
                if (byt == mStart)
                {
                    mStartFound = true;
                    mBuffer.Clear();
                }
			}
		}
	}
}

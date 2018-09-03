using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace App_121GW
{
	public class PacketProcessor
	{
		public delegate void ProcessPacket(byte[] pPacket);
		public event ProcessPacket mCallback;

		private readonly byte		mStart;
		private readonly int		mLength;

		private List<byte>			mBuffer = new List<byte>();

		public PacketProcessor(byte start, int length)
		{
			mStart = start;
			mLength = length;
		}
		public void Reset()
        {
            lock (mBuffer)
            {
                mBuffer.Clear();
            }
		}
		public void Recieve( byte[] pBytes )
		{
            lock(mBuffer)
            {
                Debug.WriteLine("Recieve");
                Debug.WriteLine(pBytes.Length.ToString());

                mBuffer.AddRange(pBytes);
                var start_index = mBuffer.FindIndex((item) => item == mStart);

                //Start was found remove useless items infront of it
                if (start_index > 0) mBuffer.RemoveRange(0, start_index);

                //Check that there is sufficient length for a packet
                if (mBuffer.Count >= mLength)
                {
                    try
                    {
                        var array = mBuffer.GetRange(0, mLength).ToArray();
                        mCallback?.Invoke(array);
                        mBuffer.RemoveRange(0, mLength);
                    }
                    catch { }
                }
            }
		}
	}
}

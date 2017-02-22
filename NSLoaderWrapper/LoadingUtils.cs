using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace NullSpace.SDK
{
	internal static class LoadingUtils
	{
		

		internal static byte[] LoadAsset(string path)
		{
			try
			{
				byte[] bytes = File.ReadAllBytes(path);
				return bytes;
				
			} catch (IOException e)
			{
				throw new HapticsLoadingException("Couldn't read the haptics asset at " + path, e);
			}
		}


	}
}

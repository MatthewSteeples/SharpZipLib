using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;

namespace ICSharpCode.SharpZipLib.Tests.Zip
{
	[TestFixture]
	internal class Zip64Handling : ZipBase
	{
		[Test]
		[Category("Zip")]
		public void CanExtractMicrosoftZip64Files()
		{
			var tempFile = Path.GetTempFileName();
			using (var stream = File.OpenWrite(tempFile))
			{
				var fourK = new byte[1024];
				var rnd = new Random();
				rnd.NextBytes(fourK);

				using (var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create))
				{
					var entry1 = zipArchive.CreateEntry("smallfile.dat", CompressionLevel.Optimal);
					using (var entryStream = entry1.Open())
					{
						entryStream.Write(fourK, 0, fourK.Length);
					}

					var entry2 = zipArchive.CreateEntry("largefile.dat", CompressionLevel.Optimal);
					
					using (var entryStream = entry2.Open())
					{
						for (int i = 0; i < 5 * 1024 * 1024; i++)
						{
							entryStream.Write(fourK, 0, fourK.Length);
						}
					}

					var entry3 = zipArchive.CreateEntry("anothersmallfile.dat", CompressionLevel.Optimal);
					using (var entryStream = entry3.Open())
					{
						entryStream.Write(fourK, 0, fourK.Length);
					}
				}
			}

			using (var stream = File.OpenRead(tempFile))
			{
				using (var zIn = new ZipInputStream(stream))
				{
					ZipEntry zipFile;

					while ((zipFile = zIn.GetNextEntry()) != null)
					{
						zIn.ReadByte();
					}
				}
			}
		}
	}
}

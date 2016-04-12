using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using YouRoomSharp;
using YouRoomSharp.Data;

namespace YouRoomConsoleSample
{
    class Program
    {
        private static byte[] ComputeHash(byte[] key, byte[] buffer)
        {
            using (var hmac = new HMACSHA1(key))
            {
                return hmac.ComputeHash(buffer);
            }
        }

        private static Task<string> RetrievePinCode(string uri)
        {
            System.Diagnostics.Process.Start(uri);

            return
                Task.Run<string>(
                    () =>
                    {
                        Console.Write("PIN: ");
                        return Console.ReadLine();
                    });
        }

        private static async Task DemoAsync()
        {
            string consumerKey = ConfigurationManager.AppSettings["ConsumerKey"];
            string consumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"];

            var client = new YouRoomClient(consumerKey, consumerSecret, ComputeHash, RetrievePinCode);

            await client.AuthorizeAsync();

            var homeEntries = await client.GetHomeTimeline();

            var rooms = await client.GetMyGroups();

            Group firstRoom = rooms.First();

            var roomEntries = await client.GetRoomTimeline(firstRoom.ToParam);

            var oneEntry =
                await client.ShowEntryAsync(firstRoom.ToParam, roomEntries.First().Id);

            var entryWithPictureAttachment =
                roomEntries
                    .First(e => e.Attachment?.AttachementType == AttachmentType.Image);

            if (entryWithPictureAttachment == null)
                return;

            using (var stream = await client.ShowAttachmentAsync(entryWithPictureAttachment.Participation.Group.ToParam, entryWithPictureAttachment.Id, true))
            {
                int count = 0;

                while (stream.ReadByte() != -1)
                    count++;

                Console.WriteLine($"Length: {count} bytes");
            }
        }

        static void Main(string[] args)
        {
            DemoAsync().Wait();
            Console.ReadLine();
        }
    }
}

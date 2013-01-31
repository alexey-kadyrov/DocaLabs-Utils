using System;
using System.Drawing;
using System.IO;

namespace DocaLabs.Http.Client.Serialization
{
    /// <summary>
    /// Deserializes an image from the response stream.
    /// </summary>
    public class ImageResponseDeserializer : IResponseDeserializationProvider
    {
        /// <summary>
        /// Deserializes an image from the response stream using intermediate MemoryStream as the Image/Bitmap classes 
        /// require that the stream was kept open for the lifetime of the image object.
        /// </summary>
        public TResult Deserialize<TResult>(HttpResponse response)
        {
            // the Image/Bitmap classes require that the stream was kept open for the lifetime of the image object.
            using (var sourceStream = response.GetDataStream())
            {
                var imageStream = new MemoryStream(response.ContentLength <= 0 ? 8192 : (int)response.ContentLength);

                sourceStream.CopyTo(imageStream);

                if(typeof(TResult) == typeof(Image))
                    return (TResult)((object)(Image.FromStream(imageStream)));

                if (typeof(TResult) == typeof(Bitmap))
                    return (TResult)((object)(new Bitmap(imageStream)));

                throw new UnrecoverableHttpClientException(string.Format(Resources.Text.expected_retsult_to_be_image_or_bitmap_classes, typeof(TResult)));
            }
        }

        /// <summary>
        /// Returns true if the content type is 'image/gif'/'image/jpeg'/'image/tiff'/'image/png' and the TResult is not "simple type", like int, string, Guid, double, etc.
        /// </summary>
        public bool CheckIfSupports<TResult>(HttpResponse response)
        {
            return
                (!string.IsNullOrWhiteSpace(response.ContentType)) &&
                    (
                        string.Compare(response.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(response.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(response.ContentType, "image/tiff", StringComparison.OrdinalIgnoreCase) == 0 ||
                        string.Compare(response.ContentType, "image/png", StringComparison.OrdinalIgnoreCase) == 0
                    ) &&
                    (
                        typeof(TResult) == typeof(Image) ||
                        typeof(TResult) == typeof(Bitmap)
                    );
        }
    }
}

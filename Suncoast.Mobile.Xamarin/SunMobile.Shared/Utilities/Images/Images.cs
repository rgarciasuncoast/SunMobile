using System;
using System.IO;

#if __IOS__
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
#endif

#if __ANDROID__
using Android.Graphics;
using Android.Util;
#endif


namespace SunMobile.Shared.Utilities.Images
{
    public static class Images
    {
		private const float JPEG_COMPRESSION = 0.3f;

		public static int CalculateInSampleSize(int originalWidth, int originalHeight, int maxWidth, int maxHeight)
		{
			double inSampleSize = 1D;

			if (originalHeight > maxHeight || originalWidth > maxWidth)
			{
				int halfHeight = (originalHeight / 2);
				int halfWidth = (originalWidth / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > maxHeight && (halfWidth / inSampleSize) > maxWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}

		public static string ConvertStreamToBase64String(Stream stream)
		{
			string returnValue = string.Empty;

			try
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					var bytes = memoryStream.ToArray();
					returnValue = Convert.ToBase64String(bytes);
					bytes = null;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertStreamToBase64String");
			}

			return returnValue;
		}

		public static byte[] ConvertStreamToByteArray(Stream stream)
		{
			byte[] returnValue = null;

			try
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					returnValue = memoryStream.ToArray();
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertStreamToByteArray");
			}

			return returnValue;
		}


#if __IOS__
        public static byte[] ConvertUIImageToBytes(UIImage image)
        {
            byte[] returnValue = null;

            try
            {
                NSData imageData = image.AsJPEG(JPEG_COMPRESSION);
                var byteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, byteArray, 0, Convert.ToInt32(imageData.Length));
                returnValue = byteArray;
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "Images:ConvertUIImageToBytes");
            }

            return returnValue;
        }

        public static UIImage ConvertViewToImage(UIView view)
        {
            var returnValue = new UIImage();

            try
            {
                UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, true, 0);
                view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
                returnValue = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }
            catch(Exception ex)
            {
                Logging.Logging.Log(ex, "Images:ConvertViewToImage");
            }

            return returnValue;
            
        }

		public static UIImage ScaledImage(UIImage image, nfloat maxWidth, nfloat maxHeight)
		{
			var returnValue = image;

			try
			{
				var maxResizeFactor = Math.Min(maxWidth / image.Size.Width, maxHeight / image.Size.Height);
				var width = maxResizeFactor * image.Size.Width;
				var height = maxResizeFactor * image.Size.Height;
				returnValue = image.Scale(new CGSize(width, height));
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ScaledImage");
			}

			return returnValue;
		}

		public static string ConvertStreamToUIImageToBase64StringWithCompression(Stream stream)
		{
			string returnValue = string.Empty;

			try
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					var bytes = memoryStream.ToArray();
					var image = ConvertByteArrayToUIImage(bytes);
					returnValue = ConvertUIImageToBase64String(image);
					bytes = null;
					image = null;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertStreamToUIImageToBase64StringWithCompression");
			}

			return returnValue;
		}

		public static byte[] CompressImageBytes(byte[] sourceBytes)
		{
			byte[] compressedBytes = null;

			try
			{
				var nsData = NSData.FromArray(sourceBytes);
				var image = UIImage.LoadFromData(nsData);
				NSData imageData = image.AsJPEG(JPEG_COMPRESSION);
				var byteArray = new byte[imageData.Length];
				System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, byteArray, 0, Convert.ToInt32(imageData.Length));
				compressedBytes = byteArray;
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:CompressImageBytes");
			}

			if (compressedBytes == null)
			{
				compressedBytes = sourceBytes;
			}

			return compressedBytes;
		}

		public static UIImage CompressUImage(UIImage image)
		{
			NSData d = image.AsJPEG(JPEG_COMPRESSION);
			return new UIImage(d);
		}

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static UIImage MaxResizeImage(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new CGSize((nfloat)width, (nfloat)height));
			sourceImage.Draw(new CGRect(0, 0, (nfloat)width, (nfloat)height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return resultImage;
		}

		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage(UIImage sourceImage, float width, float height)
		{
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			return resultImage;
		}

		public static UIImage CropImage(UIImage sourceImage, int x, int y, int width, int height)
		{
			var rect = new RectangleF(x, y, width, height);

			return CropImage(sourceImage, rect);
		}

		public static UIImage CropImage(UIImage sourceImage, RectangleF rect)
		{
			var croppedCGImage = sourceImage.CGImage.WithImageInRect(rect);

			return UIImage.FromImage(croppedCGImage);
		}

        public static string ConvertUIImageToBase64String(UIImage image)
        {
			NSData imageData = image.AsJPEG(JPEG_COMPRESSION);
            var byteArray = new byte[imageData.Length];
            System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, byteArray, 0, Convert.ToInt32(imageData.Length));
            var base64String = Convert.ToBase64String(byteArray);

            return base64String;
        }

        public static UIImage ConvertBase64StringToUIImage(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            var imageData = NSData.FromArray(imageBytes);
            var uiImage = UIImage.LoadFromData(imageData);

            return uiImage;
        }

		public static UIImage ConvertByteArrayToUIImage(byte[] imageBytes)
		{
			var imageData = NSData.FromArray(imageBytes);
			var uiImage = UIImage.LoadFromData(imageData);

			return uiImage;
		}

		private static nfloat radians(double degrees)
		{
			return (nfloat)(degrees * Math.PI / 180);
		}

		public static UIImage ScaleAndRotateImage(UIImage sourceImage, UIImageOrientation orientation) 
		{
			int maxResolution = 2048;

			CGImage imgRef = sourceImage.CGImage;
			float width = imgRef.Width;
			float height = imgRef.Height;
			CGAffineTransform transform = CGAffineTransform.MakeIdentity();
			var bounds = new RectangleF( 0, 0, width, height );

			if (width > maxResolution || height > maxResolution)
			{
				float ratio = width/height;

				if (ratio > 1)
				{
					bounds.Width  = maxResolution;
					bounds.Height = bounds.Width / ratio;
				}
				else
				{
					bounds.Height = maxResolution;
					bounds.Width  = bounds.Height * ratio;
				}
			}

			float scaleRatio = bounds.Width / width;
			var imageSize = new SizeF(width, height);
			UIImageOrientation orient = orientation;
			float boundHeight;

			switch(orient)
			{
				case UIImageOrientation.Up:                                        //EXIF = 1
					transform = CGAffineTransform.MakeIdentity();
					break;
				case UIImageOrientation.UpMirrored:                                //EXIF = 2
					transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					break;
				case UIImageOrientation.Down:                                      //EXIF = 3
					transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
					break;
				case UIImageOrientation.DownMirrored:                              //EXIF = 4
					transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
					transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
					break;
				case UIImageOrientation.LeftMirrored:                              //EXIF = 5
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation (imageSize.Height, imageSize.Width);
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI/ 2.0f);
					break;
				case UIImageOrientation.Left:                                      //EXIF = 6
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation (0.0f, imageSize.Width);
					transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
					break;
				case UIImageOrientation.RightMirrored:                             //EXIF = 7
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
					break;
				case UIImageOrientation.Right:                                     //EXIF = 8
					boundHeight = bounds.Height;
					bounds.Height = bounds.Width;
					bounds.Width = boundHeight;
					transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
					transform = CGAffineTransform.Rotate(transform, (float)Math.PI  / 2.0f);
					break;
				default:
					throw new Exception("Invalid image orientation");
			}

			UIGraphics.BeginImageContext(bounds.Size);

			CGContext context = UIGraphics.GetCurrentContext();

			if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
			{
				context.ScaleCTM(-scaleRatio, scaleRatio);
				context.TranslateCTM(-height, 0);
			}
			else
			{
				context.ScaleCTM(scaleRatio, -scaleRatio);
				context.TranslateCTM(0, -height);
			}

			context.ConcatCTM(transform);
			context.DrawImage (new RectangleF (0, 0, width, height), imgRef);

			UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return imageCopy;
		}

		public static UIImage ScaleImage(UIImage sourceImage, int maxWidth, bool forceLandscape)
		{
			nfloat smallSide = sourceImage.Size.Height < sourceImage.Size.Width ? sourceImage.Size.Height : sourceImage.Size.Width;
			nfloat bigSide = sourceImage.Size.Height > sourceImage.Size.Width ? sourceImage.Size.Height : sourceImage.Size.Width;
			nfloat minWidth = smallSide * (maxWidth / bigSide);

			var landscapeSize = new CGSize(maxWidth, minWidth);
			var portraitSize = new CGSize(minWidth, maxWidth);
			CGSize size;

			if (forceLandscape)
			{
				size = landscapeSize;
			}
			else
			{
				// Use the camera orientation to pick the correct aspect
				if (sourceImage.Orientation == UIImageOrientation.Up || sourceImage.Orientation == UIImageOrientation.Down)
				{				
					size = landscapeSize;
				} 
				else
				{
					size = portraitSize;
				}
			}

			// Setup the cg context
			UIGraphics.BeginImageContext(size);

			var context = UIGraphics.GetCurrentContext();

			// Math reminder: angle in radians = angle in degrees * Pi / 180
			if (forceLandscape || sourceImage.Orientation == UIImageOrientation.Up)
			{ 
				// Left landscape
				context.ScaleCTM(1.0f, -1.0f);
				context.TranslateCTM(0.0f, -size.Height);
			}
			else if (sourceImage.Orientation == UIImageOrientation.Right)
			{ 
				// Portrait
				context.RotateCTM(90.0f * (float)Math.PI / 180.0f);
				context.ScaleCTM(1.0f, -1.0f);
			}
			else if (sourceImage.Orientation == UIImageOrientation.Left)
			{   // Upside down
				context.RotateCTM(-90.0f * (float)Math.PI / 180.0f);
				context.ScaleCTM(1.0f, -1.0f);
				context.TranslateCTM(-size.Height , -size.Width);
			}
			else if (sourceImage.Orientation == UIImageOrientation.Down)
			{   // Right landscape
				context.ScaleCTM(-1.0f, 1.0f);
				context.TranslateCTM(-size.Width, 0.0f);
			}

			// draw the image to the context. do note that the always in landscape regardless of the choice
			context.DrawImage(new CGRect(0, 0, landscapeSize.Width, landscapeSize.Height), sourceImage.CGImage);

			// Make the UIImage
			UIImage scaledImage = UIGraphics.GetImageFromCurrentImageContext();

			// and end the context
			UIGraphics.EndImageContext();

			// return the thumbnail image
			return scaledImage;
		}

		public static float ConvertPointsToPixels(float point)
		{
			var returnValue = point * (float)UIScreen.MainScreen.Scale;

			return returnValue;
		}

#endif

#if __ANDROID__
        public static Bitmap ConvertWebViewToBitmap(Android.Webkit.WebView view)
        {
            Bitmap returnValue = null;

            try
            {
                #pragma warning disable CS0618 // Type or member is obsolete
                var picture = view.CapturePicture();
                #pragma warning restore CS0618 // Type or member is obsolete
                var bitmap = Bitmap.CreateBitmap(picture.Width, picture.Height, Bitmap.Config.Argb8888);
                var canvas = new Canvas(bitmap);
                picture.Draw(canvas);

                returnValue = bitmap;
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "Images:ConvertWebViewToBitmap");
            }

            return returnValue;
        }

		public static byte[] CompressImageBytes(byte[] sourceBytes)
		{
			byte[] compressedBytes = null;

			try
			{
				Bitmap bitmap = BitmapFactory.DecodeByteArray(sourceBytes, 0, sourceBytes.Length);

				if (bitmap != null)
				{
					var stream = new MemoryStream();
					bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(JPEG_COMPRESSION * 100), stream);
					compressedBytes = stream.ToArray();
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:CompressImageBytes");
			}

			if (compressedBytes == null)
			{
				compressedBytes = sourceBytes;
			}

			return compressedBytes;
		}

		public static int GetBitmapRotation(Bitmap bitmap)
		{
			int rotation = 0;

			try
			{
				var stream = new MemoryStream();
				bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(JPEG_COMPRESSION * 100), stream);

				var exif = new Android.Media.ExifInterface(stream);

				rotation = exif.GetAttributeInt(Android.Media.ExifInterface.TagOrientation, 0);
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:GetBitmapRotation");
			}

			return rotation;
		}

		public static string ConvertStreamToBitmapToBase64StringWithCompression(Stream stream)
		{
			string returnValue = string.Empty;

			try
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					var bytes = memoryStream.ToArray();
					var image = ConvertByteArrayToBitmap(bytes);
					returnValue = ConvertBitmapToBase64String(image);
					bytes = null;
					image = null;
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertStreamToBitmapToBase64StringWithCompression");
			}

			return returnValue;
		}

		public static Bitmap CropBitmap(Bitmap sourceBitmap, Rect cropArea)
		{
            Bitmap destinationBitmap = null;

            try
            {
                if (cropArea.Width() - cropArea.Left > sourceBitmap.Width)
                {
                    cropArea.Left = 0;
                    cropArea.Right = sourceBitmap.Width;
                }

                if (cropArea.Height() - cropArea.Top > sourceBitmap.Height)
                {
                    cropArea.Top = 0;
                    cropArea.Bottom = sourceBitmap.Height;
                }

                // Double check that our calculations are correct.  It was sometimes crashing without this.
                if ((cropArea.Left + cropArea.Width() <= sourceBitmap.Width) && (cropArea.Top + cropArea.Height() <= sourceBitmap.Height))
                {
                    destinationBitmap = Bitmap.CreateBitmap(sourceBitmap, cropArea.Left, cropArea.Top, cropArea.Width(), cropArea.Height());
                }
                else
                {
                    destinationBitmap = Bitmap.CreateBitmap(sourceBitmap);
                }
            }
            catch(Exception ex)
            {
                destinationBitmap = sourceBitmap;
                Logging.Logging.Log(ex, "Images:CropBitmap");
            }

			return destinationBitmap;
		}

        public static Bitmap ConvertBase64StringToBitmap(string base64String) 
        {
			Bitmap bitmap = null;

			try
			{
				byte[] imageBytes = Convert.FromBase64String(base64String);

				var options = new BitmapFactory.Options();
				options.InPurgeable = true;

				bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length, options);
				imageBytes = null;
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertBase64StringToBitmap");
			}

            return bitmap;
        }

        public static string ConvertBitmapToBase64String(Bitmap bitmap, bool disposeBitmap = true)
        {
			string returnValue = string.Empty;

            try
            {
                if (bitmap != null)
                {
                    var stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(JPEG_COMPRESSION * 100), stream);
                    byte[] bitmapData = stream.ToArray();

					if (disposeBitmap)
					{
						bitmap.Dispose();
					}

                    returnValue = Base64.EncodeToString(bitmapData, Base64Flags.Default);
                }
            }
            catch (Exception ex)
            {
                Logging.Logging.Log(ex, "Images:ConvertBitmapToBase64String");
            }

            return returnValue;
        }

		public static byte[] ConvertBitmapToByteArray(Bitmap bitmap, bool disposeBitmap = true)
		{
			byte[] returnValue = null;

			try
			{
				if (bitmap != null)
				{
					var stream = new MemoryStream();
					bitmap.Compress(Bitmap.CompressFormat.Jpeg, (int)(JPEG_COMPRESSION * 100), stream);
					returnValue = stream.ToArray();

					if (disposeBitmap)
					{
						bitmap.Dispose();
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertBitmapToByteArray");
			}

			return returnValue;
		}

		public static Bitmap ConvertByteArrayToBitmap(byte[] imageBytes)
		{
			Bitmap bitmap = null;

			try 
			{
				bitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
			}
			catch(Exception ex)
			{
				Logging.Logging.Log(ex, "Images:ConvertByteArrayToBitmap");
			}

			return bitmap;
		}

		public static Bitmap RotateBitmap90Degrees(Bitmap bitmap)
		{
			var matrix = new Matrix();
			matrix.PostRotate(90);
			var bitmapRotate = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

			return bitmapRotate;
		}

		public static Bitmap RotateBitmap(Bitmap bitmap, int degrees)
		{
			var matrix = new Matrix();
			matrix.PostRotate(degrees);
			var bitmapRotate = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

			return bitmapRotate;
		}       
#endif
    }
}
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Plugin.CurrentActivity;
using SunMobile.Shared.Utilities.Images;

namespace SunMobile.Droid.Cards
{
	public class OrderRaysCardImageFragment : Fragment
	{		
		public static Fragment NewInstance(Fragment context, byte[] cardImage)
		{
			var bundle = new Bundle();
            bundle.PutByteArray("cardimage", cardImage);								
			var selectCardDesignContentFragment = new OrderRaysCardImageFragment();

			return Fragment.Instantiate(CrossCurrentActivity.Current.Activity, selectCardDesignContentFragment.Class.Name, bundle);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			if (container == null)
			{
				return null;
			}

			var layout = (LinearLayout)inflater.Inflate(Resource.Layout.OrderRaysCardImageView, container, false);

            var cardImage = Arguments.GetByteArray("cardimage");			
            var bitmap = Images.ConvertByteArrayToBitmap(cardImage);
			var imageCard = layout.FindViewById<ImageView>(Resource.Id.imageCard);
            imageCard.SetImageBitmap(bitmap);				

            return layout;
		}
	}
}
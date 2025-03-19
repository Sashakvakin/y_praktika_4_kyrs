using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MegaSklad
{
    public class MultiImageConverterProfile : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string defaultImage = "user.png";
            string bucketName = "photoprofil";
            string imageName = null;

            Guid? photoGuid = values[0] as Guid?;  // Safe casting and assigning

            if (photoGuid.HasValue)
            {
                imageName = photoGuid.Value.ToString() + ".png";
            }

            try
            {
                string fullImageUrl = Task.Run(() => App.SupabaseClient.Storage.From(bucketName).GetPublicUrl(imageName)).Result;
                if (string.IsNullOrEmpty(fullImageUrl))
                {
                    return new BitmapImage(new Uri($"pack://application:,,,/{defaultImage}"));
                }

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fullImageUrl);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                return new BitmapImage(new Uri($"pack://application:,,,/{defaultImage}"));
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}